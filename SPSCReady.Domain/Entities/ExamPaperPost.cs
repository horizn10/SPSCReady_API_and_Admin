using System;

namespace SPSCReady.Domain.Entities
{
    public class ExamPaperPost
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public ExamPaper Exam { get; set; } = null!;
    }
}
