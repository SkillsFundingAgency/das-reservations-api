using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class UserRuleNotification : IEntityTypeConfiguration<Domain.Entities.UserRuleNotification>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.UserRuleNotification> builder)
        {
            builder.ToTable("UserRuleNotification");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.CourseRuleId).HasColumnName(@"CourseRuleId").HasColumnType("bigint").IsRequired(false);
            builder.Property(x => x.GlobalRuleId).HasColumnName(@"GlobalRuleId").HasColumnType("bigint").IsRequired(false);
            builder.Property(x => x.UkPrn).HasColumnName(@"UkPrn").HasColumnType("int");
            builder.Property(x => x.UserId).HasColumnName(@"UserId").HasColumnType("uniqueidentifier");
        }
    }
}
