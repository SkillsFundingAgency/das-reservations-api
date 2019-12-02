using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Api.StartupConfig;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Reservations.Api.AppStart;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Api.StartupExtensions;
using SFA.DAS.Reservations.Infrastructure.DevConfiguration;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.Mvc.Extensions;

namespace SFA.DAS.Reservations.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.development.json",true)
                .AddEnvironmentVariables()
                .AddAzureTableStorageConfiguration(
                    configuration["ConfigurationStorageConnectionString"],
                    configuration["ConfigNames"],
                    configuration["Environment"],
                    configuration["Version"]
                )
                .Build();
            
            Configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ReservationsConfiguration>(Configuration.GetSection("Reservations"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsConfiguration>>().Value);
            services.Configure<AzureActiveDirectoryConfiguration>(Configuration.GetSection("AzureAd"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value);
            
            var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetService<IOptions<ReservationsConfiguration>>();

            services.AddElasticSearch(config.Value);
            services.AddSingleton(new ReservationsApiEnvironment(Configuration["Environment"]));
            
            services.AddHealthChecks()
                    .AddSqlServer(config.Value.ConnectionString)
                    .AddCheck<QueueHealthCheck>(
                        "ServiceBus Queue Health",
                        HealthStatus.Unhealthy,
                        new []{"ready"})
                    .AddCheck<ElasticSearchHealthCheck>(
                        "Elastic Search Health",
                        HealthStatus.Unhealthy,
                        new []{"ready"});

            if (!ConfigurationIsLocalOrDev())
            {
                var azureActiveDirectoryConfiguration =
                    serviceProvider.GetService<IOptions<AzureActiveDirectoryConfiguration>>();
                services.AddAuthorization(o =>
                {
                    o.AddPolicy("default", policy => { policy.RequireAuthenticatedUser(); });
                });
                services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                    .AddJwtBearer(auth =>
                    {
                        auth.Authority =
                            $"https://login.microsoftonline.com/{azureActiveDirectoryConfiguration.Value.Tenant}";
                        auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidAudiences = new List<string>
                            {
                                azureActiveDirectoryConfiguration.Value.Identifier,
                                azureActiveDirectoryConfiguration.Value.Id
                            }
                        };
                    });
                services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            }

            services.AddMediatR(typeof(GetAccountReservationsQueryHandler).Assembly);
            services.AddMediatRValidators();

            services.AddServiceRegistration(config);
            
            if (Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Reservations"));
            }
            else
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.Value.ConnectionString));
            }

            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());
            services.AddTransient(provider => new Lazy<ReservationsDataContext>(provider.GetService<ReservationsDataContext>()));

            services
                .AddMvc(o =>
                {
                    if (!ConfigurationIsLocalOrDev())
                    {
                        o.Filters.Add(new AuthorizeFilter("default"));
                    }
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (!Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services
                    .AddEntityFramework()
                    .AddEntityFrameworkUnitOfWork<ReservationsDataContext>()
                    .AddNServiceBusClientUnitOfWork();
            }
            else
            {
                services.AddTransient<IUnitOfWorkContext, DevUnitOfWorkContext>();
                services.AddTransient<IUnitOfWorkManager, DevUnitOfWorkManager>();
            }

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

           
        }

        public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
        {
            if (!Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                serviceProvider.StartNServiceBus(Configuration, ConfigurationIsLocalOrDev());
            }
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (ConfigurationIsLocalOrDev())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseAuthentication();
            }

            app.UseUnitOfWork();
            app.UseHealthChecks();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Reservation}/{action=Index}/{id?}");
            });
        }

        private bool ConfigurationIsLocalOrDev()
        {
            return Configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
