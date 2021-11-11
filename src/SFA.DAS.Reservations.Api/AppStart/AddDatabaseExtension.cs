using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public static class AddDatabaseExtension
    {
        private const string AzureResource = "https://database.windows.net/";

        public static DbConnection GetConnectionString(bool configurationIsLocalOrDev, string connectionString)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            return configurationIsLocalOrDev
                ? new SqlConnection(connectionString)
                : new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                };
        }
    }
}
