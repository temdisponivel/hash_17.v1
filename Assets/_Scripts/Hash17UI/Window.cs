using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FH.Util.Extensions;
using Hash17.Utils;
using JetBrains.Annotations;

public class Window : MonoBehaviour
{
    #region Inner Types

    public enum ContentFitType
    {
        Fit,
        Strech,
        Free,
    }

    #endregion

    #region Properties

    #region References

    public UILabel TitleLabel;
    public UISprite Topbar;
    public UISprite Background;
    public UIDragObject DragObject;
    public UIPanel MainPanel;
    public UIPanel ContentPanel;
    public GameObject CloseButton;
    public GameObject MaximizeButton;

    #endregion

    #region Data

    public bool LosesFocus;
    public float TransitionTime;
    public float OnFocusAlpha;
    public float OutOfFocusAlpha;

    #endregion

    #region State

    private UIWidget _content;
    private bool _isMaximized;
    private Rect _rectBeforeMaximize;
    private Vector3 _initialScale;
    private Tweener _focusTweener;

    #endregion

    #region Controls

    public string Title { get { return TitleLabel.text; } set { TitleLabel.text = value; } }
    
    public Vector2 Position
    {
        get { return MainPanel.transform.position; }
        set { MainPanel.transform.localPosition = value; }
    }
    public Vector2 ScreenRelativePosition
    {
        get
        {
            var rootSize = Alias.Term.RootPanel.GetViewSize();
            var finalX = Position.x / rootSize.x;
            var finalY = Position.y / rootSize.y;
            return new Vector2(finalX, finalY);
        }
    }
    public Vector2 Size
    {
        get { return MainPanel.GetViewSize(); }
        set { MainPanel.SetRect(Position.x, Position.y, value.x, value.y); }
    }

    [SerializeField]
    private ContentFitType _contentFit;
    public ContentFitType ContentFit
    {
        get { return _contentFit; }
        set
        {
            _contentFit = value;
            AdjustContent();
        }
    }

    [SerializeField]
    private bool _positionLocked;
    public bool PositionLocked
    {
        get { return _positionLocked; }
        set
        {
            _positionLocked = value;
            DragObject.enabled = !_positionLocked;
            ShowMaximizeButton &= !_positionLocked;
        }
    }

    [SerializeField]
    private bool _showCloseButton;
    public bool ShowCloseButton
    {
        get { return _showCloseButton; }
        set
        {
            _showCloseButton = value;
            CloseButton.SetActive(_showCloseButton);
        }
    }

    [SerializeField]
    private bool _showMaximizeButton;
    public bool ShowMaximizeButton
    {
        get { return _showMaximizeButton; }
        set
        {
            _showMaximizeButton = value;
            MaximizeButton.SetActive(_showMaximizeButton);
        }
    }

    public Color BackgroundColor
    {
        get { return Background.color; }
        set { Background.color = value; }
    }

    #endregion

    #endregion

    #region Static methods

    public static Window Create()
    {
        var windowPrefab = Alias.GameConfig.WindowPrefab;
        var windowIntance = NGUITools.AddChild(Alias.Term.RootPanel.gameObject, windowPrefab).GetComponent<Window>();
        return windowIntance;
    }

    #endregion

    #region Unity callback

    void Awake()
    {
        _initialScale = transform.localScale;
        _content = new UIWidget();
    }

    void Start()
    {
        Open(null);
    }

    #endregion

    #region UI Callback

    public void OnFocus()
    {
        if (_focusTweener != null)
            _focusTweener.Kill();

        _focusTweener = DOTween.To(() => MainPanel.alpha, value => MainPanel.alpha = value, OnFocusAlpha, TransitionTime).OnComplete(
            () =>
            {
                _focusTweener = null;
            });
    }

    public void OnUnfocus()
    {
        if (!LosesFocus)
            return;

        if (_focusTweener != null)
            _focusTweener.Kill();

        _focusTweener = DOTween.To(() => MainPanel.alpha, value => MainPanel.alpha = value, OutOfFocusAlpha, TransitionTime).OnComplete(
            () =>
            {
                _focusTweener = null;
            });
    }

    public void CloseButtonClick()
    {
        Close(null);
    }

    #endregion

    #region Control methods

    #region Setup

    public void Setup(string title,
        UIWidget content,
        ContentFitType fitType,
        bool showCloseButtons = true,
        bool showMaximizeButton = true,
        bool lockPosition = false,
        bool startClosed = false)
    {
        Title = title;
        _contentFit = fitType;
        ShowCloseButton = showCloseButtons;
        ShowMaximizeButton = ShowMaximizeButton;
        PositionLocked = lockPosition;
        if (startClosed)
            transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        ShowObject(content);
    }

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    #endregion

    #region Opening/closing

    public void Open(Action callback)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(_initialScale, TransitionTime).OnComplete(() =>
        {
            if (callback != null)
                callback();
        });
    }

    public void Close(Action callback, bool destroyAfter = true)
    {
        transform.DOScale(Vector3.zero, TransitionTime).OnComplete(() =>
        {
            if (callback != null)
                callback();

            if (destroyAfter)
                Destroy();
        });
    }

    #endregion

    #region Maximize

    public void ToggleMaximize()
    {
        if (_isMaximized)
            Restore();
        else
            Maximize();

        _isMaximized = !_isMaximized;
    }

    public void Maximize()
    {
        _rectBeforeMaximize = new Rect(ScreenRelativePosition, MainPanel.GetViewSize());
        var rootSize = Alias.Term.RootPanel.GetViewSize();
        Size = rootSize;
        Position = Vector2.zero;
        Alias.Term.RootPanel.ConstrainTargetToBounds(MainPanel.transform, true);
    }

    public void Restore()
    {

        MainPanel.SetRect(_rectBeforeMaximize.x, _rectBeforeMaximize.y, _rectBeforeMaximize.width, _rectBeforeMaximize.height);
    }

    #endregion

    #region _content

    public void AdjustContent()
    {
        switch (ContentFit)
        {
            case ContentFitType.Fit:
                AdjustFitContent();
                break;
            case ContentFitType.Strech:
                AdjustStrechContent();
                break;
        }
    }

    private void AdjustFitContent()
    {
        Size = _content.localSize;
    }

    private void AdjustStrechContent()
    {
        _content.SetAnchor(ContentPanel.transform);
    }

    #endregion

    #endregion

    #region Object

    public void ShowObject(UIWidget content)
    {
        if (_content != null)
            HideObject();

        content.transform.SetParent(ContentPanel.transform);
        content.transform.Reset();
        _content = content;
        AdjustContent();
    }

    public void HideObject()
    {
        if (_contentFit == ContentFitType.Strech)
            _content.SetAnchor((GameObject)null);
        _content.transform.SetParent(null);
        _content = null;
    }

    #endregion
}