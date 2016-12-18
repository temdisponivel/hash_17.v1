using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace FH.UI.Buttons
{
	[CustomEditor(typeof(UIPassiveButtonEnhanced))]
	public class UIPassiveButtonEnhancedEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var myScript = target as UIPassiveButtonEnhanced;
			if (myScript == null)
				return;

			myScript.AnimationDuration = EditorGUILayout.FloatField("Animation Duration:", myScript.AnimationDuration);
			myScript.LongPress = EditorGUILayout.Toggle ("Long Press:", myScript.LongPress);
			myScript.Click = EditorGUILayout.Toggle ("Click:", myScript.Click);
			myScript.SupportDoubleClick = EditorGUILayout.Toggle ("Support Double Click:", myScript.SupportDoubleClick);
			myScript.DefaultDeactivatedWarningMessage = EditorGUILayout.TextField ("Default Warning Message", myScript.DefaultDeactivatedWarningMessage);

			NGUIEditorTools.BeginContents();
			myScript.HasColorAnimation = EditorGUILayout.Toggle ("Animate Color:", myScript.HasColorAnimation);
			if (myScript.HasColorAnimation) {
				myScript.OverrideColors = EditorGUILayout.Toggle ("Override Color:", myScript.OverrideColors);
				myScript.UnselectedColor = EditorGUILayout.ColorField ("Unselected Color:", myScript.UnselectedColor);
				myScript.SelectedColor = EditorGUILayout.ColorField ("Selected Color:", myScript.SelectedColor);
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
					myScript.UnselectedScaleMultiplier = EditorGUILayout.FloatField("Unselected Multiplier:", myScript.UnselectedScaleMultiplier);
					myScript.SelectedScaleMultiplier = EditorGUILayout.FloatField("Selected Multiplier:", myScript.SelectedScaleMultiplier);
					myScript.DisabledScaleMultiplier = EditorGUILayout.FloatField("Disabled Multiplier:", myScript.DisabledScaleMultiplier);
				}
				else
				{
					myScript.UnselectedTargetScale = EditorGUILayout.Vector3Field("Unselected Target Scale:", myScript.UnselectedTargetScale);
					myScript.SelectedTargetScale = EditorGUILayout.Vector3Field("Selected Target Scale:", myScript.SelectedTargetScale);
					myScript.DisabledTargetScale = EditorGUILayout.Vector3Field("Disabled Target Scale:", myScript.DisabledTargetScale);
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
			NGUIEditorTools.DrawEvents("On Double Click", myScript, myScript.onDoubleClick, false);
		}
	}
}