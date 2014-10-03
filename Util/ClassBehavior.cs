using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Traderdata.Client.TerminalWEB.Util
{
    public class ClassBehaviour : IEndpointBehavior
    {
        // Summary:
        //     Implement to pass data at runtime to bindings to support custom behavior.
        //
        // Parameters:
        //   endpoint:
        //     The endpoint to modify.
        //
        //   bindingParameters:
        //     The objects that binding elements require to support the behavior.
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }
        //
        // Summary:
        //     Implements a modification or extension of the client across an endpoint.
        //
        // Parameters:
        //   endpoint:
        //     The endpoint that is to be customized.
        //
        //   clientRuntime:
        //     The client runtime to be customized.
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new ClassMessageInspector());
        }
        //
        // Summary:
        //     Implements a modification or extension of the service across an endpoint.
        //
        // Parameters:
        //   endpoint:
        //     The endpoint that exposes the contract.
        //
        //   endpointDispatcher:
        //     The endpoint dispatcher to be modified or extended.
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }
        //
        // Summary:
        //     Implement to confirm that the endpoint meets some intended criteria.
        //
        // Parameters:
        //   endpoint:
        //     The endpoint to validate.
        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}


