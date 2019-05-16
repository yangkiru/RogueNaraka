using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.SingletonPattern {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        protected static T instance;
        public static T Instance {
            get {
                if(instance == null) {
                    instance = FindObjectOfType (typeof(T)) as T;
                    if(instance == null) {
                        instance = new GameObject("@" + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        Debug.Log(string.Format("Create MonoSingleton Instance! Component Name : {0}", instance.name));
                    }
                    //DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }

        /// <summary>
        /// 해당 싱글톤 객체를 더이상 사용하지 않는 경우 반드시 해제해주세요. 
        /// </summary>
        public virtual void OnDestroy() {
            Destroy(instance);
            instance = null;
            Resources.UnloadUnusedAssets();
        }
    }
}