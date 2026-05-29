---
created: 2026-04-27
updated: 2026-04-27
type: project
tags: [progress, hwiglija-tower, flick]
project: 회귀자는 탑을 오른다
---

# hwiglija-tower — Progress Log

> 본 파일은 **append-only** 진행상황 아카이브.
> Codex CLI 가 매 작업 세션 종료 시 한 항목씩 추가한다 (AGENTS.md §9 참조).
> 디자인 결정은 본 파일이 아닌 [[hwiglija-tower-gdd]] §1 CHANGELOG / §2 DECISIONS 에 기록.
> 본 파일은 **무엇을 했는가** (실행 로그) 만 다룬다.

---

## 항목 포맷

```
### YYYY-MM-DD HH:MM — <한 줄 요약>
- **Phase**: W1-1 / W1-2 / ...
- **Done**: 무엇을 했는가 (3-5 bullet)
- **Files**: 변경/추가 파일 N개 (주요 경로)
- **GDD impact**: SSOT 변경 여부 — 있으면 D-NNN / OQ-NNN / CHANGELOG 항목
- **Blockers**: (있으면) 막힌 지점
- **Next**: 다음 권장 작업 1-2줄
- **Agent**: Codex / Claude (Sub brain) / 작가 직접
```

---

## 진행 로그

### 2026-05-29 10:31 — feedback batch A runtime 적용
- **Phase**: Post-feedback / Runtime UI Reliability
- **Done**:
  - clean worktree `/private/tmp/hwigi-game-feedback-batch-a`에서 `origin/Proto@3985c55` 기준 CodeGraph fresh preflight를 수행
  - top HUD를 `Floor / HP / Gold / 이성` 표면으로 축소하고 map header를 `갈림길 선택`으로 단순화
  - map node layer와 rest choice panel 위치를 조정하고 map/rest 화면의 Mataios large status surface를 숨김 상태로 유지
  - enemy/player/Mataios HP fill을 current/max 비율로 갱신하고 rest background fallback persistence를 추가
  - map return/exploration context가 normal lobby BGM으로 복귀하도록 fallback reset을 추가
- **Files**: 변경/추가 5개 (`PrototypeHud.cs`, `PrototypeRoomController.cs`, `PresentationLayerTests.cs`, `AudioLobbyTests.cs`, progress)
- **GDD impact**: 없음 — D-034/D-036 잠금 범위 내 runtime 구현
- **Blockers**: Unity batchmode EditMode/PlayMode 검증은 licensing reconnect loop로 완료 불가
- **Next**: Unity Editor playtest에서 feedback item 2/3/4/5/7/8/10/11/12 체크 후 Batch B 범위(item 1/6/9/low HP)를 분리 적용
- **Agent**: Codex

### 2026-05-27 00:38 — 2026-05-27 feedback rules lock 반영
- **Phase**: Game/System Track / Feedback Rules Lock
- **Done**:
  - `Docs/Feedback/2026-05-27` 피드백에서 시스템 규칙이 필요한 pre-run, map route/reveal, combat preview, enemy stat, audio/low HP 항목을 분리
  - D-034를 pre-run placeholder + sparse route/reveal/commitment로 정식 잠금하고 OQ-026을 close
  - D-036으로 resolver-owned combat preview, supported enemy stat surface, read-only combat item inspect, map/boss BGM reset, low HP P1 feedback 계약을 신규 잠금
  - OQ-019 `ITEM_07`과 OQ-025 enemy intent deck은 open 유지하고, runtime RL/ONNX/최종 story text는 범위 밖으로 명시
- **Files**: 변경/추가 6개 (`design/feedback-rules-lock-2026-05-27.md`, `design/map-flow-route-commitment-spec.md`, `design/balance.md`, SSOT/journal/progress)
- **GDD impact**: v0.15.0 MINOR — D-034 locked, D-036 locked, OQ-026 closed
- **Next**: 개발 세션은 clean worktree에서 CodeGraph preflight 후 D-034/D-036 구현 계약만 처리하고, OQ-019/OQ-025 범위는 건드리지 않는다
- **Agent**: Codex

### 2026-05-26 00:37 — ML-Agents blog visual pack 추가
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - 기존 테스트 스크린샷 위치와 AI training 전용 시각 자료 부재를 확인
  - `mlagents_blog_metric_summary.csv`에서 블로그용 reward/action/defend/tuning 차트를 생성
  - SVG 원본과 PNG screenshot 변환본을 `mlagents_blog_visuals` 폴더에 정리
  - 새 training, Unity screenshot harness, ONNX/runtime 연결 없이 CSV 기반 시각 자료만 추가
- **Files**: 변경/추가 11개 (`Docs/Portfolio/assets/mlagents_blog_visuals/*`, `hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (portfolio visual material 정리)
- **Next**: 블로그에는 `mlagents_blog_visuals` PNG를 사용하고, 게임 QA 스크린샷은 별도 폴더로 분리 유지
- **Agent**: Codex

### 2026-05-26 00:24 — ML-Agents combat portfolio draft pack 작성
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp02-06 evidence를 바탕으로 개발 블로그용 brief와 한국어 draft를 작성
  - PPO/AttackSpam/SkillSpam/ContextPolicy 핵심 metric을 blog summary CSV로 정리
  - Exp05/05b/06은 failure/near-miss evidence로 표현하고 Exp04 strict best accepted probe를 유지
  - 추가 training, 50k, ONNX 연결, 본편 runtime RL 변경 없이 문서 산출물만 추가
- **Files**: 변경/추가 4개 (`ml-agents-blog-brief.md`, `ml-agents-blog-draft-ko.md`, `mlagents_blog_metric_summary.csv`, `hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (portfolio/blog material 정리, 본편 전투 결정 미변경)
- **Next**: PM/블로그 담당자 검토 후 문구 확정. commit/push는 PM 승인 전까지 보류
- **Agent**: Codex

### 2026-05-26 00:13 — Mataios ContextPolicy combat brain 설계 lock
- **Phase**: Game/System Track / Combat Brain Design
- **Done**:
  - AI Track Exp04-06 evidence를 읽고 PPO/ONNX runtime 연결 대신 ContextPolicy deterministic handoff가 맞다는 결론을 GDD에 반영
  - D-035로 `MataiosCombatContext → MataiosCombatBrain → MataiosActionPlan` 순수 도메인 경계를 잠금
  - OQ-024 table은 fallback/payload 기준으로 유지하고, 다음 구현은 enemy threat/tempoReady/Skill context/player history 기반 ordered brain을 따르도록 정리
  - 첫 runtime은 새 전투 수치를 만들지 않고 OQ-024 payload를 재사용하며, 새 tempo/Skill 보너스 수치는 Balance OQ 전까지 금지
  - 본편 C# runtime 변경 전 CodeGraph fresh status/sync/query/context 필수 조건을 D-035와 system spec에 명시
- **Files**: 변경/추가 6개 (`design/mataios-context-policy-combat-brain-spec.md`, `design/two-actor-party-combat-lock-spec.md`, `design/combat-core-rebuild-spec.md`, SSOT/journal/progress)
- **GDD impact**: v0.14.0 MINOR — D-035 locked, OQ-024 fallback화
- **Next**: 개발 세션은 clean `origin/Proto` worktree에서 CodeGraph preflight 후 D-035 brain을 pure C#으로 구현하고, runtime RL/ONNX/새 수치 추가는 하지 않는다
- **Agent**: Codex

### 2026-05-26 00:00 — AI Track tuning stop 및 commit hold 확정
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp05/05b/06 추가 튜닝 중단을 확정하고 failure/near-miss evidence로 보존
  - 50k PPO, ONNX runtime connection, shipped runtime RL integration 금지를 유지
  - PM 승인 후 commit 범위를 docs/config/source/curated CSV/JSON evidence로 제한
  - raw TensorBoard event, checkpoint `.pt`, ONNX, macOS app build, generated result folder 제외를 재확인
- **Files**: 변경/추가 2개 (`ai-assisted-combat-design-lab.md`, `hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (AI Track evidence/commit policy 정리, 본편 전투 결정 미변경)
- **Next**: PM 승인 전 commit/push 보류. 승인 시 `ML-Agents tuning evidence freeze` 후보 범위만 stage
- **Agent**: Codex

### 2026-05-25 23:35 — Exp05-06 evidence freeze hold 정리
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp05/05b/06을 failure/near-miss evidence로 유지하도록 포트폴리오 문서에 freeze status를 명시
  - Exp04를 strict best accepted design probe로 유지한다고 기록
  - 50k PPO, ONNX runtime connection, shipped runtime RL integration 중단을 재확인
  - Exp05/05b/06 commit 후보는 PM 승인 전까지 보류한다고 명시
- **Files**: 변경/추가 2개 (`ai-assisted-combat-design-lab.md`, `hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (portfolio/training evidence 상태 정리, 본편 전투 결정 미변경)
- **Next**: PM 승인 전에는 commit/push 없이 ContextPolicy handoff 설계 지시서만 준비
- **Agent**: Codex

### 2026-05-25 23:33 — Exp06 실패 검토 및 ContextPolicy handoff 결정
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp06 `Skill opportunity observation bit` 보고를 clean AI worktree `/private/tmp/hwigi-mlagents-bootstrap` 기준으로 검토
  - `HEAD`/`origin/Proto`가 `68b143b`로 일치하고, Exp05/05b/06 산출물 34개가 uncommitted evidence인 상태를 확인
  - Exp06 PPO reward 1.798036, Skill share 0.213534, ContextPolicyGap -0.304295로 strict fail 판정을 확인
  - observation count 11→12, Exp06 분리 산출물, runtime diff empty, raw generated file 미포함 상태를 확인
  - PM 방향을 PPO tuning 중단 및 ContextPolicy deterministic handoff 준비로 정리
- **Files**: 변경/추가 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (PM review/progress 기록 한정; 본편 deterministic policy lock은 별도 GDD/시스템 디자인 세션 필요)
- **Next**: AI 세션에는 Exp05/05b/06 failure/near-miss evidence 보존안을 정리하게 하고, Game Track에는 ContextPolicy→Mataios deterministic combat brain 설계 lock 지시서를 발행
- **Agent**: Codex

### 2026-05-25 23:29 — ML-Agents Experiment 06 Skill opportunity observation bit probe 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp06 training-only observation에 `SkillOpportunityAvailable` bit를 추가하고 TrainingCombat observation size를 12로 갱신
  - `hwigi_training_combat_exp06_10k` PPO 10k run을 실행하고 10,001 step ONNX/checkpoint/TensorBoard event 생성을 확인
  - Exp06 baseline exporter로 Random/AttackSpam/SkillSpam/DefendHeavy/ContextPolicy 1,000 episodes 비교 CSV/JSON을 생성
  - PPO reward 1.798036, Skill share 0.213534, ContextPolicyGap -0.304295로 핵심 기준을 통과하지 못해 failure evidence로 문서화
  - 본편 runtime CombatController/PrototypeRunState/Mataios policy/ONNX 연결은 변경하지 않음
- **Files**: 변경/추가 30개+ (`CombatTrainingAgent.cs`, `TrainingCombatSceneBuilder.cs`, Exp06 rules/exporter/config/docs/assets 등)
- **GDD impact**: 없음 (training/tooling/docs 한정, 본편 전투 결정 미변경)
- **Next**: PM에게 AI tuning 중단 후 ContextPolicy를 Game Track으로 handoff할지 확인. 50k와 ONNX runtime 연결은 보류
- **Agent**: Codex

### 2026-05-25 23:20 — Exp06 Skill opportunity observation bit 지시 발행
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - 다음 AI Training stage를 Exp06 `Skill opportunity observation bit` 10k probe로 지정
  - Exp04는 strict best accepted probe, Exp05/05b는 uncommitted over-correction/near-miss evidence로 구분
  - Exp06 성공/실패별 commit 판단 기준을 정리하고, 50k/ONNX/runtime RL 금지를 재확인
  - CodeGraph 규칙을 반영: 본편 C# runtime 변경 전에는 current `origin/Proto` clean worktree에서 fresh status/sync/query/context가 필요하며, 이번 AI training/docs/config 작업에는 선택 사항
  - `hwiglija-tower-codegraph-brief.md`와 `Docs/.bkit-memory.json`은 현재 main/AI clean worktree의 `rg --files` 기준 미발견으로 확인
- **Files**: 변경/추가 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (PM directive/progress 기록 한정)
- **Next**: AI 세션에 Exp06 지시서를 전달하고, 결과가 애매하면 ContextPolicy deterministic handoff로 전환 판단
- **Agent**: Codex

### 2026-05-25 23:11 — Exp05b PM review 및 다음 AI Training 전략 정리
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - accepted baseline `68b143b`와 clean AI worktree `/private/tmp/hwigi-mlagents-bootstrap` 기준을 확인
  - Exp05b report/CSV/code boundary를 검토해 PPO reward 회복, zero Skill waste, Skill share strict fail을 재확인
  - 본편 runtime RL 미도입, `CombatController`/`PrototypeRunState`/LLM/UI runtime diff 없음, raw output 제외 상태를 확인
  - 다음 전략은 50k/ONNX 연결이 아니라 Skill opportunity observation bit 10k probe 우선으로 정리
- **Files**: 변경/추가 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (PM review/progress 기록 한정)
- **Next**: Exp05/Exp05b를 over-correction/near-miss evidence로 커밋할지 승인 후, observation-bit Exp06 지시서를 발행
- **Agent**: Codex

### 2026-05-25 22:55 — ML-Agents Experiment 05b Skill gate relaxation 10k 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp05b training-only Skill opportunity gate를 HP threshold 12에서 10으로 완화
  - `hwigi_training_combat_exp05b_10k` PPO 10k run을 실행하고 10,003 step ONNX/checkpoint/TensorBoard event 생성을 확인
  - Exp05b baseline exporter로 Random/AttackSpam/SkillSpam/DefendHeavy/ContextPolicy 1,000 episodes 비교 CSV/JSON을 생성
  - PPO reward는 1.862258, SpamPolicyGap은 +0.329483으로 개선됐지만 Skill share가 0.230376으로 25% 목표에 못 미쳐 strict pass는 보류
  - 본편 runtime CombatController/PrototypeRunState/Mataios policy/ONNX 연결은 변경하지 않음
- **Files**: 변경/추가 20개+ (`CombatTrainingExp05bRules.cs`, Exp05b exporter/config/docs/assets 등)
- **GDD impact**: 없음 (training/tooling/docs 한정, 본편 전투 결정 미변경)
- **Next**: Exp05b는 near-miss evidence로 유지. 다음은 Skill opportunity observation bit 또는 design 승인 후 threshold 10 미만 10k probe
- **Agent**: Codex

### 2026-05-25 22:39 — ML-Agents Experiment 05 Skill opportunity/waste tuning 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp05 training-only Skill opportunity gate를 추가해 Skill valid 조건을 cooldown ready + enemy HP 12 이상으로 제한
  - `hwigi_training_combat_exp05_10k` PPO 10k run을 실행하고 10,001 step ONNX/checkpoint/TensorBoard event 생성을 확인
  - Exp05 baseline exporter로 Random/AttackSpam/SkillSpam/DefendHeavy/ContextPolicy 1,000 episodes 비교 CSV/JSON을 생성
  - SkillWastedPerEpisode는 0으로 감소했지만 Skill share가 0.205214로 목표 25~40% 아래로 내려가 over-correction으로 문서화
  - 본편 runtime CombatController/PrototypeRunState/Mataios policy/ONNX 연결은 변경하지 않음
- **Files**: 변경/추가 12개 (`CombatTrainingAgent.cs`, `CombatTrainingExp05Rules.cs`, Exp05 exporter/config/docs/assets 등)
- **GDD impact**: 없음 (training/tooling/docs 한정, 본편 전투 결정 미변경)
- **Next**: SkillOpportunityHpThreshold를 10~11로 완화하거나 Skill opportunity observation bit를 추가한 10k 재실험. 50k는 보류
- **Agent**: Codex

### 2026-05-25 22:11 — Exp02-04 ML-Agents evidence freeze commit/push
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp02~04 training source/config/exporter/docs/assets 32개를 `Document ML-Agents combat design experiments`로 commit
  - raw TensorBoard event, checkpoint `.pt`, ONNX, macOS TrainingCombat.app, result folder, Library cache는 commit 제외 유지
  - `git diff --check`, forbidden added-line search, runtime diff empty, generated raw not staged를 확인
  - commit `68b143b416807a36cc06edd484d2f7ca8a785b5b`를 `origin/Proto`에 push
- **Files**: commit 32개 (`TrainingCombat*`, `CombatTraining*`, `Config/MLAgents/training_combat_exp0*_10k.yaml`, `Docs/Portfolio/*`)
- **GDD impact**: 없음 (training/tooling/docs baseline, 본편 runtime RL 미도입)
- **Next**: Exp05 Skill opportunity/waste tuning은 별도 commit으로 진행
- **Agent**: Codex

### 2026-05-25 22:05 — Exp04 evidence freeze 및 ContextPolicy handoff 문서화
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - Exp04 결과를 AI-Assisted Combat Design Lab 문서로 freeze하고 Exp02/03/04 설계 루프를 정리
  - 31개 변경 파일을 Exp04 핵심, Exp02/03 보강, curated generated summary, Unity/package/cache drift로 분류
  - ContextPolicy가 PPO보다 높은 이유와 본편 deterministic policy 후보 handoff 기준을 문서화
  - raw TensorBoard/checkpoint/ONNX/result folder는 commit 제외 대상으로 명시
  - 본편 runtime CombatController/PrototypeRunState/Mataios policy/ONNX 연결은 변경하지 않음
- **Files**: 추가 1개 (`Docs/Portfolio/ai-assisted-combat-design-lab.md`) + 기존 Exp02-04 training/docs/assets freeze
- **GDD impact**: 없음 (포트폴리오 evidence/handoff 문서화, 본편 전투 결정 미변경)
- **Next**: Skill opportunity/waste 기준 소폭 튜닝 후 10k 재실험. 50k는 보류
- **Agent**: Codex

### 2026-05-25 21:56 — ML-Agents Experiment 04 combat rule redesign probe 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - training-only Exp04 규칙을 추가해 2턴 Skill cooldown, low-HP Skill waste penalty, high-threat enemy turn, Defend tempo attack payoff를 구현
  - `training_combat_exp04_10k.yaml`로 PPO 10k run을 실행하고 10,002 step ONNX/checkpoint/TensorBoard event 생성을 확인
  - Random/AttackSpam/SkillSpam/DefendHeavy/ContextPolicy baseline을 같은 Exp04 schema로 1,000 episodes씩 재평가
  - PPO reward가 1.855968로 spam policy max 1.536994를 넘고, ContextPolicy가 2.101917로 최고 성능임을 comparison CSV와 문서에 기록
  - 본편 runtime CombatController/PrototypeRunState/Mataios policy/ONNX 연결은 변경하지 않음
- **Files**: 변경/추가 15개+ (`CombatTrainingAgent.cs`, `CombatTrainingExp04Rules.cs`, Exp04 exporter/config/docs/assets 등)
- **GDD impact**: 없음 (training/tooling/docs 한정, 본편 전투 결정 임의 변경 없음)
- **Next**: ContextPolicy를 deterministic combat policy 후보로 별도 정리하고, Skill opportunity signal/waste 기준을 작게 조정한 Exp05 또는 문서화 마감 판단
- **Agent**: Codex

### 2026-05-25 21:36 — ML-Agents Experiment 03 reward/context gate 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - training-only `CombatTrainingRules`를 추가해 1턴 Skill cooldown, Skill cost, Defend prevented-damage metric/reward를 공유 규칙으로 분리
  - `training_combat_exp03_10k.yaml`로 PPO 10k run을 실행하고 10,003 step ONNX/checkpoint/TensorBoard event 생성을 확인
  - Exp03 baseline exporter로 Random/AttackSpam/SkillSpam/DefendHeavy/ContextPolicy 1,000 episodes 비교 CSV/JSON을 생성
  - Skill share가 0.719929→0.309283으로 감소하고 DamagePreventedPerStep이 0→0.245509로 surfaced됨을 문서화
  - SkillSpam/AttackSpam이 여전히 PPO reward를 상회하므로 50k보다 reward/context 재설계가 우선이라는 결론을 기록
- **Files**: 변경/추가 10개+ (`CombatTrainingAgent.cs`, `CombatTrainingRules.cs`, Exp03 exporter/config/docs/assets 등)
- **GDD impact**: 없음 (training/tooling/docs 한정, 본편 runtime RL/CombatController 미변경)
- **Next**: Skill에 더 강한 context gate를 주고 Defend에 offensive tempo payoff를 부여한 Exp04 설계
- **Agent**: Codex

### 2026-05-25 21:19 — ML-Agents Experiment 02 baseline comparison 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - training-only Editor exporter로 Random/AttackSpam/SkillSpam/DefendHeavy baseline을 1,000 episodes씩 deterministic 평가
  - baseline result CSV/JSON 및 PPO10k 포함 comparison CSV를 `Docs/Portfolio/assets`에 생성
  - PPO10k가 Random/AttackSpam보다 reward/episode length는 좋지만 SkillSpam보다 명확히 낫지 않다는 결론을 문서화
  - DefendHeavy 실패와 `DamagePreventedPerStep=0`을 근거로 Defend 보상보다 먼저 방어 효과 metric surfacing이 필요하다고 정리
  - 본편 runtime combat/RL/Mataios 경로는 변경하지 않음
- **Files**: 변경/추가 6개 (`TrainingCombatBaselineExporter.cs`, baseline CSV/JSON, comparison CSV, `ml-agents-combat-exp02.md` 등)
- **GDD impact**: 없음 (training/tooling/docs 한정, 본편 runtime RL 미도입)
- **Next**: Skill cost/context gate와 Defend prevented-damage metric을 training wrapper에서 보정한 뒤 같은 baseline 비교를 재실행
- **Agent**: Codex

### 2026-05-25 21:13 — ML-Agents Experiment 02 포트폴리오 증거 기록 보강
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - 512-step smoke를 학습 품질이 아닌 Unity executable/trainer 연결 및 ONNX export milestone으로 명확히 분리 기록
  - 10k PPO 결과의 reward 증가, episode length 감소, entropy/loss, action share 변화를 포트폴리오 evidence로 정리
  - PPO가 attack-only가 아니라 Skill-heavy policy로 이동했고 Defend가 collapse했다는 reward-design issue를 명시
  - TensorBoard scalar 요약 export CSV와 artifact manifest, training command, trainer yaml 핵심값, observation/action/reward schema를 문서에 추가
- **Files**: 변경/추가 2개 (`ml-agents-combat-exp02.md`, `mlagents_exp02_tensorboard_scalars.csv`)
- **GDD impact**: 없음 (포트폴리오 문서 보강, 본편 runtime RL 미도입)
- **Next**: Random/AttackSpam baseline을 같은 scalar schema로 비교하고 Skill/Defend reward shaping 실험을 설계
- **Agent**: Codex

### 2026-05-25 20:53 — ML-Agents Combat Experiment 02 10k PPO run 완료
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - clean worktree `/private/tmp/hwigi-mlagents-bootstrap`에서 `origin/Proto@271f0c65` 기준 exp02를 진행
  - training-only `CombatTrainingAgent`에 TensorBoard용 action share, win/loss/timeout, episode length, damage metric을 추가
  - `training_combat_exp02_10k.yaml`로 10,000 step PPO run을 실행하고 10,001 step ONNX/checkpoint export를 확인
  - event scalar에서 reward, episode length, entropy/loss, action distribution을 확인하고 portfolio 문서에 결과를 기록
  - 본편 runtime combat/Mataios/enemy intent/RL 연결 경로는 변경하지 않음
- **Files**: 변경/추가 3개 (`CombatTrainingAgent.cs`, `training_combat_exp02_10k.yaml`, `ml-agents-combat-exp02.md`)
- **GDD impact**: 없음 (포트폴리오용 isolated training experiment, 본편 runtime RL 미도입)
- **Next**: Random/AttackSpam baseline을 같은 metric schema로 별도 run/export하고 reward shaping 1차 조정 여부 결정
- **Agent**: Codex

### 2026-05-25 20:05 — ML-Agents 전투 학습 bootstrap 구현
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - clean worktree `/private/tmp/hwigi-mlagents-bootstrap`에서 `origin/Proto@3bc598b` 기준 ML-Agents training track 구현
  - `com.unity.ml-agents` release_20 package를 추가하고 본편 runtime과 분리된 `HwigiTower.Training` asmdef를 구성
  - `TrainingCombat` scene, `CombatTrainingAgent`, macOS training build helper, PPO trainer yaml을 추가
  - 지정 venv(`mlagents==0.30.0`)로 `mlagents-learn` smoke를 실행해 Unity 연결, 512 step 학습, ONNX export, TensorBoard event file 생성을 확인
  - 본편 combat/Mataios/enemy intent/runtime RL 경로는 변경하지 않음
- **Files**: 변경/추가 10개 (`Packages/manifest.json`, `CombatTrainingAgent.cs`, `TrainingCombat.unity`, `training_combat_ppo.yaml`, portfolio doc 등)
- **GDD impact**: 없음 (포트폴리오용 isolated training environment, 본편 runtime RL 미도입)
- **Next**: TensorBoard 결과를 확인하며 reward shaping/observation 개선안을 별도 AI Training 세션에서 조정
- **Agent**: Codex

### 2026-05-25 18:34 — Map/Floor Transition P0 복구
- **Phase**: Core Run UI / Map Flow
- **Done**:
  - clean worktree `/private/tmp/hwigi-map-floor-p0`에서 `origin/Proto@76403ac` 기준 지도/층 전환 P0 복구
  - 새 층 진입 시 이전 result/combat/portrait/CTA를 clear하고 새 floor map을 즉시 표시하도록 정리
  - node tap 즉시 encounter commit, active encounter 중 utility map 재선택 차단, boss return 시 map 복귀 흐름을 보정
  - floor map을 3 sparse lane, out-degree max 2, same-layer/far-lane/all-to-all edge 금지 구조로 갱신하고 first selectable row Rest 금지 반영
  - D-034 map invariant 기준으로 EditMode/PlayMode helper 기대값을 갱신하고 screenshot QA는 PM 지시에 따라 standard test에서 Explicit 제외
- **Files**: 변경 5개 (`PrototypeFloorMap.cs`, `PrototypeHud.cs`, `RuntimeShellTests.cs`, `PrototypeRoomSmokeTests.cs` 등)
- **GDD impact**: 없음 — D-034 후보/PM 피드백 구현 반영, 새 SSOT 결정 추가 없음
- **Next**: PM 수동 플레이로 map readability/commitment 확인 후 필요 시 Top HUD 또는 Combat Batch 후속으로 전환
- **Agent**: Codex

### 2026-05-25 18:39 — ML-Agents 전투 학습 재편 명세 및 로컬 trainer 준비
- **Phase**: Post-deadline / AI Portfolio Tooling
- **Done**:
  - ML-Agents 전투 학습 방향을 포트폴리오용 `TrainingCombat` 분리 씬 + ONNX 산출 우선으로 정리
  - 구현 세션 확인 질문(worktree, 독립 Agent vs 실전투 wrapper, package source, scene creation, artifact target)을 명세서에 고정
  - Python 3.10.12를 pyenv로 설치하고 ML-Agents용 별도 venv를 준비
  - `mlagents==1.1.0`은 macOS 26 arm64 `grpcio<=1.48.2` 빌드 실패로 보류하고, `mlagents==0.30.0` trainer를 설치/검증
  - `mlagents-learn --help`, `tensorboard --help`, 핵심 package freeze 확인
- **Files**: 변경 1개 (`Docs/AI/ml-agents-combat-training-implementation-spec.md`) + 외부 venv/tooling
- **GDD impact**: 없음 (본편 runtime RL 미도입, 포트폴리오/학습 tooling 범위)
- **Blockers**: Unity package 추가와 `TrainingCombat` 구현은 구현 기준 worktree를 진행 세션에서 확정한 뒤 적용 필요
- **Next**: clean `origin/Proto` 기준 구현 세션에서 ML-Agents package/source와 독립 training scene scope를 확정하고 Milestone 1 착수
- **Agent**: Codex

### 2026-05-25 10:17 — Map Flow / Route Commitment 설계 검토
- **Phase**: Core Run Design / Map Flow
- **Done**:
  - 현행 `PrototypeFloorMap`/`RunState`/HUD map flow를 audit해 dense route graph, 느슨한 commitment, early Rest, floor transition result 잔류 리스크를 분리
  - 5컬럼 유지 + 3 logical lane sparse route + adjacent cross-lane branch 제한 + irreversible node commitment를 D-034 후보로 정리
  - Rest는 Layer 1 금지, Layer 2/3 0-1개 후보로 미루고 boss 직전 Shop 구조와 충돌하지 않게 placement rule 작성
  - 다음 층 진입 시 이전 result UI clear 후 새 floor map generated/visible 보장 규칙과 QA checklist 작성
  - OQ-026을 open으로 등록해 PM 확정 전 구현 금지 상태를 명시
- **Files**: 변경/추가 5개 (`design/map-flow-route-commitment-spec.md`, `design/balance.md`, `hwiglija-tower-gdd.md` 등)
- **GDD impact**: v0.13.1 PATCH — D-034 후보 / OQ-026 open
- **Next**: PM이 Floor 1 Rest 정책, Rest frequency, single-edge explicit tap 여부를 확정하면 D-034 lock 후 Map Flow 개발 brief로 전환
- **Agent**: Codex

### 2026-05-25 04:20 — Two-Actor Combat Batch 1 구현
- **Phase**: Core Combat Rebuild
- **Done**:
  - clean worktree `/private/tmp/hwigi-two-actor-baseline-2fbaed5`에서 최신 `origin/Proto` `2fbaed5` 기준 D-032 2인 전투 baseline 구현
  - Mataios HP/down/targetable/action power 상태와 deterministic support policy를 `PrototypeRunState` 전투 흐름에 연결
  - protect/finish/counter/pressure/support 행동, down collapse flag, 붕괴도 +5 1회 penalty, combat/rest recovery를 검증
  - 기존 SWORD_03 과부하, FRENZY, ARTS_03, GUARD_01 build surface 회귀 테스트 유지
  - GUI EditMode 140/140, GUI PlayMode 20/20 및 `git diff --check` 통과
- **Files**: 변경/추가 7개 (`CombatController.cs`, `PrototypeRunState.cs`, `PrototypeHud.cs`, `RuntimeShellTests.cs` 등)
- **GDD impact**: 없음 — D-032/OQ-022 temporary baseline 구현 반영, D-033/OQ-025/ITEM_07/OQ-019 범위는 미구현 유지
- **Next**: PM 수동 전투 QA 후 Batch 2 enemy intent/counterplay 설계(OQ-025) 확정 여부 판단
- **Agent**: Codex

### 2026-05-24 23:59 — Two-Actor Combat 문서 기준선 publish
- **Phase**: Core Combat Rebuild
- **Done**:
  - latest `origin/Proto` 기준 clean worktree `/private/tmp/hwigi-docs-publish-20260524`에서 docs-only publish 수행
  - `design/two-actor-party-combat-lock-spec.md`, `design/combat-core-rebuild-spec.md`를 Proto에 반영
  - repo-local GDD / design journal / progress 문서에서 D-032, D-033, OQ-021~OQ-025 visibility 확인
  - `git diff --check HEAD^ HEAD` 통과 후 `Proto`에 push 완료
- **Files**: 변경/추가 5개 (`design/*combat*.md`, `Docs/Project/hwiglija-tower-gdd.md`, `Docs/Project/hwiglija-tower-design-journal.md`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음 — 기존 SSOT 결정을 repo 문서 기준선으로 publish
- **Next**: clean worktree/branch에서 Two-Actor Combat Batch 1 구현 착수
- **Agent**: Codex

### 2026-05-24 23:18 — Two-Actor Combat Batch 1 착수 차단
- **Phase**: Core Combat Rebuild
- **Done**:
  - 최신 `origin/Proto` 기준 clean worktree `/private/tmp/hwigi-two-actor-baseline`를 생성하고 baseline `ce83e10` 확인
  - CodeGraph 0.9.3 preflight를 실행해 127 files / 9,333 nodes / 23,187 edges, index up-to-date 상태 확인
  - `PrototypeRunState`, `CombatController`, `CombatRoundResult`, `PrototypeRunSnapshot`, `ResolveCombatRoundInteractive`, `ResolveRestInteraction` 심볼을 query/context로 확인
  - 필수 설계 문서 `design/two-actor-party-combat-lock-spec.md`, `design/combat-core-rebuild-spec.md`가 clean 최신 worktree에 없어 구현을 시작하지 않음
- **Files**: repo 변경 없음 (`.codegraph/` untracked only, commit 제외)
- **GDD impact**: 없음 — D-032/D-033 문서 기준 확인 전 구현 보류
- **Blockers**: 필수 design brief 2개가 최신 `origin/Proto` clean worktree에 없음. 메인 dirty worktree에는 존재하지만 stage/push 여부 미확인이라 기준 문서로 사용할 수 없음
- **Next**: 설계 문서를 `origin/Proto`에 반영한 뒤 Batch 1 구현 재개
- **Agent**: Codex

### 2026-05-24 22:44 — Combat Core Rebuild Spec 작성
- **Phase**: Core Combat Design / Combat Core Rebuild
- **Done**:
  - Attack spam damage race를 끊기 위한 enemy intent + action counterplay 상위 전투 구조를 D-033으로 잠금
  - normal/heavy/guard/charge/weak/special intent와 Attack/Defend/Skill/Mataios 대응 관계를 설계
  - SFX/VFX trigger map, 긴장감 설계, 1차 balance target, 개발 Batch 1~4 분해를 repo spec으로 작성
  - enemy별 exact intent deck/payload 숫자는 OQ-025로 분리하고 OQ-019/광폭 defer 금지선을 유지
- **Files**: 변경/추가 4개 (`hwiglija-tower-gdd.md`, `hwiglija-tower-design-journal.md`, `hwiglija-tower-progress.md`, `design/combat-core-rebuild-spec.md`)
- **GDD impact**: GDD v0.13.0 / D-033 추가 / OQ-025 추가 / OQ-019 open 유지
- **Blockers**: Batch 2 전 OQ-025 enemy별 intent deck 확정 필요. `ITEM_07` 구현은 OQ-019 전까지 금지
- **Next**: 개발 세션은 Batch 1 Two-Actor baseline부터 착수하고, Batch 2 전에 OQ-025를 닫을 것
- **Agent**: Codex

### 2026-05-24 22:35 — Two-Actor Combat OQ 보정 반영
- **Phase**: Core Combat Design / Two-Actor Party Baseline
- **Done**:
  - OQ-021/OQ-024를 1차 구현값으로 close하고 Mataios HP/action power/recovery/policy rule table을 문서화
  - OQ-022를 final lock이 아닌 temporary implementation contract로 partial-close 처리
  - OQ-023을 future combat expansion stage로 defer하고 1차 구현은 기존 `광폭/FRENZY` Player chain 유지로 제한
  - 개발 brief와 QA checklist를 hard-coded collapse/modal 금지, 광폭 리팩터링 금지 기준으로 보정
- **Files**: 변경 3개 (`hwiglija-tower-gdd.md`, `hwiglija-tower-design-journal.md`, `design/two-actor-party-combat-lock-spec.md`)
- **GDD impact**: GDD v0.12.1 / OQ-021 close / OQ-022 partial-close temporary / OQ-023 defer / OQ-024 close
- **Blockers**: OQ-019 open 유지. Down/collapse temporary 값은 플레이 후 교체 가능 구조로 구현해야 함
- **Next**: 개발 세션은 2인 전투 actor/policy/down event를 구현하되 광폭 리팩터링과 collapse hard-code를 금지
- **Agent**: Codex

### 2026-05-24 22:26 — Two-Actor Party Combat Lock Spec 작성
- **Phase**: Core Combat Design / Two-Actor Party Baseline
- **Done**:
  - `Player + Mataios vs Enemy` 2인 파티 전투 기준선을 D-032로 잠금
  - 마타이오스 deterministic automatic policy, down/collapse, post-combat/Rest recovery 경계를 설계
  - `SWORD_03`, `광폭`, `ARTS_03`, `GUARD_01`, controlled item pool과의 상호작용을 player-owned build surface 기준으로 정리
  - 개발 1차 구현 brief, QA checklist, AI QA metric contract를 repo design spec으로 작성
- **Files**: 변경/추가 3개 (`hwiglija-tower-gdd.md`, `hwiglija-tower-design-journal.md`, `design/two-actor-party-combat-lock-spec.md`)
- **GDD impact**: D-032 추가 / OQ-021~OQ-024 추가 / OQ-019 open 유지
- **Blockers**: OQ-021~024 PM 결정 전 exact 수치·down 표시·FRENZY chain scope·policy constants 구현 금지
- **Next**: PM이 OQ-021~024를 닫은 뒤 Two-Actor Combat 1차 개발 세션으로 actor state/policy/down event 구현 brief 전달
- **Agent**: Codex

### 2026-05-24 20:41 — 최신 Proto 전체 CodeGraph 인덱스 생성
- **Phase**: Post-deadline / Tooling
- **Done**:
  - main worktree dirty/diverged 상태를 피해 `/private/tmp/hwigi-codegraph-current-ce83e10` clean worktree를 생성
  - 최신 `origin/Proto@ce83e102f94a1cae72885b4aaa47b65ad5f1b1f7` 기준으로 `codegraph init -i` 완료
  - `codegraph status .` 기준 127 files / 9,333 nodes / 23,187 edges / index up-to-date 확인
  - `PrototypeRunState`, `CombatController`, `FRENZY`, `Sword` query smoke로 전투/빌드 표면 분석에 쓸 수 있음을 확인
  - `codegraph affected`는 현재 repo에서 test impact를 잘 잡지 못해 보조 지표로만 쓰고, `query/context` + `rg` 병행을 표준으로 정리
- **Files**: repo 변경 1개 (`Docs/.bkit-memory.json`) + 외부 `hwiglija-tower-codegraph-brief.md` / PROGRESS 기록
- **GDD impact**: 없음 (도구/운영 기준선 갱신)
- **Next**: 핵심 C# runtime 작업은 CodeGraph preflight를 표준으로 사용하되, docs/SO 단순 변경은 생략 가능하게 운영
- **Agent**: Codex

### 2026-05-24 20:45 — Progress/Memory/CodeGraph 토큰 효율 운영 규칙 갱신
- **Phase**: Post-deadline / Tooling
- **Done**:
  - CodeGraph brief를 v0.9.3 실제 명령 체계와 MCP/CLI fallback 기준에 맞춰 갱신
  - CodeGraph 사용을 매 작업 강제가 아니라 핵심 C# runtime 변경 trigger 기반으로 제한
  - Progress는 최신 3개, Memory는 주간 최신 5개 중심으로 읽고 Memory 기록은 세션당 1-3개 재사용 교훈만 남기는 토큰 절약 규칙을 문서화
  - screenshot harness는 기본 검증에서 제외하고 UI layout/배포 smoke/사용자 요청 때만 수행하도록 운영 기준을 명시
- **Files**: 변경 2개 (`hwiglija-tower-codegraph-brief.md`, `Docs/.bkit-memory.json`)
- **GDD impact**: 없음 (운영/도구 규칙 갱신)
- **Next**: 다음 핵심 C# 작업 전 current `origin/Proto` clean 기준으로 CodeGraph status/sync 확인 후 필요한 symbol만 조회
- **Agent**: Codex

### 2026-05-24 20:05 — CodeGraph 분석 흐름 bootstrap
- **Phase**: Post-deadline / Tooling
- **Done**:
  - `codegraph --version` = 0.9.3 확인
  - 원본 dirty worktree를 건드리지 않기 위해 `/private/tmp/hwigi-codegraph-bootstrap-20260524` clean clone에서 `codegraph init -i` 실행
  - `codegraph status .` 기준 119 files / 9,036 nodes / 22,356 edges / index up-to-date 확인
  - `.gitignore`가 Unity generated folders 및 Addressables/VisualScripting generated content를 제외하는지 `git check-ignore -v`로 검증
  - Combat 핵심 경로(`PrototypeRunState`, `CombatController`, `CombatAbilityModifiers`, `EncounterRuntimeCatalogBuilder`)를 CodeGraph context/query와 직접 파일 read로 분석
- **Files**: 변경 2개 (`Docs/Project/hwiglija-tower-progress.md`, `Docs/.bkit-memory.json`) + 외부 PROGRESS 기록
- **GDD impact**: 없음 (분석/운영 규칙 기록, 게임 결정 변경 없음; OQ-019 open 유지)
- **Next**: 후속 CodeGraph 사용 시 clean worktree/clone 기준으로 `query`/`context`를 먼저 돌리고, `.codegraph/`는 commit 후보에서 제외
- **Agent**: Codex

### 2026-05-22 23:04 — AI QA Seed Replay skeleton 커밋 및 Proto 푸시
- **Phase**: AI QA / Balance Lab
- **Done**:
  - clean worktree에서 의도 파일 15개만 stage하고 generated output은 ignore 상태로 유지
  - `Add AI QA seed replay skeleton` 커밋을 생성
  - `origin/Proto`에 push하고 local HEAD와 origin/Proto HEAD가 동일 commit을 가리키는지 확인
  - commit diff whitespace check와 본편 runtime/package 변경 없음 상태를 재확인
- **Files**: 추가 15개 (`Tools/RoguelikeSim`, `Tools/RoguelikeSimPython`, `Docs/Portfolio`)
- **GDD impact**: 없음 (AI QA 도구/포트폴리오 트랙, 본편 결정 아님)
- **Next**: 후속 AI QA 작업은 Unity SO exporter 또는 seed range 확장 실험으로 진행
- **Agent**: Codex

### 2026-05-22 22:54 — AI QA skeleton 기준선을 SWORD_03 과부하 commit에 재정렬
- **Phase**: AI QA / Balance Lab
- **Done**:
  - clean worktree `/private/tmp/hwigi-first-build-surface`에서 base commit `ed30e9f` 기준으로 AI QA 파일만 적용
  - Seed Replay 문서 기준선을 First Build Surface + `SWORD_03` 과부하 반영 완료 commit으로 갱신
  - 생성 CSV/summary는 검증 산출물로만 두고 commit 후보에서 제외되도록 ignore 처리
  - `dotnet run`, Python summary, AI QA 경로 `git diff --check`, 본편 runtime/package 변경 없음 검증을 완료
- **Files**: 추가 15개 (`Tools/RoguelikeSim`, `Tools/RoguelikeSimPython`, `Docs/Portfolio`)
- **GDD impact**: 없음 (AI QA 도구 기준선 정리, 본편 결정 아님)
- **Next**: 필요 시 이 clean worktree 변경만 별도 commit/push하고, 후속으로 Unity SO exporter를 설계
- **Agent**: Codex

### 2026-05-22 22:26 — AI QA Seed Replay skeleton 추가
- **Phase**: AI QA / Balance Lab
- **Done**:
  - First Build Surface 기준 commit `2bc2cbc`를 문서에 명시하고 본편 runtime과 분리된 AI QA 트랙 경계를 정리
  - `Tools/RoguelikeSim` 독립 C# console skeleton을 추가해 seed replay CSV를 생성
  - `RandomPolicy`, `GreedyPolicy`, `SurvivalPolicy`와 필수 CSV metric schema를 구현
  - `Tools/RoguelikeSimPython` 표준 라이브러리 기반 분석/시각화 skeleton을 추가하고 샘플 replay 90 rows를 검증
  - 포트폴리오 문서 2종에 실험 목적, 가드레일, P1-P5 정합성, 해석 규칙을 기록
- **Files**: 추가 19개 (`Tools/RoguelikeSim`, `Tools/RoguelikeSimPython`, `Docs/Portfolio`)
- **GDD impact**: 없음 (AI QA 도구/포트폴리오 트랙, 본편 결정 아님)
- **Next**: Unity Editor exporter로 First Build Surface SO 데이터를 neutral JSON으로 내보내 hand-authored fixture를 대체
- **Agent**: Codex

### 2026-05-22 19:25 — 본편 개발/AI QA 투트랙 병행 운영안 정리
- **Phase**: Portfolio / Balance Tooling Strategy
- **Done**:
  - 현재 First Build Surface 구현/검증 세션과 병행 가능한 방식으로 Game Track과 AI QA Track의 책임 경계를 분리
  - Game Track은 플레이어 경험, 빌드 표면, UI/QA/데모 완성도를 유지하고 AI QA Track은 결정적 재현/시뮬레이션/리포트/RL baseline을 담당하는 구조로 제안
  - AI QA 산출물은 본편 런타임에 직접 섞지 않고 `Tools/RoguelikeSim`, `Docs/Portfolio`, Python analysis로 분리하는 방향을 정리
  - 주간 병행 cadence와 Track 간 handoff 규칙을 제안해 게임 완성도 개선 과정이 포트폴리오 증거로 남도록 설계
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (운영/포트폴리오 전략)
- **Next**: Game Track은 First Build Surface 검증, AI QA Track은 seed replay simulator skeleton 설계부터 착수
- **Agent**: Codex

### 2026-05-22 19:17 — 게임 개발과 AI QA 트랙 분리 포트폴리오 전략 확정
- **Phase**: Portfolio / Balance Tooling Strategy
- **Done**:
  - 게임 본편 개발과 별개로, 본편 완성도를 높이기 위한 AI QA/시뮬레이션/RL 실험 트랙을 병렬 산출물로 잡는 방향을 정리
  - 포트폴리오 서사는 "로그라이크 게임을 만들며 수동 QA 한계를 발견했고, 결정적 재현/Monte Carlo/RL 실험 환경으로 밸런스 개선 루프를 구축했다"로 설정
  - 구현 산출물은 Unity 본편, RoguelikeSim, Python analysis/RL baseline, 실험 리포트 4개 축으로 분리 제안
  - 완성도 개선 과정을 before/after 지표와 디자인 수정 근거로 남기는 방식이 AI_MODEL 직무 어필에 가장 적합하다고 판단
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (포트폴리오/QA 트랙 전략)
- **Next**: 첫 포트폴리오 리포트 목차와 `RoguelikeSim` 구현 체크리스트 작성
- **Agent**: Codex

### 2026-05-22 19:06 — AI_MODEL 직무 요건 기반 포트폴리오 어필 전략 분석
- **Phase**: Portfolio / Balance Tooling Strategy
- **Done**:
  - AI MODEL 팀의 RL 시뮬레이터, 밸런스 테스트 자동화, 시뮬레이션 실험 프레임워크 요구사항과 현재 프로젝트 자산을 매핑
  - AI NPC보다 결정적 로그라이크 시뮬레이션과 밸런스 자동화 파이프라인을 포트폴리오 중심축으로 세우는 전략을 정리
  - 현재 부족한 RL/PyTorch 실구현 증거를 `RoguelikeSim` + Monte Carlo + DQN/PPO baseline으로 보완하는 방향을 제안
  - 자기소개서/포트폴리오에서 어필할 문제정의, 기술 구조, 산출 지표, 인터뷰 설명 포인트를 도출
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (지원 전략/도구 제안이며 게임 규칙 결정 없음)
- **Next**: `Tools/RoguelikeSim` 최소 구현 범위를 확정하고, 1차 실험 리포트 샘플을 포트폴리오 문서로 작성
- **Agent**: Codex

### 2026-05-22 18:55 — 로그라이크 한정 실험 프레임워크 범위 재판단
- **Phase**: Core Run Design & Balance Tooling Review
- **Done**:
  - 사용자가 "다양한 장르"가 아니라 로그라이크 프레임워크로 한정할 경우의 적합성을 재검토
  - 범용 장르 프레임워크가 아닌 런 생성/정책/지표/파라미터 스윕 중심의 `RoguelikeSim`/밸런스 랩이면 프로젝트 구조와 정합성이 높다고 판단
  - 기존 결정적 run context, ScriptableObject data, AutoResolveCombat, EditMode 테스트를 재사용하는 방향을 권고
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (도구 범위 판단만 기록)
- **Next**: 구현 시 `IRunPolicy`, `RunSimulationResult`, `BalanceSimulationRunner`를 최소 단위로 시작하고 외부 RL 의존성은 보류
- **Agent**: Codex

### 2026-05-22 18:53 — RL/AI 밸런스 시뮬레이션 반영 가능성 검토
- **Phase**: Core Run Design & Balance Tooling Review
- **Done**:
  - GDD P3/P4/D-029/OQ-019 제약을 기준으로 RL 시뮬레이터, AI 밸런스 테스트, 범용 실험 프레임워크 반영 가능성을 점검
  - 현재 repo의 Unity 6000.4.3f1, 런타임/테스트 asmdef, 결정적 RNG, AutoResolveCombat, catalog 기반 테스트 구조를 확인
  - 세 항목 모두 본편 기능이 아니라 오프라인 밸런스/QA 도구로 제한할 때 정합성이 높다고 판단
  - 강화학습은 즉시 구현보다 seed replay + scripted/heuristic policy + Monte Carlo 리포트 이후 후속 단계로 권고
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (새 게임 규칙/밸런스 결정 없음)
- **Next**: First Build Surface 직접 플레이/테스트 확인 후, 순수 C# 시뮬레이션 runner와 지표 CSV부터 작게 추가
- **Agent**: Codex

### 2026-05-22 15:20 — SWORD_03 피의 서약 과부하 계약 반영
- **Phase**: Core Run Design & Build Surface
- **Done**:
  - D-031/OQ-020의 기존 비자살 HP 비용 계약을 HP 3 이하 전투당 1회 과부하 계약으로 갱신
  - SWORD_03 Attack 보너스와 HP 비용 판정을 적 반격 전 순서로 정리하고, 과부하 사용 상태를 전투 시작마다 reset
  - 전투 feedback에 `blood overload`/`피의 서약 과부하` 표시를 추가하고 관련 EditMode 테스트를 보강
  - repo design docs와 GDD v0.11.2에 변경 계약을 반영, OQ-019는 open 유지
- **Files**: 변경 9개+SSOT (`PrototypeRunState.cs`, `CombatController.cs`, `RuntimeShellTests.cs`, `design/abilities.md`)
- **GDD impact**: D-031 갱신 / GDD v0.11.2 / OQ-019 open 유지
- **Blockers**: Unity batchmode가 LicenseClient channel timeout으로 테스트 실행 전에 중단되어 GUI Test Runner 확인 필요
- **Next**: GUI EditMode/PlayMode에서 SWORD_03 테스트와 전투 smoke를 확인한 뒤 커밋/push 판단
- **Agent**: Codex

### 2026-05-22 14:09 — First Build Surface runtime subset 구현
- **Phase**: Core Run Design & Build Surface
- **Done**:
  - 런 reachable build surface에서 dead pick 노출을 차단하고 controlled item pool을 catalog/shop/reward 경로에 연결
  - `SWORD_01/02/03`, `ARTS_03`, `GUARD_01` 능력 asset과 `FRENZY` 검 x3 시너지 asset/runtime 효과를 추가
  - `SWORD_03` HP 비용/추가 피해, `광폭` Attack 추가타/연쇄 보너스, `ARTS_03` skill direct damage를 전투 resolver에 반영
  - Shop/combat/harness 테스트를 새 build surface 기준으로 갱신하고 PlayMode 및 screenshot harness를 확인
- **Files**: 변경/추가 37개 내외 (`PrototypeRunState.cs`, `EncounterRuntimeCatalogBuilder.cs`, Ability/Synergy SO assets, RuntimeShellTests 등)
- **GDD impact**: SSOT 변경 없음 — D-031/OQ-020 구현 반영, OQ-019 open 유지
- **Blockers**: Unity GUI EditMode는 직접 창 조작이 불안정했고 batch `-runTests` XML 생성이 동작하지 않아 기존 GUI pass와 PlayMode/screenshot/정적 검증으로 보완
- **Next**: 직접 플레이로 검 x3 체감, `광폭`, `ARTS_03`, controlled item 선택감을 확인한 뒤 Balance Pass 범위를 정한다.
- **Agent**: Codex

### 2026-05-22 11:23 — First Build Surface design docs를 Proto 기준선에 반영
- **Phase**: Core Run Design & Balance Lock
- **Done**:
  - First Build Surface 개발 brief와 Playable Build Surface Lock spec을 repo design docs 기준선에 포함
  - D-031/OQ-020 기준값 정합을 위해 능력/아이템/시너지 design docs 변경만 분리
  - dirty main worktree의 비문서 변경을 제외하고 clean `origin/Proto` 기준선 위 문서 커밋만 push
  - clean 개발 worktree에서 brief/spec 파일 가시성을 재검증
- **Files**: repo 변경 5개 (`design/first-build-surface-development-brief.md`, `design/playable-build-surface-lock-spec.md`, `design/abilities.md`)
- **GDD impact**: 없음 (D-031/OQ-020 반영분의 repo 기준선 publish)
- **Next**: 개발 세션은 `origin/Proto`에서 first Build Surface brief를 읽고 첫 구현 배치 착수
- **Agent**: Codex

### 2026-05-22 10:49 — OQ-020 첫 플레이테스트 기준값 입력
- **Phase**: Core Run Design & Balance Lock
- **Done**:
  - `SWORD_03` Attack HP 3 비자살 비용과 해당 공격 피해 +5 기준값을 GDD/design 명세에 반영
  - `광폭` Attack 추가타 ATK×0.5, 직전 Attack 연쇄 시 ATK×0.75, Defend/Skill chain reset 기준값을 반영
  - `반사` Defend 받은 피해 50% 반사와 방어 후 다음 Attack 1회 ×1.5 반격 창 기준값을 반영
  - Build Surface Lock spec과 첫 개발 brief에서 OQ-020 numeric blocker 표기를 기준값 입력 완료 상태로 갱신
- **Files**: 변경 7개 (`hwiglija-tower-gdd.md`, `design/abilities.md`, `design/synergies.md`)
- **GDD impact**: OQ-020 close / GDD v0.11.1 기준값 patch
- **Next**: 개발 세션은 first Build Surface brief 범위로 구현 착수, `ITEM_07`은 OQ-019 전까지 보류
- **Agent**: Codex

### 2026-05-22 10:35 — OQ-018 효과 계약과 첫 개발 brief 확정
- **Phase**: Core Run Design & Balance Lock
- **Done**:
  - D-031로 `SWORD_03` 비자살 HP-cost 강공, `광폭` 공격 연쇄 break, `반사` 다음 공격 1회 강반격 창 계약을 기록
  - OQ-018을 effect contract 기준으로 close하고 숫자 입력은 첫 플레이테스트 baseline OQ-020으로 분리
  - Build Surface Lock spec에서 OQ-018 대기 상태를 D-031 계약 잠금/OQ-020 숫자 대기로 갱신
  - dead pick 차단, controlled item pool, 검 x3 surface, `ARTS_03` skill 축을 첫 개발 brief로 확정
- **Files**: 변경 7개 (`hwiglija-tower-gdd.md`, `design/first-build-surface-development-brief.md`, `design/playable-build-surface-lock-spec.md`)
- **GDD impact**: D-031 추가 / OQ-018 close / OQ-020 추가
- **Next**: PM이 OQ-020 기준값을 입력하면 개발 세션이 첫 Build Surface brief 범위로 구현 착수
- **Agent**: Codex

### 2026-05-22 10:12 — OQ-017 축 잠금과 Build Surface Lock Spec 작성
- **Phase**: Core Run Design & Balance Lock
- **Done**:
  - D-030으로 `SWORD_03`, `광폭`, `반사`, `ITEM_07`의 관계 비의존 대체 빌드 축을 잠금
  - OQ-017을 close하고 exact 효과/수치 OQ-018, `ITEM_07` 적 패턴 결과 계약 OQ-019로 분리
  - 능력 12 / 일반 아이템 12 / 유물 6 / 시너지 4의 target role과 current runtime status를 Build Surface Lock spec에 정리
  - Runtime support matrix, gap list, dead-pick 차단 원칙, 1차 개발 배치 초안을 Balance Pass 전 handoff로 분리
- **Files**: 변경 7개 (`hwiglija-tower-gdd.md`, `hwiglija-tower-design-journal.md`, `design/playable-build-surface-lock-spec.md`)
- **GDD impact**: D-030 추가 / OQ-017 close / OQ-018·OQ-019 추가
- **Next**: 개발 세션은 dead pick pool 차단과 최소 작동 빌드 표면 노출부터 착수하고, OQ-018/OQ-019는 계약 확정 후 이어서 구현
- **Agent**: Codex

### 2026-05-22 09:55 — OQ-017 대체 빌드 축 후보 정리
- **Phase**: Core Run Design & Balance Lock
- **Done**:
  - 최신 D-029/D-022/D-025/OQ-017 기준으로 관계 상태 전투 modifier 재도입 금지선을 재확인
  - `ABILITY_SWORD_03`, `ITEM_07`, 검 `광폭`, 결 `반사` 심화 슬롯별 대체 빌드 축 후보를 비교
  - 검은 공격 연쇄/반격 타이밍, 결은 방어→반격 전환, `ITEM_07`은 패턴 대응 보조 축을 추천 조합으로 정리
  - 효과 수치와 SO/런타임 변경은 사용자 확정 전 미확정으로 유지
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (OQ-017 설계 후보 단계, 확정 전)
- **Next**: PM이 OQ-017 추천 조합 또는 대안을 고른 뒤 GDD/design 명세 갱신
- **Agent**: Codex

### 2026-05-22 09:41 — 붕괴도 비전투화와 빌드 표면 우선 결정 잠금
- **Phase**: Core Run Design & Balance Lock
- **Done**:
  - Glitch 설계 용어를 플레이어 직관의 붕괴도로 치환하고 관계·회복 압박 축으로 남기는 D-029를 GDD에 기록
  - D-022 적 턴 붕괴 연동 타이머 축소를 철회하고 D-025 정서적 전투 modifier 원리를 재고 상태로 갱신
  - `ABILITY_SWORD_03`, `ITEM_07`, 검 시너지 `광폭`, 결 시너지 `반사`의 관계 상태 전투 심화 로직을 OQ-017 대체 설계 대상으로 분리
  - 로컬 능력/아이템/시너지/밸런스 명세가 붕괴도 전투 키를 그대로 구현 지시하지 않도록 정리
- **Files**: 변경 7개 (`hwiglija-tower-gdd.md`, `hwiglija-tower-design-journal.md`, `design/synergies.md`)
- **GDD impact**: D-029 추가 / D-022·D-025 갱신 / OQ-017 추가
- **Next**: 시스템 디자인 세션에서 OQ-017 대체 빌드 축과 Playable Build Surface Lock Spec을 확정
- **Agent**: Codex

### 2026-05-22 01:04 — Shop disabled card clarity polish
- **Phase**: W3-2
- **Done**:
  - Shop unavailable-choice hints now preserve price and one-line reward/effect summaries before the disabled state.
  - Locked Shop cards keep public product title, Gold cost, effect copy, and `Gold 부족` feedback on the same comparison axis as purchasable cards.
  - Added EditMode coverage for disabled Shop card public copy and tightened the opening Shop screenshot QA fixture around low-Gold comparison details.
  - Revalidated GUI EditMode, GUI PlayMode, and the 01-12 portrait screenshot harness from the clean result-polish worktree.
- **Files**: 변경 4개 (`PrototypeEncounterRuntimeResolver.cs`, `PrototypeHud.cs`, `PresentationLayerTests.cs`, `PortraitUiScreenshotQaTests.cs`)
- **GDD impact**: 없음 (상점 경제/상품 수 변경 없음, P3/P5 정렬 UI polish)
- **Next**: QA 확인 후 Top HUD density 진단
- **Agent**: Codex

### 2026-05-21 23:54 — Shop clarity 우선 UI 진단
- **Phase**: W3-2
- **Done**:
  - `69fa803` 기준 Shop Prototype 화면과 1080x1920 portrait QA 캡처를 검토
  - 5상품 카드 구조와 기존 구매 흐름을 유지하는 범위에서 상품 비교/구매 가능성/구매 결과 가독성 문제를 분리
  - Rest companion feel과 Top HUD density를 후속 후보로 비교해 개발 세션 handoff 우선순위를 정리
- **Files**: 변경 1개 (`hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (상점 경제/전투 규칙 변경 없음, P3/P5 정렬 UI 진단)
- **Next**: 개발 세션에서 Shop disabled card 정보 위계와 구매 후 변화 피드백을 먼저 polish
- **Agent**: Codex

### 2026-05-21 23:54 — Result/Combat 승인 후 UI 전용 진단 방향 확정
- **Phase**: 운영
- **Done**:
  - PM 세션에서 `e647b7368ac9e0a7deb2e6e3f4d10729f4b7e2d9` 개발 보고 승인: Result summary icon polish
  - QA 세션에서 `69fa803bee49cc5850431fbbcfad886728285436` 검증 승인: Combat decision feedback
  - QA verdict를 `ship blocker no`로 기록하고, 공격/방어/스킬 post-action feedback 전용 visual screenshot coverage 부재를 P2 메모로 보존
  - 배포 세션은 사용자 OK 전까지 보류하며, 다음 운영 방향은 UI 전용 세션에서 Shop clarity부터 진단하기로 확정
- **Files**: 기록만 갱신
- **GDD impact**: 없음
- **Next**: UI 전용 세션에서 Shop clarity를 먼저 진단하고, 배포 재개는 사용자 OK를 새 trigger로 대기
- **Agent**: Codex

### 2026-05-21 22:42 — Combat decision feedback polish
- **Phase**: W3-2
- **Done**:
  - Reordered combat feedback copy around selected action and immediate result for attack, defend, and skill.
  - Added received-damage context for attack/skill and kept defend reduction visible in the compact combat log.
  - Updated EditMode and PlayMode public-copy assertions for the combat feedback contract.
  - Revalidated GUI EditMode, GUI PlayMode, and the 01-12 portrait screenshot harness from the result-polish worktree.
- **Files**: 변경 3개 (`PrototypeHud.cs`, `PresentationLayerTests.cs`, `PrototypeRoomSmokeTests.cs`)
- **GDD impact**: 없음 (D-022/D-025 전투 규칙 유지, P3/P5 정렬 UI polish)
- **Next**: QA 확인 후 Shop clarity polish
- **Agent**: Codex

### 2026-05-20 23:40 — Result summary icon polish
- **Phase**: W3-2
- **Done**:
  - 결과 요약을 Gold/HP/Affinity/Item/Ability/Memory 아이콘 칩과 짧은 수치로 분리
  - 결과 텍스트를 `아이템 +1`, `기억 +1`, `Gold +8` 형태로 압축
  - 기존 presentation icon slot을 재사용하고 HP는 fallback chip으로 처리
  - EditMode/PlayMode/screenshot harness 검증 통과 확인
- **Files**: 변경 2개 (`PrototypeHud.cs`, `PresentationLayerTests.cs`)
- **GDD impact**: 없음 (P3/P5 정렬 UI polish, 신규 결정 없음)
- **Next**: Combat decision feedback 또는 Shop clarity polish
- **Agent**: Codex

---

### 2026-05-20 23:13 — 2550281 기준 Android APK 재빌드
- **Phase**: 제출/배포
- **Done**:
  - `/private/tmp/hwigi-play-latest` detached HEAD `2550281` / `origin/Proto` 일치 확인
  - Android 제출 설정(Product `회귀자는 탑을 오른다`, package `com.godju.hwigitower`, ARM64/IL2CPP/non-development)을 빌드 시점에 적용해 APK 생성
  - `/Users/godju/Downloads/AI Game/hwigi-tower/Builds/Android/Hwigitower-Prototype-20260520.apk` 및 업로드용 `Hwigitower-Prototype.apk` 복사본 생성
  - `aapt` 로 package/native-code 확인, `apksigner` v2 signature verify 통과, `git diff --check` 통과
  - source worktree tracked 변경 원복, `Assets/_Recovery/**` untracked는 그대로 제외
- **Files**: repo 변경 없음 (APK 산출물 2개는 ignored build output)
- **GDD impact**: 없음 (D-018 Android APK 우선순위 이행)
- **Blockers**: `adb devices -l` 기기 없음으로 실기 smoke 미실행. Google Drive connector 승인이 완료되지 않아 업로드 링크 생성 불가.
- **Next**: Android 기기 RSA 승인/USB mode 확인 후 install-run smoke, Google Drive 권한 연결 후 `Hwigitower-Prototype.apk` 업로드/링크 생성
- **Agent**: Codex

---

### 2026-05-20 22:30 — Polished map and utility flow tests repaired
- **Phase**: W3-2
- **Done**:
  - Updated EditMode expectations for `enc_complete_bg`, `상태/지도/장비`, and the fixed 10-node branch map.
  - Repaired PlayMode route helpers for click-to-open maps, 5-product shop cards, skill picker combat flow, and boss-layer targeting.
  - Extended screenshot harness coverage through `12_boss_gate_choices.png`.
  - Fixed floor map combat classification so combat slots resolve from StartCombat effects.
- **Files**: 변경 5개 (`PrototypeFloorMap.cs`, PlayMode/EditMode test files)
- **GDD impact**: 없음 (existing UI validation alignment; 신규 결정 없음)
- **Next**: Manual QA can continue from pushed `origin/Proto` 2550281; remaining UI polish can focus on result summary icons.
- **Agent**: Codex

### 2026-05-20 01:30 — Encounter map and utility polish feedback batch
- **Phase**: W3-2
- **Done**:
  - Map entry is now user-triggered from `지도`, with fixed 10 branch nodes in a 4 event / 4 combat / 2 rest mix and bottom-to-top layout.
  - Rest, event, shop, combat, status, and equipment UI layers were reworked around the latest manual QA feedback.
  - Shop encounters now expose five purchasable products, rest uses the rest background without Mataios spotlight, and run clear binds `enc_complete_bg`.
  - Combat action labels are `공격` / `방어` / `스킬`, with skill selection panel and player-style Mataios combat stats.
- **Files**: 변경/추가 12개 (`PrototypeHud.cs`, `PrototypeFloorMap.cs`, shop encounter assets, `enc_complete_bg.png`)
- **GDD impact**: 없음 (existing UI polish and asset binding; 신규 잠금 결정 없음)
- **Blockers**: Unity batch PlayMode hit `LicenseClient-godju` timeout; earlier EditMode command compiled/imported but produced no test XML. Screenshot harness not rerun.
- **Next**: GUI Test Runner에서 EditMode/PlayMode와 01-11 screenshot harness 재검증 후 push 판단.
- **Agent**: Codex

### 2026-05-19 14:30 — Boss gate choice and utility UI repair
- **Phase**: W3-2
- **Done**:
  - Boss gate choices reduced to `전투 시작` / `돌아간다`, with boss prepare choices removed from encounter assets.
  - Boss return and utility map return now cancel the selected route node without floor clear or ending progression.
  - Persistent top utility buttons added for status, map, and loadout summaries without Glitch exposure.
  - GUI Test Runner EditMode/PlayMode and portrait screenshot harness revalidated.
- **Files**: 변경 8개 (`PrototypeHud.cs`, `PrototypeRunState.cs`, boss encounter assets, tests)
- **GDD impact**: 없음 (P3/P4/P5-aligned UX repair, 신규 결정 없음)
- **Blockers**: Unity batchmode still hits LicenseClient timeout; GUI Test Runner used for Unity validation.
- **Next**: Manual touch QA for boss gate utility placement, then result summary icon polish.
- **Agent**: Codex

### 2026-05-19 07:54 — Floor map spacing and rest density polish
- **Phase**: W3-2
- **Done**:
  - Floor map UI panel expanded vertically so route nodes are easier to tap.
  - Map progression flipped to bottom-to-top, with start at bottom and shop/boss above.
  - Branch map generation now places each route step once, reducing duplicate Rest nodes and repeated encounter auto-resolution.
  - EditMode/PlayMode/screenshot QA revalidated.
- **Files**: 변경 5개 (`PrototypeFloorMap.cs`, `PrototypeHud.cs`, `PrototypeRunState.cs` 등)
- **GDD impact**: 없음 (map UX polish, 신규 디자인 결정 없음)
- **Next**: manual touch QA blocker or result summary icon polish
- **Agent**: Codex

### 2026-05-19 07:28 — Encounter screen state layer cleanup
- **Phase**: W3-2
- **Done**:
  - Map/Shop/Rest screen states now hide incompatible result/objective/debug/legacy visual layers
  - Rest action select/input/response phases are separated so cards and input do not overlap
  - Shop disabled purchase copy reduced to `Gold 부족`, map nodes enlarged, public companion/debug copy cleaned
  - EditMode/PlayMode/screenshot QA revalidated
- **Files**: 변경 1개 (`PrototypeHud.cs`)
- **GDD impact**: 없음 (UI polish, 신규 디자인 결정 없음)
- **Next**: manual touch QA blocker or result summary icon polish
- **Agent**: Codex

---

### 2026-05-18 21:50 — APK distribution prep and codebase assessment
- **Phase**: W3-2
- **Done**:
  - Project structure, AGENTS.md, and recent PROGRESS logs assessed
  - APK file verified at `Builds/Android/Hwigitower-Prototype-20260518.apk`
  - APK distribution preparation completed
- **Files**: 변경 없음 (PROGRESS 항목 추가)
- **GDD impact**: 없음
- **Next**: Final delivery instructions to user
- **Agent**: Codex

---

### 2026-05-18 21:44 — GUI PlayMode test runner regression repair
- **Phase**: W3-2
- **Done**:
  - GUI Test Runner에서 누적되던 run save/request 상태를 PlayMode smoke/screenshot fixtures 전후로 격리
  - final boss/rest/full-run/screenshot harness 실패 원인을 production route 변경 없이 테스트 상태 누수로 수리
  - PlayMode 19/19, EditMode 128/128, screenshot 01-11 1080x1920, diff/forbidden 검증 완료
- **Files**: 변경 2개 (`PrototypeRoomSmokeTests.cs`, `PortraitUiScreenshotQaTests.cs`)
- **GDD impact**: 없음 (QA/test isolation repair, 신규 디자인 결정 없음)
- **Next**: manual touch QA blocker 또는 result summary icons polish
- **Agent**: Codex

---

### 2026-05-18 21:16 — Portrait frame and UI drop-in polish
- **Phase**: W3-2
- **Done**:
  - `char_player_portrait_01`를 combat dock player portrait로 교체하고 `char_player_bust_01`도 import 보존
  - `char_mataios_portrait_01`를 combat dock 전용 Mataios portrait로 바인딩해 rest/spotlight bust와 분리
  - optional combat portrait frame slot을 추가해 별도 `ui_portrait_frame.png`가 없어도 UI가 깨지지 않도록 null-safe 처리
  - EditMode/PlayMode/screenshot QA 재검증 및 remote `Proto` push 완료
- **Files**: 변경/추가 13개 (주요: `DemoPresentationData.cs`, `PrototypeHud.cs`, `SO_DemoPresentationData.asset`, `char_*_portrait_01.png`)
- **GDD impact**: 없음 (drop-in asset binding, 신규 디자인 결정 없음)
- **Blockers**: `Assets/_Project/Art/UI/Portraits/ui_portrait_frame.png`와 `sfx_ui_disabled.wav`는 미제공 상태
- **Next**: missing frame/disabled SFX drop-in 시 슬롯 연결 또는 result summary icons polish
- **Agent**: Codex

---

### 2026-05-18 20:42 — Player portrait and audio cue binding
- **Phase**: W3-2
- **Done**:
  - `char_player_standing_01` player portrait를 `SO_DemoPresentationData.defaultPlayerPortrait`와 combat party dock에 바인딩
  - Lobby/Combat/Rest BGM과 UI/Combat/Shop/Rest SFX cue assets를 `SO_AudioCueCatalog`에 등록
  - Lobby/PrototypeRoom scene이 audio cue catalog를 주입하고 combat/rest/shop/lobby interaction cue requests를 검증
  - EditMode/PlayMode/screenshot QA 재검증 완료
- **Files**: 변경/추가 61개 (주요: `SO_AudioCueCatalog.asset`, `SO_DemoPresentationData.asset`, `PrototypeAudioService.cs`, `PrototypeRoomController.cs`)
- **GDD impact**: 없음 (asset/audio binding, 신규 디자인 결정 없음)
- **Blockers**: `char_player_bust_01.png`와 `sfx_ui_disabled.wav`는 제공되지 않아 actual asset 기준으로 바인딩
- **Next**: missing disabled SFX/player bust drop-in 시 교체 또는 result summary icons polish
- **Agent**: Codex

---

### 2026-05-18 19:47 — Floor 3-5 shop route validation repair
- **Phase**: W3-2
- **Done**:
  - GUI EditMode `PresentationLayerTests`의 `InputSystemUIInputModule` default action asset 생성 실패를 테스트 환경에서 회피하고 Play runtime 동작은 유지
  - Floor 3/4/5 shop route 테스트 기대값을 `ENC_SHOP_03/04/05`와 전용 choice id로 갱신
  - QA shop screenshot 상태가 활성 shop presentation slot을 유지하도록 보강하고 09/10/11 floor shop screenshot harness를 통과
  - EditMode 126/126, PlayMode 19/19, `git diff --check`, forbidden search 검증 완료
- **Files**: 변경 4개 (`PrototypeHud.cs`, `RuntimeShellTests.cs`, `PrototypeRoomSmokeTests.cs`, `PortraitUiScreenshotQaTests.cs`)
- **GDD impact**: 없음 (기존 Floor 3-5 shop 노출 검증 보강, 신규 디자인 결정 없음)
- **Next**: result summary icons polish 또는 manual touch QA blocker 수정
- **Agent**: Codex

---

### 2026-05-18 11:49 — Android APK submission build generated
- **Phase**: W3-2
- **Done**:
  - `origin/Proto` 기준 5468c31 이상 확인 후 로컬 HEAD 7f6856c 에서 Android release APK 빌드 성공
  - Android Build Support/SDK/NDK/OpenJDK/adb 설치 경로 확인
  - Product Name `회귀자는 탑을 오른다`, package `com.godju.hwigitower`, ARM64/IL2CPP/Portrait 제출 설정을 BuildScript/PlayerSettings에 반영
  - `Builds/Android/Hwigitower-Prototype-20260518.apk` 생성 및 aapt/apksigner 검증
  - ADB daemon은 실행됐지만 연결 기기 목록이 비어 실기기 smoke는 blocked로 기록
- **Files**: 변경/추가 3개 (`BuildScript.cs`, `ProjectSettings.asset`, `Docs/Build/Android_APK_Build_2026-05-18.md`)
- **GDD impact**: 없음 (D-018 Android APK 우선순위 이행, 신규 디자인 결정 없음)
- **Blockers**: `adb devices -l` 에 연결 기기 없음. 앱 아이콘 슬롯은 명시 연결 전이라 Unity/default icon 상태.
- **Next**: Android 기기 USB debugging 승인 후 APK install/launch/manual smoke 완료, 이후 Google Drive/Dropbox 업로드 링크 생성
- **Agent**: Codex

---

### 2026-05-18 11:35 — Floor 3-5 shop route exposure
- **Phase**: W3-2
- **Done**:
  - Floor 3/4/5 전용 `ENC_SHOP_03`, `ENC_SHOP_04`, `ENC_SHOP_05` EncounterData 자산 추가
  - `SO_Room_Prototype` Floor 3~5 shop step을 전용 shop encounter로 교체
  - QA 전용 floor jump helper를 추가해 실제 Floor 3/4/5 shop presentation screenshot 상태를 검증하도록 확장
  - EditMode route coverage와 PlayMode screenshot harness에 floor 3/4/5 shop assertions 추가
- **Files**: 변경/추가 11개 (주요: `SO_Room_Prototype.asset`, `PrototypeRunState.cs`, `PortraitUiScreenshotQaTests.cs`, `SO_Encounter_ENC_SHOP_03~05.asset`)
- **GDD impact**: 없음 (D-009/OQ-012 기존 5층·상점 구조 노출 보강, 신규 결정 없음)
- **Blockers**: Unity EditMode/PlayMode 실행은 LicenseClient channel timeout으로 테스트 본문 진입 전 차단
- **Next**: Unity licensing 복구 후 EditMode/PlayMode/screenshot harness 재실행, 이후 result summary icons polish
- **Agent**: Codex

---

### 2026-05-18 08:16 — Rest action card art binding
- **Phase**: W3-2
- **Done**:
  - `origin/Proto` 최신 확인 (`66461c2` 기준, already up to date)
  - `Assets/_Project/Art/UI/RestActions/` 카드 PNG 3종을 Sprite meta로 import하고 `SO_DemoPresentationData` 슬롯에 바인딩
  - Rest 선택지를 3개 가로 아이콘 카드(`대화`/`훈련`/`휴식`) + 짧은 preview 구조로 교체
  - 기존 Talk/Train/Recover 자연어 입력, 버프 중복 방지, 회복/Glitch 내부 처리 흐름 유지
  - Rest card label/icon/EditMode/PlayMode 검증과 screenshot QA 8종 재실행
- **Files**: 변경/추가 12개 (주요: `DemoPresentationData.cs`, `PrototypeHud.cs`, `SO_DemoPresentationData.asset`, `Art/UI/RestActions/*`)
- **GDD impact**: 없음 (P1/P2/P3 정합: 안식 선택 가독성 보강, NPC spotlight/자연어 흐름 유지, 신규 디자인 결정 없음)
- **Blockers**: 없음
- **Next**: shop/rest encounter backgrounds 및 `Assets/_Project/Art/Icons/` 후속 바인딩
- **Agent**: Codex

---

### 2026-05-17 20:02 — Floor 1-5 event script pack v0.1 작성
- **Phase**: W3-2
- **Done**:
  - Floor 1~5 각 4개, 총 20개 Event markdown 작성
  - 새 이벤트 문서 포맷에 맞춰 `_EVENT_SCRIPT_TEMPLATE.md` 갱신
  - `event_pack_floor01_05_index_v0.1.md` 인덱스 작성
  - `event_conversion_handoff_v0.1.md` 개발 변환 핸드오프 작성
  - 금지 패턴, DTO 키워드, floor별 이벤트 수, `git diff --check` 검증 통과
- **Files**: 변경/추가 23개 (주요: `Docs/Content/EventScripts/floor01-05/**`, `event_pack_floor01_05_index_v0.1.md`, `event_conversion_handoff_v0.1.md`)
- **GDD impact**: 없음 (D-009 인카운터 수량 내 draft 산출물, 최종 NPC 대사/엔딩 진실 확정 없음)
- **Next**: event script pack을 EncounterData/SO와 floor event pool로 변환
- **Agent**: Codex

---

### 2026-05-15 07:44 — Floor 1-2 event scripts draft 정리
- **Phase**: W3-2
- **Done**:
  - EventScripts 템플릿을 상세 이벤트 원고/데이터 명세 형식으로 갱신
  - 기존 `EVT_F01_JAR_ROOM`을 새 템플릿 기준으로 재정리
  - Floor 1 신규 `EVT_F01_ABANDONED_CAMP` 작성
  - Floor 2 신규 `EVT_F02_OVERGROWN_GARDEN`, `EVT_F02_MERCENARY_GEAR` 작성
  - 개발 전환 메모와 금지어/raw ID player-facing 점검 기록 추가
- **Files**: 변경/추가 6개 (주요: `Docs/Content/EventScripts/floor01/**`, `Docs/Content/EventScripts/floor02/**`, `_DEV_CONVERSION_NOTES.md`)
- **GDD impact**: 없음 (D-009 인카운터 수량 내 draft 산출물, 최종 서사 결정 없음)
- **Next**: Floor 3-5 이벤트 초안 확장 후 EncounterData/floor event pool 변환
- **Agent**: Codex

---

### 2026-05-08 12:01 — ending choice flow push 완료
- **Phase**: W2-1
- **Done**:
  - `git push origin Proto` 재시도 성공
  - 원격 `origin/Proto` 를 `d94697f..dd83d0f` 로 업데이트
  - working tree 의 excluded local 파일은 그대로 유지
- **Files**: repo 파일 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: Unity LicenseClient 복구 후 blocked fresh EditMode/PlayMode/Android smoke 재검증
- **Agent**: Codex

---

### 2026-05-08 11:46 — ending choice flow 커밋, push 인증 차단
- **Phase**: W2-1
- **Done**:
  - 지정 7개 파일만 stage해 `dd83d0f` (`Add ending choice flow`) 커밋 생성
  - repo 내부 PROGRESS에 구현 완료, C# compile pass, Unity tests blocked by license / LicenseClient 장애 명시
  - `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb` 는 커밋 제외 유지
  - `git diff --check` 및 forbidden search 통과 확인
- **Files**: 커밋 7개 (`PrototypeRoomController.cs`, `PrototypeRunSnapshot.cs`, `PrototypeRunState.cs`, `PrototypeHud.cs`, `RuntimeShellTests.cs`, `PrototypeRoomSmokeTests.cs`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음
- **Blockers**: `git push origin Proto` 는 GitHub HTTPS 인증 실패로 blocked (`gh` token invalid / username read unavailable)
- **Next**: `gh auth login -h github.com` 또는 Git credential 복구 후 `git push origin Proto` 재실행
- **Agent**: Codex

---

### 2026-05-07 21:33 — 외주 encounter pack 원복 + BossGate runtime override 분리
- **Phase**: W2-1
- **Done**:
  - `21382b6`의 `Docs/ExternalSpecs/EncounterPipeline/v0.3/pack_FULL_25_V003.json` 변경이 외주 pack 보관본 수정이었음을 확인하고 원본 내용으로 되돌림
  - 외부 `/Users/godju/Downloads/외주 폴더/pack_FULL_25_V003.json`도 repo 보관본과 일치하도록 복구
  - baker가 외주 pack을 그대로 구운 뒤 Unity prototype runtime override로 `ENC_COMBAT_GATE_02` BossGate SO 값을 재적용하도록 분리
  - EditMode regression으로 full pack bake 이후에도 `BOSS_GATE_01`이 유지되는지 검증 추가
  - fresh EditMode 74/74, fresh PlayMode 12/12 통과
- **Files**: 변경/추가 4개 + pack 원복 1개 (주요: `EncounterRuntimeCatalogBuilder.cs`, `EncounterPipelineV02Baker.cs`, `EncounterPipelineV02Tests.cs`)
- **GDD impact**: 없음
- **Blockers**: 범위 밖 local 파일 `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb`는 커밋 제외
- **Next**: origin/Proto push 후 Floor 2 content/boss pattern 확장
- **Agent**: Codex

---

### 2026-05-07 21:17 — Dedicated BossGate balance + clear/failure presentation 보강
- **Phase**: W2-1
- **Done**:
  - Floor 2 BossGate를 demo combat enemy가 아닌 전용 `BOSS_GATE_01` enemy/SO로 연결
  - BossGate 밸런스를 HP 28 / ATK 4 / gold +16 / glitch -4 / affinity +4로 조정하고 catalog/baker source를 동기화
  - run.clear / run.failed 상태에서 choice/node interaction과 combat/result overlay를 차단하고 HUD restart 상태를 명확화
  - boss reward/clear reward 재방문 중복 방지, recall anchor 1회 fallback, restart reset 경로를 EditMode/PlayMode로 보강
  - fresh EditMode 74/74, fresh PlayMode 12/12 통과
- **Files**: 변경/추가 10개 (주요: `SO_Enemy_BOSS_GATE_01.asset`, `PrototypeRoomController.cs`, `PrototypeRoomSmokeTests.cs`)
- **GDD impact**: 없음 (기존 P1 run end/boss gate 구현 보강; 신규 결정 없음)
- **Blockers**: 범위 밖 local 파일 `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb`는 커밋 제외
- **Next**: boss 패턴/skill 선택지 다양화와 Floor 2 node subset 확장
- **Agent**: Codex

---

### 2026-05-07 20:57 — Run end/restart + Floor 2 boss gate 구현
- **Phase**: W2-1
- **Done**:
  - GDD item combo / utterance memory 승인 변경을 문서-only 커밋으로 분리 보존
  - Floor 2 마지막 combat step을 BossGate로 판정하고 victory 시 run.clear, defeat 시 run.failed 상태를 연결
  - run.restartReady 및 HUD restart 버튼을 추가하고 deterministic restart runId 정책으로 새 run state를 생성
  - ABILITY_RECALL_ANCHOR 1회 revive-like fallback을 defeat 처리 전에 적용해 run failure를 막고 중복 발동을 차단
  - fresh EditMode 71/71, fresh PlayMode 10/10 통과
- **Files**: 변경/추가 7개 (주요: `PrototypeRunState.cs`, `PrototypeRoomController.cs`, `RuntimeShellTests.cs`)
- **GDD impact**: D-023 / D-024 승인 변경 반영 (`Docs/Project/hwiglija-tower-gdd.md` 문서-only 커밋 분리)
- **Blockers**: 범위 밖 local 파일 `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb`는 커밋 제외
- **Next**: Floor 2 boss/gate 전용 Enemy/SO 밸런스와 run clear/failure presentation polish
- **Agent**: Codex

---

### 2026-05-07 15:58 — Playable core loop P0 구현
- **Phase**: W2-1
- **Done**:
  - 기존 deterministic demo path 위에 floor progression을 추가해 Floor 1 clear → Stair unlock → Floor 2 진입 흐름 구현
  - PrototypeRoom 초기 gold/shop inventory 구매 루프를 보장하고 item/ability 보유 수를 HUD에 표시
  - ITEM_FIELD_BANDAGE combat_start HP 보정, ABILITY_SCOUT skill combo, ABILITY_RECALL_ANCHOR revive-like fallback 효과를 연결
  - combat victory/defeat 보상, reward 중복 방지, NPC fallback reaction key 표시와 floor transition reflection 저장을 보강
  - fresh EditMode 66/66, fresh PlayMode 9/9 통과
- **Files**: 변경/추가 14개 (주요: `PrototypeRunState.cs`, `PrototypeHud.cs`, `SO_Room_Prototype.asset`, `RuntimeShellTests.cs`)
- **GDD impact**: 없음 (P0 playable vertical slice 구현; 최종 NPC/서사/기억 본문 결정 없음)
- **Blockers**: Floor 2 이후 boss/run clear 확장은 P1 범위. 범위 밖 local 파일 `.obsidian/workspace.json`, `Docs/Project/hwiglija-tower-gdd.md`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb`는 커밋 제외
- **Next**: Floor 2/3 boss gate와 restart/failure screen을 P1로 연결
- **Agent**: Codex

---

### 2026-05-07 14:38 — P0 presentation polish + demo art binding
- **Phase**: W2-1
- **Done**:
  - recording mode 기본 라벨/route/result를 public display 중심으로 정리하고 raw stableId/textKey는 debug toggle로 제한
  - active combat 중 DemoComplete route 표시를 막고 combat cutscene/button gating 및 combat HP/enemy visual을 보강
  - 사용자가 배치한 Shop/Moral/Memory/Combat/DemoComplete 배경, Mataios bust, enemy/UI/VFX PNG를 LFS 대상 에셋으로 import/binding
  - 1080x1920 screenshots 16-20 캡처 및 QA 문서 갱신
  - fresh EditMode 62/62, fresh PlayMode 9/9 통과
- **Files**: 변경/추가 45개 (주요: `PrototypeHud.cs`, `PrototypeCutscenePlayer.cs`, `SO_DemoPresentationData.asset`, `Assets/_Project/Art/**`, QA screenshots)
- **GDD impact**: 없음 (기존 W2 presentation polish 범위; 최종 텍스트/서사 결정 없음)
- **Blockers**: CombatGate/DemoComplete focus art는 recording-safe scaffold이며 최종 승인 plate는 별도 필요. 범위 밖 local 파일 `.obsidian/workspace.json`, `Docs/Project/hwiglija-tower-gdd.md`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb`는 커밋 제외
- **Next**: final cutscene plates/button sprite styling 적용 또는 Android build smoke
- **Agent**: Codex

---

### 2026-05-07 11:35 — Presentation/cutscene screenshot QA + overlay fixes
- **Phase**: W2-1
- **Done**:
  - `Proto` push 후 `origin/Proto` HEAD `7092b17` 확인
  - 1080x1920 presentation/cutscene screenshots 10-15 캡처 및 QA 문서 갱신
  - DemoComplete cutscene early trigger, cutscene hide lifecycle, memory result raw flag leakage 수정
  - fresh EditMode 62/62, fresh PlayMode 9/9 통과
- **Files**: 변경/추가 10개 (주요: `PrototypeHud.cs`, `PrototypeCutscenePlayer.cs`, `PrototypeRoomSmokeTests.cs`, viewport QA 문서, screenshots 6개)
- **GDD impact**: 없음
- **Blockers**: forbidden search는 외주 test plan의 금지어 체크리스트와 기존 정책 문서만 hit. 범위 밖 local 파일 `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_*.ipynb`는 커밋 제외
- **Next**: asset slots에 실제 background/enemy/memory sprite 연결 또는 cropped transparent Mataios portrait 교체
- **Agent**: Codex

### 2026-05-07 09:42 — Presentation screen layer 구현 + 외주 specs cherry-pick
- **Phase**: W2-1
- **Done**:
  - PrototypeHud 기본 표시를 demo presentation 중심으로 정리하고 raw stableId/textKey는 dev toggle에서만 보이게 분리
  - DemoPresentationData SO, CutsceneData SO, PrototypeCutscenePlayer scaffold 및 PrototypeRoom binding 추가
  - CombatGate panel을 HP bar 중심 presentation text로 정리하고 combat feedback/cutscene route 회귀 테스트 추가
  - fresh EditMode 62/62, fresh PlayMode 9/9 통과 후 구현 커밋 `2c42a1f` 생성
  - 브랜치 merge 없이 외주 docs 커밋 `10967ad`만 cherry-pick하여 `7092b17`로 반영
- **Files**: 변경/추가 30개 (구현 23개 + 외주 docs 7개; 주요: Assets/_Project/Scripts/UI/**, Assets/_Project/Data/Presentation/**, Docs/Outsource/Juho/W2ProductionReadiness/**)
- **GDD impact**: 없음 (P3/P4/P5 정합; 최종 텍스트/서사 결정 없음)
- **Blockers**: forbidden search는 외주 test plan의 금지어 체크리스트와 기존 정책 문서 문구만 hit. 범위 밖 local 파일: .obsidian/workspace.json, Assets/_Recovery/**, Mataios_LoRA_Training.ipynb, Mataios_LoRA_v2.ipynb, Mataios_LoRA_v3.ipynb
- **Next**: 1080x1920 Game view screenshot QA로 presentation/cutscene overlay 확인 후 asset slots에 실제 background/enemy/memory sprites 연결
- **Agent**: Codex

### 2026-05-06 23:06 — Unityless readiness docs 반영 + PrototypeRoom debug label 숨김
- **Phase**: W2-1
- **Done**:
  - 외주 문서 커밋 `3e10a69`를 `Proto`에 cherry-pick하여 `46b28fa`로 반영
  - `PrototypeRoom` world/debug node label을 녹화 기본값에서 숨기고 개발용 toggle로 복구 가능하게 구현
  - PlayMode smoke에 기본 label hidden 및 toggle restore 검증 추가
  - 1080x1920 screenshot 4장(06-09)으로 Shop/Memory/Combat/DemoComplete label hidden QA 확인
  - fresh EditMode 60/60, fresh PlayMode 7/7 통과
- **Files**: 변경/추가 16개 (주요: `PrototypeRoomController.cs`, `PrototypeSceneRuntimeBuilder.cs`, `PrototypeRoomSmokeTests.cs`, `Docs/Outsource/Juho/W2ProductionReadiness/**`, `Docs/Outsource/Juho/W2DemoReadiness/**`)
- **GDD impact**: 없음
- **Blockers**: 범위 밖 local 파일 존재: `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_Training.ipynb`는 커밋 제외
- **Next**: result/memory placeholder text density 정리 또는 Android build smoke
- **Agent**: Codex

### 2026-05-06 22:08 — PrototypeRoom 1080x1920 screenshot QA 보존 + CombatGate UI layering 수정
- **Phase**: W2-1
- **Done**:
  - 실제 Game view 1080x1920 screenshot 5장 기록 및 QA 문서 갱신
  - QA 산출물 보존 커밋 `dac6ffb Add PrototypeRoom viewport screenshots` 생성
  - CombatGate active combat 중 `demo.complete` overlay/result text 숨김 처리
  - 전투 종료 후 combat panel을 닫고 DemoComplete를 표시하도록 HUD/PlayMode smoke expectation 갱신
  - fresh EditMode 60/60, fresh PlayMode 6/6 통과
- **Files**: 변경/추가 8개 (주요: `PrototypeHud.cs`, `PrototypeRoomSmokeTests.cs`, `Docs/Outsource/Juho/W2DemoReadiness/**`)
- **GDD impact**: 없음
- **Blockers**: 범위 밖 local 파일 존재: `.obsidian/workspace.json`, `Assets/_Recovery/**`, `Mataios_LoRA_Training.ipynb`는 커밋 제외
- **Next**: debug node label 축소/숨김과 Android build smoke 준비
- **Agent**: Codex

### 2026-05-06 16:23 — PrototypeRoom portrait/combat viewport QA 기록
- **Phase**: W2-1
- **Done**:
  - 기준 HEAD `2912ea0`에서 `PrototypeRoom` portrait + interactive combat UI QA 수행
  - `.obsidian/workspace.json` 범위 밖 변경 복원 후 QA 문서만 추가
  - fresh EditMode 60/60, fresh PlayMode 6/6 결과를 QA 문서에 기록
  - viewport size, scene, commit, pass/fail table, overlap risk, required fixes 정리
- **Files**: 변경/추가 1개 (`Docs/Outsource/Juho/W2DemoReadiness/viewport_qa_result_v0.1.md`)
- **GDD impact**: 없음
- **Blockers**: 자동 screenshot hook 없음. 이번 산출물은 screenshot 없는 QA 기록이며, memory panel/portrait 근접은 human screenshot pass 권장으로 남김
- **Next**: 실제 Game view 1080x1920 screenshot capture 자동화 또는 수동 캡처 후 memory/combat panel 간격 최종 polish
- **Agent**: Codex

### 2026-05-06 14:53 — Mataios portrait + interactive demo combat UI 연결
- **Phase**: W2-1
- **Done**:
  - `PrototypeRoom` 씬 HUD에 마타이오스 전신 portrait sprite reference와 1080x1920 CanvasScaler 기준 적용
  - CombatGate 선택 후 실제 demo scene path에서는 AutoResolve 대신 interactive combat panel 표시
  - Attack/Defend 버튼을 `ResolveCombatRoundInteractive` 경로에 연결하고 Skill은 resolver 전까지 disabled 처리
  - 전투 중/종료 HUD에 player HP, enemy HP/id, round, last result, victory/defeat, post-combat reward deltas, demo.complete 표시
  - PlayMode smoke 확장: portrait load, combat panel, attack damage, defend round, combat 종료 후 DemoComplete 확인
- **Files**: 변경/추가 9개 (주요: `PrototypeHud.cs`, `PrototypeRunState.cs`, `PrototypeRoom.unity`)
- **GDD impact**: 없음 (기존 W2-1 demo 연결; 새 결정 없음)
- **Blockers**: 없음. 금지어 검색은 `Docs/Project` 기존 정책/진행 문서의 문구만 히트. `.obsidian/workspace.json` 외부 변경은 커밋 제외.
- **Validation**: `git diff --check` 통과, fresh EditMode 60/60, fresh PlayMode 6/6
- **Next**: Skill placeholder를 실제 ability resolver/selection UI로 승격하고 enemy reward fields와 post-combat effects 중복 정책 확정
- **Agent**: Codex

### 2026-05-06 14:08 — 전투 루프 W2-1 데이터/테스트 보강
- **Phase**: W2-1
- **Done**:
  - `ItemData`/전투 modifier/round result/enemy reward 필드와 콤보 공격 경로를 구현 명세서 기준으로 보강
  - EnemyPattern 3종, 신규 Enemy 7종, Normal Item 12종, Relic Item 6종 SO placeholder 자산 추가
  - PrototypeRoom demo combat handoff가 기존 smoke 기대대로 자동 완료되도록 run bootstrap 보정
  - EditMode 회귀 테스트 추가: combo damage, item passive modifier 집계, 필수 SO 파일 존재 확인
  - fresh EditMode 60/60, fresh PlayMode 5/5 통과 확인
- **Files**: 변경/추가 68개 (주요: `Assets/_Project/Scripts/**`, `Assets/_Project/Data/Enemies/**`, `Assets/_Project/Data/Items/**`)
- **GDD impact**: 없음 (기존 W2-1 전투 루프 명세 구현; 새 결정 없음)
- **Blockers**: 금지어 검색은 새 코드가 아니라 `Docs/Project` 정책/진행 문서의 기존 문구만 히트
- **Next**: 전투 보상 적용/아이템 패시브 실제 발동 resolver를 demo encounter reward flow에 연결
- **Agent**: Codex

### 2026-05-05 22:00 — 전투 루프 C# 구현 전체 완료 확인 (Task 1~5 기구현 검증)
- **Phase**: W2-1
- **Done**:
  - Task 1~5 코드 실제 파일 직접 확인 → 이전 세션에서 이미 전부 구현 완료 상태임을 검증
    - `ItemData.cs` — `ItemTier` enum, `tier`, `passiveTrigger`, `numericParams` 존재 확인
    - `CombatAbilityModifiers.cs` — 7개 modifier 필드 + `From()` items 파라미터 존재 확인
    - `CombatRoundResult.cs` — `ComboDamage` 필드 존재 확인
    - `CombatController.cs` — `secondAction` 파라미터 + ATK×0.7 콤보 처리 존재 확인
    - `EnemyData.cs` — `goldReward` / `xpReward` / `glitchDelta` / `affinityDelta` 존재 확인
  - `EnemyPatternData.cs` 구조 확인 — id / situationText / choiceA / choiceB (damageMultiplier, directDamage)
  - Data 디렉토리 구조 확인 — EnemyPatterns/ Items/ Enemies/ 등 폴더 이미 존재
- **Files**: 프로젝트 파일 변경 없음 (읽기 전용 검증)
- **GDD impact**: 없음
- **Next**: SO 자산 생성 (Task 6) — EnemyPattern 3종, 적 신규 7종, 아이템 18종. 단, 작가 확인 필요 (수치·텍스트는 작가 권한).
- **Agent**: OpenCode (Claude)

### 2026-05-05 21:00 — 전투 루프 완성 구현 명세서 작성 + 전체 잔여 작업 목록 정리
- **Phase**: W2-1
- **Done**:
  - 현재 코드베이스 전체 파악 — `.cs` 스크립트 80개, SO 자산 전체 목록 확인 및 GDD 명세 대비 갭 분석
  - 구현 명세서 작성 (Codex 전달용): Task 1~7 포함
    - Task 1: `ItemData.cs` 확장 — `ItemTier` enum + `tier` / `passiveTrigger` / `numericParams` 필드
    - Task 2: `CombatAbilityModifiers.cs` 확장 — 아이템 패시브 수치 통합 (7개 modifier 필드)
    - Task 3: `CombatRoundResult.cs` 확장 — `ComboDamage` 필드 추가
    - Task 4: `CombatController.cs` 확장 — `secondAction` 복합행동 파라미터 (D-023 TRAIT_OFFENSE_04)
    - Task 5: `EnemyData.cs` 확장 — `goldReward` / `xpReward` / `glitchDelta` / `affinityDelta` 보상 필드
    - Task 6: SO 자산 생성 명세 — EnemyPattern 3종 / 적 신규 7종 / 아이템 18종 (YAML 포맷 + GUID 포함)
    - Task 7: 컴파일 확인 체크리스트
  - 마감(05-18)까지 전체 잔여 작업 목록 정리 (W2-1 ~ W3-2, 블로커 포함)
- **Files**: 프로젝트 파일 변경 없음 (명세서는 채팅 출력, 파일 저장 없음)
- **GDD impact**: 없음
- **Blockers**:
  - OQ-004 (on-device LLM 수용성) — 결정 전까지 NPC LLM 연결 전체 보류
  - OQ-006 (메모리 파편 작가 퇴고) — W2-2 bake 전 완료 필요
- **Next**: Codex에 구현 명세서 전달 → 전투 루프 구현 진행. 병행으로 OQ-004 / OQ-006 작가 결정.
- **Agent**: OpenCode (Claude)

### 2026-05-05 18:30 — OQ-002/003/006/010 처리, D-024 신규, GDD v0.6.1
- **Phase**: W2-1
- **Done**:
  - OQ-002 close: 마타이오스 외형 = 10대 여성, 일러스트 보유 (외주 동생). §5.1 갱신.
  - OQ-003 close: 엔딩 = 단순 컷씬 2종(안식/동행). 후속 메타 영향은 미래 고려.
  - OQ-006 임시: 메모리 파편 5개 구조 확정(오브젝트+서사 텍스트, S2~S4 순차). `design/memory-fragments.md` 임시 초안 작성. 작가 퇴고 예정.
  - OQ-010 close: `design/sound-brief.md` 작성 — BGM 8종·전투 SFX 14종·UI SFX 10종·NPC SFX 4종·환경음 4종 리스트업. 우선순위 분류 포함.
  - D-024 신규 잠금: 플레이어 발화 기억 시스템 (휴식/이벤트 텍스트 필드 → SQLite 저장 → LLM 프롬프트 주입 → S2~S4 단계별 인용 방식).
  - GDD §8.4 SQLite 스키마 `player_utterances` 테이블 추가.
  - GDD v0.6.0 → v0.6.1 bump.
- **Files**: 변경 1개 (`hwiglija-tower-gdd.md` v0.6.1), 추가 2개 (`design/memory-fragments.md`, `design/sound-brief.md`)
- **GDD impact**: D-024 신규 / OQ-002·003·010 close / OQ-006 임시
- **Next**: OQ-004 (on-device LLM 수용성 결정) 최우선. 네이티브 플러그인 빌드 or 클라우드 fallback 전환 결정 후 W2-1 LLM 통합 구현 진행.
- **Agent**: OpenCode (Claude)

### 2026-05-05 17:00 — OQ-015/016 확정, GDD v0.6.0 갱신, design/items.md 작성
- **Phase**: W2-1
- **Done**:
  - OQ-015 확정: 복합 행동(TRAIT_OFFENSE_04) = 1번 선택→처리→2번 선택, 2번째=추가공격(ATK×0.7) 또는 아이템 사용만 허용, 방어/스킬 불가. D-023 신규 잠금.
  - OQ-016 확정: 아이템 시스템 = 유물 6개 + 일반 아이템 12개, 전부 패시브 보유, 소모품 없음. 유물/일반 tier 이분화(시스템 코드 동일).
  - GDD v0.5.0 → v0.6.0 bump. D-023 신규, D-009 수량 갱신(유물 6/일반 아이템 12 추가), §7.7 신규 섹션, OQ-015/016 closed.
  - `design/items.md` 신규 작성 — 유물 6 + 일반 12 전체 NumericParam 키 명세(🟡 임시).
- **Files**: 변경 1개 (`hwiglija-tower-gdd.md` v0.6.0), 추가 1개 (`design/items.md`)
- **GDD impact**: D-023 신규 / OQ-015·016 close / D-009 갱신
- **Next**: 남은 open OQ — OQ-002(NPC 외형), OQ-003(엔딩 메커니즘), OQ-006(메모리 파편 텍스트). 또는 Codex에 `ItemData.cs` + `design/items.md` 기반 구현 의뢰 가능.
- **Agent**: OpenCode (Claude)

### 2026-05-05 — GDD v0.5.0 기반 코드 구조 갱신
- **Phase**: W2-1
- **Done**:
  - `CombatAction` enum: `Prepare` → `Defend` + `Skill` 추가 (per GDD D-022)
  - `CombatController.ResolveRound`: Attack/Skill = 공격, Defend = 피해 절반
  - `AbilityData`: `costGold` 필드 추가, 기본값 20 (per OQ-013)
  - `EnemyPatternData.cs` 신규 — 상황 텍스트 + 2택(EnemyChoice) 구조 (per D-022/OQ-014)
  - `EnemyData`: `EnemyPatternData pattern` 직접 참조 필드 추가
  - `TraitData.cs` 신규 — 태그/UnlockWinCount/NumericParams (per OQ-011)
  - `NodeKind` enum: `Encounter`/`Boss` 추가, `Shop`=상인(보스 직전 고정) 주석 (per OQ-012)
  - `GameFlowEventType`: `CombatRoundResolved`/`TraitUnlocked`/`ShopRerolled`/`PlayerDefeated`/`EnemyDefeated` 추가
- **Files**: 변경 5개, 추가 2개 (`EnemyPatternData.cs`, `TraitData.cs`)
- **GDD impact**: 없음 (코드가 GDD에 수렴)
- **Next**: `CombatRoundResult`에 `SkillTriggered` 플래그 추가 검토. 상점/맵 시스템 ScriptableObject 구조 구현
- **Agent**: OpenCode (Claude)

### 2026-05-05 — OQ-012/013/014 확정, GDD v0.5.0 갱신
- **Phase**: W2-1
- **Done**:
  - OQ-012 확정: 슬더스식 3경로 / 선택지 팝업(맵 화면 없음) / 시드 랜덤 / 상인→보스 고정. GDD §7.0 신규 섹션 추가, `design/balance.md` 노드맵 섹션 추가
  - OQ-013 확정: 상점 3개/런, 기본 20G·3번째 30G, 리롤 10G→+5G 누적. `design/balance.md` 상점 섹션 추가, `design/abilities.md` cost_gold 임시→확정
  - OQ-014 확정: 적 턴 = 상황 텍스트 1개 + 2택 1, 몬스터별 고유 기믹(피어&헝거·서울2033 참조). `design/balance.md` 적 턴 패턴 섹션 추가
  - D-022 잠긴 결정 갱신: 적 턴 3택→2택 (작가 확인 후 진행)
  - GDD v0.4.1→v0.5.0 bump
- **Files**: 변경 3개 (`hwiglija-tower-gdd.md` v0.5.0, `design/balance.md`, `design/abilities.md`)
- **GDD impact**: D-022 갱신 / OQ-012·013·014 close
- **Next**: OQ-015(복합 행동 전투 구조) → OQ-016(유물 목록) 순서로 진행. 또는 Codex에 현재 design/ 문서 기반 코드 구현 의뢰 가능
- **Agent**: OpenCode (Claude)

### 2026-05-05 — OQ-011 특성 12종 확정 및 design/traits.md 작성
- **Phase**: W2-1
- **Done**:
  - OQ-011 작가와 질의응답으로 특성 시스템 전체 결정: 메타 영구 해금 / 생존·공격·지원 3태그×4개=12종 / 승리 횟수 3단계(3·6·10회)
  - `design/traits.md` 작성 — 12종 NumericParam 키/값 전체 명세, 🟡(임시) 키 분리
  - GDD v0.4.0 갱신: frontmatter 버전·날짜, CHANGELOG, D-009 수량(특성 12 확정), OQ-011 close, OQ-015/016 신규 등록, §7.1 특성 섹션 전면 교체
  - 신규 OQ 2건 발굴: OQ-015(복합 행동 전투 구조 확장), OQ-016(유물 아이템 목록)
- **Files**: 추가 1개 (`design/traits.md`), 변경 1개 (`hwiglija-tower-gdd.md` v0.4.0)
- **GDD impact**: D-009 갱신 / OQ-011 close / OQ-015·016 신규
- **Next**: OQ-012(노드 맵), OQ-013(상점 Pool), OQ-014(적 턴 선택지) 순서로 작가 결정 진행
- **Agent**: OpenCode (Claude)

### 2026-05-05 — design/ 구현 참조 문서 3종 작성 (Codex 코드 구조 연동 준비)
- **Phase**: W2-1
- **Done**:
  - `design/abilities.md` — 능력 12종 NumericParam 키/값 전체 명세. ✅ 확정 키(`player.attack_bonus`, `player.max_hp_bonus`)와 🟡 (임시) 키 명확히 분리
  - `design/synergies.md` — 시너지 4종 NumericParam 키/값 + 심화 로직 조건(Glitch/Affinity/방어 횟수/HP 비율) 명세
  - `design/balance.md` — 플레이어 기초 스탯, 적 7종 수치 테이블, Gold 경제 역산 검증, 기존 SO 자산 3종 GDD ID 매핑 현황
  - 신규 키 전부 🟡 (임시) 표기 — Codex가 조건부 전투 시스템 구현 시 키 네이밍 확정 후 갱신하도록 명시
- **Files**: 추가 3개 (`design/abilities.md`, `design/synergies.md`, `design/balance.md`)
- **GDD impact**: 없음 (SSOT는 GDD 0.3.0 이미 반영 완료)
- **Next**: OQ-011 특성 8~12종 상세 설계 (작가 결정 필요) → 확정 후 `design/traits.md` 추가. OQ-014 적 턴 선택지 확정 후 `balance.md` patternId 갱신
- **Agent**: OpenCode (Claude)

### 2026-05-05 01:12 — external AI model intake 폴더/manifest 보강
- **Phase**: W2-1
- **Done**:
  - `/goals` 재검증 기준으로 `Proto` preflight 수행 (`git fetch`, `git switch Proto`; rebase pull은 unstaged user changes로 중단)
  - `mataios-demo-sft-v0.1` model intake manifest, example config, tokenizer/compiled/eval placeholder 추가
  - `_training/datasets`, `_training/adapters`, `_training/evals` placeholder tracking 보강
  - model intake folder existence EditMode test 추가
  - `git diff --check`, forbidden search, fresh EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 17개 (주요: `Assets/_Project/Models/mataios-demo-sft-v0.1/**`, `Assets/_Project/Models/_training/**`, `Assets/_Project/Tests/EditMode/RuntimeShellTests.cs`)
- **GDD impact**: 없음
- **Blockers**: `git pull --rebase origin Proto` 는 기존 unstaged user changes 때문에 실행 불가. `origin/Proto` 는 local HEAD 대비 behind 0 / ahead 1 상태.
- **Next**: 외부 모델 산출물 수령 후 `model_config.example.json` 값을 실제 checksum/runtime path로 복사해 `LLMRuntimeSettings` SO와 Android plugin smoke 연결
- **Agent**: Codex

---

### 2026-05-04 17:28 — external AI model runtime intake 준비
- **Phase**: W1-2
- **Done**:
  - `LLMRuntimeSettings` / `LLMRuntimeConfig` / provider mode 기반 Fake-OnDevice-CachedOnDevice 선택 구조 추가
  - LLM 전용 `model_config.json` loader 와 safe provider factory 추가
  - MLC bridge에 model/tokenizer path 초기화 지점 추가 및 native plugin 부재 시 deterministic fake fallback 유지
  - `Assets/StreamingAssets/LLM/mataios-demo-sft-v0.1/` intake placeholder 와 대용량 모델 commit 금지 README 추가
  - EditMode model intake tests 추가 및 fresh EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 22개 (주요: `Assets/_Project/Scripts/LLM/**`, `Assets/StreamingAssets/LLM/**`, `Assets/_Project/Tests/EditMode/RuntimeShellTests.cs`)
- **GDD impact**: 없음
- **Next**: 외부 SFT 산출물 도착 후 `LLMRuntimeSettings` SO asset 생성 및 Android native plugin path smoke
- **Agent**: Codex

---

### 2026-05-04 16:04 — demo route UI 바인딩 보강
- **Phase**: W1-2
- **Done**:
  - review/juho-demo-ui 브랜치에서 DemoRunPath route indicator 실패 재현
  - scene 배치 `Prototype HUD`가 `PrototypeSceneRuntimeBuilder` 경로를 타지 않아 route labels 를 받지 못하는 원인 확인
  - `PrototypeRoomController`가 active HUD에 RoomDefinition DemoRunPath 를 주입하도록 단일 바인딩 경로 보강
  - `git diff --check`, forbidden search, fresh EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 1개 (`Assets/_Project/Scripts/Run/PrototypeRoomController.cs`)
- **GDD impact**: 없음
- **Next**: `Juho/Codex` UI polish merge 진행 후 Android build smoke 준비
- **Agent**: Codex

---

### 2026-05-04 10:59 — deterministic demo run progression 고정
- **Phase**: W1-2
- **Done**:
  - `SO_Room_Prototype` 에 Shop → MoralChoice → MemoryFragment → CombatGate demo path를 SO 참조로 고정
  - `PrototypeRunState` 에 demo progression cursor/completion 상태 추가 및 선택지 적용 성공 시에만 진행
  - `PrototypeRoomController` 를 demo encounter selection 단일 진입점으로 정리하고 HUD에 next/demo.complete 상태 표시
  - resolved encounter 재방문 시 effect/progression 중복 없이 `already resolved: <choiceStableId>` 유지
  - EditMode demo progression tests 및 PlayMode 1-run smoke 확장
- **Files**: 변경/추가 12개 (주요: Assets/_Project/Scripts/Run/**, Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset, Assets/_Project/Tests/**)
- **GDD impact**: 없음
- **Blockers**: 없음. Unity Test Runner는 `-quit` 동시 지정 시 테스트 전 종료되어, fresh 검증은 `-runTests` 자체 종료 방식으로 실행.
- **Validation**: `git diff --check` 통과, forbidden search 통과, fresh EditMode 52/52 pass, fresh PlayMode 5/5 pass
- **Next**: Android build 검증 전, demo complete 이후 임시 결과 UX와 combat 후 HP/defeat edge를 한 번 더 얇게 점검
- **Agent**: Codex

---

### 2026-05-04 10:58 — Proto ahead 12 GitHub push
- **Phase**: W2-1
- **Done**:
  - local `Proto...origin/Proto [ahead 12]` 상태 확인
  - dirty working tree/untracked 파일은 그대로 둔 채 커밋된 12개만 push
  - `origin/Proto` 를 `6f34b40..c7af07e` 로 업데이트
- **Files**: repo 파일 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: 남은 unstaged 작업 범위 확인 후 다음 보존 커밋 또는 hygiene 정리
- **Agent**: Codex

---

### 2026-05-04 10:33 — encounter choice UX/revisit 정책 검증 커밋
- **Phase**: W2-1
- **Done**:
  - `71a7c8a` 위 추가 7개 변경 범위를 확인하고 unrelated untracked 파일은 제외
  - `PrototypeHud` result message 영역, 모바일 터치 버튼 레이아웃, disabled best-fit 개선 변경 검증
  - `PrototypeRunState` nodeId+encounterId resolved choice 기록 및 재방문 effect 재적용 차단 검증
  - fresh Unity EditMode 49/49, PlayMode 5/5, `git diff --check`, forbidden search 통과
  - `c7af07e` 커밋으로 UX/result/revisit 정책 변경 보존
- **Files**: 커밋 7개 (PrototypeHud.cs, PrototypeRunState.cs, PrototypeRoomController.cs, NodeInteractionController.cs, PrototypeSceneRuntimeBuilder.cs, RuntimeShellTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음 (최종 텍스트/서사 결정 없음; P4 결정성 유지)
- **Blockers**: unrelated untracked 파일 다수는 hygiene 후보로 남김
- **Next**: D1-D2 데모 플레이 path 고정: shop/moral/memory/combat gate 포함 1-run progression 지시서 작성 및 개발 세션 배분
- **Agent**: Codex

---

### 2026-05-03 23:53 — baked encounter choice button UI 연결
- **Phase**: W1-2
- **Done**:
  - `PrototypeHud` 에 runtime choice button 생성/정리, DisabledVisible reason 표시, InputSystem UI event path 추가
  - `PrototypeRoomController`/`NodeInteractionController` 가 baked choices 를 auto-resolve 하지 않고 버튼 선택 후 `ResolveEncounterChoice` 로 commit 하도록 변경
  - `PrototypeEncounterChoiceView` 에 `textKey` 전달 추가: 버튼에는 placeholder key 또는 stableId 표시
  - Prototype shop/battle node fixture 에 smoke 대상 baked encounter 참조 연결
  - PlayMode smoke 추가: shop insufficient/sufficient, moral affinity, memory unlock hidden policy, combat handoff button flow 검증
- **Files**: 변경 7개 (PrototypeHud.cs, PrototypeRoomController.cs, NodeInteractionController.cs, PrototypeRoomSmokeTests.cs, Prototype Node assets)
- **GDD impact**: 없음 (최종 NPC/서사/도덕 텍스트 미작성, placeholder key 만 표시)
- **Blockers**: 없음
- **Next**: 선택지 버튼 레이아웃/터치 UX 폴리싱 후 encounter 결과 메시지 패널과 node 재방문 정책 정리
- **Agent**: Codex

---

### 2026-05-03 23:44 — v0.3 encounter catalog/bake/bootstrap commit 보존
- **Phase**: W1-2
- **Done**:
  - 요청 scope 파일만 staging 후 `5c0da93` 커밋 생성
  - v0.3 encounter runtime catalog, full 25 encounter bake assets, item/reward/enemy/memory fragment/catalog data 포함
  - encounter catalog builder/reference validator, bootstrap scene/controller path, EditMode/PlayMode smoke tests 포함
  - unrelated local/Obsidian/NPC training/result XML 파일은 stage 제외
  - `git diff --check` 및 staged scope 검토 완료
- **Files**: 커밋 118개 (주요: Assets/_Project/Scripts/Encounters/**, Assets/_Project/Data/{Encounters,Catalogs,Items,Rewards,Enemies,MemoryFragments}/**, Docs/ExternalSpecs/EncounterPipeline/v0.3/**)
- **GDD impact**: 없음
- **Blockers**: 로컬 test result XML 은 stale(33/33, 1/1)로 남아 있어 사용자 제공 최신 검증 결과(48/48, 4/4)를 기준으로 커밋
- **Next**: unrelated untracked 정리 또는 `.gitignore` 반영 승인 후 repo hygiene follow-up
- **Agent**: Codex

---

### 2026-05-03 23:40 — catalog/bootstrap 완료 확인 + 선택지 UI 후속 지시 정리
- **Phase**: W1-2
- **Done**:
  - 개발 세션 보고 확인: EditMode 48/48, PlayMode 4/4, scene/controller catalog attach smoke 통과
  - `SO_EncounterRuntimeCatalog` scene/bootstrap 주입이 완료된 상태로 판단
  - 현재 git 상태 확인: catalog/bake/bootstrap 관련 변경이 아직 working tree 에 남아 있어 보존 커밋 우선 필요
  - 다음 개발 범위를 보존 커밋과 baked encounter 선택지 버튼 UI 연결로 분리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: 현재 변경 묶음을 stage/commit 후, full pack encounter choice UI 버튼 흐름 구현
- **Agent**: Codex

---

### 2026-05-03 23:37 — encounter runtime catalog scene/bootstrap 주입
- **Phase**: W1-2
- **Done**:
  - `PrototypeRoomController` 에 `EncounterRuntimeCatalogData` serialized reference 와 read-only accessor 추가
  - `BeginRun()` 이 새 `PrototypeRunState` 생성 직후 catalog 를 자동 attach 하도록 연결
  - `PrototypeSceneRuntimeBuilder` 의 generated scene path 도 catalog injection 을 전달하도록 확장
  - `PrototypeRoom.unity` 의 static controller 에 `SO_EncounterRuntimeCatalog` 직접 참조 바인딩
  - PlayMode smoke 추가: scene controller path 로 catalog attach 확인 후 `ITEM_FIELD_BANDAGE` purchase effect resolved
- **Files**: 변경 4개 (PrototypeRoomController.cs, PrototypeSceneRuntimeBuilder.cs, PrototypeRoom.unity, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음
- **Blockers**: 이전 stableId catalog integration 변경 묶음이 아직 커밋되지 않은 상태
- **Next**: 현재 catalog/bake/bootstrap 변경 묶음을 stage/commit 한 뒤 full pack node selection UI 버튼화로 진행
- **Agent**: Codex

---

### 2026-05-03 23:32 — 개발 세션 stableId catalog integration 후속 범위 정리
- **Phase**: W1-2
- **Done**:
  - 개발 세션 결과 확인: EditMode 48/48, PlayMode 3/3, full pack dry-run 25 encounters reports=0
  - `EncounterRuntimeCatalogData` 와 full pack catalog/memory integration 이 완료된 상태로 판단
  - 현재 남은 병목을 scene/bootstrap catalog 주입과 full pack runtime selection smoke 로 분리
  - `PrototypeRoomController.BeginRun()` 이 catalog 를 자동 attach 하지 않는 점과 scene/runtime builder 혼재 상태를 확인
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: 개발 세션에 SO_EncounterRuntimeCatalog scene/bootstrap 주입 및 full pack node smoke 확대 지시
- **Agent**: Codex

---

### 2026-05-03 23:29 — v0.3 stableId catalog + memory fragment runtime integration
- **Phase**: W1-2
- **Done**:
  - v0.3 full pack non-memory stableId catalog lookup 유지 및 item/reward/ability/enemy placeholder SO 검증
  - MemoryFragmentData SO와 catalog lookup 추가: MEM_FRAGMENT_01..05 titleKey/bodyKey/stage placeholder 생성
  - baker가 MemoryFragmentLocked/UnlockMemoryFragment stableId를 runtime sub-data로 직렬화하도록 확장
  - RunState/Resolver가 memory fragment unlock을 deterministic stableId set으로 기록하고 Hidden/DisabledVisible 흐름 유지
  - full pack dry-run/bake, missing catalog refs, duplicate stableId, EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 다수 (주요: Assets/_Project/Scripts/Encounters/**, Assets/_Project/Scripts/Run/**, Assets/_Project/Data/**, Docs/ExternalSpecs/EncounterPipeline/v0.3/**)
- **GDD impact**: 없음 (OQ-006 최종 텍스트 미작성, placeholder key만 추가)
- **Blockers**: 없음
- **Next**: scene/bootstrap에서 SO_EncounterRuntimeCatalog 주입 경로 고정 후 full pack encounter selection smoke 확대
- **Agent**: Codex

---

### 2026-05-03 15:40 — memory fragment catalog v0.1 재검수
- **Phase**: W1-2
- **Done**:
  - 외주 `memory_fragment_catalog_v0.1.md` 확인
  - `MEM_FRAGMENT_01..05` 5개가 full pack 참조와 1:1 일치함을 검증
  - title/body/writer note 가 모두 `PLACEHOLDER_*` key 로 유지되고 최종 서사 텍스트가 없음을 확인
  - stage 값은 v0.2 외주 stage 명칭이며 runtime `NpcStage.S1..S5` 매핑 대상으로 판단
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (OQ-006 open 유지; 작가 최종 텍스트 작성 전 placeholder catalog 만 수용)
- **Next**: 개발 세션에서 stableId catalog integration 작업에 memory fragment placeholder tracking 을 포함
- **Agent**: Codex

---

### 2026-05-03 15:10 — encounter runtime resolver + choice/effect flow 연결
- **Phase**: W1-2
- **Done**:
  - rebase 완료 상태 확인: `Proto` 정상 브랜치, `hwigija-tower` gitlink 없음, `hwigija-tower.slnx` 미수정
  - `EncounterData` 에 choices/requirements/effects/unavailablePolicy/combatHandoff runtime sub-data 직렬화 추가
  - `PrototypeEncounterRuntimeResolver` 추가: StatAtLeast/HasAbility, Hidden/DisabledVisible, ModifyGold/Mental/Glitch/Affinity/Hp, AddItem/AddAbility/GrantRewardBundle, StartCombat 처리
  - Items/Rewards ScriptableObject 모델 추가 및 baked sample encounter 3개에 runtime sub-data bake
  - baker dry-run/sourceHash/stableId conflict report 와 trailing whitespace cleanup 추가
  - EditMode 39/39, PlayMode smoke 3/3, diff whitespace/금지 runtime escape/non-determinism 검색 통과
- **Files**: 변경/추가 18개 (주요: EncounterData.cs, EncounterPipelineV02Baker.cs, PrototypeEncounterRuntimeResolver.cs, PrototypeRunState.cs, ItemData.cs, RewardBundleData.cs, RuntimeShellTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음 (D-016 정합 유지; schema v0.2/escape/NPC 최종 텍스트 변경 없음)
- **Blockers**: 실제 item/reward/ability 카탈로그 lookup 과 선택지 UI 버튼화는 아직 최소 구현 밖
- **Next**: 선택지 UI를 실제 버튼 입력으로 분리하고, stableId catalog lookup 으로 Item/Reward/Ability SO 지급을 완성
- **Agent**: Codex

---

### 2026-05-03 15:07 — 외주 full encounter pack v0.3 검수
- **Phase**: W1-2
- **Done**:
  - `pack_FULL_25_V003.json` JSON 문법 및 count 검증: 25개, GDD 분류 8/5/7/3/2 일치
  - escape/direct text/old NpcStage 금지 토큰 없음 확인
  - choice/effect semantic check 통과: MoralChoice, Shop, StartCombat 기본 규칙 충족
  - pack 참조 stableId 추출: item 3개, ability 2개, reward 2개, enemy 3개, memory fragment 5개
  - 현재 Unity placeholder SO id 와 외주 stableId 불일치 및 item/reward catalog 상세 필드 부족 확인
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (콘텐츠 skeleton 검수; 최종 텍스트/밸런스 결정 아님)
- **Blockers**: item/reward catalog 가 요구 필드(용도, SO 타입 후보, display key)를 충분히 제공하지 않음. Unity SO stableId 매핑 필요.
- **Next**: 외주에는 catalog v0.2 보강 요청, Unity 세션에는 stableId 기반 Items/Rewards/Ability/Enemy lookup 및 full pack dry-run/bake 지시
- **Agent**: Codex

---

### 2026-05-03 14:59 — rebase 완료 + encounter choice/effect runtime resolver 구현
- **Phase**: W1-2
- **Done**:
  - `Proto` rebase 완료: `.gitignore`/`.gitattributes` conflict marker 제거 및 local/origin union 반영
  - add/add 충돌은 로컬 작업 커밋 내용을 보존해 정리하고 `origin/Proto` 위로 8 commits 재적용
  - `EncounterData` 에 runtime choice/requirement/effect payload 추가 및 v0.2 baker 연결
  - `PrototypeEncounterRuntimeResolver` 추가: Stat/Flag/Item/Ability requirement, HP/mental/gold/glitch/affinity/flag/item/ability-ref effects 처리
  - `PrototypeRoomController` 가 formal non-battle encounter choice를 runtime resolver로 연결
- **Files**: 변경/추가 8개 (주요: EncounterData.cs, EncounterPipelineV02Baker.cs, PrototypeEncounterRuntimeResolver.cs, RuntimeShellTests.cs)
- **GDD impact**: 없음 (D-006/P4 정합; 새 디자인 결정 없음)
- **Blockers**: 실제 `AbilityData`/`ItemData` catalog ref 해석은 아직 placeholder ref tracking 수준
- **Next**: Items/Rewards SO 모델 추가 후 `AddItem`/`AddAbility` 를 catalog lookup 기반 실제 보상 지급으로 승격
- **Agent**: Codex

---

### 2026-05-03 14:49 — repo hygiene 상태 점검 및 rebase 충돌 확인
- **Phase**: W1-2
- **Done**:
  - `Proto` rebase 진행 상태 확인: `origin/Proto` onto rebase 중 1/8 커밋 적용 후 중단
  - 원래 `Proto` HEAD `717996a` 를 `backup-proto-before-rebase-20260503-717996a` 브랜치로 백업
  - 충돌 파일 `.gitattributes`, `.gitignore` 와 `.gitattributes` conflict marker 로 인한 attribute parse 오류 확인
  - untracked 파일을 ignore/검토/repo 외부 유지/테스트 산출물 후보로 분류
  - 삭제 및 `.gitignore` 수정은 수행하지 않음
- **Files**: 프로젝트 파일 변경 없음 (git backup branch 추가, PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Blockers**: rebase 충돌 미해결 상태라 추가 `git pull --rebase origin Proto` 는 실행하지 않음
- **Next**: `.gitattributes`/`.gitignore` 충돌 해결 후 `git rebase --continue`; 별도 승인 시 `.gitignore` 에 Obsidian/macOS/test-result 후보 반영
- **Agent**: Codex

---

### 2026-05-03 14:48 — Proto rebase sync 충돌로 runtime 작업 보류
- **Phase**: W1-2
- **Done**:
  - GDD §0/§0.6/§1/§2/§3 와 최근 PROGRESS 확인
  - 계획에 따라 `git pull --rebase origin Proto` 시도
  - 첫 rebase commit `3c9cd59` 적용 중 `.gitattributes`, `.gitignore` add/add 충돌 확인
  - 계획의 충돌 임의 해결 금지 조건에 따라 runtime resolver 구현 착수 전 중단
- **Files**: 프로젝트 코드 변경 없음 (rebase conflict state: `.gitattributes`, `.gitignore`)
- **GDD impact**: 없음
- **Blockers**: rebase conflict 해결 전략 필요. 현재 repo 는 rebase 진행 중 `HEAD (no branch)` 상태.
- **Next**: 사용자 승인 후 충돌을 union/정책 기반으로 해결하고 rebase continue, 또는 rebase abort 후 별도 sync 전략 결정
- **Agent**: Codex

---

### 2026-05-03 14:38 — encounter pipeline v0.2 validator + baker 기반 구현
- **Phase**: W1-2
- **Done**:
  - v0.2 DTO 구조와 외주 NpcStage enum -> runtime `NpcStage.S0..S5` mapping layer 추가
  - Editor-only JSON syntax/semantic validator 추가: schemaVersion, assetBake, assetPath, choice/time rules, escape/direct text 금지, MoralChoice/Shop custom rules
  - targetSchema/path 기반 validation case runner 추가 및 외주 validation cases 통과
  - minimal editor baker 추가: stableId 우선/updateExisting/no delete-recreate 로 sample pack 3개를 정식 `Assets/_Project/Data/Encounters` SO로 bake
  - EditMode 33/33, PlayMode smoke 1/1 통과
- **Files**: 변경/추가 22개 (주요: Assets/_Project/Scripts/Encounters/EncounterPipelineV02*, Assets/_Project/Tests/EditMode/EncounterPipelineV02Tests.cs, Assets/_Project/Data/Encounters/SO_Encounter_ENC_*.asset)
- **GDD impact**: 없음 (GDD locked 결정/NpcStage enum 변경 없음; v0.2 구현 기반)
- **Blockers**: 실제 런타임 choice/effect 실행 및 reward/item SO 모델은 미구현
- **Next**: Choice/Effect runtime resolver 와 Items/Rewards SO 모델 연결
- **Agent**: Codex

---

### 2026-05-03 13:54 — 외주 encounter schema v0.2 검수
- **Phase**: W1-2
- **Done**:
  - v0.2 schema/catalog/contract/sample/validation 파일 5개 확인
  - JSON 문법 검증 통과 및 `pack_PACK_SAMPLE_3_V002.json` 의 JSON Schema 검증 통과 확인
  - 도주 제거, 상태값 4종, 상점 샘플, GDD 정합 NpcStage, 정식 baked asset path 반영 확인
  - validation cases 의 `targetSchema` 일부 불일치와 custom semantic rule 필요성 확인
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (D-022 후보 구현 계약 검수; 아직 SSOT 잠금 전)
- **Blockers**: validation case 일부는 raw JSON Schema 단독 실행 불가. baker/validator 구현 시 target/path 기반 custom runner 필요.
- **Next**: Unity 개발 세션에 v0.2 schema 기반 DTO/validator/editor baker 구현 지시
- **Agent**: Codex

---

### 2026-05-03 13:30 — RunState economy/status hooks + shop minimum flow
- **Phase**: W1-2
- **Done**:
  - `PrototypeRunState` 에 mental/gold/glitchLevel/affinity 상태값과 clamp modify 메서드 추가
  - HUD snapshot/표시에 HP/ATK/gold/mental/glitchLevel/affinity/ability count 반영
  - Shop node 최소 흐름 구현: gold 부족 실패, gold 충분 시 10 차감 + placeholder ability 지급
  - 정식 데이터 landing zone `Assets/_Project/Data/Encounters`, `Items`, `Rewards` 추가; Prototype/Encounters 는 smoke fixture 로 유지
  - EditMode tests 4개 추가/갱신 및 PlayMode smoke 재검증
- **Files**: 변경/추가 14개 (주요: PrototypeRunState.cs, PrototypeRunSnapshot.cs, PrototypeHud.cs, RuntimeShellTests.cs, Assets/_Project/Data/{Encounters,Items,Rewards})
- **GDD impact**: 없음 (외주 schema v0.2 전 런타임 기반 구현; baker/validator/escapePolicy/NPC 최종 텍스트 미구현)
- **Blockers**: 외주 encounter schema v0.2 미수령. 실제 reward/item schema 확정 대기.
- **Next**: v0.2 schema 수령 후 정식 Encounters/Items/Rewards SO 모델과 baker/validator 설계 검토
- **Agent**: Codex

---

### 2026-05-03 13:16 — encounter schema v0.2 외주 재요청 명세 정리
- **Phase**: W1-2
- **Done**:
  - 도주 정책 제거 결정 반영: escapePolicy 비허용/Disabled 고정
  - mental/gold/glitchLevel/affinity 선택 결과 상태값 전부 구현 목표로 정리
  - 상점 구현 목표 반영: gold/item/ability/reward flow 필수화
  - baked asset 정식 경로 `Assets/_Project/Data/Encounters` 수용 기준 정리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (기존 GDD 방향 내 구현 명세 구체화; D-022 후보)
- **Next**: 외주 AI 에 v0.2 schema/catalog/contract/sample/validation 수정본 요청 후 Codex baker/validator 구현
- **Agent**: Codex

---

### 2026-05-03 13:13 — 외주 encounter schema 산출물 검토
- **Phase**: W1-2
- **Done**:
  - 외주 폴더의 schema/catalog/contract/sample/validation 파일 5개 내용 확인
  - JSON 문법과 sample pack 의 JSON Schema 검증 수행: sample pack schema 통과
  - GDD/Tone Bible 과 대조해 NpcStage 명칭/S5 누락, GDD 콘텐츠 수량 불일치, validation cases 자동화 구조 미흡 확인
  - 설계 AI 에 재요청할 수정 항목과 사용자 결정 필요 항목 정리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (산출물 검토; D-022 잠금 전 수정 필요)
- **Blockers**: NpcStage enum 정합, 25개 콘텐츠 분류, 메타 진행/영구 상태 범위 결정 필요
- **Next**: 외주 산출물 v0.2 재요청 후 baker/validator 구현 착수
- **Agent**: Codex

---

### 2026-05-03 12:43 — 텍스트 RPG 데이터 파이프라인 아키텍처 검토
- **Phase**: W1-2
- **Done**:
  - GDD §0/§0.6/§1/§2/§3 와 최근 PROGRESS 확인
  - 현재 Unity 프로젝트의 Encounter/Run/Combat placeholder 구조 점검
  - JSON schema → Editor bake → ScriptableObject runtime 파이프라인 적합성 검토
  - SerializeReference/SO/AssetDatabase/JsonUtility 리스크와 handoff payload 누락 항목 정리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (새 결정 없음; 제안 단계)
- **Blockers**: 실제 스키마/베이커 구현은 작가 승인 후 진행 필요
- **Next**: Encounter schema v0.1 과 bake validator/EditMode tests 구현 후 설계 AI 산출물을 소량 샘플로 검증
- **Agent**: Codex

---

### 2026-05-03 11:27 — AI NPC model intake + training spec 추가
- **Phase**: W1-2
- **Done**:
  - D-005 1순위/2순위 모델 배치 폴더 추가 (`hcx-seed-0.5b`, `qwen2.5-1.5b-instruct`)
  - 모델별 compiled android/ios, tokenizer, eval, training workspace 폴더와 Unity `.meta` 생성
  - MLC-LLM/SQLite third-party intake 와 Android/iOS plugin landing zone 추가
  - `Docs/AI_NPC_MODEL_SPEC.md` 작성: runtime contract, dataset JSONL schema, training scope, quality gate, conversion outline
  - `Tools/LLM/eval_prompts.jsonl` 로 최소 평가 prompt seed 추가
- **Files**: 변경/추가 42개 (주요: Assets/_Project/Models/**, Assets/ThirdParty/**, Assets/Plugins/**, Docs/AI_NPC_MODEL_SPEC.md)
- **GDD impact**: 없음 (D-005/D-006/§8/§11 정합 구현; 모델 선택 결정 변경 없음)
- **Blockers**: D-005 acceptance gate open. 실제 MLC/SQLite native plugin 미연동. Android Build Support 미설치.
- **Next**: 후보 모델 라이선스/획득 경로 확인 후 eval prompt 로 한국어 품질·지연 측정, 통과 모델만 MLC 변환
- **Agent**: Codex

---

### 2026-05-03 11:23 — asset intake folder structure 추가
- **Phase**: W1-2
- **Done**:
  - 외주/생성 에셋 수용용 `Art`, `Audio`, `Prefabs`, `Animations`, `Materials`, `VFX`, `UI` 폴더 추가
  - Art 하위 Characters/Environments/Nodes/UI/Icons/VFX, Audio 하위 Music/SFX/Ambience/Voice 폴더 추가
  - Prefabs 하위 Characters/Nodes/UI/Runtime 폴더 추가
  - 각 상위 폴더 `_README.md` 와 `Docs/ASSET_INTAKE.md` 로 import 위치/네이밍 규칙 문서화
  - Unity batchmode import 로 folder/file `.meta` 생성 확인
- **Files**: 변경/추가 40개 (주요: Assets/_Project/Art, Assets/_Project/Audio, Assets/_Project/Prefabs, Docs/ASSET_INTAKE.md)
- **GDD impact**: 없음 (GDD §11 repo 구조 정합 보강; 새 디자인 결정 없음)
- **Blockers**: 없음
- **Next**: 일러스트/사운드 파일 입수 시 해당 intake 폴더에 import 후 sprite/audio import settings 검증
- **Agent**: Codex

---

### 2026-05-03 11:16 — placeholder content pipeline + smoke tests
- **Phase**: W1-2
- **Done**:
  - `PrototypeSceneBuilder` 가 placeholder SO 49개(능력 12/시너지 4/적 6/보스 2/인카운터 25)와 MemoryFragments 폴더 생성/연결
  - `PrototypeRunState` persistent HP/ATK/ability/node snapshot 추가, HUD 상태 표시 확장
  - Battle/Rest/Shop/Remnant placeholder flow 를 scene data 에 연결
  - NPC trigger hook scaffold 추가 (`battle.victory`, `battle.defeat`, `rest.recall`, `run.completed`)
  - PlayMode smoke test 및 구현 준비 체크리스트 문서 추가
- **Files**: 변경/추가 127개 (주요: Assets/_Project/Data/Prototype/**, PrototypeSceneBuilder.cs, PrototypeRunState.cs, Docs/IMPLEMENTATION_READY_CHECKLIST.md)
- **GDD impact**: 없음 (D-009 수량에 맞춘 placeholder; 실제 수치/텍스트는 작가 결정 대기)
- **Blockers**: Android Build Support 미설치. MLC/SQLite native plugin 미연동.
- **Next**: Android Build Support 설치 후 APK build 검증, 작가 기획값을 placeholder SO에 치환
- **Agent**: Codex

---

### 2026-05-02 23:53 — hatch-pet Tower Seed base 생성
- **Phase**: W1-2
- **Done**:
  - `hatch-pet` skill 을 재실행해 `Tower Seed` pet run manifest 생성
  - base image prompt 를 생성하고 built-in image generation 으로 기준 sprite 생성
  - 생성 이미지를 `record_imagegen_result.py` 로 run manifest 에 기록해 canonical base/reference 생성
  - row-strip 8개 ready, `running-left` 는 `running-right` 이후 결정 대기 상태 확인
- **Files**: 프로젝트 변경 없음 (run: /private/tmp/hatch-pet-runs/tower-seed, generated image: /Users/godju/.codex/generated_images/019de928-6c23-7ec0-8d5d-23f5b274b810/ig_0acd04ef5560bd5c0169f60f9462088191a53e5a66e86ea93b.png)
- **GDD impact**: 없음
- **Blockers**: row-strip 생성은 hatch-pet 지침상 subagent 필수이나, 현재 상위 규칙상 사용자 명시 요청 전 subagent spawn 불가
- **Next**: 사용자가 subagent 사용을 명시 승인하면 idle/running-right row 부터 생성 후 package finalization 진행
- **Agent**: Codex

---

### 2026-05-02 23:48 — prototype run flow 연결
- **Phase**: W1-2
- **Done**:
  - `PrototypeRunState` 추가: run state, ability inventory, memory repo, cached on-device/fake LLM provider 소유
  - Battle node 선택 시 deterministic combat round 자동 처리 + CombatStarted/Completed, EncounterCompleted 이벤트 발행
  - Rest node recall 로드, Remnant node run completion + reflection 저장 연결
  - Ability/Synergy numeric hook 추가 (`player.max_hp_bonus`, `player.attack_bonus`)
  - MLC native bridge / on-device provider scaffold 추가, native plugin 없으면 fake provider fallback
  - Android build harness 가 Android target 미설치 시 조기 실패하도록 보강
- **Files**: 변경/추가 20개 (주요: Assets/_Project/Scripts/Run/PrototypeRunState.cs, LLM/MLCBridge.cs, Abilities/CombatAbilityModifiers.cs)
- **GDD impact**: 없음 (P4 결정성, D-005/D-006 scaffold 정합; 새 수치/서사 결정 없음)
- **Blockers**: Android Build Support 미설치로 APK 생성 실패. MLC/SQLite 실제 native plugin 미연동.
- **Next**: 작가 결정 전에는 placeholder enemy/ability SO 자동 생성과 HUD 세분화, 이후 능력 12/시너지 4/적 6/보스 2 데이터 입력
- **Agent**: Codex

---

### 2026-05-02 23:48 — hatch-pet Codex skill 설치
- **Phase**: W1-2
- **Done**:
  - AGENTS 규약에 따라 GDD §0/§0.6/§1/§2 와 최근 PROGRESS 확인
  - openai/skills curated `hatch-pet` skill 을 로컬 Codex skills 경로에 설치
  - 설치된 `hatch-pet/SKILL.md` 를 확인해 실제 펫 생성 workflow 와 subagent 요구사항 파악
- **Files**: 프로젝트 변경 없음 (외부 Codex skill: /Users/godju/.codex/skills/hatch-pet)
- **GDD impact**: 없음
- **Blockers**: 실제 펫 row 생성은 사용자의 pet 컨셉/이름 및 subagent 명시 승인 필요
- **Next**: Codex 재시작 후 hatch-pet skill 활성화, 만들 pet 컨셉을 정해 생성 진행
- **Agent**: Codex

---

### 2026-05-02 23:36 — Unity root baseline commit + consistency settings
- **Phase**: W1-2
- **Done**:
  - 루트 승격 변경과 runtime shell 구현을 `Normalize Unity root and runtime shell` 커밋(c165722)으로 고정
  - `.editorconfig` 추가: C#/Unity YAML/JSON indentation, LF, final newline 규칙 고정
  - README 갱신: Unity 프로젝트 루트, 표준 EditMode/Android build 명령, 추적/ignore 규칙 문서화
  - `.gitignore` 에 Unity test/build 로그 및 결과 파일 ignore 추가
  - EditMode tests 재실행: 14/14 pass
- **Files**: 변경/추가 221개 커밋 (주요: .editorconfig, README.md, Assets/_Project/**, Packages/**, ProjectSettings/**)
- **GDD impact**: 없음 (D-016 정합 유지; 새 디자인 결정 없음)
- **Blockers**: 없음
- **Next**: Combat node 를 실제 HP/HUD/승패 흐름에 연결
- **Agent**: Codex

---

### 2026-05-02 23:31 — W1-2 runtime shell + repo root 정상화
- **Phase**: W1-2
- **Done**:
  - D-016 에 맞게 Unity 프로젝트 파일을 repo 루트로 승격하고 nested gitlink 제거
  - `GameFlowEventBus` 를 PrototypeRoom / NodeInteraction 흐름에 연결
  - seed-key 기반 `EncounterSelector`, 전투 stub, Ability/Synergy runtime shell 추가
  - SQLite schema scaffold, deterministic fake/cached LLM provider, reflection/recall pipeline 추가
  - Android build harness (`BuildScript.BuildAndroid`) 추가 및 PrototypeRoom scene 재생성
- **Files**: 변경/추가 217개 (주요: Assets/_Project/Scripts/**, Assets/_Project/Tests/EditMode/RuntimeShellTests.cs, Packages/ProjectSettings 루트 승격)
- **GDD impact**: 없음 (D-016 정합화, P4 결정성 기반 구현; 새 밸런스/서사 결정 없음)
- **Blockers**: SQLite 실제 native/plugin 연동과 on-device MLC bridge 는 아직 scaffold 단계
- **Next**: Combat stub 를 실제 노드 보상/HP HUD 에 연결하고, 작가 결정 후 능력 12/시너지 4 SO 채우기
- **Agent**: Codex

---

### 2026-05-02 — D-020/D-021 잠금 + 재개 준비 (Claude)
- **Phase**: W1-2 진입 직전
- **Done**:
  - 04-30 ~ 05-02 3일 공백 사유 기록: 조사 (작가 가족사)
  - GDD v0.2.2: D-020 (외주 = 동생) + D-021 (스코프 유지, AI 처리량 기반 일정) 잠금
  - OQ-010 등록 (외주 작업 정의, 05-09 마감)
  - W1-2 슬립 인정 + W2-1 흡수 정책
- **Files**: hwiglija-tower-gdd.md, wiki/log.md, 본 파일
- **GDD impact**: D-020 / D-021 / OQ-010 / version 0.2.0 → 0.2.2
- **Next**:
  - 작가 → Gemini 토픽 2 (능력 12 효과·수치) 디벨롭 — Codex W1-2 진입 전제
  - Codex 재개 시 04-29 20:12 의 Next 흡수 (GameFlowEventBus → PrototypeRoom 연결 + Encounter stub) 후 W1-2 진입
- **Agent**: Claude (Sub brain)

---

### 2026-04-29 20:12 — 코어 아키텍처 누락분 보강
- **Phase**: W1-1
- **Done**:
  - 기존 SO 데이터 모델, LLM/memory/dialogue 인터페이스, 기본 seeded RNG 구현 상태 확인
  - `DeterministicSeed` / seed-key fork / weighted index helper 추가
  - NPC S0-S5 FSM runtime (`NpcStateMachine`) 과 SO definition / transition rule 추가
  - 게임 흐름 이벤트 primitives (`GameFlowEvent`, `GameFlowEventBus`, `GameFlowEventType`) 추가
  - RNG/FSM/event ordering EditMode tests 추가
- **Files**: 변경/추가 17개 (주요: `Assets/_Project/Scripts/Core/**`, `Assets/_Project/Scripts/NPC/NpcStateMachine*.cs`, `Assets/_Project/Tests/EditMode/CoreArchitectureTests.cs`)
- **GDD impact**: 없음 (P4 결정성 정합; 새 밸런스/서사 결정 없음)
- **Blockers**: 없음. Unity-Skills compile check 통과, EditMode tests 8/8 pass.
- **Next**: `GameFlowEventBus` 를 prototype room/node interaction 흐름에 연결하고 Encounter 선택 stub 를 seed-key 기반으로 구현
- **Agent**: Codex

---

### 2026-04-29 17:19 — Unity-Skills UPM 설치 확인
- **Phase**: W1-1
- **Done**:
  - Unity 프로젝트 `Packages/manifest.json` 에 `com.besty.unity-skills` 추가 확인
  - `Packages/packages-lock.json` 에 Git package lock 반영 확인
  - localhost `8090-8100` 확인 결과 Unity-Skills REST server 는 아직 미실행 상태 확인
  - Unity 6000.4 API 변경으로 인한 `PlayerSettings.defaultScreenOrientation` compile error 를 `defaultInterfaceOrientation` 로 수정
- **Files**: 변경/추가 3개 (주요: `Packages/manifest.json`, `Packages/packages-lock.json`, `Assets/_Project/Editor/PrototypeSceneBuilder.cs`)
- **GDD impact**: 없음
- **Blockers**: Unity-Skills server 는 Editor 메뉴에서 `Window > UnitySkills > Start Server` 실행 필요
- **Next**: Unity Editor recompile 완료 후 Start Server 실행, Codex 에서 `/health` 재확인
- **Agent**: Codex

---

### 2026-04-29 16:53 — Input System asmdef 참조 오류 수정
- **Phase**: W1-1
- **Done**:
  - `UnityEngine.InputSystem` compile error 원인 확인
  - `com.unity.inputsystem` 패키지는 설치되어 있으나 `_Project` runtime asmdef 참조가 비어 있음을 확인
  - `HwigiTower.Runtime.asmdef` 에 `Unity.InputSystem` 참조 추가
- **Files**: 변경/추가 1개 (`Assets/_Project/Scripts/HwigiTower.Runtime.asmdef`)
- **GDD impact**: 없음
- **Blockers**: Unity batchmode compile 검증은 동일 프로젝트가 Editor 에서 열려 있어 실행 불가
- **Next**: 열린 Unity Editor 에서 자동 recompile 확인 후 남은 compile error 점검
- **Agent**: Codex

---

### 2026-04-29 16:45 — Unity Licensing Client 실행 실패 진단
- **Phase**: W1-1
- **Done**:
  - macOS 디스크/메모리 상태 확인
  - Unity Editor / Licensing Client 로그 확인
  - 실행 중인 Unity Hub / Unity.Licensing.Client 프로세스 확인
  - 직접 원인이 `LocalIPC 1.17.4` Hub licensing client 와 `LocalIPC 1.18.1` Unity 6000.4.3f1 Editor 의 protocol mismatch 임을 확인
- **Files**: 프로젝트 변경/추가 0개 (진단만 수행)
- **GDD impact**: 없음
- **Blockers**: Unity 실행 불가. stale/mismatched licensing client 재시작 또는 Hub/Editor 버전 정렬 필요.
- **Next**: Unity Hub/Editor 완전 종료 후 `Unity.Licensing.Client` 프로세스 정리, Hub 재시작 및 라이선스 재인증
- **Agent**: Codex

---

### 2026-04-29 16:08 — Codex Unity-Skills 로컬 설치
- **Phase**: W1-1
- **Done**:
  - `Besty0728/Unity-Skills` GitHub repo 구조 확인
  - `SkillsForUnity/unity-skills~` 템플릿을 `~/.codex/skills/unity-skills` 로 설치
  - `agent_config.json` 에 Codex agent id 설정
  - `unity_skills.py` helper import 검증 (`1.8.0`)
- **Files**: 프로젝트 변경/추가 0개 (로컬 Codex skill 설정만 변경)
- **GDD impact**: 없음
- **Blockers**: Unity 프로젝트에는 아직 UPM package 미추가. AGENTS 기준 외부 의존성 추가라 실제 Unity 패키지 적용은 별도 확인 필요.
- **Next**: Codex 재시작 후 `unity-skills` 인식 확인, 이후 Unity Editor 에서 `Window > UnitySkills > Start Server` 사용
- **Agent**: Codex

---

### 2026-04-29 13:05 — Codex workspace branch Proto 정렬
- **Phase**: W1-1
- **Done**:
  - 바깥 Codex workspace repo 가 `main` 에 머물러 있던 원인 확인
  - `/Users/godju/Downloads/AI Game/hwigi-tower` 로컬 브랜치를 `Proto` 로 전환
  - 바깥 repo origin 을 GitHub `hwigija-tower` 로 설정
  - 안쪽 Unity repo `/hwigija-tower` 가 기존처럼 `Proto...origin/Proto` 인 상태 확인
- **Files**: 변경/추가 0개 (git branch/remote 설정만 변경)
- **GDD impact**: 없음
- **Blockers**: repo 가 바깥/안쪽 2중 git 구조라 장기적으로 root 정리 필요
- **Next**: 커밋 전 실제 작업 위치를 안쪽 Unity repo 로 고정하거나 repo topology 정리
- **Agent**: Codex

---

### 2026-04-29 12:50 — NPC memory interfaces + deterministic cache tests
- **Phase**: W1-1
- **Done**:
  - `INPCMemoryRepo`, `ILLMProvider`, `IDialogueRouter` 인터페이스 추가
  - `RunReflection`, `DeterministicCacheKey`, `LLMRequest`, `LLMResponse`, `DialogueRequest/Response` 모델 추가
  - `PromptHash` SHA-256 helper 추가: cache key = run_id + prompt_hash
  - `InMemoryNpcMemoryRepo` prototype stub 추가 (reflection + LLM response cache)
  - EditMode deterministic cache tests 4개 추가
- **Files**: 변경/추가 31개 (주요: Assets/_Project/Scripts/LLM/**, NPC/*Memory*.cs, Assets/_Project/Tests/EditMode/**)
- **GDD impact**: 없음 (D-006/P4 정합; real on-device LLM 미통합)
- **Blockers**: Unity EditMode test batchmode 는 프로젝트가 다른 Unity Editor 에서 열려 있어 실행 실패. scoped `git diff --check`, random/time/API 검색 통과.
- **Next**: Unity Editor 종료 후 EditMode tests 실행 또는 SQLite 영속 repo 구현으로 교체
- **Agent**: Codex

---

### 2026-04-29 12:42 — ScriptableObject 데이터 모델 추가
- **Phase**: W1-1
- **Done**:
  - AbilityData / SynergyData SO 모델 추가 (id/tag/description/effect/numeric params)
  - EnemyData SO 모델 추가 (id/hp/attack/pattern id)
  - EncounterData + EncounterType SO 모델 추가 (id/type/floor/weight/deterministic seed key)
  - NpcStateData + NpcStage(S0-S5) + NpcDisplayRule 모델 추가
  - NumericParam 공용 serializable 모델 추가; 최종 수치·서사 텍스트 미작성
- **Files**: 변경/추가 21개 (주요: Assets/_Project/Scripts/Abilities/**, Combat/**, Encounters/EncounterData.cs, NPC/**, Core/NumericParam.cs)
- **GDD impact**: 없음 (D-004 데이터 드리븐 / D-006 결정성 정합; 새 디자인 결정 없음)
- **Blockers**: Unity compile 미실행. 모델 파일 scoped `git diff --check` 및 random/time API 검색 통과.
- **Next**: Unity Editor 에서 컴파일 확인 후 placeholder SO asset 생성 또는 SQLite `INPCMemoryRepo` scaffold 진행
- **Agent**: Codex

---

### 2026-04-29 12:31 — 최소 세로 2D 프로토타입 스캐폴딩
- **Phase**: W1-1
- **Done**:
  - Unity repo 를 `Proto` 브랜치로 전환하고 origin 을 공유 GitHub repo 로 정렬
  - `Assets/_Project` 구조 아래 결정성 run context, keyboard/touch-compatible movement abstraction, player controller 추가
  - Battle/Rest/Shop/Remnant 노드 타입과 ScriptableObject stub, prototype room/runtime settings SO 추가
  - `PrototypeRoom` 테스트 씬 추가: Play 시 portrait room, player, wall, node placeholders, HUD 를 data-driven 으로 생성
  - BuildSettings 를 PrototypeRoom 으로 변경하고 portrait 테스트 해상도/autorotation 제한 적용
- **Files**: 변경/추가 69개 (주요: Assets/_Project/**, ProjectSettings/EditorBuildSettings.asset, ProjectSettings/ProjectSettings.asset)
- **GDD impact**: 없음 (잠긴 결정 변경 없음; D-003/D-004/D-006/D-016 정합 구현)
- **Blockers**: Unity batchmode 검증은 현재 프로젝트가 다른 Unity Editor 에서 열려 있어 실패. 소스/설정 정적 검증만 수행.
- **Next**: Unity Editor 에서 `Assets/_Project/Scenes/PrototypeRoom.unity` 열고 Play 테스트 후, SQLite `INPCMemoryRepo` scaffold 로 진행
- **Agent**: Codex

---

### 2026-04-29 12:05 — Unity Git hygiene 추가 + 프로젝트 점검
- **Phase**: W1-1
- **Done**:
  - AGENTS.md 와 GDD §0/§0.6/§1/§2/§3, PROGRESS 최신 항목 확인
  - Unity 프로젝트 구조 점검: Unity 6000.4.3f1, URP 2D, Input System, SampleScene 상태 확인
  - repo 루트에 Unity .gitignore 추가
  - repo 루트에 Git LFS 포함 .gitattributes 추가
  - D-016 과 실제 nested Unity repo 구조 불일치 확인
- **Files**: 변경/추가 3개 (.gitignore, .gitattributes, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Unity 프로젝트가 repo 루트가 아니라 hwigija-tower/ 하위에 있고, 해당 폴더가 별도 git repo 임
- **Next**: D-016 에 맞게 repo topology 정리 여부 확인 후 W1-1 코어 조작 + SQLite scaffold 착수
- **Agent**: Codex

---

### 2026-04-28 — OpenAI Cookbook 카탈로그 → agenda 토픽 9-11
- **Phase**: W1-1
- **Done**:
  - codex-cli-prompting §14: Cookbook 13 카테고리 leverage 매핑 (🔥 즉시 3 / 🟡 후순위 4 / ⚪ 참고 2 / ❌ 미적용 3)
  - design-agenda 토픽 9 (Reflection JSON 스키마, Structured Outputs) / 10 (Eval 설계) / 11 (Fallback Prompt Caching) 신설
  - design-brief §8: 외부 LLM 인용 가능한 자료원 명시
- **Files**: codex-cli-prompting.md, hwiglija-tower-{design-agenda, design-brief, progress}.md, log.md
- **GDD impact**: 없음 (워크플로우 + 학습 archive)
- **Next**: Codex 설치·Unity 프로젝트 생성. 토픽 10/11 은 W1-2 OQ-004 결정점 직전 (~05-04) 다룬다.
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — Codex Prompting Guide 학습 + AGENTS.md v0.4
- **Phase**: W1-1 (Codex 도입 직전)
- **Done**:
  - methods/codex-cli-prompting.md 신규 (v1.0, 13 섹션 압축, 재사용 체크리스트 포함)
  - AGENTS.md v0.4: §11 도구·병렬·Bias to Action·Preamble cadence + §12 Pragmatic + §4 generic 제거
  - repo AGENTS.md 동기화 (246 줄)
- **Files**: codex-cli-prompting.md, hwiglija-tower-AGENTS.md, log.md, index.md, /AI Game/hwigi-tower/AGENTS.md
- **GDD impact**: 없음 (AGENTS.md 위성 변경)
- **Next**: Codex CLI 설치 → Unity 프로젝트 생성 → 첫 호출 (.gitignore + .gitattributes)
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — 외부 LLM 디벨롭 워크플로우 (BRIEF + AGENDA)
- **Phase**: W1-1
- **Done**:
  - design-brief.md 신규 — 외부 LLM 핸드오프 컨텍스트 (Pitch / Pillars / D-001~019 / Tone / OQ / 일정 / 답변 포맷)
  - design-agenda.md 신규 — 8 토픽 우선순위 큐 + 토픽 1-5 상세 프롬프트 + 작가→Claude 확정 공유 포맷
  - 워크플로우: 작가가 brief+토픽 프롬프트 → Gemini → "확정: ..." → Claude 가 SSOT 반영
- **Files**: hwiglija-tower-{design-brief, design-agenda, progress}.md, log.md, index.md
- **GDD impact**: 없음 (워크플로우 문서, GDD 본문 미변경)
- **Next**: 작가가 토픽 1 (NPC 외형) 또는 토픽 2 (능력·시너지) 부터 디벨롭 시작 가능
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — 기획안 디벨롭 인프라 (PILLARS + TONE + JOURNAL)
- **Phase**: W1-1
- **Done**:
  - GDD §0.6 DESIGN PILLARS 잠금 (P1-P5 + Anti-Pillar 3) → v0.2.0 MINOR bump
  - tone-bible.md 신규 (4 음역 / S0-S5 샘플 7개 / 금지선 7개)
  - design-journal.md 신규 (결정의 *왜*·대안·기각 archive — D-019, Pillar 추출 사고 2건)
  - AGENTS.md v0.3 (§2.1 read 단계에 PILLARS·TONE 추가, §2.2 P1-P5 정합성 점검 의무화) + repo 동기화
- **Files**: hwiglija-tower-{gdd, tone-bible, design-journal, AGENTS, progress}.md, log.md, index.md, /AI Game/hwigi-tower/AGENTS.md
- **GDD impact**: §0.6 PILLARS / §14 연결 / v0.2.0
- **Next**: 작가 작업물(Codex 설치 + Unity Hub 프로젝트 생성). 기획 디벨롭 시 design-journal 활용.
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — 코어 내러티브 잠금 (D-019)
- **Phase**: W1-1
- **Done**:
  - D-019 잠금: 잃어버린 것 = 플레이어와의 유대 + 본인의 기억 / 종착 = 재망각의 루프
  - OQ-005 close
  - GDD §4.1 pitch, §5.1 NPC 정체성, §6.1 메타-읽기 (회복→붕괴→망각) 갱신 → v0.1.4
- **Files**: hwiglija-tower-gdd.md, log.md, hwiglija-tower-progress.md
- **GDD impact**: D-019 추가 / OQ-005 close
- **Next**: 작가 작업물(Codex 설치 + Unity Hub 프로젝트 생성) 대기 / OQ-006 메모리 파편 5개는 본 D-019 기반으로 W2-2 에 작성
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — OQ 4건 일괄 close (작가 결정)
- **Phase**: W1-1
- **Done**:
  - D-017 잠금: NPC 이름 = 마타이오스 (Mataios, μάταιος "무가치함")
  - D-018 잠금: Android APK 1순위, iOS = 영상 데모만 (Apple Dev 미가입)
  - OQ-001 / 007 / 008 / 009 close (07-08 은 외주에 작가 구두 전달)
  - GDD §1 CHANGELOG, §2 DECISIONS, §3 OPEN QUESTIONS, §5.1 NPC, §9 일정, §12 리스크 갱신 → v0.1.3
- **Files**: hwiglija-tower-gdd.md, log.md, hwiglija-tower-progress.md
- **GDD impact**: D-017, D-018 추가 / OQ-001, 007, 008, 009 close
- **Next**: 작가가 Codex CLI 설치 + Unity Hub 로 hwigi-tower 프로젝트 생성 → 첫 Codex 호출 (.gitignore + .gitattributes)
- **Agent**: Claude (Sub brain)

### 2026-04-27 — 프로젝트 부트스트랩
- **Phase**: W1-1 (D-21 시작)
- **Done**:
  - GDD v0.1 / v0.1.1 잠금 ([[hwiglija-tower-gdd]])
  - AGENTS.md 위성 문서 작성 ([[hwiglija-tower-AGENTS]])
  - 진행상황 추적 파일(본 파일) 생성
  - Codex 작업 환경 부트스트랩 절차 정의
- **Files**: wiki/projects/hwiglija-tower-{gdd, AGENTS, progress}.md
- **GDD impact**: D-001~D-015 잠금, OQ-001~OQ-009 등록
- **Blockers**: Codex CLI 미설치, Unity repo 미생성, OQ-007/008/009 작가 결정 대기 (마감 04-30)
- **Next**:
  1. 작가: Codex CLI 설치 (`npm i -g @openai/codex` 또는 공식 가이드)
  2. 작가: Unity 프로젝트 생성 위치 결정 후 repo 부트스트랩
  3. 작가: OQ-007/008 (외주 브리프 톤 레퍼런스) 결정
- **Agent**: Claude (Sub brain)

---

<!-- 이후 신규 항목은 본 줄 위에 prepend (최신이 위로). -->
