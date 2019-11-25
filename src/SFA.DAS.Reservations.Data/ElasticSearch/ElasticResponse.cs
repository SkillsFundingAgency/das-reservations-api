using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public class ElasticResponse<T>
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public Hits<T> hits { get; set; }
        public Aggregations aggregations { get; set; }

        public ICollection<T> Items => hits?.hits?.Select(h => h._source).ToList() ?? new List<T>();
    }

    public class ElasticCountResponse
    {
        public int count { get; set; }
        public Shards _shards { get; set; }
    }

    public class Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class Hits<T>
    {
        public Total total { get; set; }
        public object max_score { get; set; }
        public List<Hit<T>> hits { get; set; }
    }

    public class Total
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class Hit<T>
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public object _score { get; set; }
        public T _source { get; set; }
        public List<string> sort { get; set; }
    }

    public class Aggregations
    {
        public AggregationsTerm uniqueCourseDescription { get; set; }
        public AggregationsTerm uniqueAccountLegalEntityName { get; set; }
        public AggregationsTerm uniqueReservationPeriod { get; set; }
    }

    public class AggregationsTerm
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

    public class Bucket
    {
        public string key { get; set; }
        public int doc_count { get; set; }
    }
}
