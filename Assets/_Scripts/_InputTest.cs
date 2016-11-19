using UnityEngine;
using System.Collections;

public class _InputTest : MonoBehaviour
{
    public GameObject LabelPrefab;
    public UITable TextTable;
    public UIInput Input;
    public UILabel UserName;
    
    public void OnSubmit()
    {
        var newText = NGUITools.AddChild(TextTable.gameObject, LabelPrefab);
        newText.transform.SetAsFirstSibling();
        var inputText = Input.value.Replace("\n", string.Empty);
        newText.GetComponent<TextEntry>().Setup(UserName.text, inputText);
        TextTable.Reposition();

        Input.value = string.Empty;
    }
}
