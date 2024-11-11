using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace StonkNotes.Application.UnitTests
{
    [SetUpFixture]
    public class ApplicationLayerServiceProviderFixture
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddApplicationServices();
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }
}
