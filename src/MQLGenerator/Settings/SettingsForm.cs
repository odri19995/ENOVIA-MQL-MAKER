using System.Drawing;
using System.Windows.Forms;

namespace MqlGenerator
{
    internal sealed class SettingsForm : Form
    {
        private readonly TextBox application = new TextBox();
        private readonly TextBox version = new TextBox();
        private readonly TextBox installer = new TextBox();
        private readonly TextBox createdBy = new TextBox();
        private readonly TextBox saveFolder = new TextBox();

        public AppSettings Settings { get; private set; }

        public SettingsForm(AppSettings current)
        {
            Settings = new AppSettings
            {
                Application = current.Application,
                Version = current.Version,
                Installer = current.Installer,
                CreatedBy = current.CreatedBy,
                SaveFolder = current.SaveFolder
            };

            Text = "MQL Generator Settings";
            UiStyle.StyleForm(this);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(520, 250);

            BuildLayout();
            LoadValues();
        }

        private void BuildLayout()
        {
            var panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(18, 16, 18, 14);
            panel.BackColor = UiStyle.Window;
            panel.ColumnCount = 3;
            panel.RowCount = 6;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(panel);

            AddRow(panel, 0, "Application", application, null);
            AddRow(panel, 1, "Version", version, null);
            AddRow(panel, 2, "Installer", installer, null);
            AddRow(panel, 3, "Create by", createdBy, null);

            var browse = new Button();
            browse.Text = "Browse";
            UiStyle.StyleButton(browse, false);
            browse.Dock = DockStyle.None;
            browse.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            browse.Height = 24;
            browse.Margin = new Padding(8, 0, 0, 0);
            browse.Click += delegate { BrowseFolder(); };
            AddRow(panel, 4, "Save Folder", saveFolder, browse);

            var buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Fill;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Padding = new Padding(0, 10, 0, 0);
            buttons.BackColor = UiStyle.Window;

            var ok = new Button();
            ok.Text = "OK";
            ok.Width = 80;
            ok.Height = 30;
            UiStyle.StyleButton(ok, true);
            ok.Click += delegate { Accept(); };

            var cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Width = 80;
            cancel.Height = 30;
            UiStyle.StyleButton(cancel, false);
            cancel.Click += delegate
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            buttons.Controls.Add(ok);
            buttons.Controls.Add(cancel);
            panel.Controls.Add(buttons, 0, 5);
            panel.SetColumnSpan(buttons, 3);
        }

        private void AddRow(TableLayoutPanel panel, int row, string labelText, TextBox input, Control extra)
        {
            var label = new Label();
            label.Text = labelText;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.ForeColor = UiStyle.Muted;
            label.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);

            input.Dock = DockStyle.None;
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            input.Margin = new Padding(0);
            UiStyle.StyleInput(input);

            panel.Controls.Add(label, 0, row);
            panel.Controls.Add(input, 1, row);
            if (extra != null)
            {
                panel.Controls.Add(extra, 2, row);
                return;
            }

            var spacer = new Label();
            spacer.Dock = DockStyle.Fill;
            panel.Controls.Add(spacer, 2, row);
        }

        private void LoadValues()
        {
            application.Text = Settings.Application;
            version.Text = Settings.Version;
            installer.Text = Settings.Installer;
            createdBy.Text = Settings.CreatedBy;
            saveFolder.Text = Settings.SaveFolder;
        }

        private void BrowseFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select schema save folder";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    saveFolder.Text = dialog.SelectedPath;
                }
            }
        }

        private void Accept()
        {
            Settings.Application = application.Text.Trim();
            Settings.Version = version.Text.Trim();
            Settings.Installer = installer.Text.Trim();
            Settings.CreatedBy = createdBy.Text.Trim();
            Settings.SaveFolder = saveFolder.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
