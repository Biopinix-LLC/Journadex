using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordIndex.WinForms48
{
    internal class DisposableProgress : IDisposable
    {
        private bool _show;
        public DisposableProgress(string caption = null, bool showProgress = true)
        {
            _show = showProgress;
            if (showProgress)
            {
                ProgressManager.Show(caption: caption);
            }
        }
        public void Dispose()
        {
            if (_show)
            {
                ProgressManager.Hide();
            }
        }
    }
}
