using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using System.Windows.Forms;
namespace KeywordIndex.WinForms48
{
    /* Me> I want to add error handling to a bunch of existing classes.Most of the work in these classes happen in event handlers so I thought implementing the IErrorHandler on these classes and then raising the Error event would be a good way to do it.Is there a better way?
    ChatGPT> Implementing the IErrorHandler interface and raising the Error event in the event handlers of your existing classes is one way to add error handling to them.
Another way to handle errors in these classes is by using a global error handler which can be implemented using a try-catch block.
This way, you can catch the exceptions that occur in the event handlers of your existing classes and handle them in a centralized manner.
You can also wrap the event handlers in a try-catch block and use the catch block to call a global error handler.
This approach allows you to handle errors in a centralized and consistent way, rather than having to add error handling code to each individual event handler.
Additionally, you can use logger library like log4net, NLog, Serilog etc to log the exception details to a file or other destinations so that you can diagnose the issue later and also you can use the global error handler to send the exception details to an error monitoring service like Sentry, Rollbar, or bugsnag that can provide more detailed information about the error and help you to identify and fix the issue faster.
It ultimately depends on your use case, but both approaches have their own benefits and drawbacks, and you should choose the one that best fits your needs.*/
    public static class GlobalErrorHandler
    {
        public static void HandleError(Exception ex)
        {
            // handle the error
            var message = ex.Message;
            var details = ex.ToString();

            var builder = new MessageDialogBuilder();
            builder.Message(message)
                   .Caption("Error")
                   .Buttons(MessageBoxButtons.OK)
                   .Icon(MessageBoxIcon.Error)
                   .Details(details)
                   .Show();
        }
    }

}
