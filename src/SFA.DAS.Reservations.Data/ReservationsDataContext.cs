using System;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Data.Configuration;

namespace SFA.DAS.Reservations.Data
{
    public interface IReservationsDataContext 
    {
        DbSet<Domain.Entities.Course> Courses { get; set; }
        DbSet<Domain.Entities.Reservation> Reservations { get; set; }
        DbSet<Domain.Entities.Rule> Rules { get; set; }
        int SaveChanges();
    }

    public partial class ReservationsDataContext : DbContext, IReservationsDataContext
    {
        public DbSet<Domain.Entities.Course> Courses { get; set; }
        public DbSet<Domain.Entities.Reservation> Reservations { get; set; }
        public DbSet<Domain.Entities.Rule> Rules { get; set; }
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

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Course());
            modelBuilder.ApplyConfiguration(new Reservation());
            modelBuilder.ApplyConfiguration(new Rule());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
