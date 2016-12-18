using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace FH.UI.Buttons
{
	[CustomEditor(typeof(UIButtonEnhanced))]
	public class UIButtonEnhancedEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var myScript = target as UIButtonEnhanced;
			if (myScript == null)
				return;

			myScript.AnimationDuration = EditorGUILayout.FloatField("Animation Duration:", myScript.AnimationDuration);
			myScript.LongPress = EditorGUILayout.Toggle ("Long Press:", myScript.LongPress);
			myScript.Toggle = EditorGUILayout.Toggle ("Toggle:", myScript.Toggle);
			myScript.SupportDoubleClick = EditorGUILayout.Toggle ("Support Double Click:", myScript.SupportDoubleClick);
			if (myScript.SupportDoubleClick)
				myScript.DoubleClickWindow = EditorGUILayout.FloatField ("Double Click Window:", myScript.DoubleClickWindow);
			myScript.LongClick = EditorGUILayout.Toggle ("Long Click:", myScript.LongClick);
			if (myScript.LongClick)
				myScript.LongClickDelay = EditorGUILayout.FloatField ("Long Click Delay", myScript.LongClickDelay);
			myScript.DefaultDeactivatedWarningMessage = EditorGUILayout.TextField ("Default Warning Message", myScript.DefaultDeactivatedWarningMessage);

			NGUIEditorTools.BeginContents();
			myScript.HasColorAnimation = EditorGUILayout.Toggle ("Animate Color:", myScript.HasColorAnimation);
			if (myScript.HasColorAnimation) {
				myScript.OverrideColors = EditorGUILayout.Toggle ("Override Color:", myScript.OverrideColors);
				myScript.HasDisabledColors = EditorGUILayout.Toggle ("Has Disabled Color:", myScript.HasDisabledColors);
				myScript.HoveredColor = EditorGUILayout.ColorField ("Hover Color:", myScript.HoveredColor);
				myScript.PressedColor = EditorGUILayout.ColorField ("Press Color:", myScript.PressedColor);
				if (myScript.HasDisabledColors)
					myScript.DisabledColor = EditorGUILayout.ColorField ("Disabled Color:", myScript.DisabledColor);
			}
			NGUIEditorTools.EndContents();

			NGUIEditorTools.BeginContents();
			myScript.HasScaleAnimation = EditorGUILayout.Toggle("Animate Scale:", myScript.HasScaleAnimation);
			if (myScript.HasScaleAnimation)
			{
				myScript.MultiplyOriginal = EditorGUILayout.Toggle("Multiplicative:", myScript.MultiplyOriginal);
				if (myScript.MultiplyOriginal)
				{
					myScript.HoverScaleMultiplier = EditorGUILayout.FloatField("Hover Multiplier:", myScript.HoverScaleMultiplier);
					myScript.PressedScaleMultiplier = EditorGUILayout.FloatField("Press Multiplier:", myScript.PressedScaleMultiplier);
				}
				else
				{
					myScript.HoveredTargetScale = EditorGUILayout.Vector3Field("Hover Target Scale:", myScript.HoveredTargetScale);
					myScript.PressedTargetScale = EditorGUILayout.Vector3Field("Press Target Scale:", myScript.PressedTargetScale);
				}
			}
			NGUIEditorTools.EndContents();

			NGUIEditorTools.BeginContents();
			myScript.HasHighlightAnimation = EditorGUILayout.Toggle("Animate Highlight:", myScript.HasHighlightAnimation);
			if (myScript.HasHighlightAnimation)
				myScript.HighlightSprite = EditorGUILayout.ObjectField("Highlight Sprite:", myScript.HighlightSprite, typeof(UISprite), true) as UISprite;
			NGUIEditorTools.EndContents();

			NGUIEditorTools.BeginContents();
			myScript.PlaySounds = EditorGUILayout.Toggle("Play Sounds:", myScript.PlaySounds);
			if (myScript.PlaySounds)
			{
				myScript.DownSoundGroup = EditorGUILayout.TextField("Button Down:", myScript.DownSoundGroup);
				myScript.UpSoundGroup = EditorGUILayout.TextField("Button Up:", myScript.UpSoundGroup);
				myScript.ClickSoundGroup = EditorGUILayout.TextField("Button Click:", myScript.ClickSoundGroup);
			}
			NGUIEditorTools.EndContents();

			NGUIEditorTools.DrawEvents("On Click", myScript, myScript.onClick, false);
			NGUIEditorTools.DrawEvents("On Left Click", myScript, myScript.onLeftClick, false);
			NGUIEditorTools.DrawEvents("On Double Click", myScript, myScript.onDoubleClick, false);
			if (myScript.Toggle)
				NGUIEditorTools.DrawEvents("On Toggle", myScript, myScript.onToggleOff, false);
			if (myScript.LongClick)
			{
				NGUIEditorTools.DrawEvents ("On Long Click Start", myScript, myScript.onLongClickStart, false);
				NGUIEditorTools.DrawEvents ("On Long Click End", myScript, myScript.onLongClickEnd, false);
			}
		}
	}
}