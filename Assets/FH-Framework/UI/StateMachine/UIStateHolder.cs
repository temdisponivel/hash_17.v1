using UnityEngine;
using System.Collections;
using FH.Util.Extensions;

namespace FH.UI.StateMachine
{
	public class UIStateHolder : MonoBehaviour
	{
		public UIState InitialSceneState;

		public Transform[] States;

		private void Awake()
		{
			for (var i = 0; i < States.Length; i++)
			{
				States [i].parent = UIStateMachine.Instance.UIRoot.transform;
				States [i].Reset();
			}

			UIStateMachine.Instance.CurrentSceneState = InitialSceneState;
			UIStateMachine.Instance.onExitStateEnd += ExitStateEnd;
			InitialSceneState.EnterState ();
		}

		private void ExitStateEnd (UIStateType stateType)
		{
			if (stateType != InitialSceneState.Type)
				return;

			UIStateMachine.Instance.onExitStateEnd -= ExitStateEnd;

			for (var i = 0; i < States.Length; i++)
				States [i].parent = transform;
		}

	}
}