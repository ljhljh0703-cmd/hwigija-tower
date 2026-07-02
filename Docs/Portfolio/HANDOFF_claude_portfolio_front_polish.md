# Claude Handoff — Hwigi Tower Portfolio Front Design / Readability / Copy Polish

## Purpose

This handoff is for Claude to polish the public-facing Hwigi Tower portfolio HTML.

Primary goal:

- Improve front-end design, visual hierarchy, readability, and Korean copy quality.
- Keep the page persuasive as a game portfolio / Steam-like detail page.
- Preserve all factual claim boundaries around APK status, AI NPC training, and ML-Agents combat experiments.

This is not a Unity runtime task. Do not edit gameplay code, Unity scenes, ScriptableObjects, APK builds, or training artifacts.

## Current Source State

- Source repo: `/Users/godju/Downloads/AI Game/hwigi-tower`
- Preferred clean worktree for this package: `/private/tmp/hwigi-portfolio-steam-html`
- Source branch: `Proto`
- Current source commit at handoff time: `764aa1d21d01e8fae3e3a39b9734a42f3c9dead1`
- Public deploy repo: `https://github.com/ljhljh0703-cmd/hwigi-tower-portfolio`
- Public URL: `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/`
- Main page: `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/Docs/Portfolio/hwigi-tower-steam-portfolio.html`
- AI NPC page: `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/Docs/Portfolio/portfolio_ai_npc.html`

## Files Claude May Edit

Primary:

- `Docs/Portfolio/hwigi-tower-steam-portfolio.html`
- `Docs/Portfolio/portfolio_ai_npc.html`
- `Docs/Project/hwiglija-tower-progress.md`

Optional, only if needed for visual polish:

- `Docs/Portfolio/assets/**`

Reference-only:

- `Docs/Portfolio/HANDOFF_portfolio_ai_npc.md`
- `Docs/Portfolio/ai-assisted-combat-design-lab.md`
- `Docs/Portfolio/ml-agents-combat-exp02.md`
- `Docs/Portfolio/ml-agents-combat-exp03.md`
- `Docs/Portfolio/ml-agents-combat-exp04.md`

Do not edit:

- `Assets/_Project/**`
- `ProjectSettings/**`
- `Packages/**`
- `Builds/**`
- raw ML result folders
- ONNX/checkpoints/TensorBoard event files

## Design Direction

The page should feel like a premium game portfolio case study, not a raw internal dev page.

Use this direction:

- Steam detail page energy + technical portfolio credibility.
- Dark, cinematic, readable, editorial.
- Strong first viewport hook.
- Clear consumer-facing game premise before implementation details.
- Strong hiring-manager proof sections: problem, design decision, implementation, validation, remaining risk.
- Mobile-first readability.

Do not use ParkDal/ParkDalDesign as the visual baseline. It is legacy context only for this task family.

Avoid:

- One-note color palette.
- Decorative gradient blobs/orbs.
- Overly dense cards.
- Tiny gray text on dark panels.
- Nested cards.
- Long paragraphs without scan anchors.
- Claiming a final commercial release.

## Main Page Polish Goals

File: `Docs/Portfolio/hwigi-tower-steam-portfolio.html`

Improve:

1. Hero
   - Make the hook understandable within 5 seconds.
   - The user should know: mobile roguelike, tower loop, companion Mataios, playable APK.
   - CTA hierarchy should be clear: APK / AI NPC case / systems proof.

2. Media / proof sections
   - Separate target visual, remaster target, and source implementation capture without making the original prototype look weak.
   - Current intent: "we built the system, then develop UI/assets toward this target visual."
   - Do not phrase source screenshots as outdated or embarrassing.

3. Mataios section
   - Stronger character concept.
   - Explain companion fantasy first, then ML/AI proof.
   - Preserve distinction:
     - ML-Agents combat = combat design probe.
     - Runtime Mataios combat brain = deterministic rule table.
     - AI NPC page = separate dialogue model fine-tuning case.

4. Systems section
   - Make game design proof easier to scan.
   - Recommended structure: Route / Combat / Companion / Events / Growth / Feedback.
   - Reduce internal jargon unless immediately explained.

5. Build section
   - Keep APK link visible.
   - Keep status honest: Android playable candidate, not final release.

## AI NPC Page Polish Goals

File: `Docs/Portfolio/portfolio_ai_npc.html`

Improve:

1. First viewport
   - Stronger story: "why an AI NPC was hard" and "what was solved."
   - Keep stats visible: `3 fail`, `697`, `100% Korean`.

2. Failure timeline
   - Make failures readable as engineering evidence, not noise.
   - Highlight:
     - HCX-SEED compatibility / control failure.
     - Qwen2.5 multilingual leakage.
     - Qwen v6.2 data increase still not enough.
     - BPE / DataCollator silent failure.

3. Solution
   - EXAONE 3.5 2.4B + QLoRA + 697 JSONL should be clear.
   - Explain why model prior mattered.

4. Architecture
   - Unity client to FastAPI server should be visually simple.
   - Endpoints `/chat`, `/raw`, `/health` can remain.
   - Emphasize this is a development/integration path, not a completed on-device mobile deployment.

5. Claim boundary
   - Keep a visible boundary note:
     - not combat RL
     - not mobile on-device completion
     - not final shipped AI

## Korean Copy / Readability Rules

Priority order:

1. 맞춤법, 띄어쓰기, 문장 호흡.
2. Clear scanability.
3. Marketing hook.
4. Technical detail.

Copy style:

- Korean-first.
- Technical terms can remain in English when standard: `FastAPI`, `QLoRA`, `EXAONE`, `Unity`, `ML-Agents`.
- Avoid comma chaining.
- Avoid hype such as "혁신적", "완벽", "상용급", "최종".
- Use concrete proof language: "구현했다", "검증했다", "분리했다", "보고했다", "후보로 정리했다".
- Do not invent new gameplay facts, metrics, screenshots, or APK hashes.

## Fact / Claim Boundaries

These must remain true after editing:

- The current public HTML is a portfolio page, not a store release page.
- The APK is a playable prototype/candidate, not a final commercial release.
- ML-Agents combat experiments were used as a combat design debugger/probe.
- The shipped/prototype runtime does not connect PPO/ONNX/RL to main gameplay.
- Runtime Mataios combat behavior is deterministic rule-based.
- AI NPC training page describes dialogue model fine-tuning:
  - EXAONE 3.5 2.4B
  - QLoRA r=32, alpha=64
  - 697 SFT rows
  - FastAPI + transformers + Unity integration path
- Do not claim mobile on-device EXAONE deployment is complete.
- Do not claim Mataios is "already RL-trained."
- Do not expose raw ML result files or private/local paths in public copy.
- Keep the Google Drive APK folder link unchanged unless the user provides a new link.

Forbidden phrases / overclaims to search for:

```text
마타이오스가 이미 RL로 학습
PPO를 본편에 연결
ONNX를 본편에 연결
온디바이스 완료
모바일 탑재 완료
상용 출시 완료
최종 출시
완벽히 검증
```

## Validation Required

Run from the source worktree:

```bash
git diff --check
node - <<'NODE'
const fs = require('fs');
const pages = [
  'Docs/Portfolio/hwigi-tower-steam-portfolio.html',
  'Docs/Portfolio/portfolio_ai_npc.html'
];
for (const file of pages) {
  const text = fs.readFileSync(file, 'utf8');
  if (!text.includes('</html>')) throw new Error(`${file}: missing closing html`);
}
const main = fs.readFileSync(pages[0], 'utf8');
const ai = fs.readFileSync(pages[1], 'utf8');
if (!main.includes('portfolio_ai_npc.html')) throw new Error('main page must link AI NPC page');
if (!ai.includes('EXAONE 3.5 2.4B')) throw new Error('AI page lost EXAONE evidence');
if (!ai.includes('FastAPI')) throw new Error('AI page lost FastAPI architecture');
const bad = [
  '마타이오스가 이미 RL로 학습',
  'PPO를 본편에 연결',
  'ONNX를 본편에 연결',
  '온디바이스 완료',
  '모바일 탑재 완료',
  '상용 출시 완료',
  '최종 출시',
  '완벽히 검증'
];
for (const phrase of bad) {
  if (main.includes(phrase) || ai.includes(phrase)) {
    throw new Error(`overclaim phrase found: ${phrase}`);
  }
}
console.log('portfolio static claim checks passed');
NODE
```

Also verify local relative assets:

```bash
python3 - <<'PY'
from pathlib import Path
import re, sys
root = Path('.').resolve()
missing = []
for html in [root/'Docs/Portfolio/hwigi-tower-steam-portfolio.html', root/'Docs/Portfolio/portfolio_ai_npc.html']:
    text = html.read_text()
    for attr, ref in re.findall(r'(href|src)="([^"]+)"', text):
        if ref.startswith(('#', 'http://', 'https://', 'mailto:')):
            continue
        target = (html.parent / ref.split('#')[0]).resolve()
        if not target.exists():
            missing.append((str(html.relative_to(root)), ref))
if missing:
    print(missing)
    sys.exit(1)
print('relative link asset check passed')
PY
```

Optional but recommended:

- Open both pages in browser at desktop and mobile widths.
- Check hero readability, CTA hierarchy, image cropping, and text overflow.
- If visual screenshot tooling is unavailable, report `visual browser check: not run`.

## Public Deploy Notes

Source edits go to `origin/Proto`.

The public GitHub Pages repo is separate:

- `https://github.com/ljhljh0703-cmd/hwigi-tower-portfolio`
- `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/`

Why separate:

- The source repo is private.
- Current GitHub plan did not support Pages for the private source repo.
- The public repo contains only static portfolio HTML and display assets.

Do not deploy automatically unless the user asks. If deploying, copy only referenced static HTML/image assets. Do not copy:

- raw ML CSV/JSON evidence
- checkpoints
- ONNX
- TensorBoard event files
- Unity project source
- secrets or local config

## Expected Claude Report

Claude should report:

```text
[Done] <front/readability/copy polish summary>
[Files] <changed files>
[Public URL] unchanged / redeployed URL
[Validation] git diff --check, static claim check, relative asset check, browser visual check status
[Remaining risk] <only real risks>
[Next] <one concrete next action>
```

## Recommended Work Order

1. Read this handoff.
2. Inspect both HTML pages in source form.
3. Make design/readability/copy edits in small patches.
4. Run validation.
5. Update `Docs/Project/hwiglija-tower-progress.md`.
6. Commit/push to `origin/Proto`.
7. If user requests publish, update the public deploy repo and verify HTTP 200.
