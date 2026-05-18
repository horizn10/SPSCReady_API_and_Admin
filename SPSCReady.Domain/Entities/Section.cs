namespace SPSCReady.Domain.Entities;

public class Section
{
    public int SectionId { get; set; }
    public int MockTestId { get; set; }
    public string SectionName { get; set; } = default!;
    public string? SubjectTag { get; set; }
    public int OrderIndex { get; set; }
    public int QuestionCount { get; set; }
    public decimal MarksPerQuestion { get; set; }
    public decimal NegativeMarks { get; set; }

    public MockTest MockTest { get; set; } = default!;
    public ICollection<Question> Questions { get; set; } = [];
}
