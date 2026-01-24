using UnityEngine;
using PrecisionPlatformer.Combat.Data;
using PrecisionPlatformer.Core;
using PrecisionPlatformer.Data.Combat;
using PrecisionPlatformer.Events;

namespace PrecisionPlatformer.Combat.Services
{
    /// <summary>
    /// Central service for processing combat interactions.
    /// Subscribes to HitDetectedEvent and coordinates damage, knockback, and effects.
    ///
    /// Register this service in Bootstrap.InitializeServices().
    /// </summary>
    public class CombatService
    {
        private readonly CombatConfig config;
        private float hitstopEndTime;

        public CombatService(CombatConfig combatConfig)
        {
            config = combatConfig;

            // Subscribe to combat events
            EventBus.Subscribe<HitDetectedEvent>(OnHitDetected);
        }

        /// <summary>
        /// Unsubscribe from events. Call this in Bootstrap.OnDestroy().
        /// </summary>
        public void Cleanup()
        {
            EventBus.Unsubscribe<HitDetectedEvent>(OnHitDetected);
        }

        private void OnHitDetected(HitDetectedEvent evt)
        {
            CombatContext context = evt.Context;

            // Validate hit
            if (context.Target == null)
            {
                if (config.debugLogHits)
                {
                    Debug.Log("CombatService: Hit ignored - no target");
                }
                return;
            }

            // Check invulnerability
            if (context.Target.IsInvulnerable)
            {
                if (config.debugLogHits)
                {
                    Debug.Log($"CombatService: Hit ignored - target is invulnerable");
                }
                return;
            }

            // Check friendly fire
            if (!config.allowFriendlyFire && context.Source != null)
            {
                if (context.Source.TeamId != 0 &&
                    context.Source.TeamId == context.Target.TeamId)
                {
                    if (config.debugLogHits)
                    {
                        Debug.Log("CombatService: Hit ignored - friendly fire");
                    }
                    return;
                }
            }

            // Process damage
            ProcessDamage(context, evt.DamageInfo);

            // Process knockback
            ProcessKnockback(context, evt.KnockbackInfo);

            // Request hitstop
            if (config.enableHitstop)
            {
                float hitstopDuration = config.ClampHitstop(evt.DamageInfo.FinalDamage * 0.01f);
                if (hitstopDuration > 0)
                {
                    EventBus.Publish(new HitstopRequestedEvent(hitstopDuration));
                }
            }

            if (config.debugLogHits)
            {
                string sourceName = context.Source?.Transform.name ?? "Environment";
                string targetName = context.Target.Transform.name;
                Debug.Log($"CombatService: {sourceName} hit {targetName} for {evt.DamageInfo.FinalDamage} damage");
            }
        }

        private void ProcessDamage(CombatContext context, DamageInfo damageInfo)
        {
            // Apply damage to target
            context.Target.TakeDamage(damageInfo.FinalDamage);

            // Publish damage applied event
            // Note: RemainingHealth would come from a health component
            // For now we pass -1 to indicate unknown
            EventBus.Publish(new DamageAppliedEvent(context, damageInfo, -1f));
        }

        private void ProcessKnockback(CombatContext context, KnockbackInfo knockbackInfo)
        {
            // Scale knockback by global config
            float scaledForce = config.ScaleKnockback(knockbackInfo.FinalForce);

            // Check if knockback meets threshold
            if (scaledForce < config.knockbackThreshold)
            {
                return;
            }

            // Create scaled knockback
            Vector2 scaledKnockback = knockbackInfo.Direction * scaledForce;

            // Apply knockback to target
            context.Target.ApplyKnockback(scaledKnockback);

            // Clamp hitstun
            float clampedHitstun = config.ClampHitstun(knockbackInfo.HitstunDuration);

            // Publish knockback applied event
            KnockbackInfo scaledInfo = new KnockbackInfo(
                knockbackInfo.BaseForce,
                knockbackInfo.Direction,
                clampedHitstun,
                config.globalKnockbackMultiplier,
                knockbackInfo.CausesKnockdown
            );

            EventBus.Publish(new KnockbackAppliedEvent(context, scaledInfo));
        }

        /// <summary>
        /// Create environmental damage (hazards, spikes, etc.)
        /// </summary>
        public void ApplyEnvironmentalDamage(
            Interfaces.ICombatant target,
            float damage,
            Vector2 contactPoint,
            Vector2 knockbackDirection,
            float knockbackForce = 0f)
        {
            CombatContext context = CombatContext.Environmental(target, contactPoint, knockbackDirection);
            DamageInfo damageInfo = new DamageInfo(damage, 1f, DamageType.Environmental);
            KnockbackInfo knockbackInfo = new KnockbackInfo(knockbackForce, knockbackDirection, 0.1f);

            EventBus.Publish(new HitDetectedEvent(context, damageInfo, knockbackInfo));
        }
    }
}
