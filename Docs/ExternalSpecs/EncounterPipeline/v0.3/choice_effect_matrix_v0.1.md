# choice_effect_matrix_v0.1.md

Effect kinds present in the pack and their immediate behaviour.

| EffectKind | Affects | ImmediateFeedback | Clamp/Range | Example |
| --- | --- | --- | --- | --- |
| ModifyHp | hp | Yes | clampToMax optional | {"kind":"ModifyHp","amount":-5,"clampToMax":true} |
| ModifyMental | mental | Yes | -100..100 (recommended) | {"kind":"ModifyMental","amount":-1} |
| ModifyAffinity | affinity | Yes | -100..100 | {"kind":"ModifyAffinity","amount":-1,"clamp":"-100..100"} |
| ModifyGlitchLevel | glitchLevel | Yes | 0..100 | {"kind":"ModifyGlitchLevel","amount":2,"clamp":"0..100"} |
| SetFlag | run/story flags | Yes | - | {"kind":"SetFlag","flag":"FLAG_META_02_RECALL","value":true} |
| UnlockMemoryFragment | memory fragment unlock | Yes | - | {"kind":"UnlockMemoryFragment","memoryFragment":{"stableId":"MEM_FRAGMENT_05","stage":"S5_REST_OR_CONTINUE","textKey":"PLACEHOLDER_MEM_FRAGMENT_05"}} |
| SetNpcStage | npc stage FSM | Yes | - | {"kind":"SetNpcStage","npcStage":"S5_REST_OR_CONTINUE"} |
| StartCombat | hands off to CombatFlow | No (handoff) | - | {"kind":"StartCombat","combatHandoff":{"kind":"CombatHandoff","stableId":"COMBAT_GATE_03","sourceEncounterId":"ENC_COMBAT_GATE_03","sourceChoiceId":"CHOICE_COMBAT_03_ENGAGE","seedKey":"SEED_COMBAT_GATE_03","sourceHash":"sha256:8888888888888888888888888888888888888888888888888888888888888888","floor":5,"enemyRefs":["ENEMY_COLLAPSE_ECHO"],"playerSnapshot":{"kind":"PlayerSnapshotCaptureSpec","captureMode":"RuntimeAtChoiceCommit","include":["hp","maxHp","mental","gold","glitchLevel","affinity","abilityRefs","itemRefs","statusRefs","flagsHash","runSeed"]},"npcStage":"S4_COLLAPSE","onVictoryEffects":[{"kind":"ModifyGold","amount":11},{"kind":"ModifyGlitchLevel","amount":-2,"clamp":"0..100"},{"kind":"ModifyAffinity","amount":2,"clamp":"-100..100"},{"kind":"SetFlag","flag":"FLAG_COMBAT_GATE_03_VICTORY","value":true}],"onDefeatEffects":[{"kind":"ModifyHp","amount":-9,"clampToMax":true},{"kind":"ModifyGlitchLevel","amount":5,"clamp":"0..100"},{"kind":"ModifyAffinity","amount":-2,"clamp":"-100..100"}],"deathPolicy":{"mode":"Regression","memoryRetention":"UnlockedFragmentsOnly"},"returnNode":{"onVictory":"NODE_COMBAT_GATE_03_VICTORY","onDefeat":"NODE_COMBAT_GATE_03_DEFEAT"},"postCombatText":{"victoryTextKey":"PLACEHOLDER_POST_COMBAT_GATE_03_VICTORY","defeatTextKey":"PLACEHOLDER_POST_COMBAT_GATE_03_DEFEAT","deathTextKey":"PLACEHOLDER_POST_COMBAT_GATE_03_DEATH"},"npcImmediateReaction":{"reactionKey":"NPC_REACT_COMBAT_GATE_03_START","stage":"S4_COLLAPSE","toneHint":"warning","textKey":"PLACEHOLDER_NPC_REACT_COMBAT_GATE_03_START"},"promptHash":"sha256:9999999999999999999999999999999999999999999999999999999999999999","cacheKey":"CACHE_COMBAT_GATE_03_S4_COLLAPSE"}} |
| ModifyGold | gold | Yes | 0..999 (pack rule) | {"kind":"ModifyGold","amount":0} |
| AddItem | inventory (itemRef) | Yes | - | {"kind":"AddItem","itemRef":"ITEM_TORN_CHARM","count":1} |
| AddAbility | abilities (abilityRef) | Yes | - | {"kind":"AddAbility","abilityRef":"ABILITY_RECALL_ANCHOR"} |
| GrantRewardBundle | reward bundle resolution | Yes | - | {"kind":"GrantRewardBundle","rewardBundleRef":"REWARD_CACHE_MEMORY"} |

Notes:
- ImmediateFeedback = whether players see direct state change at choice commit (StartCombat uses Combat Handoff).
