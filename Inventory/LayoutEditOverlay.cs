using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
{
    internal static class LayoutEditOverlay
    {
        private static readonly Color BoxColor = Color.DeepSkyBlue;
        private const int HandleSize = 8;

        public static void Attach(Control root) => AttachRecursive(root);

        public static void Detach(Control root) => DetachRecursive(root);

        private static void AttachRecursive(Control control)
        {
            if (control.Controls.Count > 0)
                control.Paint += DrawBoundingBoxes;

            foreach (Control child in control.Controls)
                AttachRecursive(child);

            control.Invalidate(true);
        }

        private static void DetachRecursive(Control control)
        {
            if (control.Controls.Count > 0)
                control.Paint -= DrawBoundingBoxes;

            foreach (Control child in control.Controls)
                DetachRecursive(child);

            control.Invalidate(true);
        }

        private static void DrawBoundingBoxes(object sender, PaintEventArgs e)
        {
            var parent = (Control)sender;

            using (var pen = new Pen(BoxColor, 1) { DashStyle = DashStyle.Dash })
            using (var handleBrush = new SolidBrush(BoxColor))
            {
                foreach (Control child in parent.Controls)
                {
                    if (!IsMovable(child))
                        continue;

                    Rectangle box = Rectangle.Inflate(child.Bounds, 1, 1);
                    e.Graphics.DrawRectangle(pen, box);
                    e.Graphics.FillRectangle(handleBrush, HandleRect(box));
                }
            }
        }

        private static Rectangle HandleRect(Rectangle box) =>
            new Rectangle(box.Right - HandleSize / 2, box.Bottom - HandleSize / 2, HandleSize, HandleSize);

        public static bool HitTestHandle(Control container, Point screenPoint, out Control hitChild)
        {
            if (container.Controls.Count > 0)
            {
                Point local = container.PointToClient(screenPoint);
                foreach (Control child in container.Controls)
                {
                    if (!IsMovable(child))
                        continue;

                    Rectangle box = Rectangle.Inflate(child.Bounds, 1, 1);
                    if (HandleRect(box).Contains(local))
                    {
                        hitChild = child;
                        return true;
                    }
                }
            }

            hitChild = null;
            return false;
        }

        public static bool IsMovable(Control control) =>
            !string.IsNullOrEmpty(control.Name) &&
            !(control is Form) && !(control is TabControl) && !(control is TabPage) &&
            !(control is MenuStrip) && !(control is StatusStrip) &&
            !MainLayoutStore.UsesAutomaticLayout(control);
    }
}
