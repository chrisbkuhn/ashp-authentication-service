﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ashp.AuthenticationService.AshpService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthenticationResponse", Namespace="http://schemas.datacontract.org/2004/07/")]
    [System.SerializableAttribute()]
    public partial class AuthenticationResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime ExpirationDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool LoginOKField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ProductPurchasedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ServerOKField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorMessage {
            get {
                return this.ErrorMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true)) {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ExpirationDate {
            get {
                return this.ExpirationDateField;
            }
            set {
                if ((this.ExpirationDateField.Equals(value) != true)) {
                    this.ExpirationDateField = value;
                    this.RaisePropertyChanged("ExpirationDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool LoginOK {
            get {
                return this.LoginOKField;
            }
            set {
                if ((this.LoginOKField.Equals(value) != true)) {
                    this.LoginOKField = value;
                    this.RaisePropertyChanged("LoginOK");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ProductPurchased {
            get {
                return this.ProductPurchasedField;
            }
            set {
                if ((this.ProductPurchasedField.Equals(value) != true)) {
                    this.ProductPurchasedField = value;
                    this.RaisePropertyChanged("ProductPurchased");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ServerOK {
            get {
                return this.ServerOKField;
            }
            set {
                if ((this.ServerOKField.Equals(value) != true)) {
                    this.ServerOKField = value;
                    this.RaisePropertyChanged("ServerOK");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AshpService.IASHPService")]
    public interface IASHPService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IASHPService/ASHPProductAuthentication", ReplyAction="http://tempuri.org/IASHPService/ASHPProductAuthenticationResponse")]
        Ashp.AuthenticationService.AshpService.AuthenticationResponse ASHPProductAuthentication(string email, string password, string productcode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IASHPService/ASHPProductAuthentication", ReplyAction="http://tempuri.org/IASHPService/ASHPProductAuthenticationResponse")]
        System.Threading.Tasks.Task<Ashp.AuthenticationService.AshpService.AuthenticationResponse> ASHPProductAuthenticationAsync(string email, string password, string productcode);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IASHPServiceChannel : Ashp.AuthenticationService.AshpService.IASHPService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ASHPServiceClient : System.ServiceModel.ClientBase<Ashp.AuthenticationService.AshpService.IASHPService>, Ashp.AuthenticationService.AshpService.IASHPService {
        
        public ASHPServiceClient() {
        }
        
        public ASHPServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ASHPServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ASHPServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ASHPServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Ashp.AuthenticationService.AshpService.AuthenticationResponse ASHPProductAuthentication(string email, string password, string productcode) {
            return base.Channel.ASHPProductAuthentication(email, password, productcode);
        }
        
        public System.Threading.Tasks.Task<Ashp.AuthenticationService.AshpService.AuthenticationResponse> ASHPProductAuthenticationAsync(string email, string password, string productcode) {
            return base.Channel.ASHPProductAuthenticationAsync(email, password, productcode);
        }
    }
}