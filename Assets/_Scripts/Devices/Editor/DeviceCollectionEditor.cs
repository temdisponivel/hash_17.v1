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

        DrawList(deviceCollection);

        serializedObject.ApplyModifiedProperties();
    }

    public void DrawList(DeviceCollectionScriptableObject deviceCollection)
    {
        var toRemove = new List<int>();
        if (NGUIEditorTools.DrawHeader("Devices"))
        {
            NGUIEditorTools.BeginContents();
            for (int i = 0; i < deviceCollection.Devices.Count; i++)
            {
                var device = deviceCollection.Devices[i];
                if (NGUIEditorTools.DrawHeader(device.Name))
                {
                    NGUIEditorTools.BeginContents();
                    device.DrawDeviceInspector();

                    if (GUILayout.Button("Remove"))
                    {
                        toRemove.Add(i);
                    }
                    NGUIEditorTools.EndContents();
                }
            }
            NGUIEditorTools.EndContents();
        }

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

        for (int i = 0; i < toRemove.Count; i++)
        {
            deviceCollection.Devices.RemoveAt(toRemove[i]);
        }
    }
}
