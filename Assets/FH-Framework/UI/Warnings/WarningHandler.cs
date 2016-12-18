using System.Collections;
using UnityEngine;
using FH.UI.Animations;

namespace FH.UI.Warnings
{
    public class WarningHandler : MonoBehaviour
    {
        #region Singleton

        private static WarningHandler _instance;
        public static WarningHandler Instance
        {
            get { return _instance ?? (_instance = FindObjectOfType<WarningHandler>()); }
        }

        #endregion

		public GameObject Holder;
        public UILabel Label;
        public UIAnimation Animation;

        private float _duration;

        public void Warning(string text, float duration = 3f)
        {
            StopAllCoroutines();
            _duration = duration;

			Holder.SetActive(true);

			Label.text = text;
			Animation.StartAnimation();
			Animation.OnCompleteAnimation += CountDown;
        }

		private void CountDown(UIAnimation anim)
        {
			Animation.OnCompleteAnimation -= CountDown;
            StartCoroutine(CountDown(_duration));
        }

        private IEnumerator CountDown(float duration)
        {
            yield return new WaitForSeconds(duration);
			Animation.StartAnimation(true);
			Animation.OnCompleteAnimation += DisableInterface;
        }

		private void DisableInterface(UIAnimation anim)
        {
			Animation.OnCompleteAnimation -= DisableInterface;
			Holder.gameObject.SetActive(false);
        }
    }
}