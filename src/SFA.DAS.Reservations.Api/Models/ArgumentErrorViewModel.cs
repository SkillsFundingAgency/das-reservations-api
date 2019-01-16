namespace SFA.DAS.Reservations.Api.Models
{
    public class ArgumentErrorViewModel : ErrorViewModel
    {
        public string Message { get; set; }
        public string Params { get; set; }
    }
}