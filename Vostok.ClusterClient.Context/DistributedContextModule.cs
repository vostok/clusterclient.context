using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Modules;
using Vostok.Context;

namespace Vostok.Clusterclient.Context
{
    /// <summary>
    /// <para>A module that does following:</para>
    /// <list type="bullet">
    ///     <item><description>Sets the <see cref="RequestPriority"/> in request parameters if none was specified by user and there's currently a non-null nullable <see cref="RequestPriority"/> value in ambient context <see cref="FlowingContext.Globals"/>.</description></item>
    /// </list>
    /// </summary>
    [PublicAPI]
    public class DistributedContextModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            SetRequestPriority(context);

            return next(context);
        }

        private static void SetRequestPriority(IRequestContext context)
        {
            var priority = FlowingContext.Globals.Get<RequestPriority?>();
            if (priority.HasValue && context.Parameters.Priority == null)
                context.Parameters = context.Parameters.WithPriority(priority);
        }
    }
}
