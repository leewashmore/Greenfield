using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using GreenField.DashBoardModule.ViewModels.Targeting;

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    /// <summary>
    /// Picker + Editor
    /// </summary>
    [Export]
    public partial class RootView : UserControl, INavigationAware
    {
        private IEventAggregator eventAgregator;

        [ImportingConstructor]
        public RootView(IEventAggregator eventAggregator)
        {
            this.InitializeComponent();
            this.eventAgregator = eventAggregator;
            var @event = this.eventAgregator.GetEvent<RunEvent>().Subscribe(this.Initialize);
        }

        public void Initialize(Settings settings)
        {
            var modelTraverser = new ModelTraverser();
            var editorViewModel = new EditorViewModel(
                settings.ClientFactory,
                settings.ModelTraverser,
                settings.DefaultExpandCollapseStateSetter
            );
            var pickerViewModel = new PickerViewModel(settings.ClientFactory);

            var rootModel = new RootViewModel(
                pickerViewModel,
                editorViewModel,
                settings.BenchmarkDate
            );
            
            this.DataContext = rootModel;
        }

        public Boolean IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            /*ViewBaseUserControl control = (ViewBaseUserControl)this.placeholder.Content;
            if (control != null)
            {
                control.IsActive = false;
            }*/
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            /*
            ViewBaseUserControl control = (ViewBaseUserControl)this.placeholder.Content;
            if (control != null)
            {
                control.IsActive = true;
            }*/
        }
    }
}
