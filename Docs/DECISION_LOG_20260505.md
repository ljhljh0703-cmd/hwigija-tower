# Project Decision Log — 2026-05-05

## Context
Demo loop is mostly functional. Focus is now on combat vertical slice and AI model intake for the 8-day demo.

## Decisions

### 1. AI Model Intake (OQ-004)
- **Primary Engine**: MLC-LLM (aligned with GDD §8.2).
- **Demo Fallback**: If on-device model integration is delayed, use a **deterministic fake/cache** layer in `OnDeviceLLMProvider`.
- **Candidate Models**: HCX-SEED 0.5B (Primary), Qwen2.5 1.5B (Fallback).

### 2. Combat Vertical Slice
- **Combat Form**: **B. Mini-combat with 2 buttons**.
- **Actions**:
    - **Attack**: Normal damage roll vs normal damage roll.
    - **Prepare**: Deal 0 damage, take 50% reduced damage. (Aligned with `ABILITY_SCOUT` intent).
- **Demo Enemy**: `ENEMY_FRACTURE_HOUND` (HP: 12, ATK: 3).
- **Results**:
    - **Victory**: Gold +7, Glitch -2, Affinity +2.
    - **Defeat**: HP -5, Glitch +5, Affinity -2.
    - **Death**: Treated as optional risk; demo-safe handling.

### 3. Documentation Integration
- Integrated `Juho/Codex` documents (v0.1) for demo review script, memory fragment spec, and reaction key alignment.
- **OQ-006 (Memory Text)**: Remains open. Use placeholders `PLACEHOLDER_MEM_FRAGMENT_01_TITLE/BODY`.

### 4. Technical Implementation
- `CombatController`: Updated to support `CombatAction` (Attack/Prepare).
- `PrototypeRunState`: 
    - Added `IsInCombat` interactive state.
    - Added `ResolveCombatRoundInteractive(action)`.
    - Added `AutoResolveCombat` flag for test compatibility.
- `PrototypeRunSnapshot`: Updated to expose combat state to UI.

## Next Steps
- Implement UI buttons for Attack/Prepare in `CombatGate` view.
- Verify NPC reflection connection after combat results.
- Finalize `ABILITY_SCOUT` actual effect binding.
