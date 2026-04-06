using System;

namespace SPSCReady.Domain.Entities
{
    public class ExamPaper
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ExamCycleId { get; set; }
        public Guid ExamStageId { get; set; }
        public Guid SubjectId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string PdfUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ExamCycle ExamCycle { get; set; } = null!;
        public ExamStage ExamStage { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}