using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FH.Util.Extensions;
using JetBrains.Annotations;

public class Window : MonoBehaviour
{
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

    private bool _isMaximized;
    private Rect _rectBeforeMaximize;
    private Vector3 _initialScale;
    private Tweener _focusTweener;

    #endregion

    #region Controls

    public string Title { get { return TitleLabel.text; } set { TitleLabel.text = value; } }

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
            MaximizeButton.SetActive(_showCloseButton);
        }
    }

    #endregion

    #endregion

    #region Unity callback

    void Awake()
    {
        _initialScale = transform.localScale;
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

    public void Setup(string title, GameObject content, bool showCloseButtons = true, bool showMaximizeButton = true, bool lockPosition = false, bool startClosed = false)
    {
        Title = title;
        ShowObject(content);
        ShowCloseButton = showCloseButtons;
        ShowMaximizeButton = ShowMaximizeButton;
        PositionLocked = lockPosition;
        if (startClosed)
            transform.localScale = Vector3.zero;
    }

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

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

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
        _rectBeforeMaximize = new Rect(MainPanel.transform.position, MainPanel.GetViewSize());
        var uiRoot = FindObjectOfType<UIRoot>();
        var rootSize = uiRoot.GetComponent<UIPanel>().GetViewSize();
        MainPanel.SetRect(uiRoot.transform.position.x, uiRoot.transform.position.y, rootSize.x, rootSize.y);
    }

    public void Restore()
    {
        MainPanel.SetRect(_rectBeforeMaximize.x, _rectBeforeMaximize.y, _rectBeforeMaximize.width, _rectBeforeMaximize.height);
    }

    #endregion

    #region Object

    public void ShowObject(GameObject gameObject)
    {
        gameObject.transform.SetParent(ContentPanel.transform);
        gameObject.transform.Reset();
    }

    public void HideObject(GameObject gameObject)
    {
        gameObject.transform.SetParent(null);
    }

    #endregion

    #endregion
}