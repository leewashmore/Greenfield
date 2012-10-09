using System.Runtime.Serialization;

namespace GreenField.ServiceCaller.Helper
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public string Description { get; set; }

        public ServiceFault(string description)
        {
            this.Description = description;
        }        
    }
}