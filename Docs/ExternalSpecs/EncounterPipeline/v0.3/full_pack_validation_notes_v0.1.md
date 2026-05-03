# full_pack_validation_notes_v0.1.md

Pack: PACK_FULL_25_V003 (schema 0.2)

## Summary

- Total encounters: 25
- Manifest expected NpcRest: 8
- Manifest expected NpcMemoryFragment: 5
- Manifest expected GeneralEncounter: 7
- Manifest expected MoralChoice: 3
- Manifest expected MetaText: 2

## Encounter counts by contentCategory

| Category | Count |
| --- | --- |
| NpcRest | 8 |
| NpcMemoryFragment | 5 |
| GeneralEncounter | 7 |
| MoralChoice | 3 |
| MetaText | 2 |

## Pack vs Manifest check

- Manifest counts match actual counts.

## Forbidden content matches (should be none)

- No forbidden tokens found.

## Notes and recommendations

- Asset bake is `updateExisting` and prohibits delete/recreate — OK.
- Ensure `schemaVersion` aligns with pack consumer (this pack uses v0.2).
- Review `choice` unavailablePolicy usage for shop/display UX.
- Run schema validator against `encounter_schema_v0.2.json` if available.
