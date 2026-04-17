using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Removed ExamPapers (now hierarchical)
        
        public int StageId { get; set; }
        public ExamStage? Stage { get; set; } = null;
    }
}
