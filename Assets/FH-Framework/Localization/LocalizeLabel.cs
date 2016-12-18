using UnityEngine;
using System.Collections;

namespace FH.Localization
{
	public class LocalizeLabel : MonoBehaviour
	{
		public string DesiredKey;
		private UILabel _label;

		private void Awake ()
		{
			_label = GetComponent<UILabel> ();
		}

		private void OnEnable()
		{
			LocalizationManager.Instance.SetData (_label, DesiredKey);
			LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
		}

		private void OnDisable()
		{
			LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
		}

		private void OnLanguageChanged ()
		{
			LocalizationManager.Instance.SetData (_label, DesiredKey);
		}
	}
}