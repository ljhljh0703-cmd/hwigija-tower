# Art Intake

Place visual source imports here before wiring them into scenes or prefabs.

- `Characters/`: player, NPC, enemy, boss sprites.
- `Encounters/`: encounter-specific backgrounds and cutscene focus images.
- `Enemies/`: enemy sprites used by combat presentation slots.
- `Environments/`: tower rooms, backgrounds, floors, props.
- `Nodes/`: battle, rest, shop, remnant node visuals.
- `UI/`: HUD, panels, buttons, non-icon UI art.
- `Icons/`: ability, synergy, item, status icons.
- `VFX/`: sprite sheets or source textures for visual effects.

Keep generated/source files named by role first, then variant:
`char_mataios_bust_s0_s2`, `enc_shop_01_bg`, `enemy_fracture_hound`.

Current W2 demo intake slots:

- `Encounters/enc_shop_01_bg.png`
- `Encounters/enc_moral_choice_01_bg.png`
- `Encounters/enc_memory_fragment_01_bg.png`
- `Encounters/enc_memory_fragment_01_art.png`
- `Encounters/enc_combat_gate_01_bg.png`
- `Encounters/demo_complete_bg.png`
- `Characters/char_mataios_bust_s0_s2.png`
- `Enemies/enemy_fracture_hound.png`

Combat UI slots prepared:

- `Characters/char_player_portrait_temp.png` — optional player portrait slot. If absent, the HUD uses a built-in player silhouette fallback.
- `Nodes/node_combat.png` — temporary attack action icon source until dedicated action icons arrive.
- `Nodes/node_rest.png` — temporary defend action icon source until dedicated action icons arrive.
- `Nodes/node_event.png` — temporary skill/scout action icon source until a dedicated skill icon arrives.
