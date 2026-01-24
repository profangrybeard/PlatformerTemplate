namespace PrecisionPlatformer.Combat.State
{
    /// <summary>
    /// Character states for both movement and combat.
    /// States have priorities - higher priority states override lower ones.
    ///
    /// Movement states are from the base platformer template.
    /// Combat states are added for the combat track.
    /// </summary>
    public enum CharacterState
    {
        // === Movement States (Priority 0-99) ===

        /// <summary>Standing still on ground.</summary>
        Idle = 0,

        /// <summary>Moving horizontally on ground.</summary>
        Running = 10,

        /// <summary>Rising during a jump.</summary>
        Jumping = 20,

        /// <summary>Falling downward.</summary>
        Falling = 25,

        /// <summary>Sliding down a wall.</summary>
        WallSliding = 30,

        /// <summary>Performing a dash.</summary>
        Dashing = 40,

        // === Combat States (Priority 100-199) ===

        /// <summary>
        /// Performing an attack (startup + active frames).
        /// Cannot be interrupted by movement.
        /// </summary>
        Attacking = 100,

        /// <summary>
        /// Recovery frames after an attack.
        /// Can be canceled into certain actions.
        /// </summary>
        Recovery = 110,

        /// <summary>
        /// Reacting to being hit (hitstun).
        /// Cannot act until hitstun ends.
        /// </summary>
        Hitstun = 150,

        /// <summary>
        /// Being knocked back by an attack.
        /// Physics-driven movement, no control.
        /// </summary>
        Knockback = 160,

        /// <summary>
        /// Invulnerable state (i-frames).
        /// Cannot be hit but can act normally.
        /// </summary>
        Invulnerable = 170,

        // === Special States (Priority 200+) ===

        /// <summary>Dead/defeated state.</summary>
        Dead = 200
    }

    /// <summary>
    /// Extension methods for CharacterState.
    /// </summary>
    public static class CharacterStateExtensions
    {
        /// <summary>
        /// Gets the priority of a state. Higher priority = harder to interrupt.
        /// </summary>
        public static int GetPriority(this CharacterState state)
        {
            return (int)state;
        }

        /// <summary>
        /// Returns true if this state is a movement state (not combat).
        /// </summary>
        public static bool IsMovementState(this CharacterState state)
        {
            return state < CharacterState.Attacking;
        }

        /// <summary>
        /// Returns true if this state is a combat state.
        /// </summary>
        public static bool IsCombatState(this CharacterState state)
        {
            int priority = (int)state;
            return priority >= 100 && priority < 200;
        }

        /// <summary>
        /// Returns true if the character can receive input in this state.
        /// </summary>
        public static bool CanReceiveInput(this CharacterState state)
        {
            return state != CharacterState.Hitstun
                && state != CharacterState.Knockback
                && state != CharacterState.Dead;
        }

        /// <summary>
        /// Returns true if the character can attack from this state.
        /// </summary>
        public static bool CanAttack(this CharacterState state)
        {
            return state.IsMovementState() || state == CharacterState.Recovery;
        }

        /// <summary>
        /// Returns true if the character can move in this state.
        /// </summary>
        public static bool CanMove(this CharacterState state)
        {
            return state.IsMovementState() || state == CharacterState.Invulnerable;
        }

        /// <summary>
        /// Returns true if this state can be interrupted by a new state.
        /// </summary>
        public static bool CanBeInterruptedBy(this CharacterState current, CharacterState newState)
        {
            // Higher priority states always win
            if (newState.GetPriority() > current.GetPriority())
                return true;

            // Same priority - only movement states can transition freely
            if (current.IsMovementState() && newState.IsMovementState())
                return true;

            return false;
        }
    }
}
