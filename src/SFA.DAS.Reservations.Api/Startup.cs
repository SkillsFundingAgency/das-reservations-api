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
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;
using SFA.DAS.Reservations.Application.Courses.Services;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Infrastructure.Configuration;

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
                .AddJsonFile("appsettings.development.json", !Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
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

            if (!ConfigurationIsLocalOrDev())
            {
                var azureActiveDirectoryConfiguration = serviceProvider.GetService<IOptions<AzureActiveDirectoryConfiguration>>();
                services.AddAuthorization(o =>
                {
                    o.AddPolicy("default", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
                });
                services.AddAuthentication(auth =>
                {
                    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(auth =>
                {
                    auth.Authority = $"https://login.microsoftonline.com/{azureActiveDirectoryConfiguration.Value.Tenant}";
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
            services.AddScoped(typeof(IValidator<GetAccountReservationsQuery>), typeof(GetAccountReservationsValidator));
            services.AddScoped(typeof(IValidator<CreateAccountReservationCommand>), typeof(CreateAccountReservationValidator));
            services.AddScoped(typeof(IValidator<GetReservationQuery>), typeof(GetReservationValidator));
            services.AddTransient<IReservationRepository,ReservationRepository>();
            services.AddTransient<IRuleRepository,RuleRepository>();
            services.AddTransient<IGlobalRuleRepository, GlobalRuleRepository>();
            services.AddTransient<ICourseRepository,CourseRepository>();
            services.AddTransient<IAccountReservationService, AccountReservationService>();
            services.AddTransient<IRulesService, RulesService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IGlobalRulesService, GlobalRulesService>();
            services.AddTransient<IAvailableDatesService, AvailableDatesService>();
            
            services.AddMvc(o =>
            {
                if (!ConfigurationIsLocalOrDev())
                {
                    o.Filters.Add(new AuthorizeFilter("default"));
                }
                
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Reservations"));
            }
            else
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(config.Value.ConnectionString));
            }
            
            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
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
