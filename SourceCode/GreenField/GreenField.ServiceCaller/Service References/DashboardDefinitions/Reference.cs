﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50826.0
// 
namespace GreenField.ServiceCaller.DashboardDefinitions {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="tblDashboardPreference", Namespace="http://schemas.datacontract.org/2004/07/GreenField.DAL", IsReference=true)]
    public partial class tblDashboardPreference : GreenField.ServiceCaller.DashboardDefinitions.EntityObject {
        
        private string GadgetNameField;
        
        private int GadgetPositionField;
        
        private string GadgetStateField;
        
        private string GadgetViewClassNameField;
        
        private string GadgetViewModelClassNameField;
        
        private string PreferenceGroupIDField;
        
        private int PreferenceIDField;
        
        private string UserNameField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GadgetName {
            get {
                return this.GadgetNameField;
            }
            set {
                if ((object.ReferenceEquals(this.GadgetNameField, value) != true)) {
                    this.GadgetNameField = value;
                    this.RaisePropertyChanged("GadgetName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int GadgetPosition {
            get {
                return this.GadgetPositionField;
            }
            set {
                if ((this.GadgetPositionField.Equals(value) != true)) {
                    this.GadgetPositionField = value;
                    this.RaisePropertyChanged("GadgetPosition");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GadgetState {
            get {
                return this.GadgetStateField;
            }
            set {
                if ((object.ReferenceEquals(this.GadgetStateField, value) != true)) {
                    this.GadgetStateField = value;
                    this.RaisePropertyChanged("GadgetState");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GadgetViewClassName {
            get {
                return this.GadgetViewClassNameField;
            }
            set {
                if ((object.ReferenceEquals(this.GadgetViewClassNameField, value) != true)) {
                    this.GadgetViewClassNameField = value;
                    this.RaisePropertyChanged("GadgetViewClassName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GadgetViewModelClassName {
            get {
                return this.GadgetViewModelClassNameField;
            }
            set {
                if ((object.ReferenceEquals(this.GadgetViewModelClassNameField, value) != true)) {
                    this.GadgetViewModelClassNameField = value;
                    this.RaisePropertyChanged("GadgetViewModelClassName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PreferenceGroupID {
            get {
                return this.PreferenceGroupIDField;
            }
            set {
                if ((object.ReferenceEquals(this.PreferenceGroupIDField, value) != true)) {
                    this.PreferenceGroupIDField = value;
                    this.RaisePropertyChanged("PreferenceGroupID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PreferenceID {
            get {
                return this.PreferenceIDField;
            }
            set {
                if ((this.PreferenceIDField.Equals(value) != true)) {
                    this.PreferenceIDField = value;
                    this.RaisePropertyChanged("PreferenceID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserName {
            get {
                return this.UserNameField;
            }
            set {
                if ((object.ReferenceEquals(this.UserNameField, value) != true)) {
                    this.UserNameField = value;
                    this.RaisePropertyChanged("UserName");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StructuralObject", Namespace="http://schemas.datacontract.org/2004/07/System.Data.Objects.DataClasses", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.EntityObject))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference))]
    public partial class StructuralObject : object, System.ComponentModel.INotifyPropertyChanged {
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EntityObject", Namespace="http://schemas.datacontract.org/2004/07/System.Data.Objects.DataClasses", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference))]
    public partial class EntityObject : GreenField.ServiceCaller.DashboardDefinitions.StructuralObject {
        
        private GreenField.ServiceCaller.DashboardDefinitions.EntityKey EntityKeyField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public GreenField.ServiceCaller.DashboardDefinitions.EntityKey EntityKey {
            get {
                return this.EntityKeyField;
            }
            set {
                if ((object.ReferenceEquals(this.EntityKeyField, value) != true)) {
                    this.EntityKeyField = value;
                    this.RaisePropertyChanged("EntityKey");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EntityKey", Namespace="http://schemas.datacontract.org/2004/07/System.Data", IsReference=true)]
    public partial class EntityKey : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string EntityContainerNameField;
        
        private System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.EntityKeyMember> EntityKeyValuesField;
        
        private string EntitySetNameField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EntityContainerName {
            get {
                return this.EntityContainerNameField;
            }
            set {
                if ((object.ReferenceEquals(this.EntityContainerNameField, value) != true)) {
                    this.EntityContainerNameField = value;
                    this.RaisePropertyChanged("EntityContainerName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.EntityKeyMember> EntityKeyValues {
            get {
                return this.EntityKeyValuesField;
            }
            set {
                if ((object.ReferenceEquals(this.EntityKeyValuesField, value) != true)) {
                    this.EntityKeyValuesField = value;
                    this.RaisePropertyChanged("EntityKeyValues");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EntitySetName {
            get {
                return this.EntitySetNameField;
            }
            set {
                if ((object.ReferenceEquals(this.EntitySetNameField, value) != true)) {
                    this.EntitySetNameField = value;
                    this.RaisePropertyChanged("EntitySetName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EntityKeyMember", Namespace="http://schemas.datacontract.org/2004/07/System.Data")]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.EntityKey))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.EntityKeyMember>))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.EntityObject))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.StructuralObject))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference>))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.ServiceFault))]
    public partial class EntityKeyMember : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string KeyField;
        
        private object ValueField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Key {
            get {
                return this.KeyField;
            }
            set {
                if ((object.ReferenceEquals(this.KeyField, value) != true)) {
                    this.KeyField = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Value {
            get {
                return this.ValueField;
            }
            set {
                if ((object.ReferenceEquals(this.ValueField, value) != true)) {
                    this.ValueField = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
    public partial class ServiceFault : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string DescriptionField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description {
            get {
                return this.DescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.DescriptionField, value) != true)) {
                    this.DescriptionField = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DashboardDefinitions.DashboardOperations")]
    public interface DashboardOperations {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/DashboardOperations/GetDashboardPreferenceByUserName", ReplyAction="http://tempuri.org/DashboardOperations/GetDashboardPreferenceByUserNameResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.ServiceFault), Action="http://tempuri.org/DashboardOperations/GetDashboardPreferenceByUserNameServiceFau" +
            "ltFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginGetDashboardPreferenceByUserName(string userName, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> EndGetDashboardPreferenceByUserName(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/DashboardOperations/SetDashboardPreference", ReplyAction="http://tempuri.org/DashboardOperations/SetDashboardPreferenceResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.DashboardDefinitions.ServiceFault), Action="http://tempuri.org/DashboardOperations/SetDashboardPreferenceServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginSetDashboardPreference(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> dashBoardPreference, string userName, System.AsyncCallback callback, object asyncState);
        
        System.Nullable<bool> EndSetDashboardPreference(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface DashboardOperationsChannel : GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetDashboardPreferenceByUserNameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetDashboardPreferenceByUserNameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SetDashboardPreferenceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public SetDashboardPreferenceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Nullable<bool> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Nullable<bool>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DashboardOperationsClient : System.ServiceModel.ClientBase<GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations>, GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations {
        
        private BeginOperationDelegate onBeginGetDashboardPreferenceByUserNameDelegate;
        
        private EndOperationDelegate onEndGetDashboardPreferenceByUserNameDelegate;
        
        private System.Threading.SendOrPostCallback onGetDashboardPreferenceByUserNameCompletedDelegate;
        
        private BeginOperationDelegate onBeginSetDashboardPreferenceDelegate;
        
        private EndOperationDelegate onEndSetDashboardPreferenceDelegate;
        
        private System.Threading.SendOrPostCallback onSetDashboardPreferenceCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public DashboardOperationsClient() {
        }
        
        public DashboardOperationsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DashboardOperationsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DashboardOperationsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DashboardOperationsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetDashboardPreferenceByUserNameCompletedEventArgs> GetDashboardPreferenceByUserNameCompleted;
        
        public event System.EventHandler<SetDashboardPreferenceCompletedEventArgs> SetDashboardPreferenceCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations.BeginGetDashboardPreferenceByUserName(string userName, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetDashboardPreferenceByUserName(userName, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations.EndGetDashboardPreferenceByUserName(System.IAsyncResult result) {
            return base.Channel.EndGetDashboardPreferenceByUserName(result);
        }
        
        private System.IAsyncResult OnBeginGetDashboardPreferenceByUserName(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string userName = ((string)(inValues[0]));
            return ((GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations)(this)).BeginGetDashboardPreferenceByUserName(userName, callback, asyncState);
        }
        
        private object[] OnEndGetDashboardPreferenceByUserName(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> retVal = ((GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations)(this)).EndGetDashboardPreferenceByUserName(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetDashboardPreferenceByUserNameCompleted(object state) {
            if ((this.GetDashboardPreferenceByUserNameCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetDashboardPreferenceByUserNameCompleted(this, new GetDashboardPreferenceByUserNameCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetDashboardPreferenceByUserNameAsync(string userName) {
            this.GetDashboardPreferenceByUserNameAsync(userName, null);
        }
        
        public void GetDashboardPreferenceByUserNameAsync(string userName, object userState) {
            if ((this.onBeginGetDashboardPreferenceByUserNameDelegate == null)) {
                this.onBeginGetDashboardPreferenceByUserNameDelegate = new BeginOperationDelegate(this.OnBeginGetDashboardPreferenceByUserName);
            }
            if ((this.onEndGetDashboardPreferenceByUserNameDelegate == null)) {
                this.onEndGetDashboardPreferenceByUserNameDelegate = new EndOperationDelegate(this.OnEndGetDashboardPreferenceByUserName);
            }
            if ((this.onGetDashboardPreferenceByUserNameCompletedDelegate == null)) {
                this.onGetDashboardPreferenceByUserNameCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetDashboardPreferenceByUserNameCompleted);
            }
            base.InvokeAsync(this.onBeginGetDashboardPreferenceByUserNameDelegate, new object[] {
                        userName}, this.onEndGetDashboardPreferenceByUserNameDelegate, this.onGetDashboardPreferenceByUserNameCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations.BeginSetDashboardPreference(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> dashBoardPreference, string userName, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSetDashboardPreference(dashBoardPreference, userName, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Nullable<bool> GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations.EndSetDashboardPreference(System.IAsyncResult result) {
            return base.Channel.EndSetDashboardPreference(result);
        }
        
        private System.IAsyncResult OnBeginSetDashboardPreference(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> dashBoardPreference = ((System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference>)(inValues[0]));
            string userName = ((string)(inValues[1]));
            return ((GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations)(this)).BeginSetDashboardPreference(dashBoardPreference, userName, callback, asyncState);
        }
        
        private object[] OnEndSetDashboardPreference(System.IAsyncResult result) {
            System.Nullable<bool> retVal = ((GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations)(this)).EndSetDashboardPreference(result);
            return new object[] {
                    retVal};
        }
        
        private void OnSetDashboardPreferenceCompleted(object state) {
            if ((this.SetDashboardPreferenceCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SetDashboardPreferenceCompleted(this, new SetDashboardPreferenceCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SetDashboardPreferenceAsync(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> dashBoardPreference, string userName) {
            this.SetDashboardPreferenceAsync(dashBoardPreference, userName, null);
        }
        
        public void SetDashboardPreferenceAsync(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> dashBoardPreference, string userName, object userState) {
            if ((this.onBeginSetDashboardPreferenceDelegate == null)) {
                this.onBeginSetDashboardPreferenceDelegate = new BeginOperationDelegate(this.OnBeginSetDashboardPreference);
            }
            if ((this.onEndSetDashboardPreferenceDelegate == null)) {
                this.onEndSetDashboardPreferenceDelegate = new EndOperationDelegate(this.OnEndSetDashboardPreference);
            }
            if ((this.onSetDashboardPreferenceCompletedDelegate == null)) {
                this.onSetDashboardPreferenceCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSetDashboardPreferenceCompleted);
            }
            base.InvokeAsync(this.onBeginSetDashboardPreferenceDelegate, new object[] {
                        dashBoardPreference,
                        userName}, this.onEndSetDashboardPreferenceDelegate, this.onSetDashboardPreferenceCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations CreateChannel() {
            return new DashboardOperationsClientChannel(this);
        }
        
        private class DashboardOperationsClientChannel : ChannelBase<GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations>, GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations {
            
            public DashboardOperationsClientChannel(System.ServiceModel.ClientBase<GreenField.ServiceCaller.DashboardDefinitions.DashboardOperations> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetDashboardPreferenceByUserName(string userName, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = userName;
                System.IAsyncResult _result = base.BeginInvoke("GetDashboardPreferenceByUserName", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> EndGetDashboardPreferenceByUserName(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> _result = ((System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference>)(base.EndInvoke("GetDashboardPreferenceByUserName", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginSetDashboardPreference(System.Collections.ObjectModel.ObservableCollection<GreenField.ServiceCaller.DashboardDefinitions.tblDashboardPreference> dashBoardPreference, string userName, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = dashBoardPreference;
                _args[1] = userName;
                System.IAsyncResult _result = base.BeginInvoke("SetDashboardPreference", _args, callback, asyncState);
                return _result;
            }
            
            public System.Nullable<bool> EndSetDashboardPreference(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Nullable<bool> _result = ((System.Nullable<bool>)(base.EndInvoke("SetDashboardPreference", _args, result)));
                return _result;
            }
        }
    }
}
