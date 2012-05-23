﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50826.0
// 
namespace GreenField.ServiceCaller.SessionDefinitions {
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SessionDefinitions.SessionOperations")]
    public interface SessionOperations {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/SessionOperations/GetSession", ReplyAction="http://tempuri.org/SessionOperations/GetSessionResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.SessionDefinitions.ServiceFault), Action="http://tempuri.org/SessionOperations/GetSessionServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginGetSession(System.AsyncCallback callback, object asyncState);
        
        GreenField.DataContracts.Session EndGetSession(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/SessionOperations/SetSession", ReplyAction="http://tempuri.org/SessionOperations/SetSessionResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.SessionDefinitions.ServiceFault), Action="http://tempuri.org/SessionOperations/SetSessionServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginSetSession(GreenField.DataContracts.Session sessionVariable, System.AsyncCallback callback, object asyncState);
        
        System.Nullable<bool> EndSetSession(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SessionOperationsChannel : GreenField.ServiceCaller.SessionDefinitions.SessionOperations, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetSessionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetSessionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public GreenField.DataContracts.Session Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((GreenField.DataContracts.Session)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SetSessionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public SetSessionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public partial class SessionOperationsClient : System.ServiceModel.ClientBase<GreenField.ServiceCaller.SessionDefinitions.SessionOperations>, GreenField.ServiceCaller.SessionDefinitions.SessionOperations {
        
        private BeginOperationDelegate onBeginGetSessionDelegate;
        
        private EndOperationDelegate onEndGetSessionDelegate;
        
        private System.Threading.SendOrPostCallback onGetSessionCompletedDelegate;
        
        private BeginOperationDelegate onBeginSetSessionDelegate;
        
        private EndOperationDelegate onEndSetSessionDelegate;
        
        private System.Threading.SendOrPostCallback onSetSessionCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public SessionOperationsClient() {
        }
        
        public SessionOperationsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SessionOperationsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SessionOperationsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SessionOperationsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<GetSessionCompletedEventArgs> GetSessionCompleted;
        
        public event System.EventHandler<SetSessionCompletedEventArgs> SetSessionCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.SessionDefinitions.SessionOperations.BeginGetSession(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetSession(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GreenField.DataContracts.Session GreenField.ServiceCaller.SessionDefinitions.SessionOperations.EndGetSession(System.IAsyncResult result) {
            return base.Channel.EndGetSession(result);
        }
        
        private System.IAsyncResult OnBeginGetSession(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((GreenField.ServiceCaller.SessionDefinitions.SessionOperations)(this)).BeginGetSession(callback, asyncState);
        }
        
        private object[] OnEndGetSession(System.IAsyncResult result) {
            GreenField.DataContracts.Session retVal = ((GreenField.ServiceCaller.SessionDefinitions.SessionOperations)(this)).EndGetSession(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetSessionCompleted(object state) {
            if ((this.GetSessionCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetSessionCompleted(this, new GetSessionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetSessionAsync() {
            this.GetSessionAsync(null);
        }
        
        public void GetSessionAsync(object userState) {
            if ((this.onBeginGetSessionDelegate == null)) {
                this.onBeginGetSessionDelegate = new BeginOperationDelegate(this.OnBeginGetSession);
            }
            if ((this.onEndGetSessionDelegate == null)) {
                this.onEndGetSessionDelegate = new EndOperationDelegate(this.OnEndGetSession);
            }
            if ((this.onGetSessionCompletedDelegate == null)) {
                this.onGetSessionCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetSessionCompleted);
            }
            base.InvokeAsync(this.onBeginGetSessionDelegate, null, this.onEndGetSessionDelegate, this.onGetSessionCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.SessionDefinitions.SessionOperations.BeginSetSession(GreenField.DataContracts.Session sessionVariable, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSetSession(sessionVariable, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Nullable<bool> GreenField.ServiceCaller.SessionDefinitions.SessionOperations.EndSetSession(System.IAsyncResult result) {
            return base.Channel.EndSetSession(result);
        }
        
        private System.IAsyncResult OnBeginSetSession(object[] inValues, System.AsyncCallback callback, object asyncState) {
            GreenField.DataContracts.Session sessionVariable = ((GreenField.DataContracts.Session)(inValues[0]));
            return ((GreenField.ServiceCaller.SessionDefinitions.SessionOperations)(this)).BeginSetSession(sessionVariable, callback, asyncState);
        }
        
        private object[] OnEndSetSession(System.IAsyncResult result) {
            System.Nullable<bool> retVal = ((GreenField.ServiceCaller.SessionDefinitions.SessionOperations)(this)).EndSetSession(result);
            return new object[] {
                    retVal};
        }
        
        private void OnSetSessionCompleted(object state) {
            if ((this.SetSessionCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SetSessionCompleted(this, new SetSessionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SetSessionAsync(GreenField.DataContracts.Session sessionVariable) {
            this.SetSessionAsync(sessionVariable, null);
        }
        
        public void SetSessionAsync(GreenField.DataContracts.Session sessionVariable, object userState) {
            if ((this.onBeginSetSessionDelegate == null)) {
                this.onBeginSetSessionDelegate = new BeginOperationDelegate(this.OnBeginSetSession);
            }
            if ((this.onEndSetSessionDelegate == null)) {
                this.onEndSetSessionDelegate = new EndOperationDelegate(this.OnEndSetSession);
            }
            if ((this.onSetSessionCompletedDelegate == null)) {
                this.onSetSessionCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSetSessionCompleted);
            }
            base.InvokeAsync(this.onBeginSetSessionDelegate, new object[] {
                        sessionVariable}, this.onEndSetSessionDelegate, this.onSetSessionCompletedDelegate, userState);
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
        
        protected override GreenField.ServiceCaller.SessionDefinitions.SessionOperations CreateChannel() {
            return new SessionOperationsClientChannel(this);
        }
        
        private class SessionOperationsClientChannel : ChannelBase<GreenField.ServiceCaller.SessionDefinitions.SessionOperations>, GreenField.ServiceCaller.SessionDefinitions.SessionOperations {
            
            public SessionOperationsClientChannel(System.ServiceModel.ClientBase<GreenField.ServiceCaller.SessionDefinitions.SessionOperations> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetSession(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("GetSession", _args, callback, asyncState);
                return _result;
            }
            
            public GreenField.DataContracts.Session EndGetSession(System.IAsyncResult result) {
                object[] _args = new object[0];
                GreenField.DataContracts.Session _result = ((GreenField.DataContracts.Session)(base.EndInvoke("GetSession", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginSetSession(GreenField.DataContracts.Session sessionVariable, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = sessionVariable;
                System.IAsyncResult _result = base.BeginInvoke("SetSession", _args, callback, asyncState);
                return _result;
            }
            
            public System.Nullable<bool> EndSetSession(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Nullable<bool> _result = ((System.Nullable<bool>)(base.EndInvoke("SetSession", _args, result)));
                return _result;
            }
        }
    }
}