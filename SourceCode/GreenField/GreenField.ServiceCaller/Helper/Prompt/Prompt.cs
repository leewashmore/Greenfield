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
using System.Collections.Generic;
using System.Linq;

namespace GreenField.ServiceCaller
{
    public static class Prompt
    {
        private static List<StorePrompt> _storePrompt = new List<StorePrompt>();                

        public static void ShowDialog(string messageText, string captionText = "", MessageBoxButton buttonType = MessageBoxButton.OK, Action<MessageBoxResult> messageBoxResult = null)
        {
            if (_storePrompt.Any(msg => msg.MessageText == messageText 
                && msg.CaptionText == captionText 
                && msg.ButtonType == buttonType 
                && msg.MessageBoxResult == messageBoxResult))
                return;

            _storePrompt.Add(new StorePrompt()
            {
                MessageText = messageText,
                CaptionText = captionText,
                ButtonType = buttonType,
                MessageBoxResult = messageBoxResult
            });            

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

                _storePrompt.RemoveAt(_storePrompt.Count -1);
            };
        }               
    }

    public class StorePrompt
    {
        public string MessageText {get; set;}
        public string CaptionText {get; set;}
        public MessageBoxButton ButtonType {get; set;}
        public Action<MessageBoxResult> MessageBoxResult {get; set;}        
    }
}
