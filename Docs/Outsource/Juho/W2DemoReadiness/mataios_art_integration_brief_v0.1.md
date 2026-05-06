# Mataios Art Integration Brief v0.1

## Source Asset

- Current file: `Assets/_Project/Art/Characters/마타이오스 전신.png`
- Current state: received full-body illustration, not modified in this pass.
- Demo role: S0-S2 common portrait or bust candidate.

## Recommended Rename Candidates

| purpose | filename |
|---|---|
| keep current full-body source | `char_mataios_fullbody_s0_s2_source.png` |
| cropped UI portrait derivative | `char_mataios_bust_s0_s2.png` |
| future transparent cutout | `char_mataios_fullbody_s0_s2_transparent.png` |

Use lowercase English plus underscores for imported runtime files. Keep the original Korean filename only as a raw intake/source file if needed.

## Import Settings

| setting | recommendation |
|---|---|
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single |
| Mesh Type | Full Rect for UI portrait; Tight only if transparent cutout is clean |
| Pixels Per Unit | 100 unless the UI prefab requires another local standard |
| Max Size | 2048 for source/full-body, 1024 for portrait derivative |
| Compression | None or High Quality for portrait review; avoid visible banding on face/hood |
| Mip Maps | Off for UI |
| sRGB | On |
| Alpha | Current image appears to include a background. Use alpha only after a transparent derivative is exported. |

## UI Placement

| screen | placement |
|---|---|
| Encounter / result panel | Left or upper-left portrait anchor, with text and choices kept unobstructed. |
| Rest / NPC reaction surface | Bust crop from hood to upper torso; face should be the first read. |
| CombatGate | Small side portrait only, not centered over the enemy/action surface. |
| MemoryFragment | Optional low-opacity portrait accent; do not imply final memory truth. |

## Portrait Crop Criteria

- Keep hood silhouette, face, shoulder armor, and upper torso.
- Crop no lower than the waist for S0-S2 bust usage.
- Avoid a crop that centers abdomen or legs; the demo read should be cautious companion, not costume showcase.
- Preserve the reserved/cautious expression.
- Leave enough padding around hood and hair so mobile UI does not clip the silhouette.

## S0-S2 Usage

- Use as S0-S2 common portrait if the writer accepts the current expression as neutral/cautious.
- Do not use as S3/S4 by applying heavy filter directly to the source.
- S3/S4 need separate approved variants or controlled overlay treatment after writer direction.

## Future Variant Needs

| stage | need |
|---|---|
| S0-S2 | one neutral/cautious portrait is enough for W2 demo. |
| S3 | subtle fracture variant: expression strain, light glitch edge, still readable. |
| S4 | collapse variant: stronger breakup, but identity still recognizable. |
| S5 | separate ending-state art only after ending presentation is locked. |

## Dev Handoff Tasks

1. Rename or duplicate source under stable English filename.
2. Export or request `char_mataios_bust_s0_s2.png` as transparent 1024x1024.
3. Import as Sprite (2D and UI), mipmaps off.
4. Add a UI Image slot for Mataios portrait on the demo HUD or encounter panel.
5. Wire portrait visibility for Shop/MoralChoice/MemoryFragment/CombatGate review surfaces.
6. Keep all dialogue and story text out of the image asset.
