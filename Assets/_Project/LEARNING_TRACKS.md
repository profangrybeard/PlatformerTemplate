# Learning Tracks

This template supports two parallel learning tracks that can be followed independently or together.

---

## Track Overview

| Track | Focus | Primary Role | When to Start |
|-------|-------|--------------|---------------|
| **Platformer** | Movement, physics, game feel | Both | Session 1 |
| **Combat** | Hitboxes, damage, knockback | Advanced | After Session 2 |

---

## Designer Track (Inspector-Only)

The Designer Track focuses on tuning and configuration without writing code.

### Tools You'll Use
- Unity Inspector for editing values
- ScriptableObject assets for configuration
- Scene View for visual placement
- Gizmos for debugging hitboxes

### Platformer Skills

**Session 1-2: Movement Feel**
1. Open `Configs/MovementConfig.asset`
2. Tune these values while playtesting:
   - `maxSpeed` - How fast the character runs
   - `acceleration` - How quickly they reach max speed
   - `deceleration` - How quickly they stop
   - `airControlMultiplier` - Control while airborne

**Session 3-4: Jump Feel**
1. Open `Configs/JumpConfig.asset` (when created)
2. Tune jump properties:
   - `jumpForce` - Initial upward velocity
   - `gravityMultiplierFalling` - Snappier fall
   - `coyoteTime` - Forgiveness buffer

### Combat Skills

**Prerequisites**: Complete Platformer Sessions 1-2 first.

**Creating Attack Configs**
1. Right-click in `Configs/Combat/Attacks/`
2. Select **Create > Platformer > Config > Combat > Attack Config**
3. Name it (e.g., "LightAttack")
4. Tune in Inspector:
   - `baseDamage` - Damage dealt
   - `knockbackForce` - Push strength
   - `knockbackAngle` - Direction (45 = diagonal up)
   - `startupFrames` - Wind-up time (at 60fps)
   - `activeFrames` - Hit window
   - `recoveryFrames` - Cooldown

**Hitbox/Hurtbox Setup**
1. Add `Hurtbox` component to character body
2. Add child GameObject with `Hitbox` component for attacks
3. Adjust collider sizes in Scene View
4. Assign AttackConfig to Hitbox

**Visual Debugging**
- Red gizmos = Hitboxes (active when attacking)
- Green gizmos = Hurtboxes (damageable areas)
- Enable `showGizmos` on components to see them

---

## Coder Track (Code Implementation)

The Coder Track focuses on implementing systems and mechanics.

### Platformer Skills

**Session 1-2: Core Movement**
1. Implement `MovementController.cs`
   - Subscribe to `InputMoveEvent`
   - Apply velocity based on MovementConfig
   - Handle acceleration/deceleration curves

2. Implement `JumpController.cs`
   - Subscribe to `InputJumpPressedEvent`
   - Check ground state via GroundDetectionService
   - Apply jump force from JumpConfig

**Session 3-4: Advanced Movement**
1. Implement coyote time (input buffering)
2. Implement variable jump height (hold vs tap)
3. Create DashController with cooldown

### Combat Skills

**Prerequisites**: Complete Platformer Sessions 1-2 first.

**Implementing the Combat System**

1. **Wire Hitbox Activation**
   ```csharp
   // In attack animation or state machine
   void StartAttack(AttackConfig config)
   {
       hitbox.SetAttackConfig(config);
       Invoke(nameof(ActivateHitbox), config.StartupDuration);
   }

   void ActivateHitbox()
   {
       hitbox.ActivateForDuration(attackConfig.ActiveDuration);
   }
   ```

2. **Subscribe to Combat Events**
   ```csharp
   void OnEnable()
   {
       EventBus.Subscribe<HitDetectedEvent>(OnHitDetected);
       EventBus.Subscribe<KnockbackAppliedEvent>(OnKnockback);
   }

   void OnHitDetected(HitDetectedEvent evt)
   {
       // Spawn hit particles at contact point
       SpawnEffect(evt.Context.ContactPoint);
   }
   ```

3. **Implement Combat Feedback**
   - Camera shake on hit
   - Hit particles
   - Sound effects
   - Hitstop (freeze frames)

---

## Intersection Points

Both tracks work together. Here's how responsibilities split:

| Feature | Designer Does | Coder Does |
|---------|---------------|------------|
| **Movement Speed** | Tune `maxSpeed` value | Implement velocity application |
| **Jump Height** | Tune `jumpForce` value | Implement physics + gravity |
| **Attack Damage** | Create AttackConfig, set values | Wire hitbox to state machine |
| **Knockback Feel** | Tune force/angle/duration | Apply physics, handle state |
| **Hitbox Size** | Adjust collider in Scene | Create hitbox prefab structure |
| **Level Layout** | Place platforms, enemies | Implement interactable logic |

### Workflow Example: Adding New Attack

1. **Coder**: Creates hitbox child object, adds Hitbox component
2. **Designer**: Creates AttackConfig asset, assigns to hitbox
3. **Coder**: Wires attack input to hitbox.Activate()
4. **Designer**: Tunes damage, timing, knockback in Inspector
5. **Coder**: Adds visual feedback (particles, sound)
6. **Designer**: Tests and iterates on values

---

## Session Progression

### Standard Path (All Students)
```
Session 1: Horizontal movement
Session 2: Jump mechanics
Session 3: Dash ability
Session 4: Wall mechanics
Session 5: Forgiveness systems
Session 6: Polish and juice
```

### Combat Track (Advanced Students)
```
Session 3+: Combat foundation (parallel to standard)
           - Basic hitbox/hurtbox
           - Light attack implementation

Session 5: Combat integration
           - Merge combat branch
           - Air attacks
           - Knockback physics
```

**Entry Requirement**: Complete Session 2 (working jump) before starting Combat Track.

---

## File Ownership (Pairs)

To prevent merge conflicts:

| Folder | Owner |
|--------|-------|
| `Scripts/` | Coder |
| `Configs/` | Designer |
| `Prefabs/` (structure) | Coder |
| `Prefabs/` (values) | Designer |
| `Scenes/` | Designer |

See `.github/CONTRIBUTING.md` for detailed workflow.

---

## Getting Started Checklist

### Designer Track
- [ ] Open and explore a MovementConfig asset
- [ ] Change a value and test in Play mode
- [ ] Create a new config via right-click menu
- [ ] Understand Header/Tooltip/Range attributes

### Coder Track
- [ ] Read `ARCHITECTURE_OVERVIEW.md`
- [ ] Understand ServiceLocator pattern
- [ ] Understand EventBus pattern
- [ ] Subscribe to an event and log output

### Combat Track (Either Role)
- [ ] Complete Session 2 prerequisites
- [ ] Read Combat section of Architecture Overview
- [ ] Understand ICombatant interface
- [ ] Create first AttackConfig asset
