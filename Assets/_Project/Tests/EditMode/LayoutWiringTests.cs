using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace HwigiTower.Tests.EditMode
{
    public sealed class LayoutWiringTests
    {
        private static readonly Regex LayoutDeclaration = new Regex(@"public\s+static\s+class\s+(\w+Layout)\b");
        private static readonly Regex SlotDeclaration = new Regex("public\\s+static\\s+readonly\\s+\\w+Slot\\s+(\\w+)\\s*=\\s*(?:new\\s+\\w+Slot\\s*\\(|Slot\\s*\\()\\s*\"[^\"]+\"", RegexOptions.Singleline);

        [Test]
        public void RuntimeLayoutContractsHaveAFullRuntimeWiringPath()
        {
            var scriptsRoot = Path.Combine(Application.dataPath, "_Project", "Scripts");
            var files = Directory.GetFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories)
                .Where(path => path.IndexOf(Path.DirectorySeparatorChar + "Tests" + Path.DirectorySeparatorChar, System.StringComparison.OrdinalIgnoreCase) < 0)
                .ToArray();
            var contracts = new List<(string TypeName, string Path, MatchCollection Slots)>();

            for (var i = 0; i < files.Length; i++)
            {
                var text = File.ReadAllText(files[i]);
                var type = LayoutDeclaration.Match(text);
                if (!type.Success)
                {
                    continue;
                }

                contracts.Add((type.Groups[1].Value, files[i], SlotDeclaration.Matches(text)));
            }

            Assert.IsNotEmpty(contracts, "No layout contracts were discovered.");
            for (var i = 0; i < contracts.Count; i++)
            {
                var contract = contracts[i];
                Assert.IsNotEmpty(contract.Slots, contract.TypeName + " slot declarations were not understood by the wiring test.");

                var referrers = new List<string>();
                for (var f = 0; f < files.Length; f++)
                {
                    if (files[f] == contract.Path)
                    {
                        continue;
                    }

                    var text = File.ReadAllText(files[f]);
                    if (Regex.IsMatch(text, @"\b" + Regex.Escape(contract.TypeName) + @"\s*\."))
                    {
                        referrers.Add(files[f]);
                    }
                }

                Assert.IsNotEmpty(referrers, contract.TypeName + " W-01: no non-test runtime referrer.");
                var referrerText = string.Join("\n", referrers.Select(File.ReadAllText));
                var unwired = new List<string>();
                for (var s = 0; s < contract.Slots.Count; s++)
                {
                    var member = contract.Slots[s].Groups[1].Value;
                    if (!Regex.IsMatch(referrerText, @"\b" + Regex.Escape(contract.TypeName) + @"\s*\.\s*" + Regex.Escape(member) + @"\b"))
                    {
                        unwired.Add(member);
                    }
                }

                Assert.IsEmpty(unwired, contract.TypeName + " W-02: unwired slots: " + string.Join(", ", unwired));
            }
        }
    }
}
