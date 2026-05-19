using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Configurations;

public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.ToTable("MockUserAnswers");
        builder.HasKey(x => x.AnswerId);

        builder.Property(x => x.AttemptId)
               .IsRequired();

        builder.Property(x => x.QuestionId)
               .IsRequired();

        builder.Property(x => x.SelectedOption)
               .HasMaxLength(1);

        builder.Property(x => x.IsCorrect);

        builder.Property(x => x.MarksAwarded)
               .HasColumnType("decimal(6,2)");

        builder.Property(x => x.IsMarkedForReview)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.AnsweredAt);

        builder.HasIndex(x => new { x.AttemptId, x.QuestionId })
               .IsUnique();

        builder.HasOne(x => x.UserAttempt)
               .WithMany(a => a.UserAnswers)
               .HasForeignKey(x => x.AttemptId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
               .WithMany(q => q.UserAnswers)
               .HasForeignKey(x => x.QuestionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AttemptId);
        builder.HasIndex(x => x.QuestionId);
    }
}
