using UnityEngine;
using PrecisionPlatformer.Combat.Interfaces;
using PrecisionPlatformer.Core;
using PrecisionPlatformer.Events;

namespace PrecisionPlatformer.Combat.State
{
    /// <summary>
    /// Manages character state transitions and implements ICombatant.
    /// Attach this to player and enemy GameObjects.
    ///
    /// This component:
    /// - Tracks current state with priority-based transitions
    /// - Implements ICombatant and IHitboxOwner for combat system
    /// - Publishes state change events
    /// - Handles invulnerability frames
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterStateController : MonoBehaviour, ICombatant, IHitboxOwner
    {
        [Header("Combat Settings")]
        [Tooltip("Team ID for friendly fire prevention (0 = neutral)")]
        [SerializeField] private int teamId = 0;

        [Tooltip("Duration of invulnerability after being hit")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        // Current state
        private CharacterState currentState = CharacterState.Idle;
        private float stateStartTime;
        private float stateDuration;

        // Invulnerability
        private float invulnerabilityEndTime;

        // Components
        private Rigidbody2D rb;

        // ICombatant implementation
        public Transform Transform => transform;
        public bool IsInvulnerable => Time.time < invulnerabilityEndTime;
        public int TeamId => teamId;

        // IHitboxOwner implementation
        public ICombatant Combatant => this;

        // Public properties
        public CharacterState CurrentState => currentState;
        public float TimeInState => Time.time - stateStartTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Auto-exit timed states
            if (stateDuration > 0 && TimeInState >= stateDuration)
            {
                ExitTimedState();
            }
        }

        /// <summary>
        /// Attempts to transition to a new state.
        /// Returns true if transition was successful.
        /// </summary>
        /// <param name="newState">State to transition to</param>
        /// <param name="duration">Optional duration for timed states (0 = indefinite)</param>
        public bool TrySetState(CharacterState newState, float duration = 0f)
        {
            // Check if transition is allowed
            if (!currentState.CanBeInterruptedBy(newState))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"State transition blocked: {currentState} -> {newState}");
                }
                return false;
            }

            SetState(newState, duration);
            return true;
        }

        /// <summary>
        /// Forces a state transition regardless of priority.
        /// Use sparingly - mainly for reset/respawn scenarios.
        /// </summary>
        public void ForceSetState(CharacterState newState, float duration = 0f)
        {
            SetState(newState, duration);
        }

        private void SetState(CharacterState newState, float duration)
        {
            CharacterState previousState = currentState;
            currentState = newState;
            stateStartTime = Time.time;
            stateDuration = duration;

            if (showDebugInfo)
            {
                Debug.Log($"State: {previousState} -> {newState}" +
                    (duration > 0 ? $" (duration: {duration}s)" : ""));
            }

            // Publish state change event
            EventBus.Publish(new CharacterStateChangedEvent(previousState, newState));
        }

        private void ExitTimedState()
        {
            // Return to appropriate state based on current state
            CharacterState returnState = currentState switch
            {
                CharacterState.Attacking => CharacterState.Recovery,
                CharacterState.Recovery => CharacterState.Idle,
                CharacterState.Hitstun => CharacterState.Idle,
                CharacterState.Knockback => CharacterState.Falling,
                CharacterState.Invulnerable => CharacterState.Idle,
                _ => CharacterState.Idle
            };

            SetState(returnState, 0f);
        }

        /// <summary>
        /// ICombatant: Called when taking damage.
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (IsInvulnerable)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"{gameObject.name}: Damage blocked (invulnerable)");
                }
                return;
            }

            // Grant i-frames
            invulnerabilityEndTime = Time.time + invulnerabilityDuration;

            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Took {damage} damage");
            }

            // Note: Health system would process damage here
            // Students will implement health tracking in their projects
        }

        /// <summary>
        /// ICombatant: Called when receiving knockback.
        /// </summary>
        public void ApplyKnockback(Vector2 knockbackForce)
        {
            if (IsInvulnerable)
            {
                return;
            }

            // Enter knockback state
            TrySetState(CharacterState.Knockback, 0.2f);

            // Apply physics
            rb.linearVelocity = knockbackForce;

            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Knockback applied: {knockbackForce}");
            }
        }

        /// <summary>
        /// Enter hitstun state for the specified duration.
        /// </summary>
        public void ApplyHitstun(float duration)
        {
            TrySetState(CharacterState.Hitstun, duration);
        }

        /// <summary>
        /// Start an attack with specified frame timing.
        /// </summary>
        /// <param name="startupFrames">Frames before hitbox activates (at 60fps)</param>
        /// <param name="activeFrames">Frames hitbox is active</param>
        /// <param name="recoveryFrames">Frames after hitbox deactivates</param>
        public void StartAttack(int startupFrames, int activeFrames, int recoveryFrames)
        {
            float totalDuration = (startupFrames + activeFrames + recoveryFrames) / 60f;
            TrySetState(CharacterState.Attacking, totalDuration);
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
            if (screenPos.z > 0)
            {
                GUI.Label(
                    new Rect(screenPos.x - 50, Screen.height - screenPos.y, 100, 20),
                    currentState.ToString()
                );
            }
        }
    }
}
