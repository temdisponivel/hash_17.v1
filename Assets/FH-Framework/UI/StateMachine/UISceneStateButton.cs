using UnityEngine;
using System.Collections;

namespace FH.UI.StateMachine
{
	public class UISceneStateButton : MonoBehaviour
	{
		public UIStateType StateType;

		public void OnButtonClicked()
		{
			UIStateMachine.Instance.OpenState (StateType);
		}
	}
}