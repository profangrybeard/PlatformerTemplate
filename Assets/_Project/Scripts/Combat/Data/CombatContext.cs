using UnityEngine;
using PrecisionPlatformer.Combat.Interfaces;

namespace PrecisionPlatformer.Combat.Data
{
    /// <summary>
    /// Immutable context for combat interactions.
    /// Passed through events to provide all relevant hit information.
    ///
    /// Multiplayer-Ready: When player slots are added, access player index via
    /// context.Source.PlayerIndex or context.Target.PlayerIndex. No changes
    /// needed to this struct or the events that use it.
    /// </summary>
    public readonly struct CombatContext
    {
        /// <summary>
        /// The attacker/source of damage. Null for environmental damage.
        /// </summary>
        public readonly ICombatant Source;

        /// <summary>
        /// The defender/target receiving damage.
        /// </summary>
        public readonly ICombatant Target;

        /// <summary>
        /// World position where the hit occurred.
        /// Useful for spawning effects at impact point.
        /// </summary>
        public readonly Vector2 ContactPoint;

        /// <summary>
        /// Direction from source to target at time of hit.
        /// Used for directional knockback calculations.
        /// </summary>
        public readonly Vector2 HitDirection;

        /// <summary>
        /// Time.time when the hit occurred.
        /// Useful for hit validation and replay systems.
        /// </summary>
        public readonly float Timestamp;

        public CombatContext(
            ICombatant source,
            ICombatant target,
            Vector2 contactPoint,
            Vector2 hitDirection)
        {
            Source = source;
            Target = target;
            ContactPoint = contactPoint;
            HitDirection = hitDirection.normalized;
            Timestamp = Time.time;
        }

        /// <summary>
        /// Creates a context for environmental/hazard damage (no source).
        /// </summary>
        public static CombatContext Environmental(
            ICombatant target,
            Vector2 contactPoint,
            Vector2 hitDirection)
        {
            return new CombatContext(null, target, contactPoint, hitDirection);
        }
    }
}
