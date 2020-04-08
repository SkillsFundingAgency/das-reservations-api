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
            
            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate").HasColumnType("datetime");
            builder.Property(x => x.ExpiryDate).HasColumnName(@"ExpiryDate").HasColumnType("datetime");
            builder.Property(x => x.IsLevyAccount).HasColumnName(@"IsLevyAccount").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.Status).HasColumnName(@"Status").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.CourseId).HasColumnName(@"CourseId").HasColumnType("varchar").HasMaxLength(20);
            builder.Property(x => x.AccountLegalEntityName).HasColumnName(@"AccountLegalEntityName").HasColumnType("varchar").HasMaxLength(100);
            builder.Property(x => x.ProviderId).HasColumnName(@"ProviderId").HasColumnType("int");
            builder.Property(x => x.AccountLegalEntityId).HasColumnName(@"AccountLegalEntityId").HasColumnType("bigint");
            builder.Property(x => x.TransferSenderAccountId).HasColumnName(@"TransferSenderAccountId").HasColumnType("bigint");
            builder.Property(x => x.UserId).HasColumnName("UserId").HasColumnType("uniqueidentifier");
            builder.Property(x => x.ClonedReservationId).HasColumnName("ClonedReservationId").HasColumnType("uniqueidentifier");

            builder.HasOne(c => c.Course).WithMany(c => c.Reservations)
                   .HasForeignKey(c => c.CourseId);

        }
    }
}
