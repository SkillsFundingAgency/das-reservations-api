using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class Reservation : IEntityTypeConfiguration<Domain.Entities.Reservation>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Reservation> builder)
        {
            builder.ToTable("Reservation");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ApprenticeId).HasColumnName(@"ApprenticeId").HasColumnType("bigint");
            builder.Property(x => x.VacancyId).HasColumnName(@"VacancyId").HasColumnType("bigint");
            builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ExpiryDate).HasColumnName(@"ExpiryDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.IsLevyAccount).HasColumnName(@"IsLevyAccount").HasColumnType("tinyint").IsRequired();
        }
    }
}
