#!/bin/bash
# Unity 라이선스 활성 여부만 1분 안에 판정한다.
# 회귀탑 본 프로젝트(1.3GB)를 열지 않는다 — 빈 프로젝트로 라이선스만 묻는다.
set -u
U="/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity"
[ -x "$U" ] || { echo "FAIL: 에디터 없음 → $U"; ls /Applications/Unity/Hub/Editor/ 2>/dev/null; exit 2; }

P="/tmp/unity-license-probe-$$"
mkdir -p "$P/Assets"
echo "probe: $P"
"$U" -batchmode -quit -nographics -projectPath "$P" -logFile "$P/probe.log"
RC=$?
echo "-----"
echo "exit=$RC"
if [ $RC -eq 0 ]; then
  echo "PASS — 라이선스 활성. 배치 모드가 돈다"
elif [ $RC -eq 198 ]; then
  echo "FAIL — exit 198. 라이선스 미활성 상태 그대로다"
else
  echo "OTHER — exit $RC. 아래 로그 확인"
fi
grep -i "licens\|activat\|entitlement" "$P/probe.log" | tail -12
