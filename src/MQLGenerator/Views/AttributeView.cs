using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MqlGenerator
{
    internal sealed partial class MainForm
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

        private void BuildAttributeView()
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

            BuildPreviewPanel();
        }

        private void BuildPreviewPanel()
        {
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

            status.Dock = DockStyle.Fill;
            status.AutoEllipsis = true;
            status.ForeColor = UiStyle.Muted;
            status.BackColor = UiStyle.Surface;
            status.TextAlign = ContentAlignment.MiddleLeft;
            status.Padding = new Padding(4, 0, 4, 0);
            rightPanel.Controls.Add(status, 0, 2);
        }

        private void AddField(
            TableLayoutPanel panel,
            string labelText,
            Control input,
            int row,
            int column,
            int span)
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

        private IEnumerable<Control> AllInputs(Control parent)
        {
            foreach (Control child in parent.Controls)
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
            preview.Text = AttributeGenerator.Generate(ReadAttributeDefinition());
        }

        private AttributeDefinition ReadAttributeDefinition()
        {
            var definition = new AttributeDefinition();
            definition.CreatedDate = createdDate.Text.Trim();
            definition.CreatedBy = createdBy.Text.Trim();
            definition.DescriptionKo = descriptionKo.Text.Trim();
            definition.Name = attributeName.Text.Trim();
            definition.DescriptionEn = descriptionEn.Text.Trim();
            definition.Type = Convert.ToString(attributeType.SelectedItem);
            definition.DefaultValue = defaultValue.Text;
            definition.Multiline = multiline.Checked;
            definition.Hidden = hidden.Checked;
            definition.Application = application.Text.Trim();
            definition.Version = version.Text.Trim();
            definition.Installer = installer.Text.Trim();
            definition.InstalledDate = installedDate.Text.Trim();

            foreach (object item in rangeList.Items)
            {
                definition.Ranges.Add(Convert.ToString(item));
            }

            return definition;
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
    }
}
