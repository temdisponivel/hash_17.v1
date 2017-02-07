using Hash17.Data;
using UnityEditor;
using UnityEngine;

namespace Hash17.Editor
{
    [CustomEditor(typeof(DataHolder))]
    public class DataHolderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var black = target as DataHolder;
            if (GUILayout.Button("Bake"))
            {
                black.Bake();
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
