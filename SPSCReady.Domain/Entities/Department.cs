using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<ExamCycle> ExamCycles { get; set; } = new List<ExamCycle>();
    }
}