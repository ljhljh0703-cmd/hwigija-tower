# Asset and AI Status Matrix v0.1

## Scope

- Unity was not run.
- Status is based on repo files, manifests, design docs, and existing screenshots.
- "Needed before recording/build" separates W2 video recording from Android build readiness.

| item | status | source/evidence | missing item | owner | needed before recording/build |
|---|---|---|---|---|---|
| Mataios art | ready-for-review | `Assets/_Project/Art/Characters/마타이오스 전신.png`; screenshots 01-05; `viewport_qa_result_v0.1.md` | Optional cropped transparent portrait derivative | outsource/dev | Recording: no, unless readability fails on device. Build: no. |
| Future S3/S4 portrait | missing | GDD S3-S4 collapse stages; current art folder has one full-body source | Stage-specific portrait variants and state binding | outsource/writer/dev | Recording: no for W2 route. Build: yes for fuller NPC progression. |
| UI SFX | missing | `design/sound-brief.md`; `Assets/_Project/Audio/_README.md`; audio folders contain no payload files | `SFX_UI_01`, `SFX_UI_02`, `SFX_UI_03`, `SFX_UI_07` or silent-placeholder policy | outsource/dev | Recording: recommended. Build: yes for polish. |
| Combat SFX | missing | `design/sound-brief.md`; no WAV/OGG payload under `Assets/_Project/Audio/SFX` | Attack, hit, enemy defeated, player damage, victory cues | outsource/dev | Recording: recommended. Build: yes. |
| Memory unlock SFX | missing | `design/sound-brief.md` `SFX_UI_07`; memory route screenshots have no audio evidence | Memory unlock cue asset and event hook | outsource/dev | Recording: recommended. Build: yes. |
| Demo complete sting | missing | `design/sound-brief.md` has ending/BGM guidance but no current payload | Short completion sting or explicit silent policy | outsource/dev | Recording: recommended. Build: yes. |
| Android model artifact | pending/blocker | `Docs/AI_NPC_MODEL_SPEC.md`; `Assets/_Project/Models/**/MODEL_MANIFEST.md` | Compiled Android runtime artifact under approved storage policy | AI-training/dev | Recording: no if deterministic fallback is accepted. Build: yes. |
| Tokenizer | scaffolded/pending | `Assets/_Project/Models/*/tokenizer.meta`; `mataios-demo-sft-v0.1/tokenizer/.gitkeep` | Actual tokenizer files matched to chosen model | AI-training | Recording: no. Build/model smoke: yes. |
| Compiled model | scaffolded/pending | `Assets/_Project/Models/*/compiled/**`; `.gitkeep` only for demo SFT scaffold | q4 mobile artifact and checksums | AI-training/dev | Recording: no. Build/model smoke: yes. |
| Eval report | pending | `hcx-seed-0.5b/eval/quality_report.md`; `latency_report.md` show pending fields | Quality, latency, sample response validation filled and reviewed | AI-training/writer/dev | Recording: no if fake/cache. Build/model acceptance: yes. |
| Final memory text | writer-blocked | GDD OQ-006; `design/memory-fragments.md`; screenshots show placeholder memory keys | Writer-approved MEM_FRAGMENT_01 title/body and later fragment prose | writer | Recording: yes for polished public clip if memory panel is visible. Build: yes. |

## Summary

The visual W2 route can be recorded with deterministic fake/cache AI if debug labels are hidden and placeholder memory keys are either accepted as internal QA or hidden for public capture. Real on-device AI remains blocked by tokenizer, compiled artifact, eval report, native plugin, and Android build readiness.
