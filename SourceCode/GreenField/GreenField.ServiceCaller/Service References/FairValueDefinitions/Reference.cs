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
namespace GreenField.ServiceCaller.FairValueDefinitions {
    using System.Runtime.Serialization;
    
    
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
    [System.ServiceModel.ServiceContractAttribute(Namespace="", ConfigurationName="FairValueDefinitions.FairValueOperations")]
    public interface FairValueOperations {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:FairValueOperations/RetrieveFairValueCompostionSummary", ReplyAction="urn:FairValueOperations/RetrieveFairValueCompostionSummaryResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.FairValueDefinitions.ServiceFault), Action="urn:FairValueOperations/RetrieveFairValueCompostionSummaryServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginRetrieveFairValueCompostionSummary(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.AsyncCallback callback, object asyncState);
        
        System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> EndRetrieveFairValueCompostionSummary(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:FairValueOperations/RetrieveFairValueCompostionSummaryData", ReplyAction="urn:FairValueOperations/RetrieveFairValueCompostionSummaryDataResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.FairValueDefinitions.ServiceFault), Action="urn:FairValueOperations/RetrieveFairValueCompostionSummaryDataServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginRetrieveFairValueCompostionSummaryData(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.AsyncCallback callback, object asyncState);
        
        System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> EndRetrieveFairValueCompostionSummaryData(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:FairValueOperations/RetrieveFairValueDataWithNewUpside", ReplyAction="urn:FairValueOperations/RetrieveFairValueDataWithNewUpsideResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.FairValueDefinitions.ServiceFault), Action="urn:FairValueOperations/RetrieveFairValueDataWithNewUpsideServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginRetrieveFairValueDataWithNewUpside(GreenField.DataContracts.EntitySelectionData entitySelectionData, GreenField.DataContracts.FairValueCompositionSummaryData editedFairValueData, System.AsyncCallback callback, object asyncState);
        
        GreenField.DataContracts.FairValueCompositionSummaryData EndRetrieveFairValueDataWithNewUpside(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:FairValueOperations/SaveUpdatedFairValueData", ReplyAction="urn:FairValueOperations/SaveUpdatedFairValueDataResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.FairValueDefinitions.ServiceFault), Action="urn:FairValueOperations/SaveUpdatedFairValueDataServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginSaveUpdatedFairValueData(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> editedFairValueData, System.AsyncCallback callback, object asyncState);
        
        System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> EndSaveUpdatedFairValueData(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface FairValueOperationsChannel : GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RetrieveFairValueCompostionSummaryCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public RetrieveFairValueCompostionSummaryCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RetrieveFairValueCompostionSummaryDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public RetrieveFairValueCompostionSummaryDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RetrieveFairValueDataWithNewUpsideCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public RetrieveFairValueDataWithNewUpsideCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public GreenField.DataContracts.FairValueCompositionSummaryData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((GreenField.DataContracts.FairValueCompositionSummaryData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SaveUpdatedFairValueDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public SaveUpdatedFairValueDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FairValueOperationsClient : System.ServiceModel.ClientBase<GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations>, GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations {
        
        private BeginOperationDelegate onBeginRetrieveFairValueCompostionSummaryDelegate;
        
        private EndOperationDelegate onEndRetrieveFairValueCompostionSummaryDelegate;
        
        private System.Threading.SendOrPostCallback onRetrieveFairValueCompostionSummaryCompletedDelegate;
        
        private BeginOperationDelegate onBeginRetrieveFairValueCompostionSummaryDataDelegate;
        
        private EndOperationDelegate onEndRetrieveFairValueCompostionSummaryDataDelegate;
        
        private System.Threading.SendOrPostCallback onRetrieveFairValueCompostionSummaryDataCompletedDelegate;
        
        private BeginOperationDelegate onBeginRetrieveFairValueDataWithNewUpsideDelegate;
        
        private EndOperationDelegate onEndRetrieveFairValueDataWithNewUpsideDelegate;
        
        private System.Threading.SendOrPostCallback onRetrieveFairValueDataWithNewUpsideCompletedDelegate;
        
        private BeginOperationDelegate onBeginSaveUpdatedFairValueDataDelegate;
        
        private EndOperationDelegate onEndSaveUpdatedFairValueDataDelegate;
        
        private System.Threading.SendOrPostCallback onSaveUpdatedFairValueDataCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public FairValueOperationsClient() {
        }
        
        public FairValueOperationsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FairValueOperationsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FairValueOperationsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FairValueOperationsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<RetrieveFairValueCompostionSummaryCompletedEventArgs> RetrieveFairValueCompostionSummaryCompleted;
        
        public event System.EventHandler<RetrieveFairValueCompostionSummaryDataCompletedEventArgs> RetrieveFairValueCompostionSummaryDataCompleted;
        
        public event System.EventHandler<RetrieveFairValueDataWithNewUpsideCompletedEventArgs> RetrieveFairValueDataWithNewUpsideCompleted;
        
        public event System.EventHandler<SaveUpdatedFairValueDataCompletedEventArgs> SaveUpdatedFairValueDataCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.BeginRetrieveFairValueCompostionSummary(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRetrieveFairValueCompostionSummary(entitySelectionData, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.EndRetrieveFairValueCompostionSummary(System.IAsyncResult result) {
            return base.Channel.EndRetrieveFairValueCompostionSummary(result);
        }
        
        private System.IAsyncResult OnBeginRetrieveFairValueCompostionSummary(object[] inValues, System.AsyncCallback callback, object asyncState) {
            GreenField.DataContracts.EntitySelectionData entitySelectionData = ((GreenField.DataContracts.EntitySelectionData)(inValues[0]));
            return ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).BeginRetrieveFairValueCompostionSummary(entitySelectionData, callback, asyncState);
        }
        
        private object[] OnEndRetrieveFairValueCompostionSummary(System.IAsyncResult result) {
            System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> retVal = ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).EndRetrieveFairValueCompostionSummary(result);
            return new object[] {
                    retVal};
        }
        
        private void OnRetrieveFairValueCompostionSummaryCompleted(object state) {
            if ((this.RetrieveFairValueCompostionSummaryCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RetrieveFairValueCompostionSummaryCompleted(this, new RetrieveFairValueCompostionSummaryCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RetrieveFairValueCompostionSummaryAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData) {
            this.RetrieveFairValueCompostionSummaryAsync(entitySelectionData, null);
        }
        
        public void RetrieveFairValueCompostionSummaryAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData, object userState) {
            if ((this.onBeginRetrieveFairValueCompostionSummaryDelegate == null)) {
                this.onBeginRetrieveFairValueCompostionSummaryDelegate = new BeginOperationDelegate(this.OnBeginRetrieveFairValueCompostionSummary);
            }
            if ((this.onEndRetrieveFairValueCompostionSummaryDelegate == null)) {
                this.onEndRetrieveFairValueCompostionSummaryDelegate = new EndOperationDelegate(this.OnEndRetrieveFairValueCompostionSummary);
            }
            if ((this.onRetrieveFairValueCompostionSummaryCompletedDelegate == null)) {
                this.onRetrieveFairValueCompostionSummaryCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRetrieveFairValueCompostionSummaryCompleted);
            }
            base.InvokeAsync(this.onBeginRetrieveFairValueCompostionSummaryDelegate, new object[] {
                        entitySelectionData}, this.onEndRetrieveFairValueCompostionSummaryDelegate, this.onRetrieveFairValueCompostionSummaryCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.BeginRetrieveFairValueCompostionSummaryData(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRetrieveFairValueCompostionSummaryData(entitySelectionData, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.EndRetrieveFairValueCompostionSummaryData(System.IAsyncResult result) {
            return base.Channel.EndRetrieveFairValueCompostionSummaryData(result);
        }
        
        private System.IAsyncResult OnBeginRetrieveFairValueCompostionSummaryData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            GreenField.DataContracts.EntitySelectionData entitySelectionData = ((GreenField.DataContracts.EntitySelectionData)(inValues[0]));
            return ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).BeginRetrieveFairValueCompostionSummaryData(entitySelectionData, callback, asyncState);
        }
        
        private object[] OnEndRetrieveFairValueCompostionSummaryData(System.IAsyncResult result) {
            System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> retVal = ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).EndRetrieveFairValueCompostionSummaryData(result);
            return new object[] {
                    retVal};
        }
        
        private void OnRetrieveFairValueCompostionSummaryDataCompleted(object state) {
            if ((this.RetrieveFairValueCompostionSummaryDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RetrieveFairValueCompostionSummaryDataCompleted(this, new RetrieveFairValueCompostionSummaryDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RetrieveFairValueCompostionSummaryDataAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData) {
            this.RetrieveFairValueCompostionSummaryDataAsync(entitySelectionData, null);
        }
        
        public void RetrieveFairValueCompostionSummaryDataAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData, object userState) {
            if ((this.onBeginRetrieveFairValueCompostionSummaryDataDelegate == null)) {
                this.onBeginRetrieveFairValueCompostionSummaryDataDelegate = new BeginOperationDelegate(this.OnBeginRetrieveFairValueCompostionSummaryData);
            }
            if ((this.onEndRetrieveFairValueCompostionSummaryDataDelegate == null)) {
                this.onEndRetrieveFairValueCompostionSummaryDataDelegate = new EndOperationDelegate(this.OnEndRetrieveFairValueCompostionSummaryData);
            }
            if ((this.onRetrieveFairValueCompostionSummaryDataCompletedDelegate == null)) {
                this.onRetrieveFairValueCompostionSummaryDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRetrieveFairValueCompostionSummaryDataCompleted);
            }
            base.InvokeAsync(this.onBeginRetrieveFairValueCompostionSummaryDataDelegate, new object[] {
                        entitySelectionData}, this.onEndRetrieveFairValueCompostionSummaryDataDelegate, this.onRetrieveFairValueCompostionSummaryDataCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.BeginRetrieveFairValueDataWithNewUpside(GreenField.DataContracts.EntitySelectionData entitySelectionData, GreenField.DataContracts.FairValueCompositionSummaryData editedFairValueData, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRetrieveFairValueDataWithNewUpside(entitySelectionData, editedFairValueData, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GreenField.DataContracts.FairValueCompositionSummaryData GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.EndRetrieveFairValueDataWithNewUpside(System.IAsyncResult result) {
            return base.Channel.EndRetrieveFairValueDataWithNewUpside(result);
        }
        
        private System.IAsyncResult OnBeginRetrieveFairValueDataWithNewUpside(object[] inValues, System.AsyncCallback callback, object asyncState) {
            GreenField.DataContracts.EntitySelectionData entitySelectionData = ((GreenField.DataContracts.EntitySelectionData)(inValues[0]));
            GreenField.DataContracts.FairValueCompositionSummaryData editedFairValueData = ((GreenField.DataContracts.FairValueCompositionSummaryData)(inValues[1]));
            return ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).BeginRetrieveFairValueDataWithNewUpside(entitySelectionData, editedFairValueData, callback, asyncState);
        }
        
        private object[] OnEndRetrieveFairValueDataWithNewUpside(System.IAsyncResult result) {
            GreenField.DataContracts.FairValueCompositionSummaryData retVal = ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).EndRetrieveFairValueDataWithNewUpside(result);
            return new object[] {
                    retVal};
        }
        
        private void OnRetrieveFairValueDataWithNewUpsideCompleted(object state) {
            if ((this.RetrieveFairValueDataWithNewUpsideCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RetrieveFairValueDataWithNewUpsideCompleted(this, new RetrieveFairValueDataWithNewUpsideCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RetrieveFairValueDataWithNewUpsideAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData, GreenField.DataContracts.FairValueCompositionSummaryData editedFairValueData) {
            this.RetrieveFairValueDataWithNewUpsideAsync(entitySelectionData, editedFairValueData, null);
        }
        
        public void RetrieveFairValueDataWithNewUpsideAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData, GreenField.DataContracts.FairValueCompositionSummaryData editedFairValueData, object userState) {
            if ((this.onBeginRetrieveFairValueDataWithNewUpsideDelegate == null)) {
                this.onBeginRetrieveFairValueDataWithNewUpsideDelegate = new BeginOperationDelegate(this.OnBeginRetrieveFairValueDataWithNewUpside);
            }
            if ((this.onEndRetrieveFairValueDataWithNewUpsideDelegate == null)) {
                this.onEndRetrieveFairValueDataWithNewUpsideDelegate = new EndOperationDelegate(this.OnEndRetrieveFairValueDataWithNewUpside);
            }
            if ((this.onRetrieveFairValueDataWithNewUpsideCompletedDelegate == null)) {
                this.onRetrieveFairValueDataWithNewUpsideCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRetrieveFairValueDataWithNewUpsideCompleted);
            }
            base.InvokeAsync(this.onBeginRetrieveFairValueDataWithNewUpsideDelegate, new object[] {
                        entitySelectionData,
                        editedFairValueData}, this.onEndRetrieveFairValueDataWithNewUpsideDelegate, this.onRetrieveFairValueDataWithNewUpsideCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.BeginSaveUpdatedFairValueData(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> editedFairValueData, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSaveUpdatedFairValueData(entitySelectionData, editedFairValueData, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations.EndSaveUpdatedFairValueData(System.IAsyncResult result) {
            return base.Channel.EndSaveUpdatedFairValueData(result);
        }
        
        private System.IAsyncResult OnBeginSaveUpdatedFairValueData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            GreenField.DataContracts.EntitySelectionData entitySelectionData = ((GreenField.DataContracts.EntitySelectionData)(inValues[0]));
            System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> editedFairValueData = ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(inValues[1]));
            return ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).BeginSaveUpdatedFairValueData(entitySelectionData, editedFairValueData, callback, asyncState);
        }
        
        private object[] OnEndSaveUpdatedFairValueData(System.IAsyncResult result) {
            System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> retVal = ((GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations)(this)).EndSaveUpdatedFairValueData(result);
            return new object[] {
                    retVal};
        }
        
        private void OnSaveUpdatedFairValueDataCompleted(object state) {
            if ((this.SaveUpdatedFairValueDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SaveUpdatedFairValueDataCompleted(this, new SaveUpdatedFairValueDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SaveUpdatedFairValueDataAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> editedFairValueData) {
            this.SaveUpdatedFairValueDataAsync(entitySelectionData, editedFairValueData, null);
        }
        
        public void SaveUpdatedFairValueDataAsync(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> editedFairValueData, object userState) {
            if ((this.onBeginSaveUpdatedFairValueDataDelegate == null)) {
                this.onBeginSaveUpdatedFairValueDataDelegate = new BeginOperationDelegate(this.OnBeginSaveUpdatedFairValueData);
            }
            if ((this.onEndSaveUpdatedFairValueDataDelegate == null)) {
                this.onEndSaveUpdatedFairValueDataDelegate = new EndOperationDelegate(this.OnEndSaveUpdatedFairValueData);
            }
            if ((this.onSaveUpdatedFairValueDataCompletedDelegate == null)) {
                this.onSaveUpdatedFairValueDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSaveUpdatedFairValueDataCompleted);
            }
            base.InvokeAsync(this.onBeginSaveUpdatedFairValueDataDelegate, new object[] {
                        entitySelectionData,
                        editedFairValueData}, this.onEndSaveUpdatedFairValueDataDelegate, this.onSaveUpdatedFairValueDataCompletedDelegate, userState);
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
        
        protected override GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations CreateChannel() {
            return new FairValueOperationsClientChannel(this);
        }
        
        private class FairValueOperationsClientChannel : ChannelBase<GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations>, GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations {
            
            public FairValueOperationsClientChannel(System.ServiceModel.ClientBase<GreenField.ServiceCaller.FairValueDefinitions.FairValueOperations> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginRetrieveFairValueCompostionSummary(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = entitySelectionData;
                System.IAsyncResult _result = base.BeginInvoke("RetrieveFairValueCompostionSummary", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> EndRetrieveFairValueCompostionSummary(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> _result = ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(base.EndInvoke("RetrieveFairValueCompostionSummary", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginRetrieveFairValueCompostionSummaryData(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = entitySelectionData;
                System.IAsyncResult _result = base.BeginInvoke("RetrieveFairValueCompostionSummaryData", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> EndRetrieveFairValueCompostionSummaryData(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> _result = ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(base.EndInvoke("RetrieveFairValueCompostionSummaryData", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginRetrieveFairValueDataWithNewUpside(GreenField.DataContracts.EntitySelectionData entitySelectionData, GreenField.DataContracts.FairValueCompositionSummaryData editedFairValueData, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = entitySelectionData;
                _args[1] = editedFairValueData;
                System.IAsyncResult _result = base.BeginInvoke("RetrieveFairValueDataWithNewUpside", _args, callback, asyncState);
                return _result;
            }
            
            public GreenField.DataContracts.FairValueCompositionSummaryData EndRetrieveFairValueDataWithNewUpside(System.IAsyncResult result) {
                object[] _args = new object[0];
                GreenField.DataContracts.FairValueCompositionSummaryData _result = ((GreenField.DataContracts.FairValueCompositionSummaryData)(base.EndInvoke("RetrieveFairValueDataWithNewUpside", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginSaveUpdatedFairValueData(GreenField.DataContracts.EntitySelectionData entitySelectionData, System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> editedFairValueData, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = entitySelectionData;
                _args[1] = editedFairValueData;
                System.IAsyncResult _result = base.BeginInvoke("SaveUpdatedFairValueData", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> EndSaveUpdatedFairValueData(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData> _result = ((System.Collections.Generic.List<GreenField.DataContracts.FairValueCompositionSummaryData>)(base.EndInvoke("SaveUpdatedFairValueData", _args, result)));
                return _result;
            }
        }
    }
}
