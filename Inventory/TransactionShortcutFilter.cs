using System;
using System.Windows.Forms;

namespace Inventory
{
    internal sealed class TransactionShortcutFilter : IMessageFilter
    {
        private const int WM_KEYDOWN = 0x0100;

        public event EventHandler Triggered;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WM_KEYDOWN || (Keys)m.WParam.ToInt32() != Keys.Divide)
                return false;

            Triggered?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
