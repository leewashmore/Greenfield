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

namespace GreenField.App.Helpers
{
    public class DataItem : INotifyPropertyChanged
    {
        private string text;
        private bool isselected;
        private string category;
        private string displayText;

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public string DisplayText
        {
            get
            {
                return this.displayText;
            }
            set
            {
                if (this.displayText != value)
                {
                    this.displayText = value;
                    OnPropertyChanged("DisplayText");
                }
            }
        }

        public string Category
        {
            get
            {
                return this.category;
            }
            set
            {
                if (this.category != value)
                {
                    this.category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        public bool IsSelected
        {
            get
            {

                return this.isselected;
            }
            set
            {
                if (this.isselected != value)
                {
                    this.isselected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
