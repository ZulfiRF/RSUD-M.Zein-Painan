using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class PasienBaruRespone
    {
        [JsonProperty(PropertyName = "response")]
        public PasienBaruDetail Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }
}