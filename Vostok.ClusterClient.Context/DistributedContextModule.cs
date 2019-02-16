using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Modules;
using Vostok.Context;

namespace Vostok.Clusterclient.Context
{
    /// <summary>
    /// <para>A module that does three things:</para>
    /// <list type="bullet">
    ///     <item><description>Adds a <see cref="HeaderNames.VostokContextGlobals"/> header to request if there are any distributed <see cref="FlowingContext.Globals"/> in ambient context.</description></item>
    ///     <item><description>Adds a <see cref="HeaderNames.VostokContextProperties"/> header to request if there are any distributed <see cref="FlowingContext.Properties"/> in ambient context.</description></item>
    ///     <item><description>Sets the <see cref="RequestPriority"/> in request parameters if none was specified by user and there's currently a non-null nullable <see cref="RequestPriority"/> value in ambient context <see cref="FlowingContext.Globals"/>.</description></item>
    /// </list>
    /// </summary>
    [PublicAPI]
    public class DistributedContextModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            SerializeDistributedContext(context);

            SetRequestPriority(context);

            return next(context);
        }

        private static void SerializeDistributedContext(IRequestContext context)
        {
            var globals = FlowingContext.SerializeDistributedGlobals();
            var properties = FlowingContext.SerializeDistributedProperties();

            if (globals != null)
                context.Request = context.Request.WithHeader(HeaderNames.VostokContextGlobals, globals);

            if (properties != null)
                context.Request = context.Request.WithHeader(HeaderNames.VostokContextProperties, properties);
        }

        private static void SetRequestPriority(IRequestContext context)
        {
            var priority = FlowingContext.Globals.Get<RequestPriority?>();
            if (priority.HasValue && context.Parameters.Priority == null)
                context.Parameters = context.Parameters.WithPriority(priority);
        }
    }
}
