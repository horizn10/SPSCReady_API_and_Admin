using SPSCReady.Domain.Enums;

namespace SPSCReady.Domain.Entities;

public class Question
{
    public int QuestionId { get; set; }
    public int SectionId { get; set; }
    public string QuestionText { get; set; } = default!;
    public string OptionA { get; set; } = default!;
    public string OptionB { get; set; } = default!;
    public string OptionC { get; set; } = default!;
    public string OptionD { get; set; } = default!;
    public char CorrectOption { get; set; }
    public string? Explanation { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; } = true;

    public Section Section { get; set; } = default!;
    public ICollection<UserAnswer> UserAnswers { get; set; } = [];
}
