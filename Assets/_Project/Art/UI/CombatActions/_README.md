# Combat Action Icons

Drop dedicated combat action icons here before replacing the temporary node-icon bindings in `SO_DemoPresentationData`.

Expected files:

- `icon_action_attack.png`
- `icon_action_defend.png`
- `icon_action_skill_scout.png`

Import rules:

- Texture Type: Sprite (2D and UI)
- Alpha: enabled if the icon has transparent edges
- Compression: low or none if mobile readability suffers
- Shape: square source preferred
- Role: icon only; keep the short Korean label in UI text

Current runtime behavior:

- `DemoPresentationData` already exposes `CombatAction` icon slots.
- `PrototypeHud` renders action buttons as icon + short label.
- Until these files exist and are assigned, the project uses temporary node icon art for Attack, Defend, and Skill.
