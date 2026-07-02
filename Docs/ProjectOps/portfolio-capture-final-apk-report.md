# Portfolio Capture Final APK Report

## Status

- PLAYTEST_STATUS: `CAPTURE_APK_READY_FOR_USER_CHECK`
- Content commit: `185f794a6fd7fdf7a920bd952b4b4afe52676f5a`
- Source status record commit: pending docs-only record
- APK path: `/Users/godju/Downloads/AI Game/hwigi-tower/Builds/Android/hwigi-tower-185f794-portfolio-capture-final-20260702.apk`
- SHA-256: `836850eafd2c8bffae7b04b71cfd0562a3de53ca0131c72d404743e5aca57041`
- Size: about 113 MB

## Verification

- Clean worktree base: `/private/tmp/hwigi-portfolio-capture-final`
- Main dirty worktree: not used as source input
- CodeGraph: initialized and indexed current clean worktree
  - Files: 142
  - Nodes: 10,222
  - Edges: 25,327
  - Status: index up to date
- Android APK build: PASS
- `apksigner verify --verbose`: PASS
  - v2 scheme: true
  - signers: 1
- `aapt dump badging`: PASS
  - package: `com.godju.hwigitower`
  - label: `회귀자는 탑을 오른다`
  - minSdk: 25
  - targetSdk: 36
- ABI check: PASS
  - `lib/arm64-v8a/*` present
- `git diff --check`: PASS after Unity-generated local files were restored
- Forbidden runtime scope: no additional code changes after `185f794`

## Test Runner Note

Unity batch test runner accepted `-runTests` / `-listTests` and exited with code 0, but did not emit the requested XML result file in this environment.

This is not counted as targeted EditMode PASS. It is recorded as `N/A / tool-output unavailable`.

The build and package gates are still sufficient for a user capture check, but the APK is not marked user-accepted until the user confirms the screen flow.

## Capture Checklist

Use this APK for visual smoke:

1. New Run to Floor 1 map.
2. Combat node into active combat.
3. Confirm combat HUD readability: enemy HP, player HP, Mataios HP, compact log, separate player/Mataios action attribution.
4. Event node: no raw labels such as `MoralChoice`, `CHOICE_`, `ENC_`, `[current]`, `[locked]`.
5. Reward/growth popup: central reward rows and one clear CTA.
6. Shop, if routed: item cards with name/effect/price/current Gold/disabled reason.

## Remaining Risk

- Actual Android device visual smoke was not run by Codex.
- Unity CLI test result XML was not produced after Hub login; do not claim targeted tests passed.
- This is a capture candidate, not `Accepted`, until the user confirms the footage/screens.
