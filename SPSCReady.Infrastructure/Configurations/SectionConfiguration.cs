using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");
        builder.HasKey(x => x.SectionId);

        builder.Property(x => x.SectionName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.SubjectTag)
               .HasMaxLength(50);

        builder.Property(x => x.OrderIndex)
               .IsRequired();

        builder.Property(x => x.QuestionCount)
               .IsRequired();

        builder.Property(x => x.MarksPerQuestion)
               .IsRequired()
               .HasColumnType("decimal(4,2)");

        builder.Property(x => x.NegativeMarks)
               .IsRequired()
               .HasColumnType("decimal(4,2)");

        builder.HasOne(x => x.MockTest)
               .WithMany(m => m.Sections)
               .HasForeignKey(x => x.MockTestId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.MockTestId);
    }
}
