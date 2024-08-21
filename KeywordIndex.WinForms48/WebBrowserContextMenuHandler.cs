using System;
using System.Windows.Forms;
namespace KeywordIndex.WinForms48
{
    internal abstract class WebBrowserContextMenuHandler
    {
        public WebBrowserContextMenuHandler(WebBrowser webBrowser)
        {
            WebBrowser = webBrowser;
            WebBrowser.DocumentCompleted -= OnDocumentCompleted;
            WebBrowser.DocumentCompleted += OnDocumentCompleted;
            webBrowser.IsWebBrowserContextMenuEnabled = false;
        }

        protected WebBrowser WebBrowser { get; }

        protected virtual string CurrentId { get; set; }

        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = (WebBrowser)sender;
            var htmlDocument = webBrowser.Document;

            htmlDocument.Body.MouseDown += OnBodyMouseDown;
        }

        private void OnBodyMouseDown(object sender, HtmlElementEventArgs e)
        {
            CurrentId = null;
            if (e.MouseButtonsPressed == MouseButtons.Right)
            {
                WebBrowser.ContextMenuStrip = null;
                var element = WebBrowser.Document.GetElementFromPoint(e.ClientMousePosition);
                CurrentId = GetIdFromElement(element);
                if (CurrentId != null)
                {
                    var contextMenu = new ContextMenuStrip();
                    if (!PopulateMenu(contextMenu)) return;
                    WebBrowser.ContextMenuStrip = contextMenu;
                    contextMenu.Show(WebBrowser, e.ClientMousePosition);
                }
            }
        }

        protected virtual string GetIdFromElement(HtmlElement element)
        {
            return element.GetIdFromEditAnchor();
        }

        protected abstract bool PopulateMenu(ContextMenuStrip contextMenu);
        
    }
}