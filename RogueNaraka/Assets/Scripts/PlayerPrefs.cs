//#define USE_DEBUGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Du3Core;

public class PlayerPrefs : MonoBehaviour {

    public static float GetFloat(string key)
    {
#if USE_DEBUGING
        return UnityEngine.PlayerPrefs.GetFloat(key);
        #else
        return Aes128PlayerPrefs.GetFloat(key);
#endif
    }
    public static int GetInt(string key)
    {
#if USE_DEBUGING
        return UnityEngine.PlayerPrefs.GetInt(key);
#else
        return Aes128PlayerPrefs.GetInt(key);
#endif
    }
    public static string GetString(string key)
    {
#if USE_DEBUGING
        return UnityEngine.PlayerPrefs.GetString(key);
#else
        return Aes128PlayerPrefs.GetString(key);
#endif
    }
    public static void SetFloat(string key, float value)
    {
#if USE_DEBUGING
        UnityEngine.PlayerPrefs.SetFloat(key, value);
#else
        Aes128PlayerPrefs.SetFloat(key, value);
#endif
    }
    public static void SetInt(string key, int value)
    {
#if USE_DEBUGING
        UnityEngine.PlayerPrefs.SetInt(key, value);
#else
        Aes128PlayerPrefs.SetInt(key, value);
#endif
    }
    public static void SetString(string key, string value)
    {
#if USE_DEBUGING
        UnityEngine.PlayerPrefs.SetString(key, value);
#else
        Aes128PlayerPrefs.SetString(key, value);
#endif
    }
    public static void DeleteAll()
    {
#if USE_DEBUGING
        UnityEngine.PlayerPrefs.DeleteAll();
#else
        Aes128PlayerPrefs.DeleteAll();
#endif
    }
}