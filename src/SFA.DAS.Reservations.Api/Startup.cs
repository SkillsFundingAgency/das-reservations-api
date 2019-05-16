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
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Api.StartupConfig;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Application.Courses.Services;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Infrastructure.Configuration;
using SFA.DAS.Reservations.Api.StartupExtensions;
using SFA.DAS.UnitOfWork.EntityFrameworkCore;
using SFA.DAS.UnitOfWork.Mvc;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.Reservations.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.development.json",
                    !Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
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
            

            services.AddHealthChecks()
                    .AddSqlServer(config.Value.ConnectionString);

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
            services.AddScoped(typeof(IValidator<GetAccountReservationsQuery>),
                typeof(GetAccountReservationsValidator));
            services.AddScoped(typeof(IValidator<CreateAccountReservationCommand>),
                typeof(CreateAccountReservationValidator));
            services.AddScoped(typeof(IValidator<GetReservationQuery>), typeof(GetReservationValidator));
            services.AddScoped(typeof(IValidator<GetAccountRulesQuery>), typeof(GetAccountRulesValidator));
            services.AddScoped(typeof(IValidator<GetAccountLegalEntitiesQuery>), typeof(GetAccountLegalEntitiesQueryValidator));
            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IRuleRepository, RuleRepository>();
            services.AddTransient<IGlobalRuleRepository, GlobalRuleRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<IAccountLegalEntitiesRepository,AccountLegalEntityRepository>();
            services.AddTransient<IAccountReservationService, AccountReservationService>();
            services.AddTransient<IRulesService, RulesService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IGlobalRulesService, GlobalRulesService>();
            services.AddTransient<IAvailableDatesService, AvailableDatesService>();
            services.AddTransient<IAccountLegalEntitiesService, AccountLegalEntitiesService>();

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
                .AddEntityFramework()
                .AddEntityFrameworkUnitOfWork<ReservationsDataContext>()
                .AddNServiceBusClientUnitOfWork()
                .AddMvc(o =>
                {
                    if (!ConfigurationIsLocalOrDev())
                    {
                        o.Filters.Add(new AuthorizeFilter("default"));
                    }
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
        }

        public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
        {
            
            serviceProvider.StartNServiceBus(Configuration);
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

            app.UseHttpsRedirection();
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