namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class SelectedSearchFilters
    {
        public string CourseFilter { get; set; }
        public string EmployerNameFilter { get; set; }
        public bool HasFilters => !string.IsNullOrWhiteSpace(CourseFilter);
    }
}
