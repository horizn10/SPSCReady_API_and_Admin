using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class ExamCycle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PostId { get; set; }
        public Guid DepartmentId { get; set; } // Kept for optimized querying as requested
        public int ExamYear { get; set; }

        public Post Post { get; set; } = null!;
        public Department Department { get; set; } = null!;
        public ICollection<ExamPaper> ExamPapers { get; set; } = new List<ExamPaper>();
    }
}