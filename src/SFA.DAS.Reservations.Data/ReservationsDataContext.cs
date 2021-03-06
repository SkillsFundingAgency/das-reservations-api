﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Data.Configuration;

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

        int SaveChanges();
    }

    public partial class ReservationsDataContext : DbContext, IReservationsDataContext
    {
        public DbSet<Domain.Entities.Course> Courses { get; set; }
        public DbSet<Domain.Entities.Reservation> Reservations { get; set; }
        public DbSet<Domain.Entities.Rule> Rules { get; set; }
        public DbSet<Domain.Entities.GlobalRule> GlobalRules { get; set; }
        public DbSet<Domain.Entities.AccountLegalEntity> AccountLegalEntities { get; set; }
        public DbSet<Domain.Entities.UserRuleNotification> UserRuleNotifications { get; set; }
        public DbSet<Domain.Entities.ProviderPermission> ProviderPermissions { get; set; }
        public DbSet<Domain.Entities.Account> Accounts { get; set; }

        public ReservationsDataContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public ReservationsDataContext(DbContextOptions options) :base(options)
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
