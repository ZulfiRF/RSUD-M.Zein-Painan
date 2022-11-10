using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class SisaAntrianDetail
    {
        [JsonProperty("nomorantrean")]
        public string NomorAntrian { get; set; }
        [JsonProperty("namapoli")]
        public string NamaPoli { get; set; }
        [JsonProperty("namadokter")]
        public string NamaDokter { get; set; }
        [JsonProperty("sisaantrean")]
        public int SisaAntrian { get; set; }
        [JsonProperty("antreanpanggil")]
        public string AntrianDipanggil { get; set; }
        [JsonProperty("waktutunggu")]
        public int WaktuTunggu { get; set; }
        [JsonProperty("keterangan")]
        public string Keterangan { get; set; }
    }
}