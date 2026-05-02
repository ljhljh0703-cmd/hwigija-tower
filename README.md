# hwigi-tower

Unity 6 portrait mobile roguelike prototype for `회귀자는 탑을 오른다`.

## Project Root

This repository root is the Unity project root.

```text
/Users/godju/Downloads/AI Game/hwigi-tower
```

Do not reintroduce a nested Unity project directory. `Assets/`, `Packages/`, and `ProjectSettings/` must stay at the repository root.

## Validation

Run EditMode tests from the repository root:

```bash
/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults editmode-results.xml -logFile unity-editmode.log
```

Run the Android build harness:

```bash
/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity -batchmode -projectPath . -executeMethod HwigiTower.EditorTools.BuildScript.BuildAndroid -quit -logFile unity-android-build.log
```

## Repository Hygiene

Tracked:
- `Assets/`, including `.meta` files
- `Packages/manifest.json` and `Packages/packages-lock.json`
- `ProjectSettings/`
- `.editorconfig`, `.gitignore`, `.gitattributes`, `AGENTS.md`

Ignored:
- `Library/`, `Logs/`, `UserSettings/`, `Temp/`, `Obj/`
- generated `.csproj` files
- Unity test result XML and local Unity logs
- built `.apk`/`.aab` artifacts
