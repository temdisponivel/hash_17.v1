using UnityEngine;
using System.Collections;
using Hash17.Utils;

namespace MockSystem.Term
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

        public void Setup(string userName, string text, Transform parent)
        {
            UserName.text = userName;
            Content.text = text;
            Content.rightAnchor.target = parent;
        }
    }
}
