# Vertical Slice Test Plan v0.1

## Scope

- Unity was not available for this planning session.
- This plan defines the next dev validation pass after UI presentation/cutscene work.
- Required runtime path remains deterministic fallback/cache for AI until model acceptance passes.

## EditMode Plan

| test | purpose | setup | expected |
|---|---|---|---|
| `PresentationDataLookup_FallsBackWhenStableIdMissing` | Presentation data lookup fallback | Request missing encounter/background/portrait/SFX key. | Returns configured fallback or silent no-op; no exception; route logic unaffected. |
| `ResultSummary_DoesNotDuplicateRewards` | Result summary can be regenerated safely | Resolve Shop or Combat victory once; request summary multiple times. | Gold/item/memory/reward state is unchanged after summary generation. |
| `CutsceneTrigger_UsesStableId` | Cutscene trigger is stableId-based | Register cutscene for `CHOICE_MEMORY_01_UNLOCK`; use display text/textKey changes. | Trigger still resolves from stableId; display copy changes do not break cutscene. |
| `RawKeyDisplayToggle_DefaultOff` | Raw key display hidden in recording mode | Create HUD/presentation config with default settings. | `PLACEHOLDER_*`, `ENC_*`, `CHOICE_*` raw strings are not returned for recording labels. |
| `CombatSummary_HidesZeroDeltas` | Combat result density | Build summary with gold/glitch/affinity zero. | Zero deltas are omitted; nonzero deltas appear. |
| `SkillDisabledReason_IsPresentationOnly` | Skill remains disabled without invoking action | Build combat HUD state with no ability route. | Skill disabled reason displays or is hidden per config; no combat action is invoked. |

## PlayMode Plan

| test | purpose | expected |
|---|---|---|
| `PrototypeRoom_LoadsRuntimeAndPlayer` | Existing scene smoke | PrototypeRoom, player, controller, HUD load. |
| `PrototypeRoom_RouteCompletesDefaultDemoPath` | Full route | Shop -> Moral -> Memory -> CombatGate -> DemoComplete completes. |
| `PrototypeRoom_EncounterPresentationSlotsVisible` | Presentation slot mapping | Each encounter displays expected background/portrait slot or documented fallback. |
| `PrototypeRoom_MemoryUnlockCutsceneThenRouteProgresses` | Memory cutscene | Memory unlock cutscene appears once, then route advances to CombatGate. |
| `PrototypeRoom_CombatGateCutsceneThenCombatPanel` | Combat start cutscene | Combat start cutscene ends before Attack/Defend become interactable. |
| `PrototypeRoom_CombatVictoryThenDemoCompleteCutscene` | Completion cutscene | Combat panel closes, DemoComplete cutscene/overlay appears once. |
| `PrototypeRoom_RawKeysHiddenByDefault` | Recording UI cleanliness | No visible `PLACEHOLDER_*`, `ENC_*`, `CHOICE_*` in default recording mode, except explicit debug overlay if enabled. |
| `PrototypeRoom_DebugModeCanShowRawKeys` | Dev diagnosability | Enabling debug config restores raw ids/keys for QA. |

## 1080 x 1920 Screenshot QA

Capture after implementation:

1. Shop with portrait.
2. Moral choice.
3. Memory unlock cutscene.
4. Memory after unlock.
5. Combat start cutscene.
6. Active CombatGate after one Attack.
7. Active CombatGate after one Defend.
8. DemoComplete after victory.

Pass criteria:

- No oversized world/debug labels.
- Portrait does not overlap route/stats/choices/result/combat panel.
- Raw keys/stableIds hidden by default.
- Combat panel and DemoComplete never overlap.
- HP bars/text/damage feedback fit without shifting buttons.
- Cutscene overlays do not hide required next action unless they intentionally block input.

## Required Validation Commands

| validation | command | this Unityless session |
|---|---|---|
| whitespace | `git diff --check` | should run |
| fresh EditMode | Unity EditMode test runner | cannot run without Unity |
| fresh PlayMode | Unity PlayMode test runner | cannot run without Unity |
| forbidden search | search for forbidden patterns in `Docs design Assets` | should run, `rg` may need fallback if blocked |

Forbidden patterns:

- `escapePolicy`
- `onEscape`
- `escapeTextKey`
- `onEscapeEffects`
- `DateTime.Now`
- `DateTime.UtcNow`
- `UnityEngine.Random`
- `Random.Range`

Allowed report hits:

- Policy docs explaining forbidden deterministic patterns.
- Existing validator tests for escape policy rejection.
- Deterministic wrapper usage if not `UnityEngine.Random`.

## Release Gate

W2 vertical slice is recording-ready only when:

- Screen-by-screen spec is implemented or explicitly waived.
- Asset manifest rows needed for recording are imported or marked as acceptable silent/visual fallback.
- Combat feel P0 acceptance passes.
- Writer v0.2 decisions are answered.
- EditMode, PlayMode, and 1080 x 1920 screenshot QA pass on a Unity machine.
