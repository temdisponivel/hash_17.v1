using Hash17.Blackboard_;
using UnityEditor;
using UnityEngine;

namespace Hash17.Editor
{
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var black = target as Blackboard;
            if (GUILayout.Button("Bake"))
            {
                black.Bake();
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
