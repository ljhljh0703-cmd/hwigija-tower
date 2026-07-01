# Voxel Turnaround Tool

검증 목적의 2D -> voxel -> 8방향 sprite prepass 도구입니다.

이 도구는 Unity 런타임 에셋을 직접 만들지 않습니다. `front.png`와 `side.png`를 받아 실루엣 기반 visual hull을 만들고, 게임 에셋 파이프라인에 넘길 수 있는 검증용 PNG 렌더와 `manifest.json`을 생성합니다.

## 왜 여기 있나

기존 pixel-art/sprite pipeline을 대체하지 않고, 선택적 prepass로 붙이기 위한 실험입니다.

```text
front/side concept image
  -> voxel turnaround prepass
  -> 8-direction reference PNGs + manifest.json
  -> PerfectPixel / sprite-gen / Aseprite refinement
  -> sprite-sheet.png + engine manifest
```

## 설치

외부 패키지 없이 Python 표준 라이브러리만 사용합니다.

```bash
python3 --version
```

## 빠른 검증

```bash
cd "/Users/godju/Downloads/AI Game/hwigi-tower"
./Tools/VoxelTurnaround/run_demo.sh
```

또는 수동으로:

```bash
cd "/Users/godju/Downloads/AI Game/hwigi-tower"
python3 Tools/VoxelTurnaround/voxel_turnaround.py demo --out Tools/VoxelTurnaround/output/demo_obelisk --resolution 48
python3 Tools/VoxelTurnaround/voxel_turnaround.py verify --run Tools/VoxelTurnaround/output/demo_obelisk
```

성공하면 다음 파일이 생깁니다.

```text
Tools/VoxelTurnaround/output/demo_obelisk/
├── sources/front.png
├── sources/side.png
├── renders/dir_00_front.png
├── renders/dir_01_front_right.png
├── ...
├── turnaround_sheet.png
└── manifest.json
```

## 실제 입력으로 빌드

```bash
python3 Tools/VoxelTurnaround/voxel_turnaround.py build \
  --front path/to/front.png \
  --side path/to/side.png \
  --out Tools/VoxelTurnaround/output/my_prop \
  --asset-id my_prop \
  --resolution 64

python3 Tools/VoxelTurnaround/voxel_turnaround.py verify --run Tools/VoxelTurnaround/output/my_prop
```

입력 이미지는 투명 배경 PNG가 가장 안전합니다. 흰 배경 PNG도 corner color를 배경으로 추정해 처리합니다.

## 합격 기준

`verify`는 다음을 확인합니다.

- `manifest.json` 존재
- occupied voxel 수가 0이 아님
- 8방향 render PNG가 모두 존재
- 8방향 contact sheet가 존재
- 각 render PNG가 읽히고 non-transparent pixel을 포함
- manifest의 render SHA-256이 실제 파일과 일치

## 사용 판단

적합:
- 4방향/8방향 static prop
- 상자, 제단, 문, 탑 조각, 보스 장식물
- 방향별 identity drift를 줄이기 위한 reference render

부적합:
- 작은 단일 정면 아이콘
- rigging-ready 3D 모델
- 손으로 찍는 편이 빠른 16x16 단순 소품
- 애니메이션 프레임 자체 생성

## 현재 한계

- silhouette visual hull이라 오목한 구조는 복원하지 못합니다.
- front/side 두 장만 사용합니다.
- 렌더는 검증용 pixel projection입니다. 최종 아트 품질은 Aseprite/PerfectPixel 후처리가 필요합니다.
- PNG reader는 non-interlaced 8-bit grayscale/RGB/RGBA 입력만 지원합니다.
