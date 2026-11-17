using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    internal sealed class LayoutEditMessageFilter : IMessageFilter
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONUP = 0x0202;

        private const int MinControlSize = 10;

        private readonly Control _root;

        private Control _dragging;
        private Point _dragStartMouseScreen;
        private Point _dragStartControlLocation;

        private Control _resizing;
        private Point _resizeStartMouseScreen;
        private Size _resizeStartControlSize;

        public LayoutEditMessageFilter(Control root)
        {
            _root = root;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WM_LBUTTONDOWN && m.Msg != WM_MOUSEMOVE && m.Msg != WM_LBUTTONUP)
                return false;

            Control target = Control.FromChildHandle(m.HWnd);
            if (target == null || !IsWithin(target, _root))
                return _dragging != null || _resizing != null;

            if (m.Msg == WM_LBUTTONDOWN)
            {
                Point mouseScreen = Control.MousePosition;

                if (LayoutEditOverlay.HitTestHandle(target, mouseScreen, out Control resizeTarget))
                {
                    _resizing = resizeTarget;
                    _resizeStartMouseScreen = mouseScreen;
                    _resizeStartControlSize = resizeTarget.Size;
                    return true;
                }

                if (!LayoutEditOverlay.IsMovable(target))
                    return false;

                _dragging = target;
                _dragStartMouseScreen = mouseScreen;
                _dragStartControlLocation = target.Location;
                return true;
            }

            if (m.Msg == WM_MOUSEMOVE && _resizing != null)
            {
                Point mouseNow = Control.MousePosition;
                int newWidth = _resizeStartControlSize.Width + (mouseNow.X - _resizeStartMouseScreen.X);
                int newHeight = _resizeStartControlSize.Height + (mouseNow.Y - _resizeStartMouseScreen.Y);
                _resizing.Size = new Size(Math.Max(MinControlSize, newWidth), Math.Max(MinControlSize, newHeight));
                return true;
            }

            if (m.Msg == WM_MOUSEMOVE && _dragging != null)
            {
                Point mouseNow = Control.MousePosition;
                int newX = _dragStartControlLocation.X + (mouseNow.X - _dragStartMouseScreen.X);
                int newY = _dragStartControlLocation.Y + (mouseNow.Y - _dragStartMouseScreen.Y);
                _dragging.Location = new Point(newX, newY);
                return true;
            }

            if (m.Msg == WM_LBUTTONUP)
            {
                bool wasActive = _dragging != null || _resizing != null;
                _dragging = null;
                _resizing = null;
                return wasActive;
            }

            return false;
        }

        private static bool IsWithin(Control control, Control root)
        {
            for (Control c = control; c != null; c = c.Parent)
            {
                if (c == root)
                    return true;
            }

            return false;
        }
    }
}
