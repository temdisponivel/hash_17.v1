using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using DG.Tweening;
using FH.Util.Extensions;
using Hash17.Utils;
using JetBrains.Annotations;

public class Window : MonoBehaviour
{
    #region Properties

    #region References

    public UIScrollView ContentScrollView;
    public UILabel TitleLabel;
    public UISprite Topbar;
    public UISprite Background;
    public UIDragObject DragObject;
    public UIWidget LeftResizer;
    public UIWidget RightResizer;
    public UIWidget TopResizer;
    public UIWidget BottomResizer;
    public UIPanel MainPanel;
    public UIPanel ContentPanel;
    public GameObject CloseButton;

    #endregion

    #region Data

    public bool LosesFocus;
    public float TransitionTime;
    public float OnFocusAlpha;
    public float OutOfFocusAlpha;
    public int TopBarHeight;

    #endregion

    #region State

    private UIWidget _content;
    private Vector3 _initialScale;
    private Tweener _focusTweener;

    #endregion

    #region Controls

    public string Title { get { return TitleLabel.text; } set { TitleLabel.text = value; } }
    
    public Vector2 Size
    {
        get { return new Vector2(Background.width, Background.height); }
        set
        {
            value = Vector2.Min(MainPanel.GetViewSize() / 3, value);

            var rightResizerAnchors = GetAnchorPoints(RightResizer);
            var leftResizerAnchors = GetAnchorPoints(LeftResizer);
            var bottomResizerAnchors = GetAnchorPoints(BottomResizer);
            var topResizerAnchors = GetAnchorPoints(TopResizer);
            var backgroundAnchors = GetAnchorPoints(Background);

            var rightResizerSize = RightResizer.Size();
            var leftResizerSize = LeftResizer.Size();
            var bottomResizerSize = BottomResizer.Size();
            var topResizerSize = TopResizer.Size();

            Background.SetAnchor((GameObject) null, 0, 0, 0, 0);
            RightResizer.SetAnchor(Background.gameObject, 0, 0, 0, 0);
            LeftResizer.SetAnchor(Background.gameObject, 0, 0, 0, 0);
            BottomResizer.SetAnchor(Background.gameObject, 0, 0, 0, 0);
            TopResizer.SetAnchor(Background.gameObject, 0, 0, 0, 0);

            Background.width = (int) value.x;
            Background.height = (int) value.y;

            // Do this stuff in multiple frames because NGUI only
            // update anchors in different frames (if we change anchors more than 1 time per frame, it will make no difference)
            CoroutineHelper.Instance.WaitAndCallTimes((count) =>
            {
                if (count == 0)
                {
                    UpdateResizerAnchors();

                    // Update only the side that matters accordinly to resizer
                    rightResizerSize.y = RightResizer.Size().y;
                    leftResizerSize.y = LeftResizer.Size().y;
                    bottomResizerSize.x = BottomResizer.Size().x;
                    topResizerSize.x = TopResizer.Size().x;
                }
                else if (count == 1)
                {
                    // Update all resizers anchors and size to what was before
                    SetAnchorPoints(RightResizer, rightResizerAnchors, rightResizerSize);
                    SetAnchorPoints(LeftResizer, leftResizerAnchors, leftResizerSize);
                    SetAnchorPoints(BottomResizer, bottomResizerAnchors, bottomResizerSize);
                    SetAnchorPoints(TopResizer, topResizerAnchors, topResizerSize);
                    SetAnchorPoints(Background, backgroundAnchors, default(Vector2));
                }
                else if (count == 2)
                {
                    // Update resizer's anchors
                    UpdateResizerAnchors();
                }
                else if (count == 3)
                {
                    UpdateScrolls();
                }
            }, 4, null, null);
        }
    }
    
    [SerializeField]
    private bool _sizeLocked;
    public bool SizeLocked
    {
        get { return _sizeLocked; }
        set
        {
            _sizeLocked = value;
            LeftResizer.enabled = !_sizeLocked;
            RightResizer.enabled = !_sizeLocked;
            TopResizer.enabled = !_sizeLocked;
            BottomResizer.enabled = !_sizeLocked;
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

    public Color BackgroundColor
    {
        get { return Background.color; }
        set { Background.color = value; }
    }

    public int StartDepth { get; private set; }

    private HashSet<GameObject> _hovedObjects = new HashSet<GameObject>();

    #endregion

    #region Static

    private static readonly List<Window> _allOpenedWindows = new List<Window>();
    private static int _currentWindowQueue { get; set; }

    #endregion

    #endregion

    #region Events

    public event Action OnOpen;
    public event Action OnClose;

    #endregion

    #region Static methods

    public static Window Create()
    {
        var windowPrefab = Alias.Config.WindowPrefab;
        var windowIntance = NGUITools.AddChild(Alias.Term.RootPanel.gameObject, windowPrefab).GetComponent<Window>();

        windowIntance.StartDepth = _currentWindowQueue;

        windowIntance.MainPanel.depth = ++_currentWindowQueue;
        windowIntance.ContentPanel.depth = ++_currentWindowQueue;

        _allOpenedWindows.Add(windowIntance);

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
        UICamera.onClick += OnClickCamera;
        UICamera.onHover += OnHoverCamera;
    }

    void OnDestroy()
    {
        UICamera.onClick -= OnClickCamera;
        UICamera.onHover -= OnHoverCamera;
    }

    #endregion

    #region UI Callback

    public void Click()
    {
        BringToFront();
    }
    
    public void OnFocus()
    {
        if (_focusTweener != null)
            _focusTweener.Kill();

        _focusTweener = DOTween.To(() => MainPanel.alpha, value => MainPanel.alpha = value, OnFocusAlpha, TransitionTime).OnComplete(
            () =>
            {
                _focusTweener = null;
            });

        BringToFront();
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
        bool showCloseButtons = true,
        bool showMaximizeButton = true,
        bool lockPosition = false,
        bool startClosed = false)
    {
        Title = title;
        ShowCloseButton = showCloseButtons;
        if (startClosed)
            transform.localScale = Vector3.zero;
        ShowObject(content);
    }

    public void Destroy()
    {
        _allOpenedWindows.Remove(this);
        DestroyImmediate(gameObject);
    }

    #endregion

    #region Opening/closing

    public void Open(Action callback)
    {
        if (OnOpen != null)
            OnOpen();

        transform.localScale = Vector3.zero;
        transform.DOScale(_initialScale, TransitionTime).OnComplete(() =>
        {
            if (callback != null)
                callback();

            UpdateScrolls();
        });
        gameObject.SetActive(true);
    }

    public void Close(Action callback, bool destroyAfter = true)
    {
        if (OnClose != null)
            OnClose();

        transform.DOScale(Vector3.zero, TransitionTime).OnComplete(() =>
        {
            if (callback != null)
                callback();

            if (destroyAfter)
                Destroy();

            UpdateScrolls();
        });
    }

    #endregion
    
    #region Stack

    public void BringToFront()
    {
        for (int i = 0; i < _allOpenedWindows.Count; i++)
        {
            var currentWindow = _allOpenedWindows[i];
            if (currentWindow.MainPanel.depth == 999)
            {
                currentWindow.MainPanel.depth = currentWindow.StartDepth + 1;
                currentWindow.ContentPanel.depth = currentWindow.StartDepth + 2;
            }
        }

        MainPanel.depth = 999;
        ContentPanel.depth = 1000;
    }

    #endregion

    #endregion

    #region Object

    public void ShowObject(UIWidget content)
    {
        UpdateScrolls();

        if (_content != null)
            HideObject();

        content.transform.SetParent(ContentPanel.transform);
        content.transform.Reset();
        _content = content;
        Size = new Vector2(_content.width, _content.height + TopBarHeight);

        UpdateScrolls();
    }

    public void HideObject()
    {
        _content.transform.SetParent(null);
        _content = null;
    }

    private void UpdateScrolls()
    {
        ContentScrollView.ResetPosition();
    }

    #endregion

    #region UI Camera callback

    void OnClickCamera(GameObject clicked)
    {
        if (clicked.transform.IsChildOf(this.transform))
        {
            Click();
        }
    }

    void OnHoverCamera(GameObject hovered, bool state)
    {
        if (hovered.transform.IsChildOf(this.transform))
        {
            if (state)
            {   
                if (_hovedObjects.Contains(hovered))
                    return;

                _hovedObjects.Add(hovered);
                OnFocus();
            }
            else
            {
                if (_hovedObjects.Contains(hovered))
                    _hovedObjects.Remove(hovered);

                if (_hovedObjects.Count == 0)
                    OnUnfocus();
            }
        }
    }

    #endregion

    #region Helpers

    public List<UIRect.AnchorPoint> GetAnchorPoints(UIWidget widget)
    {
        var result = new List<UIRect.AnchorPoint>();
        result.Add(widget.rightAnchor.Clone());
        result.Add(widget.leftAnchor.Clone());
        result.Add(widget.bottomAnchor.Clone());
        result.Add(widget.topAnchor.Clone());
        return result;
    }

    public void SetAnchorPoints(UIWidget widget, List<UIRect.AnchorPoint> anchorPoints, Vector2 size)
    {
        widget.rightAnchor.target = anchorPoints[0].target;
        widget.rightAnchor.relative = anchorPoints[0].relative;
        widget.rightAnchor.absolute = anchorPoints[0].absolute;

        widget.leftAnchor.target = anchorPoints[1].target;
        widget.leftAnchor.relative = anchorPoints[1].relative;
        widget.leftAnchor.absolute = anchorPoints[1].absolute;

        widget.bottomAnchor.target = anchorPoints[2].target;
        widget.bottomAnchor.relative = anchorPoints[2].relative;
        widget.bottomAnchor.absolute = anchorPoints[2].absolute;

        widget.topAnchor.target = anchorPoints[3].target;
        widget.topAnchor.relative = anchorPoints[3].relative;
        widget.topAnchor.absolute = anchorPoints[3].absolute;

        if (size != default(Vector2))
            widget.Size(size);
    }

    public void UpdateResizerAnchors()
    {
        Background.ResetAndUpdateAnchors();
        RightResizer.ResetAndUpdateAnchors();
        LeftResizer.ResetAndUpdateAnchors();
        BottomResizer.ResetAndUpdateAnchors();
        TopResizer.ResetAndUpdateAnchors();
    }

    #endregion
}