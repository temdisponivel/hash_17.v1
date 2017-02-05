using UnityEngine;

namespace Hash17.Utils
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}