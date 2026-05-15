# Dev Handoff — Unity Retry / Lobby QA Blocker — 2026-05-15

## Context
- User requested QA/playtest from `origin/Proto`.
- Repo was already up to date at `c4cad87` (`Fix lobby portrait menu layout`).
- Initial QA report was created before Unity could open; first attempt failed at Unity LicenseClient timeout.
- User asked to retry Unity. Retry succeeded, changing the active blocker.

## Current State
- Unity version: `6000.4.3f1`
- Branch: `Proto`
- Commit: `c4cad87`
- Platform target shown in Editor title: Android
- Scene opened: `Assets/_Project/Scenes/Lobby.unity`
- Play Mode: entered successfully
- Manual QA status: blocked at first screen because Game View stayed black

## What Succeeded
- Unity Editor launched from:
  `/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity`
- LicenseClient connected on retry.
- Unity Personal entitlement resolved.
- Access token updated successfully.
- Project loaded in about 16.6s.
- Script compilation/domain reload completed without visible compile errors in the captured log.
- `Lobby.unity` opened successfully.
- Pressing Play entered Play Mode.

## What Failed / Blocked
| priority | id | issue | observed |
|---|---|---|---|
| P0 | P0-LOBBY-001 | Lobby Play Mode renders black Game View | In Play Mode, the Game View area stayed black. No Lobby title, background, buttons, profile chip, settings button, or continue state were visible. |

## Observed Editor State
- Scene title: `Lobby - hwigi-tower - Android - Unity 6.4 (6000.4.3f1) <Metal>`
- Hierarchy during Play Mode showed:
  - `Lobby`
  - `Main Camera`
  - `Lobby Runtime`
  - `DontDestroyOnLoad`
- Game View toolbar showed Free Aspect, scale 1x.
- No visible player-facing UI appeared in Game View.

## Important Correction
The previous `P0-ENV-001` LicenseClient blocker is no longer the main blocker. It happened on the first attempt, but the retry succeeded. Keep it as a watch item only.

Current blocking issue is the black Lobby Game View after Play Mode entry.

## Likely Investigation Targets
Start with runtime presentation, not package/licensing:

1. `LobbyController` UI creation path
   - Confirm `Awake`, `Start`, or fallback `Update` is running in Play Mode.
   - Confirm `EnsureRuntimeUi` or equivalent creates `Lobby Canvas`, safe-area root, background, title, and buttons.
   - Add temporary log or use Inspector to confirm active GameObjects.

2. Canvas / camera relationship
   - Check Canvas render mode.
   - If Screen Space Camera or World Space, verify camera reference, plane distance, culling mask, and sorting.
   - If Screen Space Overlay, verify Canvas active/enabled and alpha is not zero.

3. Camera / Game View
   - Verify `Main Camera` clear flags/background and culling mask.
   - Verify no black full-screen object or panel is covering UI.
   - Verify UI layer is included.

4. Presentation data references
   - Verify `SO_LobbyPresentationData` is assigned to `LobbyController`.
   - Verify missing background/logo assets do not abort UI creation.
   - Check null-safe branches around background and title creation.

5. Enter Play Mode settings / domain reload fallback
   - Recent progress says fallback was added for scene reload/enter play mode differences.
   - Verify that fallback does not early-return because `_uiCreated` or runtime references are stale.

## QA Flow To Resume After Fix
1. Open `Lobby.unity`.
2. Set Game View to `1080x1920` portrait if possible.
3. Press Play.
4. Confirm Lobby background/title/menu visible.
5. Check:
   - New Game button visible and clickable.
   - Continue disabled or summary state clear.
   - Profile opens/closes.
   - Settings opens/closes.
6. Click New Game.
7. Continue original QA path:
   Lobby -> New Game -> Floor map -> Event -> Combat -> Rest input -> Shop -> Boss -> Save -> Lobby -> Continue -> Ending if reachable.

## QA Design Notes Already Captured
See `Docs/QA/QA_Report_2026-05-15.md`.

High-priority design gaps after the P0 render bug:
- Node map needs reward/risk previews and stronger boss convergence.
- Combat needs enemy intent and action previews.
- Rest needs stronger Mataios response/effect presentation.
- Shop disabled states need price/current gold/effect/reason.
- Ending choice needs a short writer-approved pre-choice framing panel.

## Non-Goals
- Do not rewrite game design while fixing this blocker.
- Do not introduce new dependencies.
- Do not generate final Mataios lore/dialogue ad hoc.
- Keep any debug text/logging temporary unless it is behind an explicit debug toggle.
