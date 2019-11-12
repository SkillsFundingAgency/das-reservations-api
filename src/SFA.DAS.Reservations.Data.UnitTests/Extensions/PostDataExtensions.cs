using System.IO;
using System.Text;
using Elasticsearch.Net;

namespace SFA.DAS.Reservations.Data.UnitTests.ElasticSearch
{
    public static class PostDataExtensions
    {
        public static string GetRequestString(this PostData pd)
        {
            using (var stream = new MemoryStream())
            {
                pd.Write(stream, new ConnectionConfiguration());
            }

            return Encoding.UTF8.GetString(pd.WrittenBytes);
        }
    }
}
