using System;
using System.Collections.Generic;

namespace SPSCReady.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        // Removed ExamCycles
    }
}
