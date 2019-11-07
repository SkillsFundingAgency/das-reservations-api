﻿using System;
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
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    public class TestServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TestServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = GenerateConfiguration();
            
            var startup = new Startup(configuration);

            startup.ConfigureServices(serviceCollection);

            RegisterControllers(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
       
        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new[]
                {
                    new KeyValuePair<string, string>("ConfigurationStorageConnectionString", "UseDevelopmentStorage=true;"),
                    new KeyValuePair<string, string>("ConfigNames", "SFA.DAS.Reservations.Api"),
                    new KeyValuePair<string, string>("Environment", "DEV"),
                    new KeyValuePair<string, string>("Version", "1.0"),
                    new KeyValuePair<string, string>("Reservations:ElasticSearchServerUrl", "http://localhost:9200"),
                }
            };
            
            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }

        private static void RegisterControllers(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(sp => 
                new ReservationsController(
                    sp.GetService<ILogger<ReservationsController>>(),
                    sp.GetService<IMediator>())
                {
                    ControllerContext = GetControllerContext<ReservationsController>()
                });
            serviceCollection.AddTransient(sp =>
                new RulesController(sp.GetService<IMediator>())
                {
                    ControllerContext = GetControllerContext<RulesController>()
                });

            serviceCollection.AddTransient(sp =>
                new CoursesController(sp.GetService<IMediator>())
                {
                    ControllerContext = GetControllerContext<CoursesController>()
                });

            serviceCollection.AddTransient(sp => 
                new AccountLegalEntitiesController(
                    sp.GetService<IMediator>(),
                    sp.GetService<ILogger<AccountLegalEntitiesController>>())
            {
                ControllerContext = GetControllerContext<AccountLegalEntitiesController>()
            });
        }

        private static ControllerContext GetControllerContext<T>() where T : ControllerBase
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
    }
}
