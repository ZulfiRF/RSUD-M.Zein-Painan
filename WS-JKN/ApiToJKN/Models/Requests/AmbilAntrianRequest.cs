using Newtonsoft.Json;

namespace ApiToJKN.Models.Requests
{
    public class AmbilAntrianRequest
    {
        [JsonProperty("nomorkartu")]
        public string NomorKartu { get; set; }
        [JsonProperty("nik")]
        public string Nik { get; set; }
        [JsonProperty("nohp")]
        public string NoHp { get; set; }
        [JsonProperty("kodepoli")]
        public string KodePoli { get; set; }
        [JsonProperty("norm")]
        public string NoRm { get; set; }
        [JsonProperty("tanggalperiksa")]
        public string TanggalPeriksa { get; set; }
        [JsonProperty("kodedokter")]
        public string KodeDokter { get; set; }
        [JsonProperty("jampraktek")]
        public string JamPraktek { get; set; }
        [JsonProperty("jeniskunjungan")]
        public int JenisKunjungan { get; set; }
        [JsonProperty("nomorreferensi")]
        public string NomorReferensi { get; set; }

    }
}