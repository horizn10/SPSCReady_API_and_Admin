using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class ExamPaper
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ExamDate { get; set; }
        public string? UploadedBy { get; set; } 
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ExamPaperDept> Departments { get; set; } = new List<ExamPaperDept>();
        public ICollection<ExamPaperPost> Posts { get; set; } = new List<ExamPaperPost>();
        public ICollection<ExamPaperStage> Stages { get; set; } = new List<ExamPaperStage>();
        public ICollection<ExamPaperSubject> Subjects { get; set; } = new List<ExamPaperSubject>();
    }
}
