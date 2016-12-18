using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DarkTonic.MasterAudio;
using DG.Tweening;
using FH.UI.Warnings;

namespace FH.UI.Buttons
{
	public class UIPassiveButtonEnhanced : MonoBehaviour
	{
		#region Config Variables

		public float AnimationDuration = 0.2f;
		public bool LongPress = true;
		public bool Click = true;
		public bool SupportDoubleClick = false;
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

		#region Color Variables

		public bool HasColorAnimation;
		public bool OverrideColors;
		public Color UnselectedColor = new Color (0.3f, 0.3f, 0.3f, 1);
		public Color SelectedColor = new Color (1f, 1f, 1f, 1);
		public Color DisabledColor = new Color (0.08f, 0.08f, 0.08f, 1);
		protected Color[] _defaultColor;
		protected Color[] _unselectedColor;
		protected Color[] _selectedColor;
		protected Color[] _disabledColor;

		#endregion

		#region Scale Variables

		public bool HasScaleAnimation;
		public bool MultiplyOriginal = true;
		public float UnselectedScaleMultiplier;
		public float SelectedScaleMultiplier;
		public float DisabledScaleMultiplier;
		public Vector3 UnselectedTargetScale;
		public Vector3 SelectedTargetScale;
		public Vector3 DisabledTargetScale;
		protected Vector3 _defaultScale;
		protected Vector3 _unselectedScale;
		protected Vector3 _selectedScale;
		protected Vector3 _disabledScale;

		#endregion

		#region Highlight Variables

		public bool HasHighlightAnimation;
		public UISprite HighlightSprite;

		#endregion

		#region Sound Variables

		public bool PlaySounds = true;
		public string ClickSoundGroup;
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
		public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

		#endregion

		#region Cache Variables

		protected Transform _transform;
		protected List<UIWidget> _childWidgets = new List<UIWidget>();
		protected Tweener _tweenScale;
		protected Tweener[] _tweenColor;
		protected Tweener _tweenHighlight;
		protected TweenParams _tweenParams;

		#endregion

		#region Unity Events

		protected virtual void Awake()
		{
			_transform = transform;
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
				_defaultScale = _transform.localScale;
				if (MultiplyOriginal)
				{
					_unselectedScale = _defaultScale * UnselectedScaleMultiplier;
					_selectedScale = _defaultScale * SelectedScaleMultiplier;
					_disabledScale = _defaultScale * DisabledScaleMultiplier;
				}
				else
				{
					_unselectedScale = UnselectedTargetScale;
					_selectedScale = SelectedTargetScale;
					_disabledScale = DisabledTargetScale;
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

				_unselectedColor = new Color[_defaultColor.Length];
				for (byte i = 0; i < _unselectedColor.Length; i++)
				{
					if (OverrideColors)
						_unselectedColor[i] = UnselectedColor;
					else
					{
						_unselectedColor[i] = _defaultColor[i] * (UnselectedColor * 2);
						_unselectedColor[i].a = _defaultColor[i].a;
					}
				}

				_selectedColor = new Color[_defaultColor.Length];
				for (byte i = 0; i < _selectedColor.Length; i++)
				{
					if (OverrideColors)
						_selectedColor[i] = SelectedColor;
					else
					{
						_selectedColor[i] = _defaultColor[i] * (SelectedColor * 2);
						_selectedColor[i].a = _defaultColor[i].a;
					}
				}

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

				_tweenColor = new Tweener[_defaultColor.Length];
			}

			if (HasHighlightAnimation)
				HighlightSprite.alpha = 0;

			_tweenParams = new TweenParams ();
			_tweenParams.SetEase (Ease.InOutSine).SetUpdate (true);

			_started = true;
		}

		#endregion

		#region States

		public void SetDefault()
		{
			if (!_started)
				Start ();

			StopTween ();

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => _transform.localScale, x => _transform.localScale = x, _defaultScale, AnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _defaultColor [i], AnimationDuration, _tweenParams);
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0, AnimationDuration).SetAs(_tweenParams);
		}

		public void SetDisabled()
		{
			if (!_started)
				Start ();

			StopTween ();

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => _transform.localScale, x => _transform.localScale = x, _disabledScale, AnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _disabledColor [i], AnimationDuration, _tweenParams);
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0, AnimationDuration).SetAs(_tweenParams);
		}

		public void SetSelected()
		{
			if (!_started)
				Start ();

			StopTween ();

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => _transform.localScale, x => _transform.localScale = x, _selectedScale, AnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _selectedColor [i], AnimationDuration, _tweenParams);
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 1, AnimationDuration).SetAs(_tweenParams);
		}

		public void SetUnselected()
		{
			if (!_started)
				Start ();

			StopTween ();

			if (HasScaleAnimation)
				_tweenScale = DOTween.To(() => _transform.localScale, x => _transform.localScale = x, _unselectedScale, AnimationDuration).SetAs(_tweenParams);
			
			if (HasColorAnimation)
				for (byte i = 0; i < _defaultColor.Length; i++)
					AnimateColors(i, _unselectedColor [i], AnimationDuration, _tweenParams);
			
			if (HasHighlightAnimation)
				_tweenHighlight = DOTween.To(() => HighlightSprite.alpha, x => HighlightSprite.alpha = x, 0, AnimationDuration).SetAs(_tweenParams);
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

			if (!Click)
				return;

			if (SupportDoubleClick)
			{
				if (_tooFastClick)
				{
					OnDoubleClickInternal();
					return;
				}
				StartCoroutine (RunDoubleClickWindow ());
			}
			
			if (_hasClickSound)
				MasterAudio.PlaySoundAndForget (_clickSoundName);

			EventDelegate.Execute(onClick);
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
			if (SupportDoubleClick)
			{
				if (_tooFastClick)
					return;
			}

			if (!_started)
				Start ();

			if (!isActivated && LongPress)
			{
				if (!args [0] && args [1])
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
			}

			if (!isActivated)
				return;

			if (!LongPress)
				return;

			StopTween ();

			if (args[0])
			{
				if (_hasDownSound)
					MasterAudio.PlaySoundAndForget(_downSoundName);
			}
			else
			{
				if (args[1])
				{
					if (_hasClickSound)
						MasterAudio.PlaySoundAndForget(_clickSoundName);

				    EventDelegate.Execute(onClick);
				}
				else
				{
					if (_hasUpSound)
						MasterAudio.PlaySoundAndForget(_upSoundName);
				}
			}
		}

		#endregion

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

				_tweenColor [index] = DOTween.To(() => _childWidgets[index].color, x => _childWidgets[index].color = x, _defaultColor[index], AnimationDuration).SetAs(_tweenParams);
			}
		}

		protected virtual void AnimateColors(int index, Color targetColor, float animationDuration, TweenParams tweenParams)
		{
			_tweenColor [index] = DOTween.To(() => _childWidgets[index].color, x => _childWidgets[index].color = x, targetColor, animationDuration).SetAs(tweenParams);
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

		public virtual void DeactivateBehaviour(string deactivatedMessage = null)
		{
			if (!_started)
				Start ();

			_deactivatedWarningMessageOverride = deactivatedMessage;

			if (!isActivated)
				return;

			isActivated = false;

			SetDisabled ();
		}

		public virtual void ActivateBehaviour()
		{
			if (!_started)
				Start ();

			_deactivatedWarningMessageOverride = null;
			isActivated = true;

			SetDefault ();
		}

		#endregion
	}
}
