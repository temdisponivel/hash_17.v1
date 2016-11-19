using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InputHelper))]
public class InputHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var inputHelper = target as InputHelper;

        NGUIEditorTools.DrawProperty("Key code", serializedObject, "KeyCode");
        NGUIEditorTools.DrawProperty("Type", serializedObject, "Type");

        NGUIEditorTools.DrawEvents("Notify", inputHelper, inputHelper.Notify);

        serializedObject.ApplyModifiedProperties();
    }
}
