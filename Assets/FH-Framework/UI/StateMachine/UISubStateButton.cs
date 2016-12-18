using UnityEngine;
using System.Collections;

namespace FH.UI.StateMachine
{
	public class UISubStateButton : MonoBehaviour
	{
		public UIStateType StateType;
		public UIState ParentState;

		public void OnButtonClicked()
		{
		    if (ParentState == null) return;
			ParentState.OpenSubstate (StateType);
		}
	}
}