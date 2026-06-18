using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AttributeMqlGenerator
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Log("Starting");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += delegate(object sender, System.Threading.ThreadExceptionEventArgs e)
                {
                    ShowError(e.Exception);
                };
                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
                {
                    ShowError(e.ExceptionObject as Exception);
                };
                Application.Run(new MainForm());
                Log("Exited");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static string LogPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AttributeMqlGenerator.log");
        }

        internal static void Log(string message)
        {
            File.AppendAllText(LogPath(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message + Environment.NewLine);
        }

        private static void ShowError(Exception ex)
        {
            string message = ex == null ? "Unknown error" : ex.ToString();
            try
            {
                Log("ERROR " + message);
            }
            catch
            {
            }

            MessageBox.Show(
                message + Environment.NewLine + Environment.NewLine + "Log: " + LogPath(),
                "Attribute MQL Generator Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    internal sealed class MainForm : Form
    {
        private readonly TextBox attributeName = new TextBox();
        private readonly TextBox descriptionKo = new TextBox();
        private readonly TextBox descriptionEn = new TextBox();
        private readonly ComboBox attributeType = new ComboBox();
        private readonly TextBox defaultValue = new TextBox();
        private readonly TextBox createdDate = new TextBox();
        private readonly TextBox installedDate = new TextBox();
        private readonly TextBox createdBy = new TextBox();
        private readonly TextBox application = new TextBox();
        private readonly TextBox version = new TextBox();
        private readonly TextBox installer = new TextBox();
        private readonly CheckBox multiline = new CheckBox();
        private readonly CheckBox hidden = new CheckBox();
        private readonly TextBox rangeInput = new TextBox();
        private readonly ListBox rangeList = new ListBox();
        private readonly TextBox preview = new TextBox();
        private readonly Label status = new Label();
        private readonly ToolTip statusTip = new ToolTip();
        private readonly TableLayoutPanel shell = new TableLayoutPanel();
        private readonly SplitContainer root = new SplitContainer();
        private AppSettings settings = new AppSettings();

        public MainForm()
        {
            Text = "Attribute MQL Generator";
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            MinimumSize = new Size(1040, 720);
            Size = new Size(1220, 780);

            settings = AppSettings.Load();
            var menu = BuildMenu();
            MainMenuStrip = menu;
            shell.Dock = DockStyle.Fill;
            shell.ColumnCount = 1;
            shell.RowCount = 2;
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(shell);
            shell.Controls.Add(menu, 0, 0);
            BuildLayout();
            SetDefaults();
            WireEvents();
            UpdatePreview();
            Shown += delegate
            {
                Program.Log("Shown");
                int desired = 430;
                int max = Math.Max(root.Panel1MinSize, root.Width - 80);
                root.SplitterDistance = Math.Max(root.Panel1MinSize, Math.Min(desired, max));
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

        private void BuildLayout()
        {
            root.Dock = DockStyle.Fill;
            root.FixedPanel = FixedPanel.Panel1;
            root.Panel1MinSize = 360;
            root.Panel1.Padding = new Padding(12);
            root.Panel2.Padding = new Padding(12);
            shell.Controls.Add(root, 0, 1);

            var formPanel = new TableLayoutPanel();
            formPanel.Dock = DockStyle.Fill;
            formPanel.ColumnCount = 2;
            formPanel.RowCount = 16;
            formPanel.AutoScroll = true;
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            formPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.Panel1.Controls.Add(formPanel);

            AddField(formPanel, "Name", attributeName, 0, 0, 2);
            AddField(formPanel, "Description KO", descriptionKo, 1, 0, 1);
            AddField(formPanel, "Description EN", descriptionEn, 1, 1, 1);

            attributeType.DropDownStyle = ComboBoxStyle.DropDownList;
            attributeType.Items.AddRange(new object[] { "string", "date", "integer", "real", "boolean" });
            AddField(formPanel, "Type", attributeType, 2, 0, 1);
            AddField(formPanel, "Default", defaultValue, 2, 1, 1);

            AddField(formPanel, "Created Date", createdDate, 3, 0, 1);
            AddField(formPanel, "Installed Date", installedDate, 3, 1, 1);
            AddField(formPanel, "Create by", createdBy, 4, 0, 1);
            AddField(formPanel, "Application", application, 4, 1, 1);
            AddField(formPanel, "Version", version, 5, 0, 1);
            AddField(formPanel, "Installer", installer, 5, 1, 1);

            var checks = new FlowLayoutPanel();
            checks.Dock = DockStyle.Fill;
            checks.Height = 34;
            checks.FlowDirection = FlowDirection.LeftToRight;
            multiline.Text = "multiline";
            multiline.AutoSize = true;
            hidden.Text = "hidden";
            hidden.AutoSize = true;
            checks.Controls.Add(multiline);
            checks.Controls.Add(hidden);
            formPanel.Controls.Add(checks, 0, 6);
            formPanel.SetColumnSpan(checks, 2);

            var rangeTitle = new Label();
            rangeTitle.Text = "Range";
            rangeTitle.Font = new Font(Font.FontFamily, 10, FontStyle.Bold);
            rangeTitle.Dock = DockStyle.Fill;
            rangeTitle.Padding = new Padding(0, 12, 0, 0);
            formPanel.Controls.Add(rangeTitle, 0, 7);
            formPanel.SetColumnSpan(rangeTitle, 2);

            var rangeInputPanel = new TableLayoutPanel();
            rangeInputPanel.Dock = DockStyle.Fill;
            rangeInputPanel.ColumnCount = 2;
            rangeInputPanel.RowCount = 1;
            rangeInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rangeInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 74));
            rangeInputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var addRange = new Button();
            addRange.Text = "Add";
            addRange.Dock = DockStyle.Fill;
            addRange.Click += delegate { AddRange(); };
            rangeInput.Dock = DockStyle.Fill;
            rangeInputPanel.Controls.Add(rangeInput, 0, 0);
            rangeInputPanel.Controls.Add(addRange, 1, 0);
            formPanel.Controls.Add(rangeInputPanel, 0, 8);
            formPanel.SetColumnSpan(rangeInputPanel, 2);

            rangeList.Dock = DockStyle.Fill;
            rangeList.Height = 130;
            formPanel.Controls.Add(rangeList, 0, 9);
            formPanel.SetColumnSpan(rangeList, 2);

            var rangeButtons = new FlowLayoutPanel();
            rangeButtons.Dock = DockStyle.Fill;
            var removeRange = new Button();
            removeRange.Text = "Remove";
            removeRange.Width = 86;
            removeRange.Click += delegate { RemoveSelectedRange(); };
            var clearRange = new Button();
            clearRange.Text = "Clear";
            clearRange.Width = 72;
            clearRange.Click += delegate
            {
                rangeList.Items.Clear();
                UpdatePreview();
            };
            rangeButtons.Controls.Add(removeRange);
            rangeButtons.Controls.Add(clearRange);
            formPanel.Controls.Add(rangeButtons, 0, 10);
            formPanel.SetColumnSpan(rangeButtons, 2);

            var rightPanel = new TableLayoutPanel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.ColumnCount = 1;
            rightPanel.RowCount = 3;
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            root.Panel2.Controls.Add(rightPanel);

            var actionPanel = new FlowLayoutPanel();
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.FlowDirection = FlowDirection.LeftToRight;
            actionPanel.Padding = new Padding(0, 4, 0, 4);
            rightPanel.Controls.Add(actionPanel, 0, 0);

            var copy = new Button();
            copy.Text = "Copy";
            copy.Width = 80;
            copy.Height = 28;
            copy.Click += delegate
            {
                Clipboard.SetText(preview.Text);
                SetStatus("Copied", string.Empty);
            };

            var saveSchema = new Button();
            saveSchema.Text = "Save to schema";
            saveSchema.Width = 120;
            saveSchema.Height = 28;
            saveSchema.Click += delegate { SaveToSchema(); };

            var saveAs = new Button();
            saveAs.Text = "Save As";
            saveAs.Width = 90;
            saveAs.Height = 28;
            saveAs.Click += delegate { SaveAs(); };

            status.Dock = DockStyle.Fill;
            status.AutoEllipsis = true;
            status.ForeColor = Color.DimGray;
            status.TextAlign = ContentAlignment.MiddleLeft;
            status.Padding = new Padding(4, 0, 4, 0);
            rightPanel.Controls.Add(status, 0, 2);

            actionPanel.Controls.Add(copy);
            actionPanel.Controls.Add(saveSchema);
            actionPanel.Controls.Add(saveAs);

            preview.Dock = DockStyle.Fill;
            preview.Multiline = true;
            preview.ScrollBars = ScrollBars.Both;
            preview.WordWrap = false;
            preview.Font = new Font("Consolas", 10);
            preview.BackColor = Color.FromArgb(17, 24, 39);
            preview.ForeColor = Color.FromArgb(229, 231, 235);
            rightPanel.Controls.Add(preview, 0, 1);
        }

        private MenuStrip BuildMenu()
        {
            var menu = new MenuStrip();
            menu.Dock = DockStyle.Fill;

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

        private void AddField(TableLayoutPanel panel, string labelText, Control input, int row, int column, int span)
        {
            var container = new TableLayoutPanel();
            container.Dock = DockStyle.Fill;
            container.RowCount = 2;
            container.ColumnCount = 1;
            container.Padding = new Padding(0, 4, 8, 4);
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

            var label = new Label();
            label.Text = labelText;
            label.Dock = DockStyle.Fill;
            label.ForeColor = Color.DimGray;
            label.Font = new Font(Font.FontFamily, 8, FontStyle.Bold);
            input.Dock = DockStyle.Fill;

            container.Controls.Add(label, 0, 0);
            container.Controls.Add(input, 0, 1);
            panel.Controls.Add(container, column, row);
            if (span > 1)
            {
                panel.SetColumnSpan(container, span);
            }
        }

        private void SetDefaults()
        {
            var now = DateTime.Now;
            attributeName.Text = "custSampleAttribute";
            descriptionKo.Text = "샘플 속성";
            descriptionEn.Text = "Sample Attribute";
            attributeType.SelectedItem = "string";
            createdDate.Text = now.ToString("yyyy.MM.dd");
            installedDate.Text = now.ToString("yyyy-MM-dd");
            createdBy.Text = Environment.GetEnvironmentVariable("CREATED_BY");
            if (string.IsNullOrWhiteSpace(createdBy.Text))
            {
                createdBy.Text = settings.CreatedBy;
            }
            application.Text = settings.Application;
            version.Text = settings.Version;
            installer.Text = settings.Installer;
        }

        private void WireEvents()
        {
            foreach (Control control in AllInputs(this))
            {
                var textBox = control as TextBox;
                if (textBox != null && textBox != preview)
                {
                    textBox.TextChanged += delegate { UpdatePreview(); };
                }

                var combo = control as ComboBox;
                if (combo != null)
                {
                    combo.SelectedIndexChanged += delegate { UpdatePreview(); };
                }

                var check = control as CheckBox;
                if (check != null)
                {
                    check.CheckedChanged += delegate { UpdatePreview(); };
                }
            }

            rangeInput.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    AddRange();
                }
            };
        }

        private IEnumerable<Control> AllInputs(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control inner in AllInputs(child))
                {
                    yield return inner;
                }
            }
        }

        private void AddRange()
        {
            if (rangeInput.Text.Length == 0)
            {
                return;
            }

            rangeList.Items.Add(rangeInput.Text);
            rangeInput.Clear();
            rangeInput.Focus();
            UpdatePreview();
        }

        private void RemoveSelectedRange()
        {
            while (rangeList.SelectedIndices.Count > 0)
            {
                rangeList.Items.RemoveAt(rangeList.SelectedIndices[0]);
            }
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            preview.Text = NormalizeLineEndings(BuildMql());
        }

        private string BuildMql()
        {
            string descriptionComment = string.Empty;
            if (!string.IsNullOrWhiteSpace(descriptionKo.Text) && descriptionKo.Text != descriptionEn.Text)
            {
                descriptionComment = " #" + descriptionKo.Text;
            }

            var ranges = new StringBuilder();
            foreach (object item in rangeList.Items)
            {
                ranges.Append("    range = '").Append(EscapeMql(item.ToString())).Append("'\r\n");
            }

            var tokens = new Dictionary<string, string>();
            tokens["{{CREATED_DATE}}"] = createdDate.Text.Trim();
            tokens["{{CREATED_BY}}"] = createdBy.Text.Trim();
            tokens["{{DESCRIPTION_KO}}"] = descriptionKo.Text.Trim();
            tokens["{{ATTRIBUTE_NAME}}"] = attributeName.Text.Trim();
            tokens["{{DESCRIPTION_EN}}"] = descriptionEn.Text.Trim();
            tokens["{{DESCRIPTION_COMMENT}}"] = descriptionComment;
            tokens["{{ATTRIBUTE_TYPE}}"] = Convert.ToString(attributeType.SelectedItem);
            tokens["{{DEFAULT_VALUE}}"] = EscapeMql(defaultValue.Text);
            tokens["{{RANGES}}"] = ranges.ToString();
            tokens["{{MULTILINE_FLAG}}"] = multiline.Checked ? "multiline" : "notmultiline";
            tokens["{{HIDDEN_FLAG}}"] = hidden.Checked ? "hidden" : "nothidden";
            tokens["{{APPLICATION}}"] = application.Text.Trim();
            tokens["{{VERSION}}"] = version.Text.Trim();
            tokens["{{INSTALLER}}"] = installer.Text.Trim();
            tokens["{{INSTALLED_DATE}}"] = installedDate.Text.Trim();

            string content = TemplateText();
            foreach (var item in tokens)
            {
                content = content.Replace(item.Key, item.Value);
            }

            return NormalizeLineEndings(content);
        }

        private static string NormalizeLineEndings(string value)
        {
            return (value ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
        }

        private static string EscapeMql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static string TemplateText()
        {
            return @"################################################################################################
# Created Date : {{CREATED_DATE}}
# Create by {{CREATED_BY}}
# Description : {{DESCRIPTION_KO}}
################################################################################################
#del attribute '{{ATTRIBUTE_NAME}}';
add attribute '{{ATTRIBUTE_NAME}}'
    description '{{DESCRIPTION_EN}}'{{DESCRIPTION_COMMENT}}
    type '{{ATTRIBUTE_TYPE}}'
    default '{{DEFAULT_VALUE}}'
{{RANGES}}    {{MULTILINE_FLAG}}
    {{HIDDEN_FLAG}}
    property    'application'    value '{{APPLICATION}}'
    property    'version'        value '{{VERSION}}'
    property    'installer'      value '{{INSTALLER}}'
    property    'installed date' value '{{INSTALLED_DATE}}'
    property    'original name'  value '{{ATTRIBUTE_NAME}}'
;
add property 'attribute_{{ATTRIBUTE_NAME}}' on program eServiceSchemaVariableMapping.tcl to attribute '{{ATTRIBUTE_NAME}}';";
        }

        private string CurrentFileName()
        {
            string name = attributeName.Text.Trim();
            if (name.Length == 0)
            {
                name = "attribute";
            }
            return name + ".mql";
        }

        private void SaveToSchema()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string targetDir = ResolveSaveFolder(baseDir);
            Directory.CreateDirectory(targetDir);
            string targetPath = Path.Combine(targetDir, CurrentFileName());

            if (File.Exists(targetPath))
            {
                DialogResult result = MessageBox.Show(
                    "File already exists. Overwrite?\r\n" + targetPath,
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            WriteMql(targetPath);
            SetStatus("Saved to schema: " + CurrentFileName(), targetPath);
        }

        private void SaveAs()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "MQL files (*.mql)|*.mql|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.FileName = CurrentFileName();
                dialog.Title = "Save MQL";

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    WriteMql(dialog.FileName);
                    SetStatus("Saved as: " + Path.GetFileName(dialog.FileName), dialog.FileName);
                }
            }
        }

        private void WriteMql(string path)
        {
            File.WriteAllText(path, preview.Text, new UTF8Encoding(true));
        }

        private void SetStatus(string message, string detail)
        {
            status.Text = message;
            statusTip.SetToolTip(status, string.IsNullOrEmpty(detail) ? message : detail);
        }

        private string ResolveSaveFolder(string baseDir)
        {
            string folder = settings.SaveFolder;
            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = @"..\..\01.Attribute";
            }

            if (Path.IsPathRooted(folder))
            {
                return Path.GetFullPath(folder);
            }

            return Path.GetFullPath(Path.Combine(baseDir, folder));
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

    internal sealed class AppSettings
    {
        public string Application = "Framework";
        public string Version = "R2026x";
        public string Installer = "CUST";
        public string CreatedBy = "USER";
        public string SaveFolder = @"..\..\01.Attribute";

        public static string ConfigPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AttributeMqlGenerator.ini");
        }

        public static AppSettings Load()
        {
            var settings = new AppSettings();
            string path = ConfigPath();
            if (!File.Exists(path))
            {
                return settings;
            }

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue;
                }

                int index = line.IndexOf('=');
                if (index < 0)
                {
                    continue;
                }

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1);
                if (key.Equals("Application", StringComparison.OrdinalIgnoreCase)) settings.Application = value;
                if (key.Equals("Version", StringComparison.OrdinalIgnoreCase)) settings.Version = value;
                if (key.Equals("Installer", StringComparison.OrdinalIgnoreCase)) settings.Installer = value;
                if (key.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase)) settings.CreatedBy = value;
                if (key.Equals("SaveFolder", StringComparison.OrdinalIgnoreCase)) settings.SaveFolder = value;
            }

            return settings;
        }

        public void Save()
        {
            var lines = new List<string>();
            lines.Add("Application=" + Application);
            lines.Add("Version=" + Version);
            lines.Add("Installer=" + Installer);
            lines.Add("CreatedBy=" + CreatedBy);
            lines.Add("SaveFolder=" + SaveFolder);
            File.WriteAllLines(ConfigPath(), lines.ToArray(), Encoding.UTF8);
        }
    }

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
            Settings = new AppSettings();
            Settings.Application = current.Application;
            Settings.Version = current.Version;
            Settings.Installer = current.Installer;
            Settings.CreatedBy = current.CreatedBy;
            Settings.SaveFolder = current.SaveFolder;

            Text = "Settings";
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
            panel.Padding = new Padding(12);
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
            browse.Dock = DockStyle.Fill;
            browse.Click += delegate { BrowseFolder(); };
            AddRow(panel, 4, "Save Folder", saveFolder, browse);

            var buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Fill;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            var ok = new Button();
            ok.Text = "OK";
            ok.Width = 80;
            ok.Click += delegate { Accept(); };
            var cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Width = 80;
            cancel.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
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
            input.Dock = DockStyle.Fill;

            panel.Controls.Add(label, 0, row);
            panel.Controls.Add(input, 1, row);
            if (extra != null)
            {
                panel.Controls.Add(extra, 2, row);
            }
            else
            {
                var spacer = new Label();
                spacer.Dock = DockStyle.Fill;
                panel.Controls.Add(spacer, 2, row);
            }
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
