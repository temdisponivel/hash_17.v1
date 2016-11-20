using UnityEngine;
using System.Collections;

namespace Hash17.Terminal_
{
    public class TextEntry : MonoBehaviour
    {
        public UILabel UserName;
        public UILabel Content;

        public void Setup(string userName, string text)
        {
            UserName.text = userName;
            Content.text = text;
        }
    }
}
