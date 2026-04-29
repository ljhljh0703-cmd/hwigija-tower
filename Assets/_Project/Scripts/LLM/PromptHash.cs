using System.Security.Cryptography;
using System.Text;

namespace HwigiTower.LLM
{
    public static class PromptHash
    {
        public static string FromPrompt(string prompt)
        {
            var bytes = Encoding.UTF8.GetBytes(prompt ?? string.Empty);

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);

                for (var i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
