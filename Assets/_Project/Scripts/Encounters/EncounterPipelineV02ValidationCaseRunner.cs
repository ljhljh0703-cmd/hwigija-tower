#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

namespace HwigiTower.Encounters
{
    public sealed class EncounterValidationCaseRun
    {
        public EncounterValidationCaseRun(string id, bool passed, string expectedErrorCode, EncounterValidationResult validation)
        {
            Id = id ?? string.Empty;
            Passed = passed;
            ExpectedErrorCode = expectedErrorCode ?? string.Empty;
            Validation = validation;
        }

        public string Id { get; }
        public bool Passed { get; }
        public string ExpectedErrorCode { get; }
        public EncounterValidationResult Validation { get; }
    }

    public sealed class EncounterValidationCaseRunSummary
    {
        private readonly List<EncounterValidationCaseRun> _runs = new List<EncounterValidationCaseRun>();

        public IReadOnlyList<EncounterValidationCaseRun> Runs => _runs;
        public bool Passed
        {
            get
            {
                for (var i = 0; i < _runs.Count; i++)
                {
                    if (!_runs[i].Passed)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void Add(EncounterValidationCaseRun run)
        {
            _runs.Add(run);
        }
    }

    public static class EncounterPipelineV02ValidationCaseRunner
    {
        public const string ExternalValidationCasesPath = "/Users/godju/Downloads/외주 폴더/encounter_validation_cases_v0.2.json";

        public static EncounterValidationCaseRunSummary RunFile(string path)
        {
            return RunJson(File.ReadAllText(path));
        }

        public static EncounterValidationCaseRunSummary RunJson(string json)
        {
            var summary = new EncounterValidationCaseRunSummary();
            var parseResult = new EncounterValidationResult();
            object root;
            if (!EncounterPipelineV02Validator.TryParse(json, parseResult, out root))
            {
                summary.Add(new EncounterValidationCaseRun("parse", false, "JSON_SYNTAX_INVALID", parseResult));
                return summary;
            }

            var map = root as Dictionary<string, object>;
            if (map == null || !map.TryGetValue("cases", out var casesValue) || !(casesValue is List<object> cases))
            {
                var validation = new EncounterValidationResult();
                validation.Add("VALIDATION_CASES_INVALID", "$.cases", "cases array is required.");
                summary.Add(new EncounterValidationCaseRun("cases", false, "VALIDATION_CASES_INVALID", validation));
                return summary;
            }

            for (var i = 0; i < cases.Count; i++)
            {
                var caseMap = cases[i] as Dictionary<string, object>;
                if (caseMap == null)
                {
                    var invalidCase = new EncounterValidationResult();
                    invalidCase.Add("VALIDATION_CASE_INVALID", "$.cases[" + i + "]", "case must be an object.");
                    summary.Add(new EncounterValidationCaseRun("case-" + i, false, "VALIDATION_CASE_INVALID", invalidCase));
                    continue;
                }

                var id = GetString(caseMap, "id");
                var targetSchema = GetString(caseMap, "targetSchema");
                var expected = GetString(caseMap, "expected");
                var expectedErrorCode = GetString(caseMap, "expectedErrorCode");
                var path = GetString(caseMap, "path");
                caseMap.TryGetValue("data", out var data);

                var effectiveTargetSchema = ResolveEffectiveTargetSchema(targetSchema, path, data);
                var validation = EncounterPipelineV02Validator.ValidateTargetObject(effectiveTargetSchema, data, string.IsNullOrEmpty(path) ? "$" : path);
                var passed = expected == "Pass"
                    ? validation.IsValid
                    : !validation.IsValid && validation.HasErrorCode(expectedErrorCode);
                summary.Add(new EncounterValidationCaseRun(id, passed, expectedErrorCode, validation));
            }

            return summary;
        }

        private static string GetString(Dictionary<string, object> map, string key)
        {
            return map.TryGetValue(key, out var value) && value != null ? value.ToString() : string.Empty;
        }

        private static string ResolveEffectiveTargetSchema(string targetSchema, string path, object data)
        {
            if (targetSchema == "Encounter" &&
                !string.IsNullOrEmpty(path) &&
                path.Contains("$.choices[") &&
                data is Dictionary<string, object> map &&
                GetString(map, "kind") == "Choice")
            {
                return "Choice";
            }

            return targetSchema;
        }
    }
}
#endif
