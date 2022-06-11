using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MetaMetrics.Api
{
    public class MetaMetricsTime4LinesDTO
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("table")]
        public int Table { get; set; }

        [JsonProperty("_start")]
        public DateTime Start { get; set; }

        [JsonProperty("_stop")]
        public DateTime Stop { get; set; }

        [JsonProperty("_time")]
        public string Time { get; set; }

        [JsonProperty("_value")]
        public double Value { get; set; }

        [JsonProperty("_field")]
        public string Field { get; set; }

        [JsonProperty("_measurement")]
        public string Measurement { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("env")]
        public string Env { get; set; }

        [JsonProperty("license")]
        public string License { get; set; }

        [JsonProperty("mtype")]
        public string Mtype { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("sublicense")]
        public string Sublicense { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
        public string Name { get; set; }

        public MetaMetricsTimeLineDto MetaMetricsTimeLine { get; set; } = new MetaMetricsTimeLineDto();
        public List<MetaMetricsTimeLineDto> TimeLines { get; set; } = new List<MetaMetricsTimeLineDto>();
    }
}
