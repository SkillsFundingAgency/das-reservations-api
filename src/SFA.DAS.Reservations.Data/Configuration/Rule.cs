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
            builder.Property(x => x.CourseId).HasColumnName(@"CourseId").HasColumnType("varchar").HasMaxLength(20).IsRequired();
            builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ActiveFrom).HasColumnName(@"ActiveFrom").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ActiveTo).HasColumnName(@"ActiveTo").HasColumnType("datetime").IsRequired();

            builder.HasOne(c => c.Course).WithMany(c => c.ReservationRule)
                .HasForeignKey(c => c.CourseId);

            builder.HasMany(c => c.UserRuleNotifications).WithOne(c => c.CourseRule)
                .HasForeignKey(c => c.CourseRuleId)
                .IsRequired(false);
        }
    }
}