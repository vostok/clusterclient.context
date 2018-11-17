using Vostok.Clusterclient.Core.Modules;

namespace Vostok.ClusterClient.Context
{
    internal static class RequestContextExtensions
    {
        public static void SetHeader(this IRequestContext context, string name, string value)
            => context.Request = context.Request.WithHeader(name, value);
    }
}