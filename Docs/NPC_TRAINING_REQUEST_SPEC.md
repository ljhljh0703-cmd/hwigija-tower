# NPC Training & Model Selection Specification (RFP)

## 1. Project Overview
- **Project Name:** 회귀자는 탑을 오른다 (hwiglija-tower)
- **Target NPC:** Mataios (10s Girl Warrior, Silver hair, Giant sword)
- **Core Loop:** Recovery → Collapse → Oblivion (S0 to S4 stages)
- **Target Platform:** Mobile (Portrait), On-device Inference (Vulkan/Metal)

## 2. Technical Constraints (Strict)
- **Model Size:** 0.5B to 1.5B parameters (for real-time mobile performance).
- **Format:** MLC-LLM compatible (quantized q4f16_1 or similar).
- **Latency Target:** First token under 200ms on modern mobile devices.
- **Context Window:** Max 512 tokens.

## 3. Training Requirements
Your proposal must address the following three methodologies as defined in our `Master Guide`:

### 3.1 BIW (Brain-inspired Warm-up) for Honest Forgetting
- **Goal:** Prevent hallucinations during the 'Collapse' (S3-S4) stage.
- **Requirement:** Propose a warm-up phase using noise data to ensure the model admits "I don't know" or "Memory is severed" (narrative honesty) instead of inventing facts.

### 3.2 RoleLLM & Tone C Fidelity
- **Tone:** Poetic, Fragmentary, Distant yet Aesthetic (Tone C).
- **Requirement:** Use Dialogue Engineering and Few-shot datasets to lock the "Silver-haired Girl Warrior" persona. Avoid generic AI assistant helpfulness.

### 3.3 Persona Vector Lock
- **Requirement:** Suggest a method to prevent 'Persona Drift' during long runs. Propose how to use Persona Vectors or PEFT (LoRA) to keep the 0.5B model's personality stable.

## 4. Evaluation Criteria (Ensemble-Judge)
The proposed training plan will be evaluated by an ensemble of 5 LLMs based on:
1. **Persona Consistency:** Does she sound like the 10s warrior defined in the Tone Bible?
2. **Narrative Honesty:** Does she refuse unknown facts gracefully during S3-S4?
3. **On-device Feasibility:** Is the proposed model optimized for the MLC-LLM runtime?

## 5. Expected Output from AI Trainer
1. **Model Recommendation:** Select a base model (e.g., HCX-Seed-0.5B, Qwen2.5-1.5B-Instruct, etc.) and justify based on Korean language performance.
2. **Training Recipe:** Detail the stages of Fine-tuning (Warm-up -> SFT -> LoRA).
3. **Data Augmentation Plan:** How to expand the 60+ samples provided in the content draft into a full training set.
4. **Validation Strategy:** Metrics for measuring BIW effectiveness and Tone C fidelity.
