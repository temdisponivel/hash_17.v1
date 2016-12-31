using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hash17.Devices;
using UnityEditor;
using DeviceType = Hash17.Devices.DeviceType;

[CustomEditor(typeof(DeviceCollectionScriptableObject))]
public class DeviceCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var deviceCollection = target as DeviceCollectionScriptableObject;

        if (deviceCollection.Devices == null)
            deviceCollection.Devices = new List<Device>();

        DrawDefaultInspector();

        if (GUILayout.Button("Add device"))
        {
            var type = (DeviceType)EditorGUILayout.EnumPopup("Type", DeviceType.Normal);
            switch (type)
            {
                case DeviceType.Normal:
                    deviceCollection.Devices.Add(new Device());
                    break;
                case DeviceType.Passworded:
                    deviceCollection.Devices.Add(new PasswordedDevice());
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
