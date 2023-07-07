﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchedulerEarlyClaim.sms {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="sms.INetcoreService")]
    public interface INetcoreService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/INetcoreService/NetcoreMessageSender", ReplyAction="http://tempuri.org/INetcoreService/NetcoreMessageSenderResponse")]
        SchedulerEarlyClaim.sms.ServiceResponse NetcoreMessageSender(SchedulerEarlyClaim.sms.ServiceRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/INetcoreService/NetcoreMessageSender", ReplyAction="http://tempuri.org/INetcoreService/NetcoreMessageSenderResponse")]
        System.Threading.Tasks.Task<SchedulerEarlyClaim.sms.ServiceResponse> NetcoreMessageSenderAsync(SchedulerEarlyClaim.sms.ServiceRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ServiceRequest", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ServiceRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string ApplicationNo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string DataKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string DataValue;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=3)]
        public string MessageText;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=4)]
        public string MobileNumber;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=5)]
        public string PolicyNo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=6)]
        public string SMS_Resource;
        
        public ServiceRequest() {
        }
        
        public ServiceRequest(string ApplicationNo, string DataKey, string DataValue, string MessageText, string MobileNumber, string PolicyNo, string SMS_Resource) {
            this.ApplicationNo = ApplicationNo;
            this.DataKey = DataKey;
            this.DataValue = DataValue;
            this.MessageText = MessageText;
            this.MobileNumber = MobileNumber;
            this.PolicyNo = PolicyNo;
            this.SMS_Resource = SMS_Resource;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ServiceResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ServiceResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string ErrorCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string ErrorDescr;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string RequestID;
        
        public ServiceResponse() {
        }
        
        public ServiceResponse(string ErrorCode, string ErrorDescr, string RequestID) {
            this.ErrorCode = ErrorCode;
            this.ErrorDescr = ErrorDescr;
            this.RequestID = RequestID;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface INetcoreServiceChannel : SchedulerEarlyClaim.sms.INetcoreService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class NetcoreServiceClient : System.ServiceModel.ClientBase<SchedulerEarlyClaim.sms.INetcoreService>, SchedulerEarlyClaim.sms.INetcoreService {
        
        public NetcoreServiceClient() {
        }
        
        public NetcoreServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public NetcoreServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NetcoreServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NetcoreServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SchedulerEarlyClaim.sms.ServiceResponse SchedulerEarlyClaim.sms.INetcoreService.NetcoreMessageSender(SchedulerEarlyClaim.sms.ServiceRequest request) {
            return base.Channel.NetcoreMessageSender(request);
        }
        
        public string NetcoreMessageSender(string ApplicationNo, string DataKey, string DataValue, string MessageText, string MobileNumber, string PolicyNo, string SMS_Resource, out string ErrorDescr, out string RequestID) {
            SchedulerEarlyClaim.sms.ServiceRequest inValue = new SchedulerEarlyClaim.sms.ServiceRequest();
            inValue.ApplicationNo = ApplicationNo;
            inValue.DataKey = DataKey;
            inValue.DataValue = DataValue;
            inValue.MessageText = MessageText;
            inValue.MobileNumber = MobileNumber;
            inValue.PolicyNo = PolicyNo;
            inValue.SMS_Resource = SMS_Resource;
            SchedulerEarlyClaim.sms.ServiceResponse retVal = ((SchedulerEarlyClaim.sms.INetcoreService)(this)).NetcoreMessageSender(inValue);
            ErrorDescr = retVal.ErrorDescr;
            RequestID = retVal.RequestID;
            return retVal.ErrorCode;
        }
        
        public System.Threading.Tasks.Task<SchedulerEarlyClaim.sms.ServiceResponse> NetcoreMessageSenderAsync(SchedulerEarlyClaim.sms.ServiceRequest request) {
            return base.Channel.NetcoreMessageSenderAsync(request);
        }
    }
}
