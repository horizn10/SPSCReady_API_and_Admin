namespace SPSCReady.Domain.Entities;

public class Exam
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = default!;
    public string ExamCode { get; set; } = default!;
    public int ExamYear { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<MockTest> MockTests { get; set; } = [];
}
