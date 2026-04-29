using HwigiTower.LLM;
using HwigiTower.NPC;
using NUnit.Framework;

namespace HwigiTower.Tests.EditMode
{
    public sealed class DeterministicCacheTests
    {
        [Test]
        public void PromptHash_IsStableForSamePrompt()
        {
            var first = PromptHash.FromPrompt("same prompt");
            var second = PromptHash.FromPrompt("same prompt");

            Assert.AreEqual(first, second);
            Assert.AreEqual("66fddd00ccb86fb21cdba02ac7bdda0449cd8c00622ffeaed38f241dadf03e1a", first);
        }

        [Test]
        public void CacheKey_UsesRunIdAndPromptHash()
        {
            var promptHash = PromptHash.FromPrompt("same prompt");
            var first = new DeterministicCacheKey("run-001", promptHash);
            var same = new DeterministicCacheKey("run-001", promptHash);
            var differentRun = new DeterministicCacheKey("run-002", promptHash);
            var differentPrompt = new DeterministicCacheKey("run-001", PromptHash.FromPrompt("other prompt"));

            Assert.AreEqual(first, same);
            Assert.AreNotEqual(first, differentRun);
            Assert.AreNotEqual(first, differentPrompt);
        }

        [Test]
        public void InMemoryRepo_ReturnsCachedResponseForSameRunAndPromptHash()
        {
            var repo = new InMemoryNpcMemoryRepo();
            var request = new LLMRequest("run-001", "same prompt", "profile.prototype");
            var response = new LLMResponse(request.CacheKey, "cached response");

            repo.SaveCachedResponse(response);

            Assert.IsTrue(repo.TryGetCachedResponse(request.CacheKey, out var cached));
            Assert.AreEqual(response.CacheKey, cached.CacheKey);
            Assert.AreEqual("cached response", cached.Text);
        }

        [Test]
        public void InMemoryRepo_DoesNotReturnCacheForDifferentRunId()
        {
            var repo = new InMemoryNpcMemoryRepo();
            var promptHash = PromptHash.FromPrompt("same prompt");
            var savedKey = new DeterministicCacheKey("run-001", promptHash);
            var otherRunKey = new DeterministicCacheKey("run-002", promptHash);

            repo.SaveCachedResponse(new LLMResponse(savedKey, "cached response"));

            Assert.IsFalse(repo.TryGetCachedResponse(otherRunKey, out _));
        }
    }
}
