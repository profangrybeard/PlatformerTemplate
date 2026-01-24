using PrecisionPlatformer.Combat.Data;
using PrecisionPlatformer.Combat.State;

namespace PrecisionPlatformer.Events
{
    /// <summary>
    /// Combat event definitions.
    /// Using readonly structs for zero-allocation event passing.
    /// Follows the same pattern as InputEvents.
    /// </summary>

    /// <summary>
    /// Published when a hitbox overlaps with a hurtbox.
    /// Subscribe to this for hit validation, effects, sound, etc.
    /// </summary>
    public readonly struct HitDetectedEvent
    {
        /// <summary>
        /// Full context of the hit including source, target, and position.
        /// </summary>
        public readonly CombatContext Context;

        /// <summary>
        /// Damage information for this hit.
        /// </summary>
        public readonly DamageInfo DamageInfo;

        /// <summary>
        /// Knockback information for this hit.
        /// </summary>
        public readonly KnockbackInfo KnockbackInfo;

        public HitDetectedEvent(
            CombatContext context,
            DamageInfo damageInfo,
            KnockbackInfo knockbackInfo)
        {
            Context = context;
            DamageInfo = damageInfo;
            KnockbackInfo = knockbackInfo;
        }
    }

    /// <summary>
    /// Published after damage has been applied to a target.
    /// Subscribe for damage numbers, health bar updates, etc.
    /// </summary>
    public readonly struct DamageAppliedEvent
    {
        /// <summary>
        /// Full context of the hit.
        /// </summary>
        public readonly CombatContext Context;

        /// <summary>
        /// Damage that was applied.
        /// </summary>
        public readonly DamageInfo DamageInfo;

        /// <summary>
        /// Target's remaining health after damage.
        /// </summary>
        public readonly float RemainingHealth;

        public DamageAppliedEvent(
            CombatContext context,
            DamageInfo damageInfo,
            float remainingHealth)
        {
            Context = context;
            DamageInfo = damageInfo;
            RemainingHealth = remainingHealth;
        }
    }

    /// <summary>
    /// Published after knockback has been applied to a target.
    /// Subscribe for camera shake, particle effects, etc.
    /// </summary>
    public readonly struct KnockbackAppliedEvent
    {
        /// <summary>
        /// Full context of the hit.
        /// </summary>
        public readonly CombatContext Context;

        /// <summary>
        /// Knockback that was applied.
        /// </summary>
        public readonly KnockbackInfo KnockbackInfo;

        public KnockbackAppliedEvent(
            CombatContext context,
            KnockbackInfo knockbackInfo)
        {
            Context = context;
            KnockbackInfo = knockbackInfo;
        }
    }

    /// <summary>
    /// Published when a character's state changes.
    /// Subscribe for animation transitions, state-based logic, etc.
    /// </summary>
    public readonly struct CharacterStateChangedEvent
    {
        /// <summary>
        /// The previous state.
        /// </summary>
        public readonly CharacterState PreviousState;

        /// <summary>
        /// The new current state.
        /// </summary>
        public readonly CharacterState NewState;

        /// <summary>
        /// Time when the state changed.
        /// </summary>
        public readonly float Timestamp;

        public CharacterStateChangedEvent(
            CharacterState previousState,
            CharacterState newState)
        {
            PreviousState = previousState;
            NewState = newState;
            Timestamp = UnityEngine.Time.time;
        }
    }

    /// <summary>
    /// Published when hitstop (freeze frames) should occur.
    /// Subscribe for game feel effects.
    /// </summary>
    public readonly struct HitstopRequestedEvent
    {
        /// <summary>
        /// Duration of hitstop in seconds.
        /// </summary>
        public readonly float Duration;

        /// <summary>
        /// Intensity of the hitstop (0-1). Higher = more dramatic.
        /// </summary>
        public readonly float Intensity;

        public HitstopRequestedEvent(float duration, float intensity = 1f)
        {
            Duration = duration;
            Intensity = intensity;
        }
    }
}
