using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AttributeMqlGenerator
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

        private static void SetTextBoxMargins(TextBox textBox, int left, int right)
        {
            int margins = left | (right << 16);
            SendMessage(
                textBox.Handle,
                EmSetMargins,
                new IntPtr(EcLeftMargin | EcRightMargin),
                new IntPtr(margins));
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
            }
            else
            {
                button.BackColor = Surface;
                button.ForeColor = Text;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(236, 241, 243);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 234, 237);
            }
        }
    }

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

        private void BuildLayout()
        {
            root.Dock = DockStyle.Fill;
            root.FixedPanel = FixedPanel.Panel1;
            root.BackColor = UiStyle.Border;
            root.SplitterWidth = 1;
            root.Panel1MinSize = 360;
            root.Panel1.BackColor = UiStyle.Window;
            root.Panel2.BackColor = UiStyle.Surface;
            root.Panel1.Padding = new Padding(18, 16, 18, 16);
            root.Panel2.Padding = new Padding(20, 16, 20, 16);
            shell.Controls.Add(root, 0, 1);

            var formPanel = new TableLayoutPanel();
            formPanel.Dock = DockStyle.Fill;
            formPanel.BackColor = UiStyle.Window;
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
            checks.WrapContents = false;
            checks.Padding = new Padding(0, 7, 0, 0);
            multiline.Text = "multiline";
            multiline.AutoSize = false;
            multiline.Size = new Size(98, 26);
            multiline.CheckAlign = ContentAlignment.MiddleLeft;
            multiline.TextAlign = ContentAlignment.MiddleLeft;
            multiline.ForeColor = UiStyle.Text;
            multiline.Margin = new Padding(0);
            hidden.Text = "hidden";
            hidden.AutoSize = false;
            hidden.Size = new Size(82, 26);
            hidden.CheckAlign = ContentAlignment.MiddleLeft;
            hidden.TextAlign = ContentAlignment.MiddleLeft;
            hidden.ForeColor = UiStyle.Text;
            hidden.Margin = new Padding(0);
            checks.Controls.Add(multiline);
            checks.Controls.Add(hidden);
            formPanel.Controls.Add(checks, 0, 6);
            formPanel.SetColumnSpan(checks, 2);

            var rangeTitle = new Label();
            rangeTitle.Text = "Range";
            rangeTitle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            rangeTitle.ForeColor = UiStyle.Text;
            rangeTitle.Dock = DockStyle.Fill;
            rangeTitle.Padding = new Padding(0, 12, 0, 0);
            formPanel.Controls.Add(rangeTitle, 0, 7);
            formPanel.SetColumnSpan(rangeTitle, 2);

            var rangeInputPanel = new TableLayoutPanel();
            rangeInputPanel.Dock = DockStyle.Fill;
            rangeInputPanel.ColumnCount = 2;
            rangeInputPanel.RowCount = 1;
            rangeInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rangeInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84));
            rangeInputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rangeInputPanel.Padding = new Padding(0, 5, 0, 5);
            var addRange = new Button();
            addRange.Text = "Add";
            addRange.Dock = DockStyle.None;
            addRange.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            addRange.Height = 30;
            UiStyle.StyleButton(addRange, true);
            addRange.Margin = new Padding(10, 0, 0, 0);
            addRange.Click += delegate { AddRange(); };
            rangeInput.Dock = DockStyle.None;
            rangeInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            rangeInput.Margin = new Padding(0);
            UiStyle.StyleInput(rangeInput);
            rangeInputPanel.Controls.Add(rangeInput, 0, 0);
            rangeInputPanel.Controls.Add(addRange, 1, 0);
            formPanel.Controls.Add(rangeInputPanel, 0, 8);
            formPanel.SetColumnSpan(rangeInputPanel, 2);

            rangeList.Dock = DockStyle.Fill;
            rangeList.Height = 130;
            rangeList.BorderStyle = BorderStyle.FixedSingle;
            rangeList.BackColor = UiStyle.Surface;
            rangeList.ForeColor = UiStyle.Text;
            rangeList.Font = UiStyle.DefaultFont;
            rangeList.IntegralHeight = false;
            formPanel.Controls.Add(rangeList, 0, 9);
            formPanel.SetColumnSpan(rangeList, 2);

            var rangeButtons = new FlowLayoutPanel();
            rangeButtons.Dock = DockStyle.Fill;
            var removeRange = new Button();
            removeRange.Text = "Remove";
            removeRange.Width = 86;
            removeRange.Height = 30;
            UiStyle.StyleButton(removeRange, false);
            removeRange.Click += delegate { RemoveSelectedRange(); };
            var clearRange = new Button();
            clearRange.Text = "Clear";
            clearRange.Width = 72;
            clearRange.Height = 30;
            UiStyle.StyleButton(clearRange, false);
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
            rightPanel.BackColor = UiStyle.Surface;
            rightPanel.ColumnCount = 1;
            rightPanel.RowCount = 3;
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            root.Panel2.Controls.Add(rightPanel);

            var actionPanel = new FlowLayoutPanel();
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.FlowDirection = FlowDirection.LeftToRight;
            actionPanel.BackColor = UiStyle.Surface;
            actionPanel.Padding = new Padding(0, 5, 0, 5);
            rightPanel.Controls.Add(actionPanel, 0, 0);

            var copy = new Button();
            copy.Text = "Copy";
            copy.Width = 80;
            copy.Height = 30;
            UiStyle.StyleButton(copy, false);
            copy.Click += delegate
            {
                Clipboard.SetText(preview.Text);
                SetStatus("Copied", string.Empty);
            };

            var saveSchema = new Button();
            saveSchema.Text = "Save to schema";
            saveSchema.Width = 120;
            saveSchema.Height = 30;
            UiStyle.StyleButton(saveSchema, true);
            saveSchema.Click += delegate { SaveToSchema(); };

            var saveAs = new Button();
            saveAs.Text = "Save As";
            saveAs.Width = 90;
            saveAs.Height = 30;
            UiStyle.StyleButton(saveAs, false);
            saveAs.Click += delegate { SaveAs(); };

            status.Dock = DockStyle.Fill;
            status.AutoEllipsis = true;
            status.ForeColor = UiStyle.Muted;
            status.BackColor = UiStyle.Surface;
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
            preview.BorderStyle = BorderStyle.FixedSingle;
            rightPanel.Controls.Add(preview, 0, 1);
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
            label.ForeColor = UiStyle.Muted;
            label.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold);
            input.Dock = DockStyle.Fill;
            UiStyle.StyleInput(input);

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
            browse.Dock = DockStyle.Fill;
            browse.Margin = new Padding(8, 1, 0, 1);
            UiStyle.StyleButton(browse, false);
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
            label.ForeColor = UiStyle.Muted;
            label.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            input.Dock = DockStyle.Fill;
            UiStyle.StyleInput(input);

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
