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
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Gadgets.Models
{
    public class ICPBlogInfo : NotificationObject
    {
        private DateTime _blogCreationDate;
        public DateTime BlogCreationDate
        {
            get { return _blogCreationDate; }
            set
            {
                if (_blogCreationDate != value)
                {
                    _blogCreationDate = value;
                    RaisePropertyChanged(() => this.BlogCreationDate);
                }
            }
        }

        private string _blogCreatedBy;
        public string BlogCreatedBy
        {
            get { return _blogCreatedBy; }
            set
            {
                if (_blogCreatedBy != value)
                {
                    _blogCreatedBy = value;
                    RaisePropertyChanged(() => this.BlogCreatedBy);
                }
            }
        }

        private string _blogComment;
        public string BlogComment
        {
            get { return _blogComment; }
            set
            {
                if (_blogComment != value)
                {
                    _blogComment = value;
                    RaisePropertyChanged(() => this.BlogComment);
                }
            }
        }

    }
}
