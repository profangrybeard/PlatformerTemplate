#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using PrecisionPlatformer.Data.Combat;
using PrecisionPlatformer.Combat.Data;
using System.IO;

namespace PrecisionPlatformer.Editor
{
    /// <summary>
    /// Editor utility to create default combat configuration assets.
    /// Access via menu: Platformer > Create Default Combat Configs
    /// </summary>
    public static class CombatConfigCreator
    {
        private const string ConfigPath = "Assets/_Project/Configs/Combat";
        private const string AttacksPath = "Assets/_Project/Configs/Combat/Attacks";

        [MenuItem("Platformer/Create Default Combat Configs")]
        public static void CreateDefaultConfigs()
        {
            // Ensure directories exist
            EnsureDirectoryExists(ConfigPath);
            EnsureDirectoryExists(AttacksPath);

            // Create global combat config
            CreateCombatConfig();

            // Create attack configs
            CreateLightAttackConfig();
            CreateHeavyAttackConfig();
            CreateAirAttackConfig();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Combat configs created successfully in " + ConfigPath);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static void CreateCombatConfig()
        {
            string assetPath = $"{ConfigPath}/DefaultCombatConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<CombatConfig>(assetPath) != null)
            {
                Debug.Log("DefaultCombatConfig already exists, skipping.");
                return;
            }

            var config = ScriptableObject.CreateInstance<CombatConfig>();
            config.description = "Global combat settings for the game. Adjust these to change overall combat feel.";

            // I-frames
            config.defaultInvulnerabilityDuration = 0.5f;
            config.invulnerabilityFlashRate = 10f;

            // Hitstop
            config.enableHitstop = true;
            config.defaultHitstopDuration = 0.05f;
            config.maxHitstopDuration = 0.2f;
            config.hitstopTimeScale = 0.05f;

            // Knockback
            config.globalKnockbackMultiplier = 1f;
            config.knockbackThreshold = 2f;
            config.knockbackGravityMultiplier = 0.5f;

            // Hitstun
            config.globalHitstunMultiplier = 1f;
            config.minHitstunDuration = 0.1f;
            config.maxHitstunDuration = 2f;

            // Debug
            config.debugLogHits = true;
            config.showCombatGizmos = true;

            AssetDatabase.CreateAsset(config, assetPath);
            Debug.Log($"Created: {assetPath}");
        }

        private static void CreateLightAttackConfig()
        {
            string assetPath = $"{AttacksPath}/LightAttack.asset";
            if (AssetDatabase.LoadAssetAtPath<AttackConfig>(assetPath) != null)
            {
                Debug.Log("LightAttack already exists, skipping.");
                return;
            }

            var config = ScriptableObject.CreateInstance<AttackConfig>();
            config.description = "Fast, low-damage attack. Quick startup, short recovery.";

            // Damage
            config.baseDamage = 10f;
            config.damageType = DamageType.Normal;

            // Knockback
            config.knockbackForce = 5f;
            config.knockbackAngle = 45f;
            config.knockbackRelativeToHitDirection = true;
            config.knockbackScaling = AnimationCurve.Linear(0f, 1f, 1f, 1.5f);

            // Frame data (at 60fps)
            config.startupFrames = 5;   // ~0.08s
            config.activeFrames = 3;    // ~0.05s
            config.recoveryFrames = 10; // ~0.17s

            // Hitstun
            config.hitstunDuration = 0.2f;
            config.causesKnockdown = false;

            // Effects
            config.hitstopDuration = 0.03f;
            config.cameraShakeIntensity = 0.2f;

            AssetDatabase.CreateAsset(config, assetPath);
            Debug.Log($"Created: {assetPath}");
        }

        private static void CreateHeavyAttackConfig()
        {
            string assetPath = $"{AttacksPath}/HeavyAttack.asset";
            if (AssetDatabase.LoadAssetAtPath<AttackConfig>(assetPath) != null)
            {
                Debug.Log("HeavyAttack already exists, skipping.");
                return;
            }

            var config = ScriptableObject.CreateInstance<AttackConfig>();
            config.description = "Slow, high-damage attack. Long startup, big knockback.";

            // Damage
            config.baseDamage = 25f;
            config.damageType = DamageType.Heavy;

            // Knockback
            config.knockbackForce = 12f;
            config.knockbackAngle = 30f;
            config.knockbackRelativeToHitDirection = true;
            config.knockbackScaling = AnimationCurve.Linear(0f, 1f, 1f, 2f);

            // Frame data (at 60fps)
            config.startupFrames = 15;  // ~0.25s
            config.activeFrames = 5;    // ~0.08s
            config.recoveryFrames = 20; // ~0.33s

            // Hitstun
            config.hitstunDuration = 0.4f;
            config.causesKnockdown = true;

            // Effects
            config.hitstopDuration = 0.08f;
            config.cameraShakeIntensity = 0.5f;

            AssetDatabase.CreateAsset(config, assetPath);
            Debug.Log($"Created: {assetPath}");
        }

        private static void CreateAirAttackConfig()
        {
            string assetPath = $"{AttacksPath}/AirAttack.asset";
            if (AssetDatabase.LoadAssetAtPath<AttackConfig>(assetPath) != null)
            {
                Debug.Log("AirAttack already exists, skipping.");
                return;
            }

            var config = ScriptableObject.CreateInstance<AttackConfig>();
            config.description = "Aerial attack with diagonal knockback. Good for combos.";

            // Damage
            config.baseDamage = 15f;
            config.damageType = DamageType.Normal;

            // Knockback
            config.knockbackForce = 8f;
            config.knockbackAngle = 60f; // More upward
            config.knockbackRelativeToHitDirection = true;
            config.knockbackScaling = AnimationCurve.Linear(0f, 1f, 1f, 1.8f);

            // Frame data (at 60fps)
            config.startupFrames = 6;   // ~0.10s
            config.activeFrames = 4;    // ~0.07s
            config.recoveryFrames = 12; // ~0.20s

            // Hitstun
            config.hitstunDuration = 0.3f;
            config.causesKnockdown = false;

            // Effects
            config.hitstopDuration = 0.05f;
            config.cameraShakeIntensity = 0.3f;

            AssetDatabase.CreateAsset(config, assetPath);
            Debug.Log($"Created: {assetPath}");
        }
    }
}
#endif
