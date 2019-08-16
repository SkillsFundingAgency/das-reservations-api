using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Api.Controllers;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    public class TestContainer
    {
        private static TestContainer _instanceOf;

        public static TestContainer GetInstance()
        {
            return _instanceOf ?? (_instanceOf = new TestContainer());
        }

        private ServiceProvider _serviceProvider;

        public T Get<T>()
        {
            if (_serviceProvider == null)
            {
                Initialise();
            }

            var repo = _serviceProvider.GetService(typeof(ReservationsController));

            return _serviceProvider.GetService<T>();
        }

        private void Initialise()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = GenerateConfiguration();

            var startup = new Startup(configuration);

            startup.ConfigureServices(serviceCollection);

            RegisterControllers(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void RegisterControllers(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(sp => 
                new ReservationsController(
                sp.GetService<ILogger<ReservationsController>>(),
                sp.GetService<IMediator>())
            {
                ControllerContext = GetContext<ReservationsController>()
            });
        }

        private static ControllerContext GetContext<T>() where T : ControllerBase
        {
            var controllerName = typeof(T).Name.Replace("Controller", "");

            var descriptor = new ControllerActionDescriptor 
            {
                ControllerName = controllerName, 
                ControllerTypeInfo = typeof(T).GetTypeInfo()
            };

            var httpContext = new DefaultHttpContext();
            var context = new ControllerContext(new ActionContext(httpContext, new RouteData(), descriptor));

            return context;
        }

        private IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new[]
                {
                    new KeyValuePair<string, string>("ConfigurationStorageConnectionString", "UseDevelopmentStorage=true;"),
                    new KeyValuePair<string, string>("ConfigNames", "SFA.DAS.Reservations.Api"),
                    new KeyValuePair<string, string>("Environment", "DEV"),
                    new KeyValuePair<string, string>("Version", "1.0"),
                }
            };
            
            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
