using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.SingletonPattern {
    public class Singleton<T> where T : class {
        protected static T instance = null;
        public static T Instance {
            get {
                if(instance == null) {
                    instance = System.Activator.CreateInstance(typeof(T)) as T;
                }
                return instance;
            }
        }

        /// <summary>
        /// 해당 싱글톤 객체를 더이상 사용하지 않는 경우 반드시 해제해주세요. 
        /// </summary>
        public virtual void Destroy() {
            instance = null;
        }
    }
}