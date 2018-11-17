using JetBrains.Annotations;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Misc;
using Vostok.Clusterclient.Core.Modules;
using Vostok.Context;

namespace Vostok.ClusterClient.Context
{
    /// <summary>
    /// An extensions for distributed context integration.
    /// </summary>
    [PublicAPI]
    public static class ClusterClientConfigurationContextExtensions
    {
        /// <summary>
        /// Sets up an integration with vostok.context module which transfer <see cref="FlowingContext"/> data between client and server.
        /// </summary>
        public static IClusterClientConfiguration SetupDistributedContext(this IClusterClientConfiguration configuration)
        {
            configuration.AddRequestModule(new DistributedContextModule(), RequestModule.AuxiliaryHeaders, ModulePosition.Before);
            return configuration;
        }
    }
}