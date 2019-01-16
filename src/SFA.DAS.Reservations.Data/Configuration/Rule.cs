using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class Rule : IEntityTypeConfiguration<Domain.Entities.Rule>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Rule> builder)
        {
            builder.ToTable("Rule");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Restriction).HasColumnName(@"Restriction").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ActiveFrom).HasColumnName(@"ActiveFrom").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ActiveTo).HasColumnName(@"ActiveTo").HasColumnType("datetime").IsRequired();

            builder.HasOne(c => c.ApprenticeshipCourse).WithMany(c => c.ReservationRule)
                .HasForeignKey(c => c.ApprenticeshipId);

        }
    }
}