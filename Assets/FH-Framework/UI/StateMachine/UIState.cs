//#define Debugging

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FH.UI.Panels;
using FH.Util.Time;

namespace FH.UI.StateMachine
{
	public class UIState : MonoBehaviour
	{
		public static UIState CurrentState;

		public UIStateType Type;
		public BasePanel Panel;
		public List<UIStateTransition> SubStateList;
		public UIState ParentState { get { return _parentState; } }

		public bool OverrideBackState;
		public UIState ParentToReceiveBackStateCall;
		public UIStateType StateToCallOnBack;

		public delegate void OnEnterStateStart ();
		public event OnEnterStateStart onEnterStateStart;

		protected void EnterStateStart()
		{
			if (onEnterStateStart != null)
				onEnterStateStart ();
		}

		public delegate void OnEnterStateFinish ();
		public event OnEnterStateFinish onEnterStateFinish;

		public delegate void OnExitStateStart ();
		public event OnExitStateStart onExitStateStart;

		public delegate void OnExitStateFinish ();
		public event OnExitStateFinish onExitStateFinish;

		protected UIState _parentState;
		protected int _currentSubstateIndex = -1;
		public int CurrentSubstateIndex { get { return _currentSubstateIndex; } }
		protected int _runningTasks;
		public bool HasRunningTasks { get { return _runningTasks > 0; } }
		protected GameObject _gameObject;

		#region Enter State

		public virtual void EnterState(UIState parentState = null)
		{
			if (UIStateMachine.Instance.LockTransitions || _runningTasks > 0)
			{
				#if Debugging
				Debug.Log("There are still running tasks. Unable to enter state " + Type + ".");
				#endif
				return;
			}

			if (SubStateList != null && SubStateList.Count > 0)
				_currentSubstateIndex = _initialSubstate;

			if (_gameObject == null)
				_gameObject = gameObject;
			_gameObject.SetActive (true);

			if (onEnterStateStart != null)
				onEnterStateStart ();
			UIStateMachine.Instance.EnterStateStart (Type);

			CurrentState = this;
			_parentState = parentState;
			OpenPanel ();

			if (SubStateList != null && SubStateList.Count > 0)
				StartEnteringSubState();
		}

		protected void OpenPanel()
		{
			Panel.onOpenPanelFinish += FinishOpeningPanel;
			Panel.OpenPanel ();
			_runningTasks ++;

			#if Debugging
			Debug.Log ("OpenPanel in " + Type + " +1: " + _runningTasks);
			#endif
		}

		protected void FinishOpeningPanel(BasePanel panel)
		{
			Panel.onOpenPanelFinish -= FinishOpeningPanel;
			_runningTasks --;

			#if Debugging
			Debug.Log ("FinishOpeningPanel in " + Type + " -1: " + _runningTasks);
			#endif

			FinishEnteringState ();
		}

		protected void StartEnteringSubState()
		{
			SubStateList[_currentSubstateIndex].TargetState.onEnterStateFinish += FinishEnteringSubstate;
			SubStateList[_currentSubstateIndex].TargetState.EnterState (this);
			_runningTasks ++;

			#if Debugging
			Debug.Log ("StartEnteringSubState in " + Type + " +1: " + _runningTasks);
			#endif
		}

		protected void FinishEnteringSubstate()
		{
			SubStateList[_currentSubstateIndex].TargetState.onEnterStateFinish -= FinishEnteringSubstate;
			_runningTasks --;

			#if Debugging
			Debug.Log ("FinishEnteringSubstate in " + Type + " -1: " + _runningTasks);
			#endif
			
			FinishEnteringState ();
		}

		protected void FinishEnteringState()
		{
			if (_runningTasks == 0)
			{
				if (onEnterStateFinish != null)
					onEnterStateFinish ();
				UIStateMachine.Instance.EnterStateEnd (Type);
			}
		}

		#endregion

		#region Exit State

		public void ExitState()
		{
			if (UIStateMachine.Instance.LockTransitions || _runningTasks > 0)
			{
				#if Debugging
				Debug.Log("There are still running tasks. Unable to exit state " + Type + ".");
				#endif
				return;
			}

			if (onExitStateStart != null)
				onExitStateStart ();
			UIStateMachine.Instance.ExitStateStart (Type);

			ClosePanel ();
			
			if (_currentSubstateIndex != -1)
				StartExitingSubState();
		}

		protected void ClosePanel()
		{
			_runningTasks ++;
			Panel.onClosePanelFinish += FinishClosingPanel;
			Panel.ClosePanel ();

			#if Debugging
			Debug.Log ("ClosePanel in " + Type + " +1: " + _runningTasks);
			#endif
		}
		
		protected void FinishClosingPanel(BasePanel basePanel)
		{
			Panel.onClosePanelFinish -= FinishClosingPanel;
			_runningTasks --;

			#if Debugging
			Debug.Log ("FinishClosingPanel in " + Type + " -1: " + _runningTasks);
			#endif

			FinishExitingState ();
		}

		protected void StartExitingSubState()
		{
			SubStateList[_currentSubstateIndex].TargetState.onExitStateFinish += FinishExitingSubstate;
			SubStateList[_currentSubstateIndex].TargetState.ExitState ();
			_runningTasks ++;

			#if Debugging
			Debug.Log ("StartExitingSubState in " + Type + " +1: " + _runningTasks);
			#endif
		}
		
		protected void FinishExitingSubstate()
		{
			SubStateList[_currentSubstateIndex].TargetState.onExitStateFinish -= FinishExitingSubstate;
			_runningTasks --;

			#if Debugging
			Debug.Log ("FinishExitingSubstate in " + Type + " -1: " + _runningTasks);
			#endif
			
			FinishExitingState ();
		}

		protected void FinishExitingState()
		{
			if (_runningTasks == 0)
			{
				if (onExitStateFinish != null)
					onExitStateFinish ();
				UIStateMachine.Instance.ExitStateEnd (Type);

				_gameObject.SetActive (false);
			}
		}

		#endregion

		#region SubState Handling

		public delegate void OnSwitchingOutSubstateStart (UIStateType currentSubstate, UIStateType nextSubstate);
		public event OnSwitchingOutSubstateStart onSwitchingOutSubstateStart;

		public delegate void OnSwitchingOutSubstateFinish (UIStateType currentSubstate, UIStateType nextSubstate);
		public event OnSwitchingOutSubstateFinish onSwitchingOutSubstateFinish;

		public delegate void OnSwitchingInSubstateStart (UIStateType previousSubstate, UIStateType nextSubstate);
		public event OnSwitchingInSubstateStart onSwitchingInSubstateStart;
		
		public delegate void OnSwitchingInSubstateFinish (UIStateType previousSubstate, UIStateType nextSubstate);
		public event OnSwitchingInSubstateFinish onSwitchingInSubstateFinish;

		protected int _previousSubstateIndex = -1;
		protected int _nextSubstateIndex = -1;
		protected int _initialSubstate = 0;

		public void OpenInitialSubstate()
		{
			if (SubStateList == null || SubStateList.Count == 0)
			{
				#if Debugging
				Debug.Log("The state " + Type + " does not have substates!");
				#endif
				return;
			}
			
			if (_currentSubstateIndex == _initialSubstate)
			{
				#if Debugging
				Debug.Log("The initial substate in " + Type + " is already open!");
				#endif
				return;
			}
			
			OpenSubstate (SubStateList [_initialSubstate].TargetState.Type);
		}

		public bool OpenSubstate(UIStateType stateType)
		{
			if (UIStateMachine.Instance.LockTransitions || _runningTasks > 0)
			{
				#if Debugging
				Debug.Log("There are still running tasks. Unable to open Substate " + stateType + ".");
				#endif
				return false;
			}

			_nextSubstateIndex = GetSubstateIndex(stateType);
			if (_nextSubstateIndex == -1)
			{
				#if Debugging
				Debug.Log("Substate " + stateType + " was not found! Transition cancelled!");
				#endif
				return false;
			}

			if (_currentSubstateIndex != -1)
			{
				if (SubStateList[_currentSubstateIndex].TargetState.Type == stateType)
				{
					#if Debugging
					Debug.Log ("Cannot open the state " + stateType + " because it is currently open!");
					#endif
					return false;
				}

				if (SubStateList [_currentSubstateIndex].TargetState.HasRunningTasks)
				{
					#if Debugging
					Debug.Log ("Cannot open the state " + stateType + " because " + SubStateList [_currentSubstateIndex].TargetState.Type.ToString() + " still has running tasks!");
					#endif
					return false;
				}
                
                if (onSwitchingOutSubstateStart != null)
                    onSwitchingOutSubstateStart(SubStateList[_currentSubstateIndex].TargetState.Type, stateType);

                StartSwitchingOutSubState();
				return true;
			}
			
			FinishSwitchingOutState ();
			return false;
		}

		protected void StartSwitchingOutSubState()
		{
			if (SubStateList [_nextSubstateIndex].UseBlackFade)
			{
				UIStateMachine.Instance.StartFadeBlackIn ();
			}

			SubStateList[_currentSubstateIndex].TargetState.onExitStateFinish += FinishSwitchingOutSubstate;
			SubStateList[_currentSubstateIndex].TargetState.ExitState ();
			_runningTasks ++;

			#if Debugging
			Debug.Log ("StartSwitchingOutSubState in " + Type + " +1: " + _runningTasks);
			#endif
		}
		
		protected void FinishSwitchingOutSubstate()
		{
			SubStateList[_currentSubstateIndex].TargetState.onExitStateFinish -= FinishSwitchingOutSubstate;
			_runningTasks --;

			#if Debugging
			Debug.Log ("FinishSwitchingOutSubstate in " + Type + " -1: " + _runningTasks);
			#endif

			FinishSwitchingOutState ();
		}

		protected void FinishSwitchingOutState()
		{
			if (_runningTasks == 0)
			{
				if (onSwitchingOutSubstateFinish != null)
					onSwitchingOutSubstateFinish(SubStateList[_currentSubstateIndex].TargetState.Type, SubStateList[_nextSubstateIndex].TargetState.Type);

				_previousSubstateIndex = _currentSubstateIndex;
				_currentSubstateIndex = _nextSubstateIndex;
				_nextSubstateIndex = -1;

				StartCoroutine(StartSwitchingInSubStateDelayed(SubStateList[_currentSubstateIndex].Delay));
			}
		}

		protected IEnumerator StartSwitchingInSubStateDelayed(float waitTime)
		{
			yield return StartCoroutine(TimeUtil.WaitForRealSeconds(waitTime));
			StartSwitchingInSubState();

			if (onSwitchingInSubstateStart != null)
				onSwitchingInSubstateStart(_previousSubstateIndex != -1 ? SubStateList[_previousSubstateIndex].TargetState.Type : UIStateType.None, SubStateList[_currentSubstateIndex].TargetState.Type);
		}

		protected void StartSwitchingInSubState()
		{
			if (SubStateList[_currentSubstateIndex].UseBlackFade)
				UIStateMachine.Instance.StartFadeBlackOut ();

			SubStateList[_currentSubstateIndex].TargetState.onEnterStateFinish += FinishSwitchingInSubstate;
			SubStateList[_currentSubstateIndex].TargetState.EnterState (this);
			_runningTasks ++;

			#if Debugging
			Debug.Log ("StartSwitchingInSubState in " + Type + " +1: " + _runningTasks);
			#endif
		}
		
		protected void FinishSwitchingInSubstate()
		{
			SubStateList[_currentSubstateIndex].TargetState.onEnterStateFinish -= FinishSwitchingInSubstate;
			_runningTasks --;

			#if Debugging
			Debug.Log ("FinishSwitchingInSubstate in " + Type + " -1: " + _runningTasks);
			#endif
			
			FinishSwitchingInState ();
		}

		protected void FinishSwitchingInState()
		{
			if (_runningTasks == 0)
			{
				if (onSwitchingInSubstateFinish != null)
					onSwitchingInSubstateFinish(_previousSubstateIndex != -1 ? SubStateList[_previousSubstateIndex].TargetState.Type : UIStateType.None, SubStateList[_currentSubstateIndex].TargetState.Type);

				_previousSubstateIndex = -1;
			}
		}

		#endregion

		#region Back Handling

		public delegate void OnBackFromState (UIStateType currentState);
		public event OnBackFromState onBackFromState;

		public void Back()
		{
			if (UIStateMachine.Instance.LockTransitions || _runningTasks > 0)
			{
				#if Debugging
				Debug.Log("Cannot go back a state from " + Type + " because there are still running tasks!");
				#endif
				return;
			}

			if (onBackFromState != null)
				onBackFromState (Type);

			if (OverrideBackState)
			{
				ParentToReceiveBackStateCall.OpenSubstate(StateToCallOnBack);
				return;
			}

			if (SubStateList != null && SubStateList.Count > 0 && _currentSubstateIndex != 0)
			{
				#if Debugging
				Debug.Log("Opening Initial Substate");
				#endif
				OpenSubstate (SubStateList [_initialSubstate].TargetState.Type);
				return;
			}

			if (_parentState != null)
			{
				_parentState.Back();
				return;
			}

			#if Debugging
			Debug.Log ("Warning FHUIStateMachine to go back a state");
			#endif
			UIStateMachine.Instance.Back ();
		}

		#endregion

		#region Helper Methods

		protected int GetSubstateIndex(UIStateType stateType)
		{
			for (var i = 0; i < SubStateList.Count; i++)
			{
				if (SubStateList[i].TargetState.Type == stateType)
					return i;
			}
			
			return -1;
		}

		#endregion
	}
}