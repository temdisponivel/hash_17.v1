using UnityEngine;
using System.Collections;
using Hash17.Utils;

namespace Hash17.Terminal_
{
    public class TextEntry : MonoBehaviour
    {
        public UILabel UserName;
        public UILabel Content;

        void Awake()
        {
            UserName.SetupWithHash17Settings();
            Content.SetupWithHash17Settings();
        }

        public void Setup(string userName, string text)
        {
            UserName.text = userName;
            Content.text = text;
        }
    }
}
