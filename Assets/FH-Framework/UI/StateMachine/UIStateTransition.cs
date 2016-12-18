using UnityEngine;
using System.Collections;

namespace FH.UI.StateMachine
{
	[System.Serializable]
	public class UIStateTransition
	{
		public UIState TargetState;
		public float Delay;
		public bool UseBlackFade;
		public bool ForceSceneReload;
	}
}