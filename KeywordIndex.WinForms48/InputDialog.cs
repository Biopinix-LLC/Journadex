using System;
using System.Linq;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public class InputDialog<T> : Form where T : IConvertible
    {
        private ComboBox comboBox;
        private Button okButton;
        private Button cancelButton;
        private GroupBox optionsGroupBox;
        private CheckBox[] optionCheckBoxes;
        private TextBox memoBox;
        private FlowLayoutPanel flowLayout;

        private InputDialog(string caption, string labelText, T defaultResult, T[] items, string[] options, Control[] flowControls)
        {
            // Set the form properties
            Text = caption;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            int y = 0;
            // Create the label
            Label label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Location = new System.Drawing.Point(9, 20),
                Size = new System.Drawing.Size(35, 13)
            };

            Control inputBox;
            if (items != null)
            {
                // Create the combo box
                comboBox = CreateComboBox(defaultResult, items);
                inputBox = comboBox;
            }
            else
            {
                memoBox = new TextBox
                {
                    Size = new System.Drawing.Size(268, 63),
                    Multiline = true,
                    Text = defaultResult?.ToString() ?? "",

                };
                inputBox = memoBox;
            }
            inputBox.Location = new System.Drawing.Point(12, 36);
            inputBox.TabIndex = 0;
            y = inputBox.Bottom + 6;
            if (options?.Length > 0)
            {
                // Create the options group box
                optionsGroupBox = new GroupBox
                {
                    Location = new System.Drawing.Point(12, y),
                    Size = new System.Drawing.Size(268, 75),
                    Text = "Options",
                    TabIndex = 1
                };
                y += optionsGroupBox.Height;
                // Create the option check boxes
                optionCheckBoxes = new CheckBox[options.Length];
                for (int i = 0; i < options.Length; i++)
                {
                    optionCheckBoxes[i] = new CheckBox
                    {
                        Text = options[i],
                        AutoSize = true,
                        Location = new System.Drawing.Point(6, 19 + (i * 21))
                    };
                    optionsGroupBox.Controls.Add(optionCheckBoxes[i]);
                }
            }
            if (flowControls?.Length > 0)
            {
                flowLayout = new FlowLayoutPanel
                {
                    Location = new System.Drawing.Point(12, y),
                    AutoSize= true,
                    //Size = new System.Drawing.Size(268, inputBox.Height + 6)
                };
                flowLayout.Controls.AddRange(flowControls);
                y += flowLayout.Height + 3; 

            }
            // Create the OK button
            okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(124, y),
                Size = new System.Drawing.Size(75, 23),
                Text = "OK",
                UseVisualStyleBackColor = true,
                TabIndex = 1
            };

            // Create the Cancel button
            cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(205, y),
                Size = new System.Drawing.Size(75, 23),
                Text = "Cancel",
                UseVisualStyleBackColor = true,
                TabIndex = 2
            };

            // Add the controls to the form
            Controls.AddRange(new Control[] { label, inputBox, okButton, cancelButton });
            if (optionsGroupBox != null)
            {
                Controls.Add(optionsGroupBox);
            }
            if (flowLayout != null)
            {
                Controls.Add(flowLayout);
            }
            AcceptButton = okButton;
            CancelButton = cancelButton;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        internal static ComboBox CreateComboBox(T defaultResult, T[] items)
        {
            ComboBox comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDown,
                Size = new System.Drawing.Size(268, 21),
                AutoCompleteMode = AutoCompleteMode.SuggestAppend, 
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            comboBox.Items.AddRange(items.Cast<object>().ToArray());
            comboBox.SelectedItem = defaultResult;
            if (defaultResult is string stringDefault)
            {
                comboBox.Text = stringDefault;
            }

            return comboBox;
        }

        public DialogResult ShowDialog(out Tuple<T, bool[]> selectionWithOptions)
        {
            var result = ShowDialog();
            if (result != DialogResult.OK)
            {
                selectionWithOptions = null;
                return result;
            }
            bool[] checkedOptions = null;
            if (optionCheckBoxes != null)
            {


                // Get the array of checked options
                checkedOptions = new bool[optionCheckBoxes.Length];
                for (int i = 0; i < optionCheckBoxes.Length; i++)
                {
                    checkedOptions[i] = optionCheckBoxes[i].Checked;
                }
            }
            T returnValue;
            if (comboBox != null)
            {
                returnValue = (T)comboBox.SelectedItem;

                if (returnValue == null && !string.IsNullOrWhiteSpace(comboBox.Text))
                {
                    returnValue = (T)Convert.ChangeType(comboBox.Text, typeof(T));
                }
            }
            else if (memoBox != null)
            {
                returnValue = (T)Convert.ChangeType(memoBox.Text, typeof(T));
            }
            else throw new NotSupportedException();
            selectionWithOptions = new Tuple<T, bool[]>(returnValue, checkedOptions ?? Array.Empty<bool>());
            return result;

        }
        public static Tuple<T, bool[]> ShowDialog(string caption, string labelText, T defaultResult = default, T[] items = null, string[] options = null, Control[] flowControls = null)
        {

            var inputDialog = new InputDialog<T>(caption, labelText, defaultResult, items, options, flowControls);
            // Show the dialog and get the result
            _ = inputDialog.ShowDialog(out Tuple<T, bool[]> resultWithOptions);
            return resultWithOptions;

        }
    }
}
