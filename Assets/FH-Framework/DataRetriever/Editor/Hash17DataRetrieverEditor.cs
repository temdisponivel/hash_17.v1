﻿using UnityEngine;
using System.Collections;
using FH.DataRetrieving;
using Hash17.Utils;
using UnityEditor;

[CustomEditor(typeof(Hash17DataRetriever))]
public class Hash17DataRetrieverEditor : DataRetrieverBaseEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var config = (Hash17DataRetriever)target;

        if (NGUIEditorTools.DrawHeader("Programs"))
        {
            NGUIEditorTools.BeginContents();
            config.ProgramsSpreadSheetId = EditorGUILayout.TextField("Programs Sheet Id", config.ProgramsSpreadSheetId);
            if (GUILayout.Button("Fetch all programs data"))
                Hash17DataRetrieverInstance.Instance.FetchProgramsInfo(config.ProgramsSpreadSheetId);
            NGUIEditorTools.EndContents();
        }

        if (NGUIEditorTools.DrawHeader("Devices"))
        {
            NGUIEditorTools.BeginContents();
            config.DevicesSpreadSheetId = EditorGUILayout.TextField("Devices Sheet Id", config.DevicesSpreadSheetId);
            if (GUILayout.Button("Fetch all devices data"))
                Hash17DataRetrieverInstance.Instance.FetchDeviceInfo(config.DevicesSpreadSheetId);
            NGUIEditorTools.EndContents();
        }
    }
}
