using System.Windows.Forms;
using System;

namespace KeywordIndex.WinForms48
{
    public static class ClipboardWithMessage
    {
        public static void SetText(string text, Control parent)
        {
            try
            {
                Clipboard.SetText(text);
                MessagePanel.Show(parent, $"\"{text}\" copied to clipboard.");
            }
            catch (Exception ex)
            {
                MessagePanel.Show(parent, ex.Message, MessagePanel.MessageType.Error);
            }


        }
    }
}