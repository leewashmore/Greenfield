using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BottomUp
{
    public class Settings
    {
        [DebuggerStepThrough]
        public Settings(IClientFactory clientFactory)
        {
            this.ClientFactory = clientFactory;
        }

        public IClientFactory ClientFactory { get; private set; }
    }
}
