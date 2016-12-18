using UnityEngine;
using System.Collections;

namespace FH.UI.StateMachine
{
	public class UILoadingHandler : MonoBehaviour
	{
		public UIWidget Widget;
		private GameObject _gameObject;

		private void Awake()
		{
			Widget.alpha = 0;
			_gameObject = gameObject;
			_gameObject.SetActive (false);
		}

		public void StartLoading()
		{
			if (_gameObject == null)
				_gameObject = gameObject;
			
			_gameObject.SetActive (true);
			TweenAlpha.Begin (_gameObject, 0.2f, 1);
		}

		public void FinishLoading()
		{
			if (_gameObject == null)
				_gameObject = gameObject;

			if (_gameObject.activeInHierarchy)
				StartCoroutine (RunFinishLoading ());
		}

		private IEnumerator RunFinishLoading()
		{
			TweenAlpha.Begin (_gameObject, 0.2f, 0);
			yield return new WaitForSeconds (0.2f);
			_gameObject.SetActive (false);
		}
	}
}