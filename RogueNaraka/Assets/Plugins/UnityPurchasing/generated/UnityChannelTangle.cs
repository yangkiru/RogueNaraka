#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class UnityChannelTangle
    {
        private static byte[] data = System.Convert.FromBase64String("xooAWTG1tVMgds7uJb8gA2yXVCeNdKytXPkXH+6+GNg+Z3O9MoH01T/dS/f99XIP6aMGgGEXPJ1nfS/SiiyzIhTPjywYZkfQxwTT6EU8oV5LHLdvGqT5cwXbHzY0EPQB5zpp6DF8AsalexIEqFXHdSgN78zrwBsekccO58rvRE4RuFw5/pcDWALN27p5+vT7y3n68fl5+vr7UtoxvVB9IIIWWpL8m3hK9M/voVgM2kFbU0fzzhDCioe2z922V5dtjj40CXPu4K0mb7+4B0RJIO+BuTEaw7p6ysxQrk+BL09fVNpdZNCs7iHYTSZ+7qLOEys98SiO+UdU2psQT5rRDEMst7/LefrZy/b98tF9s30M9vr6+v77+F0KZoQwbv3ITPn4+vv6");
        private static int[] order = new int[] { 7,13,10,4,12,9,11,13,8,12,11,12,12,13,14 };
        private static int key = 251;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
