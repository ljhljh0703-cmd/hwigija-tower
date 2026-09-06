# Android Candidate QA Plan v0.1

## Scope

- Baseline: `origin/Proto` `d94697f`.
- This document is a command/report format and checklist only.
- Unity was not executed in this outsource session.
- Runtime/SO changes are not included here.

## Known Baseline

| Area | Evidence | Current call |
|---|---|---|
| Build harness | `Assets/_Project/Editor/BuildScript.cs` builds `Builds/Android/hwigi-tower.apk`. | Present. |
| Android support | `Docs/AI_NPC_MODEL_SPEC.md` and readiness docs still list Android Build Support missing. | Blocked until verified. |
| Portrait/mobile target | GDD D-003 locks mobile portrait one-hand play; D-018 prioritizes Android APK. | Required. |
| Model runtime | Android model artifact/eval pending. | Smoke can use deterministic fallback; model acceptance is separate. |

## Preflight Report Format

| Field | Value |
|---|---|
| QA date | `<YYYY-MM-DD>` |
| Machine | `<host name>` |
| Unity editor version | `<version>` |
| Android Build Support installed | `<yes/no>` |
| Android SDK/NDK/JDK resolved by Unity | `<yes/no/path>` |
| Git commit | `<commit hash>` |
| Branch | `<branch>` |
| Model mode | `<deterministic fallback / on-device model>` |
| Device/emulator | `<model / OS / resolution>` |
| APK path | `Builds/Android/hwigi-tower.apk` |
| Unity log path | `<log path>` |

## Commands for Dev Session

> Do not run these in this outsource session. They are for the Unity/build owner.

```powershell
where.exe Unity
```

```powershell
Unity -batchmode -projectPath . -executeMethod HwigiTower.EditorTools.BuildScript.BuildAndroid -quit -logFile Logs/android-build.log
```

```powershell
adb install -r Builds/Android/hwigi-tower.apk
```

```powershell
adb logcat -c
adb logcat -v time > Logs/android-first-run-logcat.txt
```

## Build Support Checklist

| ID | Check | Pass condition | Report field |
|---|---|---|---|
| AND-PRE-01 | Unity executable found. | Command resolves a Unity path. | `Unity path` |
| AND-PRE-02 | Android Build Support installed. | Build harness does not fail with "Android build target is not installed". | `Android Build Support installed` |
| AND-PRE-03 | APK generated. | `Builds/Android/hwigi-tower.apk` exists and non-zero size. | `APK size` |
| AND-PRE-04 | No runtime compile errors. | Unity build log has no C# compile failure. | `Build result` |
| AND-PRE-05 | No final prose leak. | Build log and first-run UI show placeholder/approved text only. | `Content gate` |

## Device Smoke Checklist

| ID | Scenario | Steps | Pass condition |
|---|---|---|---|
| AND-SMOKE-01 | First launch | Install APK, launch app. | App opens without crash, first scene reaches PrototypeRoom or approved start screen. |
| AND-SMOKE-02 | Portrait | Rotate device or inspect launch orientation. | Game remains portrait; UI does not require landscape. |
| AND-SMOKE-03 | Safe area | Check top/bottom gesture/status areas. | Main controls and text are not clipped by notch/status/nav bars. |
| AND-SMOKE-04 | Touch target | Tap Shop choices, combat buttons, restart button. | Buttons respond reliably with one-hand portrait touch. |
| AND-SMOKE-05 | Shop | Buy item, buy ability if affordable, leave. | Known refs grant expected item/ability or show unavailable state without blocking route. |
| AND-SMOKE-06 | Combat | Use Attack, Defend, Skill. | HP/damage/result feedback makes each action legible within 3 seconds. |
| AND-SMOKE-07 | Memory | Unlock a memory fragment. | Placeholder key or approved public label appears; raw debug route text is hidden. |
| AND-SMOKE-08 | BossGate | Reach Floor 2 BossGate or later configured gate. | Clear/failure/Recall Anchor states do not overlap. |
| AND-SMOKE-09 | Ending choice | On completion build, clear final boss and choose both branches in separate runs. | `ending.rest` and `ending.continue` are distinct states with placeholder/approved text only. |
| AND-SMOKE-10 | Restart | Restart after clear/failure/ending. | Volatile run state resets; memory/reflection retention matches policy. |

## Android Log Triage

| Severity | Pattern | Required action |
|---|---|---|
| P0 | Crash, ANR, black screen, missing scene, C# exception blocking input | Stop candidate; assign dev fix. |
| P0 | Android Build Support missing | Install support before further APK QA. |
| P0 | Raw final prose or unapproved ending/memory truth appears | Stop public build; writer gate. |
| P1 | UI overlap, touch target unreliable, portrait clipping | Fix before recording/public smoke. |
| P1 | Model init fails but deterministic fallback works | Build can continue for non-model smoke; model acceptance remains blocked. |
| P2 | Minor placeholder casing or visual polish | Track for recording polish. |

## Completion Report Template

```markdown
# Android Candidate Report

- Commit:
- Branch:
- Unity version:
- Android support installed:
- APK path:
- Device:
- Model mode:
- Result: pass / fail / blocked

## Smoke Results

| ID | Result | Notes |
|---|---|---|
| AND-SMOKE-01 |  |  |
| AND-SMOKE-02 |  |  |
| AND-SMOKE-03 |  |  |
| AND-SMOKE-04 |  |  |
| AND-SMOKE-05 |  |  |
| AND-SMOKE-06 |  |  |
| AND-SMOKE-07 |  |  |
| AND-SMOKE-08 |  |  |
| AND-SMOKE-09 |  |  |
| AND-SMOKE-10 |  |  |

## Blockers

- <pending>

## Next Actions

- <pending>
```
