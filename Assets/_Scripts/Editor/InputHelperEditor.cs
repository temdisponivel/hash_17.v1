using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hash17.Terminal_;
using Hash17.Utils;
using UnityEditor;

[CustomEditor(typeof(InputHelper))]
public class InputHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var inputHelper = target as InputHelper;
        
        GUILayout.Label("Events");

        NGUIEditorTools.BeginContents();

        if (GUILayout.Button("Add event"))
        {
            inputHelper.EventList.Add(new InputHelper.TupleEvents());
        }

        for (int i = 0; i < inputHelper.EventList.Count; i++)
        {
            var current = inputHelper.EventList[i];

            NGUIEditorTools.BeginContents();

            if (NGUIEditorTools.DrawHeader("Event " + i))
            {
                NGUIEditorTools.BeginContents();
                current.Key.Key = (KeyCode) EditorGUILayout.EnumPopup("Key code", current.Key.Key);
                current.Key.Value = (InputHelper.InputType) EditorGUILayout.EnumPopup("Event type", current.Key.Value);

                NGUIEditorTools.DrawEvents("Notify", inputHelper, current.Value);

                NGUIEditorTools.EndContents();
                
                if (GUILayout.Button("Remove"))
                {
                    inputHelper.EventList.Remove(current);
                    continue;
                }
            }

            NGUIEditorTools.EndContents();
        }

        NGUIEditorTools.EndContents();

        serializedObject.ApplyModifiedProperties();
    }
}
