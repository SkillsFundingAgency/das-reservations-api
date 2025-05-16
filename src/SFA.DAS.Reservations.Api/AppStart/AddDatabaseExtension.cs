using System.Data.Common;
using Azure.Identity;
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
                var credential = new DefaultAzureCredential();
                var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { AzureResource })).Token;

                return new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = token
                };
            }
            else
            {
                return new SqlConnection(connectionString);
            }
        }
    }
}
