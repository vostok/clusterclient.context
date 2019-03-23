using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Transport;
using Vostok.Context;

namespace Vostok.Clusterclient.Context
{
    /// <summary>
    /// <para>A transport decorator that does following:</para>
    /// <list type="bullet">
    ///     <item><description>Adds a <see cref="HeaderNames.ContextGlobals"/> header to request if there are any distributed <see cref="FlowingContext.Globals"/> in ambient context.</description></item>
    ///     <item><description>Adds a <see cref="HeaderNames.ContextProperties"/> header to request if there are any distributed <see cref="FlowingContext.Properties"/> in ambient context.</description></item>
    /// </list>
    /// </summary>
    [PublicAPI]
    public class DistributedContextTransport : ITransport
    {
        private readonly ITransport transport;

        public DistributedContextTransport([NotNull] ITransport transport)
        {
            this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public TransportCapabilities Capabilities => transport.Capabilities;

        public Task<Response> SendAsync(Request request, TimeSpan? connectionTimeout, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (request.Headers?[HeaderNames.ContextGlobals] == null)
            {
                var globals = FlowingContext.SerializeDistributedGlobals();
                if (globals != null)
                    request = request.WithHeader(HeaderNames.ContextGlobals, globals);
            }

            if (request.Headers?[HeaderNames.ContextProperties] == null)
            {
                var properties = FlowingContext.SerializeDistributedProperties();
                if (properties != null)
                    request = request.WithHeader(HeaderNames.ContextProperties, properties);
            }

            return transport.SendAsync(request, connectionTimeout, timeout, cancellationToken);
        }
    }
}