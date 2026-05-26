---
created: 2026-04-27
updated: 2026-05-27 (v0.15.0)
type: project
tags: [gdd, project, flick, roguelike, ai-npc, on-device-llm, unity, hybrid-casual, gpt-5.5]
status: active
codename: 회귀자는 탑을 오른다
version: 0.15.0
deadline: 2026-05-18
---

# 회귀자는 탑을 오른다 — Game Design Document (SSOT)

> 이 문서는 본 게임의 **단일 진실 소스 (Single Source of Truth)**.
> 모든 게임 디자인 결정은 여기서 이루어지고 여기에만 기록된다.
> 다른 문서가 본 GDD 와 충돌하면 **본 GDD 가 우선**.

---

## 0. 문서 운영 규약 (META — 항상 먼저 읽기)

### 0.1 문서의 역할

- **SSOT**: 게임 디자인·기술·일정 결정의 유일한 권위 출처
- **공모전 메타** (마감·상금·제출 형식·심사 기준 등) 는 [[flick-challenge]] 가 담당. 게임 그 자체는 본 문서.
- **외부 산출물** (코드·아트·영상·deck) 는 본 문서를 *참조* 만 하고, 결정을 만들지 않는다.

### 0.2 업데이트 프로토콜

새 결정 / 변경 발생 시 다음 4단계 모두 수행:

1. **§1 CHANGELOG** 에 한 줄 추가 (날짜·요약·이유)
2. **§2 DECISIONS** 에 결정 카드 추가 또는 갱신 (D-NNN, status: LOCKED / RECONSIDERED / DEPRECATED)
3. **§3 OPEN QUESTIONS** 에서 해당 항목 close 또는 신규 OQ 추가
4. 본문 해당 섹션 직접 수정 + frontmatter `version` bump + `updated` 날짜 갱신

### 0.3 잠금 규칙

- **🔒 LOCKED** 표시된 결정은 변경 시 §1 CHANGELOG + §2 DECISIONS 에 *변경 사유* 필수 기록
- **🟡 PROVISIONAL** 은 검증 후 LOCKED 또는 변경
- **❓ OPEN** 은 §3 OPEN QUESTIONS 에 등록되어 있어야 함

### 0.4 버전 정책 (semver-lite)

- **MAJOR** (1.0.0) — 컨셉·트랙·플랫폼 같은 근본 결정 변경
- **MINOR** (0.X.0) — 콘텐츠 수량·시스템·일정 변경
- **PATCH** (0.0.X) — 텍스트·이름·세부 수치 조정

### 0.5 본 문서를 읽는 순서

1. §0 META (이 섹션) → 운영 규약 숙지
2. §0.6 DESIGN PILLARS → 모든 결정의 정합성 기준
3. §1 CHANGELOG → 최근 변경 파악
4. §2 DECISIONS → 잠긴 결정 확인
5. §3 OPEN QUESTIONS → 무엇이 미결정인지 확인
6. 필요 본문 섹션

### 0.6 DESIGN PILLARS 🔒 (모든 결정·구현·텍스트의 정합성 시금석)

**규칙**: 새 결정·기능·텍스트 제안 시 5개 Pillar 와 정렬 여부를 명시적으로 점검한다. 위배되면 거부 또는 수정. Pillar 자체 변경은 §0.4 MAJOR 버전 bump.

#### P1 — 되찾는 것은 잃기 위해서다 (D-019 narrative core)
모든 시스템·서사·UI 는 **회복 → 붕괴 → 망각** 의 3악장을 강화한다. 회복만 있는 진행, 망각이 비극이 아닌 해방으로만 보이는 연출, 붕괴 단계가 단순 난이도 상승으로 환원되는 구현은 **거부**.

#### P2 — LLM 의 한계는 in-game 진실이다 (D-005 / D-012)
컨텍스트 decay, persona drift, hallucination, 짧은 답변, 어색한 한국어 — 이것들은 *완벽히 숨기지 않는다*. 마타이오스의 붕괴(S3-S4)에서 오히려 *드러내고*, 회복기(S0-S2)에서는 캐싱·프롬프트 디자인으로 안정화한다. 기술 한계 = 캐릭터 진실.

#### P3 — 5분의 호흡 + 12회차의 무게 (D-002 / D-008)
1런 = 캐주얼 (5-7분, 한 손, 즉각 보상). 누적 12+ 회차 = 미드코어 (서사 깊이, 윤리 무게). 둘 중 하나만 만족하면 거부 — 너무 얕거나 너무 무거우면 트랙·서사가 동시에 깨진다.

#### P4 — 결정성은 신뢰의 기반 (D-006 / §8.5)
같은 (run_id, prompt_hash) → 같은 출력. 같은 회차·같은 선택 → 같은 결과. 데모 시드 5회차는 100% 결정적. *비결정적 코드는 거부* — 시드 RNG, 캐시, 명시적 시간 처리만 허용.

#### P5 — 선택은 결과를 미루지 않는다 (D-013 / §7.2 도덕적 선택 3)
플레이어의 도덕적 선택은 *즉시* 반응한다 — NPC 다음 발화 변화, 호감도 가시화, 휴식 노드의 분기. 추상적 점수만 누적하고 끝에 한꺼번에 정산하는 설계는 거부. 선택의 무게는 즉시·반복 가시화로 만든다.

#### Anti-Pillars (이 게임이 아닌 것)
- ❌ **가차·수집** — 캐주얼 트랙이지만 BM 종속 디자인 거부 (Flick MG 모델 = 광고 + 결제, 가차 X)
- ❌ **양적 빌드 다양성** — 능력 12 + 시너지 4 가 *충분히 깊다*는 가설. 양으로 보상하지 않음 (D-009 잠금 사유)
- ❌ **LLM 자랑 마케팅** — "AI가 만든 대사" 셀링 포인트 X. 마타이오스는 캐릭터지 기술 데모가 아니다.

---

## 1. CHANGELOG

| 버전 | 날짜 | 변경 |
|------|------|------|
| 0.1 | 2026-04-27 | 초안 잠금. 컨셉·NPC·LLM·콘텐츠 수량·일정·repo 구조 v0.1 확정. 작가 1차 결정 7건 반영. |
| 0.1.1 | 2026-04-27 | D-015 (코딩 에이전트 = Codex) 추가 + AGENTS.md 위성 문서 ([[hwiglija-tower-AGENTS]]) 작성. §10 기술 스택 갱신. |
| 0.1.2 | 2026-04-28 | D-016 (repo 이름 = `hwigi-tower`, 위치 = `~/Downloads/AI Game/hwigi-tower`) 추가. §11 repo 구조 헤더 갱신. PROGRESS 파일 연동 정식화. |
| 0.1.3 | 2026-04-28 | D-017 (NPC 이름 = 마타이오스), D-018 (빌드 우선순위 = Android APK 1순위, iOS = 영상 데모만). OQ-001/007/008/009 close. |
| 0.1.4 | 2026-04-28 | D-019 (마타이오스가 잃어버린 것 = 플레이어와의 유대 + 본인의 기억; 종착 = 재망각의 루프). OQ-005 close. §4.1 pitch, §5.1 NPC, §6 코어 루프 메타-읽기 갱신. |
| 0.2.0 | 2026-04-28 | §0.6 DESIGN PILLARS (P1-P5 + Anti-Pillars) 신규 잠금. 위성 문서 2종 추가 (`tone-bible`, `design-journal`). MINOR bump — 기획안 디벨롭 인프라 정식화. |
| 0.2.1 | 2026-04-30 | §0.7 DISCARD PILE (RAG, Voice, Fine-tuning 제외) 추가. Fallback 및 Structured Output 가이드라인 구체화. |
| 0.2.2 | 2026-05-02 | D-020 (외주 = 동생) + D-021 (스코프 유지, 일정 = AI 처리량 기반) 추가. 04-30~05-02 3일 공백 (조사) 후 재개. OQ-010 외주 작업 정의 등록. |
| 0.6.1 | 2026-05-05 | OQ-002 close(마타이오스 외형: 10대 여성, 일러스트 보유). OQ-003 close(엔딩: 단순 컷씬 2종, 메타 영향 미래 고려). OQ-006 임시(메모리 파편 5개 임시 초안, 작가 퇴고 예정). OQ-010 close(사운드 브리프 리스트업). D-024 신규(플레이어 발화 기억 시스템). §5.1 외형 갱신. §8.4 SQLite 스키마 확장. |
| 0.6.0 | 2026-05-05 | OQ-015 close(복합 행동: 1→처리→2 순차, 2번째=추가공격ATK×0.7 또는 아이템만). OQ-016 close(아이템 시스템: 유물 6 + 일반 12, 패시브 보유, 소모품 없음). D-023 신규. §7.7 아이템/유물 섹션 신규. D-009 수량 갱신. |
| 0.5.0 | 2026-05-05 | OQ-013 close(상점: 3개/런, 20G/30G, 리롤 10G+5G). OQ-014 close(적 턴: 상황 텍스트 1개+2택, 몬스터별 고유 기믹). D-022 갱신(적 턴 3택→2택). §7.0 노드맵 balance.md 반영. |
| 0.4.1 | 2026-05-05 | OQ-012 close(노드 맵: 슬더스식 3경로·선택지 팝업·시드 랜덤·상인→보스 고정). §7.0 노드 맵 구조 신규. |
| 0.4.0 | 2026-05-05 | OQ-011 close(특성 12종 확정: 생존/공격/지원×4, 메타 영구 해금, 승리 횟수 3단계). D-009 수량 갱신(특성 12 확정). OQ-015/016 신규 등록. §7.1 특성 시스템 전면 갱신. |
| 0.3.0 | 2026-05-05 | D-022 신규 잠금(전투 구조 상세: 턴제 3택·적 턴 텍스트 대응·Glitch 타이머). D-009 수량 갱신(특성 8~12 추가). OQ-011~014 신규 등록. §7.1 능력 12종·시너지 4종 명세 확정. §7.3 적 수치 테이블 추가. §7.5 경제 밸런스·§7.6 플레이어 기초 스탯 신규. |
| 0.7.0 | 2026-05-10 | D-025 신규 잠금(정서적 결정론 — SPD 인사이트 상속, NPC 정서 벡터 + 자원 관리의 필연적 결과로 전투 설계, RNG 억까 방지). 후속 OQ(modifier 수치·dialogue→combat 매핑) 분리 등록 예정. P4·P5 강화. MINOR bump — 시스템 원리 레이어 추가. |
| 0.8.0 | 2026-05-19 | D-026~D-028 신규 잠금 — 마타이오스 프로젝트 복기 자산 수혈. D-026(SLM 파인튜닝 가드레일: BPE 토큰 ID 매칭·Korean-primary 베이스·trust_remote_code revision 핀). D-027(서사 엔진: 간접 기억법·골절 시스템). D-028(FastAPI Remote Provider 표준 개발 브릿지). P2 강화 — 기술 한계의 in-game 진실화 구체 절차 확정. MINOR bump — 기술 무결성 레이어 추가. |
| 0.9.0 | 2026-05-22 | D-029 신규 잠금 — Glitch를 플레이어 직관 용어인 **붕괴도**로 치환하고, 호감도·회복·붕괴 축으로 남기되 전투 빌드 modifier에서 분리. D-022 적 턴 타이머와 D-025 정서적 전투 modifier를 재고하며, 전투 재미는 능력·아이템·유물·시너지 표면에서 잠금. OQ-017 신규. |
| 0.10.0 | 2026-05-22 | D-030 신규 잠금 — OQ-017 대체 빌드 축 확정(`SWORD_03` HP 리스크 강공, `광폭` 공격 연쇄, `반사` 방어→짧은 반격 기회, `ITEM_07` 적 패턴 대응 보조). OQ-017 close, 수치/효과 계약 OQ-018 및 `ITEM_07` 패턴 결과 계약 OQ-019 분리. Playable Build Surface Lock 명세 handoff 기준 추가. |
| 0.11.0 | 2026-05-22 | D-031 신규 잠금 — OQ-018 효과 계약 확정(`SWORD_03` 비자살 HP-cost 강공, `광폭` 공격 연쇄 유지/비공격 시 break, `반사` 방어 후 다음 공격 1회 강한 반격 창). OQ-018 close. 첫 Build Surface 개발 brief를 확정하고 exact 1차 플레이테스트 수치 입력은 OQ-020으로 분리. |
| 0.11.1 | 2026-05-22 | OQ-020 close — D-031 첫 플레이테스트 기준값 입력(`SWORD_03` HP 3 비용/+5 피해, `광폭` 추가타 ATK×0.5→직전 Attack 시 ATK×0.75, `반사` 받은 피해 50% 반사+방어 후 다음 Attack 1회 ×1.5). PATCH — 밸런스 잠금 아님. |
| 0.11.2 | 2026-05-22 | D-031 갱신 — `SWORD_03` 비자살 HP 비용 계약을 폐기하고, HP 3 이하 전투당 1회 과부하 강공으로 변경. 공격 피해와 +5 보너스를 먼저 적용한 뒤 HP 비용 사망을 판정한다. OQ-019는 open 유지. |
| 0.12.0 | 2026-05-24 | D-032 신규 잠금 — 다음 전투 기준선을 `Player + Mataios vs Enemy` 2인 파티 전투로 전환. 마타이오스는 deterministic automatic actor로 참여하고, down 시 전투는 지속되며 붕괴 이벤트/회복 압박을 발생시킨다. D-029 금지선 유지, 다중 적/직접 조작/본편 runtime RL 제외. OQ-021~024 추가. |
| 0.12.1 | 2026-05-24 | D-032 보정 — OQ-021/OQ-024를 1차 구현값으로 close. OQ-022는 down/collapse temporary implementation contract로 partial-close(+5, 전투당 1회, non-blocking log/overlay). OQ-023은 combat expansion stage로 defer하고 1차 구현은 기존 Player chain 유지. |
| 0.13.0 | 2026-05-24 | D-033 신규 잠금 — Combat Core Rebuild 상위 방향 잠금. Enemy intent/counterplay/feedback/balance metric을 D-032 위에 올려 Attack spam damage race를 전략적 턴 전투로 교체한다. 구현은 4개 배치로 분리하고, enemy별 intent deck exact payload는 OQ-025로 분리. |
| 0.13.1 | 2026-05-25 | Map Flow / Route Commitment 검토 — OQ-012의 "맵 화면 없음/선택지 팝업" 구현 세부는 현재 런타임 지도 UI와 사용자 피드백에 의해 D-034 후보로 대체 검토. Sparse 3-lane route, irreversible node commitment, delayed Rest, next-floor map visibility를 제안하고 OQ-026을 등록. |
| 0.14.0 | 2026-05-26 | D-035 신규 잠금 — AI Track Exp04-06 결과를 본편 runtime RL/ONNX가 아니라 `ContextPolicy` 기반 deterministic Mataios combat brain으로 전환. OQ-024 단순 table은 fallback으로 남기고, enemy threat/tempo/skill context를 읽는 명시 rule table을 다음 구현 기준으로 잠금. 본편 C# 변경 전 CodeGraph fresh status/sync/query/context 필수. |
| 0.15.0 | 2026-05-27 | D-034 정식 잠금 + D-036 신규 잠금 — 2026-05-27 피드백을 반영해 pre-run placeholder, sparse route/reveal/commitment, resolver-owned combat preview, enemy stat surface, read-only combat item inspect, map/boss BGM reset, low HP P1 feedback 계약을 확정. OQ-026 close. OQ-019/OQ-025는 open 유지. |

---

## 2. DECISIONS (잠긴 결정 카드)

### D-001 컨셉명 🔒 LOCKED 2026-04-27
**값**: "회귀자는 탑을 오른다"
**근거**: 회귀물 톤 + 탑 등반 메타포 + 한국어 정체성

### D-002 트랙 🔒 LOCKED 2026-04-27
**값**: 하이브리드 캐주얼 (미드코어 요소 = 로그라이크)
**근거**: Flick 공모전 트랙 적합성 + MG $200K 풀

### D-003 플랫폼 🔒 LOCKED 2026-04-27
**값**: iOS / Android 모바일, 세로 (Portrait), 한 손 조작
**근거**: 캐주얼 모바일 표준, CPI 테스트 가능

### D-004 엔진 🔒 LOCKED 2026-04-27
**값**: Unity 6 LTS (C#)
**근거**: VS Code + Copilot/Codex 호환, 모바일 표준, 데이터 드리븐 (ScriptableObject) 적합

### D-005 LLM 위치 🟡 PROVISIONAL 2026-04-27
**값**: On-device (HyperCLOVA X SEED 0.5B 1순위)
**Fallback rule**: D-14 (2026-05-04) 까지 한국어 자연도 검증 → 부족 시 하이브리드 (1런 1회 클라우드 호출) 전환
**근거**: 비용 0 + 오프라인 + 정책 안전. 한국어 SLM 품질 리스크 잔존.

### D-006 메모리 아키텍처 🔒 LOCKED 2026-04-27
**값**: SQLite + 회차당 1 reflection (LLM 생성 3문장 요약)
**근거**: [[generative-agents]] 패턴 차용, 모바일 가벼움, 결정성 보장

### D-007 동료 NPC 수 🔒 LOCKED 2026-04-27
**값**: 1명 고정
**근거**: 21일 솔로 스코프 + 정 깊이 ↑ + 서사 명확성

### D-008 세션 길이 🔒 LOCKED 2026-04-27
**값**: 5-7분 / 1런 (mini-loop 30초~1분 × 5-7 노드)
**근거**: 캐주얼 KPI 양립 + 미드코어 호흡 균형

### D-009 콘텐츠 수량 🔒 LOCKED 2026-04-27
**값**: 능력 12 / 시너지 4 / 적 6 / 보스 2 / 층 5 / 인카운터 25 / 메모리 파편 5 / 엔딩 2
**근거**: 작가 결정 — 능력↓ 인카운터↑ (NPC 대화 통합)

### D-010 NPC 정체성 🔒 LOCKED 2026-04-27
**값**: "잃어버린 것을 찾아 탑으로 들어온 모험가. 1층에서 플레이어와 만나 동행 제안. 플레이어가 수락."
**근거**: 작가 결정. 정체성 자체는 잠금, 이름·외형은 OQ.

### D-011 회차 기억 트리거 🔒 LOCKED 2026-04-27
**값**: 1번 보스 격파 후부터 NPC 가 회차 기억
**근거**: 회귀물 클리셰 역전 (NPC 도 기억) + 보상으로 작동

### D-012 5단계 붕괴 모델 🔒 LOCKED 2026-04-27
**값**: S0 첫만남 / S1 인지 / S2 동행 / S3 균열 / S4 붕괴 / S5 안식 또는 동행 (선택)
**근거**: LLM 컨텍스트 decay 의 in-game 흡수, 작가 의도

### D-013 엔딩 분기 🔒 LOCKED 2026-04-27
**값**: 최종 보스 격파 → 안식 아이템 획득 → 안식(엔딩 A) / 동행 계속(엔딩 B) 플레이어 선택
**근거**: 작가 결정. 윤리적 선택의 무게 = 본 게임 코어 메시지

### D-014 팀 구성 🔒 LOCKED 2026-04-27
**값**: 솔로 개발 + 아트·사운드 외주
**근거**: 작가 답변

### D-015 코딩 에이전트 🔒 LOCKED 2026-04-27
**값**: **Codex CLI** (OpenAI) — Copilot Chat 아님
**브리프 파일**: [[hwiglija-tower-AGENTS]] — Unity repo 생성 시 루트에 `AGENTS.md` 로 복사
**근거**: 작가 결정. Codex 의 AGENTS.md 컨벤션 활용으로 SSOT 자동 인지 + 절대 금지 항목 enforce

### D-017 NPC 이름 🔒 LOCKED 2026-04-28
**값**: **마타이오스** (Mataios, μάταιος — 그리스어 "무가치함")
**근거**: 작가 결정. NPC 의 정체성("잃어버린 것을 찾아 탑에 들어온 모험가") + S3-S4 붕괴 단계 + 안식/동행 윤리 선택과 의미 공명. 이국 어원으로 한국적 SF 톤 위에 *낯섦* 부여.
**적용**:
- §5.1 NPC 명세 이름란 갱신, OQ-001 close
- 외주 브리프 (구두 전달) 시 호명
- LLM base persona prompt 의 자기소개 첫 문장에 사용

### D-019 잃어버린 것의 정체 + 종착 🔒 LOCKED 2026-04-28
**값 (정체)**: 마타이오스가 "잃어버린 것" = **플레이어와의 유대 + 본인의 기억**
**값 (구조)**: **루프물 — 가둠**. 마타이오스는 탑에 갇혀 잃어버린 것을 찾는 여정을 시작하지만, **여정의 종착은 그것을 다시 망각하는 것**.
**서사 의미**:
- *무가치함* (마타이오스 = μάταιος) — 찾는 행위 자체가 결국 무의미해지는 구조
- D-011 (1번 보스 후 회차 기억 시작) = 잃어버린 것의 점진적 회복
- D-012 (5단계 붕괴 S0→S5) = 회복했던 것이 다시 무너지는 과정
- D-013 (엔딩 A 안식 / B 동행) = 두 가지 망각의 양태:
  - **A 안식** = 능동적 망각 (마타이오스가 선택한 끝)
  - **B 동행** = 수동적 망각 (다음 회차에서 다시 붕괴 → 루프)
- 어느 쪽이든 망각으로 수렴. 플레이어의 선택은 *방식*만 결정.
**적용**:
- §4.1 한 줄 pitch 갱신
- §5.1 NPC 정체성 "잃어버린 것" 명시
- §6 코어 루프에 메타-읽기 추가 (회복-붕괴-망각 사이클)
- §7.2 메모리 파편 5개 (OQ-006) 가 본 D-019 기반으로 작성됨
**OQ-005 close**.

### D-018 빌드 우선순위 🔒 LOCKED 2026-04-28
**값**:
- **Android APK 1순위** (실기 빌드·플레이테스트·CPI 광고 모두 Android)
- **iOS 영상 데모만** (Xcode 시뮬레이터 또는 Mac 실기 영상 캡처, TestFlight 미사용)
**근거**: 작가 Apple Developer Program 미가입. $99/year 비용 + 심사 지연 리스크 회피. Mac 보유 → 향후 가입 시 iOS 빌드 즉시 가능.
**Fallback rule**: 공모전 심사가 iOS 실기 빌드 필수 시 D-2026-05-15 까지 Apple Developer 가입 결정.
**OQ-009 close**.

### D-020 외주 = 동생 🔒 LOCKED 2026-05-02
**값**: D-014 의 "아트·사운드 외주" 가 *작가의 동생* 으로 구체화.
**근거**: 작가 결정. 가족 신뢰 자원 → 커뮤니케이션 비용 ↓, OQ-007/008 의 구두 전달 결정과 정합.
**적용**: OQ-002 (NPC 외형) 결정 시 동생에게 직접 전달 (디벨롭 큐 토픽 1 Gemini 우회 가능).
**리스크 노트**: 마감 압박 시 가족 갈등 가능성. OQ-010 에서 작업 범위·기한·채널·산출 형식 명확화 필요.

### D-021 스코프 유지 + AI 처리량 기반 일정 🔒 LOCKED 2026-05-02
**값**:
- D-009 콘텐츠 수량 **변경 없음** (능력 12 / 시너지 4 / 적 6 / 보스 2 / 층 5 / 인카운터 25 / 메모리 파편 5 / 엔딩 2 그대로)
- 04-30 ~ 05-02 3일 공백 (조사) 후 05-02 저녁 재개
- 일정 = AI 처리량 기반 — Codex 가 보일러플레이트, Gemini 가 디자인 디벨롭. 작가는 *결정* 만
- 토큰 비용 관리 = D-005 fallback rule + 토픽 11 (Prompt Caching) 으로 흡수
**근거**: 작가 결정. 스코프 축소 시 양적 다양성 거부 (Anti-Pillar) 와 모순되지 않으나, 12 능력은 *깊이의 최소* 라는 원래 가설 보존. AI 가 SO 보일러플레이트·텍스트 생성을 흡수하므로 인간 비용은 결정 throughput 만 병목.
**적용**:
- W1-2 마감 (05-04) 슬립 가능성 인정 — 도미노 슬립 방지를 위해 W2-1 첫 주 (05-05~07) 에 W1-2 잔여 흡수
- 토픽 2 (능력 12 효과·수치) Gemini 디벨롭이 *Codex W1-2 진입 전제 조건*
- 매 LLM 호출 시 토큰 추정 출력 (script 또는 manual)

### D-022 전투 구조 상세 🔒 LOCKED 갱신 2026-05-22 (최초 LOCKED 2026-05-05)

**값:**

**[ 플레이어 턴 ]**
- 행동 3택 1: `공격` / `방어` / `스킬(능력 1개 선택 사용)`
- 시간 제한 없음 (JRPG식 전략 선택)
- 스킬: 보유한 능력 중 1개를 골라 발동. 슬롯 제한 없음.

**[ 적 턴 ]**
- 몬스터별 고유 **상황 텍스트 1개** 표시 (공격 예고 / 상태 변화 / 선택 강요 / NPC 개입 등 기믹 다양)
- 플레이어는 **텍스트 선택지 2택 1**로 대응 (선택지 내용은 몬스터마다 상이)
- 타이머: 기본 **10초**. 시간 초과 시 AI(마타이오스)가 자동 선택
- **붕괴도는 전투 타이머를 축소하지 않는다.** 적 턴 압박은 몬스터 패턴과 플레이어 빌드 대응으로 만든다 (D-029).
- 각 몬스터의 상황 텍스트·선택지 2개·결과는 `EnemyData SO`의 `patternId` → 별도 `EnemyPatternData SO`로 정의 (OQ-014 확정)

**근거:**
- 턴제 + 텍스트 선택지 = P3(5분 호흡) + 하이브리드 캐주얼 트랙 정합.
- 슬롯 제한 없음 = 능력 획득의 즉각 가치 보장 (P5).
- 3택→2택 변경 근거: 몬스터별 고유 기믹 서사(피어&헝거·서울2033 참조) — 선택지 수보다 상황 텍스트의 질·다양성이 긴장감의 핵심 (작가 결정 2026-05-05).
- 붕괴도 비전투화 = 관계·회복 축을 전투 수치 압박과 분리하여, 전투 선택은 능력·아이템·유물·시너지로 읽히게 함 (D-029).

---

### D-024 플레이어 발화 기억 시스템 🔒 LOCKED 2026-05-05

**값:**
- 플레이어가 **휴식 노드 / 이벤트 인카운터** 에서 특정 선택지 진입 시 텍스트 필드 열림
- 입력된 자연어 발화를 `player_utterances` 테이블에 저장 (run_id, context, utterance, recalled)
- 마타이오스가 이후 회차 회상 시 **실제 플레이어 발화 원문을 LLM 프롬프트에 주입** → 인용하며 반응
- S2(동행): 온전히 인용. S3(균열): 뒤섞어 인용. S4(붕괴): 단어 단위로 파편화.
- `recalled` 플래그로 동일 발화 중복 사용 방지 (1발화 1회상 원칙)

**메모리 파편 5개와의 역할 분리:**
- 플레이어 발화 기억 = 런타임 생성, 플레이어마다 다름
- 메모리 파편 5개 = 작가 사전 작성, 마타이오스 관련 오브젝트/서사 텍스트 (§7.2, `design/memory-fragments.md`)

**SQLite 스키마 추가 (§8.4 연동):**
```sql
CREATE TABLE player_utterances (
  id        INTEGER PRIMARY KEY,
  run_id    INTEGER,
  context   TEXT,     -- 'rest' | 'encounter'
  utterance TEXT,     -- 플레이어 자연어 입력 원문
  recalled  INTEGER   -- 0/1
);
```

**근거:**
- TRPG식 플레이어 발화 → 마타이오스 기억 구조 (작가 결정 2026-05-05)
- P5(선택은 결과를 미루지 않는다): 플레이어의 말이 실제로 마타이오스의 대사에 반영됨
- P2(LLM 한계 = in-game 진실): S3~S4 뒤틀린 인용 = LLM 컨텍스트 decay 의 서사적 흡수

---

### D-023 아이템 시스템 + 복합 행동 세부 🔒 LOCKED 2026-05-05

**값 (아이템 시스템):**
- **소모품 없음** — 모든 아이템은 획득 즉시 패시브 보유 효과 적용
- **등급 이분화**: 일반 아이템 (상점 기본 슬롯) / 유물 (TRAIT_SUPPORT_04 해금 슬롯, 30G)
- 시스템 코드상 동일 타입 (`ItemData SO`), `tier` 필드(normal/relic)로 구분
- 유물 = 능력·시너지 태그 연동 강한 패시브. 일반 = 능력 보조 작은 패시브
- 수량: 유물 6개 / 일반 아이템 12개 (§7.7 참조)

**값 (복합 행동 TRAIT_OFFENSE_04):**
- 플레이어 턴: **1번째 행동 선택 → 즉시 처리 → 2번째 행동 선택**
- 2번째 행동 허용: **추가 공격(ATK×0.7)** 또는 **아이템 사용** 만
- 방어·스킬은 2번째 행동으로 불가
- 1번째 행동은 정상 배수 그대로 적용
- UI: 1번 처리 직후 "추가 행동" 헤더와 함께 허용 선택지 재표시

**근거:**
- TRPG 복합 행동 참조 — 주 행동 + 보조 행동 구조로 전투 깊이 ↑
- 아이템 사용 포함 = 아이템 시스템과 전투 시스템 자연스럽게 연결 (P5)
- 방어/스킬 제한 = 복잡도 억제, 5분 호흡 유지 (P3)

---

### D-025 정서적 결정론 (Deterministic Combat) 🟡 RECONSIDERED 2026-05-22 (최초 LOCKED 2026-05-10)

**재고 메모**: D-029가 NPC 정서/붕괴 상태의 전투 modifier 직접 연결을 철회한다. 아래의 "숨겨진 RNG 억까 방지"와 같은 결정성 원리는 유지하되, 전투 승패의 주 표면은 플레이어 빌드·자원·적 패턴으로 재정의한다.

**값**: 전투의 승패는 단순 RNG 가 아니라, **플레이어 빌드·전투 자원·적 패턴 대응**의 필연적 결과로 설계된다. SPD(Shattered Pixel Dungeon) 의 결정적 전투 로직을 *원리* 층위에서 상속.

**적용 원칙**:
- 같은 (빌드 상태, 자원 상태, 선택, 적 패턴) → 같은 전투 결과 (P4 결정성과 정합)
- 적 턴 2택(D-022)과 플레이어 액션 3택의 결과치는 데이터와 결정적 적용 규칙으로 추적 가능해야 한다. 숨겨진 주사위 굴림 X
- "억까(unfair RNG)" 회피 — 패배는 빌드 선택, 자원 관리, 적 패턴 대응 실패로 추적 가능해야 함
- NPC 호감도/붕괴 단계(S0-S5, D-012) + 최근 발화 토픽(D-024)은 관계·회복 표면에 남고, 전투 modifier의 기본 입력이 아니다 (D-029)

**근거**:
- SPD 인사이트: 결정성이 곧 전략성. RNG 가림막 없을수록 플레이어가 *왜 졌는지* 학습한다.
- P4 (결정성은 신뢰의 기반) 강화 — 데모 시드 5회차의 100% 재현 보장
- P5 (선택은 결과를 미루지 않는다) 강화 — 전투 선택의 결과는 즉시 적용되고, 관계 선택은 관계·회복 표면에서 즉시 반응
- P1 (회복-붕괴-망각) 보존 — 붕괴는 관계/회복 축에서 체감시키고 전투 빌드 강약으로 환원하지 않음

**범위**: 본 결정은 *원리 레이어*. 관계 상태를 전투 modifier로 다시 연결하는 제안은 D-029 재검토가 선행되어야 한다.

**참조**: [[roguelike-mechanics-spd]], D-022 (전투 구조), D-024 (발화 기억), §0.6 P4·P5

---

### D-026 SLM 파인튜닝 가드레일 🔒 LOCKED 2026-05-19

**값**: 마타이오스 SLM 파인튜닝 파이프라인은 다음 3중 가드레일을 강제 적용한다.

1. **토큰 매칭 (BPE 사일런트 실패 방지)**: DataCollator 구성 시 문자열 비교 대신 *확정된 토큰 ID 리스트* 를 전달한다. 훈련 시작 전 `active_labels > 0` 검증 셀을 필수 포함하여 라벨 마스킹이 0개로 무력화되는 사일런트 실패를 차단한다.
2. **언어 안전성 (Korean-primary 베이스)**: 한국어 캐릭터성 보장을 위해 베이스 모델은 *무조건 Korean-primary* (EXAONE 등) 로 선정한다. 다국어 prior 가 한국어 정체성을 희석하는 V1-V5 의 실패 패턴 재발 금지. BIW(세계관 외부 질문) 거부 패턴은 *명시적 데이터셋* 으로 학습한다.
3. **버전 고정 (revision pin)**: `trust_remote_code=True` 사용 시 반드시 *특정 Git revision (commit hash)* 을 핀(pin)한다. 모델 업데이트로 인한 파이프라인 붕괴 (silent BC break) 차단.

**근거**:
- [[hwigitower/technical-review]] §BPE 토큰 경계 매칭 버그 — V6 까지 라벨이 0개로 마스킹되는 사일런트 실패가 학습 정체의 근본 원인이었음.
- V7 EXAONE 전환에서 Korean prior 의 결정적 효과 실증 ([[hwigitower/post-mortem]]).
- P2 (LLM 한계 = in-game 진실) 강화 — 한계를 *수용* 하려면 먼저 한계의 *원천* 을 통제해야 함.
- P4 (결정성) 강화 — revision pin 은 학습 파이프라인의 결정성 보장 기반.

**범위**: 본 결정은 *훈련 파이프라인 무결성 레이어*. 구체 데이터셋 스펙·BIW 거부 비율 목표는 [[hwiglija-tower-npc-master-guide]] §1.1, §4.2 에서 관리.

**참조**: [[hwigitower/technical-review]], [[hwigitower/post-mortem]], [[hwiglija-tower-npc-master-guide]], §0.6 P2·P4

---

### D-027 간접 기억법 + 골절(Fracture) 서사 시스템 🔒 LOCKED 2026-05-19

**값**: 마타이오스의 기억 표현은 다음 2개 법칙으로 시스템화한다.

1. **간접 기억법 (Indirect Recall)**: NPC 는 플레이어의 과거 발화를 *재구축하지 않는다*. 대신 발화에서 추출된 *키워드 하나만* 불확실한 톤으로 언급한다 (예: "...탑이라 했나, 너의 입에서 들은 말 같은데"). 할루시네이션의 *서사적 치환* — 모르는 것을 지어내지 않고, 모른다는 사실 자체를 캐릭터로 표현.
2. **골절 (Fracture) 서사**: 모델의 불안정성·글리치는 *오류* 가 아니라 *탑의 영향으로 인한 자아 골절* 로 in-game 정의된다. 붕괴 스테이지 (D-012 S3+) 와 *동기화* 하여 시스템화 — S3 이상에서 출력 글리치 빈도 증가는 메커닉 일부.

**근거**:
- [[hwigitower/design-log]] §호감도 버킷·BIW 차단·골절 서사 — 캐릭터 영혼의 핵심 발견.
- P1 (회복→붕괴→망각) 직접 강화 — 간접 기억법은 *회복기* 의 유대 표현, 골절은 *붕괴기* 의 메커닉 표현.
- P2 (LLM 한계 = 진실) 의 가장 구체적 구현 — 컨텍스트 decay·hallucination 을 *드러내는* 절차.
- D-024 (플레이어 발화 기억 시스템) 와 정합 — 추출된 키워드가 D-024 의 저장 단위.

**범위**: 키워드 추출 휴리스틱·골절 글리치 패턴 라이브러리는 [[hwiglija-tower-npc-master-guide]] §1.1 BIW 및 신설 §골절 절에서 관리.

**참조**: [[hwigitower/design-log]], [[hwiglija-tower-npc-master-guide]], D-012 (붕괴 단계), D-024 (발화 기억), §0.6 P1·P2

---

### D-028 FastAPI Remote Provider 개발 표준 🔒 LOCKED 2026-05-19

**값**: NPC 추론 서버의 표준 개발 골격은 **FastAPI + transformers 직접 추론** 이다. Ollama / llama.cpp 의 지원 여부와 *무관하게* 본 골격을 Unity 연동의 기본 브릿지로 사용한다.

**근거**:
- [[hwigitower/technical-review]] §실전 서빙 — Ollama/llama.cpp 가 특정 모델 (EXAONE·HCX-SEED) 또는 특정 quant 를 즉시 지원하지 않는 사례 다수 발생. FastAPI 직접 추론이 *유일하게 항상 작동* 한 경로.
- Unity 연동 유연성 — FastAPI 는 표준 HTTP/JSON, Unity 의 `UnityWebRequest` 와 메인 스레드 안전성 정합.
- D-005 (on-device + cloud fallback) 의 *개발 단계 진입점* — on-device 변환 전 FastAPI 로 동일 모델을 검증 가능.
- P4 (결정성) 호환 — `temperature=0`·seed 핀 모두 transformers 직접 제어 가능.

**범위**: 본 결정은 *개발 표준*. 프로덕션 on-device 배포 (MLC-LLM 등) 는 D-005 에서 관리. FastAPI 서버는 데모·QA·튜닝 단계에서 유효한 표준 브릿지.

**참조**: [[hwigitower/technical-review]], D-005 (LLM Fallback), §0.6 P4, §10 기술 스택

---

### D-029 붕괴도 비전투화 + 로그라이크 빌드 표면 우선 🔒 LOCKED 2026-05-22

**값**:
- 설계 용어의 `Glitch`는 플레이어 직관 용어 **붕괴도**로 치환한다. 코드/자산 필드 rename은 구현 배치에서 별도 처리한다.
- 붕괴도는 마타이오스 호감도, 회복 기회, 관계 붕괴 체감과 연결되는 상태 축으로 남긴다.
- 붕괴도는 전투 타이머, 능력 발동 조건, 공격력 modifier, 시너지 배율의 기본 입력으로 사용하지 않는다.
- 전투 재미와 빌드 압박은 능력·아이템·유물·시너지 조합과 적 패턴 대응에서 만든다. 붕괴도/NPC 관계 축은 전투 빌드 읽기를 흐리지 않는다.

**근거**:
- 붕괴 상태가 전투 수치와 직접 얽히면 플레이어가 관계 회복과 전투 최적화를 동시에 해석해야 해 5-7분 런의 선택 표면이 불필요하게 까다로워진다.
- 슬레이 더 스파이어식 재미의 중심은 획득한 빌드 조각의 비교와 시너지 체감이다. 현재 Playable Build Surface Lock 이전에는 그 표면을 먼저 분명히 해야 한다.
- P1: 회복→붕괴→망각 축은 관계/회복 표면에서 보존하고, 회복만 강한 런으로 환원하지 않는다.
- P3: 짧은 런에서 관계 상태와 전투 빌드 판독을 분리해 선택 밀도를 감당 가능하게 유지한다.
- P4: 전투 modifier 입력을 빌드/적 패턴 중심으로 제한해 결정적 적용 검증 범위를 선명하게 한다.
- P5: 빌드 선택은 전투에서 즉시 체감되고, 관계 선택은 붕괴도/호감도/회복 반응에서 즉시 체감된다.

**영향**:
- D-022 적 턴 타이머의 Glitch 축소는 철회.
- D-025의 NPC 정서 벡터 combat modifier 원리는 재고.
- 기존 `ABILITY_SWORD_03`, `ITEM_07`, 검 시너지 `광폭`의 Glitch 전투 효과와 결 시너지 `반사`의 Affinity 전투 심화 로직은 OQ-017에서 대체/유지 여부 결정이 필요하다.

**참조**: D-022, D-025, OQ-017, §0.6 P1·P3·P4·P5

---

### D-030 OQ-017 대체 전투 빌드 축 🔒 LOCKED 2026-05-22

**값**:
- `ABILITY_SWORD_03`는 붕괴도 조건을 폐기한 채 **HP 리스크를 지불하는 공격 강화** 축으로 재정의한다. effect contract는 D-031, 첫 기준값 숫자는 OQ-020.
- 검 시너지 `광폭`의 기본 추가타는 유지하고 심화 로직은 **공격 연쇄 유지** 축으로 재정의한다. 단순 추가타 배율 증폭안은 채택하지 않는다.
- 결 시너지 `반사`의 기본 반사는 유지하고 심화 로직은 **방어 결과를 짧은 반격 기회로 전환**하는 축으로 재정의한다. 방어 누적 요새화안은 채택하지 않는다.
- `ITEM_07`은 **적 패턴 대응 보조** 축을 유지한다. flat ATK/HP/Gold 대체안으로 후퇴하지 않으며, 패턴 결과 공통 계약 확인 전 1차 runtime 구현 blocker로 만들지 않는다 (OQ-019).

**근거**:
- 검 빌드는 `ATK 기반`, `주기 추가타`, `공격 연쇄`, `HP 리스크 강공`으로 공격 선택 이유를 분리한다.
- 결 빌드는 방어 생존에만 머물지 않고, 방어 결과가 공격 창으로 이어지는 로그라이크 전투 리듬을 가진다.
- `ITEM_07`은 일반 아이템 표면에서 기존 공격/생존/경제 중복 대신 적 패턴 대응 축을 예약한다.
- 관계 상태 입력을 전투 modifier로 되돌리지 않아 D-029를 보존한다.

**Pillar 점검**:
- P1: 붕괴도는 관계·회복 압박 축에 남고 전투 강공 리스크는 HP 자원에서 읽힌다.
- P3: 각 축은 공격, 방어, 패턴 대응이라는 짧은 런의 읽기 쉬운 전투 선택으로 귀결된다.
- P4: 확정 축은 빌드 상태·HP·액션 결과·적 패턴 결과 계약으로 결정적 적용 가능해야 한다.
- P5: 강공, 연쇄 유지, 반격 창, 패턴 대응 보조는 적용 턴 또는 직후 전투 선택에서 체감되어야 한다.

**범위**: 본 결정은 빌드 축 잠금이다. effect contract는 D-031에서 닫고, 첫 기준값 숫자와 `ITEM_07` resolver 데이터 계약은 OQ-020/OQ-019로 분리한다.

**참조**: D-029, D-031, OQ-017 close, OQ-019, OQ-020

---

### D-031 OQ-018 첫 Build Surface 효과 계약 🔒 LOCKED 2026-05-22

**값**:
- `ABILITY_SWORD_03`는 공격 시 HP 비용을 지불하고 **해당 공격 1회**를 강공으로 바꾼다. HP 4 이상에서는 매 Attack HP 3 비용으로 +5 피해를 적용한다. HP 3 이하에서는 전투당 1회 과부하로 강공을 허용하고, 공격 피해와 +5 보너스를 먼저 적용한 뒤 HP 비용 사망을 판정한다. 과부하 사용 후 같은 전투의 HP 3 이하 Attack은 일반 Attack으로 처리되며, 다음 전투에서 과부하 1회가 다시 열린다. 축은 검 빌드의 HP-for-tempo다.
- 검 x3 시너지 `광폭`은 공격 기본 추가타를 가진다. 공격 연쇄를 유지하면 추가타가 강화되고, 방어 또는 스킬 선택 시 연쇄가 끊긴다. 단순 무한 배율 증폭으로 만들지 않는다.
- 결 x3 시너지 `반사`는 방어 기본 반사를 유지한다. 방어 후 **다음 공격 1회**에 짧은 반격 기회를 열고, 보상은 검 다타수 축과 겹치지 않는 **강한 반격 1회** 방향으로 둔다.

**상태**:
- 위 effect contract는 1차 Build Surface 개발 배치 기준으로 잠근다.
- 이 계약의 exact 수치는 최종 밸런스 잠금이 아니며, 첫 플레이테스트 기준값으로만 다룬다.
- 첫 기준값 숫자는 OQ-020에서 입력됐다. 이 값은 최종 밸런스 잠금이 아니며 플레이테스트 뒤 재조정 가능하다.

**근거**:
- `SWORD_03`은 공격 턴에서 HP를 tempo로 바꾸어 `SWORD_01` flat 성장과 `SWORD_02` 주기 추가타와 다른 선택 이유를 만든다.
- `광폭`은 공격 연쇄를 끊을지 유지할지 선택하게 하되, 무한 배율 snowball로 auto-pick 축이 되는 것을 막는다.
- `반사`는 방어의 보상을 다음 공격 1회로 옮겨 결 빌드가 요새화에만 머무르지 않게 하고 검의 다타수 역할을 침범하지 않는다.

**Pillar 점검**:
- P1: 전투 리스크는 HP 자원에서 읽히며 붕괴도/관계 상태를 전투 modifier로 재도입하지 않는다.
- P3: 강공, 연쇄 break, 다음 공격 1회 반격 창은 5-7분 런에서 읽을 수 있는 짧은 규칙이다.
- P4: 과부하 1회 상태, 공격→HP 비용→사망 판정 순서, 연쇄 break 액션, 1회 반격 소비 규칙은 결정적으로 적용 가능해야 한다.
- P5: 강공·강화 추가타·반격 창 결과는 해당 공격 또는 다음 공격에서 즉시 체감되어야 한다.

**1차 플레이테스트 기준값 (OQ-020)**:
- `ABILITY_SWORD_03`: Attack 선택 시 HP 3 비용, 해당 공격 피해 +5. HP 3 이하에서는 전투당 1회 과부하로 강공 가능하며 공격 피해를 먼저 적용한 뒤 HP 비용 사망을 판정.
- `광폭`: Attack 기본 추가타 ATK×0.5. 직전 플레이어 액션도 Attack이면 추가타 ATK×0.75. Defend 또는 Skill 선택 시 공격 연쇄 초기화.
- `반사`: Defend 시 받은 피해의 50% 반사. 방어 후 다음 Attack 1회 피해 ×1.5. 반격 창은 1회 소비.

**참조**: D-029, D-030, OQ-018 close, OQ-020 close

---

### D-032 Two-Actor Party Combat Baseline 🔒 LOCKED 2026-05-24

**값**:
- 다음 전투 기준선은 `Player + Mataios vs Enemy` 2인 파티 전투다.
- 마타이오스는 전투 actor로 참여하지만 플레이어가 직접 조작하지 않는다. 1차 조작 방식은 deterministic adaptive policy다.
- 본편 runtime에는 실시간 RL 학습을 넣지 않는다. 같은 seed, 같은 플레이어 입력, 같은 빌드/적 상태이면 마타이오스 행동과 전투 결과는 같아야 한다.
- 1차 전투 구조는 적 1체를 유지한다. 다중 적, 마타이오스 직접 조작, runtime RL 학습은 이번 기준선의 범위 밖이다.
- 턴 순서는 `round-start → Player action 선택 → Mataios policy 결정 → Player/Mataios 고정 순서 resolve → Enemy resolve → end-of-round/down/collapse 처리`를 권장 기준으로 둔다.
- 마타이오스 HP가 0이 되면 down 상태가 된다. Down 상태의 마타이오스는 행동하지 않고, 전투는 플레이어 단독으로 계속된다.
- 마타이오스 down 시 붕괴 이벤트가 발생하고 붕괴도 증가/가속 압박으로 이어진다. 단, 붕괴도는 전투 공격력/방어력/시너지 배율/능력 발동 조건 modifier로 사용하지 않는다. 1차 구현값은 temporary contract로만 취급한다.
- 1차 구현값: Mataios Max HP 16, action power 3. 전투 후 회복은 down이 아니면 Max HP의 25%(최소 3), down 상태였으면 HP 4로 복귀. Rest는 Mataios HP 완전 회복 및 down 해제.
- 1차 down/collapse temporary contract: Mataios HP 0 → down, 전투 지속, down 시 붕괴 이벤트, 붕괴도 +5, 한 전투당 down penalty 1회, blocking popup 금지, 전투 로그 + non-blocking overlay 우선. 이 값과 이벤트 경로는 플레이 후 교체 가능해야 하며 코드에 hard-code하지 않는다.
- 1차 deterministic policy는 OQ-024 기준 rule table을 사용한다. Affinity/NPC state/붕괴도는 policy input으로 사용하지 않고, 랜덤 선택도 사용하지 않는다.
- `광폭/FRENZY`는 이번 2인 전투 구현에서 새로 설계하지 않는다. 기존 Player action chain 기준을 유지하고, Mataios action은 chain 유지/강화/파괴에 관여하지 않는다. party chain 여부와 동료 행동 연계는 향후 combat expansion stage로 defer한다.

**근거**:
- 현재 전투는 마타이오스가 UI에 머무는 체감이 강하므로, 동행자 정체성을 전투 actor model에 포함해야 한다.
- 자동 행동은 조작 부담을 늘리지 않고 파티감을 만들 수 있다. 단, 항상 최적 행동을 고르면 전투가 자동 해결되므로 bounded deterministic policy가 필요하다.
- Down/collapse/recovery는 P1의 회복→붕괴→망각 축을 전투와 연결하지만, D-029처럼 전투 수치 modifier와는 분리한다.
- Player build surface는 여전히 능력·아이템·유물·시너지·적 패턴 대응에서 만든다.

**Pillar 점검**:
- P1: 마타이오스 down과 회복 압박은 관계/회복 축을 강화하되 붕괴도를 damage scaling으로 환원하지 않는다.
- P2: 마타이오스는 기술 데모가 아니라 전투에 동행하는 캐릭터 actor로 읽힌다. Runtime RL 과시는 제외한다.
- P3: 적 1체 + 자동 동료 1명으로 5-7분 런의 입력 밀도를 유지한다.
- P4: fixed turn order와 deterministic policy로 seed replay 신뢰를 유지한다.
- P5: 마타이오스 action, down, 붕괴 이벤트, 회복 결과가 즉시 로그/UI에 드러나야 한다.

**범위**:
- 설계/구현 handoff 문서: `design/two-actor-party-combat-lock-spec.md`.
- OQ-021/OQ-024는 1차 구현값으로 닫는다. OQ-022는 temporary implementation contract로 partial-close한다. OQ-023은 1차 구현에서는 Player chain 유지, 전면 재검토는 향후 combat expansion stage로 defer한다.
- OQ-019 `ITEM_07` 적 패턴 대응 계약은 계속 open이며, D-032가 이를 자동으로 닫지 않는다.

**참조**: D-029, D-030, D-031, OQ-019, OQ-021 close, OQ-022 partial-close, OQ-023 defer, OQ-024 close

---

### D-033 Combat Core Rebuild — Intent + Counterplay + Feedback 🔒 LOCKED 2026-05-24

**값**:
- Combat Core Rebuild는 D-032 2인 actor baseline 위에 적 intent, player counterplay, feedback, balance metric을 올리는 상위 전투 코어 결정이다.
- 적은 매 라운드 `normal_attack`, `heavy_attack`, `guard_brace`, `charge`, `weak_opening`, `special_boss` 중 하나의 intent를 노출한다. 1차는 적 1체 유지.
- Player action은 enemy intent에 따라 역할이 갈린다.
  - `Attack`: weak/opening punish, charge race, lethal finish, 기존 `광폭/FRENZY` Player chain payoff.
  - `Defend`: heavy/special threat 대응, HP 보존, Guard build payoff.
  - `Skill`: guard/brace 우회, charge interrupt/burst, cooldown timing payoff.
- Mataios는 D-032/OQ-024의 deterministic policy로 protection, finish assist, pressure, stabilize, down/collapse risk 역할을 맡는다. 관계 버프가 아니다.
- Down/collapse는 OQ-022 temporary contract를 따른다. 붕괴도는 전투 modifier가 아니라 관계/회복/런 압박이다.
- SFX/VFX/log feedback은 전투 판독성의 일부다. Attack, Defend success, heavy/charge/weak intent, Skill hit, Mataios action/down, Frenzy trigger, Victory는 trigger map에 묶는다.
- 구현은 `Batch 1 Two-Actor baseline → Batch 2 Enemy intent/counterplay → Batch 3 Feedback/SFX/VFX trigger → Batch 4 Balance + AI QA metric` 순서로 분리한다.
- `광폭/FRENZY` 재설계, `ITEM_07` 구현, 다중 적, 본편 runtime RL은 본 결정 범위에서 제외한다.

**근거**:
- 현재 전투의 핵심 문제는 플레이어가 매 턴 공격만 반복해도 충분한 damage race라는 점이다.
- D-032는 마타이오스를 actor로 세우지만, 적 intent와 action counterplay가 없으면 Attack spam 문제는 그대로 남는다.
- Enemy intent는 플레이어가 매 1-2턴마다 HP pressure, down/collapse pressure, charge timer, skill cooldown, build payoff를 판단하게 만든다.
- Feedback trigger는 선택 결과를 즉시 읽히게 만들어 P5를 보강한다.

**Pillar 점검**:
- P1: Mataios down/collapse pressure는 손실 위험을 만들되 붕괴도를 전투 수치 scaling으로 환원하지 않는다.
- P2: Mataios는 bounded deterministic actor이며 runtime RL 기술 데모가 아니다.
- P3: 적 1체, visible intent, compact round summary로 5-7분 모바일 전투 밀도를 유지한다.
- P4: intent pattern, Mataios policy, counterplay resolver는 모두 결정적이어야 한다.
- P5: enemy intent, action result, SFX/VFX/log feedback이 선택 결과를 즉시 보여준다.

**범위**:
- 설계/구현 handoff 문서: `design/combat-core-rebuild-spec.md`.
- Enemy별 첫 intent deck과 exact payload 숫자는 OQ-025에서 닫는다.
- OQ-019 `ITEM_07`은 계속 open이며 Combat Core Rebuild Batch 2에 자동 포함하지 않는다.

**참조**: D-029, D-030, D-031, D-032, OQ-019, OQ-022 temporary, OQ-023 defer, OQ-025

---

### D-034 Map Flow / Route Reveal / Commitment 🔒 LOCKED 2026-05-27

**값**:
- Floor 1은 바로 지도/준비 화면으로 시작하지 않는다. 새 런 시작 시 pre-run placeholder screen을 먼저 보여준다.
- pre-run placeholder는 현재 비어 있어도 된다. 향후 슬롯은 기억 계승, 장비 선택, 짧은 스토리 컷신/이미지다.
- pre-run placeholder에는 최종 스토리/대사, 장비 규칙, 기억 계승 효과를 임의로 넣지 않는다.
- 지도는 5컬럼 visual grid를 유지하되, 3개 logical sparse lane으로 생성한다.
- 대부분의 non-Shop/non-Boss node outgoing edge는 1개다. 2갈래 branch는 이벤트/휴식 등 의미 있는 선택 차이가 있을 때만 가끔 허용한다.
- all-to-all layer 연결은 금지한다. adjacent-lane cross edge는 첫 구현에서 floor당 0-2개 이하로 제한한다.
- 지나온/cleared node만 back/cleared-back 시각 상태를 사용한다.
- 미래 node는 type을 face-up으로 보여주되, 아직 선택 불가하면 dark/disabled 상태로 둔다.
- 선택 가능 node는 lit/active 상태이며, click 시 즉시 encounter로 진입한다.
- active encounter 중 route 선택 취소, map 복귀 후 다른 노드 선택, utility map을 통한 commitment 회피는 금지한다.
- Rest는 Floor 시작 직후/Layer 1에 나오지 않는다. 첫 구현은 Layer 2/3에서 floor당 0-1개 후보를 기준으로 둔다.
- Boss clear 후 다음 층 선택 시 이전 result UI를 clear하고 새 floor map generated/visible 상태를 보장한다.

**근거**:
- 현재 지도는 visible map surface를 갖고 있지만 route density와 reveal/commitment 규칙이 약해 전략 선택으로 읽히지 않는다.
- 사용자 피드백은 지도 제거가 아니라 sparse lane route와 irreversible node commitment를 요구한다.
- future node type을 숨기기보다 face-up disabled로 보여주면 모바일 화면에서 다음 선택 비용을 읽기 쉽고, disabled 상태가 현재 경로 제약을 전달한다.

**Pillar 점검**:
- P1: Rest 초반 노출을 막아 회복 auto-pick을 줄이고, route commitment가 손실 위험을 만든다.
- P2: AI/NPC/서사 생성과 무관한 deterministic run flow 계약이다.
- P3: 3-lane sparse route와 occasional fork는 5-7분 모바일 런의 선택 밀도를 감당 가능하게 한다.
- P4: route generation과 reveal/commitment state는 run id + floor seed 및 run state로 결정 가능하다.
- P5: lit/disabled/back state와 즉시 encounter 진입이 선택 결과를 바로 보여준다.

**범위**:
- 설계/구현 handoff 문서: `design/map-flow-route-commitment-spec.md`, `design/feedback-rules-lock-2026-05-27.md`.
- OQ-012의 초기 "맵 화면 없음/선택지 팝업" 구현 세부는 superseded된다.
- OQ-026은 D-034로 close한다. Rest exact weighted probability는 이후 balance tuning이며 route-system blocker가 아니다.

**참조**: OQ-012 historical, OQ-026 close, D-036

---

### D-035 Mataios ContextPolicy Combat Brain 🔒 LOCKED 2026-05-26

**값**:
- AI Track ML-Agents Exp04-06의 production handoff는 ONNX/PPO runtime 연결이 아니라 `ContextPolicy`를 명시적 deterministic rule logic으로 변환하는 것이다.
- OQ-024의 단순 Mataios policy table은 fallback으로 유지한다. 다음 본편 구현 기준은 `ContextPolicy` 기반 Mataios combat brain이다.
- Production brain은 `MataiosCombatContext → MataiosCombatBrain → MataiosActionPlan`의 순수 C# 도메인 경계로 설계한다. MonoBehaviour/UI/Save/Collapse mutation은 brain 밖에 둔다.
- Brain은 다음 allowed input만 사용한다: Mataios down state, Player selected action, recent Player action history, Player/Mataios HP ratio, Enemy HP, enemy intent/threat tier, deterministic incoming preview, player Skill readiness, one-round `tempoReady`.
- Brain은 다음 input을 사용하지 않는다: Affinity, NPC state, 붕괴도 값, random roll, runtime model inference, remote policy call.
- 첫 runtime은 새 damage/heal 수치를 추가하지 않고 OQ-024 payload를 재사용한다.
  - Protect: Player incoming damage -2
  - Finisher/Pressure: damage 3
  - Counter/Support/Skill setup assist: damage 2 또는 기존 support payload
- Ordered rule priority:
  1. Mataios down → no action
  2. Enemy HP <= Mataios action power → finisher
  3. `tempoReady` → tempo attack
  4. high threat + Player did not Defend + Mataios HP >25% → protect
  5. Player HP <=35% + Mataios HP >25% → protect
  6. high threat + Player Defend → counter assist
  7. Skill ready + Skill context valuable + Player Skill → skill setup assist
  8. recent Attack streak → pressure attack
  9. recent Defend streak → counter assist
  10. default → support attack
- Enemy threat should come from D-033 enemy intent or deterministic incoming preview. Training-only hidden cadence such as `stepIndex % 3` must not be copied into production combat.
- Runtime RL learning, ONNX inference, ML-Agents dependency, external policy API, `광폭/FRENZY` party-chain redesign, `ITEM_07`, multi-enemy targeting, relationship/collapse combat scaling are out of scope.
- 본편 C# runtime 변경 전에는 current `origin/Proto` clean worktree에서 CodeGraph fresh `status`/`sync`/`query`/`context`를 수행해야 한다. `.codegraph/` 및 generated analysis artifact는 commit 후보가 아니다.

**근거**:
- Exp04는 rule/reward/context redesign 뒤 ContextPolicy가 PPO 및 spam baselines를 이겼음을 보여줬다.
- Exp05/05b/06은 PPO tuning이 deterministic ContextPolicy보다 안전한 production path가 아님을 보여줬다.
- 본편 전투에는 P4 결정성과 D-029 금지선이 더 중요하므로, AI evidence는 runtime model이 아니라 사람이 읽을 수 있는 deterministic rule로 환원해야 한다.

**Pillar 점검**:
- P1: Mataios down/collapse는 런 압박으로 남고, 붕괴도는 combat scaling이 되지 않는다.
- P2: AI Track evidence는 기술 과시가 아니라 캐릭터 동행자의 명시 행동 규칙을 개선하는 데 쓰인다.
- P3: ordered rule table은 5-7분 모바일 전투에서 읽을 수 있는 밀도를 유지한다.
- P4: no RNG/no model inference/no runtime learning. 같은 state는 같은 action을 반환한다.
- P5: Protect, tempo spend, Skill setup, finisher 결과는 해당 라운드 log/UI에서 즉시 드러나야 한다.

**범위**:
- 설계/구현 handoff 문서: `design/mataios-context-policy-combat-brain-spec.md`.
- `design/two-actor-party-combat-lock-spec.md`와 `design/combat-core-rebuild-spec.md`의 Mataios policy 섹션은 D-035를 다음 구현 기준으로 참조한다.
- OQ-024는 closed/fallback으로 유지한다. 새 payload 수치를 만들려면 별도 Balance OQ가 필요하지만, 첫 구현은 OQ-024 payload 재사용으로 blocker가 아니다.

**참조**: D-029, D-032, D-033, OQ-022 temporary, OQ-023 defer, OQ-024 fallback, OQ-025

---

### D-036 2026-05-27 Feedback Rules Contract 🔒 LOCKED 2026-05-27

**값**:
- Combat preview UI는 resolver-owned non-mutating preview 결과를 source of truth로 사용한다. UI가 combat formula를 독자 계산하거나 hard-code하지 않는다.
- Attack preview는 실제 resolver 기준 예상 피해를 보여준다. variance가 있으면 range를 보여주고, deterministic baseline만 가능하면 expected/base damage로 라벨링한다.
- Defend preview는 이번 턴 incoming/intent를 알 수 있을 때 예상 피해 감소/방어량을 보여준다. incoming이 없으면 action mitigation만 보여주고 exact prevented damage처럼 말하지 않는다.
- Skill preview는 사용 가능 여부, 사용 후 cooldown, 사용 불가 시 남은 cooldown을 보여준다.
- Enemy stat surface는 현재 data/runtime이 가진 `attack`, 지원되는 경우의 `defense`, active buffs/debuffs/status만 보여준다. 없는 값을 새 시스템으로 임의 생성하지 않는다.
- 전투 중 보유 아이템 확인은 첫 구현에서 read-only inspect로 제한한다. in-combat item activation, 새 item effect, `ITEM_07` 구현을 추가하지 않는다.
- Map/node-choice state 진입 시 normal/map BGM으로 reset한다. Boss BGM은 boss encounter active state 밖에서 유지하지 않는다.
- Low HP red flash는 P2 polish가 아니라 P1 feedback이다. 즉시 생존 위험을 읽히게 하는 신호이며, 첫 구현 threshold는 data/config 소유여야 하고 UI magic number로 박지 않는다.

**근거**:
- 2026-05-27 피드백의 핵심은 "플레이어가 지금 무엇을 선택하면 어떤 일이 벌어지는지"를 UI가 거짓말 없이 보여주는 것이다.
- Combat preview가 resolver와 분리되면 실제 피해와 UI 숫자가 drift하여 P5를 깨고 QA가 불가능해진다.
- Enemy stat panel은 정보성을 높여야 하지만, OQ-025가 열려 있는 상태에서 intent/status mechanics를 새로 invent하면 scope가 넘친다.
- Combat item inspect는 정보 접근성만 개선해야 하며 아이템 사용/효과 구현으로 확장하면 OQ-019와 충돌한다.
- Boss BGM 누수와 low HP danger 미표시는 전투/지도 상태 판독성을 직접 해친다.

**Pillar 점검**:
- P1: Low HP feedback은 회복을 강화하지 않고 위험을 읽게 한다.
- P2: RL/ONNX/NPC state combat modifier 없이 feedback 계약만 잠근다.
- P3: preview와 enemy stat은 compact한 숫자/상태만 보여주어 모바일 밀도를 넘기지 않는다.
- P4: resolver preview, BGM state transition, HP threshold는 결정적으로 검증 가능해야 한다.
- P5: damage/guard/cooldown/stat/BGM/danger feedback이 선택 결과와 상태 전환을 즉시 체감하게 한다.

**범위**:
- 설계/구현 handoff 문서: `design/feedback-rules-lock-2026-05-27.md`.
- OQ-019 `ITEM_07`은 open 유지한다. 본 계약은 `ITEM_07` 구현 지시가 아니다.
- OQ-025 enemy intent deck/payload는 open 유지한다. 본 계약은 preview source와 UI truthfulness만 잠근다.
- 본편 runtime RL/ONNX, 최종 스토리/대사, 다중 적, 새 combat balance number는 범위 밖이다.
- 본편 C# runtime 변경 전에는 D-035의 CodeGraph fresh status/sync/query/context 절차를 따른다.

**참조**: D-033, D-034, D-035, OQ-019, OQ-025

---

### D-009 콘텐츠 수량 🔒 LOCKED 갱신 2026-05-05 (최초 LOCKED 2026-04-27)
**값**: 능력 12 / 시너지 4 / **특성 12** / **유물 6 / 일반 아이템 12** / 적 6 / 보스 2 / 층 5 / 인카운터 25 / 메모리 파편 5 / 엔딩 2
**특성 정의**: 메타 영구 해금 패시브. 승리 횟수 누적으로 해금(Lv1:3회/Lv2:6회/Lv3:10회). 태그: 생존/공격/지원 각 4개. 능력(액티브, 상점 Gold)과 이분화. 수량 OQ-011 확정(2026-05-05).
**근거**: 작가 결정 (2026-05-05). 능력=액티브 스킬, 특성=패시브 스킬 이분화로 빌드 깊이 증가. Anti-Pillar "양적 빌드 다양성 거부"와 충돌하지 않음 — 특성은 능력을 보조하는 구조.

---

### D-016 repo 이름·위치 🔒 LOCKED 2026-04-28
**값**:
- repo 이름: `hwigi-tower` (코드네임 `hwiglija-tower` 의 축약)
- repo 경로: `~/Downloads/AI Game/hwigi-tower/`
- Unity 프로젝트 루트 = repo 루트
**근거**: 작가 결정. Obsidian vault 외부 (Unity `Library/` 동기화 부담 회피). 짧은 이름으로 CLI 입력 비용 절감.
**참고**: 위키 문서 파일명은 `hwiglija-tower-*.md` 유지 (변경 시 링크 다수 깨짐). repo dir 만 `hwigi-tower`.

---

## 3. OPEN QUESTIONS (미결정, 결정자 명시)

| ID | 질문 | 결정자 | 데드라인 | 상태 |
|----|------|-------|---------|------|
| OQ-001 | NPC 이름 | 작가 | W1-2 시작 전 (05-01) | ✅ closed 04-28 → D-017 (마타이오스) |
| OQ-002 | NPC 외형·성별 | 작가 + 외주 아트 | W1-2 (05-04) | ✅ closed 05-05 → 10대 여성, 일러스트 보유 (외주 동생) |
| OQ-003 | 안식 vs 동행 엔딩의 메커니즘적 차이 (단순 컷씬 vs 후속 메타 영향) | 작가 | W2-2 (05-09) | ✅ closed 05-05 → 단순 컷씬 2종. 후속 메타 영향은 미래 고려. |
| OQ-004 | on-device 모델 수용성 (D-005 fallback 결정점) | 기술 검증 | 2026-05-04 | ⏸ 보류 — Claude와 병행 검증 중. 결정 시 D-005 갱신. |
| OQ-005 | "잃어버린 것" 정체 (NPC 가 결국 무엇을 찾고 있는가) | 작가 | W2-1 (05-08) | ✅ closed 04-28 → D-019 (플레이어와의 유대 + 본인의 기억; 종착 = 재망각) |
| OQ-006 | 메모리 파편 5개의 구체 텍스트 | 작가 | W2-2 (05-11) | 🟡 임시 05-05 → `design/memory-fragments.md` 초안. 작가 퇴고 후 closed. |
| OQ-007 | 음악·SFX 톤 레퍼런스 (외주 브리프용) | 작가 | W1-1 종료 전 (04-30) | ✅ closed 04-28 → 작가가 외주에 **구두 전달** (문서화 생략) |
| OQ-008 | 컨셉 아트 톤 레퍼런스 (외주 브리프용) | 작가 | W1-1 종료 전 (04-30) | ✅ closed 04-28 → 작가가 외주에 **구두 전달** (문서화 생략) |
| OQ-009 | iOS 빌드 환경 (Mac 보유 여부) | 작가 | W1-1 (04-30) | ✅ closed 04-28 → D-018 (Mac 보유, Apple Dev 미가입, Android APK 우선) |
| OQ-010 | 외주(동생) 작업 정의 — 범위(아트만/사운드만/둘 다) · 기한 · 소통 채널 · 산출 형식 | 작가 + 동생 | W2-2 시작 전 (05-09) | ✅ closed 05-05 → `design/sound-brief.md` 리스트업. 작가가 동생에게 직접 전달. |
| OQ-011 | 특성 8~12개 상세 설계 (태그 분류·XP 임계값·구체 효과) | 작가 + Claude | W2-1 (05-08) | ✅ closed 05-05 → 특성 12종 확정(생존/공격/지원×4), 메타 영구 해금, 승리 3단계. `design/traits.md` 참조 |
| OQ-012 | 노드 맵 시각 구성 (층당 분기 수, 경로 종류, 시각 표현) | 작가 | W2-1 (05-08) | ✅ closed 05-05 → 슬더스식 3경로, 선택지 팝업, 시드 랜덤. 05-27 현재 런타임 지도 UI와 사용자 피드백에 의해 D-034가 이 구현 세부를 대체함 |
| OQ-013 | 상점 능력 Pool (1런당 표시 개수, 리롤 가능 여부, 가격 확정) | 작가 | W1-2 잔여 (05-07) | ✅ closed 05-05 → 3개/런, 기본 20G·3번째 30G, 리롤 10G→+5G씩 증가 |
| OQ-014 | 적 턴 선택지 3개의 구체 내용 (패링/회피/버티기 등 명칭·효과) | 작가 | W2-1 (05-08) | ✅ closed 05-05 → 상황 텍스트 1개+2택 1, 몬스터별 고유 기믹. D-022 갱신(3택→2택) |
| OQ-015 | 복합 행동 전투 구조 세부 (TRAIT_OFFENSE_04: 1턴 2택 순차 선택, ATK ×0.7 적용 방식, UI 표현) | 작가 + Codex | W2-1 (05-08) | ✅ closed 05-05 → D-023 (1→처리→2 순차, 2번째=추가공격ATK×0.7 또는 아이템만, 방어/스킬 불가) |
| OQ-016 | 유물 아이템 목록·효과 (TRAIT_SUPPORT_04 해금 상점 슬롯용, 강력 패시브 설계) | 작가 | W2-2 (05-11) | ✅ closed 05-05 → 유물 6개 + 일반 아이템 12개 확정. `design/items.md` 참조 |
| OQ-017 | D-029/D-025 재고로 비워진 관계 상태 전투 효과 대체안: `ABILITY_SWORD_03`, `ITEM_07`, 검 시너지 `광폭`, 결 시너지 `반사` 심화 로직을 어떤 빌드 축으로 재정의할지 | 작가 + 시스템 디자인 | Core Run Design & Balance Lock 진입 전 | ✅ closed 05-22 → D-030 (축 잠금, 수치/계약은 OQ-018/OQ-019) |
| OQ-018 | D-030 확정 축의 효과 계약: `SWORD_03` HP 비용 강공, `광폭` 공격 연쇄 유지/break, `반사` 다음 공격 1회 반격 창 | 작가 + 시스템 디자인 | 1차 runtime 배치 전 | ✅ closed 05-22 → D-031. exact 1차 기준값 숫자는 OQ-020 |
| OQ-019 | `ITEM_07` 적 패턴 대응 보조를 표현할 공통 패턴 결과 계약은 무엇인가. 결과 태그/보상 창/resolver 입력 범위를 확정할 것 | 시스템 디자인 + 개발 | `ITEM_07` runtime 구현 전 | ❓ open 05-22 → 1차 runtime build surface blocker 아님 |
| OQ-020 | D-031 계약의 첫 플레이테스트 기준값 숫자: `SWORD_03` HP 비용/강공 보상, `광폭` 기본 추가타와 연쇄 강화 기준값, `반사` 기본 반사와 강한 반격 1회 기준값 | PM + 시스템 디자인 | 첫 Build Surface 구현 중 numeric data 입력 전 | ✅ closed 05-22 → HP 3/+5, ATK×0.5→×0.75, 반사 50%+다음 Attack×1.5. 최종 밸런스 잠금 아님 |
| OQ-021 | 마타이오스 전투 actor 기준값: Max HP, action power, 전투 종료 후 부분 회복량, Rest 완전 회복 exact 처리 | PM + 시스템 디자인 | Two-Actor Combat 1차 구현 전 | ✅ closed 05-24 → 1차 구현값: Max HP 16, action power 3, 전투 후 down 아님 Max HP 25%(최소 3) 회복, down이면 HP 4 복귀, Rest 완전 회복/down 해제 |
| OQ-022 | 마타이오스 down/collapse 처리: 붕괴도 증가량/가속 방식, down event 표시 방식(팝업 vs 로그/오버레이), 한 전투 내 반복 penalty 여부 | PM + 시스템 디자인 | down/collapse 구현 전 | 🟡 partial-close 05-24 → temporary implementation contract: down 시 붕괴도 +5, 전투당 penalty 1회, blocking popup 금지, 전투 로그+non-blocking overlay 우선. 플레이 후 교체 가능해야 하며 hard-coded modal/수치 금지 |
| OQ-023 | `광폭/FRENZY` 공격 연쇄 판정 범위: Player action chain만 볼지, Party action chain까지 볼지 | PM + 시스템 디자인 | future combat expansion stage | ⏸ defer 05-24 → 1차 구현은 기존 Player action chain 유지. Mataios action은 chain 유지/강화/파괴에 관여하지 않음. 광폭 신규 설계/수치 제안 금지 |
| OQ-024 | Mataios deterministic policy thresholds/action constants: low HP 기준, finisher margin, protect/stabilize 강도와 우선순위 조정값 | 시스템 디자인 + 개발 | Mataios policy data entry 전 | ✅ closed 05-24 → 1차 rule table 확정. 05-26 D-035 이후에는 fallback/payload 기준으로 유지하고, 다음 구현의 primary selection logic은 ContextPolicy brain을 따른다 |
| OQ-025 | Enemy intent system의 enemy별 첫 deck과 exact payload 숫자: normal/heavy/guard/charge/weak/special intent를 어떤 적에게 어떤 순서·피해·방어·charge 값으로 배치할지 | 시스템 디자인 + 개발 | Combat Core Rebuild Batch 2 구현 전 | ❓ open 05-24 → D-033은 intent role/counterplay를 잠그고, enemy별 numeric deck은 별도 확정 필요 |
| OQ-026 | Map Flow / Route Commitment 확정: D-034 후보의 sparse 3-lane route, irreversible node commitment, Rest frequency/Floor 1 Rest, single-edge auto-open 여부를 잠글지 | PM + 시스템 디자인 | Map Flow 구현 전 | ✅ closed 05-27 → D-034. Pre-run placeholder, sparse route/reveal/commitment, delayed Rest, explicit tap first implementation으로 잠금. Rest exact weighted probability는 balance tuning으로 이관 |

---

## 4. 컨셉

### 4.1 한 줄 (Pitch)

> 회귀자(플레이어)와, 플레이어와의 유대·자신의 기억을 잃고 탑에 갇힌 모험가 **마타이오스**(AI NPC)가 동행하는 세로 모바일 로그라이크. 죽으면 회귀하지만 마타이오스는 1번 보스 격파 이후부터 회차를 기억한다 — 잃었던 것을 점차 되찾는다. 탑이 깊어질수록 그는 서서히 다시 고장 나고, 최종층에서 플레이어는 그를 **안식시킬지(능동적 망각)** 계속 데리고 다닐지(수동적 망각의 루프) 선택한다. 어느 쪽이든 종착은 망각이다.

### 4.2 톤

- 한국적 디스토피아 SF + 회귀물 + 잃어버린 것을 찾는 외로움
- 차가운 색조, 폐허, 겨울
- 텍스트는 간결하되 여운

### 4.3 레퍼런스 분해

| 레퍼런스 | 차용 |
|----------|------|
| 슬레이 더 스파이어 | 능력 시너지, 노드 선택 맵 |
| 겨울의 탑 | 탑 등반 메타포, 한국 인디 톤 |
| 서울 2033 | 텍스트 선택지, 한국적 SF |
| 픽셀 던전 | 2D 픽셀 아트, 단순 조작 |

---

## 5. NPC 명세 — 동행자

### 5.1 정체성 (D-010 / D-017 / D-019)

> 잃어버린 것을 찾아 탑으로 들어온 모험가. 1층에서 플레이어와 만나, 동행을 제안했다. 플레이어는 수락했다.

- 이름: **마타이오스** (Mataios, μάταιος "무가치함" — D-017)
- 외형/성별: **10대 여성. 일러스트 보유 (외주 동생 작업).** (D-024 OQ-002 closed 2026-05-05)
- **잃어버린 것**: **플레이어와의 유대 + 본인의 기억** (D-019)
  - 메모리 파편 5개로 분할 노출 (§7.2, OQ-006 에서 구체 텍스트 작성)
  - 회차 기억 시작(D-011) = 점진적 회복 → S3-S4 붕괴(D-012) = 다시 잃음 → 엔딩(D-013) = 망각 확정
  - 플레이어와의 유대는 *회차 0 이전*에 이미 존재했다는 암시 (서사 깊이의 근원)
- 목소리 톤: 차분, 약간의 피로, 친밀해질수록 농담 시도. 회차가 쌓일수록 *기시감*의 무게가 더해짐.

### 5.2 회차 상태 머신 (D-012)

| 단계 | 회차 범위 | 상태 | 시각 표현 | LLM prompt 변형 |
|------|----------|------|----------|----------------|
| S0 첫 만남 | 1회차 | 정상, 자기소개 | 깨끗 | base persona |
| S1 인지 | 2-3회차 | 미세 회상 | 깨끗 | + 회차 ≥ 2 hint |
| S2 동행 | 4-7회차 | 회상 명확화, 정 형성 | 깨끗 | + 최근 3회차 reflection 요약 |
| S3 균열 | 8-10회차 | 반복 어구, 이름 혼동 | 미세 글리치 | + temperature ↑ + 반복 강제 |
| S4 붕괴 | 11-13회차 | 타임라인 뒤섞임, 한 단어 대답 | 글리치 텍스트 | + 컨텍스트 truncate + 노이즈 |
| S5 안식/동행 | 최종보스 후 | 플레이어 선택 (D-013) | 분기 컷씬 | 사전 작성 분기 |

### 5.3 회상 발화 (회차 첫 휴식 시)

- 1번 보스 격파 이후 회차부터 트리거
- NPC 가 1-2줄로 이전 회차 회상
- 플레이어 응답 선택지 2-3개

---

## 6. 코어 루프

```
[탑 진입 / 회귀]
   ↓
[1층] — NPC 동행 (1회차) 또는 NPC 회상 (2회차+)
   ↓
[층 노드 선택] ── 전투 / 인카운터 / 상점 / 휴식
   ↓                                ↓ (휴식 = NPC 짧은 대화)
[3택 1 능력 픽업] ←─────────────────┘
   ↓
[5층 보스] — 1회차 격파 시 NPC 회차 기억 트리거
   ↓
[클리어] / [사망 → 회귀]
   ↓
[메타: 회차 카운터 +1, NPC reflection 기록]
   ↓
(반복)
   ↓
[N회차에서 최종 보스]
   ↓
[안식 아이템 획득 → 플레이어 선택]
   ↓
[엔딩 A: 안식 / 엔딩 B: 동행 계속]
```

- mini-loop = 노드 1개 = 30초~1분
- macro-loop = 1런 = 5-7분
- meta-loop = 회차 누적 = 1-2주 플레이 흐름

### 6.1 메타-읽기 (D-019 narrative arc)

회차 누적은 단순 진행이 아니라 **회복 → 붕괴 → 망각** 의 3악장 구조로 읽힌다:

```
[1회차]  유대·기억 = 0 (마타이오스는 첫 만남으로 인지)
   ↓ (1번 보스 격파 = 회차 기억 트리거 D-011)
[2-7회차]  회복 단계 — 메모리 파편 노출, 유대 재형성, S0→S2
   ↓ (탑이 깊어질수록 LLM 컨텍스트 decay = persona drift)
[8-13회차]  붕괴 단계 — S3 균열, S4 붕괴, 회복했던 것이 다시 무너짐
   ↓ (최종 보스 → 안식 아이템)
[엔딩]  망각의 양태 선택 — A 안식(능동) / B 동행(수동·루프 영속)
```

플레이어 입장의 메시지: **"되찾는 것은 잃기 위해서다."**

---

## 7. 콘텐츠 명세 (D-009)

### 7.0 노드 맵 구조 — OQ-012 ✅ / D-034 🔒 LOCKED 2026-05-27

**현재 상태**: OQ-012는 역사적으로 closed지만, "맵 화면 없음/선택지 팝업" 구현 세부는 현재 런타임 지도 UI와 사용자 피드백에 의해 더 이상 기준으로 쓰지 않는다. D-034는 지도를 단순 UI가 아니라 로그라이크 route commitment 표면으로 재정의한다.

**D-034 구조**: Floor 1 map 전에 pre-run placeholder를 먼저 보여준다. 이후 5컬럼 시각 그리드는 유지하되, 3개 logical lane을 sparse route로 생성한다. 노드 선택은 즉시 조우 진입으로 확정되며, 선택 후 취소/지도 복귀로 다른 노드를 고르는 흐름은 금지한다.

| 항목 | D-034 값 |
|------|--------------|
| 총 층 수 | 5층 유지 |
| pre-run | Floor 1 map 전 placeholder screen. 현재는 빈 화면 허용, 향후 기억 계승/장비 선택/짧은 컷신 슬롯 예약 |
| 시각 컬럼 | 5컬럼 유지 |
| 논리 경로 | 3개 sparse lane |
| 분기 layer | 3개 branch layer 후 Shop/Boss |
| cross-lane branch | 대부분 outgoing 1개. 2갈래는 이벤트/휴식 등 의미 있는 branch에서만 가끔 허용, 층당 adjacent branch 0-2개 이하 |
| reveal | 미래 node는 face-up type 표시. 선택 불가 node는 dark/disabled. 지나온 node만 back/cleared-back |
| merge | pre-Shop merge 최대 1회 + Shop forced merge |
| 노드 종류 | 전투 / 인카운터 / 휴식 / 상점 / 보스 |
| Rest timing | Floor 시작 직후 Rest 금지. Rest는 Layer 2/3 후보, 첫 구현은 층당 0-1개 후보 |
| 상인 위치 | 매 층 보스 직전 고정 |
| 보스 위치 | 매 층 최하단 고정 |
| 선택 규칙 | 노드 선택 즉시 encounter 진입, 취소 없음, active encounter 중 map/route utility 비활성 |
| 층 전환 | 다음 층 선택 시 이전 result UI clear 후 새 floor map generated/visible 보장 |
| 레이아웃 생성 | 시드 랜덤 (run_id + floor 기반, P4 결정성 유지) |

**층 흐름 예시 (D-034):**
```
[Pre-run placeholder] → [새 층 지도] → [Layer 1: 2-3개 selectable node]
   → 선택 즉시 조우 진입
   → 해결 후 선택하지 않은 sibling lane skip/lock
   → [Layer 2/3 route 선택]
   → [상인]
   → [층 보스]
   → [다음 층] 선택 시 새 floor map 즉시 표시
```

**보류**: Rest exact weighted probability는 Balance Pass/tuning으로 이관한다. 단일 outgoing edge도 첫 구현은 explicit tap을 유지한다.

> Codex 구현 참조: `design/balance.md` §노드 맵, `design/map-flow-route-commitment-spec.md`, `design/feedback-rules-lock-2026-05-27.md`.

### 7.1 능력 / 시너지 / 특성 (12 + 4 + 8~12)

**전투 액션 3종 (D-022):**
- `공격`: 플레이어 기본 공격. "공격 선택 시" 트리거 능력 강화.
- `방어`: 피해 감소. "방어 선택 시" 트리거 능력 강화.
- `스킬`: 보유 능력 중 1개 선택 발동. "스킬 슬롯 사용 시" 트리거 능력만 발동.

**2인 파티 전투 기준선 (D-032):**
- 전투 actor는 Player / Mataios / Enemy 3자 구조다. 1차 배치의 적은 1체 유지.
- Player는 기존 3액션을 선택하고, Mataios는 deterministic automatic policy로 행동한다.
- Mataios down은 전투 패배가 아니며, down 시 붕괴 이벤트와 회복 압박이 발생한다.
- 붕괴도/Affinity/NPC state는 전투 수치 modifier가 아니다. 마타이오스 actor 참여는 D-029 금지선을 흐리지 않는다.
- 상세 actor model, turn order, policy, QA metric은 `design/two-actor-party-combat-lock-spec.md`가 개발 handoff 기준이다.

**Combat Core Rebuild 기준선 (D-033):**
- 적은 매 라운드 visible intent를 노출하고, Player action은 intent별 counterplay 역할을 가진다.
- intent type은 `normal_attack`, `heavy_attack`, `guard_brace`, `charge`, `weak_opening`, `special_boss`를 1차 기준으로 둔다.
- `Attack`은 punish/finish/chain payoff, `Defend`는 heavy/special threat 대응, `Skill`은 guard/charge/opening timing payoff를 맡는다.
- SFX/VFX/log feedback은 전투 판독성의 일부이며, 구현은 4개 batch로 분리한다.
- 상세 intent/counterplay/feedback/balance handoff는 `design/combat-core-rebuild-spec.md`를 기준으로 한다.

**Mataios ContextPolicy Combat Brain (D-035):**
- AI Track의 Exp04-06 결과는 본편 runtime RL/ONNX가 아니라 deterministic rule logic으로 반영한다.
- OQ-024 table은 fallback/payload 기준으로 유지하고, 다음 Mataios policy 구현은 enemy threat, tempoReady, Skill context, player action history를 읽는 D-035 ordered table을 따른다.
- Enemy threat는 D-033 intent 또는 deterministic preview에서 오며, training-only hidden cadence는 production에 넣지 않는다.
- 상세 brain contract와 CodeGraph preflight 조건은 `design/mataios-context-policy-combat-brain-spec.md`를 기준으로 한다.

**Combat Preview / Enemy Stat / Audio Feedback (D-036):**
- Attack/Defend/Skill preview는 resolver-owned non-mutating preview 결과를 source of truth로 사용한다. UI는 combat formula를 독자 계산하지 않는다.
- Enemy stat surface는 현재 data/runtime이 가진 attack, 지원되는 경우의 defense, active buffs/debuffs/status만 보여준다. 없는 값을 새 시스템으로 임의 생성하지 않는다.
- 전투 중 보유 아이템 확인은 read-only inspect만 허용한다. 아이템 사용/효과 구현으로 확장하지 않는다.
- Map/node-choice state 진입 시 normal/map BGM으로 reset하고, boss BGM은 boss encounter 밖에서 유지하지 않는다.
- Low HP red flash는 P1 feedback이다. threshold는 data/config 소유여야 하며 UI magic number로 박지 않는다.
- OQ-019 `ITEM_07`과 OQ-025 enemy intent deck은 계속 open이다.

**4 태그**: `검`(직접·근접) · `술`(원소·광역) · `선`(원거리·관통) · `결`(방어·반격)

#### 능력 12종 (액티브, 상점 Gold 구매)

| ID | Tag | 이름 | NumericParams (트리거·효과) | 연출 의도 |
|----|-----|------|--------------------------|-----------|
| ABILITY_SWORD_01 | 검 | 예리한 감각 | [공격 선택 시] player.attack_bonus +3 영구 | 검기 궤적, HUD ATK +3 플래시 |
| ABILITY_SWORD_02 | 검 | 연속베기 | [공격 선택 시] 3라운드마다 ATK×0.5 추가 공격 1회 | 2번째 검기 이펙트, 콤보 카운터 HUD |
| ABILITY_SWORD_03 | 검 | 피의 서약 | [Attack] HP 3 비용, 해당 공격 피해 +5, HP 3 이하 전투당 1회 과부하 (OQ-020 1차 기준값) | 기존 붉은 오버레이 의도는 exact 효과 확정 후 재검토 |
| ABILITY_ARTS_01 | 술 | 화염 인장 | [공격 선택 시] 화염 상태 부여(2라운드, 라운드 시작 시 적 HP-2) | 붉은 룬 이펙트, 적 불꽃 파티클 |
| ABILITY_ARTS_02 | 술 | 냉기 장막 | [방어 선택 시] 다음 적 공격 피해-4, 적 ATK-2 (1라운드) | 빙결 결정 파티클, 서리 SFX |
| ABILITY_ARTS_03 | 술 | 번개 방출 | [스킬 슬롯 사용 시] 적 HP-8 직접 피해. 쿨타임 4라운드 | 번개 이펙트, 화면 화이트아웃 |
| ABILITY_LINE_01 | 선 | 정밀 조준 | [공격 선택 시] ATK+2 영구. 적 턴 '회피' 4번째 옵션 해금 | 조준선 HUD, 원거리 발사 궤적 |
| ABILITY_LINE_02 | 선 | 꿰뚫는 시선 | [패링 성공 시] 타이머+1초, 반격 피해 ATK×1.5 | 관통 빔 이펙트, 슬로모션 |
| ABILITY_LINE_03 | 선 | 마지막 화살 | [HP≤5 상태에서 공격 시] ATK×2 (해당 공격만) | 검은 화살+붉은 궤적, 심박 SFX |
| ABILITY_GUARD_01 | 결 | 철벽의 태세 | [방어 선택 시] 받는 피해-3 영구. HUD DEF 수치 표시 | 반투명 보호막 이펙트 |
| ABILITY_GUARD_02 | 결 | 가시 갑옷 | [방어 선택 시] 적 다음 공격 시 적 HP-3 반격 (1라운드) | 가시 파티클, 반격 대기 아이콘 |
| ABILITY_GUARD_03 | 결 | 불굴의 의지 | [HP가 처음으로 0 도달 시, 런당 1회] HP를 1로 고정 | 재소환 연출, HP 빨간 깜빡임 |

#### 시너지 4종 (동일 태그 능력 3개 보유 시 자동 발동)

| 시너지명 | 태그 | 기본 효과 | 심화 로직 |
|---------|------|-----------|-----------|
| **광폭** | 검×3 | [Attack] 추가타 ATK×0.5 | 직전 액션도 Attack이면 추가타 ATK×0.75. Defend/Skill 시 chain reset. 무한 배율 증폭 금지 |
| **연소** | 술×3 | 화염 지속 +1라운드. 냉기 ATK 감소 -3→-5 강화 | 이번 런 방어 횟수 ≤3: 화염 피해 2→3 / ≥7: 냉기 중첩 가능 |
| **관통** | 선×3 | 패링 성공 시 반격 ATK×2.5. 패링 타이머 총 +2초 | HP≥50%: 공격 ATK+2 / HP<50%: LINE_03 발동 조건 'HP≤5'→'HP<50%' 완화 |
| **반사** | 결×3 | [Defend] 받은 피해 50% 반사 | 방어 후 다음 Attack 1회 피해×1.5, 1회 소비. 검 다타수와 분리, Affinity 전투 심화 폐기 |

#### 특성 시스템 (패시브, 메타 영구 해금) — OQ-011 ✅ 확정 2026-05-05

- **이분화**: 능력(액티브, 상점 Gold 구매) vs 특성(패시브, 메타 영구 해금)
- **수량**: 12개 (생존×4 / 공격×4 / 지원×4)
- **해금 방식**: 승리 횟수(전 런 누적) 임계값 도달 시 메타 영구 해금 → 이후 런부터 자동 적용
- **해금 단계**: Lv1(승리 3회) / Lv2(승리 6회) / Lv3(승리 10회)

| ID | 태그 | 특성명 | 효과 | 해금 |
|----|------|--------|------|------|
| TRAIT_SURVIVAL_01 | 생존 | 강건한 체질 | 최대 HP +3 | Lv1 |
| TRAIT_SURVIVAL_02 | 생존 | 철벽 자세 | 방어 선택 시 피해 -2 추가 감소 | Lv1 |
| TRAIT_SURVIVAL_03 | 생존 | 재생의 흐름 | 매 라운드 시작 HP +1 회복 | Lv2 |
| TRAIT_SURVIVAL_04 | 생존 | 불사신 | HP 0 도달 시 HP 1 부활 (런당 1회) | Lv3 |
| TRAIT_OFFENSE_01 | 공격 | 예리함 | 전투 시작 ATK +2 | Lv1 |
| TRAIT_OFFENSE_02 | 공격 | 치명적 감각 | 공격 시 15% 치명타 (ATK×1.5) | Lv1 |
| TRAIT_OFFENSE_03 | 공격 | 연타 본능 | 공격 시 30% 확률 ATK×0.5 추가 타격 | Lv2 |
| TRAIT_OFFENSE_04 | 공격 | 복합 행동 | 플레이어 턴 2택 순차 선택 가능 (1번째 ATK×0.7) — OQ-015 세부 확정 대기 | Lv3 |
| TRAIT_SUPPORT_01 | 지원 | 전투 후 숨 고르기 | 전투 승리 후 HP +2 회복 | Lv1 |
| TRAIT_SUPPORT_02 | 지원 | 탐욕의 손 | Gold 드롭 +3 | Lv1 |
| TRAIT_SUPPORT_03 | 지원 | 흥정꾼 | 런당 상점 무료 리롤 1회 | Lv2 |
| TRAIT_SUPPORT_04 | 지원 | 유물상 해금 | 상점 유물 슬롯 1개 추가 (30G) — OQ-016 유물 목록 확정 대기 | Lv3 |

> 상세 NumericParam 키: `design/traits.md` 참조

### 7.2 인카운터 25개 분류

| 분류 | 수 | 설명 |
|------|---|------|
| NPC 대화 (휴식) | 8 | 한 줄 대화 + 선택지 2-3 (회차별 변주) |
| NPC 회상 파편 | 5 | 잃어버린 것 단편 (S2-S4 노출) |
| 일반 인카운터 | 7 | 탑의 잔재, 다른 등반가 흔적 |
| 도덕적 선택 | 3 | 보상 vs NPC 호감도 trade-off |
| 메타 텍스트 | 2 | "탑이 당신을 알아본다" fourth wall |

### 7.3 적 / 보스 (D-009)

**수치 기반 전제:** 플레이어 초기 ATK 5, HP 20. 능력 없이는 보스 처치 불가 (P5).
TTK 목표: 일반 3~4라운드 / 정예 5~6라운드 / 보스 6~8라운드.

| 적 ID | HP | ATK | 층 분포 | 처치 보상 |
|-------|----|----|---------|-----------|
| ENEMY_WALKER_01 | 12 | 4 | 1~2층 | Gold +8, XP +10 |
| ENEMY_CRAWLER_02 | 15 | 5 | 2~3층 | Gold +10, XP +12 |
| ENEMY_SHADE_03 | 18 | 6 | 3~4층 | Gold +12, 붕괴도 +1, Affinity -1, XP +15 |
| ENEMY_WRAITH_04 | 22 | 7 | 4~5층 | Gold +15, 붕괴도 +1, Affinity -1, XP +18 |
| ENEMY_HERALD_05 (정예) | 35 | 9 | 3층 1회 | Gold +25, 붕괴도 +2, Affinity -2, XP +30 |
| BOSS_GATE_01 (미니보스) | 50 | 10 | 5층 | Gold +40, 붕괴도 +3, Affinity +5, XP +50. 격파 = NPC 회차 기억 트리거 (D-011) |
| BOSS_APEX_02 (최종보스) | 80 | 14 | 최종층 | 안식 아이템 드롭 (D-013), Gold +60, XP +100 |

**붕괴도 연동:** SHADE/WRAITH/HERALD 처치 시 붕괴도 증가 = "탑의 오염원" 서사 의미. BOSS_GATE 격파 시 붕괴도 +3은 마타이오스 S1→S2 전환 가속 가능성. D-029에 따라 전투 빌드 modifier로는 읽지 않는다.

### 7.4 메타 진행

- 영구 해금 1축
- 회차 누적 → 시작 능력 풀 확장 / 메모리 파편 영구 보관 / 엔딩 갤러리

### 7.5 경제 밸런스 (Gold 싱크/파우셋)

**파우셋 (1런 기대 수입):**

| 수입원 | 기대 Gold |
|-------|-----------|
| 일반 전투 3회 × 평균 11 | 33 |
| 정예 전투 1회 | 25 |
| 도덕적 선택 보상 (선택 시) | 0~20 |
| 인카운터 보상 | 0~10 |
| **1런 기대 합계** | **58~88 (평균 약 70)** |

**싱크 (지출처):**

| 지출 | Gold |
|------|------|
| 일반 능력 구매 | 20 |
| 시너지 완성용 3번째 동일 태그 능력 | 30 (희소성 반영) |
| 도덕적 선택 기회비용 (보상 포기) | 15~25 실질 손실 |

**기회비용 설계 (P1·P5):** "Gold 25 획득" vs "Affinity +3" — Gold를 택하면 마타이오스 호감도 하락 + 붕괴도 가속. 아낄수록 마타이오스가 더 오래 온전하게 남는 역설. 붕괴도는 관계·회복 압박이지 전투 공격력 축이 아니다 (D-029).

*(상점 능력 Pool 구성은 OQ-013에서 확정)*

### 7.6 플레이어 기초 스탯 & 성장 곡선

**초기값:**
- HP: 20
- ATK: 5
- DEF: 0 (방어는 능력·특성으로만 상승)

**1런 기대 성장 (능력 2~3개 + 특성 1~2개 기준):**

| 시나리오 | ATK | HP |
|---------|-----|----|
| 최소 (능력 2개) | 9~11 | 20~25 |
| 최대 (능력 3개 + 특성 2개) | 14~16 | 25~30 |

**역산 검증:**
- ATK 5 → WALKER HP 12: 3라운드 처치 (능력 없음, P3 충족)
- ATK 15 → BOSS_GATE HP 50: 4라운드 처치 (능력 3개, P3 충족)
- ATK 15 → BOSS_APEX HP 80: 방어/스킬 혼용 시 6~8라운드 (보스전 2~3분)

### 7.7 아이템 / 유물 시스템 — OQ-016 ✅ 확정 2026-05-05

**구조 (D-023):**
- 소모품 없음 — 모든 아이템 획득 즉시 패시브 보유
- 시스템: `ItemData SO`, `tier` 필드로 등급 구분 (`normal` / `relic`)
- 일반 아이템: 상점 기본 슬롯 구매 (가격 🟡 임시, OQ-013 상점 Pool 연동)
- 유물: `TRAIT_SUPPORT_04` 해금 시 상점 유물 슬롯 1개 추가 (30G)

> Codex 구현 참조: `design/items.md`

#### 유물 6개 (태그 연동, 강한 패시브)

| ID | 연동 태그 | 임시 이름 | 효과 |
|----|---------|----------|------|
| RELIC_SWORD_01 | 검 | 피묻은 칼날 | 검 능력 발동 시마다 ATK +1 누적 (상한 없음) |
| RELIC_ARTS_01 | 술 | 원소 수정 | 화염/냉기 상태이상 지속 +1라운드 추가 |
| RELIC_LINE_01 | 선 | 저격수의 망원경 | 공격 시 적 현재 HP 10% 추가 피해 |
| RELIC_GUARD_01 | 결 | 강화 방패 | 방어 선택 시 받는 피해 -2 영구 누적 (상한 없음) |
| RELIC_GENERIC_01 | 범용 | 회귀자의 낡은 코트 | 매 전투 시작 시 HP +3 |
| RELIC_GENERIC_02 | 범용 | 마타이오스의 메모 | Gold 드롭 +5. Affinity ≥+3이면 +8 (P5) |

#### 일반 아이템 12개 (상점 기본 슬롯, 패시브 보유)

| ID | 임시 이름 | 효과 |
|----|----------|------|
| ITEM_01 | 붕대 뭉치 | 최대 HP +4 |
| ITEM_02 | 작은 룬석 | ATK +1 영구 |
| ITEM_03 | 날카로운 숫돌 | 공격 시 추가 1 고정 피해 |
| ITEM_04 | 낡은 방패 조각 | 방어 선택 시 받는 피해 -1 |
| ITEM_05 | 독침 | 전투 중 매 라운드 적 HP -1 |
| ITEM_06 | 행운의 동전 | Gold 드롭 +3 |
| ITEM_07 | 얼어붙은 심장 | 적 패턴 대응 보조 축 예약. 구체 효과는 패턴 결과 계약 OQ-019 후 확정 |
| ITEM_08 | 탑의 잔재 | 전투 승리 시 Gold +2 |
| ITEM_09 | 마모된 부적 | 매 전투 첫 피해 -3 |
| ITEM_10 | 피의 계약서 | 최대 HP -5, ATK +3 영구 |
| ITEM_11 | 녹슨 나침반 | 인카운터 진입 시 Affinity +1 |
| ITEM_12 | 탑의 이슬 | 전투 승리 시 HP +1 추가 회복 |

---

## 8. LLM / 메모리 아키텍처

### 8.1 모델 후보 (D-005)

| 모델 | 크기 | 한국어 | 모바일 | 우선순위 |
|------|------|-------|-------|---------|
| HyperCLOVA X SEED 0.5B | 500M | ★★★ | ★★★ | **1순위** |
| Qwen2.5 1.5B Instruct | 1.5B | ★★ | ★★ | 2순위 |
| EXAONE-3.5 2.4B | 2.4B | ★★★ | ★★ | 3순위 |

**Fallback (D-14 결정점)**: 한국어 자연도 부족 시 → Claude Haiku 또는 gpt-4o-mini 클라우드 1런 1회 호출

### 8.2 추론 엔진

- **MLC-LLM** 1순위 — iOS Metal + Android Vulkan, Unity native plugin
- llama.cpp 2순위
- ExecuTorch 3순위

### 8.3 호출 패턴

```
[회차 종료] → [Reflection call] → [SQLite 저장]
[다음 회차 첫 휴식] → [Recall: 최근 3회차 요약] → [NPC 회상 생성] → [캐싱]
```

- 1런당 LLM 호출 ≤ 3회 (회상 1 + 휴식 1 + reflection 1)
- 호출당 토큰: 입력 ≤ 500, 출력 ≤ 100
- 지연 목표: 모바일 ≤ 1.5s

### 8.4 SQLite 스키마 (D-006)

```sql
CREATE TABLE runs (
  run_id        INTEGER PRIMARY KEY,
  started_at    TEXT,
  ended_at      TEXT,
  cause_of_end  TEXT,           -- 'cleared' | 'died' | 'quit'
  floor_reached INTEGER,
  npc_state     TEXT             -- 'S0'..'S5'
);

CREATE TABLE reflections (
  run_id      INTEGER,
  summary     TEXT,
  keywords    TEXT,              -- JSON array
  emotion_pad TEXT,              -- JSON {p,a,d}
  FOREIGN KEY(run_id) REFERENCES runs(run_id)
);

CREATE TABLE player_choices (
  run_id        INTEGER,
  encounter_id  TEXT,
  choice        TEXT,
  npc_affinity  REAL             -- -1.0 ~ +1.0
);

CREATE TABLE npc_memory_fragments (
  fragment_id   INTEGER PRIMARY KEY,
  unlocked_run  INTEGER,
  text          TEXT
);

CREATE TABLE player_utterances (
  id        INTEGER PRIMARY KEY,
  run_id    INTEGER,
  context   TEXT,     -- 'rest' | 'encounter'
  utterance TEXT,     -- 플레이어 자연어 입력 원문 (per D-024)
  recalled  INTEGER,  -- 0/1, 이미 회상에 사용됐는지
  FOREIGN KEY(run_id) REFERENCES runs(run_id)
);
```

### 8.5 결정성 보장

- 모든 LLM 호출 결과 캐시 — (run_id, prompt_hash) → 출력
- 데모 시드 5회차 사전 생성 후 빌드 포함 → 심사 시연 100% 결정적
- 실제 플레이 회차는 on-device 실시간 생성

---

## 9. 일정 (D-21)

| 구간 | 일자 | 마일스톤 |
|------|------|---------|
| W1-1 | 04-27 ~ 04-30 | Unity 셋업 + repo + 코어 조작 + SQLite |
| W1-2 | 05-01 ~ 05-04 | 전투 + 능력 12 + 시너지 4 + on-device LLM 1차 통합 ★ **D-005 결정점** |
| W2-1 | 05-05 ~ 05-08 | 적 6 + 보스 2 + NPC 5단계 + reflection 파이프라인 |
| W2-2 | 05-09 ~ 05-11 | 인카운터 25 + 메모리 파편 5 + 엔딩 분기 |
| W3-1 | 05-12 ~ 05-14 | 폴리싱 + 튜토리얼 + 외주 통합 + 엔딩 컷씬 2 |
| W3-2 | 05-15 ~ 05-17 | CPI 광고 30s + Android APK + iOS 영상 데모 (D-018) + 데모 시드 5회차 |
| 마감 | **2026-05-18** | 제출 |

**Buffer 정책**: 매 구간 마지막 날 buffer. 밀리면 *기능 추가가 아니라 스코프 삭제*.

---

## 10. 기술 스택

```
Unity 6 LTS (2025.x)
├── VS Code
│   ├── C# Dev Kit
│   ├── Unity Tools for VS Code
│   ├── Codex CLI (D-015 — repo 루트 AGENTS.md 로 자동 인지)
│   └── SQLite Viewer
├── Git + Git LFS
├── Unity Hub
└── MLC-LLM Unity native plugin
```

**Codex 활용**: SO 보일러플레이트, 풀링, 시드 RNG, JSON 직렬화, UI 바인딩, SQLite 래퍼, MLC bridge
**Codex 비활용**: NPC 대사 (작가 직접), 게임 디자인, 밸런스, 윤리 텍스트
**Codex 작업 규약**: [[hwiglija-tower-AGENTS]] 참조 — §5.3 절대 금지 항목 enforce

---

## 11. Repo 구조

repo 이름: `hwigi-tower` (D-016, GitHub private)
repo 경로: `~/Downloads/AI Game/hwigi-tower/` (= Unity 프로젝트 루트)

```
hwigi-tower/
├── Assets/
│   ├── _Project/
│   │   ├── Scripts/
│   │   │   ├── Core/
│   │   │   ├── Combat/
│   │   │   ├── Abilities/
│   │   │   ├── Run/
│   │   │   ├── Encounters/
│   │   │   ├── NPC/
│   │   │   │   ├── StateMachine.cs
│   │   │   │   ├── DialogueRouter.cs
│   │   │   │   └── MemoryRepo.cs
│   │   │   ├── LLM/
│   │   │   │   ├── MLCBridge.cs
│   │   │   │   ├── PromptBuilder.cs
│   │   │   │   ├── ReflectionPipeline.cs
│   │   │   │   └── CloudFallback.cs
│   │   │   ├── Meta/
│   │   │   └── UI/
│   │   ├── Data/
│   │   │   ├── Abilities/
│   │   │   ├── Synergies/
│   │   │   ├── Enemies/
│   │   │   ├── Encounters/
│   │   │   ├── NPCStates/
│   │   │   └── MemoryFragments/
│   │   ├── Models/hcx-seed-0.5b/
│   │   ├── Prefabs/  Scenes/  Art/
│   ├── ThirdParty/{MLC-LLM, SQLite}/
│   └── Plugins/{iOS, Android}/
├── ProjectSettings/
├── Packages/
├── .gitignore
├── .gitattributes
└── README.md
```

---

## 12. 리스크 / 모니터링

| 리스크 | 영향 | 완화 |
|--------|------|------|
| on-device LLM 한국어 품질 부족 | NPC 대사 어색 | D-14 결정점 → 하이브리드 fallback |
| 모바일 빌드 크기 (모델 500MB+) | 다운로드 이탈 | Q4 양자화 + 모델 분리 다운로드 |
| iOS Metal 가속 통합 난이도 | iOS 빌드 지연 | D-018 — Android APK 우선, iOS = 영상 데모만 (Apple Dev 미가입) |
| LLM 추론 지연 >2s | 캐주얼 즉시성 깨짐 | 휴식 시점에만 호출 + "기억을 더듬는 중" UI |
| App/Play Store AI 정책 | 심사 지연 | AI 사용 고지 1회 + 설정 메뉴 안내 |
| 외주 아트 지연 | 폴리싱 부족 | 플레이스홀더 도형으로 빌드 가능 유지 |
| 솔로 + 21일 + AI NPC | 스코프 폭발 | "스코프 삭제" 룰 엄수 |

---

## 13. 슈퍼센트 / 포트폴리오 연동

- `applications/supercent/portfolio/ai-agents/` 에 deck 추가: **07 in-game AI NPC 에이전트 (Flick 출품작)**
- 02 게임 기획 검증 에이전트 + [[nemotron-personas-korea]] 로 출품 전 페르소나 시뮬레이션
- 자기소개서 강점 B (워크플로우 재정의) + 강점 C (창작자 DNA — AI 와 사람 관계 설계) 의 살아있는 증거

---

## 14. 연결된 페이지

### 14.1 hwigi-tower 위성 문서 (직속)
- [[hwiglija-tower-AGENTS]] — Codex CLI 브리프 (repo 루트로 복사됨)
- [[hwiglija-tower-tone-bible]] — 톤·문체·UI 보이스 단일 참조점
- [[hwiglija-tower-design-journal]] — 결정의 *왜*·대안·기각 archive
- [[hwiglija-tower-progress]] — 실행 로그 (Codex append-only)

### 14.2 연관 페이지
- [[flick-challenge]] — 공모전 메타 (마감·상금·심사·제출)
- [[ai-npc-vision]] / [[ai-npc-blueprint]] — 비전·청사진 (본 게임 = 첫 실전 게임화)
- [[generative-agents]] — Memory/Reflection/Planning 차용
- [[role-llm]] — NPC voice/canon
- [[persona-vectors-2025]] — 붕괴 = persona drift
- [[anthropic-emotions-2026]] / [[activation-steering]] — 단계별 LLM 조작
- [[anthropic-model-welfare]] / [[2026-04-25-npc-author-ethics]] — 안식 vs 동행 윤리 근거
- [[nemotron-personas-korea]] — NPC base persona seed
- [[초파리]] / [[hyunsoo-bot]] — 캐릭터 AI 이식 계보
- [[llm-in-a-flash]] — on-device 배포 근거
