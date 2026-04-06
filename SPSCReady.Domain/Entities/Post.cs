using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;

        public Department Department { get; set; } = null!;
        public ICollection<ExamCycle> ExamCycles { get; set; } = new List<ExamCycle>();
    }
}