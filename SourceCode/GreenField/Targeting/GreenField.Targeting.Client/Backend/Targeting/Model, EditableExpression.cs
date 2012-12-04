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
            if (issues.Any())
            {
                return issues;

            }
            else
            {
                return issues;
            }
        }

        public Boolean HasErrors
        {
            get { return this.Issues.Any(); }
        }

        public void RegisterForBeingWatched(IValueChangeWatcher watcher)
        {
            // in the beginning we memorize the original values so that we can use the original values to detemine whether the value was changed later
            this.OriginalEditedValue = this.EditedValue;
            this.OriginalComment = this.Comment;

            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "EditedValue")
                {
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

        protected String OriginalComment { get; private set; }
        protected Decimal? OriginalEditedValue { get; private set; }
    }
}
