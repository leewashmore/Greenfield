using System.ComponentModel;


namespace GreenField.Common.DragGridPanelControls
{
    public class LayoutDefinition : INotifyPropertyChanged
    {
        private DependencyObjectCollection _definitions;

        public DependencyObjectCollection Definitions
        {
            get { return _definitions; }
            set
            {
                _definitions = value;
                Notify("Definitions");
            }
        }

        private CellCollection _cells;

        public CellCollection Cells
        {
            get { return _cells; }
            set { 
                _cells = value;
                Notify("Cells");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}