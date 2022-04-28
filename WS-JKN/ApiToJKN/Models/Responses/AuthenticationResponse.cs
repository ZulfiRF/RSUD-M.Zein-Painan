using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToJKN.Models.Responses
{
    public class AuthenticationResponse
    {
        [JsonProperty(PropertyName = "response")]
        public Response Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
       
    }
    public class SlotingResponse
    {
        [JsonProperty(PropertyName = "response")]
        public ResponseChekSloting Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    
    }
    public class responEror
    {
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }

    }
    public class responOk
    {
        [JsonProperty(PropertyName = "response")]
        public Response Response { get; set; }

    }
    public class GetJadwalOprasiResponse
    {

        [JsonProperty(PropertyName = "response")]
        public object Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }
    public class GetJadwalOprasiResponseAll
    {

        [JsonProperty(PropertyName = "response")]
        public object Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }
    public class ListJadwalOprasiResponse
    {
        [JsonProperty(PropertyName = "list")]
        public List<JadwalOprasiResponse> List { get; set; }
    }
    public class ListJadwalOprasiResponseAll
    {
        [JsonProperty(PropertyName = "list")]
        public List<JadwalOprasiResponseall> List { get; set; }
    }
    public class GetNoAntrianResponse
    {
        [JsonProperty(PropertyName = "response")]
        public DaftarAntrean Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }

    }
    public class Metadata
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }
    }

    public class Response
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        
    }
    public class ResponseChekSloting
    {
        [JsonProperty(PropertyName = "namaPoli")]
        public string NamaPoli { get; set; }
        [JsonProperty(PropertyName = "totalantrean")]
        public int TotalAntrean { get; set; }
        [JsonProperty(PropertyName = "jumlahterlayani")]
        public Int32 JumlahTerlayani { get; set; }
        public Int64 lastupdate { get; set; }
    }

    public class DaftarAntrean
    {
        [JsonProperty(PropertyName = "nomorantrean")]
        public string NomorAntrean { get; set; }
        [JsonProperty(PropertyName = "kodebooking")]
        public string KodeBooking { get; set; }
        [JsonProperty(PropertyName = "jenisantrean")]
        public Int32 JenisAntrean { get; set; }
        [JsonProperty(PropertyName = "estimasidilayani")]
        
        public Int64 estimasidilayani { get; set; }
        [JsonProperty(PropertyName = "namapoli")]
        public string NamaPoli { get; set; }
        [JsonProperty(PropertyName = "namadokter")]
        public string NamaDokter { get; set; }
     
    }

    public class TokenResponse
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string password { get; set; }
       
    }
    public class JadwalOprasiResponseall
    {
        [JsonProperty(PropertyName = "kodebooking")]
        public string KodeBooking { get; set; }
        [JsonProperty(PropertyName = "tanggaloperasi")]
        public string TanggalOprasi { get; set; }
        [JsonProperty(PropertyName = "jenistindakan")]
        public string JenisTindakan { get; set; }
        [JsonProperty(PropertyName = "kodepoli")]
        public string KodePoli { get; set; }
        [JsonProperty(PropertyName = "namapoli")]
        public string NamaPoli { get; set; }
        [JsonProperty(PropertyName = "terlaksana")]
        public Int32 Terlaksana { get; set; }
        [JsonProperty(PropertyName = "nopeserta")]
        public string NoPeserta { get; set; }
        [JsonProperty(PropertyName = "lastupdate")]
        public Int64 LastUpdate { get; set; }

    }
    public class JadwalOprasiResponse
    {
        [JsonProperty(PropertyName = "kodebooking")]
        public string KodeBooking { get; set; }
        [JsonProperty(PropertyName = "tanggaloperasi")]
        public string TanggalOprasi { get; set; }
        [JsonProperty(PropertyName = "jenistindakan")]
        public string JenisTindakan { get; set; }
        [JsonProperty(PropertyName = "kodepoli")]
        public string KodePoli { get; set; }
        [JsonProperty(PropertyName = "namapoli")]
        public string NamaPoli { get; set; }
        [JsonProperty(PropertyName = "terlaksana")]
        public Int32 Terlaksana { get; set; }
    }
}