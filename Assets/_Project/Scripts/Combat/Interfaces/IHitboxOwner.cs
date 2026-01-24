namespace PrecisionPlatformer.Combat.Interfaces
{
    /// <summary>
    /// Interface for entities that own hitboxes and hurtboxes.
    /// Hitbox and Hurtbox components use GetComponentInParent to find their owner.
    ///
    /// This separation allows hitboxes to be on child objects while still
    /// identifying which combatant they belong to.
    /// </summary>
    public interface IHitboxOwner
    {
        /// <summary>
        /// The combatant that owns this hitbox/hurtbox.
        /// Used to prevent self-hits and for event context.
        /// </summary>
        ICombatant Combatant { get; }
    }
}
