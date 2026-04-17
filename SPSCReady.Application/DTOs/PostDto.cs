using System;

namespace SPSCReady.Application.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }
}
