using UnityEngine;
using System.Collections;
using System.Security.AccessControl;
using Hash17.Blackboard_;
using Hash17.Files;
using Hash17.Programs;
using UnityEditor;

namespace Hash17.Blackboard_
{
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
