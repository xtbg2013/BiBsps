﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BiBspsUnitTest.MesServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MesServiceReference.IMesService")]
    public interface IMesService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/GetWorkStep", ReplyAction="http://tempuri.org/IMesService/GetWorkStepResponse")]
        BiBspsUnitTest.MesServiceReference.GetWorkStepResponse GetWorkStep(BiBspsUnitTest.MesServiceReference.GetWorkStepRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/GetWorkStep", ReplyAction="http://tempuri.org/IMesService/GetWorkStepResponse")]
        System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.GetWorkStepResponse> GetWorkStepAsync(BiBspsUnitTest.MesServiceReference.GetWorkStepRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/MoveStandard", ReplyAction="http://tempuri.org/IMesService/MoveStandardResponse")]
        BiBspsUnitTest.MesServiceReference.MoveStandardResponse MoveStandard(BiBspsUnitTest.MesServiceReference.MoveStandardRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/MoveStandard", ReplyAction="http://tempuri.org/IMesService/MoveStandardResponse")]
        System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.MoveStandardResponse> MoveStandardAsync(BiBspsUnitTest.MesServiceReference.MoveStandardRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/Hold", ReplyAction="http://tempuri.org/IMesService/HoldResponse")]
        BiBspsUnitTest.MesServiceReference.HoldResponse Hold(BiBspsUnitTest.MesServiceReference.HoldRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/Hold", ReplyAction="http://tempuri.org/IMesService/HoldResponse")]
        System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.HoldResponse> HoldAsync(BiBspsUnitTest.MesServiceReference.HoldRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/GetStepState", ReplyAction="http://tempuri.org/IMesService/GetStepStateResponse")]
        BiBspsUnitTest.MesServiceReference.GetStepStateResponse GetStepState(BiBspsUnitTest.MesServiceReference.GetStepStateRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/GetStepState", ReplyAction="http://tempuri.org/IMesService/GetStepStateResponse")]
        System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.GetStepStateResponse> GetStepStateAsync(BiBspsUnitTest.MesServiceReference.GetStepStateRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/GetCocInfoBySn", ReplyAction="http://tempuri.org/IMesService/GetCocInfoBySnResponse")]
        BiBspsUnitTest.MesServiceReference.GetCocInfoBySnResponse GetCocInfoBySn(BiBspsUnitTest.MesServiceReference.GetCocInfoBySnRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMesService/GetCocInfoBySn", ReplyAction="http://tempuri.org/IMesService/GetCocInfoBySnResponse")]
        System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.GetCocInfoBySnResponse> GetCocInfoBySnAsync(BiBspsUnitTest.MesServiceReference.GetCocInfoBySnRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetWorkStep", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetWorkStepRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string sn;
        
        public GetWorkStepRequest() {
        }
        
        public GetWorkStepRequest(string sn) {
            this.sn = sn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetWorkStepResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetWorkStepResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool GetWorkStepResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string workStep;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string errorMessage;
        
        public GetWorkStepResponse() {
        }
        
        public GetWorkStepResponse(bool GetWorkStepResult, string workStep, string errorMessage) {
            this.GetWorkStepResult = GetWorkStepResult;
            this.workStep = workStep;
            this.errorMessage = errorMessage;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="MoveStandard", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class MoveStandardRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string sn;
        
        public MoveStandardRequest() {
        }
        
        public MoveStandardRequest(string sn) {
            this.sn = sn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="MoveStandardResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class MoveStandardResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool MoveStandardResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string errorMessage;
        
        public MoveStandardResponse() {
        }
        
        public MoveStandardResponse(bool MoveStandardResult, string errorMessage) {
            this.MoveStandardResult = MoveStandardResult;
            this.errorMessage = errorMessage;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Hold", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class HoldRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string sn;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string reason;
        
        public HoldRequest() {
        }
        
        public HoldRequest(string sn, string reason) {
            this.sn = sn;
            this.reason = reason;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="HoldResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class HoldResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool HoldResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string errorMessage;
        
        public HoldResponse() {
        }
        
        public HoldResponse(bool HoldResult, string errorMessage) {
            this.HoldResult = HoldResult;
            this.errorMessage = errorMessage;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetStepState", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetStepStateRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string sn;
        
        public GetStepStateRequest() {
        }
        
        public GetStepStateRequest(string sn) {
            this.sn = sn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetStepStateResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetStepStateResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool GetStepStateResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string StepState;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string errorMessage;
        
        public GetStepStateResponse() {
        }
        
        public GetStepStateResponse(bool GetStepStateResult, string StepState, string errorMessage) {
            this.GetStepStateResult = GetStepStateResult;
            this.StepState = StepState;
            this.errorMessage = errorMessage;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetCocInfoBySn", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetCocInfoBySnRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string sn;
        
        public GetCocInfoBySnRequest() {
        }
        
        public GetCocInfoBySnRequest(string sn) {
            this.sn = sn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetCocInfoBySnResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetCocInfoBySnResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool GetCocInfoBySnResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string[] info;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string errorMessage;
        
        public GetCocInfoBySnResponse() {
        }
        
        public GetCocInfoBySnResponse(bool GetCocInfoBySnResult, string[] info, string errorMessage) {
            this.GetCocInfoBySnResult = GetCocInfoBySnResult;
            this.info = info;
            this.errorMessage = errorMessage;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMesServiceChannel : BiBspsUnitTest.MesServiceReference.IMesService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MesServiceClient : System.ServiceModel.ClientBase<BiBspsUnitTest.MesServiceReference.IMesService>, BiBspsUnitTest.MesServiceReference.IMesService {
        
        public MesServiceClient() {
        }
        
        public MesServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MesServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MesServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MesServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BiBspsUnitTest.MesServiceReference.GetWorkStepResponse BiBspsUnitTest.MesServiceReference.IMesService.GetWorkStep(BiBspsUnitTest.MesServiceReference.GetWorkStepRequest request) {
            return base.Channel.GetWorkStep(request);
        }
        
        public bool GetWorkStep(string sn, out string workStep, out string errorMessage) {
            BiBspsUnitTest.MesServiceReference.GetWorkStepRequest inValue = new BiBspsUnitTest.MesServiceReference.GetWorkStepRequest();
            inValue.sn = sn;
            BiBspsUnitTest.MesServiceReference.GetWorkStepResponse retVal = ((BiBspsUnitTest.MesServiceReference.IMesService)(this)).GetWorkStep(inValue);
            workStep = retVal.workStep;
            errorMessage = retVal.errorMessage;
            return retVal.GetWorkStepResult;
        }
        
        public System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.GetWorkStepResponse> GetWorkStepAsync(BiBspsUnitTest.MesServiceReference.GetWorkStepRequest request) {
            return base.Channel.GetWorkStepAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BiBspsUnitTest.MesServiceReference.MoveStandardResponse BiBspsUnitTest.MesServiceReference.IMesService.MoveStandard(BiBspsUnitTest.MesServiceReference.MoveStandardRequest request) {
            return base.Channel.MoveStandard(request);
        }
        
        public bool MoveStandard(string sn, out string errorMessage) {
            BiBspsUnitTest.MesServiceReference.MoveStandardRequest inValue = new BiBspsUnitTest.MesServiceReference.MoveStandardRequest();
            inValue.sn = sn;
            BiBspsUnitTest.MesServiceReference.MoveStandardResponse retVal = ((BiBspsUnitTest.MesServiceReference.IMesService)(this)).MoveStandard(inValue);
            errorMessage = retVal.errorMessage;
            return retVal.MoveStandardResult;
        }
        
        public System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.MoveStandardResponse> MoveStandardAsync(BiBspsUnitTest.MesServiceReference.MoveStandardRequest request) {
            return base.Channel.MoveStandardAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BiBspsUnitTest.MesServiceReference.HoldResponse BiBspsUnitTest.MesServiceReference.IMesService.Hold(BiBspsUnitTest.MesServiceReference.HoldRequest request) {
            return base.Channel.Hold(request);
        }
        
        public bool Hold(string sn, string reason, out string errorMessage) {
            BiBspsUnitTest.MesServiceReference.HoldRequest inValue = new BiBspsUnitTest.MesServiceReference.HoldRequest();
            inValue.sn = sn;
            inValue.reason = reason;
            BiBspsUnitTest.MesServiceReference.HoldResponse retVal = ((BiBspsUnitTest.MesServiceReference.IMesService)(this)).Hold(inValue);
            errorMessage = retVal.errorMessage;
            return retVal.HoldResult;
        }
        
        public System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.HoldResponse> HoldAsync(BiBspsUnitTest.MesServiceReference.HoldRequest request) {
            return base.Channel.HoldAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BiBspsUnitTest.MesServiceReference.GetStepStateResponse BiBspsUnitTest.MesServiceReference.IMesService.GetStepState(BiBspsUnitTest.MesServiceReference.GetStepStateRequest request) {
            return base.Channel.GetStepState(request);
        }
        
        public bool GetStepState(string sn, out string StepState, out string errorMessage) {
            BiBspsUnitTest.MesServiceReference.GetStepStateRequest inValue = new BiBspsUnitTest.MesServiceReference.GetStepStateRequest();
            inValue.sn = sn;
            BiBspsUnitTest.MesServiceReference.GetStepStateResponse retVal = ((BiBspsUnitTest.MesServiceReference.IMesService)(this)).GetStepState(inValue);
            StepState = retVal.StepState;
            errorMessage = retVal.errorMessage;
            return retVal.GetStepStateResult;
        }
        
        public System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.GetStepStateResponse> GetStepStateAsync(BiBspsUnitTest.MesServiceReference.GetStepStateRequest request) {
            return base.Channel.GetStepStateAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BiBspsUnitTest.MesServiceReference.GetCocInfoBySnResponse BiBspsUnitTest.MesServiceReference.IMesService.GetCocInfoBySn(BiBspsUnitTest.MesServiceReference.GetCocInfoBySnRequest request) {
            return base.Channel.GetCocInfoBySn(request);
        }
        
        public bool GetCocInfoBySn(string sn, out string[] info, out string errorMessage) {
            BiBspsUnitTest.MesServiceReference.GetCocInfoBySnRequest inValue = new BiBspsUnitTest.MesServiceReference.GetCocInfoBySnRequest();
            inValue.sn = sn;
            BiBspsUnitTest.MesServiceReference.GetCocInfoBySnResponse retVal = ((BiBspsUnitTest.MesServiceReference.IMesService)(this)).GetCocInfoBySn(inValue);
            info = retVal.info;
            errorMessage = retVal.errorMessage;
            return retVal.GetCocInfoBySnResult;
        }
        
        public System.Threading.Tasks.Task<BiBspsUnitTest.MesServiceReference.GetCocInfoBySnResponse> GetCocInfoBySnAsync(BiBspsUnitTest.MesServiceReference.GetCocInfoBySnRequest request) {
            return base.Channel.GetCocInfoBySnAsync(request);
        }
    }
}
