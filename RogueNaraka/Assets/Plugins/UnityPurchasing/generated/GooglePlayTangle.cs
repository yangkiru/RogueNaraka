#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("cGaumnE7Ju+mW+mI7S/I0oejfcmgEpGyoJ2WmboW2BZnnZGRkZWQk8Ar0PUsD/4MavW9rt5rf1JU0G8OK8jo8N93e/vC6HsIqO5PhaZv8TOL0Z3DksRPPDoceDksBWBV1Q8ZoxKRn5CgEpGakhKRkZAO7etmoY+4EaONDbKoGhzS0yqhZGPeLVSIbMP8Xmfa7jmEff/vR4pz+gQaky1Ge0HvVrZuNpVeUUkONghLaQg4ahl/wcRnhVJcFgkxqWkLCiegw4P0k22QDi50nNI6E/jFN1azIhIaV4MIKd/eJYAgoT45EU1hJ1G5QHn3f+WjTuxuG9LZuwggznZ2PO3ndsW9NfOmpk6RxkGGiYNZKVsWeGy55N/6bG8foEiNH+iIw5KTkZCR");
        private static int[] order = new int[] { 13,13,4,12,7,13,13,9,9,9,12,11,12,13,14 };
        private static int key = 144;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
