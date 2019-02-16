using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Modules;
using Vostok.Context;

// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Clusterclient.Context.Tests
{
    public class DistributedContextModule_Tests
    {
        private DistributedContextModule module;
        private IRequestContext context;
        private Request request;
        private RequestParameters parameters;

        [SetUp]
        public void Setup()
        {
            context = Substitute.For<IRequestContext>();
            request = Request.Get("http://a/");
            parameters = RequestParameters.Empty;
            context.Request = request;
            context.Parameters = parameters;

            module = new DistributedContextModule();
        }

        [Test]
        public void Should_serialize_distributed_globals()
        {
            FlowingContext.Configuration.RegisterDistributedGlobal("a", ContextSerializers.Int);

            SetGlobals(123, 456);

            module.ExecuteAsync(
                    context,
                    requestContext =>
                    {
                        ResetGlobals();
                        var globals = requestContext.Request.Headers?[HeaderNames.VostokContextGlobals];

                        globals.Should().NotBeNull();
                        FlowingContext.RestoreDistributedGlobals(globals);
                        GetGlobals().Should().Be((123, 0));
                        Assert.Pass();
                        return null;
                    })
                .GetAwaiter()
                .GetResult();
        }

        [Test]
        public void Should_serialize_distributed_properties()
        {
            FlowingContext.Configuration.RegisterDistributedProperty("a", ContextSerializers.Int);

            SetProperties(234, 567);

            module.ExecuteAsync(
                    context,
                    requestContext =>
                    {
                        ResetProperties();
                        var properties = requestContext.Request.Headers?[HeaderNames.VostokContextProperties];

                        properties.Should().NotBeNull();
                        FlowingContext.RestoreDistributedProperties(properties);
                        GetProperties().Should().Be((234, 0));
                        Assert.Pass();
                        return null;
                    })
                .GetAwaiter()
                .GetResult();
        }

        [Test]
        public void Should_do_nothing_when_globals_and_properties_are_empty()
        {
            FlowingContext.Configuration.ClearDistributedGlobals();
            FlowingContext.Configuration.ClearDistributedProperties();

            module.ExecuteAsync(
                    context,
                    requestContext =>
                    {
                        ResetProperties();
                        var globals = requestContext.Request.Headers?[HeaderNames.VostokContextGlobals];
                        var properties = requestContext.Request.Headers?[HeaderNames.VostokContextProperties];

                        globals.Should().BeNull();
                        properties.Should().BeNull();

                        Assert.Pass();
                        return null;
                    })
                .GetAwaiter()
                .GetResult();
        }

        [TestCase(RequestPriority.Critical)]
        [TestCase(RequestPriority.Ordinary)]
        [TestCase(RequestPriority.Sheddable)]
        public void Should_set_request_priority(RequestPriority priority)
        {
            FlowingContext.Globals.Set<RequestPriority?>(priority);

            module.ExecuteAsync(
                    context,
                    requestContext =>
                    {
                        requestContext.Parameters.Priority.Should().Be(priority);
                        Assert.Pass();
                        return null;
                    })
                .GetAwaiter()
                .GetResult();
        }

        private static void SetGlobals(int int32, long int64)
        {
            FlowingContext.Globals.Set(int32);
            FlowingContext.Globals.Set(int64);
        }

        private static void ResetGlobals()
            => SetGlobals(default, default);

        private static (int, long) GetGlobals()
        {
            var int32 = FlowingContext.Globals.Get<int>();
            var int64 = FlowingContext.Globals.Get<long>();

            return (int32, int64);
        }

        private static void SetProperties(int int32, long int64)
        {
            FlowingContext.Properties.Set("a", int32);
            FlowingContext.Properties.Set("b", int64);
        }

        private static void ResetProperties()
            => SetProperties(default, default);

        private static (int, long) GetProperties()
        {
            var int32 = FlowingContext.Properties.Get<int>("a");
            var int64 = FlowingContext.Properties.Get<long>("b");

            return (int32, int64);
        }
    }
}
