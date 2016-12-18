using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace FH.Localization
{
	public class LocalizationManager : MonoBehaviour
    {
		protected static LocalizationManager _instance;
		public static LocalizationManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = GameObject.FindObjectOfType<LocalizationManager> () as LocalizationManager;
				return _instance;
			}
		}

		#region Events

		public event Action OnLanguageChanged;

		#endregion

        #region Properies

        public const string PlayerPrefsKey = "system_language";
		public List<SystemLanguage> Languages = new List<SystemLanguage>();

        public bool HasLanguagePref
        {
            get { return PlayerPrefs.HasKey(PlayerPrefsKey); }
        }

        public SystemLanguage Language
        {
            get
            {
				return (SystemLanguage)PlayerPrefs.GetInt(PlayerPrefsKey);
            }
        }

        public static string Path(string language)
        {
            return string.Format("{0}{1}{2}{1}{3}", PathPrefix, System.IO.Path.DirectorySeparatorChar, language, "Data");
        }

        public bool HasLaguage(SystemLanguage language)
        {
			return Languages.Contains (language);
        }

        #endregion

        #region Paths

        public const string PathPrefix = "Languages";

        #endregion

        #region Dictionaries

        public Dictionary<string, string> Data = new Dictionary<string, string>();

		//TODO: Esse metodo era usado pra trocar de fonte com base na lingua, pq arabe precisa de outra fonte
		public void SetData(UILabel label, string key)
		{
			//label.trueTypeFont = ScriptableObjectHolder.Instance.GameConfiguration.CurrentFont;
			label.text = Data [key];
		}

		public string GetData(string key)
		{
			return Data [key];
		}

        #endregion

        #region Unity events

		public SystemLanguage ForcedLanguage = SystemLanguage.Unknown;

        void Awake()
        {
			if (ForcedLanguage != SystemLanguage.Unknown && HasLaguage(ForcedLanguage))
				ChangeLanguage (ForcedLanguage);
			else
				ChangeLanguage (Application.systemLanguage);
			
			Load ();
        }

        public void ChangeLanguage(SystemLanguage language)
        {
			if (HasLaguage(language))
                PlayerPrefs.SetInt(PlayerPrefsKey, (int)language);
            else
                PlayerPrefs.SetInt(PlayerPrefsKey, (int)SystemLanguage.English);

			Load();

			if (OnLanguageChanged != null)
				OnLanguageChanged ();
        }

        #endregion

        #region Load

        public void Load()
        {
            InnerLoad(Language.ToString(), fileContent =>
            {
                if (string.IsNullOrEmpty(fileContent))
                    return;

                Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
				//ScriptableObjectHolder.Instance.GameConfiguration.SetCurrentFontByLanguage(Language);
            });
        }

        private void InnerLoad(string language, Action<string> callback)
        {
            var textAsset = Resources.Load<TextAsset>(Path(language));
			callback(textAsset.text);
        }

        #endregion
    }
}
