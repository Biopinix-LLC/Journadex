using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{


    /// <summary>
    /// ChatGPT: write a C# fluent builder class that displays a WinForms message dialog with the same options as the MessageBox but with the addition of a details button. the details button should appear below the message and should expand the message form to display the details and should have a way to hide the details.
    /// </summary>
    public class MessageDialogBuilder
    {
        private string message;
        private string caption = "Message";
        private MessageBoxButtons buttons = MessageBoxButtons.OK;
        private MessageBoxIcon icon = MessageBoxIcon.Information;
        private string details;

        private Form messageForm;
        private Button detailsButton;
        private TextBox detailsText;

        public MessageDialogBuilder Message(string message)
        {
            this.message = message;
            return this;
        }

        public MessageDialogBuilder Caption(string caption)
        {
            this.caption = caption;
            return this;
        }

        public MessageDialogBuilder Buttons(MessageBoxButtons buttons)
        {
            this.buttons = buttons;
            return this;
        }

        public MessageDialogBuilder Icon(MessageBoxIcon icon)
        {
            this.icon = icon;
            return this;
        }

        public MessageDialogBuilder Details(string details)
        {
            this.details = details;
            return this;
        }

        public DialogResult Show()
        {
            messageForm = new Form
            {
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label messageLabel = new Label
            {
                Text = message,
                Dock = DockStyle.Fill
            };
            messageForm.Controls.Add(messageLabel);

            if (!string.IsNullOrEmpty(details))
            {
                detailsButton = new Button
                {
                    Text = "Details >>",
                    Dock = DockStyle.Bottom
                };
                detailsButton.Click += new EventHandler(DetailsButton_Click);
                messageForm.Controls.Add(detailsButton);
            } 

            if (buttons == MessageBoxButtons.OK || buttons == MessageBoxButtons.OKCancel)
            {
                Button okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Dock = DockStyle.Bottom
                };
                messageForm.Controls.Add(okButton);
                messageForm.AcceptButton = okButton;
            }

            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                Button yesButton = new Button
                {
                    Text = "Yes",
                    DialogResult = DialogResult.Yes,
                    Dock = DockStyle.Left
                };
                messageForm.Controls.Add(yesButton);

                Button noButton = new Button
                {
                    Text = "No",
                    DialogResult = DialogResult.No,
                    Dock = DockStyle.Right
                };
                messageForm.Controls.Add(noButton);
            }

            if (buttons == MessageBoxButtons.OKCancel || buttons == MessageBoxButtons.YesNoCancel)
            {
                Button cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Dock = DockStyle.Right
                };
                messageForm.Controls.Add(cancelButton);
                messageForm.CancelButton = cancelButton;
            }


            switch (icon)
            {
                case MessageBoxIcon.Error:
                    messageForm.Icon = SystemIcons.Error;
                    break;
                case MessageBoxIcon.Information:
                    messageForm.Icon = SystemIcons.Information;
                    break;
                case MessageBoxIcon.Question:
                    messageForm.Icon = SystemIcons.Question;
                    break;
                case MessageBoxIcon.Warning:
                    messageForm.Icon = SystemIcons.Warning;
                    break;
            }

            return messageForm.ShowDialog();
        }

        private void DetailsButton_Click(object sender, EventArgs e)
        {
            if (detailsText != null)
            {
                messageForm.Controls.Remove(detailsText);
                detailsButton.Text = "Details >>";
                detailsText = null;
            }
            else
            {
                detailsText = new TextBox
                {
                    Text = details,
                    Dock = DockStyle.Bottom,
                    Multiline= true,
                    Width = messageForm.Width
                    
                };
                messageForm.Controls.Add(detailsText);
                detailsButton.Text = "<< Hide Details";
            }
        }
    }
}