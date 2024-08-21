using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public class MessagePanel
    {
        /// <summary>
        /// ChatGPT: Write a C# method that will display a closable message WinForms panel above a specified control. If an interval is specified, the panel should close automatically after that many seconds have elapsed. The background color of the panel should be determined based on whether the message is Information, Warning or Error.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="durationInSeconds"></param>
        public static void Show(Control parent, string message, MessageType messageType = MessageType.Information, int? durationInSeconds = null, Control[] flowControls = null)
        {
            // Create a new Panel control to display the message
            Panel messagePanel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = GetBackgroundColor(messageType),
                Padding = new Padding(3),
                Height = 40,
                Dock = DockStyle.Top
            };
            //messagePanel.Size = new Size(parent.Width - 20, 60);
            //messagePanel.Location = new Point(10, parent.Top - messagePanel.Height - 10);
            //messagePanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // Create a new Label control to display the message text
            Label messageLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Text = message.RemoveNewLines().Truncate(100, Factory.CreateMessagePanelTruncateStrategy())
            };
            // Add the Label control to the Panel control
            messagePanel.Controls.Add(messageLabel);

            if (flowControls?.Length > 0)
            {
                FlowLayoutPanel flowLayout = new FlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Bottom,
                };
                flowLayout.Controls.AddRange(flowControls);
                messagePanel.Height += 25;
                messagePanel.Controls.Add(flowLayout);
                messageLabel.BringToFront();
            }
           

            // Add the Panel control to the parent's container
            parent.Parent.Controls.Add(messagePanel);
            // TODO truncate message before setting the duration

            if (durationInSeconds == null)
            {
                durationInSeconds = messageLabel.Text.CalculateReadingTimeInSeconds();
            }
            // If a duration is specified, close the message panel after that many seconds
            if (durationInSeconds > 0)
            {
                Timer timer = new Timer();
                timer.Interval = durationInSeconds.Value * 1000;
                timer.Tick += (s, e) =>
                {
                    parent.Parent.Controls.Remove(messagePanel);
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }

            // Add a MouseClick event handler to the message panel to allow it to be closed manually
            messageLabel.MouseClick += (s, e) =>
            {
                parent.Parent.Controls.Remove(messagePanel);
            };
        }

        // Helper method to get the background color for the message type
        private static Color GetBackgroundColor(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Information:
                    return Color.FromArgb(0, 122, 204); // Blue
                case MessageType.Warning:
                    return Color.FromArgb(255, 193, 7); // Yellow
                case MessageType.Error:
                    return Color.FromArgb(220, 53, 69); // Red
                default:
                    return Color.FromArgb(0, 0, 0); // Black
            }
        }

        // Enum to represent the message type
        public enum MessageType
        {
            Information,
            Warning,
            Error
        }

    }
}
