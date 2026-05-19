using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("MockQuestions");
        builder.HasKey(x => x.QuestionId);

        builder.Property(x => x.QuestionText)
               .IsRequired()
               .HasMaxLength(-1);  // nvarchar(max)

        builder.Property(x => x.OptionA)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.OptionB)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.OptionC)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.OptionD)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.CorrectOption)
               .IsRequired()
               .HasMaxLength(1);

        builder.Property(x => x.Explanation)
               .HasMaxLength(-1);  // nvarchar(max)

        builder.Property(x => x.DifficultyLevel)
               .HasConversion<string>()
               .HasMaxLength(10);

        builder.Property(x => x.OrderIndex)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.HasOne(x => x.Section)
               .WithMany(s => s.Questions)
               .HasForeignKey(x => x.SectionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SectionId);
    }
}
