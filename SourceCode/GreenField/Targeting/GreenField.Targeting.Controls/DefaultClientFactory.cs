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
using TopDown.FacingServer.Backend.Targeting;

namespace GreenField.Targeting.Controls
{
	public class DefaultClientFactory : IClientFactory
	{
		public TopDown.FacingServer.Backend.Targeting.FacadeClient CreateClient()
		{
			var result = new FacadeClient();
            
			return result;
		}

        public void Initialize(string username)
        {
            this.username = username;
        }

        private string username;

        public string GetUsername()
        {
            return username;
        }
    }
}
