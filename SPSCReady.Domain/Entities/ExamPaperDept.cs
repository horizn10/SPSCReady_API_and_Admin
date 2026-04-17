using System;

namespace SPSCReady.Domain.Entities
{
    public class ExamPaperDept
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public ExamPaper Exam { get; set; } = null!;
    }
}
