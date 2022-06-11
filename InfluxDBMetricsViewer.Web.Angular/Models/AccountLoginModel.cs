using Newtonsoft.Json;

namespace MetaMetricsViewer.Web.Angular.Models
{
    public class AccountLoginModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}