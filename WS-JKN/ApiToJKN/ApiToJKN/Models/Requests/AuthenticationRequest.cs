using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToJKN.Models.Requests
{
    public class AuthenticationRequest
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }

    public class ChekSlotingReques
    {
        [JsonProperty(PropertyName = "tanggalperiksa")]
        public string TanggalPeriksa { get; set; }
        [JsonProperty(PropertyName = "kodepoli")]
        public string KodePoli { get; set; }
        [JsonProperty(PropertyName = "polieksekutif")]
        public string PoliEksekutif { get; set; }
        
    }


    public class DaftarAntean
    {  [JsonProperty(PropertyName = "nomorkartu")]
        public string NoKartu { get; set; }
        [JsonProperty(PropertyName = "nik")]
        public string Nik { get; set; }
        [JsonProperty(PropertyName = "nomorrm")]
        public string NomorRm { get; set; }
        [JsonProperty(PropertyName = "notelp")]
        public string NoTelp { get; set; }
        [JsonProperty(PropertyName = "tanggalperiksa")]
        public string TanggalPeriksa { get; set; }
        [JsonProperty(PropertyName = "kodepoli")]
        public string KodePoli { get; set; }
        [JsonProperty(PropertyName = "nomorreferensi")]
        public string NomorReferensi { get; set; }
        [JsonProperty(PropertyName = "jenisreferensi")]
        public string JenisReferensi { get; set; }
        [JsonProperty(PropertyName = "jenisrequest")]
        public string JenisRequest { get; set; }
        [JsonProperty(PropertyName = "polieksekutif")]
        public string PoliEksekutif { get; set; }
        
    }

    public class getJadwalOprasi
    {
        [JsonProperty(PropertyName = "nopeserta")]
        public string NoPeserta { get; set; }
    }


    public class getAllJadwalOprasi
    {
        [JsonProperty(PropertyName = "tanggalawal")]
        public string TglAwal { get; set; }
        [JsonProperty(PropertyName = "tanggalakhir")]
        public string TglAkhir { get; set; }
    }
}