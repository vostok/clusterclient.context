using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Modules;
using Vostok.Context;

namespace Vostok.ClusterClient.Context
{
    internal class DistributedContextModule : IRequestModule
    {
        private readonly bool useContextualRequestPriority;

        public DistributedContextModule(bool useContextualRequestPriority)
        {
            this.useContextualRequestPriority = useContextualRequestPriority;
        }

        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            SerializeDistributedContext(context);

            if (useContextualRequestPriority)
                SetRequestPriority(context);
            
            return next(context);
        }

        private static void SetRequestPriority(IRequestContext context)
        {
            var priority = FlowingContext.Globals.Get<RequestPriority?>();
            if (priority.HasValue && context.Parameters.Priority == null)
                context.Parameters = context.Parameters.WithPriority(priority);
        }

        private static void SerializeDistributedContext(IRequestContext context)
        {
            var globals = FlowingContext.SerializeDistributedGlobals();
            var properties = FlowingContext.SerializeDistributedProperties();

            if (globals != null)
                context.SetHeader(ContextHeaders.Globals, globals);

            if (properties != null)
                context.SetHeader(ContextHeaders.Properties, globals);
        }
    }
}