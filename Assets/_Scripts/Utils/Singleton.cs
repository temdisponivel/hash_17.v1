using UnityEngine;

namespace Hash17.Utils
{
    public class Singleton<T> : MonoBehaviour
        where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                object locker = new object();

                lock (locker)
                {
                    if (_instance == null)
                    {
                        
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            _instance = new GameObject(typeof(T).Name + "SINGLETON").AddComponent<T>();
                        }
                    }

                    return _instance;
                }
            }
        }
    }
}
