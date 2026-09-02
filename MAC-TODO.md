# 🍎 MAC-TODO — 작가가 맥 앞에서 직접 해야 하는 것

> 🔒 **이 파일은 코덱스 몫이 아니다.** 코덱스는 읽지도 말고 손대지도 마라.
> 여기 있는 건 전부 **GUI 로그인 · 자격증명 · 삭제권한** 이 필요해서 자동화가 못 하는 것들이다.
> 작업 지시는 `NEXT.md` 에 있다. 이 파일에는 **지시가 없다.**

**마지막 갱신 2026-09-02** · 미완 3건

---

## 🔴 M-1. Unity 라이선스 복구 — **유일한 실병목**

이거 하나가 **M3(맵 진행 실측)** 와 **F1-b(폰트 TMP 굽기)** 를 동시에 막고 있다.
나머지 전부는 이거 없이도 굴러간다.

```
증상   Unity Licensing Client: Unsupported protocol version '1.18.1'
범위   -nographics 유무 두 경로 모두 테스트 도달 전 중단
에디터 6000.4.3f1 (39d1a88d4dd1)
```

**⚠️ 원인을 내가 특정하지 못했다.** 라이선스 클라이언트는 연결 폴더 밖(`/Applications`, `~/Library`)이라 **내 사정권 밖**이다.
아래는 비용 낮은 순 후보다. **1번부터 하나씩, 될 때까지.**

| # | 할 것 | 왜 |
|---|---|---|
| 1 | Unity Hub 를 완전 종료 후 재실행 → 로그인 상태 확인 → **Preferences ▸ Licenses** 에서 라이선스 한 번 제거하고 재활성화 | 대개 Hub 와 에디터의 licensing client 버전이 어긋난 것이다. 재활성화가 클라이언트를 다시 깐다 |
| 2 | **Unity Hub 자체를 최신으로 업데이트** | 에디터 6000.4.3f1 이 요구하는 프로토콜을 구버전 Hub 가 못 말한다 |
| 3 | 좀비 프로세스 정리 후 1번 재시도 (아래 명령) | 죽다 만 클라이언트가 소켓을 잡고 있으면 새 것이 못 뜬다 |

```bash
pkill -f 'Unity.Licensing.Client' ; pkill -f 'UnityLicensingClient'
ls ~/Library/Application\ Support/Unity/       # Unity_lic.ulf 존재 확인
```

**✅ 풀렸는지 확인하는 법 — 여기까지만 하면 된다.**

```bash
cd "/Users/godju/Downloads/AI Game/hwigi-rest-ui-20260901"
bash Tools/unity_license_probe.sh
```

**PASS 가 나오면 나한테 「라이선스 풀렸다」 한 마디만.** 나머지는 코덱스가 `NEXT.md` 보고 알아서 간다.
❌ **직접 M3 를 돌리지 마라.** 그건 코덱스 일이다.

---

## 🟡 M-2. push — 자격증명이 있어야 한다

브랜치 `codex/hwigi-lobby-ui-pilot-r3` 가 원격보다 앞서 있다. **몇 개인지는 아래 첫 줄이 알려준다** — 숫자를 여기 박아두면 금방 낡는다.

```bash
cd "/Users/godju/Downloads/AI Game/hwigi-rest-ui-20260901"
git log --oneline origin/codex/hwigi-lobby-ui-pilot-r3..HEAD   # 올라갈 것 먼저 눈으로
git push origin codex/hwigi-lobby-ui-pilot-r3
```

⚠️ **이 저장소는 Git LFS 를 쓴다.** 맥에는 `git-lfs` 가 깔려 있으니 그냥 push 하면 된다.
(내 쪽 리눅스에는 없어서, 나는 **텍스트 파일만** 커밋했다. 바이너리는 손대지 않았다.)

---

## 🟢 M-3. `.git` 찌꺼기 청소 — 무해, 언제 해도 됨

내가 리눅스 쪽에서 git 을 돌리며 남긴 것이다. **동작에 지장 없다.**

```bash
cd "/Users/godju/Downloads/AI Game/hwigi-tower"
find .git -name 'tmp_obj_*' -delete
find .git -name '*.lock.stale-*' -delete
git prune
```

**재발을 막으려면** — 채팅에서 「hwigi-tower 폴더 삭제 권한 줘」 라고 해주면 내가 알아서 치운다.
안 줘도 된다. 그럼 이 명령을 가끔 한 번 돌리면 그만이다.

---

## ⚪ 참고 — 작가가 **안 해도 되는** 것

헷갈리지 않게 적어둔다. 아래는 **전부 코덱스 몫**이다. 손대지 마라.

| | 누가 | 언제 |
|---|---|---|
| M3 맵 진행 실측 · M4 수리 | 코덱스 | M-1 풀린 직후 |
| F1-a 폰트 파일 배치 + `OFL.txt` 동봉 | 코덱스 | **지금 당장 가능** |
| F1-b TMP SDF 굽기 · 12화면 재캡처 · 검사기 4종 | 코덱스 | M-1 풀린 뒤 |
| `MF-03` cross-lane 위반 수리 | 코덱스 | M4 |
| 커밋 | 나(게이트) 또는 코덱스 | 상시 |

**결정이 필요해지면 내가 선택지 2~3개로 물어본다. 먼저 찾아다니지 마라.**
