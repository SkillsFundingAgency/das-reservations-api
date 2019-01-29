using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Infrastructure.Configuration
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        private readonly string _environment;
        private readonly string _version;
        private readonly string _appName;
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageConfigurationProvider(string connection,string appName, string environment, string version)
        {
            _environment = environment;
            _version = version;
            _appName = appName;
            _storageAccount = CloudStorageAccount.Parse(connection);
        }

        public override void Load()
        {
            var table = GetTable();
            var operation = GetOperation(_appName, _environment, _version);
            var result = table.ExecuteAsync(operation).Result;

            var configItem = (ConfigurationItem)result.Result;

            var jsonObject = JObject.Parse(configItem.Data);

            foreach (var child in jsonObject.Children())
            {
                foreach (var jToken in child.Children().Children())
                {
                    var child1 = (JProperty)jToken;
                    Data.Add($"{child.Path}:{child1.Name}", child1.Value.ToString());
                }
            }
        }

        private CloudTable GetTable()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("Configuration");
        }

        private TableOperation GetOperation(string serviceName, string environmentName, string version)
        {
            return TableOperation.Retrieve<ConfigurationItem>(environmentName, $"{serviceName}_{version}");
        }
    }
}
