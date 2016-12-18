using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FH.UI.StateMachine
{
	public class UISceneState : MonoBehaviour
	{
		[SerializeField]
		private string _sceneState;
		public virtual string StateScene { get { return _sceneState; } set { _sceneState = value; } }
		public UIStateType State;
		public UIStateType UpperState;
		public List<UISceneStateTransition> Transitions;
	}
}