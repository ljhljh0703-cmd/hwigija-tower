# MLC-LLM Native Plugin Intake

Place MLC-LLM Unity/native integration files here before copying platform binaries
into `Assets/Plugins/Android` or `Assets/Plugins/iOS`.

Expected Unity bridge:

- Native library name: `mlc_llm_unity`
- Required entry point: `hwigi_mlc_complete`
- C# caller: `Assets/_Project/Scripts/LLM/MLCBridge.cs`

The plugin is not vendored yet.
