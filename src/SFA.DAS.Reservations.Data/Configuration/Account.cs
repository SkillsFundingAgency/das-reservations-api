using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class Account : IEntityTypeConfiguration<Domain.Entities.Account>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Account> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.Name).HasColumnName("Name").HasColumnType("varchar").HasMaxLength(500).IsRequired();
            builder.Property(x => x.IsLevy).HasColumnName(@"IsLevy").HasColumnType("bit").IsRequired();
            builder.Property(x => x.ReservationLimit).HasColumnName(@"ReservationLimit").HasColumnType("int");
            
            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
}