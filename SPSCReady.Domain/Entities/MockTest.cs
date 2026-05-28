using SPSCReady.Domain.Enums;

namespace SPSCReady.Domain.Entities;

public class MockTest
{
    public int MockTestId { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; } = default!;
    public PaperType PaperType { get; set; }
    public int PaperNumber { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalMarks { get; set; }
    public decimal TotalQuestions { get; set; }
    public decimal? PassingMarks { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Exam Exam { get; set; } = default!;
    public ICollection<Section> Sections { get; set; } = [];
    public ICollection<UserAttempt> UserAttempts { get; set; } = [];
}
