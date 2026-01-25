# Session Plan: Precision Platformer Project

## Course Structure

**Format:** 2.5 hour sessions
- 20 min lecture
- 45 min work
- 15 min break
- 20 min lecture
- Remaining time: work

**Teams:** 2 students (1 Programmer focus, 1 Artist focus — both code and design)

---

## Session Overview

| Session | Focus | Lecture Topics |
|---------|-------|----------------|
| 1 | Setup & Foundations | Git basics, template architecture, input systems |
| 2 | Core Mechanic | Extending existing systems, state management |
| 3 | Integration | Animation state machines, connecting code to art |
| 4 | Juice & Polish | Screen shake, particles, visual feedback |
| 5 | Tuning & Testing | Parameter tuning, playtesting methodology |
| 6 | Presentation | Live demos |

---

## Session 1: Setup & Foundations

### Lecture 1 (20 min): Git & Collaboration
- What is version control and why it matters
- Clone, branch, commit, push workflow
- Lab machine constraints (no GitHub Desktop)
- Live demo: clone template, create branch

### Work Block 1 (45 min)
- Both teammates: Git setup, clone repo, create team branch
- Verify template runs in Unity

### Lecture 2 (20 min): Template Architecture
- ServiceLocator pattern — what and why
- InputReader/PlayerController separation
- ScriptableObject configs for tuning
- Code walkthrough: trace a jump from input to physics

### Work Block 2 (remaining)
- Read all template code files
- Test template with keyboard and controller
- Discuss and choose mechanic as team
- First commit checkpoint

---

## Session 2: Core Implementation

### Lecture 1 (20 min): Extending the Controller
- How to add a new movement mechanic
- Creating a new controller script
- Creating config ScriptableObjects
- Exposing state for other systems

### Work Block 1 (45 min)
- **Programmer:** Begin mechanic implementation
- **Artist:** Set up Animator Controller, begin idle/run animations

### Lecture 2 (20 min): Animation Fundamentals for Programmers
- Animator parameters and how code drives them
- State machine basics
- Why animation timing matters for feel

### Work Block 2 (remaining)
- **Programmer:** Continue mechanic, get basic version working
- **Artist:** Complete idle, run, jump, fall animations
- Commit checkpoint

---

## Session 3: Integration

### Lecture 1 (20 min): Connecting Animation to Code
- Creating a PlayerAnimator script
- Caching animator parameter hashes
- Driving parameters from gameplay state
- Transition conditions in the Animator

### Work Block 1 (45 min)
- Both: Set up PlayerAnimator, connect basic states
- Test animation transitions match gameplay

### Lecture 2 (20 min): Mechanic-Specific Animation
- Timing windows: when does animation start vs mechanic?
- Anticipation and follow-through
- Communicating state clearly to the player

### Work Block 2 (remaining)
- **Artist:** Mechanic-specific animations
- **Programmer:** Expose all mechanic states, test integration
- Commit checkpoint

---

## Session 4: Juice & Polish

### Lecture 1 (20 min): What is Juice?
- Feedback loops: action → feedback → satisfaction
- Screen shake fundamentals
- Particle systems for movement
- Video reference: "Juice It or Lose It"

### Work Block 1 (45 min)
- **Programmer:** Implement camera shake, integrate with mechanics
- **Artist:** Create particle effects (dust, impact, trails)

### Lecture 2 (20 min): Squash, Stretch, and Timing
- The 12 principles in game animation
- Scale-based juice (code-driven)
- Landing impact and anticipation
- When juice is too much

### Work Block 2 (remaining)
- Integrate particles and shake into gameplay
- Input tuning: test deadzone, buffer values
- Commit checkpoint

---

## Session 5: Tuning & Testing

### Lecture 1 (20 min): The Tuning Process
- One variable at a time
- Feel targets: tight vs floaty, weighty vs snappy
- Reference games and why they feel the way they do
- Config organization for rapid iteration

### Work Block 1 (45 min)
- Pair tuning: one plays, one adjusts values
- Decide on a feel target, tune toward it

### Lecture 2 (20 min): Playtesting
- How to run a playtest (watch silently)
- What to look for: confusion, failed inputs, missed communication
- Bug vs design issue

### Work Block 2 (remaining)
- Cross-team playtesting
- Bug fixing and final polish
- Final commit

---

## Session 6: Presentations

### Format
- 5-7 minutes per team
- Live demo (no slides)
- Technical and art highlights
- Q&A

### Schedule
- Brief intro (5 min)
- Team demos (5-7 min each)
- Wrap-up discussion

---

## Lecture Resources

### Session 1
- Git Basics (Atlassian): https://www.atlassian.com/git/tutorials

### Session 2
- Celeste & TowerFall Physics (Matt Thorson): https://www.youtube.com/watch?v=yorTG9at90g

### Session 4
- Juice It or Lose It: https://www.youtube.com/watch?v=Fy0aCDmgnxg
- The Art of Screenshake: https://www.youtube.com/watch?v=AJdEqssNZ-U

### Session 5
- Game Feel by Steve Swink (book reference)

---

## Assessment Checkpoints

| Session | What to Check |
|---------|---------------|
| 1 | Git working, template running, mechanic chosen |
| 2 | Mechanic partially functional, base animations exist |
| 3 | Animations connected to gameplay state |
| 4 | Juice elements integrated (particles, shake) |
| 5 | Tuned feel, playtested, bugs fixed |
| 6 | Live demo, can explain decisions |
