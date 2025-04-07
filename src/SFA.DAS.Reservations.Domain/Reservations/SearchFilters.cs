using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class SearchFilters
    {
        public ICollection<string> CourseFilters { get; set; } = new List<string>();
        public ICollection<string> EmployerFilters { get; set; } = new List<string>();
        public ICollection<string> StartDateFilters { get; set; } = new List<string>();
    }
}
