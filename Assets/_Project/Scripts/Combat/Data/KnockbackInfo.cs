using UnityEngine;

namespace PrecisionPlatformer.Combat.Data
{
    /// <summary>
    /// Immutable struct containing knockback calculation results.
    /// Separate from CombatContext to allow knockback modification by systems.
    /// </summary>
    public readonly struct KnockbackInfo
    {
        /// <summary>
        /// Base knockback force from the attack config.
        /// </summary>
        public readonly float BaseForce;

        /// <summary>
        /// Final knockback force after modifiers.
        /// </summary>
        public readonly float FinalForce;

        /// <summary>
        /// Direction of knockback in world space.
        /// </summary>
        public readonly Vector2 Direction;

        /// <summary>
        /// Calculated knockback vector (Direction * FinalForce).
        /// Apply this directly to the target's velocity or AddForce.
        /// </summary>
        public readonly Vector2 KnockbackVector;

        /// <summary>
        /// Duration of hitstun in seconds.
        /// Target cannot act during hitstun.
        /// </summary>
        public readonly float HitstunDuration;

        /// <summary>
        /// Whether this knockback causes a hard knockdown.
        /// </summary>
        public readonly bool CausesKnockdown;

        public KnockbackInfo(
            float baseForce,
            Vector2 direction,
            float hitstunDuration,
            float forceMultiplier = 1f,
            bool causesKnockdown = false)
        {
            BaseForce = baseForce;
            FinalForce = baseForce * forceMultiplier;
            Direction = direction.normalized;
            KnockbackVector = Direction * FinalForce;
            HitstunDuration = hitstunDuration;
            CausesKnockdown = causesKnockdown;
        }

        /// <summary>
        /// Creates knockback relative to hit direction with angle offset.
        /// </summary>
        /// <param name="hitDirection">Direction from attacker to target</param>
        /// <param name="angleOffset">Angle offset in degrees (positive = upward)</param>
        /// <param name="baseForce">Base knockback force</param>
        /// <param name="hitstunDuration">Hitstun duration in seconds</param>
        public static KnockbackInfo FromHitDirection(
            Vector2 hitDirection,
            float angleOffset,
            float baseForce,
            float hitstunDuration)
        {
            // Rotate hit direction by angle offset
            float radians = angleOffset * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            Vector2 rotatedDirection = new Vector2(
                hitDirection.x * cos - hitDirection.y * sin,
                hitDirection.x * sin + hitDirection.y * cos
            );

            return new KnockbackInfo(baseForce, rotatedDirection, hitstunDuration);
        }
    }
}
