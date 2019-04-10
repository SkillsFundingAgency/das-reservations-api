using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class GlobalRule : IEntityTypeConfiguration<Domain.Entities.GlobalRule>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.GlobalRule> builder)
        {
            builder.ToTable("GlobalRule");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ActiveFrom).HasColumnName(@"ActiveFrom").HasColumnType("datetime");
            builder.Property(x => x.ActiveTo).HasColumnName(@"ActiveTo").HasColumnType("datetime");
            builder.Property(x => x.Restriction).HasColumnName(@"Restriction").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.RuleType).HasColumnName(@"RuleType").HasColumnType("tinyint").IsRequired();
        }
    }
}