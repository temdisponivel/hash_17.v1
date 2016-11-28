using UnityEngine;
using System.Collections;
using System.Security.AccessControl;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Files.SO;
using Hash17.Programs;
using UnityEditor;

namespace Hash17.Blackboard_
{
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Bake"))
            {
                serializedObject.Update();
                var blackboard = target as Blackboard;
                blackboard.FileSystemScriptableObject = Resources.LoadAll<FileSystemScriptableObject>("")[0];
                blackboard.ProgramsScriptableObjects = Resources.LoadAll<ProgramScriptableObject>("");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
