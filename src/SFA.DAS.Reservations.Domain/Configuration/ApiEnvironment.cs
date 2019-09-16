namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ApiEnvironment
    {
        public string EnvironmentName { get; }

        public ApiEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }
    }
}