using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Browser;

namespace GreenField.App
{
    /// <summary>
    /// Creates WCF service clients that use SOAP 1.2 via the ClientHttp stack, and passes cookies
    /// to the server.
    /// </summary>
    public static class ServiceClientFactory
    {
        /// <summary>
        /// Backing store for cookies.
        /// </summary>
        private static IDictionary<string, string> m_Cookies;

        /// <summary>
        /// Return the lazily initialized dictionary of cookies.
        /// </summary>
        public static IDictionary<string, string> Cookies
        {
            get
            {
                if (m_Cookies == null)
                {
                    throw new InvalidOperationException("The cookie jar has not been initialized: Call ServiceClientFactory.ReadCookies when the application starts");
                }
                return m_Cookies;
            }
        }

        /// <summary>
        /// Creates a service proxy for a given service contract.
        /// </summary>
        public static T CreateServiceClient<T>(Uri serviceUrl)
        {
            // Determine whether to use transport layer security (HTTP over SSL).
            // Depending on whether HTTPS is used, create a different BindingElement for
            // the transport layer of the Binding.
            bool useTransportSecurity = serviceUrl.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase);
            BindingElement transportBinding = useTransportSecurity ?
              new HttpsTransportBindingElement() :
              new HttpTransportBindingElement();

            // Since the ClientHttp stack is used, it is necessary to define cookies manually.
            // We read the cookies from the browser window and attach them to the WCF binding
            // via an additional BindingElement that lets us inject cookies.
            BindingElement cookieBinding = new HttpCookieContainerBindingElement();

            // Create a custom binding that uses the specified elements for transport and 
            // accepting cookies. The order of the bindings is relevant; the transport binding 
            // needs to come last it's the lowest in the stack.
            Binding binding = new CustomBinding(cookieBinding, transportBinding);

            // Set the address for the service endpoint.
            EndpointAddress address = new EndpointAddress(serviceUrl);

            // Let the ChannelFactory create the service client.
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            T channel = factory.CreateChannel();

            // Because we've injected the HttpCookieContainerBindingElement, the channel supports cookies:
            // it exposes a CookieContainerManager. 
            IHttpCookieContainerManager cookieManager =
              ((IChannel)channel).GetProperty<IHttpCookieContainerManager>();

            // The CookieContainerManager can, but does not have to, provide a Cookie Container;
            // in the case it has not yet been created instantiate one now.
            if (cookieManager.CookieContainer == null)
            {
                cookieManager.CookieContainer = new CookieContainer();
            }

            // Walk through the cookies of the browser window the application lives in and 
            // add the cookies to the channel. The URL for the cookie is set to the application root.
            // We get to that URL by quering the application URL - that's the URL of the XAP - 
            // and go one level up.
            Uri applicationUri = new Uri(Application.Current.Host.Source, "../");
            foreach (var cookieContent in Cookies)
            {
                Cookie cookie = new Cookie(cookieContent.Key, cookieContent.Value);
                cookieManager.CookieContainer.Add(applicationUri, cookie);
            }
            return channel;
        }

        /// <summary>
        /// Read cookies from the browser into a local store.
        /// </summary>
        public static void ReadCookies()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // Initialize dictionary
                m_Cookies = new Dictionary<string, string>();

                // Get the cookies from the browser. It returns it in a single, semicolon-delimited string.
                string cookies = HtmlPage.Document.Cookies;
                string[] cookieSplit = cookies.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cookie in cookieSplit)
                {
                    string[] keyAndValue = cookie.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyAndValue.Length == 2)
                    {
                        string cookieKey = keyAndValue[0].Trim();
                        string cookieValue = keyAndValue[1].Trim();

                        // add cookie to dictionary
                        m_Cookies.Add(cookieKey, cookieValue);
                    }
                }
            });
        }
    }
}