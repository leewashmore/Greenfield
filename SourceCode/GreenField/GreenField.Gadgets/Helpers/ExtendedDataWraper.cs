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
using System.ComponentModel;
using Telerik.Windows.Controls.Map;

namespace GreenField.Gadgets.Helpers
{
    public class ExtendedDataWraper : INotifyPropertyChanged
    {

        private ExtendedData data;

        /// <summary> 
        /// Occurs when property changed. Implemented for binding. 
        /// </summary> 
        public event PropertyChangedEventHandler PropertyChanged;

        public ExtendedData Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
                this.data.PropertyChanged += new PropertyChangedEventHandler(data_PropertyChanged);
                this.OnPropertyChanged("Data");
            }
        }

        private void data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("Data");
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs eventArgs = new PropertyChangedEventArgs(
                    propertyName);
                this.PropertyChanged(this, eventArgs);
            }
        }
    }
}
