using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.Reservations.Api.AppStart;
using SFA.DAS.Reservations.Api.StartupConfig;
using SFA.DAS.Reservations.Api.StartupExtensions;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Infrastructure.DevConfiguration;
using SFA.DAS.Reservations.Infrastructure.HealthCheck;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.Mvc.Extensions;

namespace SFA.DAS.Reservations.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.development.json", true)
            .AddEnvironmentVariables();

        if (!configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["Environment"];
                    options.PreFixConfigurationKeys = false;
                }
            );
        }

        _configuration = config.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.Configure<ReservationsConfiguration>(_configuration.GetSection("Reservations"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsConfiguration>>().Value);
        services.Configure<AzureActiveDirectoryConfiguration>(_configuration.GetSection("AzureAd"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value);

        var config = _configuration
            .GetSection("Reservations")
            .Get<ReservationsConfiguration>();

        services.AddElasticSearch(config);
        services.AddSingleton(new ReservationsApiEnvironment(_configuration["Environment"]));

        services.AddHealthChecks().AddDbContextCheck<ReservationsDataContext>();
        services.AddHealthChecks()
            .AddCheck<QueueHealthCheck>(
                "ServiceBus Queue Health",
                HealthStatus.Unhealthy,
                new[] { "ready" })
            .AddCheck<ElasticSearchHealthCheck>(
                "Elastic Search Health",
                HealthStatus.Unhealthy,
                new[] { "ready" });

        if (!ConfigurationIsLocalOrDev())
        {
            var azureActiveDirectoryConfiguration = _configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();

            services.AddAuthorization(o =>
            {
                o.AddPolicy("default", policy =>
                {
                    policy.RequireRole("Default");
                    policy.RequireAuthenticatedUser();
                });
            });
            services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(auth =>
                {
                    auth.Authority =
                        $"https://login.microsoftonline.com/{azureActiveDirectoryConfiguration.Tenant}";
                    auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = azureActiveDirectoryConfiguration.Identifier.Split(",")
                    };
                });
            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
        }

        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetAccountReservationsQueryHandler).Assembly));
        services.AddMediatRValidators();
        services.AddLogging();

        services.AddServiceRegistration(config);

        if (_configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<ReservationsDataContext>(options =>
                options.UseInMemoryDatabase("SFA.DAS.Reservations"));
        }
        else
        {
            services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.ConnectionString));
        }

        services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider =>
            provider.GetService<ReservationsDataContext>());
        services.AddTransient(provider =>
            new Lazy<ReservationsDataContext>(provider.GetService<ReservationsDataContext>()));

        services
            .AddControllersWithViews(o =>
            {
                if (!ConfigurationIsLocalOrDev())
                {
                    o.Filters.Add(new AuthorizeFilter("default"));
                }
            });

        if (!_configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services
                .AddEntityFramework(config)
                .AddEntityFrameworkUnitOfWork<ReservationsDataContext>()
                .AddNServiceBusClientUnitOfWork();
        }
        else
        {
            services.AddTransient<IUnitOfWorkContext, DevUnitOfWorkContext>();
            services.AddTransient<IUnitOfWorkManager, DevUnitOfWorkManager>();
        }

        services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReservationsAPI", Version = "v1" });
        });

        services
            .AddControllers()
            .AddNewtonsoftJson();
    }

    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        if (_configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }
        
        serviceProvider.StartNServiceBus(_configuration, ConfigurationIsLocalOrDev());

        // Replacing ClientOutboxPersisterV2 with a local version to fix unit of work issue due to propogating Task up the chain rathert than awaiting on DB Command.
        // not clear why this fixes the issue. Attempted to make the change in SFA.DAS.Nservicebus.SqlServer however it conflicts when upgraded with SFA.DAS.UnitOfWork.Nservicebus
        // which would require upgrading to NET6 to resolve.
        var serviceDescriptor =  serviceProvider.FirstOrDefault(serv => serv.ServiceType == typeof(IClientOutboxStorageV2));
        serviceProvider.Remove(serviceDescriptor);
        serviceProvider.AddScoped<IClientOutboxStorageV2, AppStart.ClientOutboxPersisterV2>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseUnitOfWork();

        if (ConfigurationIsLocalOrDev())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
            app.UseAuthentication();
        }

        app.UseHealthChecks();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReservationsAPI");
            c.RoutePrefix = string.Empty;
        });
    }

    private bool ConfigurationIsLocalOrDev()
    {
        return _configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               _configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}