using MelonLoader;
using System.IO;
using UnityEngine;

public static class Config
{
    public static MelonPreferences_Category? Category { get; private set; }
    public static MelonPreferences_Entry<int>? ScrollStep { get; private set; }
    public static MelonPreferences_Entry<int>? StackLimit { get; private set; }
    
    private static string _filePath = null!;
    
    public static void Initialize()
    {
        Category = MelonPreferences.CreateCategory("StackControl");
        _filePath = Path.Combine(GetUserDataDirectory(), "StackControl.cfg");
        Category.SetFilePath(_filePath);
        
        ScrollStep = Category.CreateEntry("ScrollStep", 5, 
            description: "Définit l'incrément de scroll (1-20)");
        
        StackLimit = Category.CreateEntry("StackLimit", 40,
            description: "Définit la limite globale de stack pour tous les items (1-999)");

        ScrollStep.OnEntryValueChanged.Subscribe((_, newValue) => 
        {
            int validatedValue = Mathf.Clamp(newValue, 1, 20);
            if(newValue != validatedValue)
            {
                ScrollStep.Value = validatedValue;
                MelonLogger.Warning($"Valeur ajustée à {validatedValue} (doit être entre 1 et 20)");
            }
            MelonPreferences.Save();
            MelonLogger.Msg($"[Config] ScrollStep mis à jour : {validatedValue}");
        });

        StackLimit.OnEntryValueChanged.Subscribe((_, newValue) =>
        {
            int validatedValue = Mathf.Clamp(newValue, 1, 999);
            if (newValue != validatedValue)
            {
                StackLimit.Value = validatedValue;
                MelonLogger.Warning($"StackLimit ajusté à {validatedValue} (doit être entre 1 et 999)");
            }
            MelonPreferences.Save();
            MelonLogger.Msg($"[Config] StackLimit mis à jour : {validatedValue}");
            StackControl.ItemDefinitionPatch.GlobalStackLimit = validatedValue;
            StackControl.ApplyStackLimitToAllItems();
        });
        
        EnsureConfigFileExists();
        MelonPreferences.Save();
    }
    private static string GetUserDataDirectory()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "UserData");
    }
    private static void EnsureConfigFileExists()
    {
        string configPath = Path.Combine("UserData", "StackControl.cfg");
        if(!File.Exists(configPath))
        {
            string defaultConfig = 
                "[StackControl]\n" +
                "# Définit l'incrément de scroll (1-20)\n" +
                "ScrollStep = 5\n" +
                "# Définit la limite globale de stack (1-999)\n" +
                "StackLimit = 40\n";
            
            Directory.CreateDirectory("UserData");
            File.WriteAllText(configPath, defaultConfig);
        }
    }
}
