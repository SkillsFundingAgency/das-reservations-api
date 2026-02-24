using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public static class AddDatabaseExtension
    {
        public static DbConnection GetSqlConnection(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            bool useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);

            if (useManagedIdentity)
            {
                var credential = new DefaultAzureCredential();
                var tokenRequestContext = new TokenRequestContext(["https://database.windows.net/.default"]);
                
                var accessToken = credential.GetTokenAsync(tokenRequestContext, default).GetAwaiter().GetResult();

                return new SqlConnection
                {
                    ConnectionString = connectionString,
                    AccessToken = accessToken.Token
                };
            }
            else
            {
                return new SqlConnection(connectionString);
            }
        }
    }
}
