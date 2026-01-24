namespace PrecisionPlatformer.Combat.Data
{
    /// <summary>
    /// Immutable struct containing damage calculation results.
    /// Separate from CombatContext to allow damage modification by systems.
    /// </summary>
    public readonly struct DamageInfo
    {
        /// <summary>
        /// Base damage before modifiers.
        /// </summary>
        public readonly float BaseDamage;

        /// <summary>
        /// Final damage after all modifiers applied.
        /// </summary>
        public readonly float FinalDamage;

        /// <summary>
        /// Multiplier applied (e.g., from hurtbox weak points).
        /// </summary>
        public readonly float DamageMultiplier;

        /// <summary>
        /// Type of damage for resistance calculations.
        /// </summary>
        public readonly DamageType Type;

        /// <summary>
        /// Whether this hit was a critical hit.
        /// </summary>
        public readonly bool IsCritical;

        public DamageInfo(
            float baseDamage,
            float damageMultiplier = 1f,
            DamageType type = DamageType.Normal,
            bool isCritical = false)
        {
            BaseDamage = baseDamage;
            DamageMultiplier = damageMultiplier;
            FinalDamage = baseDamage * damageMultiplier * (isCritical ? 2f : 1f);
            Type = type;
            IsCritical = isCritical;
        }
    }

    /// <summary>
    /// Types of damage for resistance/vulnerability systems.
    /// </summary>
    public enum DamageType
    {
        Normal,      // Standard physical damage
        Heavy,       // Breaks guards, high hitstun
        Piercing,    // Ignores some armor
        Environmental // Hazards, falling, etc.
    }
}
