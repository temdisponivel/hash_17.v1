using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DarkTonic.MasterAudio;
using DG.Tweening;
using FH.UI.Warnings;

namespace FH.UI.Buttons
{
	public class UIButtonEnhanced : MonoBehaviour
	{
		#region Config Variables

		public float AnimationDuration = 0.2f;
		public bool LongPress = true;
		public bool LongClick = false;
		public bool Toggle = false;
		public bool SupportDoubleClick = false;
		protected float _currentAnimationDuration;
		protected bool _callbackRegistered;
		protected bool _started;

		#endregion

		#region Activation Variables

		protected bool isActivated = true;
		public string DefaultDeactivatedWarningMessage;
		protected string _deactivatedWarningMessageOverride;
		public bool IsActive { get { return isActivated; } }

		#endregion

		#region Double Click Variables

		public float DoubleClickWindow = 0.5f;
		protected bool _alreadySentMessage;
		protected bool _tooFastClick;

		#endregion

		#region Toggle Variables

		protected bool _isToggled;

		#endregion

		#region Long Click Variables

		public float LongClickDelay;
		private float _longClickCounter;

		#endregion

		#region Color Variables

		public bool HasColorAnimation;
		public bool OverrideColors;
		public bool HasDisabledColors;
		public Color HoveredColor = new Color (0.3f, 0.3f, 0.3f, 1);
		public Color PressedColor = new Color (1f, 1f, 1f, 1);
		public Color DisabledColor = new Color (0.3f, 0.3f, 0.3f, 1);
		protected Color[] _defaultColor;
		protected Color[] _hoveredColor;
		protected Color[] _pressedColor;
		protected Color[] _disabledColor;

		#endregion

		#region Scale Variables

		public bool HasScaleAnimation;
		public bool MultiplyOriginal = true;
		public float HoverScaleMultiplier;
		public float PressedScaleMultiplier;
		public Vector3 HoveredTargetScale;
		public Vector3 PressedTargetScale;
		protected Vector3 _defaultScale;
		protected Vector3 _hoveredScale;
		protected Vector3 _pressedScale;

		#endregion

		#region Highlight Variables

		public bool HasHighlightAnimation;
		public UISprite HighlightSprite;

		#endregion

		#region Sound Variables

		public bool PlaySounds = true;
		public string ClickSoundGroup = "Menu-Click-01";
		public string DownSoundGroup;
		public string UpSoundGroup;
		protected string _clickSoundName;
		protected string _downSoundName;
		protected string _upSoundName;
		protected bool _hasClickSound;
		protected bool _hasDownSound;
		protected bool _hasUpSound;

		#endregion

		#region Delegate Variables

		public List<EventDelegate> onClick = new List<EventDelegate>();
		public List<EventDelegate> onLeftClick = new List<EventDelegate>();
		public List<EventDelegate> onLongClickStart = new List<EventDelegate>();
		public List<EventDelegate> onLongClickEnd = new List<EventDelegate>();
		public List<EventDelegate> onToggleOff = new List<EventDelegate>();
		public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

		#endregion

		#region Cache Variables

		protected List<UIWidget> _childWidgets = new List<UIWidget>();
		protected Tweener _tweenScale;
		protected Tweener[] _tweenColor;
		protected Tweener _tweenHighlight;
		protected TweenParams _tweenParams;
		protected TweenParams _tweenParamsCallback;

		#endregion

		#region Unity Events

		protected virtual void Awake()
		{
			if (PlaySounds)
			{
				if (string.IsNullOrEmpty(ClickSoundGroup))
					_hasClickSound = false;
				else
				{
					_hasClickSound = true;
					_clickSoundName = ClickSoundGroup;
				}

				if (string.IsNullOrEmpty(DownSoundGroup))
					_hasDownSound = false;
				else
				{
					_hasDownSound = true;
					_downSoundName = DownSoundGroup;
				}

				if (string.IsNullOrEmpty(UpSoundGroup))
					_hasUpSound = false;
				else
				{
					_hasUpSound = true;
					_upSoundName = UpSoundGroup;
				}
			}
		}

		protected virtual void Start()
		{
			if (_started)
				return;

			if (HasScaleAnimation)
			{
				_defaultScale = transform.localScale;
				if (MultiplyOriginal)
				{
					_hoveredScale = _defaultScale * HoverScaleMultiplier;
					_pressedScale = _defaultScale * PressedScaleMultiplier;
				}
				else
				{
					_hoveredScale = HoveredTargetScale;
					_pressedScale = PressedTargetScale;
				}
			}

			if (HasColorAnimation)
			{
				var tempWidgets = GetComponentsInChildren<UIWidget>(true);
				for (byte i = 0; i < tempWidgets.Length; i++)
				{
					if (HasHighlightAnimation)
						if (tempWidgets[i] == (UIWidget)HighlightSprite)
							continue;

					_childWidgets.Add(tempWidgets[i]);
				}

				_defaultColor = new Color[_childWidgets.Count];
				for (byte i = 0; i < _defaultColor.Length; i++)
					_defaultColor[i] = _childWidgets[i].color;

				_hoveredColor = new Color[_defaultColor.Length];
				for (byte i = 0; i < _hoveredColor.Length; i++)
				{
					if (OverrideColors)
						_hoveredColor[i] = HoveredColor;
					else
					{
						_hoveredColor[i] = _defaultColor[i] * (HoveredColor * 2);
						_hoveredColor[i].a = _defaultColor[i].a;
					}
				}

				_pressedColor = new Color[_defaultColor.Length];
				for (byte i = 0; i < _pressedColor.Length; i++)
				{
					if (OverrideColors)
						_pressedColor[i] = PressedColor;
					else
					{
						_pressedColor[i] = _defaultColor[i] * (PressedColor * 2);
						_pressedColor[i].a = _defaultColor[i].a;
					}
				}

				if (HasDisabledColors)
				{
					_disabledColor = new Color[_defaultColor.Length];
					for (byte i = 0; i < _disabledColor.Length; i++)
					{
						if (OverrideColors)
							_disabledColor[i] = DisabledColor;
						else
						{
							_disabledColor[i] = _defaultColor[i] * (DisabledColor * 2);
							_disabledColor[i].a = _defaultColor[i].a;
						}
					}
				}

				_tweenColor = new Tweener[_defaultColor.Length];
			}

			if (HasHighlightAnimation)
				HighlightSprite.alpha = 0;

			_tweenParams = new TweenParams ();
			_tweenParams.SetEase (Ease.InOutSine).SetUpdate (true);

			_tweenParamsCallback = new TweenParams ();
			_tweenParamsCallback.SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(OnPressAnimationComplete);

			_started = true;
		}

		#endregion

		#region Custom Events

		#region OnClick

		protected virtual void OnClick()
		{
			if (!_started)
				Start ();

			if (!isActivated)
			{
				if (string.IsNullOrEmpty (_deactivatedWarningMessageOverride))
				{
					if (!string.IsNullOrEmpty (DefaultDeactivatedWarningMessage))
					{
						WarningHandler.Instance.Warning (DefaultDeactivatedWarningMessage);
						if (_hasClickSound)
							MasterAudio.PlaySoundAndForget (_clickSoundName);
					}
				}
				else
				{
					WarningHandler.Instance.Warning (_deactivatedWarningMessageOverride);
					if (_hasClickSound)
						MasterAudio.PlaySoundAndForget (_clickSoundName);
				}
			}

			if (!isActivated)
				return;
            
            if (LongPress)
				return;
            
            if (SupportDoubleClick)
			{
				if (_tooFastClick)
				{
					StopCoroutine ("RunDoubleClickWindow");
					_tooFastClick = false;
					_alreadySentMessage = false;
					OnDoubleClickInternal();
					return;
				}
				else
					StartCoroutine ("RunDoubleClickWindow");
			}
            
            if (Toggle)
			{
				if (_isToggled)
				{
					OnToggleOff();
					return;
				}

				_isToggled = true;
			}
            
            StopTween ();

			_callbackRegistered = false;
			_currentAnimationDuration = AnimationDuration;


			if (HasScaleAnimation)
			{
				_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _pressedScale, _currentAnimationDuration).SetAs(_callbackRegistered ? _tweenParams : _tweenParamsCallback);

				if (!Toggle && !_callbackRegistered)
					_callbackRegistered = true;
			}
			
			if (HasColorAnimation)
			{
				for (byte i = 0; i < _defaultColor.Length; i++)
				{
					AnimateColors(i, _pressedColor[i], _currentAnimationDuration, _callbackRegistered ? _tweenParams : _tweenParamsCallback);

					if (!Toggle && !_callbackRegistered)
						_callbackRegistered = true;
				}
			}
			
			if (HasHighlightAnimation)
			{
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 1f, _currentAnimationDuration).SetAs(_callbackRegistered ? _tweenParams : _tweenParamsCallback);

				if (!Toggle && !_callbackRegistered)
					_callbackRegistered = true;
			}
			
			if (_hasClickSound)
				MasterAudio.PlaySoundAndForget (_clickSoundName);

			if ((UICamera.currentTouchID < 0 && UICamera.currentTouchID == -1) || UICamera.currentTouchID >= 0)
				EventDelegate.Execute(onClick);
			else if (UICamera.currentTouchID == -2)
				EventDelegate.Execute(onLeftClick);
			
			_callbackRegistered = false;
        }

		#endregion

		#region Toggle Off

		protected virtual void OnToggleOff()
		{
			if (!_started)
				Start ();

			if (!isActivated)
				return;

			_isToggled = false;

			StopTween ();

			_currentAnimationDuration = AnimationDuration;
			
			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _defaultScale, _currentAnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
			{
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _defaultColor[i], _currentAnimationDuration, _tweenParams);
			}
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0f, _currentAnimationDuration).SetAs(_tweenParams);

			EventDelegate.Execute(onToggleOff);
		}

		#endregion

		#region Double Click

		private void OnDoubleClickInternal()
		{
			if (!_alreadySentMessage)
			{
				_alreadySentMessage = true;
				EventDelegate.Execute (onDoubleClick);
			}
		}

		#endregion

		#region Long Press

		protected virtual void OnCustomLongPress(bool[] args)
		{
			if (!_started)
				Start ();

			if (!isActivated && (LongPress || LongClick))
			{
				if (!args [0] && args [1])
				{
					if (string.IsNullOrEmpty (_deactivatedWarningMessageOverride))
					{
						if (!string.IsNullOrEmpty (DefaultDeactivatedWarningMessage))
							WarningHandler.Instance.Warning (DefaultDeactivatedWarningMessage);
					}
					else
						WarningHandler.Instance.Warning (_deactivatedWarningMessageOverride);
				}
			}

			if (!isActivated)
				return;

			if (SupportDoubleClick && args[0])
			{
				if (_tooFastClick)
					return;
			}

			if (LongClick)
				OnLongClick (args[0]);

			if (!LongPress)
				return;

			_callbackRegistered = false;

			StopTween ();

			if (args[0])
			{
				_currentAnimationDuration = AnimationDuration;

				if (HasScaleAnimation)
					_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _hoveredScale, _currentAnimationDuration).SetAs(_tweenParams);

				if (HasColorAnimation)
					for (byte i = 0; i < _defaultColor.Length; i++)
						AnimateColors(i, _hoveredColor[i], _currentAnimationDuration, _tweenParams);

				if (HasHighlightAnimation)
					_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 1f, _currentAnimationDuration).SetAs(_tweenParams);

				if (_hasDownSound)
					MasterAudio.PlaySoundAndForget(_downSoundName);
			}
			else
			{
				if (args[1])
				{
					_currentAnimationDuration = AnimationDuration * 0.5f;

					if (HasScaleAnimation)
					{
						_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _pressedScale, _currentAnimationDuration).SetAs(_callbackRegistered ? _tweenParams : _tweenParamsCallback);

						if (!_callbackRegistered)
							_callbackRegistered = true;
					}

					if (HasColorAnimation)
					{
						for (byte i = 0; i < _defaultColor.Length; i++)
							AnimateColors(i, _pressedColor[i], _currentAnimationDuration, _callbackRegistered ? _tweenParams : _tweenParamsCallback);
							
						if (!_callbackRegistered)
							_callbackRegistered = true;
					}

					if (HasHighlightAnimation)
					{
						_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 1f, _currentAnimationDuration).SetAs(_callbackRegistered ? _tweenParams : _tweenParamsCallback);

						if (!_callbackRegistered)
							_callbackRegistered = true;
					}

					if (_hasClickSound)
						MasterAudio.PlaySoundAndForget(_clickSoundName);

					if ((UICamera.currentTouchID < 0 && UICamera.currentTouchID == -1) || UICamera.currentTouchID >= 0)
						EventDelegate.Execute(onClick);
					else if (UICamera.currentTouchID == -2)
						EventDelegate.Execute(onLeftClick);
				}
				else
				{
					_currentAnimationDuration = AnimationDuration;

					if (HasScaleAnimation)
						_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _defaultScale, _currentAnimationDuration).SetAs(_tweenParams);

					if (HasColorAnimation)
						for (byte i = 0; i < _defaultColor.Length; i++)
							AnimateColors(i, _defaultColor[i], _currentAnimationDuration, _tweenParams);

					if (HasHighlightAnimation)
						_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0f, _currentAnimationDuration).SetAs(_tweenParams);

					if (_hasUpSound)
						MasterAudio.PlaySoundAndForget(_upSoundName);
				}
			}

			_callbackRegistered = false;
		}

		#endregion

		#region Long Click

		protected virtual void OnLongClick(bool started)
		{
			if (!started)
			{
				EventDelegate.Execute(onLongClickEnd);
				StopCoroutine ("LongClickCheck");
			}
			else
				StartCoroutine ("LongClickCheck");
		}

		private IEnumerator LongClickCheck()
		{
			yield return new WaitForSeconds (LongClickDelay);
			EventDelegate.Execute(onLongClickStart);
		}

		#endregion

		protected virtual void OnPressAnimationComplete()
		{
			if (!isActivated)
				return;

			StopTween ();

			_currentAnimationDuration = AnimationDuration * 0.5f;

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _defaultScale, _currentAnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _defaultColor[i], _currentAnimationDuration, _tweenParams);
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0f, _currentAnimationDuration).SetAs(_tweenParams);
		}

		#endregion

		#region Helper Methods
		
		protected virtual void StopTween()
		{
			if (_tweenScale != null)
				_tweenScale.Kill ();

			if (HasColorAnimation)
			{
				if (_tweenColor != null &&_tweenColor [0] != null)
				{
					for (byte i = 0; i < _tweenColor.Length; i++)
						_tweenColor [i].Kill ();
				}
			}

			if (_tweenHighlight != null)
				_tweenHighlight.Kill ();
		}

		protected virtual void AnimateColors(int index, Color targetColor, float animationDuration, TweenParams tweenParams)
		{
			_tweenColor [index] = DOTween.To(() => _childWidgets[index].color, x => _childWidgets[index].color = x, targetColor, animationDuration).SetAs(tweenParams);
		}

		public virtual void UpdateDefaultColor(UIWidget widget, Color newColor)
		{
			var index = _childWidgets.IndexOf (widget);
			if (index == -1)
				return;

			var lastColor = _defaultColor [index];
			_defaultColor[index] = newColor;
			if (_childWidgets [index].color == lastColor)
			{
				if (_tweenColor[index] != null)
					_tweenColor[index].Kill();

				_tweenColor [index] = DOTween.To(() => _childWidgets[index].color, x => _childWidgets[index].color = x, _defaultColor [index], _currentAnimationDuration).SetAs(_tweenParams);
			}
		}

		private IEnumerator RunDoubleClickWindow()
		{
			_tooFastClick = true;
			yield return new WaitForSeconds (DoubleClickWindow);
			_alreadySentMessage = false;
			_tooFastClick = false;
		}

		#endregion

		#region Activation

		public virtual void DeactivateBehaviour(bool invisible = false, string deactivatedMessage = null)
		{
			if (!_started)
				Start ();

			_deactivatedWarningMessageOverride = deactivatedMessage;

			if (!isActivated)
				return;

			isActivated = false;

			if (invisible)
			{
				SetInvisible();
				return;
			}

			StopTween ();
			
			_isToggled = false;
			_currentAnimationDuration = AnimationDuration;

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _defaultScale, _currentAnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _tweenColor.Length; i++)
					AnimateColors(i, HasDisabledColors ? _disabledColor [i] : _defaultColor [i], _currentAnimationDuration, _tweenParams);

			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0f, _currentAnimationDuration).SetAs(_tweenParams);
		}

		public virtual void ActivateBehaviour(bool visible = false)
		{
			if (!_started)
				Start ();

			_deactivatedWarningMessageOverride = null;
			isActivated = true;

			if (visible)
			{
				SetVisible();
				return;
			}

			StopTween ();

			_currentAnimationDuration = AnimationDuration;

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => transform.localScale, x => transform.localScale = x, _defaultScale, _currentAnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _defaultColor [i], _currentAnimationDuration, _tweenParams);
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0f, _currentAnimationDuration).SetAs(_tweenParams);
		}

		private void SetInvisible()
		{
			StopTween ();

			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, new Color(_defaultColor [i].r, _defaultColor [i].g, _defaultColor [i].b, 0), _currentAnimationDuration, _tweenParams);
		}

		private void SetVisible()
		{
			StopTween ();
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _defaultColor[i], _currentAnimationDuration, _tweenParams);
		}

		#endregion
	}
}