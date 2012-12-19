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
using System.Linq;

namespace TopDown.FacingServer.Backend.Targeting
{
    public partial class EditableExpressionModel : INotifyDataErrorInfo, IExpressionModel
    {

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(String propertyName)
        {
            var issues = ValidationHelper.GetErrors(propertyName, "EditedValue", this);
            return issues;
        }

        public Boolean HasErrors
        {
            get { return this.Issues.Any(); }
        }

        private ICommand requestCommentsCommand;
        public ICommand RequestCommentsCommand
        {
            get { return this.requestCommentsCommand; }
            set
            {
                this.requestCommentsCommand = value;
                this.RaisePropertyChanged("RequestCommentsCommand");
            }
        }

        public void RegisterForBeingWatched(IValueChangeWatcher watcher, ICommand command)
        {
            this.RequestCommentsCommand = command;
            // in the beginning we memorize the original values so that we can use the original values to detemine whether the value was changed later
            this.OriginalEditedValue = this.EditedValue;
            this.OriginalComment = this.Comment;

            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "EditedValue")
                {
                    this.RaisePropertyChanged("IsModified");
                    if (this.EditedValue == this.OriginalEditedValue) return;
                    watcher.GetNotifiedAboutChangedValue(this);
                }
                else if (e.PropertyName == "Comment")
                {
                    if (this.OriginalComment == this.Comment) return;
                    watcher.GetNotifiedAboutChangedValue(this);
                }
            };
        }


        private Boolean isFocusSet;
        public Boolean IsFocusSet
        {
            get { return this.isFocusSet; }
            set { this.isFocusSet = value; this.RaisePropertyChanged("IsFocusSet"); }
        }

        

        protected String OriginalComment { get; private set; }
        protected Decimal? OriginalEditedValue { get; private set; }
        public Boolean IsModified {
            get 
            {
                return this.InitialValue != this.EditedValue;
            } 
        }

        
    }
}
