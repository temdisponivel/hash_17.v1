using System;
using UnityEngine;

namespace Hash17.Hash17UI
{
    public class ResizeHandle : MonoBehaviour
    {
        #region Inner type

        public enum Side
        {
            Left,
            Right,
            Top,
            Bottom,
        }

        #endregion

        #region Properties

        #region References

        public Side Pivot;
        public UIWidget Widget;
        public Window WindowToResize;

        #endregion

        #region Control

        private Vector3 _deltaDrag;
        
        #endregion
        
        #endregion

        void OnDragStart()
        {
            switch (Pivot)
            {
                case Side.Left:
                    Widget.leftAnchor.target = null;
                    Widget.rightAnchor.target = null;
                    WindowToResize.MainPanel.leftAnchor.target = transform;
                    WindowToResize.MainPanel.rightAnchor.target = transform;
                    break;
                case Side.Right:
                    break;
                case Side.Top:
                    break;
                case Side.Bottom:
                    break;
            }
        }
    }
}