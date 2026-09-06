# hwigi-tower

Unity 6 portrait mobile roguelike prototype for `회귀자는 탑을 오른다`.

## 이 저장소에서 제가 직접 작성한 것

`Tools/` 의 검사기와 `Docs/Portfolio/assets/concept-to-ui/` 의 화면 사양 파일입니다.
게임 코드의 상당 부분은 코딩 에이전트가 작성했고, 저는 사양과 검사기로 판정해 병합했습니다.

처음에는 판정 없이 맡겼습니다. 그 결과 빌드가 계속 통과하는데도 전투 노드를 누르면
이벤트가 열리고, 이벤트를 고른 뒤 화면이 맵으로 돌아오지 않는 상태가 남아 있었습니다.
아래 검사기들은 그 뒤에 하나씩 생겼고, 각 파일 첫머리에 어떤 실패 때문에 생겼는지가
날짜와 함께 적혀 있습니다.

| 파일 | 검사하는 것 |
|---|---|
| `Tools/check_layout.py` | 화면 사양 대조. 겹침 금지, 최소 크기, 층 순서, 필수 항목 등 규칙 13종 |
| `Tools/check_wiring.py` | 계약이 화면에 실제로 배선됐는지. 사양 통과와 화면 반영은 다른 문제입니다 |
| `Tools/check_databinding.py` | 사양이 가리키는 데이터가 실제 C# 멤버로 존재하는지 |
| `Tools/check_mapflow.py` | 맵 흐름 덤프를 계약과 대조 |
| `Tools/check_color_tokens.py` | 런타임 코드가 색 토큰을 우회하는지 |

`Tools/DumpLayout/` 과 `*LayoutContract.cs` 는 화면 배치값을 Unity 타입에서 떼어내,
에디터 라이선스 없이도 검증이 돌게 만든 부분입니다.
`Docs/Portfolio/assets/concept-to-ui/` 의 사양 JSON 이 기준이고 코드가 파생입니다.

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
