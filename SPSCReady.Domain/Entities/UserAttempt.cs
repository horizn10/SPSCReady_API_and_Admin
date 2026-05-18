using SPSCReady.Domain.Enums;

namespace SPSCReady.Domain.Entities;

public class UserAttempt
{
    public int AttemptId { get; set; }
    public int UserId { get; set; }
    public int MockTestId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public decimal? TotalScore { get; set; }
    public int? CorrectCount { get; set; }
    public int? WrongCount { get; set; }
    public int? SkippedCount { get; set; }
    public decimal? Percentage { get; set; }
    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;

    public MockTest MockTest { get; set; } = default!;
    public ICollection<UserAnswer> UserAnswers { get; set; } = [];
}
