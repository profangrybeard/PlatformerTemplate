using UnityEngine;
using PrecisionPlatformer.Combat.Data;

namespace PrecisionPlatformer.Data.Combat
{
    /// <summary>
    /// Configuration for individual attacks.
    /// Designers create these assets to define attack properties without code.
    ///
    /// Designer Track: Create via right-click menu, tune values in Inspector.
    /// Coder Track: Reference in Hitbox components, read values for attack logic.
    /// </summary>
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "Platformer/Config/Combat/Attack Config", order = 1)]
    public class AttackConfig : ConfigBase
    {
        [Header("Damage")]
        [Tooltip("Base damage dealt by this attack")]
        [Min(0)]
        public float baseDamage = 10f;

        [Tooltip("Type of damage for resistance calculations")]
        public DamageType damageType = DamageType.Normal;

        [Header("Knockback")]
        [Tooltip("Force applied to target on hit")]
        [Min(0)]
        public float knockbackForce = 5f;

        [Tooltip("Angle of knockback in degrees (0 = horizontal, 90 = straight up)")]
        [Range(-90, 90)]
        public float knockbackAngle = 45f;

        [Tooltip("If true, knockback direction is relative to hit direction. If false, uses attacker facing.")]
        public bool knockbackRelativeToHitDirection = true;

        [Tooltip("Curve for knockback scaling based on damage dealt (X = damage %, Y = knockback multiplier)")]
        public AnimationCurve knockbackScaling = AnimationCurve.Linear(0, 1, 1, 1);

        [Header("Frame Data (at 60 FPS)")]
        [Tooltip("Frames before hitbox becomes active (windup)")]
        [Min(0)]
        public int startupFrames = 5;

        [Tooltip("Frames the hitbox is active (can hit)")]
        [Min(1)]
        public int activeFrames = 3;

        [Tooltip("Frames after hitbox deactivates (cooldown)")]
        [Min(0)]
        public int recoveryFrames = 10;

        [Header("Hitstun")]
        [Tooltip("Duration target cannot act after being hit")]
        [Min(0)]
        public float hitstunDuration = 0.2f;

        [Tooltip("Does this attack cause a hard knockdown?")]
        public bool causesKnockdown = false;

        [Header("Effects")]
        [Tooltip("Hitstop duration on hit (freeze frames for impact feel)")]
        [Min(0)]
        public float hitstopDuration = 0.05f;

        [Tooltip("Camera shake intensity on hit (0 = none)")]
        [Range(0, 1)]
        public float cameraShakeIntensity = 0.3f;

        // Computed properties
        /// <summary>Total attack duration in seconds.</summary>
        public float TotalDuration => (startupFrames + activeFrames + recoveryFrames) / 60f;

        /// <summary>Time until hitbox activates in seconds.</summary>
        public float StartupDuration => startupFrames / 60f;

        /// <summary>Duration hitbox is active in seconds.</summary>
        public float ActiveDuration => activeFrames / 60f;

        /// <summary>Time after hitbox deactivates in seconds.</summary>
        public float RecoveryDuration => recoveryFrames / 60f;

        /// <summary>
        /// Get knockback force scaled by the knockback curve.
        /// </summary>
        /// <param name="damagePercent">Target's damage percentage (0-1+ range)</param>
        public float GetScaledKnockback(float damagePercent)
        {
            return knockbackForce * knockbackScaling.Evaluate(damagePercent);
        }

        public override bool Validate()
        {
            bool isValid = true;

            if (baseDamage < 0)
            {
                Debug.LogWarning($"AttackConfig '{name}': baseDamage cannot be negative");
                isValid = false;
            }

            if (activeFrames < 1)
            {
                Debug.LogWarning($"AttackConfig '{name}': activeFrames must be at least 1");
                isValid = false;
            }

            return isValid;
        }
    }
}
