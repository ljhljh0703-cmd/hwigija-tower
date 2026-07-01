#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
cd "$REPO_ROOT"

python3 Tools/VoxelTurnaround/voxel_turnaround.py demo \
  --out Tools/VoxelTurnaround/output/demo_obelisk \
  --resolution 48

python3 Tools/VoxelTurnaround/voxel_turnaround.py verify \
  --run Tools/VoxelTurnaround/output/demo_obelisk
