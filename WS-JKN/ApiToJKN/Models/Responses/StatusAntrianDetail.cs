using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class StatusAntrianDetail
    {
        [JsonProperty("namapoli")]
        public string NamaPoli { get; set; }
        [JsonProperty("namadokter")]
        public string NamaDokter { get; set; }
        [JsonProperty("totalantrean")]
        public int TotalAntrian { get; set; }
        [JsonProperty("sisaantrean")]
        public int SisaAntrian { get; set; }
        [JsonProperty("antreanpanggil")]
        public string AntrianDipanggil { get; set; }
        [JsonProperty("sisakuotajkn")]
        public int SisaKuotaJkn { get; set; }
        [JsonProperty("kuotajkn")]
        public int KuotaJkn { get; set; }
        [JsonProperty("sisakuotanonjkn")]
        public int SisaKuotaNonJkn { get; set; }
        [JsonProperty("kuotanonjkn")]
        public int KuotaNonJkn { get; set; }
        [JsonProperty("keterangan")]
        public string Keterangan { get; set; }
    }
}