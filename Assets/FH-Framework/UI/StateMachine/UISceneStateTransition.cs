using UnityEngine;
using System.Collections;

namespace FH.UI.StateMachine
{
	[System.Serializable]
	public class UISceneStateTransition
	{
		public UISceneState TargetState;
		public float Delay;
		public bool UseBlackFade;
		public bool ForceSceneReload;
	}
}