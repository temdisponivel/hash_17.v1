using UnityEditor;
using UnityEngine;

namespace Hash17.Utils
{
    public class EditorKeyboardShortcuts : ScriptableObject
    {
        [MenuItem("GameObject/Toggle activated %&a")]
        public static void ActivateToggle(MenuCommand menuCommand)
        {
            var objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}