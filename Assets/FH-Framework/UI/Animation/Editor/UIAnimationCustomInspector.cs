using UnityEditor;
using DG.Tweening;
using FH.UI.Panels;

namespace FH.UI.Animations
{
	[CustomEditor(typeof(UIAnimation))]
	public class UIAnimationCustomInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var anim = (UIAnimation) target;

			NGUIEditorTools.BeginContents ();
			anim.OwnerPanel = (BasePanel)EditorGUILayout.ObjectField ("Owner Panel", anim.OwnerPanel, typeof(BasePanel), true);
			anim.StartsOnOpenPanel = EditorGUILayout.Toggle ("Starts On Open Panel", anim.StartsOnOpenPanel);
			anim.StartsOnClosePanel = EditorGUILayout.Toggle ("Starts On Close Panel", anim.StartsOnClosePanel);
			anim.Duration = EditorGUILayout.FloatField ("Duration", anim.Duration);
			anim.Delay = EditorGUILayout.FloatField ("Delay", anim.Delay);
			anim.Ease = (Ease)EditorGUILayout.EnumPopup("Easing", anim.Ease);
			NGUIEditorTools.EndContents ();

			NGUIEditorTools.BeginContents ();
			anim.AnimatePosition = EditorGUILayout.Toggle ("Animate Position", anim.AnimatePosition);
			if (anim.AnimatePosition)
			{
			    anim.UseNGUIAnimation = EditorGUILayout.Toggle("Use NGUI Animation", anim.UseNGUIAnimation);
			    if (!anim.UseNGUIAnimation)
			    {
			        anim.UnscaledTimePos = EditorGUILayout.Toggle("Unscaled Time", anim.UnscaledTimePos);
			        anim.PosUpdateType = (UpdateType) EditorGUILayout.EnumPopup("Update Type", anim.PosUpdateType);
			    }
			    anim.UseCurrentPositionXForInitial = EditorGUILayout.Toggle ("Use Position X For Initial", anim.UseCurrentPositionXForInitial);
				anim.UseCurrentPositionYForInitial = EditorGUILayout.Toggle ("Use Position Y For Initial", anim.UseCurrentPositionYForInitial);
				anim.UseCurrentPositionXForFinal = EditorGUILayout.Toggle ("Use Position X For Final", anim.UseCurrentPositionXForFinal);
				anim.UseCurrentPositionYForFinal = EditorGUILayout.Toggle ("Use Position Y For Final", anim.UseCurrentPositionYForFinal);

				if (anim.UseCurrentPositionXForInitial)
					anim.InitialPosition.x = anim.transform.localPosition.x;

				if (anim.UseCurrentPositionYForInitial)
					anim.InitialPosition.y = anim.transform.localPosition.y;

				if (anim.UseCurrentPositionXForFinal)
					anim.FinalPosition.x = anim.transform.localPosition.x;
				
				if (anim.UseCurrentPositionYForFinal)
					anim.FinalPosition.y = anim.transform.localPosition.y;

				anim.InitialPosition = EditorGUILayout.Vector2Field("Initial Position", anim.InitialPosition);
				anim.FinalPosition = EditorGUILayout.Vector2Field("Final Position", anim.FinalPosition);
			}
			NGUIEditorTools.EndContents ();

			NGUIEditorTools.BeginContents ();
			anim.AnimateScale = EditorGUILayout.Toggle ("Animate Scale", anim.AnimateScale);
			if (anim.AnimateScale)
			{
				anim.UseSpriteForScale = EditorGUILayout.Toggle ("Use Sprite for Scale", anim.UseSpriteForScale);

				anim.UseCurrentScaleXForInitial = EditorGUILayout.Toggle ("Use Scale X For Initial", anim.UseCurrentScaleXForInitial);
				anim.UseCurrentScaleYForInitial = EditorGUILayout.Toggle ("Use Scale Y For Initial", anim.UseCurrentScaleYForInitial);
				anim.UseCurrentScaleXForFinal = EditorGUILayout.Toggle ("Use Scale X For Final", anim.UseCurrentScaleXForFinal);
				anim.UseCurrentScaleYForFinal = EditorGUILayout.Toggle ("Use Scale Y For Final", anim.UseCurrentScaleYForFinal);

				if (!anim.UseSpriteForScale)
				{
					if (anim.UseCurrentScaleXForInitial)
						anim.InitialScale.x = anim.transform.localScale.x;
					
					if (anim.UseCurrentScaleYForInitial)
						anim.InitialScale.y = anim.transform.localScale.y;
					
					if (anim.UseCurrentScaleXForFinal)
						anim.FinalScale.x = anim.transform.localScale.x;
					
					if (anim.UseCurrentScaleYForFinal)
						anim.FinalScale.y = anim.transform.localScale.y;
				}
				else
				{
					if (anim.UseCurrentScaleXForInitial)
						anim.InitialScale.x = anim.GetComponent<UIWidget>().width;
					
					if (anim.UseCurrentScaleYForInitial)
						anim.InitialScale.y = anim.GetComponent<UIWidget>().height;
					
					if (anim.UseCurrentScaleXForFinal)
						anim.FinalScale.x = anim.GetComponent<UIWidget>().width;
					
					if (anim.UseCurrentScaleYForFinal)
						anim.FinalScale.y = anim.GetComponent<UIWidget>().height;
				}

				anim.InitialScale = EditorGUILayout.Vector2Field("Initial Scale", anim.InitialScale);
				anim.FinalScale = EditorGUILayout.Vector2Field("Final Scale", anim.FinalScale);
			}
			NGUIEditorTools.EndContents ();

			NGUIEditorTools.BeginContents ();
			anim.AnimateAlpha = EditorGUILayout.Toggle ("Animate Alpha", anim.AnimateAlpha);
			if (anim.AnimateAlpha)
			{
				anim.InitialAlpha = EditorGUILayout.Slider("Initial Alpha", anim.InitialAlpha, 0f, 1f);
				anim.FinalAlpha = EditorGUILayout.Slider("Final Alpha", anim.FinalAlpha, 0f, 1f);
			}
			NGUIEditorTools.EndContents ();
		}
	}
}