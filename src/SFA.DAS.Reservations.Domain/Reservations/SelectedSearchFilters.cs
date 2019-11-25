namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class SelectedSearchFilters
    {
        public string CourseFilter { get; set; }
        public string EmployerNameFilter { get; set; }
        public string StartDateFilter { get; set; }

        public bool HasFilters => 
            !string.IsNullOrWhiteSpace(CourseFilter) ||
            !string.IsNullOrWhiteSpace(EmployerNameFilter) ||
            !string.IsNullOrWhiteSpace(StartDateFilter);
    }
}
