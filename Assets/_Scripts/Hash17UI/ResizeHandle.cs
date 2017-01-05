using System;
using UnityEngine;

namespace Hash17.Hash17UI
{
    public class ResizeHandle
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
        public Window WindowToResize;

        #endregion

        #region Control

        private Vector3 _deltaDrag;
        
        #endregion
        
        #endregion

        void OnDrag(Vector3 delta)
        {
            _deltaDrag = delta;
            switch (Pivot)
            {
                case Side.Left:
                case Side.Right:
                    DragHorizontal();
                    break;
                case Side.Top:
                case Side.Bottom:
                    DragVertical();
                    break;
            }
        }

        void DragHorizontal()
        {
            var quantity = _deltaDrag.x;
            var windowSize = WindowToResize.MainPanel.GetViewSize();
            var windowPos = WindowToResize.MainPanel.transform.position;
            WindowToResize.MainPanel.SetRect(windowPos.x, windowSize.y, windowSize.x + quantity, windowSize.y);
        }

        void DragVertical()
        {
            var quantity = _deltaDrag.y;
            var windowSize = WindowToResize.MainPanel.GetViewSize();
            var windowPos = WindowToResize.MainPanel.transform.position;
            WindowToResize.MainPanel.SetRect(windowPos.x, windowSize.y, windowSize.x, windowSize.y + quantity);
        }
    }
}