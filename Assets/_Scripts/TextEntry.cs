using UnityEngine;
using System.Collections;

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
