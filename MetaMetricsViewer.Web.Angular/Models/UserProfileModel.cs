using Newtonsoft.Json;

namespace MetaMetricsViewer.Web.Angular.Models
{
    public class UserProfileModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; set; }
    }
}