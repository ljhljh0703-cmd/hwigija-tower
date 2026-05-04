# Asset Delivery Manifest v0.1

| priority | asset stableId or usage | filename recommendation | Unity target folder | format | size/duration guidance | needed by date | notes |
|---|---|---|---|---|---|---|---|
| P0 | player placeholder replacement | char_player_demo_placeholder.png | Assets/_Project/Art/Characters | PNG transparent | 1024x1024 | 2026-05-07 KST | Bust or half-body, readable in vertical mobile UI. |
| P0 | Mataios portrait or bust | char_mataios_bust_s0_s2.png | Assets/_Project/Art/Characters | PNG transparent | 1024x1024 | 2026-05-07 KST | One S0-S2 neutral/cautious expression, no final costume lock required. |
| P0 | ENC_SHOP_01 background | enc_shop_01_bg.png | Assets/_Project/Art/Encounters | PNG | 1080x1920 | 2026-05-07 KST | Quiet tower-market impression, no readable text signs. |
| P0 | ENC_MORAL_CHOICE_01 image | enc_moral_01_bg.png | Assets/_Project/Art/Encounters | PNG | 1080x1920 | 2026-05-08 KST | Use traces and staging, avoid explicit moral instruction. |
| P0 | MEM_FRAGMENT_01 visual | enc_memory_fragment_01.png | Assets/_Project/Art/Encounters | PNG with alpha | 1024x1024 | 2026-05-08 KST | Fragment/card object, usable over dark background. |
| P0 | ENEMY_FRACTURE_HOUND visual | enemy_fracture_hound_demo.png | Assets/_Project/Art/Encounters | PNG transparent | 1024x1024 | 2026-05-08 KST | Demo enemy silhouette, no final monster approval implied. |
| P1 | choice button frame | ui_choice_button_frame_demo.png | Assets/_Project/Art/UI | PNG 9-slice | 512x256 | 2026-05-08 KST | Should support 2-3 stacked mobile choices. |
| P1 | result panel frame | ui_result_panel_demo.png | Assets/_Project/Art/UI | PNG 9-slice | 768x512 | 2026-05-08 KST | Must fit numeric result lines. |
| P0 | button click sound | sfx_ui_button_click.wav | Assets/_Project/Audio/SFX | WAV | 0.05-0.12s | 2026-05-07 KST | Soft mobile UI click. |
| P0 | choice confirm sound | sfx_choice_confirm.wav | Assets/_Project/Audio/SFX | WAV | 0.15-0.35s | 2026-05-07 KST | Slightly more final than basic click. |
| P0 | combat start sound | sfx_combat_start.wav | Assets/_Project/Audio/SFX | WAV | 0.5-1.2s | 2026-05-08 KST | Used at CombatGate handoff. |
| P0 | memory unlock sound | sfx_memory_unlock.wav | Assets/_Project/Audio/SFX | WAV | 0.8-1.5s | 2026-05-08 KST | Fragile, unstable, not triumphant. |
| P0 | demo complete sting | mx_demo_complete_sting.wav | Assets/_Project/Audio/Music | WAV | 2.0-4.0s | 2026-05-09 KST | Short completion cue, non-looping. |
| P1 | tower ambient loop | amb_tower_demo_loop.ogg | Assets/_Project/Audio/Music | OGG | 30-45s loop | 2026-05-09 KST | Low-volume mobile ambience. |

## Delivery Rules

- Do not embed final Korean dialogue or story text into images.
- Do not include readable UI labels in art assets.
- Keep all assets replaceable placeholders unless art director approval says otherwise.
- File names should remain lowercase with underscores.
