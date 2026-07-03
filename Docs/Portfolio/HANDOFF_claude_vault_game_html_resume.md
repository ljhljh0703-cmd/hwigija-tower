# Claude / Vault Shared Brief — Game HTML Portfolio to Resume

## Purpose

이 문서는 `회귀자는 탑을 오른다` 게임 포트폴리오 HTML을 이력서/포트폴리오 문장으로 흡수하기 위한 공통 브리프다.

대상:

- Claude 이력서/포트폴리오 세션
- Vault Claude Gate / Sub-brain 흡수 세션
- 이후 게임 HTML 포트폴리오를 이력서에 묶는 세션

핵심 목표:

- public HTML 링크를 이력서에 붙일 수 있는 형태로 정리한다.
- 구현 사실과 claim boundary를 분리한다.
- 게임 기획, 개발, AI NPC, ML-Agents, multi-agent production pipeline을 하나의 resume-ready project story로 묶는다.

## Current Source State

- Source repo: `/Users/godju/Downloads/AI Game/hwigi-tower`
- Source branch: `Proto`
- Current source commit at this brief: `4827ca89cbd2b01c17c254da8b667cbd8f9762b3`
- Public deploy repo: `hwigi-tower-portfolio`
- Public deploy commit checked during image evidence update: `73ba049fe008bb12b87a220219869fb83decb4ac`
- Main dirty Unity worktree is not the evidence baseline.

## Public Links

| Page | URL | Role |
|---|---|---|
| Main game portfolio | `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/Docs/Portfolio/hwigi-tower-steam-portfolio.html` | Steam-style playable prototype / vertical slice case study |
| AI NPC case study | `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/Docs/Portfolio/portfolio_ai_npc.html` | EXAONE / QLoRA / FastAPI / Unity AI NPC training case |
| APK folder | `https://drive.google.com/drive/folders/1GSTXDCrkHHI-TVfRA-hYQs7hh3PtnTZG?usp=drive_link` | Android APK download folder |

Verification at brief creation:

- Main game portfolio: HTTP 200
- AI NPC case study: HTTP 200
- AI NPC page contains the updated evidence image captions such as `실험 증거 contact sheet`

## Source Files

Primary HTML:

- `Docs/Portfolio/hwigi-tower-steam-portfolio.html`
- `Docs/Portfolio/portfolio_ai_npc.html`

Current supporting docs:

- `Docs/ProjectOps/vertical-slice-case-study-harness.md`
- `Docs/ProjectOps/vertical-slice-session-dispatches.md`
- `Docs/Portfolio/vertical-slice-case-study-outline.md`
- `Docs/Portfolio/HANDOFF_portfolio_ai_npc.md`
- `Docs/Portfolio/HANDOFF_claude_portfolio_front_polish.md`

Visual/evidence assets:

- `Docs/Portfolio/assets/hwigi_target_*.jpg`
- `Docs/Portfolio/assets/hwigi_impl_remaster_*.jpg`
- `Docs/Portfolio/assets/ai-npc/*`
- `Docs/Portfolio/assets/mlagents_exp04_*`

## Resume Positioning

Recommended project title variants:

1. `AI-assisted Mobile Roguelike Vertical Slice`
2. `Hwigi Tower — AI NPC & Combat Design Probe Case Study`
3. `Unity Game Prototype + AI NPC Production Pipeline`
4. `AI-assisted Game Production Pipeline Case Study`

Recommended role framing:

- Solo developer / technical PM / game systems designer
- AI-assisted production orchestrator
- Unity prototype developer
- AI NPC experiment owner
- Portfolio packaging / release artifact owner

Best-fit resume categories:

- Game Development
- Technical Product / PM
- AI-native production workflow
- AI NPC / LLM application
- Prototyping and validation pipeline

## Resume-Ready Summary

Short version:

> Built a playable Unity Android roguelike prototype and packaged it as a public vertical slice case study, combining two-actor combat, route/event systems, deterministic companion behavior, ML-Agents combat design probes, and an EXAONE/QLoRA AI NPC training pipeline.

Korean version:

> Unity 기반 모바일 로그라이크 프로토타입을 Android APK와 공개 포트폴리오 HTML로 패키징하고, 2인 전투, 이벤트/지도 루프, deterministic 동행자 전투 brain, ML-Agents 전투 설계 검증, EXAONE/QLoRA AI NPC 대화 실험을 하나의 vertical slice case study로 정리했습니다.

## Resume Bullets

Use only bullets that fit the target JD.

- Built and packaged a playable Unity 6 Android roguelike prototype with route selection, event choices, two-actor combat, rewards, shop/rest, and boss flow.
- Designed Mataios as a deterministic companion system rather than a decorative NPC, separating player and companion combat actions in UI and system logic.
- Used ML-Agents as a combat design probe to expose degenerate strategies such as AttackSpam/SkillSpam and translate the findings into deterministic companion policy design.
- Developed an AI NPC dialogue case study using EXAONE 3.5 2.4B, QLoRA, FastAPI, and Unity integration boundaries, with evidence for Korean retention, BIW blocking, and tokenizer failure diagnosis.
- Operated a multi-agent production pipeline across Command Center, System Design, Game Dev, UI/Asset, AI, Release/Ops, and Portfolio sessions, using clean worktrees, evidence gates, artifact SHA tracking, and public deployment checks.
- Converted prototype artifacts into public portfolio pages with claim boundaries, APK download path, visual evidence, and implementation proof.

## Public Claim Boundary

Allowed:

- `Playable prototype`
- `Vertical slice case study`
- `Unity Android APK candidate`
- `Deterministic Mataios combat brain`
- `ML-Agents combat design probe`
- `AI NPC dialogue training case study`
- `EXAONE 3.5 2.4B + QLoRA experiment`
- `FastAPI / Unity integration pipeline`
- `Multi-agent production workflow`

Do not claim:

- shipped commercial release
- production-ready game
- runtime RL / PPO in shipped gameplay
- ONNX model connected to the main game runtime
- fully on-device EXAONE deployment
- all Unity tests green
- device smoke completed by Codex
- final story/dialogue complete

## Claude Usage

Claude 이력서/포트폴리오 세션은 이 문서를 사실 출처로 사용한다.

Recommended workflow:

1. Read this shared brief.
2. Read only the public pages if wording needs current tone.
3. Convert the project into a resume project card.
4. Keep claim boundary intact.
5. Do not invent business metrics, DAU, revenue, hiring impact, or commercial release status.
6. If the resume has multiple game projects, place this as the strongest `AI-assisted game production / AI NPC` case.

Suggested resume card structure:

```text
Project: Hwigi Tower — AI-assisted Mobile Roguelike Vertical Slice
Role: Solo Developer / Technical PM / AI Workflow Orchestrator
Stack: Unity 6, C#, Android, ML-Agents, EXAONE 3.5 2.4B, QLoRA, FastAPI, GitHub Pages
Evidence: public game portfolio, AI NPC case study, APK folder
Impact: playable prototype + evidence-backed AI/game development pipeline
Boundary: prototype, not commercial release; AI training is design/evidence pipeline, not shipped runtime RL
```

## Vault Usage

Vault Claude는 이 문서를 직접 정본으로 승격하지 말고 Gate 후보로 검토한다.

Recommended absorption targets:

- Portfolio positioning note
- Game project artifact registry
- AI-assisted game production method
- Multi-agent development pipeline lesson
- Claim-boundary checklist for public game portfolio pages

Recommended cross-links:

- `vertical-slice-case-study-harness.md`
- `vertical-slice-case-study-outline.md`
- `portfolio_ai_npc.html`
- `hwigi-tower-steam-portfolio.html`
- `ai-assisted-combat-design-lab.md`

Do not absorb as:

- final GDD decision
- proof of shipped AI runtime
- finished commercial release record
- general AI NPC success claim without boundaries

## Game HTML Portfolio Registry

Current registry for resume use:

| Project | Status | Public page | Resume angle |
|---|---:|---|---|
| Hwigi Tower main | Live public | `hwigi-tower-steam-portfolio.html` | Playable Unity Android vertical slice |
| Hwigi Tower AI NPC | Live public | `portfolio_ai_npc.html` | EXAONE/QLoRA AI NPC training case |

Expansion rule:

- Add future game HTMLs to this table only after public URL HTTP 200 verification.
- Separate `live public`, `internal draft`, and `missing/stale` instead of mixing them.
- Do not paste private local paths into public-facing resume HTML.

## Next Recommended Work

1. Claude resume session: convert this into one resume project card and 3-5 targeted bullets.
2. Portfolio session: add a compact `Game Projects` index section if multiple public game HTML pages are attached.
3. Vault session: absorb the workflow as a proposed portfolio/career-positioning asset, not as a gameplay SSOT.
4. Later optimization: compress AI NPC character PNGs to WebP/JPG before final public resume packaging.

## 10-line Handoff

1. Hwigi Tower is now positioned as a portfolio vertical slice case study.
2. Public main page is live and should be linked from the resume.
3. Public AI NPC page is live and should support the AI/ML project narrative.
4. APK folder is available as the playable artifact link.
5. Main claim is playable prototype plus AI-assisted production pipeline, not commercial release.
6. ML-Agents claim is combat design probe only, not shipped runtime RL.
7. AI NPC claim is EXAONE/QLoRA dialogue training case study, not fully on-device mobile deployment.
8. Claude should convert this into resume cards and bullets without inventing metrics.
9. Vault Claude should Gate/absorb as portfolio/career positioning knowledge.
10. Future game HTMLs should be added through the same live/public vs internal/draft registry.
