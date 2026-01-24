using UnityEngine;

namespace PrecisionPlatformer.Data.Combat
{
    /// <summary>
    /// Global combat configuration settings.
    /// One instance per project - controls overall combat feel.
    ///
    /// Designer Track: Tune global combat feel without touching code.
    /// Coder Track: Access via CombatService for consistent settings.
    /// </summary>
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "Platformer/Config/Combat/Combat Config", order = 0)]
    public class CombatConfig : ConfigBase
    {
        [Header("Invulnerability")]
        [Tooltip("Default i-frame duration after taking damage")]
        [Min(0)]
        public float defaultInvulnerabilityDuration = 0.5f;

        [Tooltip("Visual effect for i-frames (sprite flashing rate)")]
        [Min(0)]
        public float invulnerabilityFlashRate = 10f;

        [Header("Hitstop (Freeze Frames)")]
        [Tooltip("Enable hitstop effect on hits")]
        public bool enableHitstop = true;

        [Tooltip("Default hitstop duration if not specified by attack")]
        [Min(0)]
        public float defaultHitstopDuration = 0.05f;

        [Tooltip("Maximum hitstop duration (prevents game from freezing too long)")]
        [Min(0)]
        public float maxHitstopDuration = 0.2f;

        [Tooltip("Time scale during hitstop (0 = complete freeze, 0.1 = slow motion)")]
        [Range(0, 0.5f)]
        public float hitstopTimeScale = 0.05f;

        [Header("Knockback")]
        [Tooltip("Global knockback multiplier (1 = normal, 2 = double)")]
        [Min(0)]
        public float globalKnockbackMultiplier = 1f;

        [Tooltip("Minimum knockback to trigger knockback state")]
        [Min(0)]
        public float knockbackThreshold = 2f;

        [Tooltip("Gravity multiplier during knockback")]
        [Min(0)]
        public float knockbackGravityMultiplier = 0.5f;

        [Header("Hitstun")]
        [Tooltip("Global hitstun multiplier")]
        [Min(0)]
        public float globalHitstunMultiplier = 1f;

        [Tooltip("Minimum hitstun duration")]
        [Min(0)]
        public float minHitstunDuration = 0.1f;

        [Tooltip("Maximum hitstun duration")]
        [Min(0)]
        public float maxHitstunDuration = 2f;

        [Header("Hit Confirmation")]
        [Tooltip("Layer mask for hitbox detection")]
        public LayerMask hurtboxLayerMask;

        [Tooltip("Allow hits between same team (0 = always allow friendly fire)")]
        public bool allowFriendlyFire = false;

        [Header("Debug")]
        [Tooltip("Show hit detection debug info in console")]
        public bool debugLogHits = false;

        [Tooltip("Show hitbox/hurtbox gizmos in Scene view")]
        public bool showCombatGizmos = true;

        /// <summary>
        /// Clamp hitstun duration to configured bounds.
        /// </summary>
        public float ClampHitstun(float duration)
        {
            float scaled = duration * globalHitstunMultiplier;
            return Mathf.Clamp(scaled, minHitstunDuration, maxHitstunDuration);
        }

        /// <summary>
        /// Clamp hitstop duration to configured bounds.
        /// </summary>
        public float ClampHitstop(float duration)
        {
            return Mathf.Min(duration, maxHitstopDuration);
        }

        /// <summary>
        /// Scale knockback by global multiplier.
        /// </summary>
        public float ScaleKnockback(float baseKnockback)
        {
            return baseKnockback * globalKnockbackMultiplier;
        }

        public override bool Validate()
        {
            bool isValid = true;

            if (maxHitstopDuration < defaultHitstopDuration)
            {
                Debug.LogWarning($"CombatConfig '{name}': maxHitstopDuration should be >= defaultHitstopDuration");
                isValid = false;
            }

            if (maxHitstunDuration < minHitstunDuration)
            {
                Debug.LogWarning($"CombatConfig '{name}': maxHitstunDuration should be >= minHitstunDuration");
                isValid = false;
            }

            return isValid;
        }
    }
}
