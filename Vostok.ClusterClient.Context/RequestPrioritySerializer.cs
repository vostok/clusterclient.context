using JetBrains.Annotations;
using Vostok.Clusterclient.Core.Model;
using Vostok.Context;

namespace Vostok.Clusterclient.Context
{
    [PublicAPI]
    public class RequestPrioritySerializer : IContextSerializer<RequestPriority?>
    {
        private readonly IContextSerializer<RequestPriority> serializer = ContextSerializers.Enum<RequestPriority>();

        public string Serialize(RequestPriority? value) =>
            value == null ? string.Empty : serializer.Serialize(value.Value);

        public RequestPriority? Deserialize(string input) =>
            string.IsNullOrEmpty(input) ? (RequestPriority?)null : serializer.Deserialize(input);
    }
}
