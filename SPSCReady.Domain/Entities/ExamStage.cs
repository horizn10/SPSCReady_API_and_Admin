using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class ExamStage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty; // e.g., "Prelims", "Mains"

        public ICollection<ExamPaper> ExamPapers { get; set; } = new List<ExamPaper>();
    }
}