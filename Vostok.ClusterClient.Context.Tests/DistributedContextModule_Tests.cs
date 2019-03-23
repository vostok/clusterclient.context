using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Modules;
using Vostok.Context;

// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Clusterclient.Context.Tests
{
    [TestFixture]
    internal class DistributedContextModule_Tests
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
    }
}
