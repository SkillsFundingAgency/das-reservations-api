using System.IO;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            Configuration = builder;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ReservationConfiguration>(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMediatR(typeof(GetAccountReservationsQueryHandler).Assembly);
            services.AddScoped(typeof(IValidator<GetAccountReservationsQuery>), typeof(GetAccountReservationsValidator));
            services.AddScoped(typeof(IValidator<CreateAccountReservationCommand>), typeof(CreateAccountReservationValidator));
            services.AddTransient<IReservationRepository,ReservationRepository>();
            services.AddTransient<IRuleRepository,RuleRepository>();
            services.AddTransient<IAccountReservationService, AccountReservationService>();
            services.AddTransient<IRulesService, RulesService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<ReservationsDataContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Reservations"]));
            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Reservation}/{action=Index}/{id?}");
            });
        }
    }
}
