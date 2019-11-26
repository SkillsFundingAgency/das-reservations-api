using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class ProviderPermission : IEntityTypeConfiguration<Domain.Entities.ProviderPermission>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ProviderPermission> builder)
        {
            builder.ToTable("ProviderPermission");
            builder.HasKey(x => new {x.AccountId, x.UkPrn, x.AccountLegalEntityId});


            builder.Property(x => x.AccountId).HasColumnName("AccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.UkPrn).HasColumnName("UkPrn").HasColumnType("UkPrn").IsRequired();
            builder.Property(x => x.AccountLegalEntityId).HasColumnName("AccountLegalEntityId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CanCreateCohort).HasColumnName("CanCreateCohort").HasColumnType("bit").IsRequired();

            builder.HasOne(c => c.AccountLegalEntity)
                .WithMany(c => c.ProviderPermissions)
                .HasForeignKey(c => c.AccountLegalEntityId)
                .HasPrincipalKey(c => c.AccountLegalEntityId);
        }
    }
}