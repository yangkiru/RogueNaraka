// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("wDJW3cZ/u6DclNIZgOALYMO0XVptUQQjQMAvUJhem+RJ02kO7mlNzVFhInt0ESugB93s8hb0GxSYVY//uJfmmfL9tuOtilrDod4etL9ImBd3UF9q+LsFvJj1cvpgbuJDJ68PZYFt2LgBFYRcRFvUThAnZqGTeUVnWdrU2+tZ2tHZWdra20IwRlFTNnet3BNprDb67hVmk37uzI8H888bbqVIqhiVtpbBX0Y7uJIUdN6BmomI6VAttHxpAUOn5fEXohRJWV4A2wcx8qNLGmywhhM0xstUkxc4FEnmFloXe2WeKkoFJlsLuqQ2sg9r2p9KJ4u+rNOWdR8olBFcViVJ3mPPHtzrWdr569bd0vFdk10s1tra2t7b2FonTFAvp50xctnY2tva");
        private static int[] order = new int[] { 12,8,7,9,6,9,8,7,10,10,10,11,13,13,14 };
        private static int key = 219;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
