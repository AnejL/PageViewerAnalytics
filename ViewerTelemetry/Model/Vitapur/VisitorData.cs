using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ViewerTelemetry.Model.Contracts;

namespace ViewerTelemetry.Model.Vitapur
{
    public class VisitorData : ITelemetryData
    {
        [JsonPropertyName("visitors")]
        public int Visitors { get; set; }
    }
}
