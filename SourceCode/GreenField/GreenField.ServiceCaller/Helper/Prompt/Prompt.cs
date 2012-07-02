using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GreenField.ServiceCaller
{
    public static class Prompt
    {
        public static void ShowDialog(string messageText, string captionText = "", MessageBoxButton buttonType = MessageBoxButton.OK, Action<MessageBoxResult> messageBoxResult = null)
        {
            Message prompt = new Message(messageText, captionText, buttonType);
            prompt.Show();

            prompt.Unloaded += (se, e) =>
            {
                MessageBoxResult result;
                if (prompt.DialogResult == null)
                    result = MessageBoxResult.Cancel;

                result = Convert.ToBoolean(prompt.DialogResult) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                if (messageBoxResult != null)
                    messageBoxResult(result);
            };

        }
                        
    }
}
