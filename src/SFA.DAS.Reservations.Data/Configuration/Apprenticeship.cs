using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class Apprenticeship : IEntityTypeConfiguration<Domain.Entities.Apprenticeship>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Apprenticeship> builder)
        {
            builder.ToTable("Apprenticeship");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.CourseId).HasColumnName(@"CourseId").HasColumnType("varchar").HasMaxLength(20).IsRequired();
            builder.Property(x => x.Title).HasColumnName(@"Title").HasColumnType("varchar").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Level).HasColumnName(@"Level").HasColumnType("tinyint").IsRequired();

        }
    }
}