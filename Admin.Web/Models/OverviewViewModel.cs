using System.Collections.Generic;

namespace SPSCReady.Admin.Models
{
    public class OverviewViewModel
    {
        public int TotalActiveUsers { get; set; }
        public int TotalPapers { get; set; }
        public long TotalViews { get; set; }

        public List<PaperPerformanceDto> Papers { get; set; } = new();
    }

    public class PaperPerformanceDto
    {
        public int Id { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Status { get; set; } = "Published";
        public int TotalViews { get; set; }
    }
}