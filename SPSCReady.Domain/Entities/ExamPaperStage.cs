using System;

namespace SPSCReady.Domain.Entities
{
    public class ExamPaperStage
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int StageId { get; set; }
        public ExamStage Stage { get; set; } = null!;

        public ExamPaper Exam { get; set; } = null!;
    }
}
