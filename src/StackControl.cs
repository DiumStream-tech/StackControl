using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Items;
using Il2CppScheduleOne.ItemFramework;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

[assembly: MelonInfo(typeof(StackControl), PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, PluginInfo.PLUGIN_AUTHOR)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: System.Reflection.AssemblyMetadata("NexusModID", "788")]

public class StackControl : MelonMod
{
    private static ItemSlot? cachedSlot = null;
    private static bool rightClickHeld = false;
    private static MelonLogger.Instance? Logger;
    private static bool _sceneLoaded = false;
    private static bool _hasInitialized = false;
    private static int _frameCounter = 0;
    private const int CHECK_INTERVAL = 60;

    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;
        Logger?.Msg("✅ StackControl initialized");
        Config.Initialize();
        HarmonyInstance.PatchAll(typeof(StackControl));
        ApplyStackLimitToAllItems();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "Main")
        {
            Logger?.Msg("Main scene loaded");
            _sceneLoaded = true;
            _hasInitialized = false;
        }
    }

    public override void OnUpdate()
    {
        if (_sceneLoaded && !_hasInitialized)
        {
            _frameCounter++;
            if (_frameCounter >= CHECK_INTERVAL)
            {
                _frameCounter = 0;
                
                if (IsPlayerLoaded())
                {
                    ApplyStackLimitToAllItems();
                    _hasInitialized = true;
                }
            }
        }
    }

    private bool IsPlayerLoaded()
    {
        var player = GameObject.Find("Player_Local");
        return player != null && player.activeInHierarchy;
    }

    public static void ApplyStackLimitToAllItems()
    {
        if (Config.StackLimit == null) return;
        
        int limit = Config.StackLimit.Value;
        var allItems = Resources.FindObjectsOfTypeAll<ItemDefinition>();
        
        foreach (var item in allItems)
        {
            if (item != null && item.StackLimit != limit) 
            {
                item.StackLimit = limit;
                Logger?.Msg($"Mise à jour stack pour {item.name} : {limit}");
            }
        }
        
        ItemDefinitionPatch.GlobalStackLimit = limit;
        Logger?.Msg($"StackLimit global mis à jour : {limit}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemUIManager), "Update")]
    private static void PostfixUpdate(ItemUIManager __instance)
    {
        if (Input.GetMouseButtonDown(1))
        {
            cachedSlot = __instance.HoveredSlot?.assignedSlot;
            rightClickHeld = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rightClickHeld = false;
            cachedSlot = null;
        }

        bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (!ctrlHeld || !rightClickHeld) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f) return;

        int currentAmount = __instance.draggedAmount;
        if (currentAmount <= 0) return;

        if (Config.ScrollStep == null) return;
        int step = Config.ScrollStep.Value;
        
        int maxAmount = 999;
        var instance = cachedSlot?.ItemInstance;
        if (instance != null) maxAmount = Mathf.Max(instance.GetItemData()?.Quantity - 1 ?? 1, 1);

        int newAmount = scroll > 0 
            ? (currentAmount == 1 ? step : Mathf.Min(((currentAmount - 1) / step + 1) * step, maxAmount))
            : (currentAmount <= step ? 0 : Mathf.Max(((currentAmount - 1) / step) * step - step, 1));

        __instance.SetDraggedAmount(newAmount);
        Logger?.Msg($"Ctrl+Scroll: {currentAmount} → {newAmount} (Step: {step})");
    }

    [HarmonyPatch(typeof(ItemDefinition))]
    public static class ItemDefinitionPatch
    {
        public static int GlobalStackLimit = 40;

        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor)]
        private static void Postfix(ItemDefinition __instance)
        {
            __instance.StackLimit = GlobalStackLimit;
        }
    }
}
