---
created: 2026-04-27
updated: 2026-04-30
type: project
tags: [codex, agents, hwiglija-tower, gpt-5.5]
---

# AGENTS.md — Codex Brief for `hwiglija-tower`

> 본 파일은 Codex 의 컨벤션 파일.
> Unity repo (`hwiglija-tower`) 생성 시 **그대로 복사하여 repo 루트의 `AGENTS.md` 로 배치**.
> Codex 는 작업 디렉토리의 `AGENTS.md` 를 자동 로드하여 프로젝트 컨텍스트를 인지한다.
>
> 본 파일 자체는 Sub brain 의 SSOT 위성 문서 — 변경은 [[hwiglija-tower-gdd]] 의 §0 프로토콜을 따른다.

---

## 0. 메타 상속 (Sub brain CLAUDE.md §"메타 행동 원칙")

본 AGENTS.md 모든 절은 **Sub brain CLAUDE.md 의 Karpathy 4 원칙** 을 상속한다. 본 파일이 4 원칙과 모순되면 CLAUDE.md 가 우선.

1. **Think Before** — 가정 surface, 다중 해석 시 옵션 다 제시
2. **Simplicity First** — 요청 외 코드·추상화 0
3. **Surgical Changes** — 인접 코드 손대지 마, 본인이 만든 orphan 만 정리
4. **Goal-Driven** — 검증 가능 성공 기준 명시 후 루프

상세: [[karpathy-guidelines]] / CLAUDE.md §"메타 행동 원칙". 본 페이지의 §11 (Codex 도구 규약) / §5 (Codex 작업 모드) 는 4 원칙을 *프로젝트 특화* 로 구체화한 것이지 대체가 아님.

---

## 1. 프로젝트 정체성

- **프로젝트**: 회귀자는 탑을 오른다 (Flick x KRAFTON 출품작)
- **repo 이름**: `hwigi-tower` (D-016)
- **repo 경로**: `~/Downloads/AI Game/hwigi-tower/` (= Unity 프로젝트 루트, 본 AGENTS.md 가 이 디렉토리 루트에 위치)
- **장르**: 모바일 세로 로그라이크, 하이브리드 캐주얼 트랙, 미드코어 요소 (빌드 시너지)
- **차별화**: on-device AI NPC 동행, 회귀 메모리, LLM 컨텍스트 붕괴를 in-game 기믹으로 흡수, 안식/동행 엔딩 선택
- **마감**: 2026-05-18 (D-21 from 2026-04-27)
- **개발자**: 솔로 + 아트/사운드 외주

## 2. 단일 진실 소스 (SSOT) + 진행 로그

본 게임의 모든 디자인·기술·일정·콘텐츠 결정의 권위 출처는 **외부 SSOT** :

```
SSOT (디자인 결정):
  ~/Downloads/AI AGENT/Obsidian/Sub brain/Sub brain/wiki/projects/hwiglija-tower-gdd.md
  → §0.6 DESIGN PILLARS (P1-P5 + Anti) 는 모든 결정의 정합성 시금석

TONE BIBLE (텍스트·UI·NPC 보이스):
  ~/Downloads/AI AGENT/Obsidian/Sub brain/Sub brain/wiki/projects/hwiglija-tower-tone-bible.md

DESIGN JOURNAL (사고 archive — 결정의 *왜*):
  ~/Downloads/AI AGENT/Obsidian/Sub brain/Sub brain/wiki/projects/hwiglija-tower-design-journal.md

PROGRESS (실행 로그, append-only):
  ~/Downloads/AI AGENT/Obsidian/Sub brain/Sub brain/wiki/projects/hwiglija-tower-progress.md

SATELLITE (본 파일의 원본):
  ~/Downloads/AI AGENT/Obsidian/Sub brain/Sub brain/wiki/projects/hwiglija-tower-AGENTS.md
```

Codex 는 매 작업 세션마다 다음 절차를 반드시 준수:

### 2.1 작업 시작 시 (READ)
1. SSOT §0 META **+ §0.6 DESIGN PILLARS**, §1 CHANGELOG, §2 DECISIONS, §3 OPEN QUESTIONS 를 읽는다
2. 텍스트·UI 작업이면 TONE BIBLE 도 읽는다 (음역 A/B/C/D 식별)
3. PROGRESS 의 최신 항목 1-3개를 읽어 직전 작업 컨텍스트 복원
4. 본 AGENTS.md §7 일정에서 현재 구간 확인

### 2.2 작업 중 (RULES)
1. **모든 새 제안은 P1-P5 정합성 명시** — 충돌 시 거부 또는 사용자 확인
2. **잠긴 결정 (🔒 LOCKED)** 은 변경 금지 — 충돌 시 사용자에게 확인 요청
3. **미결정 (❓ OPEN)** 영역은 임의 결정 금지 — 사용자에게 질문
4. **새 결정 발생 시** SSOT 의 §0 업데이트 프로토콜 4단계 모두 수행 후 코드 작성
5. **판단의 무게가 있는 결정**은 DESIGN JOURNAL 에 사고 과정 archive (대안·기각 사유 포함)
6. **텍스트 산출물**은 TONE BIBLE §3 금지선 위배 점검

### 2.3 작업 종료 시 (WRITE)
1. **PROGRESS 에 항목 추가** — 본 AGENTS.md §9 포맷, 최신이 위로 (prepend)
2. SSOT 변경이 있었다면 SSOT 의 §1 CHANGELOG / §2 DECISIONS 에도 기록
3. 사용자에게 §9 의 4줄 요약 출력

## 3. 기술 스택 (잠금)

- **엔진**: Unity 6 LTS (C#)
- **에디터**: VS Code + C# Dev Kit + Unity Tools for VS Code
- **on-device LLM**: HyperCLOVA X SEED 0.5B (1순위) / Qwen2.5 1.5B (2순위)
- **추론 엔진**: MLC-LLM (Unity native plugin)
- **LLM fallback**: 모델 수용성 부족 시 (D-2026-05-04 결정점) 클라우드 (Claude Haiku / gpt-4o-mini) 1런 1회 호출
- **메모리**: SQLite (회차당 1 reflection)
- **VCS**: Git + Git LFS

## 4. 코드 컨벤션 (프로젝트 특화)

> 일반 코드 품질 (에러 처리, 단일 책임, 가독성 등) 은 시스템 프롬프트가 담당. 본 §4 는 **이 게임 고유** 규칙만.

### 4.1 아키텍처 원칙 (게임 특화)

- **데이터 드리븐** — 능력·적·인카운터·NPC 상태·메모리 파편 모두 ScriptableObject. 코드 수정 없이 데이터로 콘텐츠 확장
- **결정성 (P4)** — 모든 LLM 호출은 캐시 (run_id, prompt_hash) → 같은 출력. 시드 RNG 명시. `DateTime.Now`·시드 없는 `Random` 거부
- **인터페이스 우선** — `ILLMProvider`, `INPCMemoryRepo`, `IDialogueRouter` 인터페이스 → on-device ↔ 클라우드 fallback 무중단 전환

### 4.2 네이밍 (게임 자산)

- ScriptableObject 자산: `SO_<Category>_<Name>` (예: `SO_Ability_FireBlade`)
- 이벤트: `On<Subject><Verb>` (예: `OnRunCompleted`, `OnNpcStateChanged`)
- C# 표준 (PascalCase / camelCase / _camelCase) 은 .editorconfig + linter 가 enforce — AGENTS.md 에 중복 금지

### 4.3 폴더 구조 (잠금)

SSOT §11 참조. 변경 금지.

### 4.4 주석 (게임 도메인)

- 기본 무주석 (시스템 프롬프트 정책 동일)
- LLM·NPC 의도적 결정은 SSOT 인용 필수: `// per GDD D-005`, `// per Pillar P2`
- 매직 넘버는 SO 로 추출 — 코드에 박지 않음

### 4.5 Structured Outputs 통제 (필수)

- **포맷 파괴 방지**: NPC Reflection 생성 시 자유 텍스트 출력을 엄격히 금지.
- **스키마 강제**: 반드시 `reflections.emotion_pad` 테이블 스키마에 맞는 JSON만 출력하도록 prompt 를 구성하고, 출력 후 `JSON.Parse` 검증 필수.
- **지침**: "Do not provide any conversational filler. Output ONLY the raw JSON object matching the schema." 를 시스템 프롬프트에 포함.

### 4.6 Fallback & Caching (D-005)

- **분기점**: On-device LLM 이 OQ-004 합격선(자연도 < 80% 또는 지연 > 1.5s) 미달 시 즉시 클라우드 Fallback 으로 전환.
- **캐싱 전략**: 클라우드 API 호출 시 **Tone Bible 캐싱**을 통해 토큰 비용 낭비 방지. 시스템 프롬프트와 페르소나 데이터는 호출 간 고정하여 Prompt Caching 을 활성화함.

## 5. Codex 작업 모드

### 5.1 자동 수행 가능

- ScriptableObject 보일러플레이트 생성
- 풀링·시드 RNG·JSON 직렬화·UI 바인딩 헬퍼
- SQLite 래퍼 (CRUD)
- MLC-LLM Unity bridge (네이티브 함수 시그니처 매핑)
- 인터페이스 → 구현체 stub
- 단위 테스트 (PlayMode/EditMode)
- .gitignore / .gitattributes (Unity + LFS)

### 5.2 사용자 확인 필요

- 새 게임 디자인 결정 (능력 수치, 시너지 효과, 인카운터 텍스트)
- NPC 대사·서사 (작가 직접 작성)
- 윤리적 선택지 텍스트
- 잠긴 결정과 충돌하는 변경
- 외부 의존성 추가 (NuGet, Unity Package, native plugin)
- 빌드 설정 변경 (BuildTarget, Scripting Backend)

### 5.3 절대 금지

- SSOT 의 잠긴 결정 변경
- on-device LLM 우회 (D-005 fallback rule 외)
- LLM 으로 NPC 대사·서사 즉석 생성 (작가 권한)
- 시간 측정 외 비결정적 코드 (`DateTime.Now`, `Random` without seed)
- 임의의 외부 API 호출 추가
- `Library/`, `Temp/`, `Logs/`, `obj/`, `bin/` 커밋

## 6. 자주 쓰는 명령

```bash
# Unity 프로젝트 빌드 검증 (CI 외 로컬)
Unity -batchmode -projectPath . -executeMethod BuildScript.BuildAndroid

# 테스트
Unity -batchmode -projectPath . -runTests -testPlatform PlayMode

# LFS 트래킹 추가 (새 에셋 타입)
git lfs track "*.fbx"

# MLC-LLM 모델 양자화 (개발 PC)
mlc_llm convert_weight ./models/hcx-seed-0.5b --quantization q4f16_1
```

## 7. 일정 인지 (D-21 마감)

| 구간 | 현재 작업 영역 |
|------|---------------|
| W1-1 (~04-30) | 코어 조작 + SQLite + repo 셋업 |
| W1-2 (~05-04) | 전투 + 능력 + **on-device LLM 1차 통합** ★ |
| W2-1 (~05-08) | 적·보스 + NPC 5단계 + reflection |
| W2-2 (~05-11) | 인카운터 + 메모리 파편 + 엔딩 |
| W3-1 (~05-14) | 폴리싱 + 외주 통합 |
| W3-2 (~05-17) | CPI 광고 + 빌드 + 데모 시드 |
| 마감 | 2026-05-18 |

**현재 구간을 벗어나는 작업 제안 시 사용자 확인 필요.**

## 8. 외부 자료 참조

- SSOT: 위 §2 경로
- 공모전 메타: `wiki/projects/flick-challenge.md`
- AI NPC 비전·청사진: `wiki/thoughts/ai-npc-vision.md` / `ai-npc-blueprint.md`
- 페르소나 데이터셋 후보: `wiki/learnings/techniques/nemotron-personas-korea.md`

## 9. 보고 형식

### 9.1 사용자 출력 (4줄 요약)

작업 완료 시 채팅에 다음 4줄 출력:

```
[Done] <한 줄 요약>
[Files] <변경 파일 N개>
[GDD impact] <SSOT 변경 필요 여부, 있으면 D-NNN/OQ-NNN>
[Next] <다음 권장 작업 1줄>
```

### 9.2 PROGRESS 항목 (필수 기록)

**같은 세션에서** PROGRESS 파일 (§2 경로) 상단 `<!-- prepend -->` 마커 위에 항목 추가:

```markdown
### YYYY-MM-DD HH:MM — <한 줄 요약>
- **Phase**: W1-1 / W1-2 / ...
- **Done**:
  - <bullet 3-5개>
- **Files**: 변경/추가 N개 (주요 경로 2-3개)
- **GDD impact**: 없음 / D-NNN 추가 / OQ-NNN close
- **Blockers**: (없으면 생략)
- **Next**: <1-2줄>
- **Agent**: Codex
```

PROGRESS 기록 누락 시 다음 세션에서 컨텍스트 복원 불가 — **절대 생략 금지**.

## 11. Codex 도구·실행 규약 (OpenAI Prompting Guide 적용)

### 11.1 도구 선호도

- **검색**: `rg` / `rg --files` 우선. `grep`·`find` 사용 금지 (속도)
- **편집**: `apply_patch` (단일 파일). 자동 생성·대량 변경은 스크립트
- **계획**: 다단계 작업 시 `update_plan` 으로 TODO 유지. 단일 step 작업은 plan 생략
- **셸**: `shell_command` 의 command type = `"string"` (list 보다 빠름)

### 11.2 병렬 실행 (필수)

- **Think first**: tool call 전에 필요한 모든 파일·리소스를 *한 번에* 결정
- **Batch everything**: 의존성 없으면 한 메시지에 다중 tool call
- **Sequential 은 최후의 선택**: "다음 파일이 직전 결과 없이는 모를 때만"
- 응답 순서: `function_call×N → function_call_output×N`

### 11.3 Bias to Action

- 합리적 가정으로 즉시 시작. 확인 질문으로 턴 종료 금지 (진짜 막혔을 때만)
- tool call 전 긴 분석 금지. 분석은 tool 결과 *이후*
- 파일 반복 read 만 하고 진행 없으면 멈추고 사용자에게 묻기

### 11.4 Preamble (진행 업데이트) cadence

- 평소 1-2문장. 마일스톤만 길게
- 빈도: 매 1-3 실행 단계마다 1회. **하드 플로어**: 6 단계 또는 10 tool call 안에 1회
- 협업 톤 ("we"/"let's"). "✓ Reading file..." 류 loggy 업데이트 금지

### 11.5 절대 금지 (가이드 표준 + 우리 프로젝트)

- `git reset --hard`, `git push --force`, dirty worktree 의 사용자 변경 reset (사용자 명시 요청 시만)
- 시크릿 (`OPENAI_API_KEY`, certs) 커밋
- §5.3 의 게임 특화 금지 (LLM 우회, 비결정적 코드 등)

## 12. Personality

**Pragmatic** 모드 채택 — 솔로 + 21일 마감 + 작가가 워크플로우 익숙.
- 짧고 직설적. 사회적 미사여구 최소
- "Got it–", "Aha", "Good catch" 류 반복 tic 금지
- 답변 = 결과·다음 단계·미해결 질문 (이 순서)

향후 외주 협업 단계 (W3-1 폴리싱) 진입 시 Friendly 로 전환 검토.

## 13. 변경 이력

- 2026-04-27 — v0.1 초안. SSOT 연결, Codex 모드 정의, 절대 금지 항목 명시.
- 2026-04-27 — v0.2. PROGRESS 파일 연동(§2.3 / §9.2). 작업 시작·중·종료 3단계 절차로 재구성.
- 2026-04-28 — v0.3. TONE BIBLE / DESIGN JOURNAL 연동. §2.1 read 단계에 PILLARS·TONE 추가, §2.2 rules 에 P1-P5 정합성 점검 + 사고 archive 의무 명시.
- 2026-04-28 — v0.4. OpenAI Codex Prompting Guide 적용. §11 Codex 도구·실행 규약 신설 (rg/apply_patch/parallel/update_plan). §12 Personality = Pragmatic. §4 generic 항목 정리 (시스템 프롬프트 중복 회피). [[codex-cli-prompting]] 참조.
- 2026-04-30 — v0.5. JSON Schema 통제 및 Cloud Fallback 캐싱 지침 추가. MVP 제약 사항(Discard Pile) 인지 의무화.
