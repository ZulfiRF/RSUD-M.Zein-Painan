using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class SisaAntrianRespone
    {
        [JsonProperty(PropertyName = "response")]
        public SisaAntrianDetail Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }
}