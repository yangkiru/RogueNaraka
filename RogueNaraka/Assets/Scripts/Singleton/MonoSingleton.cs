using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.Singleton {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        protected static T instance;
        public static T Instance {
            get {
                instance = FindObjectOfType (typeof(T)) as T;

                if(instance == null) {
                    instance = new GameObject("@" + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    DontDestroyOnLoad(instance);
                    Debug.Log(string.Format("Create MonoSingleton Instance! Component Name : {0}", instance.name));
                }

                return instance;
            }
        }

        public virtual void OnDestroy() {
            Destroy(instance);
            instance = null;
            Resources.UnloadUnusedAssets();
        }
    }
}