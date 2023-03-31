using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public static class AddDatabaseExtension
    {
        private const string AzureResource = "https://database.windows.net/";

        public static DbConnection GetSqlConnection(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);

            if (useManagedIdentity)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                return new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                };
            }
            else
            {
                return new SqlConnection(connectionString);
            }
        }
    }
}
