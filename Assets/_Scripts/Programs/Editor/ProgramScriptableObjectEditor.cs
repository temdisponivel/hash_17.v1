using System.ComponentModel;
using UnityEditor;

namespace Hash17.Programs.Editor
{
    [CustomEditor(typeof(ProgramScriptableObject))]
    public class ProgramScriptableObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var program = target as ProgramScriptableObject;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Id"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Command"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PrefabPath"));

            EditorGUILayout.LabelField("Description");
            program.Description = EditorGUILayout.TextArea(program.Description);
            EditorGUILayout.LabelField("Usage");
            program.Usage = EditorGUILayout.TextArea(program.Usage);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("KnownParametersAndOptions"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}