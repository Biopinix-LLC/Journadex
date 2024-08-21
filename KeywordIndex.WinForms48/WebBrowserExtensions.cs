using System.Windows.Forms;

public static class WebBrowserExtensions
{
    /// <summary>
    /// ChatGPT: Write C# extension method for a WebBrowser that will check the ready state before setting the document text and set the document text after the document has completed if it is not ready yet.
    /// </summary>
    /// <param name="webBrowser"></param>
    /// <param name="html"></param>
    public static void SetDocumentTextSafe(this WebBrowser webBrowser, string html)
    {
        switch (webBrowser.ReadyState)
        {
            case WebBrowserReadyState.Complete:
            case WebBrowserReadyState.Uninitialized:
                webBrowser.DocumentText = html;
                break;
            default:
                {
                    WebBrowserDocumentCompletedEventHandler handler = null;
                    handler = (sender, e) =>
                    {
                        webBrowser.DocumentCompleted -= handler;
                        webBrowser.DocumentText = html;
                    };
                    webBrowser.DocumentCompleted += handler;
                    break;
                }
        }
    }
}
