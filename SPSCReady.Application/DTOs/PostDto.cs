using System;

namespace SPSCReady.Application.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
    }
}

