using JetBrains.Annotations;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Modules;

namespace Vostok.Clusterclient.Context
{
    [PublicAPI]
    public static class IClusterClientConfigurationExtensions
    {
        /// <summary>
        /// Adds a <see cref="DistributedContextModule"/> to request modules pipeline and wraps transport with <see cref="DistributedContextTransport"/>.
        /// </summary>
        public static void SetupDistributedContext([NotNull] this IClusterClientConfiguration configuration)
        {
            configuration.AddRequestModule(new DistributedContextModule(), RequestModule.AuxiliaryHeaders);

            if (!(configuration.Transport is DistributedContextTransport))
                configuration.Transport = new DistributedContextTransport(configuration.Transport);
        }
    }
}
