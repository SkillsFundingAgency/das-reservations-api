using System;
using System.Data;
using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Data.Configuration;
using SFA.DAS.Reservations.Domain.Configuration;
using Rule = SFA.DAS.Reservations.Data.Configuration.Rule;

namespace SFA.DAS.Reservations.Data
{
    public interface IReservationsDataContext
    {
        DbSet<Domain.Entities.Course> Courses { get; set; }
        DbSet<Domain.Entities.Reservation> Reservations { get; set; }
        DbSet<Domain.Entities.Rule> Rules { get; set; }
        DbSet<Domain.Entities.GlobalRule> GlobalRules { get; set; }
        DbSet<Domain.Entities.AccountLegalEntity> AccountLegalEntities { get; set; }
        DbSet<Domain.Entities.UserRuleNotification> UserRuleNotifications { get; set; }
        DbSet<Domain.Entities.ProviderPermission> ProviderPermissions { get; set; }
        DbSet<Domain.Entities.Account> Accounts { get; set; }
        DbSet<Domain.Entities.GlobalRuleAccountExemption> GlobalRulesAccountExemption { get; set; }

        int SaveChanges();
    }

    public partial class ReservationsDataContext : DbContext, IReservationsDataContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly IDbConnection _connection;

        public DbSet<Domain.Entities.Course> Courses { get; set; }
        public DbSet<Domain.Entities.Reservation> Reservations { get; set; }
        public DbSet<Domain.Entities.Rule> Rules { get; set; }
        public DbSet<Domain.Entities.GlobalRule> GlobalRules { get; set; }
        public DbSet<Domain.Entities.AccountLegalEntity> AccountLegalEntities { get; set; }
        public DbSet<Domain.Entities.UserRuleNotification> UserRuleNotifications { get; set; }
        public DbSet<Domain.Entities.ProviderPermission> ProviderPermissions { get; set; }
        public DbSet<Domain.Entities.Account> Accounts { get; set; }
        public DbSet<Domain.Entities.GlobalRuleAccountExemption> GlobalRulesAccountExemption { get; set; }

        private readonly ReservationsConfiguration _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public ReservationsDataContext()
        {
        }

        public ReservationsDataContext(IDbConnection connection, ReservationsConfiguration configuration, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
        {
            _configuration = configuration;
            _azureServiceTokenProvider = azureServiceTokenProvider;
            _connection = connection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            if (_connection != null)
            {
                optionsBuilder.UseSqlServer(_connection as DbConnection);
            }
            else
            {
                if (_configuration == null || _azureServiceTokenProvider == null)
                {
                    return;
                }

                var connectionStringBuilder = new SqlConnectionStringBuilder(_configuration.ConnectionString);
                bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                
                var connection = useManagedIdentity
                    ? new SqlConnection
                    {
                        ConnectionString = _configuration.ConnectionString,
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    }
                    : new SqlConnection(_configuration.ConnectionString);

            optionsBuilder.UseSqlServer(connection, options =>
                 options.EnableRetryOnFailure(
                     5,
                     TimeSpan.FromSeconds(20),
                     null
                 ));
        }
    }

    public ReservationsDataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Course());
        modelBuilder.ApplyConfiguration(new Reservation());
        modelBuilder.ApplyConfiguration(new Rule());
        modelBuilder.ApplyConfiguration(new GlobalRule());
        modelBuilder.ApplyConfiguration(new AccountLegalEntity());
        modelBuilder.ApplyConfiguration(new UserRuleNotification());
        modelBuilder.ApplyConfiguration(new ProviderPermission());
        modelBuilder.ApplyConfiguration(new Account());
        modelBuilder.ApplyConfiguration(new GlobalRuleAccountExemption());

        base.OnModelCreating(modelBuilder);
    }
}
}
