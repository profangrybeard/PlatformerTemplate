using UnityEngine;

namespace PrecisionPlatformer.Combat.Interfaces
{
    /// <summary>
    /// Interface for any entity that can participate in combat.
    /// Implemented by characters, enemies, destructibles, etc.
    ///
    /// Multiplayer-Ready: When player slots are added later, this interface
    /// will gain a PlayerIndex property. No event refactoring needed since
    /// CombatContext already carries ICombatant references.
    /// </summary>
    public interface ICombatant
    {
        /// <summary>
        /// The transform of this combatant, used for position-based calculations.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Whether this combatant is currently invulnerable to damage.
        /// Used for i-frames, special states, etc.
        /// </summary>
        bool IsInvulnerable { get; }

        /// <summary>
        /// Optional team identifier for friendly fire prevention.
        /// 0 = neutral (hits everyone), same team = no friendly fire.
        /// </summary>
        int TeamId { get; }

        /// <summary>
        /// Called when this combatant receives damage.
        /// </summary>
        /// <param name="damage">Amount of damage to apply</param>
        void TakeDamage(float damage);

        /// <summary>
        /// Called when this combatant receives knockback.
        /// </summary>
        /// <param name="knockbackForce">Direction and magnitude of knockback</param>
        void ApplyKnockback(Vector2 knockbackForce);
    }
}
