﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.AspNet.SignalR.Client {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.AspNet.SignalR.Client.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Detected a connection attempt to an ASP.NET Core SignalR Server. This client only supports connecting to an ASP.NET SignalR Server. See https://aka.ms/signalr-core-differences for details..
        /// </summary>
        internal static string Error_AspNetCoreServerDetected {
            get {
                return ResourceManager.GetString("Error_AspNetCoreServerDetected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Client Certificates cannot be added after the connection has started..
        /// </summary>
        internal static string Error_CertsCanOnlyBeAddedWhenDisconnected {
            get {
                return ResourceManager.GetString("Error_CertsCanOnlyBeAddedWhenDisconnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A client callback for event {0} with {1} argument(s) was found, however an error occurred because {2}.
        /// </summary>
        internal static string Error_ClientCallbackArgumentTypeMismatch {
            get {
                return ResourceManager.GetString("Error_ClientCallbackArgumentTypeMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A client callback for event {0} with {1} argument(s) could not be found.
        /// </summary>
        internal static string Error_ClientCallbackInvalidNumberOfArguments {
            get {
                return ResourceManager.GetString("Error_ClientCallbackInvalidNumberOfArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection was stopped before it could be started..
        /// </summary>
        internal static string Error_ConnectionCancelled {
            get {
                return ResourceManager.GetString("Error_ConnectionCancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection has not been established..
        /// </summary>
        internal static string Error_ConnectionHasNotBeenEstablished {
            get {
                return ResourceManager.GetString("Error_ConnectionHasNotBeenEstablished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data cannot be sent because the connection is in the disconnected state. Call start before sending any data..
        /// </summary>
        internal static string Error_DataCannotBeSentConnectionDisconnected {
            get {
                return ResourceManager.GetString("Error_DataCannotBeSentConnectionDisconnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data cannot be sent because the WebSocket connection is reconnecting..
        /// </summary>
        internal static string Error_DataCannotBeSentDuringWebSocketReconnect {
            get {
                return ResourceManager.GetString("Error_DataCannotBeSentDuringWebSocketReconnect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error message received from the server: &apos;{0}&apos;..
        /// </summary>
        internal static string Error_ErrorFromServer {
            get {
                return ResourceManager.GetString("Error_ErrorFromServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are using a version of the client that isn&apos;t compatible with the server. Client version {0}, server version {1}..
        /// </summary>
        internal static string Error_IncompatibleProtocolVersion {
            get {
                return ResourceManager.GetString("Error_IncompatibleProtocolVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uri scheme &apos;{0}&apos; is not valid. The only valid uri schemes are &apos;http&apos; and &apos;https&apos;..
        /// </summary>
        internal static string Error_InvalidUriScheme {
            get {
                return ResourceManager.GetString("Error_InvalidUriScheme", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Negotiate redirection limit exceeded..
        /// </summary>
        internal static string Error_NegotiationLimitExceeded {
            get {
                return ResourceManager.GetString("Error_NegotiationLimitExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No client transport is compatible with the server. This either means that the AutoTransport was configured to use only WebSockets which is not compatible with the server or that the AutoTransport was configured with no sub-transports at all..
        /// </summary>
        internal static string Error_NoCompatibleTransportFound {
            get {
                return ResourceManager.GetString("Error_NoCompatibleTransportFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Possible deadlock detected. A callback registered with &quot;HubProxy.On&quot; or &quot;Connection.Received&quot; has been executing for at least {0} seconds..
        /// </summary>
        internal static string Error_PossibleDeadlockDetected {
            get {
                return ResourceManager.GetString("Error_PossibleDeadlockDetected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The ProcessResponse method cannot be called before the transport is started..
        /// </summary>
        internal static string Error_ProcessResponseBeforeStart {
            get {
                return ResourceManager.GetString("Error_ProcessResponseBeforeStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A HubProxy cannot be added after the connection has been started..
        /// </summary>
        internal static string Error_ProxiesCannotBeAddedConnectionStarted {
            get {
                return ResourceManager.GetString("Error_ProxiesCannotBeAddedConnectionStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t reconnect within the configured timeout of {0}, disconnecting..
        /// </summary>
        internal static string Error_ReconnectTimeout {
            get {
                return ResourceManager.GetString("Error_ReconnectTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The client has been inactive since {0} and it has exceeded the inactivity timeout of {1}. Stopping the connection..
        /// </summary>
        internal static string Error_ReconnectWindowTimeout {
            get {
                return ResourceManager.GetString("Error_ReconnectWindowTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server negotiation failed..
        /// </summary>
        internal static string Error_ServerNegotiationFailed {
            get {
                return ResourceManager.GetString("Error_ServerNegotiationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error during start request. Stopping the connection..
        /// </summary>
        internal static string Error_StartFailed {
            get {
                return ResourceManager.GetString("Error_StartFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Request failed - task cancelled..
        /// </summary>
        internal static string Error_TaskCancelledException {
            get {
                return ResourceManager.GetString("Error_TaskCancelledException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The transport instance passed to the Negotiate method has already been used. Use a new transport instance each time you start a new connection..
        /// </summary>
        internal static string Error_TransportCannotBeReused {
            get {
                return ResourceManager.GetString("Error_TransportCannotBeReused", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The transport disconnected before the connection could be fully initialized..
        /// </summary>
        internal static string Error_TransportDisconnectedBeforeConnectionFullyInitialized {
            get {
                return ResourceManager.GetString("Error_TransportDisconnectedBeforeConnectionFullyInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transport failed trying to connect..
        /// </summary>
        internal static string Error_TransportFailedToConnect {
            get {
                return ResourceManager.GetString("Error_TransportFailedToConnect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transport timed out trying to connect.
        /// </summary>
        internal static string Error_TransportTimedOutTryingToConnect {
            get {
                return ResourceManager.GetString("Error_TransportTimedOutTryingToConnect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Url cannot contain query string directly. Pass query string values in using available overload..
        /// </summary>
        internal static string Error_UrlCantContainQueryStringDirectly {
            get {
                return ResourceManager.GetString("Error_UrlCantContainQueryStringDirectly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection was disconnected before invocation result was received..
        /// </summary>
        internal static string Message_ConnectionClosed {
            get {
                return ResourceManager.GetString("Message_ConnectionClosed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection started reconnecting before invocation result was received..
        /// </summary>
        internal static string Message_Reconnecting {
            get {
                return ResourceManager.GetString("Message_Reconnecting", resourceCulture);
            }
        }
    }
}