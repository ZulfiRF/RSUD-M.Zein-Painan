using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class PasienBaruDetail
    {
        [JsonProperty("norm")]
        public string NoRm { get; set; }        
    }
}