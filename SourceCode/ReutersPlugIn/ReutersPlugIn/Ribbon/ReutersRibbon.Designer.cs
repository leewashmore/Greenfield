namespace ReutersPlugIn
{
    partial class ReutersRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ReutersRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabReuters = this.Factory.CreateRibbonTab();
            this.groupReuters = this.Factory.CreateRibbonGroup();
            this.btnImport = this.Factory.CreateRibbonButton();
            this.button1 = this.Factory.CreateRibbonButton();
            this.tabReuters.SuspendLayout();
            this.groupReuters.SuspendLayout();
            // 
            // tabReuters
            // 
            this.tabReuters.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabReuters.ControlId.OfficeId = "TabReview";
            this.tabReuters.Groups.Add(this.groupReuters);
            this.tabReuters.Label = "TabReview";
            this.tabReuters.Name = "tabReuters";
            // 
            // groupReuters
            // 
            this.groupReuters.Items.Add(this.btnImport);
            this.groupReuters.Items.Add(this.button1);
            this.groupReuters.Label = "Reuters";
            this.groupReuters.Name = "groupReuters";
            // 
            // btnImport
            // 
            this.btnImport.Label = "Refresh";
            this.btnImport.Name = "btnImport";
            this.btnImport.OfficeImageId = "RefreshAll";
            this.btnImport.ShowImage = true;
            this.btnImport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnImport_Click);
            // 
            // button1
            // 
            this.button1.ImageName = "Undo";
            this.button1.Label = "Display Pane";
            this.button1.Name = "button1";
            this.button1.OfficeImageId = "ShowClipboard";
            this.button1.ShowImage = true;
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // ReutersRibbon
            // 
            this.Name = "ReutersRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabReuters);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ReutersRibbon_Load);
            this.tabReuters.ResumeLayout(false);
            this.tabReuters.PerformLayout();
            this.groupReuters.ResumeLayout(false);
            this.groupReuters.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabReuters;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupReuters;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnImport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
    }

    partial class ThisRibbonCollection
    {
        internal ReutersRibbon ReutersRibbon
        {
            get { return this.GetRibbon<ReutersRibbon>(); }
        }
    }
}
