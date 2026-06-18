using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MqlGenerator
{
    internal static class UiStyle
    {
        private const int EmSetMargins = 0x00D3;
        private const int EcLeftMargin = 0x0001;
        private const int EcRightMargin = 0x0002;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        internal static readonly Color Window = Color.FromArgb(244, 247, 249);
        internal static readonly Color Surface = Color.White;
        internal static readonly Color Border = Color.FromArgb(207, 216, 222);
        internal static readonly Color Text = Color.FromArgb(32, 44, 52);
        internal static readonly Color Muted = Color.FromArgb(95, 109, 118);
        internal static readonly Color Accent = Color.FromArgb(0, 121, 107);
        internal static readonly Color AccentHover = Color.FromArgb(0, 105, 92);
        internal static readonly Font DefaultFont = new Font("Segoe UI", 9F, FontStyle.Regular);

        internal static void StyleForm(Form form)
        {
            form.Font = DefaultFont;
            form.BackColor = Window;
            form.ForeColor = Text;
        }

        internal static void StyleInput(Control control)
        {
            control.Font = DefaultFont;
            control.BackColor = Surface;
            control.ForeColor = Text;
            control.Margin = new Padding(0);

            var textBox = control as TextBox;
            if (textBox != null)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.HandleCreated += delegate { SetTextBoxMargins(textBox, 6, 6); };
                if (textBox.IsHandleCreated)
                {
                    SetTextBoxMargins(textBox, 6, 6);
                }
            }

            var comboBox = control as ComboBox;
            if (comboBox != null)
            {
                comboBox.FlatStyle = FlatStyle.Standard;
                comboBox.IntegralHeight = false;
                comboBox.DropDownHeight = 160;
            }
        }

        internal static void StyleButton(Button button, bool primary)
        {
            button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.Cursor = Cursors.Hand;
            button.Margin = new Padding(0, 0, 8, 0);

            if (primary)
            {
                button.BackColor = Accent;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = Accent;
                button.FlatAppearance.MouseOverBackColor = AccentHover;
                button.FlatAppearance.MouseDownBackColor = AccentHover;
                return;
            }

            button.BackColor = Surface;
            button.ForeColor = Text;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(236, 241, 243);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 234, 237);
        }

        private static void SetTextBoxMargins(TextBox textBox, int left, int right)
        {
            int margins = left | (right << 16);
            SendMessage(
                textBox.Handle,
                EmSetMargins,
                new IntPtr(EcLeftMargin | EcRightMargin),
                new IntPtr(margins));
        }
    }
}
