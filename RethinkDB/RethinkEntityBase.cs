using Newtonsoft.Json;

namespace ECS.Framework.Data.RethinkDB
{
    public class RethinkEntityBase
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}