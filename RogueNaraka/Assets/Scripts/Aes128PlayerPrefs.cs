//출처 : https://blog.naver.com/inus_7373/220987000764
//#define USE_DEBUGING

using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.IO;


// 자동 키 생성기 부분
// https://asecuritysite.com/encryption/keygen
// http://www.hipenpal.com/tool/automatic-random-password-generator-in-korean.php
namespace Du3Core
{
    public class Aes128PlayerPrefs
    {
        // 사용하지않음
        public static string NickNameAESKey = "n5a9y0u0t2a2soft";


        // 키는 마음대로 늘리면됨
        public static string[] AESKeys = new string[]{
                          "a1c2eB1[U%&.mnRp"
                        , "3L=He@Eh^j05mnRp"
                        , "A!P61*ghU]klmnR|"
                        , "}LPdefEhUj)l8ARp"
                        , "abcdAf$hU8&lmnR7"
                        , "a/cHABg2i$[*m[Rp"
                        , "ab!dfg1iP7lmnRRz"
                        , "A4|deBEhiPklRnop"
                        , "#bPdA75h8PP.@*op"
                        , "abcHe1g'*P_Emno7"};
        //aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static bool ISOnlyDebugUse = false;


        public static bool ISAes()
        {
#if USE_DEBUGING
            return false;
#else
            return true;
#endif
        }


        static RijndaelManaged m_Aes = new RijndaelManaged();
        protected static RijndaelManaged GetAESKeyInput(string p_aesKeyval)
        {
            m_Aes.BlockSize = 128;
            m_Aes.KeySize = 128;
            m_Aes.Padding = PaddingMode.PKCS7;
            m_Aes.Mode = CipherMode.ECB;
            m_Aes.Key = System.Text.Encoding.UTF8.GetBytes(p_aesKeyval);

            return m_Aes;
        }
        protected static RijndaelManaged GetAES(string p_inval)
        {
            m_Aes.BlockSize = 128;
            m_Aes.KeySize = 128;
            m_Aes.Padding = PaddingMode.Zeros;
            m_Aes.Mode = CipherMode.ECB;
            m_Aes.Key = System.Text.Encoding.UTF8.GetBytes(GetAESKeyVal(p_inval));

            return m_Aes;
        }
        protected static RijndaelManaged GetAES(int p_aeskeyval)
        {
            m_Aes.BlockSize = 128;
            m_Aes.KeySize = 128;
            // PaddingMode.PKCS7 방식은 위에 AESKeys 8바이트가 안되었을때 빈자리를 채우기위한 방식임
            m_Aes.Padding = PaddingMode.Zeros;
            m_Aes.Mode = CipherMode.ECB;
            m_Aes.Key = System.Text.Encoding.UTF8.GetBytes(GetAESKeyVal(p_aeskeyval));

            return m_Aes;
        }


        protected static string GetAESKeyVal(string p_inval)
        {
            int index = p_inval.Length % AESKeys.Length;
            return AESKeys[index];
        }
        protected static string GetAESKeyVal(int p_index)
        {
            int index = p_index % AESKeys.Length;
            return AESKeys[index];
        }

        /// <summary>
        /// 복호화
        /// </summary>
        /// <param name="p_val"></param>
        /// <param name="p_aeskeystr"></param>
        /// <returns></returns>
        public static string GetAESDecryptorValue(string p_val, string p_aeskeystr)
        {
            RijndaelManaged aesval = GetAESKeyInput(p_aeskeystr);
            ICryptoTransform decryptor = aesval.CreateDecryptor();

            byte[] encrypted = System.Convert.FromBase64String(p_val);
            byte[] planeText = new byte[encrypted.Length];

            MemoryStream memoryStream = new MemoryStream(encrypted);
            CryptoStream cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            cryptStream.Read(planeText, 0, planeText.Length);

            return (System.Text.Encoding.UTF8.GetString(planeText));
        }
        /// <summary>
        /// 암호화
        /// </summary>
        /// <param name="p_val"></param>
        /// <param name="p_aeskeystr"></param>
        /// <returns></returns>
        public static string GetAESEncryptorAesValue(string p_val, string p_aeskeystr)
        {
            RijndaelManaged aesval = GetAESKeyInput(p_aeskeystr);
            ICryptoTransform encrypt = aesval.CreateEncryptor();

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
            byte[] text_bytes = System.Text.Encoding.UTF8.GetBytes(p_val);
            cryptStream.Write(text_bytes, 0, text_bytes.Length);
            cryptStream.FlushFinalBlock();

            byte[] encrypted = memoryStream.ToArray();

            return System.Convert.ToBase64String(encrypted);
        }

        protected static string GetAESEncryptorAesKey(string p_inval, string p_aeskeystr)
        {
            RijndaelManaged aesval = GetAESKeyInput(p_aeskeystr);
            ICryptoTransform encrypt = aesval.CreateEncryptor();

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
            byte[] text_bytes = System.Text.Encoding.UTF8.GetBytes(p_inval);
            cryptStream.Write(text_bytes, 0, text_bytes.Length);
            cryptStream.FlushFinalBlock();

            byte[] encrypted = memoryStream.ToArray();

            return System.Convert.ToBase64String(encrypted);
        }

        protected static string GetAESEncryptor(string p_inval, int p_seedval)
        {
            RijndaelManaged aesval = GetAES(p_seedval);
            ICryptoTransform encrypt = aesval.CreateEncryptor();

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
            byte[] text_bytes = System.Text.Encoding.UTF8.GetBytes(p_inval);
            cryptStream.Write(text_bytes, 0, text_bytes.Length);
            cryptStream.FlushFinalBlock();

            byte[] encrypted = memoryStream.ToArray();

            return System.Convert.ToBase64String(encrypted);
        }

        protected static string GetAESDecryptor(string p_inval, int p_seedval)
        {
            RijndaelManaged aesval = GetAES(p_seedval);
            ICryptoTransform decryptor = aesval.CreateDecryptor();

            byte[] encrypted = System.Convert.FromBase64String(p_inval);
            byte[] planeText = new byte[encrypted.Length];

            MemoryStream memoryStream = new MemoryStream(encrypted);
            CryptoStream cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            cryptStream.Read(planeText, 0, planeText.Length);

            return (System.Text.Encoding.UTF8.GetString(planeText));
        }

        public static void SavePlayerPrefsAesKey(string p_key, string p_val, string p_aeskey)
        {

            // #if USE_DEBUGING
            //             PlayerPrefs.SetString(p_key, p_val);
            // #else
            string keyval;
            string val;

            if (!ISAes())
            {
                keyval = p_key;
                val = p_val;
            }
            else
            {
                keyval = GetAESEncryptorAesKey(p_key, p_aeskey);
                val = GetAESEncryptorAesKey(p_val, p_aeskey);
            }


            UnityEngine.PlayerPrefs.SetString(keyval, val);
            //#endif


        }

        public static void SavePlayerPrefs(string p_key, string p_val)
        {

            // #if USE_DEBUGING
            //             UnityEngine.PlayerPrefs.SetString(p_key, p_val);
            // #else
            string keyval;
            string val;
            if (!ISAes())
            {
                keyval = p_key;
                val = p_val;
            }
            else
            {
                keyval = GetAESEncryptor(p_key, p_key.Length);
                val = GetAESEncryptor(p_val, p_key.Length);
            }
            UnityEngine.PlayerPrefs.SetString(keyval, val);
            //#endif
        }


        public static void SetBool(string p_key, bool p_val)
        {
            // #if USE_DEBUGING
            //             UnityEngine.PlayerPrefs.SetInt(p_key, p_val ? 1 : 0);
            // #else
            int val = p_val ? 1 : 0;
            SavePlayerPrefs(p_key, val.ToString());
            //#endif
        }

        public static void SetInt(string p_key, int p_val)
        {
            // #if USE_DEBUGING
            //             UnityEngine.PlayerPrefs.SetInt( p_key, p_val );
            // #else
            SavePlayerPrefs(p_key, p_val.ToString());
            //#endif
        }
        public static void SetFloat(string p_key, float p_val)
        {
            // #if USE_DEBUGING
            //             UnityEngine.PlayerPrefs.SetFloat(p_key, p_val);
            // #else
            SavePlayerPrefs(p_key, p_val.ToString());
            //#endif

        }
        public static void SetString(string p_key, string p_val)
        {
            // #if USE_DEBUGING
            //             UnityEngine.PlayerPrefs.SetString(p_key, p_val);
            // #else
            SavePlayerPrefs(p_key, p_val);
            //#endif

        }

        public static void SetStringAesKey(string p_key, string p_val, string p_aeskey)
        {
            // #if USE_DEBUGING
            //             Debug.LogError("Null Data :");
            //             UnityEngine.PlayerPrefs.SetString(p_key, p_val);
            // #else
            SavePlayerPrefsAesKey(p_key, p_val, p_aeskey);
            //#endif


        }



        public static bool HasKey(string p_key)
        {

            // #if USE_DEBUGING
            //             return UnityEngine.PlayerPrefs.HasKey(p_key);
            // #else
            if (!ISAes())
            {
                m_key = p_key;
            }
            else
            {
                m_key = GetAESEncryptor(p_key, p_key.Length);
            }

            return UnityEngine.PlayerPrefs.HasKey(m_key);
            //#endif
        }

        static string m_key = "";
        static string m_Value = "";

        public static bool GetBool(string p_key, bool p_defaultoutval = false)
        {
            string outval = "";
            if (LoadPlayerPrefs(p_key, ref outval))
            {
                try
                {
                    return outval == "1" ? true : false;
                }
                catch (System.Exception ex)
                {
                    return p_defaultoutval;
                }

            }
            return p_defaultoutval;
        }

        public static int GetInt(string p_key, int p_defaultoutval = 0)
        {
            string outval = "";
            if (LoadPlayerPrefs(p_key, ref outval))
            {
                try
                {
                    return int.Parse(outval);
                }
                catch (System.Exception ex)
                {
                    return p_defaultoutval;
                }

            }
            return p_defaultoutval;
        }
        public static float GetFloat(string p_key, float p_defaultoutval = 0f)
        {
            string outval = "";
            if (LoadPlayerPrefs(p_key, ref outval))
            {
                try
                {
                    return float.Parse(outval);
                }
                catch (System.Exception ex)
                {
                    return p_defaultoutval;
                }

            }
            return p_defaultoutval;
        }

        public static string GetStringAesKey(string p_key, string p_aeskey, string p_defaultoutval = "")
        {
            // #if USE_DEBUGING
            // 
            //             m_Value = UnityEngine.PlayerPrefs.GetString(p_key);
            // 
            // #else

            if (!ISAes())
            {
                m_key = p_key;
            }
            else
            {
                m_key = GetAESEncryptorAesKey(p_key, p_aeskey);
            }


            if (!UnityEngine.PlayerPrefs.HasKey(m_key))
                return p_defaultoutval;


            m_Value = UnityEngine.PlayerPrefs.GetString(m_key);

            if (!ISAes())
            {

            }
            else
            {
                m_Value = GetAESEncryptorAesKey(m_Value, p_aeskey);
            }
            //#endif

            return m_Value;
        }
        public static string GetString(string p_key, string p_defaultoutval = "")
        {

            // #if USE_DEBUGING
            //             m_Value = UnityEngine.PlayerPrefs.GetString(p_key);
            // 
            // #else
            if (!ISAes())
            {
                m_key = p_key;
            }
            else
            {
                m_key = GetAESEncryptor(p_key, p_key.Length);
            }


            if (!UnityEngine.PlayerPrefs.HasKey(m_key))
                return p_defaultoutval;


            m_Value = UnityEngine.PlayerPrefs.GetString(m_key);

            if (!ISAes())
            {

            }
            else
            {
                m_Value = GetAESDecryptor(m_Value, p_key.Length);
            }
            //#endif
            return m_Value;
        }

        protected static bool LoadPlayerPrefs(string p_key, ref string p_defaultoutval)
        {

            // #if USE_DEBUGING
            //             p_defaultoutval = PlayerPrefs.GetString(p_key);
            // 
            // #else
            if (!ISAes())
            {
                m_key = p_key;
            }
            else
            {
                m_key = GetAESEncryptor(p_key, p_key.Length);
            }


            if (!UnityEngine.PlayerPrefs.HasKey(m_key))
                return false;


            p_defaultoutval = UnityEngine.PlayerPrefs.GetString(m_key);

            if (!ISAes())
            {

            }
            else
            {
                p_defaultoutval = GetAESDecryptor(p_defaultoutval, p_key.Length);
            }
            //#endif



            return true;
        }

        public static string LoadPlayerPrefs(string p_key, string p_defaultoutval = "")
        {
            // #if USE_DEBUGING
            //             m_Value = UnityEngine.PlayerPrefs.GetString(p_key);
            // #else

            if (!ISAes())
            {
                m_key = p_key;
            }
            else
            {
                m_key = GetAESEncryptor(p_key, p_key.Length);
            }


            if (!UnityEngine.PlayerPrefs.HasKey(m_key))
                return p_defaultoutval;

            m_Value = UnityEngine.PlayerPrefs.GetString(m_key);

            if (!ISAes())
            {

            }
            else
            {
                m_Value = GetAESDecryptor(m_Value, p_key.Length);
            }
            //#endif

            return m_Value;
        }

        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }
    }
}

