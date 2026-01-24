# Combat Track Guide

This guide covers the combat/brawler foundation for students who complete the platformer basics early.

---

## Prerequisites

**You must complete these before starting the Combat Track:**

- [ ] Session 1: Horizontal movement working
- [ ] Session 2: Jump mechanics working
- [ ] Understand EventBus pattern (can subscribe to events)
- [ ] Understand ServiceLocator pattern (can register/get services)

---

## Combat Architecture Overview

```
Player/Enemy (CharacterStateController)
├── Implements ICombatant (can receive damage/knockback)
├── Implements IHitboxOwner (owns hitboxes/hurtboxes)
│
├── Body Hurtbox (receives hits)
│   └── Hurtbox.cs + Collider2D
│
└── Attack Hitbox (deals damage when active)
    └── Hitbox.cs + Collider2D + AttackConfig reference
```

### Data Flow

```
Input → Attack Start → Startup Frames → Hitbox Active → Hit Detected
                                                             ↓
                                        ←───────────────────────────
                                        ↓
                               HitDetectedEvent published
                                        ↓
                               CombatService processes
                                        ↓
                        ┌───────────────┴───────────────┐
                        ↓                               ↓
                 Target.TakeDamage()           Target.ApplyKnockback()
                        ↓                               ↓
                 DamageAppliedEvent            KnockbackAppliedEvent
                        ↓                               ↓
                 UI/Health systems             Physics/State systems
```

---

## Setup Checklist

### Step 1: Enable CombatService

In `Bootstrap.cs`, uncomment the combat service registration:

```csharp
// Combat Service (Combat Track)
if (combatConfig != null)
{
    combatService = new CombatService(combatConfig);
    ServiceLocator.Register<CombatService>(combatService);
    Debug.Log("Bootstrap: CombatService registered");
}
```

### Step 2: Create CombatConfig Asset

1. Right-click in `Assets/_Project/Configs/Combat/`
2. Select **Create > Platformer > Config > Combat > Combat Config**
3. Name it `DefaultCombatConfig`
4. Drag into Bootstrap's `combatConfig` field

### Step 3: Add CharacterStateController

1. Select your Player GameObject
2. Add Component: `CharacterStateController`
3. Configure `teamId` (0 = neutral, 1 = player, 2 = enemy)
4. Enable `showDebugInfo` for development

### Step 4: Create Hurtbox

1. Create child GameObject under Player: "BodyHurtbox"
2. Add BoxCollider2D (or CapsuleCollider2D), set as Trigger
3. Add Component: `Hurtbox`
4. Size to match character body
5. Set layer to "Hurtbox" (create layer if needed)

### Step 5: Create Hitbox

1. Create child GameObject under Player: "AttackHitbox"
2. Add BoxCollider2D, set as Trigger
3. Add Component: `Hitbox`
4. Position in front of character
5. Set `hurtboxLayer` to match hurtbox layer
6. Create and assign an `AttackConfig`

### Step 6: Create AttackConfig

1. Right-click in `Assets/_Project/Configs/Combat/Attacks/`
2. Select **Create > Platformer > Config > Combat > Attack Config**
3. Name it `LightAttack`
4. Configure values:
   - `baseDamage`: 10
   - `knockbackForce`: 5
   - `knockbackAngle`: 45
   - `startupFrames`: 5
   - `activeFrames`: 3
   - `recoveryFrames`: 10

---

## Implementation Guide

### Basic Attack Input

```csharp
// In your PlayerController or InputHandler
void OnAttackPressed()
{
    // Check if can attack
    var stateController = GetComponent<CharacterStateController>();
    if (!stateController.CurrentState.CanAttack())
        return;

    // Start attack
    var attackConfig = hitbox.AttackConfig;
    stateController.StartAttack(
        attackConfig.startupFrames,
        attackConfig.activeFrames,
        attackConfig.recoveryFrames
    );

    // Schedule hitbox activation
    StartCoroutine(AttackSequence(attackConfig));
}

IEnumerator AttackSequence(AttackConfig config)
{
    // Wait for startup
    yield return new WaitForSeconds(config.StartupDuration);

    // Activate hitbox
    hitbox.Activate();

    // Wait for active frames
    yield return new WaitForSeconds(config.ActiveDuration);

    // Deactivate hitbox
    hitbox.Deactivate();
}
```

### Combat Feedback

```csharp
public class CombatFeedback : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private float screenShakeIntensity = 0.3f;

    void OnEnable()
    {
        EventBus.Subscribe<HitDetectedEvent>(OnHitDetected);
        EventBus.Subscribe<HitstopRequestedEvent>(OnHitstopRequested);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<HitDetectedEvent>(OnHitDetected);
        EventBus.Unsubscribe<HitstopRequestedEvent>(OnHitstopRequested);
    }

    void OnHitDetected(HitDetectedEvent evt)
    {
        // Spawn particles at hit location
        hitParticles.transform.position = evt.Context.ContactPoint;
        hitParticles.Play();

        // Play hit sound
        hitSound.Play();

        // TODO: Add camera shake
    }

    void OnHitstopRequested(HitstopRequestedEvent evt)
    {
        StartCoroutine(ApplyHitstop(evt.Duration));
    }

    IEnumerator ApplyHitstop(float duration)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
```

---

## Testing Zones

Add these to your Dev Playground scene:

### Zone K: Combat Arena
- Flat ground area
- Training dummy (Hurtbox only, no AI)
- Visual damage number spawning

### Zone L: Knockback Trajectory
- Angled surfaces for knockback testing
- Height markers for launch distance
- Damage percentage display

---

## Tuning Guide

### Attack Speed
- `startupFrames`: Lower = faster, less telegraphed
- `activeFrames`: Higher = easier to land
- `recoveryFrames`: Lower = less punishable

### Damage Balance
- Light Attack: 10 damage, 5 knockback
- Heavy Attack: 25 damage, 12 knockback
- Air Attack: 15 damage, 8 knockback (45-60 degree angle)

### Knockback Feel
- `knockbackAngle`: 45 = diagonal up, 0 = horizontal, 90 = straight up
- `knockbackRelativeToHitDirection`: true for facing-based, false for fixed
- `hitstunDuration`: 0.2s (light) to 0.5s (heavy)

### Global Tuning (CombatConfig)
- `globalKnockbackMultiplier`: Scale all knockback (1.0 = normal)
- `knockbackThreshold`: Minimum force to trigger knockback state
- `defaultInvulnerabilityDuration`: I-frames after hit (0.5s default)

---

## Common Issues

### Hitbox not detecting hits
1. Check both objects have correct layers
2. Verify `hurtboxLayer` is set on Hitbox
3. Ensure colliders are set as Triggers
4. Check that hitbox is calling `Activate()`

### Self-hit detected
1. Ensure `IHitboxOwner` is on parent with `CharacterStateController`
2. Hitbox auto-filters self via owner comparison

### Knockback not working
1. Check target has `Rigidbody2D`
2. Verify `CharacterStateController` is on target
3. Check `knockbackForce` in AttackConfig is > 0
4. Verify `knockbackThreshold` in CombatConfig

### Events not firing
1. Ensure `CombatService` is registered in Bootstrap
2. Check `combatConfig` is assigned in Bootstrap
3. Verify EventBus subscriptions use correct event types

---

## Multiplayer Extension Notes

When player slots are added in future:

1. **ICombatant** gains `int PlayerIndex { get; }`
2. **InputHandler** splits by player index
3. **Events**: Access via `context.Source.PlayerIndex`
4. **No event refactoring needed** - CombatContext already uses interfaces

---

## Files Reference

### Core Combat
- `Scripts/Combat/Interfaces/ICombatant.cs`
- `Scripts/Combat/Interfaces/IHitboxOwner.cs`
- `Scripts/Combat/State/CharacterState.cs`
- `Scripts/Combat/State/CharacterStateController.cs`

### Hitbox System
- `Scripts/Combat/Hitboxes/Hitbox.cs`
- `Scripts/Combat/Hitboxes/Hurtbox.cs`

### Data
- `Scripts/Combat/Data/CombatContext.cs`
- `Scripts/Combat/Data/DamageInfo.cs`
- `Scripts/Combat/Data/KnockbackInfo.cs`

### Events
- `Scripts/Events/CombatEvents.cs`

### Configs
- `Scripts/Data/Combat/AttackConfig.cs`
- `Scripts/Data/Combat/CombatConfig.cs`

### Services
- `Scripts/Combat/Services/CombatService.cs`
