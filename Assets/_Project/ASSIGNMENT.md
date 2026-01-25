# Precision Platformer – Controls & Feel

GAME 326 – Applied Principles: Programming

Professor: Tim Lindsey

Assignment Due: Session 6 (In-Class Live Demo)

---

## Assignment Overview

In this assignment, you will work from a provided Unity project template to build a precision platformer focused on input handling, movement feel, and visual feedback.

**This is not a clone exercise.**

You are not being graded on level design, art quality, or feature count. You are being evaluated on whether you understand why the provided architecture exists, how to extend it correctly, and how input flows from hardware to character movement.

**You will implement one new movement mechanic and polish it until it feels intentional.**

A mechanic that works but feels bad is incomplete. A mechanic that feels good demonstrates understanding.

**This is a team assignment (2 students).**

One programmer focus, one artist focus. Both teammates write code and both contribute to feel.

**Template Repository:**

https://github.com/profangrybeard/PlatformerTemplate

---

## Core Goal (Non-Negotiable)

Your game must include:

- Working walk and jump mechanics (provided in template)
- One new movement mechanic implemented by your team
- Animations that communicate player state
- Visual feedback (particles, screen shake, or equivalent juice)

The new mechanic must be playable and feel intentional. If it's broken or feels like an accident, the assignment is incomplete.

---

## Required Architectural Understanding

You are expected to understand, use, and extend the following systems from the template:

- **ServiceLocator** — How systems find each other
- **InputReader** — How hardware input becomes game input
- **PlayerController** — How input becomes movement
- **MovementConfig / InputConfig** — How tunable values are separated from code

You must be able to explain what each system does, why it exists, and how input flows from button press to character movement.

---

## Mechanic Options

Choose ONE new movement mechanic to implement:

| Mechanic | Description |
|----------|-------------|
| **Wall Slide & Jump** | Stick to walls, slide down slowly, jump off |
| **Dash** | Quick horizontal burst with cooldown |
| **Climb** | Grab and climb specific surfaces |
| **Double Jump** | Second jump in mid-air with distinct feel |
| **Ground Pound** | Fast fall attack with landing impact |

Other mechanics may be proposed to the instructor for approval.

---

## Content Requirements

### Code Requirements

| Requirement | Description |
|-------------|-------------|
| New mechanic script | Follows template patterns (uses ServiceLocator, InputReader) |
| New config ScriptableObject | Tunable values for your mechanic |
| Exposed state | Animator can read mechanic state (IsWallSliding, IsDashing, etc.) |

### Animation Requirements

| Requirement | Description |
|-------------|-------------|
| Core states | Idle, Run, Jump, Fall, Land |
| Mechanic states | Animations specific to your chosen mechanic |
| PlayerAnimator script | Drives Animator parameters from gameplay state |

### Juice Requirements

At least TWO of the following:

- Particle effects (dust, trails, impact)
- Screen shake on impactful actions
- Squash and stretch on jump/land
- Visual feedback for mechanic activation

---

## Rules & Constraints

### Required

- Input reading happens in `Update()`, physics in `FixedUpdate()`
- Cross-system access uses ServiceLocator
- Tunable values live in ScriptableObject configs, not hardcoded
- Both keyboard and controller input must work
- Animations respond to actual gameplay state, not faked

### Forbidden

- Hardcoded gameplay values in MonoBehaviours
- Direct references between systems that bypass ServiceLocator
- Animations that don't match gameplay state
- Builds that only work in the editor

---

## Student Learning Outcomes

By completing this assignment, students will demonstrate the ability to:

1. **Analyze and extend an existing codebase** — Read, understand, and modify a provided Unity project template while preserving architectural intent.

2. **Implement responsive input handling** — Use Unity's Input System with proper deadzone processing and input buffering.

3. **Create movement systems with good game feel** — Apply acceleration, coyote time, variable jump height, and gravity scaling.

4. **Integrate animation with gameplay** — Drive Animator parameters from code and create clean state transitions.

5. **Add visual feedback that reinforces feel** — Implement juice elements that make actions feel impactful.

6. **Collaborate effectively** — Work as a team where both members contribute to code and feel.

7. **Communicate technical reasoning** — Verbally explain system behavior, input flow, and design decisions during a live demo.

---

## Final Demo (In-Class)

There are no slides and no formal presentation.

You will:

- Launch your game
- Play it live
- Explain what is happening while the game is running

While playing, you must be able to explain:

- How input flows from button press to character movement
- How your new mechanic works and why you made specific choices
- How animations connect to gameplay state
- One architectural decision you now understand because of this project

**This is a live technical demo, not a speech.**

---

## Grading Rubric

| Component | Points | Description |
|-----------|--------|-------------|
| **Git Usage** | 10 | Regular commits, meaningful messages, both teammates committing |
| **Core Mechanic** | 25 | New mechanic works correctly and feels intentional |
| **Animation Integration** | 20 | Animations respond to gameplay state, transitions are clean |
| **Juice & Polish** | 20 | Particles, screen shake, squash/stretch — the details |
| **Code Quality** | 15 | Follows template patterns, proper architecture |
| **Presentation** | 10 | Clear demo, can explain input flow and decisions |
| **Total** | 100 | |

---

## Project Delivery Instructions

You are responsible for delivering a complete, runnable project.

### Required Deliverables

You must:

- ✅ Push all code to your team branch on GitHub
- ✅ Ensure project runs from a fresh clone
- ✅ Participate in the live in-class demo

**If it only works on your machine, it is not finished.**

### Common Submission Errors (Avoid These)

- Missing ScriptableObject assets (configs not saved)
- Input Actions asset not included or not assigned
- Code that only works in the editor
- Mechanic that is broken or feels accidental
- Animations that don't match gameplay state
- Only one teammate's commits in the repo

These issues indicate lack of testing or incomplete work.

---

## Resources

- [Unity Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Celeste & TowerFall Physics (GDC Talk)](https://www.youtube.com/watch?v=yorTG9at90g)
- [The Art of Screenshake](https://www.youtube.com/watch?v=AJdEqssNZ-U)
- [Juice It or Lose It](https://www.youtube.com/watch?v=Fy0aCDmgnxg)

For Git help, see **GIT_GUIDE.md** in this repository.

---

## Questions?

Ask your instructor or post in the course Discord.
