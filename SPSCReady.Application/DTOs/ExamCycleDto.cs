using System;

namespace SPSCReady.Application.DTOs
{
    public class ExamCycleDto
    {
        public Guid Id { get; set; }
        public int ExamYear { get; set; }
        public Guid PostId { get; set; }
    }
}

