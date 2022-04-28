using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class AmbilAntrianDetail
    {
        [JsonProperty("nomorantrean")]
        public string NomorAntrian { get; set; }
        [JsonProperty("angkaantrean")]
        public long AngkaAntrian { get; set; }
        [JsonProperty("kodebooking")]
        public string KodeBooking { get; set; }
        [JsonProperty("pasienbaru")]
        public int PasienBaru { get; set; }
        [JsonProperty("norm")]
        public string NoRm { get; set; }
        [JsonProperty("namapoli")]
        public string NamaPoli { get; set; }
        [JsonProperty("namadokter")]
        public string NamaDokter { get; set; }
        [JsonProperty("estimasidilayani")]
        public long EstimasiDilayani { get; set; }
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