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
namespace GreenField.ServiceCaller.LoggingDefinitions {
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LoggingDefinitions.LoggingOperations")]
    public interface LoggingOperations {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/LoggingOperations/LogToFile", ReplyAction="http://tempuri.org/LoggingOperations/LogToFileResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.LoggingDefinitions.ServiceFault), Action="http://tempuri.org/LoggingOperations/LogToFileServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginLogToFile(string message, string category, string priority, System.AsyncCallback callback, object asyncState);
        
        void EndLogToFile(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/LoggingOperations/GetLoggingLevel", ReplyAction="http://tempuri.org/LoggingOperations/GetLoggingLevelResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(GreenField.ServiceCaller.LoggingDefinitions.ServiceFault), Action="http://tempuri.org/LoggingOperations/GetLoggingLevelServiceFaultFault", Name="ServiceFault", Namespace="http://schemas.datacontract.org/2004/07/GreenField.Web.Helpers.Service_Faults")]
        System.IAsyncResult BeginGetLoggingLevel(System.AsyncCallback callback, object asyncState);
        
        int EndGetLoggingLevel(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LoggingOperationsChannel : GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetLoggingLevelCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetLoggingLevelCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public int Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LoggingOperationsClient : System.ServiceModel.ClientBase<GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations>, GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations {
        
        private BeginOperationDelegate onBeginLogToFileDelegate;
        
        private EndOperationDelegate onEndLogToFileDelegate;
        
        private System.Threading.SendOrPostCallback onLogToFileCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetLoggingLevelDelegate;
        
        private EndOperationDelegate onEndGetLoggingLevelDelegate;
        
        private System.Threading.SendOrPostCallback onGetLoggingLevelCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public LoggingOperationsClient() {
        }
        
        public LoggingOperationsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LoggingOperationsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoggingOperationsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoggingOperationsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> LogToFileCompleted;
        
        public event System.EventHandler<GetLoggingLevelCompletedEventArgs> GetLoggingLevelCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations.BeginLogToFile(string message, string category, string priority, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginLogToFile(message, category, priority, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations.EndLogToFile(System.IAsyncResult result) {
            base.Channel.EndLogToFile(result);
        }
        
        private System.IAsyncResult OnBeginLogToFile(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string message = ((string)(inValues[0]));
            string category = ((string)(inValues[1]));
            string priority = ((string)(inValues[2]));
            return ((GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations)(this)).BeginLogToFile(message, category, priority, callback, asyncState);
        }
        
        private object[] OnEndLogToFile(System.IAsyncResult result) {
            ((GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations)(this)).EndLogToFile(result);
            return null;
        }
        
        private void OnLogToFileCompleted(object state) {
            if ((this.LogToFileCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.LogToFileCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void LogToFileAsync(string message, string category, string priority) {
            this.LogToFileAsync(message, category, priority, null);
        }
        
        public void LogToFileAsync(string message, string category, string priority, object userState) {
            if ((this.onBeginLogToFileDelegate == null)) {
                this.onBeginLogToFileDelegate = new BeginOperationDelegate(this.OnBeginLogToFile);
            }
            if ((this.onEndLogToFileDelegate == null)) {
                this.onEndLogToFileDelegate = new EndOperationDelegate(this.OnEndLogToFile);
            }
            if ((this.onLogToFileCompletedDelegate == null)) {
                this.onLogToFileCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnLogToFileCompleted);
            }
            base.InvokeAsync(this.onBeginLogToFileDelegate, new object[] {
                        message,
                        category,
                        priority}, this.onEndLogToFileDelegate, this.onLogToFileCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations.BeginGetLoggingLevel(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetLoggingLevel(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        int GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations.EndGetLoggingLevel(System.IAsyncResult result) {
            return base.Channel.EndGetLoggingLevel(result);
        }
        
        private System.IAsyncResult OnBeginGetLoggingLevel(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations)(this)).BeginGetLoggingLevel(callback, asyncState);
        }
        
        private object[] OnEndGetLoggingLevel(System.IAsyncResult result) {
            int retVal = ((GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations)(this)).EndGetLoggingLevel(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetLoggingLevelCompleted(object state) {
            if ((this.GetLoggingLevelCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetLoggingLevelCompleted(this, new GetLoggingLevelCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetLoggingLevelAsync() {
            this.GetLoggingLevelAsync(null);
        }
        
        public void GetLoggingLevelAsync(object userState) {
            if ((this.onBeginGetLoggingLevelDelegate == null)) {
                this.onBeginGetLoggingLevelDelegate = new BeginOperationDelegate(this.OnBeginGetLoggingLevel);
            }
            if ((this.onEndGetLoggingLevelDelegate == null)) {
                this.onEndGetLoggingLevelDelegate = new EndOperationDelegate(this.OnEndGetLoggingLevel);
            }
            if ((this.onGetLoggingLevelCompletedDelegate == null)) {
                this.onGetLoggingLevelCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetLoggingLevelCompleted);
            }
            base.InvokeAsync(this.onBeginGetLoggingLevelDelegate, null, this.onEndGetLoggingLevelDelegate, this.onGetLoggingLevelCompletedDelegate, userState);
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
        
        protected override GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations CreateChannel() {
            return new LoggingOperationsClientChannel(this);
        }
        
        private class LoggingOperationsClientChannel : ChannelBase<GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations>, GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations {
            
            public LoggingOperationsClientChannel(System.ServiceModel.ClientBase<GreenField.ServiceCaller.LoggingDefinitions.LoggingOperations> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginLogToFile(string message, string category, string priority, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[3];
                _args[0] = message;
                _args[1] = category;
                _args[2] = priority;
                System.IAsyncResult _result = base.BeginInvoke("LogToFile", _args, callback, asyncState);
                return _result;
            }
            
            public void EndLogToFile(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("LogToFile", _args, result);
            }
            
            public System.IAsyncResult BeginGetLoggingLevel(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("GetLoggingLevel", _args, callback, asyncState);
                return _result;
            }
            
            public int EndGetLoggingLevel(System.IAsyncResult result) {
                object[] _args = new object[0];
                int _result = ((int)(base.EndInvoke("GetLoggingLevel", _args, result)));
                return _result;
            }
        }
    }
}
