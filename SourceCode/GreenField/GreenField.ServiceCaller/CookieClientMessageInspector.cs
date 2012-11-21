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
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace GreenField.ServiceCaller
{
    public class CookieClientMessageInspector : IClientMessageInspector
    {
        // Use a static cookie container so that all client proxy instances share the same cookies
        private static readonly CookieContainer _cookieContainer = new CookieContainer();

        public CookieClientMessageInspector()
        {

        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            // Bind the cookie container from the message inspector to the channel
            var _cookieManager = channel.GetProperty<IHttpCookieContainerManager>();
            //_cookieContainer.Add(new Uri("http://"), new Cookie("UserName", UserSession.SessionManager.SESSION == null 
            //    || UserSession.SessionManager.SESSION.UserName == null ? "null" : UserSession.SessionManager.SESSION.UserName));
            
            if (_cookieManager != null)
            {
                _cookieManager.CookieContainer = _cookieContainer;
            }

            return null;
        }
    }
}
