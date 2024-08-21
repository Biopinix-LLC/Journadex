using System.Drawing;
using System.Windows.Forms;
using System;

namespace KeywordIndex.WinForms48
{
    public static class ProgressManager
    {
        private static ProgressBarForm _progressBarForm;

        private static ProgressBarForm Form
        {
            get
            {
                if (_progressBarForm == null || _progressBarForm.IsDisposed)
                {
                    _progressBarForm = new ProgressBarForm();
                    _progressBarForm.Hide();
                }
                return _progressBarForm;
            }
        }

        public static void Show(Control control = null, string caption = null) => Form.ShowProgressBar(control, caption);
        public static void Hide() => Form.HideProgressBar();

        /// <summary>
        /// ChatGPT:
        /// </summary>
        private class ProgressBarForm : Form
        {
            private ProgressBar progressBar;

            public ProgressBarForm()
            {
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                this.progressBar = new ProgressBar();
                this.SuspendLayout();

                // Set progress bar properties
                this.progressBar.Style = ProgressBarStyle.Marquee;
                this.progressBar.Dock = DockStyle.Fill;

                // Add progress bar to form
                this.Controls.Add(this.progressBar);

                // Set form properties
                this.ClientSize = new System.Drawing.Size(300, 50);
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "ProgressBarForm";
                this.ResumeLayout(false);
            }

            /// <summary>
            /// Shows the progress bar form in the middle of the parent or screen.
            /// </summary>
            /// <param name="parentControl"></param>
            
            public void ShowProgressBar(Control parentControl = null, string caption = null)
            {
                StartPosition = parentControl != null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
                Text = caption ?? "Loading...";
                if (!Visible)
                {
                    this.Show(parentControl);
                }

                Refresh();
            }

            public void HideProgressBar() => this.Hide();
        }
    }

}
