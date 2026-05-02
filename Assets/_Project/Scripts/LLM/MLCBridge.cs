using System;
using System.Runtime.InteropServices;

namespace HwigiTower.LLM
{
    public static class MLCBridge
    {
        private const string LibraryName = "mlc_llm_unity";

        public static bool IsAvailable
        {
            get
            {
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX
                return true;
#else
                return false;
#endif
            }
        }

        public static bool TryComplete(string prompt, int maxOutputTokens, out string text)
        {
            text = string.Empty;

            if (!IsAvailable || string.IsNullOrEmpty(prompt))
            {
                return false;
            }

            try
            {
                var pointer = HwigiMlcComplete(prompt, Math.Max(1, maxOutputTokens));
                if (pointer == IntPtr.Zero)
                {
                    return false;
                }

                text = Marshal.PtrToStringUTF8(pointer) ?? string.Empty;
                return !string.IsNullOrEmpty(text);
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
        }

        [DllImport(LibraryName, EntryPoint = "hwigi_mlc_complete")]
        private static extern IntPtr HwigiMlcComplete(string prompt, int maxOutputTokens);
    }
}
