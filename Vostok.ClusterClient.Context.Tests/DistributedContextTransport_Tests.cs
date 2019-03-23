using System;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Transport;
using Vostok.Context;

// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Clusterclient.Context.Tests
{
    [TestFixture]
    internal class DistributedContextTransport_Tests
    {
        private ITransport baseTransport;
        private DistributedContextTransport transport;
        private Action<Request> assertion;

        [SetUp]
        public void TestSetup()
        {
            assertion = null;

            baseTransport = Substitute.For<ITransport>();
            baseTransport.SendAsync(default, default, default, default).ReturnsForAnyArgs(Responses.Ok);

            baseTransport
                .WhenForAnyArgs(t => t.SendAsync(default, default, default, default))
                .Do(info => assertion(info.Arg<Request>()));

            transport = new DistributedContextTransport(baseTransport);
        }

        [Test]
        public void Should_serialize_distributed_globals()
        {
            FlowingContext.Configuration.RegisterDistributedGlobal("a", ContextSerializers.Int);

            SetGlobals(123, 456);

            assertion = request =>
            {
                ResetGlobals();
                var globals = request.Headers?[HeaderNames.ContextGlobals];

                globals.Should().NotBeNull();
                FlowingContext.RestoreDistributedGlobals(globals);
                GetGlobals().Should().Be((123, 0));
                Assert.Pass();
            };

            transport
                .SendAsync(Request.Get("foo/bar"), null, 1.Seconds(), CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        [Test]
        public void Should_serialize_distributed_properties()
        {
            FlowingContext.Configuration.RegisterDistributedProperty("a", ContextSerializers.Int);

            SetProperties(234, 567);

            assertion = request =>
            {
                ResetProperties();
                var properties = request.Headers?[HeaderNames.ContextProperties];

                properties.Should().NotBeNull();
                FlowingContext.RestoreDistributedProperties(properties);
                GetProperties().Should().Be((234, 0));
                Assert.Pass();
                Assert.Pass();
            };

            transport
                .SendAsync(Request.Get("foo/bar"), null, 1.Seconds(), CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        [Test]
        public void Should_do_nothing_when_globals_and_properties_are_empty()
        {
            FlowingContext.Configuration.ClearDistributedGlobals();
            FlowingContext.Configuration.ClearDistributedProperties();

            assertion = request =>
            {
                ResetProperties();
                var globals = request.Headers?[HeaderNames.ContextGlobals];
                var properties = request.Headers?[HeaderNames.ContextProperties];

                globals.Should().BeNull();
                properties.Should().BeNull();

                Assert.Pass();
            };
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