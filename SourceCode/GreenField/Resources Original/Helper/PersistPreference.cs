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
using System.Runtime.Serialization;
using System.IO;
using System.Text;

namespace GreenField.Common.Helper
{    
    public class PersistPreference
    {
        #region Private memebers

        /// <summary>
        /// Stores the position in parent Layout.
        /// </summary>
        private int position;

        /// <summary>
        /// Stores the unique id of child control.
        /// </summary>
        private int id;


        /// <summary>
        /// Header of the dragdock panel 
        /// </summary>
        private object header;

        /// <summary>
        /// content of the dragdock panel
        /// </summary>
        private object content;

        #endregion

        #region Events       

        public event EventHandler Deserialized;

        protected virtual void OnDeserialized(EventArgs e)
        {
            if (Deserialized!=null)
            {
                Deserialized(this, e);
            }
        }
        public event EventHandler Serialized;

        protected virtual void OnSerialized(EventArgs e)
        {
            if (Serialized != null)
            {
                Serialized(this, e);
            }
        }
        #endregion

        /// <summary>
        /// Blank Constructor
        /// </summary>
        public PersistPreference()
        {
            
        }

        #region Public members

        /// <summary>
        /// Gets or sets the position.
        /// </summary>        
        public int Position
        {
            get { return position; }

            set
            {
                position = value;
                
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>       
        public int Id
        {
            get { return id; }

            set
            {
                id = value;

            }
        }
        
        public object Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
            }
        }
        
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }
        #endregion


        #region Public methods

        public string Serialize<T>(T data)
        {
            using (var memoryStream = new MemoryStream())
            {
                OnSerialized(EventArgs.Empty);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, data);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(memoryStream);
                string content = reader.ReadToEnd();
                return content;
            }
        }

        public void Deserialize<T>(string xml,T objDeserialized)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                var serializer = new DataContractSerializer(typeof(T));
                //T theObject = (T)serializer.ReadObject(stream);
                objDeserialized = (T)serializer.ReadObject(stream);
                OnDeserialized(EventArgs.Empty);
                //return theObject;
            }
        }

        #endregion
    }
}
