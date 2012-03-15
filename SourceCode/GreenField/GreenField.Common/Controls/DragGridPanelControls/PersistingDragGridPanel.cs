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

namespace DragGridPanelControls
{
    [DataContract]
    public class PersistingDragGridPanel : DragGridPanel
    {
         #region Private memebers

        ///// <summary>
        ///// Stores the position in parent Layout.
        ///// </summary>
        //private int position;
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
        public PersistingDragGridPanel()
        {
            
        }

        #region Public members

        ///// <summary>
        ///// Gets or sets the position.
        ///// </summary>
        //public int Position
        //{
        //    get { return position; }

        //    set
        //    {
        //        position = value;
                
        //    }
        //}

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
