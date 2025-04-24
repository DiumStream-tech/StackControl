# StackControl - Version History

## 1.0.4 (2025-04-24)
- **Added**
 - Compatible with Update Checker

## 1.0.3 (2025-04-24)

### Corrections
- **BugFix**
  - Patch du bugs que le fichier StackControl.cfg n'ai pas utiliser mais sa soit le fichier MelonPreferences.cfg qui était utiliser

## 1.0.2 (2025-04-23)

### Features
- **Configuration Dynamique**  
  - Limite de stack configurable de 1 à 999 (au lieu du system par défaut du jeux)
  - Pas de scroll personnalisable 1-20 (précision améliorée)
- **Compatibilité Étendue**  
  - Support des items non-stackables via système de patch Harmony
  - Détection automatique des nouveaux items chargés
- **Optimisations**  
  - Mise à jour instantanée sans redémarrage
  - Logs détaillés avec seuils de stack

### Corrections
- **Bugfix UI**  
  - Limite maximale fixée à 999
  - Validation des quantités négatives désactivée
- **Améliorations Techniques**  
  - Refonte du système de détection de scène Main
  - Patch constructeur d'items plus robuste

## 1.0.1 (2025-04-20)
**Version Initiale**  
Première release stable

### Features
- **Contrôle des Stacks**  
  - Modification via Ctrl+Click Droit+Molette
  - Incréments configurables (5/10/15/20)
- **Système de Configuration**  
  - Fichier `StackControl.cfg` dans UserData
  - Rechargement à chaud des paramètres
- **Logging Avancé**  
  - Affichage temps réel des modifications
  - Détection des erreurs de configuration

### Spécifications Techniques
- Intégration MelonLoader 0.6.1
- Compatibilité .NET 6.0
- Architecture modulaire
