# Architecture Overview

This document explains the decentralized architecture patterns used in this platformer template.

---

## Why Decentralized Architecture?

**Problem**: Traditional Unity games often use tightly coupled systems:
- `GameObject.Find()` everywhere
- Direct component references across systems
- Singleton managers for everything
- Hard to test, hard to modify, hard to tune

**Solution**: Decentralized architecture with three core patterns:
1. **ServiceLocator**: Dependency injection for accessing services
2. **EventBus**: Publish/subscribe for cross-system communication
3. **ScriptableObjects**: Data-driven configuration for rapid tuning

---

## Pattern 1: ServiceLocator

### What It Does
Provides centralized registration and retrieval of services without singleton pattern.

### When to Use
- Services that multiple systems need to access (GroundDetection, Physics, Input)
- Systems that should have only one instance (but not MonoBehaviour singletons)
- Replacing `GameObject.Find()` or `FindObjectOfType()` calls

### How It Works

**Registration** (in Bootstrap.cs):
```csharp
var groundDetection = new GroundDetectionService();
ServiceLocator.Register<GroundDetectionService>(groundDetection);
```

**Retrieval** (in MovementController.cs):
```csharp
private GroundDetectionService groundDetection;

void Start()
{
    groundDetection = ServiceLocator.Get<GroundDetectionService>();
    if (groundDetection == null)
    {
        Debug.LogError("GroundDetectionService not found!");
    }
}
```

### Benefits
✅ No `GameObject.Find()` in Update loops (performance)
✅ Services registered in one place (Bootstrap)
✅ Easy to swap implementations for testing
✅ Clear dependencies (explicit Get calls)

### Anti-Patterns to Avoid
❌ Calling `ServiceLocator.Get()` in Update loop (cache reference in Start)
❌ Registering MonoBehaviours directly (create service classes instead)
❌ Using for GameObjects (use prefabs/object pooling instead)

---

## Pattern 2: EventBus

### What It Does
Decoupled publish/subscribe messaging system for cross-system communication.

### When to Use
- One system needs to notify multiple other systems (jump → particles, audio, animation)
- Avoiding direct references between unrelated systems
- Creating reactive systems (UI responds to game state changes)

### How It Works

**Define Event** (in Events/InputEvents.cs):
```csharp
public readonly struct InputJumpPressedEvent
{
    public readonly float Timestamp;

    public InputJumpPressedEvent(float timestamp)
    {
        Timestamp = timestamp;
    }
}
```

**Publish Event** (in JumpController.cs):
```csharp
void OnJumpPressed()
{
    EventBus.Publish(new InputJumpPressedEvent(Time.time));
}
```

**Subscribe to Event** (in FeedbackManager.cs):
```csharp
void OnEnable()
{
    EventBus.Subscribe<InputJumpPressedEvent>(OnJumpStarted);
}

void OnDisable()
{
    EventBus.Unsubscribe<InputJumpPressedEvent>(OnJumpStarted);
}

void OnJumpStarted(InputJumpPressedEvent evt)
{
    // Spawn jump dust particle
    // Play jump sound
    // Trigger jump animation
}
```

### Benefits
✅ Publisher doesn't know about subscribers (loose coupling)
✅ Easy to add new listeners without modifying existing code
✅ Zero-allocation events (readonly structs)
✅ Clean separation of concerns

### Anti-Patterns to Avoid
❌ Forgetting to Unsubscribe in OnDisable (memory leaks!)
❌ Publishing events in Update loop (performance - batch if needed)
❌ Using for direct responses (use ServiceLocator for request/response)
❌ Creating event types for every tiny thing (group related data)

### Memory Leak Prevention
**CRITICAL**: Always unsubscribe in OnDisable/OnDestroy!

```csharp
void OnEnable() => EventBus.Subscribe<JumpEvent>(OnJump);
void OnDisable() => EventBus.Unsubscribe<JumpEvent>(OnJump);
```

If you forget, subscribers stay in memory even when objects are destroyed!

---

## Pattern 3: ScriptableObjects for Configuration

### What It Does
Data-driven configuration that separates tunable values from code logic.

### When to Use
- Any value a designer needs to tune (speed, jump force, timings)
- Shared data between multiple objects (enemy stats, weapon configs)
- Creating preset configurations (Easy/Normal/Hard modes)

### How It Works

**Define Config** (in Data/JumpConfig.cs):
```csharp
[CreateAssetMenu(fileName = "JumpConfig", menuName = "Platformer/Config/Jump Config")]
public class JumpConfig : ConfigBase
{
    [Header("Jump Forces")]
    [Tooltip("Upward force applied when jump button pressed")]
    [Min(0.1f)]
    public float jumpForce = 15f;

    [Tooltip("Maximum jump height for held button")]
    public float maxJumpHeight = 7f;

    [Header("Gravity Scaling")]
    [Tooltip("Gravity multiplier while rising (1.0 = normal)")]
    [Range(0.5f, 2f)]
    public float gravityMultiplierRising = 1f;

    [Tooltip("Gravity multiplier while falling (higher = faster fall)")]
    [Range(1f, 3f)]
    public float gravityMultiplierFalling = 2f;
}
```

**Use Config** (in JumpController.cs):
```csharp
public class JumpController : MonoBehaviour
{
    [SerializeField] private JumpConfig config;

    void Jump()
    {
        rigidbody.AddForce(Vector2.up * config.jumpForce, ForceMode2D.Impulse);
    }
}
```

### Benefits
✅ Designers tune values without touching code
✅ Changes visible immediately in Inspector
✅ Easy to create multiple configs (presets)
✅ Version control friendly (separate .asset files)
✅ Reusable across multiple objects

### Anti-Patterns to Avoid
❌ Hardcoding values in scripts
❌ Using public fields on MonoBehaviours for tuning (use ScriptableObjects!)
❌ Modifying ScriptableObject values at runtime (creates shared state bugs)
❌ Skipping [Header], [Tooltip], [Min], [Range] attributes (designers need context!)

### Validation Example
```csharp
public override bool Validate()
{
    bool isValid = true;

    if (jumpForce <= 0f)
    {
        Debug.LogWarning("JumpConfig: jumpForce must be positive!");
        isValid = false;
    }

    if (maxJumpHeight < 1f)
    {
        Debug.LogWarning("JumpConfig: maxJumpHeight should be at least 1 unit");
        isValid = false;
    }

    return isValid;
}
```

---

## Data Flow Example: Jump System

Let's trace a complete jump from input to visual feedback:

### 1. Input Layer
```csharp
// InputHandler.cs
void OnJumpPerformed(InputAction.CallbackContext context)
{
    EventBus.Publish(new InputJumpPressedEvent(Time.time));
}
```

### 2. Logic Layer
```csharp
// JumpController.cs
void OnEnable()
{
    EventBus.Subscribe<InputJumpPressedEvent>(OnJumpPressed);
}

void OnJumpPressed(InputJumpPressedEvent evt)
{
    // Check if can jump (using GroundDetectionService)
    var groundDetection = ServiceLocator.Get<GroundDetectionService>();
    if (!groundDetection.IsGrounded()) return;

    // Apply jump force (using JumpConfig)
    rigidbody.AddForce(Vector2.up * jumpConfig.jumpForce, ForceMode2D.Impulse);

    // Publish jump started event
    EventBus.Publish(new JumpStartedEvent(evt.Timestamp));
}
```

### 3. Feedback Layer
```csharp
// FeedbackManager.cs
void OnEnable()
{
    EventBus.Subscribe<JumpStartedEvent>(OnJumpStarted);
}

void OnJumpStarted(JumpStartedEvent evt)
{
    // Spawn particle from pool
    var poolService = ServiceLocator.Get<ParticlePoolService>();
    var jumpDust = poolService.Get(ParticleType.JumpDust);
    jumpDust.transform.position = player.position;

    // Play audio
    audioSource.PlayOneShot(jumpSound);

    // Trigger camera shake
    var screenShake = ServiceLocator.Get<ScreenshakeService>();
    screenShake.AddTrauma(0.1f);
}
```

### Notice:
- **No direct references** between systems
- **InputHandler** doesn't know about **JumpController**
- **JumpController** doesn't know about **FeedbackManager**
- Each system is independently testable
- Adding new feedback (animation, UI) doesn't require modifying existing code

---

## System Diagram

```
┌─────────────────────────────────────────────────────────┐
│                      BOOTSTRAP                          │
│  - Initializes ServiceLocator and EventBus              │
│  - Registers all services at startup                    │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                   SERVICE LOCATOR                        │
│  - GroundDetectionService                               │
│  - PhysicsService                                        │
│  - ParticlePoolService                                   │
│  - ScreenshakeService                                    │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                      EVENT BUS                           │
│  - InputJumpPressedEvent                                │
│  - JumpStartedEvent                                      │
│  - MovementStateChangedEvent                            │
│  - DashStartedEvent                                      │
│  - ...                                                   │
└─────────────────────────────────────────────────────────┘
                            │
                ┌───────────┼───────────┐
                ▼           ▼           ▼
        ┌──────────┐  ┌──────────┐  ┌──────────┐
        │  Input   │  │ Movement │  │ Feedback │
        │ Handler  │  │Controller│  │ Manager  │
        └──────────┘  └──────────┘  └──────────┘
             │             │              │
             └─────────────┴──────────────┘
                          │
                    Uses Configs
                          ▼
            ┌───────────────────────────┐
            │  ScriptableObject Configs │
            │  - MovementConfig         │
            │  - JumpConfig             │
            │  - DashConfig             │
            └───────────────────────────┘
```

---

## Comparison: Centralized vs. Decentralized

### Centralized (Traditional) Approach
```csharp
// ❌ Tightly coupled, hard to maintain
public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;
    private ParticleManager particleManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        particleManager = GameObject.Find("ParticleManager").GetComponent<ParticleManager>();
    }

    void Jump()
    {
        // Jump logic here...

        audioManager.PlayJumpSound();
        particleManager.SpawnJumpDust(transform.position);
        gameManager.OnPlayerJumped();
    }
}
```

**Problems**:
- GameObject.Find() is slow
- Player depends on specific manager names
- Can't test Player in isolation
- Adding new feedback requires modifying Player class

### Decentralized (This Template) Approach
```csharp
// ✅ Loosely coupled, easy to extend
public class JumpController : MonoBehaviour
{
    [SerializeField] private JumpConfig config;
    private GroundDetectionService groundDetection;

    void Start()
    {
        groundDetection = ServiceLocator.Get<GroundDetectionService>();
        EventBus.Subscribe<InputJumpPressedEvent>(OnJumpPressed);
    }

    void OnJumpPressed(InputJumpPressedEvent evt)
    {
        if (!groundDetection.IsGrounded()) return;

        // Jump logic here...

        EventBus.Publish(new JumpStartedEvent(evt.Timestamp));
    }
}

// Completely separate FeedbackManager handles audio/particles
public class FeedbackManager : MonoBehaviour
{
    void OnEnable()
    {
        EventBus.Subscribe<JumpStartedEvent>(OnJumpStarted);
    }

    void OnJumpStarted(JumpStartedEvent evt)
    {
        // Play audio, spawn particles, etc.
    }
}
```

**Benefits**:
- No GameObject.Find()
- JumpController has no idea FeedbackManager exists
- Can test JumpController in isolation
- Adding new feedback = create new subscriber (no code changes to JumpController!)

---

## Testing Benefits

### Unit Testing
With decentralized architecture, you can test systems in isolation:

```csharp
[Test]
public void TestGroundDetection()
{
    // Arrange
    var mockConfig = ScriptableObject.CreateInstance<GroundDetectionConfig>();
    var service = new GroundDetectionService(mockConfig);

    // Act
    bool isGrounded = service.CheckGround(Vector2.zero, Vector2.down);

    // Assert
    Assert.IsFalse(isGrounded); // Nothing below, should be false
}
```

No need for entire Unity scene or Player GameObject!

---

## Performance Considerations

### ServiceLocator
- ✅ Dictionary lookup is O(1) - very fast
- ✅ Cache references in Start() instead of calling Get() every frame
- ❌ Don't call ServiceLocator.Get() in Update loop

### EventBus
- ✅ Readonly structs = zero allocations (no garbage collection)
- ✅ Delegate invocation is fast (microseconds)
- ❌ Don't publish events every frame for trivial changes (batch updates)
- ❌ Always unsubscribe to prevent memory leaks

### ScriptableObjects
- ✅ Shared data = low memory footprint (one instance, many users)
- ✅ Reading values is instant (direct field access)
- ❌ Don't modify ScriptableObject values at runtime (creates bugs!)
- ❌ Use runtime classes for per-instance data

---

## Best Practices Summary

### DO ✅
- Register all services in Bootstrap.InitializeServices()
- Publish events for "something happened" notifications
- Use ScriptableObjects for all tunable parameters
- Cache ServiceLocator.Get() results in Start/Awake
- Unsubscribe from EventBus in OnDisable/OnDestroy
- Add [Header], [Tooltip], [Min], [Range] to config fields
- Document events with XML comments

### DON'T ❌
- Call ServiceLocator.Get() in Update loop
- Forget to unsubscribe from EventBus (memory leak!)
- Modify ScriptableObject values at runtime
- Use MonoBehaviour singletons (except Bootstrap)
- Tightly couple systems with direct references
- Skip validation logic in ScriptableObject configs

---

## Combat System Architecture (Combat Track)

The combat system extends the base architecture patterns for hitbox-based combat.

### Combat Events

Combat uses the same EventBus pattern with specialized events:

```csharp
// Events/CombatEvents.cs
public readonly struct HitDetectedEvent
{
    public readonly CombatContext Context;   // Source, target, contact point
    public readonly DamageInfo DamageInfo;   // Damage amount and type
    public readonly KnockbackInfo KnockbackInfo;  // Force and direction
}

public readonly struct DamageAppliedEvent { ... }
public readonly struct KnockbackAppliedEvent { ... }
public readonly struct CharacterStateChangedEvent { ... }
```

### ICombatant Interface

All combat participants implement ICombatant:

```csharp
public interface ICombatant
{
    Transform Transform { get; }
    bool IsInvulnerable { get; }
    int TeamId { get; }
    void TakeDamage(float damage);
    void ApplyKnockback(Vector2 knockbackForce);
}
```

**Multiplayer-Ready**: When player slots are added, ICombatant gains a `PlayerIndex` property. No event refactoring needed since CombatContext carries ICombatant references.

### Hitbox/Hurtbox System

```
Character (IHitboxOwner, ICombatant)
├── Body (Hurtbox - green gizmo)
├── Head (Hurtbox - weak point, 1.5x damage multiplier)
└── AttackPoint
    └── Hitbox (red gizmo - activates during attacks)
```

- **Hitbox**: Deals damage on overlap. Activate/Deactivate API for frame timing.
- **Hurtbox**: Receives damage. Optional damage multiplier for weak points.
- Both auto-find owner via `GetComponentInParent<IHitboxOwner>()`.

### CombatService

Central service that processes hit events:

```csharp
// Registered in Bootstrap.cs
combatService = new CombatService(combatConfig);
ServiceLocator.Register<CombatService>(combatService);
```

Responsibilities:
- Validates hits (invulnerability, friendly fire)
- Processes damage and knockback
- Applies global config modifiers (knockback multiplier, hitstun clamping)
- Publishes follow-up events (DamageApplied, KnockbackApplied)

### CharacterState Machine

State priorities ensure combat states override movement:

```csharp
public enum CharacterState
{
    // Movement States (0-99)
    Idle = 0,
    Running = 10,
    Jumping = 20,
    Falling = 25,

    // Combat States (100-199)
    Attacking = 100,
    Recovery = 110,
    Hitstun = 150,
    Knockback = 160,
    Invulnerable = 170,

    // Special States (200+)
    Dead = 200
}
```

### Combat Data Flow

```
1. Attack input → Character starts attack
2. StartupFrames pass → Hitbox.Activate()
3. Hitbox overlaps Hurtbox → ProcessHit()
4. EventBus.Publish(HitDetectedEvent)
5. CombatService receives event:
   - Validates hit
   - Calculates damage/knockback
   - Calls target.TakeDamage() and ApplyKnockback()
   - Publishes DamageAppliedEvent, KnockbackAppliedEvent
6. FeedbackManager receives events:
   - Spawns hit particles
   - Plays hit sound
   - Triggers camera shake
7. ActiveFrames end → Hitbox.Deactivate()
8. RecoveryFrames pass → Return to Idle
```

### AttackConfig (ScriptableObject)

Designer-tunable attack properties:

```csharp
[CreateAssetMenu(menuName = "Platformer/Config/Combat/Attack Config")]
public class AttackConfig : ConfigBase
{
    // Damage
    public float baseDamage = 10f;
    public DamageType damageType = DamageType.Normal;

    // Knockback
    public float knockbackForce = 5f;
    public float knockbackAngle = 45f;
    public bool knockbackRelativeToHitDirection = true;

    // Frame Data (at 60 FPS)
    public int startupFrames = 5;
    public int activeFrames = 3;
    public int recoveryFrames = 10;

    // Hitstun
    public float hitstunDuration = 0.2f;
}
```

---

## Additional Resources

### Recommended Reading
- [Unite Talk: Overthrowing the MonoBehaviour Tyranny](https://www.youtube.com/watch?v=6vmRwLYWNRo)
- [ScriptableObject Architecture by Ryan Hipple](https://www.youtube.com/watch?v=raQ3iHhE_Kk)
- [Dependency Injection in Unity](https://www.youtube.com/watch?v=2PIMEjhWrGo)

### External Examples
- Vampire Survivors template (your previous project)
- Celeste source code (GDC talk references)

---

## Questions for Understanding

Test your knowledge:

1. **When should you use ServiceLocator vs. EventBus?**
   - ServiceLocator: Request data/service (GroundDetection.IsGrounded())
   - EventBus: Notify multiple systems (JumpStarted event)

2. **Why use readonly structs for events?**
   - Zero allocations = no garbage collection = better performance

3. **Why can't you modify ScriptableObject values at runtime?**
   - ScriptableObjects are shared assets - modifications persist in editor!
   - Use runtime classes for per-instance data

4. **What happens if you forget to unsubscribe from EventBus?**
   - Memory leak - subscriber stays in memory even after destruction
   - Events still fire for destroyed objects (null reference errors)

5. **Why cache ServiceLocator.Get() results?**
   - Calling Get() every frame wastes CPU cycles
   - Cache in Start() and reuse the reference

---

**Master these patterns and you'll write cleaner, more maintainable Unity code! 🎮**
