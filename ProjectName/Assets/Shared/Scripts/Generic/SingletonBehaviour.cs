using System.Collections;
using System.Collections.Generic;
using Assets.DebugApplication;
using UnityEngine;

namespace Assets.Generic
{
    /// <summary>
    /// Implement this class in order to turn any class into a Singleton Monobehaviour
    /// </summary>
    /// <typeparam name="T">The class that will extend this.</typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : class
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    //DebugConsole.Instance.Log(string.Format("{0} is non existing.", typeof(T).ToString()));
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else{
                Destroy(this.gameObject);
                DebugConsole.Instance.Log(string.Format("{0} singleton is repeated...", typeof(T).ToString()));
                return;
            }
        }

        public void DestroyInstances(){
            Destroy(this.gameObject);
        }


        private void OnDestroy() {
            if(this as T == instance)
                instance = null;
        }
    }
}