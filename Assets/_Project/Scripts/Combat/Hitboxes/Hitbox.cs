using System.Collections.Generic;
using UnityEngine;
using PrecisionPlatformer.Combat.Data;
using PrecisionPlatformer.Combat.Interfaces;
using PrecisionPlatformer.Core;
using PrecisionPlatformer.Data.Combat;
using PrecisionPlatformer.Events;

namespace PrecisionPlatformer.Combat.Hitboxes
{
    /// <summary>
    /// Hitbox component that deals damage when overlapping with Hurtboxes.
    /// Attach to child GameObjects under the character.
    ///
    /// Features:
    /// - Activate/Deactivate API for frame-based timing
    /// - Prevents multi-hit per activation (hit each target once)
    /// - Auto-finds owner via GetComponentInParent
    /// - Visual gizmos for debugging (red when active)
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [Header("Attack Reference")]
        [Tooltip("Attack configuration for damage/knockback values")]
        [SerializeField] private AttackConfig attackConfig;

        [Header("Hitbox Settings")]
        [Tooltip("Layer mask for hurtboxes")]
        [SerializeField] private LayerMask hurtboxLayer;

        [Tooltip("Can hit the same target multiple times per activation")]
        [SerializeField] private bool allowMultiHit = false;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color activeColor = new Color(1f, 0f, 0f, 0.5f);
        [SerializeField] private Color inactiveColor = new Color(1f, 0f, 0f, 0.2f);

        private Collider2D hitboxCollider;
        private IHitboxOwner owner;
        private HashSet<ICombatant> hitTargets = new HashSet<ICombatant>();
        private bool isActive = false;

        public bool IsActive => isActive;
        public AttackConfig AttackConfig => attackConfig;

        private void Awake()
        {
            hitboxCollider = GetComponent<Collider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;

            // Find owner in parent hierarchy
            owner = GetComponentInParent<IHitboxOwner>();
            if (owner == null)
            {
                Debug.LogWarning($"Hitbox '{gameObject.name}' has no IHitboxOwner in parent hierarchy!");
            }
        }

        /// <summary>
        /// Activate the hitbox. Call this when attack's active frames begin.
        /// </summary>
        public void Activate()
        {
            if (isActive) return;

            isActive = true;
            hitboxCollider.enabled = true;
            hitTargets.Clear();
        }

        /// <summary>
        /// Deactivate the hitbox. Call this when attack's active frames end.
        /// </summary>
        public void Deactivate()
        {
            if (!isActive) return;

            isActive = false;
            hitboxCollider.enabled = false;
            hitTargets.Clear();
        }

        /// <summary>
        /// Activate for a specific duration (in seconds).
        /// Useful for simple attacks without frame-perfect timing.
        /// </summary>
        public void ActivateForDuration(float duration)
        {
            Activate();
            Invoke(nameof(Deactivate), duration);
        }

        /// <summary>
        /// Set the attack config at runtime.
        /// Useful for characters with multiple attacks sharing one hitbox.
        /// </summary>
        public void SetAttackConfig(AttackConfig config)
        {
            attackConfig = config;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;

            ProcessHit(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // Only process if multi-hit is enabled
            if (!isActive || !allowMultiHit) return;

            ProcessHit(other);
        }

        private void ProcessHit(Collider2D other)
        {
            // Check if it's a hurtbox
            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox == null) return;

            // Get the target combatant
            ICombatant target = hurtbox.GetOwnerCombatant();
            if (target == null) return;

            // Don't hit self
            if (owner != null && target == owner.Combatant) return;

            // Don't hit same team (unless neutral)
            if (owner?.Combatant != null && target.TeamId != 0 &&
                owner.Combatant.TeamId == target.TeamId) return;

            // Check if already hit this activation
            if (!allowMultiHit && hitTargets.Contains(target)) return;

            // Mark as hit
            hitTargets.Add(target);

            // Build combat context
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            Vector2 hitDirection = (target.Transform.position - transform.position).normalized;

            CombatContext context = new CombatContext(
                owner?.Combatant,
                target,
                contactPoint,
                hitDirection
            );

            // Build damage info
            float baseDamage = attackConfig != null ? attackConfig.baseDamage : 10f;
            DamageInfo damageInfo = new DamageInfo(
                baseDamage,
                hurtbox.DamageMultiplier,
                attackConfig != null ? attackConfig.damageType : DamageType.Normal
            );

            // Build knockback info
            float knockbackForce = attackConfig != null ? attackConfig.knockbackForce : 5f;
            float knockbackAngle = attackConfig != null ? attackConfig.knockbackAngle : 45f;
            float hitstunDuration = attackConfig != null ? attackConfig.hitstunDuration : 0.2f;

            Vector2 knockbackDirection = attackConfig != null && attackConfig.knockbackRelativeToHitDirection
                ? hitDirection
                : Vector2.right * Mathf.Sign(hitDirection.x);

            KnockbackInfo knockbackInfo = KnockbackInfo.FromHitDirection(
                knockbackDirection,
                knockbackAngle,
                knockbackForce,
                hitstunDuration
            );

            // Publish hit event
            EventBus.Publish(new HitDetectedEvent(context, damageInfo, knockbackInfo));
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Collider2D col = GetComponent<Collider2D>();
            if (col == null) return;

            Gizmos.color = isActive ? activeColor : inactiveColor;

            if (col is BoxCollider2D box)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(
                    transform.position,
                    transform.rotation,
                    transform.lossyScale
                );
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(box.offset, box.size);
                Gizmos.DrawWireCube(box.offset, box.size);
            }
            else if (col is CircleCollider2D circle)
            {
                Vector3 center = transform.TransformPoint(circle.offset);
                float radius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
                Gizmos.DrawSphere(center, radius);
                Gizmos.DrawWireSphere(center, radius);
            }
        }
    }
}
