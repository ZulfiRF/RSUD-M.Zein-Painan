using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToJKN.Models.Requests
{
    public class PasienBaruRequest
    {
        [JsonProperty("nomorkartu")]
        public string NomorKartu { get; set; }
        [JsonProperty("nik")]
        public string Nik { get; set; }
        [JsonProperty("nomorkk")]
        public string NomorKk { get; set; }
        [JsonProperty("nama")]
        public string Nama { get; set; }
        [JsonProperty("jeniskelamin")]
        public string JenisKelamin { get; set; }
        [JsonProperty("tanggallahir")]
        public string TanggalLahir { get; set; }
        [JsonProperty("nohp")]
        public string NoHp { get; set; }
        [JsonProperty("alamat")]
        public string Alamat { get; set; }
        [JsonProperty("kodeprop")]
        public string KodeProp { get; set; }
        [JsonProperty("namaprop")]
        public string NamaProp { get; set; }
        [JsonProperty("kodedati2")]
        public string KodeDati2 { get; set; }
        [JsonProperty("namadati2")]
        public string NamaDati2 { get; set; }
        [JsonProperty("kodekec")]
        public string KodeKec { get; set; }
        [JsonProperty("namakec")]
        public string NamaKec { get; set; }
        [JsonProperty("kodekel")]
        public string KodeKel { get; set; }
        [JsonProperty("namakel")]
        public string NamaKel { get; set; }
        [JsonProperty("rw")]
        public string Rw { get; set; }
        [JsonProperty("rt")]
        public string Rt { get; set; }
    }
}