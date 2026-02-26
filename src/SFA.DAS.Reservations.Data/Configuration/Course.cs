using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Reservations.Data.Configuration
{
    public class Course : IEntityTypeConfiguration<Domain.Entities.Course>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Course> builder)
        {
            builder.ToTable("Course");
            builder.HasKey(x => x.CourseId);

            builder.Property(x => x.CourseId).HasColumnName(@"CourseId").HasColumnType("varchar").HasMaxLength(20).IsRequired();
            builder.Property(x => x.Title).HasColumnName(@"Title").HasColumnType("varchar").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Level).HasColumnName(@"Level").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.EffectiveTo).HasColumnName(@"EffectiveTo").HasColumnType("datetime");
            builder.Property(x => x.ApprenticeshipType).HasColumnName(@"ApprenticeshipType").HasColumnType("varchar").HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.LearningType).HasColumnName(@"LearningType").HasColumnType("tinyint").IsRequired(false);
        }
    }
}