using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class SearchFilters
    {
        public SearchFilters()
        {
            CourseFilters = new List<string>();
            EmployerFilters = new List<string>();
        }

        public ICollection<string> CourseFilters { get; set; }
        public ICollection<string> EmployerFilters { get; set; }
    }
}
