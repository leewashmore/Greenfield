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
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace GreenField.Common.Helper
{
    public class Serializer
    {
        #region Public methods

        public static string Serialize<T>(T data)
        {
            using (var memoryStream = new MemoryStream())
            {
                //OnSerialized(EventArgs.Empty);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, data);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(memoryStream);
                string content = reader.ReadToEnd();
                return content;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                var serializer = new DataContractSerializer(typeof(T));
                T theObject = (T)serializer.ReadObject(stream);
                //objDeserialized = (T)serializer.ReadObject(stream);
                //OnDeserialized(EventArgs.Empty);
                return theObject;
            }
        }

        #endregion
    }
}
