using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using FH.Util.Time;
using FH.UI.Panels;

namespace FH.UI.Animations
{
	public class UIAnimation : MonoBehaviour
	{
		#region Config Variables

		public BasePanel OwnerPanel;
		public bool StartsOnOpenPanel;
		public bool StartsOnClosePanel;
		public float Duration;
		public float Delay;
		public Ease Ease = Ease.InOutSine;

		#endregion

		#region Position Variables
        
		public bool AnimatePosition;
	    public bool UnscaledTimePos = true;
	    public bool UseNGUIAnimation;
		public bool UseCurrentPositionXForInitial;
		public bool UseCurrentPositionYForInitial;
		public bool UseCurrentPositionXForFinal;
		public bool UseCurrentPositionYForFinal;
		public Vector2 InitialPosition;
		public Vector2 FinalPosition;
	    public UpdateType PosUpdateType = UpdateType.Normal;

		#endregion

		#region Scale Variables

		public bool AnimateScale;
		public bool UseSpriteForScale;
		public bool UseCurrentScaleXForInitial;
		public bool UseCurrentScaleYForInitial;
		public bool UseCurrentScaleXForFinal;
		public bool UseCurrentScaleYForFinal;
		public Vector2 InitialScale;
		public Vector2 FinalScale;
		
		#endregion

		#region Alpha Variables

		public bool AnimateAlpha;
		public float InitialAlpha;
		public float FinalAlpha;

		#endregion

		#region Cache Variables

		private float _delay;
		private Vector3 _initialPosition;
		private Vector3 _finalPosition;
		private Vector3 _initialScale;
		private Vector3 _finalScale;

		private Vector3 _targetPosition;
		private Vector3 _targetScale;
		private float _targetAlpha;

		private Transform _transform;
		private UIWidget _sprite;

		private Tweener _positionTweener;
		private Tweener _alphaTweener;
		private Tweener _scaleSpriteWidthTweener;
		private Tweener _scaleSpriteHeightTweener;
		private Tweener _scaleTweener;

		#endregion

		#region Event Variables

		public event Action OnStartAnimation;
		public event Action<UIAnimation> OnCompleteAnimation;

		#endregion

		#region Unity Events

		private void Awake()
		{
			_transform = transform;

			if (AnimatePosition)
			{
				_initialPosition.x = UseCurrentPositionXForInitial ? _transform.localPosition.x : InitialPosition.x;
				_initialPosition.y = UseCurrentPositionYForInitial ? _transform.localPosition.y : InitialPosition.y;
				_initialPosition.z = _transform.localPosition.z;
				_finalPosition.x = UseCurrentPositionXForFinal ? _transform.localPosition.x : FinalPosition.x;
				_finalPosition.y = UseCurrentPositionYForFinal ? _transform.localPosition.y : FinalPosition.y;
				_finalPosition.z = _transform.localPosition.z;
			}
			
			if (AnimateScale)
			{
				if (UseSpriteForScale)
				{
					_sprite = GetComponent<UIWidget>();
					
					_initialScale.x = UseCurrentScaleXForInitial ? _sprite.width : InitialScale.x;
					_initialScale.y = UseCurrentScaleYForInitial ? _sprite.height : InitialScale.y;
					_finalScale.x = UseCurrentScaleXForFinal ? _sprite.width : FinalScale.x;
					_finalScale.y = UseCurrentScaleYForFinal ? _sprite.height : FinalScale.y;
				}
				else
				{
					_initialScale.x = UseCurrentScaleXForInitial ? _transform.localScale.x : InitialScale.x;
					_initialScale.y = UseCurrentScaleYForInitial ? _transform.localScale.y : InitialScale.y;
					_finalScale.x = UseCurrentScaleXForFinal ? _transform.localScale.x : FinalScale.x;
					_finalScale.y = UseCurrentScaleYForFinal ? _transform.localScale.y : FinalScale.y;
				}
			}
			
			if (AnimateAlpha)
			{
				if (_sprite == null)
					_sprite = GetComponent<UIWidget> ();
			}

			if (OwnerPanel == null)
				return;

			if (StartsOnOpenPanel)
			{
				OwnerPanel.onOpenPanelStart += OnTriggerAnimation;
				Relocate ();
			}

			if (StartsOnClosePanel)
				OwnerPanel.onClosePanelStart += OnTriggerAnimation;
		}

		private void OnDestroy()
		{
			if (_positionTweener != null)
				_positionTweener.Kill ();

			if (_scaleSpriteWidthTweener != null)
				_scaleSpriteWidthTweener.Kill ();
			
			if (_scaleSpriteHeightTweener != null)
				_scaleSpriteHeightTweener.Kill ();

			if (_scaleTweener != null)
				_scaleTweener.Kill ();

			if (_alphaTweener != null)
				_alphaTweener.Kill ();

			if (OwnerPanel == null)
				return;

			if (StartsOnOpenPanel)
				OwnerPanel.onOpenPanelStart -= OnTriggerAnimation;
			
			if (StartsOnClosePanel)
				OwnerPanel.onClosePanelStart -= OnTriggerAnimation;
		}

		#endregion

		#region Initializing Methods

		public void Relocate()
		{
			if (AnimatePosition)
				_transform.localPosition = _initialPosition;
			
			if (AnimateScale)
			{
				if (UseSpriteForScale)
				{
					_sprite.width = (int)_initialScale.x;
					_sprite.height = (int)_initialScale.y;
				}
				else
					_transform.localScale = new Vector2(_initialScale.x, _initialScale.y);
			}
			
			if (AnimateAlpha)
				_sprite.alpha = InitialAlpha;
		}

		#endregion
		
		#region Panel Events Handling

		public void OnTriggerAnimation(BasePanel panel)
		{
			OwnerPanel.StartTask ();

			if (AnimatePosition)
				_targetPosition = _finalPosition;
			
			if (AnimateScale)
				_targetScale = _finalScale;
			
			if (AnimateAlpha)
				_targetAlpha = FinalAlpha;

			StartCoroutine(Animate());
		}

		#endregion

		#region On Demand Animations

		public void StartAnimation(bool backwards = false)
		{
			if (!gameObject.activeInHierarchy)
				return;
			
			if (OwnerPanel != null)
				OwnerPanel.StartTask ();

			if (!backwards)
			{
				if (AnimatePosition)
					_targetPosition = _finalPosition;
				
				if (AnimateScale)
					_targetScale = _finalScale;
				
				if (AnimateAlpha)
					_targetAlpha = FinalAlpha;
			}
			else
			{
				if (AnimatePosition)
					_targetPosition = _initialPosition;
				
				if (AnimateScale)
					_targetScale = _initialScale;
				
				if (AnimateAlpha)
					_targetAlpha = InitialAlpha;
			}

			StartCoroutine(Animate());
		}

		#endregion

		#region Animation Methods

		private IEnumerator Animate()
		{
			yield return StartCoroutine (TimeUtil.WaitForRealSeconds (0.1f));

			if (OnStartAnimation != null)
				OnStartAnimation ();

			_delay = Delay;
			
			if (AnimatePosition)
				InnerAnimatePosition();
			
			if (AnimateScale)
				InnerAnimateScale();
			
			if (AnimateAlpha)
				InnerAnimateAlpha();
			
			StartCoroutine(CompleteAnimation(_delay + Duration));
		}

		private void InnerAnimatePosition()
		{
            if (UseNGUIAnimation)
				StartCoroutine (StartNGUIAnimation ());
			else
			{
				_positionTweener = DOTween.To (() => _transform.localPosition, x => _transform.localPosition = x, _targetPosition, Duration)
                    .SetDelay (_delay)
		            .SetEase (Ease)
		            .SetUpdate (PosUpdateType, UnscaledTimePos);
			}
		}

	    private IEnumerator StartNGUIAnimation()
	    {
            yield return new WaitForSeconds(_delay);
            TweenPosition.Begin(gameObject, Duration, _targetPosition, false).method = UITweener.Method.EaseInOut;
	    }

		private void InnerAnimateScale()
		{
			if (UseSpriteForScale)
			{
				_scaleSpriteWidthTweener = DOTween.To (() => _sprite.width, x => _sprite.width = x, (int)_targetScale.x, Duration).SetDelay (_delay).SetEase (Ease).SetUpdate (true);
				_scaleSpriteHeightTweener = DOTween.To (() => _sprite.height, x => _sprite.height = x, (int)_targetScale.y, Duration).SetDelay (_delay).SetEase (Ease).SetUpdate (true);
				return;
			}

			_scaleTweener = DOTween.To (() => _transform.localScale, x => _transform.localScale = x, _targetScale, Duration).SetDelay (_delay).SetEase (Ease).SetUpdate (true);
		}

		private void InnerAnimateAlpha()
		{
			_alphaTweener = DOTween.To (() => _sprite.alpha, x => _sprite.alpha = x, _targetAlpha, Duration).SetDelay (_delay).SetEase (Ease).SetUpdate (true);
		}

		private IEnumerator CompleteAnimation(float delayToComplete)
		{
			yield return StartCoroutine(TimeUtil.WaitForRealSeconds(delayToComplete));

			if (OnCompleteAnimation != null)
				OnCompleteAnimation (this);

			if (OwnerPanel != null)
				OwnerPanel.CompleteTask ();
		}

		#endregion
	}
}