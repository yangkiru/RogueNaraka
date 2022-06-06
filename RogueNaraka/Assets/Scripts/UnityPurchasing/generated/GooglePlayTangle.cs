// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1qdoEtdNgZVuHegFlbf0fIi0YBUhbAAe5VExfl0gcMHfTcl0EKHkMQwrJBGDwH7H444JgRsVmThc1HQeIqGvoJAioaqiIqGhoDlLPSooTQzeM9Fj7s3tuiQ9QMPpbw+l+uHy87tJLaa9BMDbp++pYvubcBu4zyYhkCKhgpCtpqmKJugmV62hoaGloKNKidgwYRfL/WhPvbAv6GxDbzKdbVzwxdeo7Q5kU+9qJy1eMqUYtGWnkitWzwcSejjcnops2W8yIiV7oHwWKn9YO7tUK+Ml4J8yqBJ1lRI2tvoWo8N6bv8nPyCvNWtcHdroAj4cKhpZAA9qUNt8ppeJbY9gb+Mu9ITD7J3iiYbNmNbxIbjapWXPxDPjbCFcNytU3OZKCaKjoaCh");
        private static int[] order = new int[] { 2,11,6,11,10,12,6,12,13,10,13,11,12,13,14 };
        private static int key = 160;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
