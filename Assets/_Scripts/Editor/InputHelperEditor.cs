using UnityEngine;
using System.Collections;
using Hash17.Terminal_;
using UnityEditor;

[CustomEditor(typeof(InputHelper))]
public class InputHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var inputHelper = target as InputHelper;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("KeyCode"), true);
        NGUIEditorTools.DrawProperty("Type", serializedObject, "Type");

        NGUIEditorTools.DrawEvents("Notify", inputHelper, inputHelper.Notify);

        serializedObject.ApplyModifiedProperties();
    }
}
