using System;

namespace SPSCReady.Domain.Entities
{
    public class ExamPaperSubject
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int StageId { get; set; }
        public ExamStage Stage { get; set; } = null!;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        public string? SubjectName { get; set; } 
        public string? Url { get; set; } 
        public DateTime? Date { get; set; }

        public ExamPaper ExamPaper { get; set; } = null!;
    }
}
