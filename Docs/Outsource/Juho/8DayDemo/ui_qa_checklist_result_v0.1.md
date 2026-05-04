# UI QA Checklist Result v0.1

## Environment

| item | result |
|---|---|
| Branch | `Juho/Codex` |
| Required base commit | `0357d75 Add deterministic demo run progression` present |
| Unity project version | `6000.4.3f1` from `ProjectSettings/ProjectVersion.txt` |
| Unity executable | Not found in the local Windows PATH or default Hub folder |
| .NET SDK | Not installed; `dotnet build` could not run |

## Manual/Static QA Results

| area | result | notes |
|---|---|---|
| Portrait screen | Pass by code review | HUD text is split into multiple wrapped lines and runtime canvas remains 1080x1920 reference resolution. |
| Choice button | Pass by code review and test coverage update | Button height remains 96px, 2-3 choices are asserted by PlayMode smoke test. |
| Disabled choice | Pass by code review and test coverage update | DisabledVisible reason key remains appended to disabled button labels. |
| Result panel | Pass by code review and test coverage update | Result messages include Gold, Affinity, Glitch, item, memory, and combat refs. |
| Memory unlock | Pass by code review and test coverage update | `MEM_FRAGMENT_01` appears in result and memory panel assertions. |
| Combat handoff | Pass by code review and test coverage update | `COMBAT_GATE_01` and `ENEMY_FRACTURE_HOUND` are asserted after handoff. |
| Demo complete | Pass by code review and test coverage update | `demo.complete` remains asserted after the combat gate result. |

## Test Commands

| command | result | notes |
|---|---|---|
| `git diff --check` | Pass | Only line-ending warnings were reported by Git; no whitespace errors. |
| forbidden search | Pass | Case-sensitive search over `Assets` and `Docs` returned no matches. |
| EditMode | Not run | Unity executable not available in this environment. |
| PlayMode | Not run | Unity executable not available in this environment. |
| `dotnet build hwigija-tower.slnx --no-restore` | Fail: environment | Local machine has .NET runtime but no SDK. |

## PlayMode Items to Verify in Unity

- [ ] `PrototypeRoom` loads.
- [ ] First available demo node is Shop.
- [ ] Shop choices show 2-3 buttons.
- [ ] Gold-gated choice is disabled and shows `PLACEHOLDER_REASON_NOT_ENOUGH_GOLD`.
- [ ] Moral choice result shows Affinity/Glitch deltas.
- [ ] Memory unlock result shows `MEM_FRAGMENT_01`.
- [ ] Memory panel shows titleKey/bodyKey placeholders or `-` if unavailable.
- [ ] CombatGate choice shows combat started state.
- [ ] Enemy stableId appears for combat gate.
- [ ] DemoComplete panel appears after the route completes.

## Found Bugs

| id | status | detail |
|---|---|---|
| QA-UI-001 | Open | Unity executable is not installed or not discoverable on this machine, so fresh EditMode/PlayMode could not be executed. |
| QA-UI-002 | Watch | The memory/combat panel now has multiple lines; verify no overlap on narrow Android devices. |
