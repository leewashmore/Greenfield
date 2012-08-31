using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GreenField.DAL;
using System.IO;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class PresentationAttachedFileStreamData
    {
        [DataMember]
        public FileMaster PresentationAttachedFileData { get; set; }

        [DataMember]
        public Byte[] FileStream { get; set; }
    }
}