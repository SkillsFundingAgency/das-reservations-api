namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsApiEnvironment(string environmentName)
    {
        public virtual string EnvironmentName { get; } = environmentName.ToLower();
    }
}
