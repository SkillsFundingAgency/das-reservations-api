using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class GlobalRuleAccountExemption : IEntityTypeConfiguration<Domain.Entities.GlobalRuleAccountExemption>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.GlobalRuleAccountExemption> builder)
        {
            builder.ToTable("GlobalRuleAccountExemption");
            builder.HasKey(x => new { x.GlobalRuleId, x.AccountId });

            builder.Property(x => x.GlobalRuleId).HasColumnName("GlobalRuleId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.AccountId).HasColumnName("AccountId").HasColumnType("bigint").IsRequired();

            builder.HasOne(c => c.Account)
                .WithMany(c => c.GlobalRuleAccountExemptions)
                .HasForeignKey(c => c.AccountId)
                .HasPrincipalKey(c => c.Id);

            builder.HasOne(c => c.GlobalRule)
                .WithMany(c => c.GlobalRuleAccountExemptions)
                .HasForeignKey(c => c.GlobalRuleId)
                .HasPrincipalKey(c => c.Id);
        }
    }
}
