using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hash17.Utils
{
    [ExecuteInEditMode]
    public class CoroutineHelper : PersistentSingleton<CoroutineHelper>
    {
        public Coroutine WaitAndCall(Action callback, float secondsToWait)
        {
            return ((MonoBehaviour) this).StartCoroutine(InnerWaitAndCall(callback, secondsToWait));
        }

        private IEnumerator InnerWaitAndCall(Action callback, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (callback != null)
                callback();
        }

        public Coroutine WaitAndCallTimes(Action<int> callback, int timesToCall, float interval, Action finishCallback)
        {
            return ((MonoBehaviour) this).StartCoroutine(InnerWaitAndCallTimes(callback, timesToCall, interval, finishCallback));
        }

        private IEnumerator InnerWaitAndCallTimes(Action<int> callback, int timestoCall, float interval, Action finishCallback)
        {
            for (int i = 0; i < timestoCall; i++)
            {
                yield return new WaitForSeconds(interval);
                if (callback != null)
                    callback(i);
            }

            if (finishCallback != null)
                finishCallback();
        }

    }
}
