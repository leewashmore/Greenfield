using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core = TopDown.Core;

namespace GreenField.Targeting.Server
{
    public class FacadeSettings
    {
        [DebuggerStepThrough]
        public FacadeSettings(
            Core.Facade facade,
            Serializer serializer,
            BroadGlobalActive.Serializer bgaSerializer,
            BroadGlobalActive.Deserializer bgaDeserializer,
            BasketTargets.Serializer btSerializer,
            BasketTargets.Deserializer btDeserializer,
            BottomUp.Serializer buSerializer,
            BottomUp.Deserializer buDeserializer,
			Boolean shouldDropRepositoriesOnEveryReload
        )
        {
            this.Facade = facade;
            this.Serializer = serializer;
            
            this.BgaSerializer = bgaSerializer;
            this.BgaDeserializer = bgaDeserializer;

            this.BtSerializer = btSerializer;
            this.BtDeserializer = btDeserializer;

            this.BuSerializer = buSerializer;
            this.BuDeserializer = buDeserializer;

			this.ShouldDropRepositoriesOnEveryReload = shouldDropRepositoriesOnEveryReload;
        }
        
        public Core.Facade Facade { get; private set; }
        public Serializer Serializer { get; private set; }
        
        public BroadGlobalActive.Serializer BgaSerializer { get; private set; }
        public BroadGlobalActive.Deserializer BgaDeserializer { get; private set; }

        public BasketTargets.Serializer BtSerializer { get; private set; }
        public BasketTargets.Deserializer BtDeserializer { get; private set; }

        public BottomUp.Serializer BuSerializer { get; private set; }
        public BottomUp.Deserializer BuDeserializer { get; private set; }

		public Boolean ShouldDropRepositoriesOnEveryReload { get; private set; }
    }
}
