using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using FH.UI.Animations;

namespace FH.UI.StateMachine
{
    public class UIStateMachine : MonoBehaviour
    {
        private static UIState _currentSceneState;
        public UIState CurrentSceneState
        {
            get { return _currentSceneState; }
            set
            {
                _currentSceneState = value;
                _currentSceneState.onEnterStateFinish += FinishOpeningNextState;
                _runningTasks++;

                if (DebugOn)
                    Debug.Log("StartSwitchingInNextState +1: " + _runningTasks);
            }
        }

		public bool LockTransitions;
        public bool DragOn;
        public bool DebugOn;
        public UIRoot UIRoot;
        public UIPanel FadePanel;
        public UISceneState InitialState;
        public UISceneState CurrentState;
        public UISceneState[] AllSceneStates;

		/*public UIAnimation LoadingImageAnimation;
		public UIAnimation LoadingAnimation;
		public UISprite LoadingBar;
		public UILabel LoadingHint;
		public UITexture LoadingImage;
		public int LoadingBarMaxSize;*/

        private int _runningTasks;
		private Tweener _fadeTweener;

        #region Singleton

        private static UIStateMachine instance;
        public static UIStateMachine Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<UIStateMachine>() as UIStateMachine;
                return instance;
            }
        }

        #endregion

        #region Unity Events

        protected void Awake()
        {
            instance = this;
        }

        protected void Start()
        {
            OpenInitialState();
        }

        #endregion

        #region State Handling

        private int _currentStateTransitionIndex = -1;

        private void OpenInitialState()
        {
			/*if (PlayerHandler.Instance.Player.HasSeenIntroCutscene)
				InitialState = SceneStateByType (UIStateType.Menu);
			else
				InitialState = SceneStateByType (UIStateType.Intro);*/

			CurrentState = InitialState;
			StartCoroutine(ChangeScene (InitialState.StateScene));
			FadePanel.gameObject.SetActive (true);
			FadePanel.alpha = 1;
			StartFadeBlackOut ();

            /*_currentStateTransitionIndex = SceneStateTransitionByType(targetState);
            if (_currentStateTransitionIndex == -1)
            {
                Debug.Log("Target state " + targetState + " is not a valid transition from state " + CurrentState.State);
                return;
            }

			StartSwitchOutCurrentState();*/
        }

		public void OpenState(UIStateType targetState)
        {
			if (LockTransitions || _runningTasks > 0)
            {
                Debug.Log("There are still tasks running! Transition from " + CurrentState.State + " to " + targetState + " cancelled!");
                return;
            }

			_currentStateTransitionIndex = SceneStateTransitionByType (targetState);
			if (_currentStateTransitionIndex == -1)
			{
				Debug.Log ("Target state " + targetState + " is not a valid transition from state " + CurrentState.State);
				return;
			}

            StartSwitchOutCurrentState();
            FinishSwitchingOutCurrentState();
        }

        public void Back()
        {
            OpenState(CurrentState.UpperState);
        }

        #endregion

        #region Switch State Handling

        private void StartSwitchOutCurrentState()
        {
            if (onExitStateStart != null)
                onExitStateStart(CurrentState.State);

            CurrentSceneState.onExitStateFinish += FinishExitingCurrentState;
            CurrentSceneState.ExitState();
            _runningTasks++;

            if (DebugOn)
                Debug.Log("StartSwitchOutCurrentState +1: " + _runningTasks);

            if (CurrentState.Transitions[_currentStateTransitionIndex].UseBlackFade)
            {
                StartFadeBlackIn(0.2f, 0.3f);
                onFadeBlackInEnd += FinishFadingBlackIn;
                _runningTasks++;

                if (DebugOn)
                    Debug.Log("StartSwitchOutCurrentState Fade +1: " + _runningTasks);
            }
        }

        private void FinishFadingBlackIn()
        {
            onFadeBlackInEnd -= FinishFadingBlackIn;
            _runningTasks--;

            if (DebugOn)
                Debug.Log("FinishFadingBlackIn -1: " + _runningTasks);

            FinishSwitchingOutCurrentState();
        }

        private void FinishExitingCurrentState()
        {
            CurrentSceneState.onExitStateFinish -= FinishExitingCurrentState;
            _runningTasks--;

            if (DebugOn)
                Debug.Log("FinishExitingCurrentState -1: " + _runningTasks);

            FinishSwitchingOutCurrentState();
        }

        private void FinishSwitchingOutCurrentState()
        {
            if (_runningTasks == 0)
            {
                if (onExitStateEnd != null)
                    onExitStateEnd(CurrentState.State);

                StartCoroutine(DelayedStartSwitchingInNextState(CurrentState.Transitions[_currentStateTransitionIndex].Delay));
            }
        }

        private IEnumerator DelayedStartSwitchingInNextState(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
			onSceneLoad += HandleOnSceneLoad;
			SceneChange(CurrentState.Transitions[_currentStateTransitionIndex].TargetState.StateScene);
        }

		private void HandleOnSceneLoad (string loadedLevel)
        {
			onSceneLoad -= HandleOnSceneLoad;
			StartSwitchingInNextState();
        }

        private void StartSwitchingInNextState()
        {
            if (CurrentState.Transitions[_currentStateTransitionIndex].UseBlackFade)
            {
                StartFadeBlackOut(0.2f, 0f);
                onFadeBlackOutEnd += FinishFadingBlackOut;
                _runningTasks++;

                if (DebugOn)
                    Debug.Log("StartSwitchingInNextState Fade +1: " + _runningTasks);
            }

            CurrentState = CurrentState.Transitions[_currentStateTransitionIndex].TargetState;

            if (onEnterStateStart != null)
                onEnterStateStart(CurrentState.State);

            FinishSwitchingInNextState();
        }

        private void FinishFadingBlackOut()
        {
            onFadeBlackOutEnd -= FinishFadingBlackOut;
            _runningTasks--;

            if (DebugOn)
                Debug.Log("FinishFadingBlackOut -1: " + _runningTasks);

            FinishSwitchingInNextState();
        }

        private void FinishOpeningNextState()
        {
            CurrentSceneState.onEnterStateFinish -= FinishOpeningNextState;
            _runningTasks--;

            if (DebugOn)
                Debug.Log("FinishOpeningNextState -1: " + _runningTasks);

            FinishSwitchingInNextState();
        }

        private void FinishSwitchingInNextState()
        {
            if (_runningTasks == 0)
            {
                if (onEnterStateEnd != null)
                    onEnterStateEnd(CurrentState.State);
            }
        }

        #endregion

        #region Helper Methods

		private int SceneStateTransitionByType(UIStateType targetState)
        {
            for (var i = 0; i < CurrentState.Transitions.Count; i++)
            {
                if (CurrentState.Transitions[i].TargetState.State == targetState)
                    return i;
            }

            return -1;
        }

		private UISceneState SceneStateByType(UIStateType stateType)
        {
            for (var i = 0; i < AllSceneStates.Length; i++)
            {
				if (AllSceneStates[i].State == stateType)
                    return AllSceneStates[i];
            }

            return null;
        }

        #endregion

        #region Fade Handling

		public void StartFadeBlackOut(float duration = 0.3f, float delay = 0.2f)
        {
			if (_fadeTweener != null)
				_fadeTweener.Kill ();

			_fadeTweener = DOTween.To (() => FadePanel.alpha, x => FadePanel.alpha = x, 0f, duration).SetDelay (delay).SetUpdate (false).OnComplete (() => {
				FadePanel.cachedGameObject.SetActive(false);
				if (onFadeBlackOutEnd != null)
					onFadeBlackOutEnd();
			});

            if (onFadeBlackOutStart != null)
                onFadeBlackOutStart();
        }

		public void StartFadeBlackIn(float duration = 0.3f, float delay = 0.2f)
        {
			if (_fadeTweener != null)
				_fadeTweener.Kill ();

			if (!FadePanel.cachedGameObject.activeSelf)
				FadePanel.cachedGameObject.SetActive(true);
			_fadeTweener = DOTween.To (() => FadePanel.alpha, x => FadePanel.alpha = x, 1f, duration).SetDelay (delay).SetUpdate (false).OnComplete (() => {
				if (onFadeBlackInEnd != null)
					onFadeBlackInEnd();
			});

            if (onFadeBlackInStart != null)
                onFadeBlackInStart();
        }

        #endregion

        #region Event handling

		public event Action<UIStateType> onEnterStateStart;
		public event Action<UIStateType> onEnterStateEnd;
		public event Action<UIStateType> onExitStateStart;
		public event Action<UIStateType> onExitStateEnd;

		public event Action onFadeBlackOutStart;
		public event Action onFadeBlackOutEnd;
		public event Action onFadeBlackInStart;
		public event Action onFadeBlackInEnd;

		public event Action<string> onSceneUnload;
		public event Action<string> onSceneLoad;

		public void EnterStateStart(UIStateType stateType)
		{
			if (onEnterStateStart != null)
				onEnterStateStart(stateType);
		}

		public void EnterStateEnd(UIStateType stateType)
		{
			if (onEnterStateEnd != null)
				onEnterStateEnd(stateType);
		}

		public void ExitStateStart(UIStateType stateType)
		{
			if (onExitStateStart != null)
				onExitStateStart(stateType);
		}

		public void ExitStateEnd(UIStateType stateType)
		{
			if (onExitStateEnd != null)
				onExitStateEnd(stateType);
		}

		public virtual void SceneChange(string loadedLevel)
        {
			StartCoroutine (ChangeScene (loadedLevel));
        }

		private IEnumerator ChangeScene(string levelToLoad)
		{
			if (onSceneUnload != null)
				onSceneUnload (SceneManager.GetActiveScene ().name);

			//SceneManager.LoadScene("Loading-Scene");

			yield return null;

			SceneManager.LoadScene(levelToLoad);

			if (onSceneLoad != null)
				onSceneLoad (levelToLoad);

			/*MasterAudio.StopBus("SFX");

			var hintItem = ScriptableObjectHolder.Instance.HintCollection.GetAppropriateHint();
			LoadingHint.text = string.Format ("{0} {1}", LocalizationManager.Instance.Data[LocalizationKeys.HINT_LABEL], LocalizationManager.Instance.Data[hintItem.Hint]);
			LoadingImage.mainTexture = Resources.Load<Texture> (hintItem.HintImagePath);

			LoadingImageAnimation.StartAnimation ();
			LoadingAnimation.StartAnimation ();
			LoadingBar.width = 0;

			yield return new WaitForSeconds (LoadingAnimation.Duration);

			if (onSceneUnload != null)
				onSceneUnload (SceneManager.GetActiveScene ().name);

			SceneManager.LoadScene("Loading-Scene");

			yield return null;

			var asyncOp = SceneManager.LoadSceneAsync (levelToLoad);
			while (asyncOp.progress < 0.9f)
			{
				LoadingBar.width = Mathf.RoundToInt(LoadingBarMaxSize * asyncOp.progress);
				yield return null;
			}

			LoadingBar.width = LoadingBarMaxSize;

			while (asyncOp.progress < 1f)
				yield return null;

			yield return null;

			LoadingAnimation.StartAnimation (true);
			LoadingImageAnimation.StartAnimation (true);

			yield return new WaitForSeconds (LoadingAnimation.Duration * 2);

			if (onSceneLoad != null)
				onSceneLoad (levelToLoad);

			Resources.UnloadAsset (LoadingImage.mainTexture);*/
		}

        /*public delegate void OnEnterStateStart(UIPanelType stateType);
        public event OnEnterStateStart onEnterStateStart;
        public void EnterStateStart(UIPanelType stateType)
        {
            if (onEnterStateStart != null)
                onEnterStateStart(stateType);
        }

        public delegate void OnEnterStateEnd(UIPanelType stateType);
        public event OnEnterStateEnd onEnterStateEnd;
        public void EnterStateEnd(UIPanelType stateType)
        {
            if (onEnterStateEnd != null)
                onEnterStateEnd(stateType);
        }

        public delegate void OnExitStateStart(UIPanelType stateType);
        public event OnExitStateStart onExitStateStart;
        public void ExitStateStart(UIPanelType stateType)
        {
            if (onExitStateStart != null)
                onExitStateStart(stateType);
        }

        public delegate void OnExitStateEnd(UIPanelType stateType);
        public event OnExitStateEnd onExitStateEnd;
        public void ExitStateEnd(UIPanelType stateType)
        {
            if (onExitStateEnd != null)
                onExitStateEnd(stateType);
        }

        public delegate void OnFadeBlackOutStart();
        public event OnFadeBlackOutStart onFadeBlackOutStart;

        public delegate void OnFadeBlackOutEnd();
        public event OnFadeBlackOutEnd onFadeBlackOutEnd;

        public delegate void OnFadeBlackInStart();
        public event OnFadeBlackInStart onFadeBlackInStart;

        public delegate void OnFadeBlackInEnd();
        public event OnFadeBlackInEnd onFadeBlackInEnd;*/

        #endregion
    }
}