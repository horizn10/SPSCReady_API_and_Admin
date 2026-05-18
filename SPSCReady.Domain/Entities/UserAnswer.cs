namespace SPSCReady.Domain.Entities;

public class UserAnswer
{
    public int AnswerId { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public char? SelectedOption { get; set; }  // null = skipped
    public bool? IsCorrect { get; set; }
    public decimal? MarksAwarded { get; set; }
    public bool IsMarkedForReview { get; set; }
    public DateTime? AnsweredAt { get; set; }

    public UserAttempt UserAttempt { get; set; } = default!;
    public Question Question { get; set; } = default!;
}
