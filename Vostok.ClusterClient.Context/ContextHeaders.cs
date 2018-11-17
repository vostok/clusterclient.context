namespace Vostok.ClusterClient.Context
{
    internal static class ContextHeaders
    {
        //TODO: choose better names, then expose it into public in common place
        public static readonly string Globals = "Vostok-Context-Globals";
        public static readonly string Properties = "Vostok-Context-Properties";
    }
}