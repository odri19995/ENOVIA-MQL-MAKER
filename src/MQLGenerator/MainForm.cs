using System;
using System.Drawing;
using System.Windows.Forms;

namespace MqlGenerator
{
    internal sealed partial class MainForm : Form
    {
        private AppSettings settings = new AppSettings();

        public MainForm()
        {
            Text = "MQL Generator";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            UiStyle.StyleForm(this);
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            MinimumSize = new Size(1040, 720);
            Size = new Size(1220, 780);

            settings = AppSettings.Load();
            var menu = BuildMenu();
            MainMenuStrip = menu;

            shell.Dock = DockStyle.Fill;
            shell.BackColor = UiStyle.Window;
            shell.ColumnCount = 1;
            shell.RowCount = 2;
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(shell);
            shell.Controls.Add(menu, 0, 0);

            BuildAttributeView();
            SetDefaults();
            WireEvents();
            UpdatePreview();

            Shown += delegate
            {
                Program.Log("Shown");
                int desired = 430;
                int max = Math.Max(root.Panel1MinSize, root.Width - 80);
                root.SplitterDistance = Math.Max(root.Panel1MinSize, Math.Min(desired, max));
                ResetInputScrollPositions();
                TopMost = true;
                BringToFront();
                Activate();

                var timer = new Timer();
                timer.Interval = 1500;
                timer.Tick += delegate
                {
                    TopMost = false;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            };
        }

        private MenuStrip BuildMenu()
        {
            var menu = new MenuStrip();
            menu.Dock = DockStyle.Fill;
            menu.Font = UiStyle.DefaultFont;
            menu.BackColor = UiStyle.Surface;
            menu.ForeColor = UiStyle.Text;
            menu.Padding = new Padding(8, 2, 0, 2);
            menu.RenderMode = ToolStripRenderMode.System;

            var file = new ToolStripMenuItem("File");
            file.DropDownItems.Add("Save to schema", null, delegate { SaveToSchema(); });
            file.DropDownItems.Add("Save As", null, delegate { SaveAs(); });
            file.DropDownItems.Add(new ToolStripSeparator());
            file.DropDownItems.Add("Exit", null, delegate { Close(); });

            var edit = new ToolStripMenuItem("Edit");
            edit.DropDownItems.Add("Copy MQL", null, delegate
            {
                Clipboard.SetText(preview.Text);
                SetStatus("Copied", string.Empty);
            });
            edit.DropDownItems.Add("Clear Range", null, delegate
            {
                rangeList.Items.Clear();
                UpdatePreview();
            });

            var tools = new ToolStripMenuItem("Tools");
            tools.DropDownItems.Add("Settings...", null, delegate { OpenSettings(); });

            menu.Items.Add(file);
            menu.Items.Add(edit);
            menu.Items.Add(tools);
            return menu;
        }

        private void ResetInputScrollPositions()
        {
            foreach (Control control in AllInputs(this))
            {
                var textBox = control as TextBox;
                if (textBox == null || textBox == preview || string.IsNullOrEmpty(textBox.Text))
                {
                    continue;
                }

                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
            }

            attributeName.Focus();
            attributeName.SelectionStart = 0;
            attributeName.SelectionLength = 0;
        }

        private void OpenSettings()
        {
            settings.Application = application.Text.Trim();
            settings.Version = version.Text.Trim();
            settings.Installer = installer.Text.Trim();
            settings.CreatedBy = createdBy.Text.Trim();

            using (var dialog = new SettingsForm(settings))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                settings = dialog.Settings;
                settings.Save();
                application.Text = settings.Application;
                version.Text = settings.Version;
                installer.Text = settings.Installer;
                createdBy.Text = settings.CreatedBy;
                UpdatePreview();
                SetStatus("Settings saved", AppSettings.ConfigPath());
            }
        }
    }
}
