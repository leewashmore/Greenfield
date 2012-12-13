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
using System.Collections.ObjectModel;
using TopDown.FacingServer.Backend.Targeting;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Targeting.Controls
{
    public abstract class EditorViewModelBase<TInput> : CommunicatingViewModelBase
        where TInput : class
    {
        public const Int32 NumberOfMillisecondsBetweenLastChangeAndRecalcualtion = 100;

        private DispatcherTimer waitBeforeRecalculating;
        public EditorViewModelBase()
        {
            this.waitBeforeRecalculating = new DispatcherTimer();
            this.waitBeforeRecalculating.Interval = TimeSpan.FromMilliseconds(NumberOfMillisecondsBetweenLastChangeAndRecalcualtion);
            this.waitBeforeRecalculating.Stop();
            this.waitBeforeRecalculating.Tick += this.WhenTimeout;
            this.RequestRecalculatingCommand = new DelegateCommand(this.RequestRecalculating, () => !this.IsRecalculationAutomatic);
        }

        public event EventHandler GotData;
        protected virtual void OnGotData()
        {
            var handler = this.GotData;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected void SetProvenValidInput(TInput input)
        {
            this.lastValidInput = input;
        }

        private TInput lastValidInput;

        protected TInput LastValidInput { get { return this.lastValidInput; } }

        public void RequestReloading()
        {
            if (this.lastValidInput != null)
            {
                this.RequestReloading(this.lastValidInput);

            }
        }

        protected abstract void RequestReloading(TInput input);

        protected virtual void FinishSaving(ObservableCollection<IssueModel> issues)
        {
            if (issues.Any())
            {
                this.FinishLoading(issues);
            }
            else
            {
                this.RequestReloading();
            }
        }

        protected void ResetRecalculationTimer()
        {
            this.waitBeforeRecalculating.Stop();
            this.waitBeforeRecalculating.Start();
        }

        protected void WhenTimeout(Object sender, EventArgs e)
        {
            this.waitBeforeRecalculating.Stop();
            if (this.IsRecalculationAutomatic)
            {
                this.RequestRecalculating();
            }
        }

        public DelegateCommand RequestRecalculatingCommand { get; private set; }
        public abstract void RequestRecalculating();


        private Boolean isRecalculationAutomatic;
        
        public Boolean IsRecalculationAutomatic
        {
            get { return this.isRecalculationAutomatic; }
            set
            {
                this.isRecalculationAutomatic = value;
                this.RequestRecalculatingCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged(() => this.IsRecalculationAutomatic);
            }
        }
    }
}
