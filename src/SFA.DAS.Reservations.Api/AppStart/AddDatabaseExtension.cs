using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public static class AddDatabaseExtension
    {
        private const string AzureResource = "https://database.windows.net/";

        public static DbConnection GetSqlConnection(bool configurationIsLocalOrDev, string connectionString)
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
