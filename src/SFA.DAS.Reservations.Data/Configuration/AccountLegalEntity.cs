using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class AccountLegalEntity : IEntityTypeConfiguration<Domain.Entities.AccountLegalEntity>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.AccountLegalEntity> builder)
        {
            builder.ToTable("AccountLegalEntity");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.LegalEntityId).HasColumnName(@"LegalEntityId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.AccountLegalEntityId).HasColumnName(@"AccountLegalEntityId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.AccountLegalEntityName).HasColumnName(@"AccountLegalEntityName").HasColumnType("varchar").HasMaxLength(100);
            builder.Property(x => x.ReservationLimit).HasColumnName(@"ReservationLimit").HasColumnType("int");
            builder.Property(x => x.AgreementSigned).HasColumnName(@"AgreementSigned").HasColumnType("bit").IsRequired();
            builder.Property(x => x.IsLevy).HasColumnName(@"IsLevy").HasColumnType("bit").IsRequired();
            builder.Property(x => x.AgreementType).HasColumnName(@"AgreementType").HasColumnType("tinyint").IsRequired();

            builder.HasIndex(x => x.AccountLegalEntityId).IsUnique();
        }
    }
}
