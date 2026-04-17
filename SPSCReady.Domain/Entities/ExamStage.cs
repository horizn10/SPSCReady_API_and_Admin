using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class ExamStage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Removed ExamPapers (now hierarchical)
    }
}
