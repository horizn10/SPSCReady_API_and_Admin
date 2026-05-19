using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPSCReady.Domain.Entities;

namespace SPSCReady.Infrastructure.Configurations;

public class UserAttemptConfiguration : IEntityTypeConfiguration<UserAttempt>
{
    public void Configure(EntityTypeBuilder<UserAttempt> builder)
    {
        builder.ToTable("MockUserAttempts");
        builder.HasKey(x => x.AttemptId);

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.MockTestId)
               .IsRequired();

        builder.Property(x => x.StartedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.SubmittedAt);

        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.TotalScore)
               .HasColumnType("decimal(8,2)");

        builder.Property(x => x.CorrectCount);

        builder.Property(x => x.WrongCount);

        builder.Property(x => x.SkippedCount);

        builder.Property(x => x.Percentage)
               .HasColumnType("decimal(5,2)");

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.HasIndex(x => new { x.UserId, x.MockTestId })
               .IsUnique(false);  // Set true if you want to enforce 1 attempt per user per test

        builder.HasOne(x => x.MockTest)
               .WithMany(m => m.UserAttempts)
               .HasForeignKey(x => x.MockTestId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.MockTestId);
    }
}
