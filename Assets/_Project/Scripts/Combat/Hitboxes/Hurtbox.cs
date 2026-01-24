using UnityEngine;
using PrecisionPlatformer.Combat.Interfaces;

namespace PrecisionPlatformer.Combat.Hitboxes
{
    /// <summary>
    /// Hurtbox component that receives damage from Hitboxes.
    /// Attach to child GameObjects under the character.
    ///
    /// Features:
    /// - Damage multiplier for weak/strong points
    /// - Auto-finds owner via GetComponentInParent
    /// - Visual gizmos for debugging (green)
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hurtbox : MonoBehaviour
    {
        [Header("Hurtbox Settings")]
        [Tooltip("Damage multiplier (1.0 = normal, 1.5 = weak point, 0.5 = armored)")]
        [SerializeField] private float damageMultiplier = 1.0f;

        [Tooltip("Is this hurtbox currently active?")]
        [SerializeField] private bool isActive = true;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color activeColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private Color inactiveColor = new Color(0f, 1f, 0f, 0.1f);

        private Collider2D hurtboxCollider;
        private IHitboxOwner owner;

        public float DamageMultiplier => damageMultiplier;
        public bool IsActive => isActive;

        private void Awake()
        {
            hurtboxCollider = GetComponent<Collider2D>();
            hurtboxCollider.isTrigger = true;

            // Find owner in parent hierarchy
            owner = GetComponentInParent<IHitboxOwner>();
            if (owner == null)
            {
                Debug.LogWarning($"Hurtbox '{gameObject.name}' has no IHitboxOwner in parent hierarchy!");
            }
        }

        /// <summary>
        /// Get the ICombatant that owns this hurtbox.
        /// </summary>
        public ICombatant GetOwnerCombatant()
        {
            return owner?.Combatant;
        }

        /// <summary>
        /// Enable this hurtbox.
        /// </summary>
        public void Activate()
        {
            isActive = true;
            hurtboxCollider.enabled = true;
        }

        /// <summary>
        /// Disable this hurtbox (makes character unhittable by this collider).
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
            hurtboxCollider.enabled = false;
        }

        /// <summary>
        /// Set the damage multiplier at runtime.
        /// </summary>
        public void SetDamageMultiplier(float multiplier)
        {
            damageMultiplier = multiplier;
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
            else if (col is CapsuleCollider2D capsule)
            {
                // Approximate capsule as sphere + cylinder
                Vector3 center = transform.TransformPoint(capsule.offset);
                float radius = capsule.size.x * 0.5f * transform.lossyScale.x;
                float height = capsule.size.y * transform.lossyScale.y;
                Gizmos.DrawWireSphere(center + Vector3.up * (height * 0.5f - radius), radius);
                Gizmos.DrawWireSphere(center - Vector3.up * (height * 0.5f - radius), radius);
            }
        }
    }
}
