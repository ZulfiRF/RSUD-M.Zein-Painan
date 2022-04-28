using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Jasamedika.Sdk.Vclaim.Attr;
using Jasamedika.Sdk.Vclaim.Event;

namespace Jasamedika.Sdk.Vclaim
{
    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
    public class ContextVclaim
    {
        #region Event

        public event EventHandler<LogEvent> LogLoad;
        public event EventHandler<XcodeEvent> XcodeLoad;

        protected virtual void OnLogLoad(LogEvent e)
        {
            LogLoad?.Invoke(this, e);
        }

        protected virtual void OnXcodeLoad(XcodeEvent e)
        {
            XcodeLoad?.Invoke(this, e);
        }

        #endregion

        #region Properties

        public string Url { get; set; }
        public string ConsumerId { get; set; }
        public string PasswordKey { get; set; }
        public string DefaultTime { get; set; }
        public int Tahun { get; set; }
        public int Bulan { get; set; }
        public int Hari { get; set; }
        public int Jam { get; set; }
        public int Menit { get; set; }
        public int Detik { get; set; }
        public int IsByPassSsl { get; set; }
        public string Send { get; set; }
        public string Receive { get; set; }
        public string UrlFinal { get; set; }

        #endregion

        #region Constructor

        public ContextVclaim()
        {
            Tahun = 1970;
            Bulan = 1;
            Hari = 1;
            Jam = 0;
            Menit = 0;
            Detik = 0;
            IsByPassSsl = 1;
        }

        #endregion

        #region 1. Pembuatan SEP

        #region 1.1. Cari Sep

        [PropertyLabel("No SEP", "nosep", "0", typeof(string))]
        public string[] CariSep(string noSep)
        {
            try
            {
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                var url = Url + "SEP/" + noSep;
                UrlFinal = url;
                Send = "";
                var result = client.DownloadString(url);
                OnLogLoad(new LogEvent("Receive:" + result));
                Receive = "Receive:" + result;
                //var result = "{\"metaData\":{\"code\":\"200\",\"message\":\"Sukses\"},\"response\":{\"catatan\":\"test\",\"diagnosa\":\"Cholera due to Vibrio cholerae 01, biovar eltor\",\"jnsPelayanan\":\"Rawat INap\",\"kelasRawat\":\"1\",\"noSep\":\"0301R0011017V000015\",\"penjamin\":null,\"peserta\":{\"asuransi\":null,\"hakKelas\":\"Kelas 2\",\"jnsPeserta\":\"PNS Pusat\",\"kelamin\":\"L\",\"nama\":\"SRI MULYONO\",\"noKartu\":\"0001267311161\",\"noMr\":\"123456\",\"tglLahir\":\"1982 - 01 - 05\"},\"poli\":\"Poli Penyakit Dalam\",\"poliEksekutif\":\"0\",\"tglSep\":\"2017 - 10 - 30\"}}";
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            var listResult = new List<string>();
                            JToken source = objResult["response"];
                            foreach (JProperty property in source.OfType<JProperty>())
                            {
                                JToken item = property.Value;
                                if (item is JArray)
                                {
                                    JArray array = item as JArray;
                                    foreach (JToken token4 in array)
                                    {
                                        MappingResult(listResult, token4);
                                    }
                                }
                                else if (item is JValue)
                                {
                                    var val = item as JValue;
                                    if (val != null)
                                    {
                                        var keyValue = property.Name;
                                        var isi = val.Value;
                                        listResult.Add(keyValue + ":" + isi);
                                    }
                                }
                                else
                                {
                                    MappingResult(listResult, item);
                                }
                            }
                            return listResult.ToArray();
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 1.2. Delete Sep

        [PropertyLabel("No SEP", "nosep", "0", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] DeleteSep(
            string noSep,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                OnLogLoad(new LogEvent("Send:" + json));
                Send = "Send:" + json;
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "SEP/Delete";
                UrlFinal = url;
                var result = client.UploadString(url, "DELETE", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 1.3. Update Sep

        [PropertyLabel("No SEP", "nosep", "0", typeof(string))]
        [PropertyLabel("Kelas Rawat", "klsRawat", "0", typeof(string))]
        [PropertyLabel("No. CM", "noCm", "0", typeof(string))]
        [PropertyLabel("Asal Rujukan", "asalRujukan", "0", typeof(string))]
        [PropertyLabel("Tanggal Rujukan", "tglRujukan", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        [PropertyLabel("Kode PPK Rujukan", "ppkRujukan", "0", typeof(string))]
        [PropertyLabel("Catatan", "catatan", "-", typeof(string))]
        [PropertyLabel("Diagnosa Awal", "diagAwal", "A00", typeof(string))]
        [PropertyLabel("Eksekutif", "eksekutif", "0", typeof(string))]
        [PropertyLabel("COB", "cob", "0", typeof(string))]
        [PropertyLabel("Lakalantas", "lakaLantas", "0", typeof(string))]
        [PropertyLabel("Penjamin", "penjamin", "0", typeof(string))]
        [PropertyLabel("Lokasi Lakalantas", "lokasiLaka", "-", typeof(string))]
        [PropertyLabel("No. Telpon", "noTelp", "0", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] UpdateSep(
            string noSep,
            string klsRawat,
            string noCm,
            string asalRujukan,
            string tglRujukan,
            string noRujukan,
            string ppkRujukan,
            string catatan,
            string diagAwal,
            string eksekutif,
            string cob,
            string lakaLantas,
            string penjamin,
            string lokasiLaka,
            string noTelp,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"klsRawat\":\"" + klsRawat + "\"," +
                           "\"noMR\":\"" + noCm + "\"," +
                           "\"rujukan\":{" +
                           "\"asalRujukan\":\"" + asalRujukan + "\"," +
                           "\"tglRujukan\":\"" + tglRujukan + "\"," +
                           "\"noRujukan\":\"" + noRujukan + "\"," +
                           "\"ppkRujukan\":\"" + ppkRujukan + "\"" +
                           "}," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagAwal\":\"" + diagAwal + "\"," +
                           "\"poli\":{" +
                           "\"eksekutif\":\"" + eksekutif + "\"" +
                           "}," +
                           "\"cob\":{" +
                           "\"cob\":\"" + cob + "\"" +
                           "}," +
                           "\"jaminan\":{" +
                           "\"lakaLantas\":\"" + lakaLantas + "\"," +
                           "\"penjamin\":\"" + penjamin + "\"," +
                           "\"lokasiLaka\":\"" + lokasiLaka + "\"" +
                           "}," +
                           "\"noTelp\":\"" + noTelp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                OnLogLoad(new LogEvent("Send:" + json));
                Send = "Send:" + json;
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "SEP/Update";
                UrlFinal = url;
                var result = client.UploadString(url, "PUT", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses                        
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        [PropertyLabel("No SEP", "nosep", "0", typeof(string))]
        [PropertyLabel("Kelas Rawat", "klsRawat", "0", typeof(string))]
        [PropertyLabel("No. CM", "noCm", "0", typeof(string))]
        [PropertyLabel("Asal Rujukan", "asalRujukan", "0", typeof(string))]
        [PropertyLabel("Tanggal Rujukan", "tglRujukan", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        [PropertyLabel("Kode PPK Rujukan", "ppkRujukan", "0", typeof(string))]
        [PropertyLabel("Catatan", "catatan", "-", typeof(string))]
        [PropertyLabel("Diagnosa Awal", "diagAwal", "A00", typeof(string))]
        [PropertyLabel("Eksekutif", "eksekutif", "0", typeof(string))]
        [PropertyLabel("COB", "cob", "0", typeof(string))]
        [PropertyLabel("Katarak {katarak --> 0.Tidak 1.Ya}", "katarak", "0", typeof(string))]
        [PropertyLabel("No Surat SKDP {Nomor Surat Kontrol}", "noSuratSkdp", "0", typeof(string))]
        [PropertyLabel("Kode DPJP SKDP {kode dokter DPJP --> baca di referensi dokter DPJP}", "kdDpjp", "0", typeof(string))]
        [PropertyLabel("Lakalantas Kecelakaan Lalu Lintas --> 0.Tidak 1.Ya", "lakaLantas", "0", typeof(string))]
        [PropertyLabel("Penjamin {penjamin lakalantas -> 1=Jasa raharja PT, 2=BPJS Ketenagakerjaan, 3=TASPEN PT, 4=ASABRI PT} jika lebih dari 1 isi -> 1,2 (pakai delimiter koma)}", "penjamin", "1", typeof(string))]
        [PropertyLabel("Tanggal Kejadian Laka {tanggal kejadian KLL format: yyyy-mm-dd}", "tglLakaLantas", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Keterangan Laka {Keterangan Kejadian KLL}", "keteranganLakaLantas", "-", typeof(string))]
        [PropertyLabel("Suplesi {Suplesi --> 0.Tidak 1. Ya}", "suplesi", "0", typeof(string))]
        [PropertyLabel("No SEP Suplesi {No.SEP yang Jika Terdapat Suplesi}", "noSepSuplesi", "0", typeof(string))]
        [PropertyLabel("Kode Propinsi Lokasi Lakalantas", "kdPropinsilokasiLaka", "03", typeof(string))]
        [PropertyLabel("Kode Kota Kabupaten Lokasi Lakalantas", "kdKabupatenlokasiLaka", "0050", typeof(string))]
        [PropertyLabel("Kode Kecamatan Lokasi Lakalantas", "kdKecamatanlokasiLaka", "0574", typeof(string))]
        [PropertyLabel("No. Telpon", "noTelp", "0", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] UpdateSepV1_1(
            string noSep,
            string klsRawat,
            string noCm,
            string asalRujukan,
            string tglRujukan,
            string noRujukan,
            string ppkRujukan,
            string catatan,
            string diagAwal,
            string eksekutif,
            string cob,
            string katarak,
            string noSuratSkdp,
            string kdDpjp,
            string lakaLantas,
            string penjamin,
            string tglLakaLantas,
            string keteranganLakaLantas,
            string suplesi,
            string noSepSuplesi,
            string kdPropinsilokasiLaka,
            string kdKabupatenlokasiLaka,
            string kdKecamatanlokasiLaka,
            string noTelp,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"klsRawat\":\"" + klsRawat + "\"," +
                           "\"noMR\":\"" + noCm + "\"," +
                           "\"rujukan\":{" +
                               "\"asalRujukan\":\"" + asalRujukan + "\"," +
                               "\"tglRujukan\":\"" + tglRujukan + "\"," +
                               "\"noRujukan\":\"" + noRujukan + "\"," +
                               "\"ppkRujukan\":\"" + ppkRujukan + "\"" +
                           "}," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagAwal\":\"" + diagAwal + "\"," +
                           "\"poli\":{" +
                               "\"eksekutif\":\"" + eksekutif + "\"" +
                           "}," +
                           "\"cob\":{" +
                               "\"cob\":\"" + cob + "\"" +
                           "}," +
                            "\"katarak\": {" +
                                "\"katarak\": \"" + katarak + "\"" +
                           "}," +
                           "\"skdp\": {" +
                                "\"noSurat\": \"" + noSuratSkdp + "\"," +
                                "\"kodeDPJP\": \"" + kdDpjp + "\"" +
                           "}," +
                            "\"jaminan\": {" +
                                "\"lakaLantas\": \"" + lakaLantas + "\"," +
                                "\"penjamin\": {" +
                                    "\"penjamin\": \"" + penjamin + "\"," +
                                    "\"tglKejadian\": \"" + tglLakaLantas + "\"," +
                                    "\"keterangan\": \"" + keteranganLakaLantas + "\"," +
                                    "\"suplesi\": {" +
                                        "\"suplesi\": \"" + suplesi + "\"," +
                                        "\"noSepSuplesi\": \"" + noSepSuplesi + "\"," +
                                        "\"lokasiLaka\": {" +
                                            "\"kdPropinsi\": \"" + kdPropinsilokasiLaka + "\"," +
                                            "\"kdKabupaten\": \"" + kdKabupatenlokasiLaka + "\"," +
                                            "\"kdKecamatan\": \"" + kdKecamatanlokasiLaka + "\"" +
                                            "}" +
                                        "}" +
                                    "}" +
                           "}," +
                           "\"noTelp\":\"" + noTelp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "SEP/1.1/Update";
                UrlFinal = url;
                var result = client.UploadString(url, "PUT", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses                        
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 1.4. Insert Sep

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tanggal SEP", "tglSep", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Kode PPK Pelayanan", "ppkPelayanan", "0", typeof(string))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Kelas Rawat", "klsRawat", "0", typeof(string))]
        [PropertyLabel("No. CM", "noCm", "0", typeof(string))]
        [PropertyLabel("Asal Rujukan", "asalRujukan", "0", typeof(string))]
        [PropertyLabel("Tanggal Rujukan", "tglRujukan", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        [PropertyLabel("Kode PPK Rujukan", "ppkRujukan", "0", typeof(string))]
        [PropertyLabel("Catatan", "catatan", "-", typeof(string))]
        [PropertyLabel("Diagnosa Awal", "diagAwal", "A00", typeof(string))]
        [PropertyLabel("Kode Poli Rujuan", "poliTujuan", "INT", typeof(string))]
        [PropertyLabel("Eksekutif", "eksekutif", "0", typeof(string))]
        [PropertyLabel("COB", "cob", "0", typeof(string))]
        [PropertyLabel("Lakalantas", "lakaLantas", "0", typeof(string))]
        [PropertyLabel("Penjamin", "penjamin", "0", typeof(string))]
        [PropertyLabel("Lokasi Lakalantas", "lokasiLaka", "-", typeof(string))]
        [PropertyLabel("No. Telpon", "noTelp", "0", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] InsertSep(
            string noKartu,
            string tglSep,
            string ppkPelayanan,
            string jnsPelayanan,
            string klsRawat,
            string noCm,
            string asalRujukan,
            string tglRujukan,
            string noRujukan,
            string ppkRujukan,
            string catatan,
            string diagAwal,
            string poliTujuan,
            string eksekutif,
            string cob,
            string lakaLantas,
            string penjamin,
            string lokasiLaka,
            string noTelp,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noKartu\":\"" + noKartu + "\"," +
                           "\"tglSep\":\"" + tglSep + "\"," +
                           "\"ppkPelayanan\":\"" + ppkPelayanan + "\"," +
                           "\"jnsPelayanan\":\"" + jnsPelayanan + "\"," +
                           "\"klsRawat\":\"" + klsRawat + "\"," +
                           "\"noMR\":\"" + noCm + "\"," +
                           "\"rujukan\":{" +
                           "\"asalRujukan\":\"" + asalRujukan + "\"," +
                           "\"tglRujukan\":\"" + tglRujukan + "\"," +
                           "\"noRujukan\":\"" + noRujukan + "\"," +
                           "\"ppkRujukan\":\"" + ppkRujukan + "\"" +
                           "}," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagAwal\":\"" + diagAwal + "\"," +
                           "\"poli\":{" +
                           "\"tujuan\":\"" + poliTujuan + "\"," +
                           "\"eksekutif\":\"" + eksekutif + "\"" +
                           "}," +
                           "\"cob\":{" +
                           "\"cob\":\"" + cob + "\"" +
                           "}," +
                           "\"jaminan\":{" +
                           "\"lakaLantas\":\"" + lakaLantas + "\"," +
                           "\"penjamin\":\"" + penjamin + "\"," +
                           "\"lokasiLaka\":\"" + lokasiLaka + "\"" +
                           "}," +
                           "\"noTelp\":\"" + noTelp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                var url = Url + "SEP/insert";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        return AutoMaps(objResult, "sep");
                    }
                    else
                    {
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        public string InsertSepJsonForm(
            string noKartu,
            string tglSep,
            string ppkPelayanan,
            string jnsPelayanan,
            string klsRawat,
            string noCm,
            string asalRujukan,
            string tglRujukan,
            string noRujukan,
            string ppkRujukan,
            string catatan,
            string diagAwal,
            string poliTujuan,
            string eksekutif,
            string cob,
            string lakaLantas,
            string penjamin,
            string lokasiLaka,
            string noTelp,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noKartu\":\"" + noKartu + "\"," +
                           "\"tglSep\":\"" + tglSep + "\"," +
                           "\"ppkPelayanan\":\"" + ppkPelayanan + "\"," +
                           "\"jnsPelayanan\":\"" + jnsPelayanan + "\"," +
                           "\"klsRawat\":\"" + klsRawat + "\"," +
                           "\"noMR\":\"" + noCm + "\"," +
                           "\"rujukan\":{" +
                           "\"asalRujukan\":\"" + asalRujukan + "\"," +
                           "\"tglRujukan\":\"" + tglRujukan + "\"," +
                           "\"noRujukan\":\"" + noRujukan + "\"," +
                           "\"ppkRujukan\":\"" + ppkRujukan + "\"" +
                           "}," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagAwal\":\"" + diagAwal + "\"," +
                           "\"poli\":{" +
                           "\"tujuan\":\"" + poliTujuan + "\"," +
                           "\"eksekutif\":\"" + eksekutif + "\"" +
                           "}," +
                           "\"cob\":{" +
                           "\"cob\":\"" + cob + "\"" +
                           "}," +
                           "\"jaminan\":{" +
                           "\"lakaLantas\":\"" + lakaLantas + "\"," +
                           "\"penjamin\":\"" + penjamin + "\"," +
                           "\"lokasiLaka\":\"" + lokasiLaka + "\"" +
                           "}," +
                           "\"noTelp\":\"" + noTelp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                var url = Url + "SEP/insert";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                return result;
            }
            catch (Exception e)
            {
                return "error:" + e.Message + " => " + e.GetBaseException();
            }
        }

        [PropertyLabel("No. Kartu {nokartu BPJS}", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tanggal SEP {tanggal penerbitan sep format yyyy-mm-dd}", "tglSep", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Kode PPK Pelayanan {kode faskes pemberi pelayanan}", "ppkPelayanan", "0", typeof(string))]
        [PropertyLabel("Jenis Pelayanan {jenis pelayanan = 1. r.inap 2. r.jalan}", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Kelas Rawat {kelas rawat 1. kelas 1, 2. kelas 2 3.kelas 3}", "klsRawat", "0", typeof(string))]
        [PropertyLabel("No. CM {nomor medical record RS}", "noCm", "0", typeof(string))]
        [PropertyLabel("Asal Rujukan {asal rujukan ->1.Faskes 1, 2. Faskes 2(RS)}", "asalRujukan", "0", typeof(string))]
        [PropertyLabel("Tanggal Rujukan {tanggal rujukan format: yyyy-mm-dd}", "tglRujukan", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("No. Rujukan {nomor rujukan}", "noRujukan", "0", typeof(string))]
        [PropertyLabel("Kode PPK Rujukan {kode faskes rujukam -> baca di referensi faskes}", "ppkRujukan", "0", typeof(string))]
        [PropertyLabel("Catatan {catatan peserta}", "catatan", "-", typeof(string))]
        [PropertyLabel("Diagnosa Awal {diagnosa awal ICD10 -> baca di referensi diagnosa}", "diagAwal", "A00", typeof(string))]
        [PropertyLabel("Kode Poli Rujuan {kode poli -> baca di referensi poli}", "poliTujuan", "INT", typeof(string))]
        [PropertyLabel("Eksekutif {poli eksekutif -> 0. Tidak 1.Ya}", "eksekutif", "0", typeof(string))]
        [PropertyLabel("COB {cob -> 0.Tidak 1. Ya}", "cob", "0", typeof(string))]
        [PropertyLabel("Katarak {katarak --> 0.Tidak 1.Ya}", "katarak", "0", typeof(string))]
        [PropertyLabel("Lakalantas Kecelakaan Lalu Lintas --> 0.Tidak 1.Ya", "lakaLantas", "0", typeof(string))]
        [PropertyLabel("Penjamin {penjamin lakalantas -> 1=Jasa raharja PT, 2=BPJS Ketenagakerjaan, 3=TASPEN PT, 4=ASABRI PT} jika lebih dari 1 isi -> 1,2 (pakai delimiter koma)}", "penjamin", "1", typeof(string))]
        [PropertyLabel("Tanggal Kejadian Laka {tanggal kejadian KLL format: yyyy-mm-dd}", "tglLakaLantas", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Keterangan Laka {Keterangan Kejadian KLL}", "keteranganLakaLantas", "-", typeof(string))]
        [PropertyLabel("Suplesi {Suplesi --> 0.Tidak 1. Ya}", "suplesi", "0", typeof(string))]
        [PropertyLabel("No SEP Suplesi {No.SEP yang Jika Terdapat Suplesi}", "noSepSuplesi", "0", typeof(string))]
        [PropertyLabel("Kode Propinsi Lokasi Lakalantas", "kdPropinsilokasiLaka", "03", typeof(string))]
        [PropertyLabel("Kode Kota Kabupaten Lokasi Lakalantas", "kdKabupatenlokasiLaka", "0050", typeof(string))]
        [PropertyLabel("Kode Kecamatan Lokasi Lakalantas", "kdKecamatanlokasiLaka", "0574", typeof(string))]
        [PropertyLabel("No Surat SKDP {Nomor Surat Kontrol}", "noSuratSkdp", "0", typeof(string))]
        [PropertyLabel("Kode DPJP SKDP {kode dokter DPJP --> baca di referensi dokter DPJP}", "kdDpjp", "0", typeof(string))]
        [PropertyLabel("No. Telpon", "noTelp", "0", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] InsertSepV1_1(
            string noKartu,
            string tglSep,
            string ppkPelayanan,
            string jnsPelayanan,
            string klsRawat,
            string noCm,
            string asalRujukan,
            string tglRujukan,
            string noRujukan,
            string ppkRujukan,
            string catatan,
            string diagAwal,
            string poliTujuan,
            string eksekutif,
            string cob,
            string katarak,
            string lakaLantas,
            string penjamin,
            string tglLakaLantas,
            string keteranganLakaLantas,
            string suplesi,
            string noSepSuplesi,
            string kdPropinsilokasiLaka,
            string kdKabupatenlokasiLaka,
            string kdKecamatanlokasiLaka,
            string noSuratSkdp,
            string kdDpjp,
            string noTelp,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noKartu\":\"" + noKartu + "\"," +
                           "\"tglSep\":\"" + tglSep + "\"," +
                           "\"ppkPelayanan\":\"" + ppkPelayanan + "\"," +
                           "\"jnsPelayanan\":\"" + jnsPelayanan + "\"," +
                           "\"klsRawat\":\"" + klsRawat + "\"," +
                           "\"noMR\":\"" + noCm + "\"," +
                           "\"rujukan\":{" +
                               "\"asalRujukan\":\"" + asalRujukan + "\"," +
                               "\"tglRujukan\":\"" + tglRujukan + "\"," +
                               "\"noRujukan\":\"" + noRujukan + "\"," +
                               "\"ppkRujukan\":\"" + ppkRujukan + "\"" +
                           "}," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagAwal\":\"" + diagAwal + "\"," +
                           "\"poli\":{" +
                               "\"tujuan\":\"" + poliTujuan + "\"," +
                               "\"eksekutif\":\"" + eksekutif + "\"" +
                           "}," +
                           "\"cob\":{" +
                               "\"cob\":\"" + cob + "\"" +
                           "}," +
                           "\"katarak\": {" +
                                "\"katarak\": \"" + katarak + "\"" +
                           "}," +
                           "\"jaminan\": {" +
                                "\"lakaLantas\": \"" + lakaLantas + "\"," +
                                "\"penjamin\": {" +
                                    "\"penjamin\": \"" + penjamin + "\"," +
                                    "\"tglKejadian\": \"" + tglLakaLantas + "\"," +
                                    "\"keterangan\": \"" + keteranganLakaLantas + "\"," +
                                    "\"suplesi\": {" +
                                        "\"suplesi\": \"" + suplesi + "\"," +
                                        "\"noSepSuplesi\": \"" + noSepSuplesi + "\"," +
                                        "\"lokasiLaka\": {" +
                                            "\"kdPropinsi\": \"" + kdPropinsilokasiLaka + "\"," +
                                            "\"kdKabupaten\": \"" + kdKabupatenlokasiLaka + "\"," +
                                            "\"kdKecamatan\": \"" + kdKecamatanlokasiLaka + "\"" +
                                            "}" +
                                        "}" +
                                    "}" +
                           "}," +
                           "\"skdp\": {" +
                                "\"noSurat\": \"" + noSuratSkdp + "\"," +
                                "\"kodeDPJP\": \"" + kdDpjp + "\"" +
                           "}," +
                           "\"noTelp\":\"" + noTelp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                var url = Url + "SEP/1.1/insert";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        return AutoMaps(objResult, "sep");
                    }
                    else
                    {
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 2. Approval Penjaminan SEP

        #region 2.1. Pengajuan

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tanggal SEP", "tglSep", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Keterangan", "keterangan", "-", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] PengajuanSep(
            string noKartu,
            string tglSep,
            string jnsPelayanan,
            string keterangan,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noKartu\":\"" + noKartu + "\"," +
                           "\"tglSep\":\"" + tglSep + "\"," +
                           "\"jnsPelayanan\":\"" + jnsPelayanan + "\"," +
                           "\"keterangan\":\"" + keterangan + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "SEP/pengajuanSEP";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                //var result = "{\"metadata\":{\"code\":\"200\",\"message\":\"OK\"},\"response\":\"0003814312013\"}";
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metadata"] as JToken;
                    if (arr == null)
                        arr = objResult["metaData"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 2.2. Aproval Pengajuan SEP

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tanggal SEP", "tglSep", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Keterangan", "keterangan", "-", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] AprovalPengajuanSep(
            string noKartu,
            string tglSep,
            string jnsPelayanan,
            string keterangan,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noKartu\":\"" + noKartu + "\"," +
                           "\"tglSep\":\"" + tglSep + "\"," +
                           "\"jnsPelayanan\":\"" + jnsPelayanan + "\"," +
                           "\"keterangan\":\"" + keterangan + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "SEP/aprovalSEP";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metadata"] as JToken;
                    if (arr == null)
                        arr = objResult["metaData"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 3. Update Tgl Pulang SEP

        [PropertyLabel("No. SEP", "noSep", "0", typeof(string))]
        [PropertyLabel("Tanggal Pulang", "tglPlg", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] UpdateTgPulangSep(
            string noSep,
            string tglPlg,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_sep\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"tglPulang\":\"" + tglPlg + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "SEP/updtglplg";
                UrlFinal = url;
                var result = client.UploadString(url, "PUT", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses                        
                        return new string[] { "sukses" };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }

                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 4. Peserta 

        #region 4.1. By No Kartu

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tanggal Pelayanan", "tglPelayanan", "2018-01-01", typeof(DateTime))]
        public string[] CariPesertaByNoKartuBpjs(string noKartu, string tglPelayanan)
        {
            try
            {
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                //client.Headers.Add("Content-Type", "application/json;");
                Send = "";
                var url = Url + "Peserta/nokartu/" + noKartu + "/tglSEP/" + tglPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                //var result = "{\"metaData\":{\"code\":\"200\",\"message\":\"OK\"},\"response\":{\"peserta\":{\"cob\":{\"nmAsuransi\":null,\"noAsuransi\":null,\"tglTAT\":null,\"tglTMT\":null},\"hakKelas\":{\"keterangan\":\"KELAS I\",\"kode\":\"1\"},\"informasi\":{\"dinsos\":null,\"noSKTM\":null,\"prolanisPRB\":null},\"jenisPeserta\":{\"keterangan\":\"PEGAWAI SWASTA\",\"kode\":\"13\"},\"mr\":{\"noMR\":null,\"noTelepon\":null},\"nama\":\"TRI M\",\"nik\":\"3319022010810007\",\"noKartu\":\"0011336526592\",\"pisa\":\"1\",\"provUmum\":{\"kdProvider\":\"0138U020\",\"nmProvider\":\"KPRJ PALA MEDIKA\"},\"sex\":\"L\",\"statusPeserta\":{\"keterangan\":\"AKTIF\",\"kode\":\"0\"},\"tglCetakKartu\":\"2016 - 02 - 12\",\"tglLahir\":\"1981 - 10 - 10\",\"tglTAT\":\"2014 - 12 - 31\",\"tglTMT\":\"2008 - 10 - 01\",\"umur\":{\"umurSaatPelayanan\":\"35 tahun ,1 bulan ,11 hari\",\"umurSekarang\":\"35 tahun ,2 bulan ,10 hari\"}}}}";
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        if (code != null)
                        {
                            var codeVal = code.Value.ToString();
                            if (codeVal.Contains("200"))
                            {
                                //jika sukses
                                return AutoMaps(objResult, "peserta");
                            }
                            else
                            {
                                //jika error
                                var msg = arr["message"] as JValue;
                                var msgVal = msg.Value.ToString();
                                return new[] { "error:" + msgVal };
                            }
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        public string CariPesertaByNoKartuBpjsJsonForm(string noKartu, string tglPelayanan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Peserta/nokartu/" + noKartu + "/tglSEP/" + tglPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                return result;
            }
            catch (Exception e)
            {
                return "error:" + e.Message + " => " + e.GetBaseException();
            }
        }

        #endregion

        #region 4.2. By NIK

        [PropertyLabel("NIK", "nik", "0", typeof(string))]
        [PropertyLabel("Tanggal Pelayanan", "tglPelayanan", "2018-01-01", typeof(DateTime))]
        public string[] CariPesertaByNik(string nik, string tglPelayanan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Peserta/nik/" + nik + "/tglSEP/" + tglPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                //var result = "{\"metaData\":{\"code\":\"200\",\"message\":\"OK\"},\"response\":{\"peserta\":{\"cob\":{\"nmAsuransi\":null,\"noAsuransi\":null,\"tglTAT\":null,\"tglTMT\":null},\"hakKelas\":{\"keterangan\":\"KELAS I\",\"kode\":\"1\"},\"informasi\":{\"dinsos\":null,\"noSKTM\":null,\"prolanisPRB\":null},\"jenisPeserta\":{\"keterangan\":\"PEGAWAI SWASTA\",\"kode\":\"13\"},\"mr\":{\"noMR\":null,\"noTelepon\":null},\"nama\":\"TRI M\",\"nik\":\"3319022010810007\",\"noKartu\":\"0011336526592\",\"pisa\":\"1\",\"provUmum\":{\"kdProvider\":\"0138U020\",\"nmProvider\":\"KPRJ PALA MEDIKA\"},\"sex\":\"L\",\"statusPeserta\":{\"keterangan\":\"AKTIF\",\"kode\":\"0\"},\"tglCetakKartu\":\"2016 - 02 - 12\",\"tglLahir\":\"1981 - 10 - 10\",\"tglTAT\":\"2014 - 12 - 31\",\"tglTMT\":\"2008 - 10 - 01\",\"umur\":{\"umurSaatPelayanan\":\"35 tahun ,1 bulan ,11 hari\",\"umurSekarang\":\"35 tahun ,2 bulan ,10 hari\"}}}}";
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "peserta");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        public string CariPesertaByNikJsonForm(string nik, string tglPelayanan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Peserta/nik/" + nik + "/tglSEP/" + tglPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                return result;
            }
            catch (Exception e)
            {
                return "error:" + e.Message + " => " + e.GetBaseException();
            }
        }

        #endregion

        #endregion

        #region 5. Rujukan

        #region 5.1. Rujukan PCare By Nomor Rujukan

        public string RujukanPcareByNoRujukanJsonForm(string noRujukan)
        {
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "Rujukan/" + noRujukan + "";
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        public string[] RujukanPcareByNoRujukan(string noRujukan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/" + noRujukan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 5.2. Rujukan RS By Nomor Rujukan

        public string RujukanRsByNoRujukanJsonForm(string noRujukan)
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "Rujukan/RS/" + noRujukan + "";
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        public string[] RujukanRsByNoRujukan(string noRujukan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/RS/" + noRujukan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 5.3. Rujukan PCare By Nomor Kartu

        public string RujukanPcareByNoKartuJsonForm(string noKartu)
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "Rujukan/Peserta/" + noKartu + "";
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        public string[] RujukanPcareByNoKartu(string noKartu)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/Peserta/" + noKartu + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 5.4. Rujukan RS By Nomor Kartu

        public string RujukanRsByNoKartuJsonForm(string noKartu)
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "Rujukan/RS/Peserta/" + noKartu + "";
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        public string[] RujukanRsByNoKartu(string noKartu)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/RS/Peserta/" + noKartu + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 5.5. Rujukan PCare By Nomor Kartu Multi Record

        [PropertyLabel("No. Kartu", "noKartu", "0", typeof(string))]
        public string[] RujukanPcareByNoKartuMultiRecord(string noKartu)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/List/Peserta/" + noKartu + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                //result ="{\"metaData\":{\"code\":\"200\",\"message\":\"OK\"},\"response\":{\"rujukan\":[{\"diagnosa\":{\"kode\":\"N40\",\"nama\":\"Hyperplasia of prostate\"},\"keluhan\":\"kencing tidak puas\",\"noKunjungan\":\"030107010217Y001465\",\"pelayanan\":{\"kode\":\"2\",\"nama\":\"Rawat Jalan\"},\"peserta\":{\"cob\":{\"nmAsuransi\":null,\"noAsuransi\":null,\"tglTAT\":null,\"tglTMT\":null},\"hakKelas\":{\"keterangan\":\"KELAS I\",\"kode\":\"1\"},\"informasi\":{\"dinsos\":null,\"noSKTM\":null,\"prolanisPRB\":null},\"jenisPeserta\":{\"keterangan\":\"PENERIMA PENSIUN PNS\",\"kode\":\"15\"},\"mr\":{\"noMR\":\"298036\",\"noTelepon\":null},\"nama\":\"MUSDIWAR,BA\",\"nik\":null,\"noKartu\":\"0000416382632\",\"pisa\":\"2\",\"provUmum\":{\"kdProvider\":\"03010701\",\"nmProvider\":\"SITEBA\"},\"sex\":\"L\",\"statusPeserta\":{\"keterangan\":\"AKTIF\",\"kode\":\"0\"},\"tglCetakKartu\":\"2017 - 11 - 13\",\"tglLahir\":\"1938 - 08 - 31\",\"tglTAT\":\"2038 - 08 - 31\",\"tglTMT\":\"1996 - 08 - 20\",\"umur\":{\"umurSaatPelayanan\":\"78 tahun ,6 bulan ,6 hari\",\"umurSekarang\":\"79 tahun ,3 bulan ,18 hari\"}},\"poliRujukan\":{\"kode\":\"URO\",\"nama\":\"UROLOGI\"},\"provPerujuk\":{\"kode\":\"03010701\",\"nama\":\"SITEBA\"},\"tglKunjungan\":\"2017 - 02 - 25\"}]}}";
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                            //return AutoMaps(objResult, "response");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 5.6. Rujukan PCare By Tgl Rujukan Multi Record

        [PropertyLabel("Tgl. Rujukan {yyyy-mm-dd}", "tglRujukan", "2018-01-01", typeof(DateTime))]
        public string[] RujukanPcareByTglRujukanMultiRecord(string tglRujukan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/List/TglRujukan/" + tglRujukan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 5.7. Rujukan RS By Tgl Rujukan Multi Record

        [PropertyLabel("Tgl. Rujukan {yyyy-mm-dd}", "tglRujukan", "2018-01-01", typeof(DateTime))]
        public string[] RujukanRsByTglRujukanMultiRecord(string tglRujukan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Rujukan/RS/List/TglRujukan/" + tglRujukan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "rujukan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 6. Pembuatan Rujukan

        #region 6.1. Insert Rujukan

        [PropertyLabel("No. SEP", "noSep", "0", typeof(string))]
        [PropertyLabel("Tanggal Rujukan", "tglRujukan", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Kode PPK Dirujuk", "ppkDirujuk", "0", typeof(string))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Catatan", "catatan", "-", typeof(string))]
        [PropertyLabel("Diagnosa Rujukan", "diagnosaRujukan", "A00", typeof(string))]
        [PropertyLabel("Tipe Rujukan", "tipeRujukan", "0", typeof(string))]
        [PropertyLabel("Poli Rujukan", "poliRujukan", "INT", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] InsertRujukan(
            string noSep,
            string tglRujukan,
            string ppkDirujuk,
            string jnsPelayanan,
            string catatan,
            string diagnosaRujukan,
            string tipeRujukan,
            string poliRujukan,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_rujukan\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"tglRujukan\":\"" + tglRujukan + "\"," +
                           "\"ppkDirujuk\":\"" + ppkDirujuk + "\"," +
                           "\"jnsPelayanan\":\"" + jnsPelayanan + "\"," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagRujukan\":\"" + diagnosaRujukan + "\"," +
                           "\"tipeRujukan\":\"" + tipeRujukan + "\"," +
                           "\"poliRujukan\":\"" + poliRujukan + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "Rujukan/insert";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                //var result = "{\"metaData\":{\"code\":\"200\",\"message\":\"OK\"},\"response\":null}";
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        return AutoMaps(objResult, "rujukan");
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }

                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 6.2. Update Rujukan

        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        [PropertyLabel("Kode PPK Dirujuk", "ppkDirujuk", "0", typeof(string))]
        [PropertyLabel("Tipe", "tipe", "0", typeof(string))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Catatan", "catatan", "-", typeof(string))]
        [PropertyLabel("Diagnosa Rujukan", "diagnosaRujukan", "A00", typeof(string))]
        [PropertyLabel("Tipe Rujukan", "tipeRujukan", "0", typeof(string))]
        [PropertyLabel("Poli Rujukan", "poliRujukan", "INT", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] UpdateRujukan(
            string noRujukan,
            string ppkDirujuk,
            string tipe,
            string jnsPelayanan,
            string catatan,
            string diagnosaRujukan,
            string tipeRujukan,
            string poliRujukan,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_rujukan\":{" +
                           "\"noRujukan\":\"" + noRujukan + "\"," +
                           "\"ppkDirujuk\":\"" + ppkDirujuk + "\"," +
                           "\"tipe\":\"" + tipe + "\"," +
                           "\"jnsPelayanan\":\"" + tipeRujukan + "\"," +
                           "\"catatan\":\"" + catatan + "\"," +
                           "\"diagRujukan\":\"" + diagnosaRujukan + "\"," +
                           "\"tipeRujukan\":\"" + tipeRujukan + "\"," +
                           "\"poliRujukan\":\"" + poliRujukan + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "Rujukan/update";
                UrlFinal = url;
                var result = client.UploadString(url, "PUT", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }

                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 6.3. Delete Rujukan

        [PropertyLabel("No. Rujukan", "noRujukan", "0", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] DeleteRujukan(
            string noRujukan,
            string user
            )
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_rujukan\":{" +
                           "\"noRujukan\":\"" + noRujukan + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "Rujukan/delete";
                UrlFinal = url;
                var result = client.UploadString(url, "DELETE", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }

                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 7. Lembar Pengajuan Klaim

        #region 7.1. Insert LPK

        [PropertyLabel("No. SEP", "noSep", "0", typeof(string))]
        [PropertyLabel("Tanggal Masuk", "tglMasuk", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Tanggal Keluar", "tglKeluar", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jaminan", "jaminan", "0", typeof(string))]
        [PropertyLabel("Poli", "poli", "INT", typeof(string))]
        [PropertyLabel("Ruang Rawat", "ruanganRawat", "-", typeof(string))]
        [PropertyLabel("Kelas Rawat", "kelasRawat", "0", typeof(string))]
        [PropertyLabel("Spesialistik", "spesialistik", "0", typeof(string))]
        [PropertyLabel("Cara Keluar", "caraKeluar", "0", typeof(string))]
        [PropertyLabel("Kondisi Pulang", "kondisiPulang", "0", typeof(string))]
        [PropertyLabel("Diagnosa", "diagnosas", "A01:1;Z00:1", typeof(string))]
        [PropertyLabel("Prosedur / Diagnosa Tindakan", "prosedur", "00.82;00.83", typeof(string))]
        [PropertyLabel("Tindak Lanjut", "tindakLanjut", "0", typeof(string))]
        [PropertyLabel("Kode PPK Dirujuk", "dirujukKeKdPpk", "0", typeof(string))]
        [PropertyLabel("Tanggal Kontrol", "tglKontrol", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Poli Kontrol", "poliKontrol", "INT", typeof(string))]
        [PropertyLabel("DPJP", "dpjp", "-", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] InsertLpk(
            string noSep,
            string tglMasuk,
            string tglKeluar,
            string jaminan,
            string poli,
            string ruanganRawat,
            string kelasRawat,
            string spesialistik,
            string caraKeluar,
            string kondisiPulang,
            string diagnosas, //A01:1;Z00:1;
            string prosedur, //00.82;00.83
            string tindakLanjut,
            string dirujukKeKdPpk,
            string tglKontrol,
            string poliKontrol,
            string dpjp,
            string user
            )
        {
            try
            {
                #region Split Diagnosa

                var listDiagnosa = new List<string>();
                var diagnosa = diagnosas.Split(';');
                foreach (var diag in diagnosa)
                {
                    if (!string.IsNullOrEmpty(diag))
                    {
                        listDiagnosa.Add(diag);
                    }
                }
                var i = 0;
                var jsonDiagnosa = "[";
                foreach (var diag in listDiagnosa)
                {
                    i++;
                    var join = "";
                    var split = diag.Split(':');
                    var kdDiag = split[0];
                    var lvlDiag = split[1];
                    join = "{" +
                           "\"kode\":\"" + kdDiag + "\"," +
                           "\"level\":\"" + lvlDiag + "\"" +
                           "}";
                    jsonDiagnosa = jsonDiagnosa + join;
                    if (i < listDiagnosa.Count)
                    {
                        jsonDiagnosa = jsonDiagnosa + ",";
                    }
                }
                jsonDiagnosa = jsonDiagnosa + "]";

                #endregion

                #region Split Prosedur

                var listProsedur = new List<string>();
                var prosedurs = prosedur.Split(';');
                foreach (var pro in prosedurs)
                {
                    if (!string.IsNullOrEmpty(pro))
                    {
                        listProsedur.Add(pro);
                    }
                }
                var x = 0;
                var jsonProsedur = "[";
                foreach (var kdPro in listProsedur)
                {
                    x++;
                    var join = "{" +
                               "\"kode\":\"" + kdPro + "\"" +
                               "}";
                    jsonProsedur = jsonProsedur + join;
                    if (x < listProsedur.Count)
                    {
                        jsonProsedur = jsonProsedur + ",";
                    }
                }
                jsonProsedur = jsonProsedur + "]";

                #endregion

                var json = "{" +
                           "\"request\":{" +
                           "\"t_lpk\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"tglMasuk\":\"" + tglMasuk + "\"," +
                           "\"tglKeluar\":\"" + tglKeluar + "\"," +
                           "\"jaminan\":\"" + jaminan + "\"," +
                           "\"poli\":{" +
                           "\"poli\":\"" + poli + "\"" +
                           "}," +
                           "\"perawatan\":{" +
                           "\"ruangRawat\":\"" + ruanganRawat + "\"," +
                           "\"kelasRawat\":\"" + kelasRawat + "\"," +
                           "\"spesialistik\":\"" + spesialistik + "\"," +
                           "\"caraKeluar\":\"" + caraKeluar + "\"," +
                           "\"kondisiPulang\":\"" + kondisiPulang + "\"" +
                           "}," +
                           "\"diagnosa\":" + jsonDiagnosa + "," +
                           "\"procedure\":" + jsonProsedur + "," +
                           "\"rencanaTL\":{" +
                           "\"tindakLanjut\":\"" + tindakLanjut + "\"," +
                           "\"dirujukKe\":{" +
                           "\"kodePPK\":\"" + dirujukKeKdPpk + "\"" +
                           "}," +
                           "\"kontrolKembali\":{" +
                           "\"tglKontrol\":\"" + tglKontrol + "\"," +
                           "\"poli\":\"" + poliKontrol + "\"" +
                           "}" +
                           "}," +
                           "\"DPJP\":\"" + dpjp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                OnLogLoad(new LogEvent("Send:" + json));
                Send = "Send:" + json;
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "LPK/insert";
                UrlFinal = url;
                var result = client.UploadString(url, "POST", json);
                OnLogLoad(new LogEvent("Receive:" + result));
                Receive = "Receive:" + json;
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 7.2. Update LPK

        [PropertyLabel("No. SEP", "noSep", "0", typeof(string))]
        [PropertyLabel("Tanggal Masuk", "tglMasuk", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Tanggal Keluar", "tglKeluar", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jaminan", "jaminan", "0", typeof(string))]
        [PropertyLabel("Poli", "poli", "INT", typeof(string))]
        [PropertyLabel("Ruang Rawat", "ruanganRawat", "-", typeof(string))]
        [PropertyLabel("Kelas Rawat", "kelasRawat", "0", typeof(string))]
        [PropertyLabel("Spesialistik", "spesialistik", "0", typeof(string))]
        [PropertyLabel("Cara Keluar", "caraKeluar", "0", typeof(string))]
        [PropertyLabel("Kondisi Pulang", "kondisiPulang", "0", typeof(string))]
        [PropertyLabel("Diagnosa", "diagnosas", "A01:1;Z00:1", typeof(string))]
        [PropertyLabel("Prosedur / Diagnosa Tindakan", "prosedur", "00.82;00.83", typeof(string))]
        [PropertyLabel("Tindak Lanjut", "tindakLanjut", "0", typeof(string))]
        [PropertyLabel("Kode PPK Dirujuk", "dirujukKeKdPpk", "0", typeof(string))]
        [PropertyLabel("Tanggal Kontrol", "tglKontrol", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Poli Kontrol", "poliKontrol", "INT", typeof(string))]
        [PropertyLabel("DPJP", "dpjp", "-", typeof(string))]
        [PropertyLabel("User", "user", "-", typeof(string))]
        public string[] UpdateLpk(
            string noSep,
            string tglMasuk,
            string tglKeluar,
            string jaminan,
            string poli,
            string ruanganRawat,
            string kelasRawat,
            string spesialistik,
            string caraKeluar,
            string kondisiPulang,
            string diagnosas, //A01:1;Z00:1;
            string prosedur, //00.82;00.83
            string tindakLanjut,
            string dirujukKeKdPpk,
            string tglKontrol,
            string poliKontrol,
            string dpjp,
            string user
            )
        {
            try
            {
                #region Split Diagnosa

                var listDiagnosa = new List<string>();
                var diagnosa = diagnosas.Split(';');
                foreach (var diag in diagnosa)
                {
                    if (!string.IsNullOrEmpty(diag))
                    {
                        listDiagnosa.Add(diag);
                    }
                }
                var i = 0;
                var jsonDiagnosa = "[";
                foreach (var diag in listDiagnosa)
                {
                    i++;
                    var join = "";
                    var split = diag.Split(':');
                    var kdDiag = split[0];
                    var lvlDiag = split[1];
                    join = "{" +
                           "\"kode\":\"" + kdDiag + "\"," +
                           "\"level\":\"" + lvlDiag + "\"" +
                           "}";
                    jsonDiagnosa = jsonDiagnosa + join;
                    if (i < listDiagnosa.Count)
                    {
                        jsonDiagnosa = jsonDiagnosa + ",";
                    }
                }
                jsonDiagnosa = jsonDiagnosa + "]";

                #endregion

                #region Split Prosedur

                var listProsedur = new List<string>();
                var prosedurs = prosedur.Split(';');
                foreach (var pro in prosedurs)
                {
                    if (!string.IsNullOrEmpty(pro))
                    {
                        listProsedur.Add(pro);
                    }
                }
                var x = 0;
                var jsonProsedur = "[";
                foreach (var kdPro in listProsedur)
                {
                    x++;
                    var join = "{" +
                               "\"kode\":\"" + kdPro + "\"" +
                               "}";
                    jsonProsedur = jsonProsedur + join;
                    if (x < listProsedur.Count)
                    {
                        jsonProsedur = jsonProsedur + ",";
                    }
                }
                jsonProsedur = jsonProsedur + "]";

                #endregion

                var json = "{" +
                           "\"request\":{" +
                           "\"t_lpk\":{" +
                           "\"noSep\":\"" + noSep + "\"," +
                           "\"tglMasuk\":\"" + tglMasuk + "\"," +
                           "\"tglKeluar\":\"" + tglKeluar + "\"," +
                           "\"jaminan\":\"" + jaminan + "\"," +
                           "\"poli\":{" +
                           "\"poli\":\"" + poli + "\"" +
                           "}," +
                           "\"perawatan\":{" +
                           "\"ruangRawat\":\"" + ruanganRawat + "\"," +
                           "\"kelasRawat\":\"" + kelasRawat + "\"," +
                           "\"spesialistik\":\"" + spesialistik + "\"," +
                           "\"caraKeluar\":\"" + caraKeluar + "\"," +
                           "\"kondisiPulang\":\"" + kondisiPulang + "\"" +
                           "}," +
                           "\"diagnosa\":" + jsonDiagnosa + "," +
                           "\"procedure\":" + jsonProsedur + "," +
                           "\"rencanaTL\":{" +
                           "\"tindakLanjut\":\"" + tindakLanjut + "\"," +
                           "\"dirujukKe\":{" +
                           "\"kodePPK\":\"" + dirujukKeKdPpk + "\"" +
                           "}," +
                           "\"kontrolKembali\":{" +
                           "\"tglKontrol\":\"" + tglKontrol + "\"," +
                           "\"poli\":\"" + poliKontrol + "\"" +
                           "}" +
                           "}," +
                           "\"DPJP\":\"" + dpjp + "\"," +
                           "\"user\":\"" + user + "\"" +
                           "}}}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "LPK/update";
                UrlFinal = url;
                var result = client.UploadString(url, "PUT", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 7.3. Delete LPK

        [PropertyLabel("No. SEP", "noSep", "0", typeof(string))]
        public string[] DeleteLpk(string noSep)
        {
            try
            {
                var json = "{" +
                           "\"request\":{" +
                           "\"t_lpk\":{" +
                           "\"noSep\":\"" + noSep + "\"" +
                           "}" +
                           "}" +
                           "}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
                //client.Headers.Add("Content-Type", "Application/json");
                var url = Url + "LPK/delete";
                UrlFinal = url;
                var result = client.UploadString(url, "DELETE", json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                var objResult = JsonConvert.DeserializeObject(result) as JObject;
                if (objResult != null)
                {
                    var arr = objResult["metaData"] as JToken;
                    if (arr == null)
                        arr = objResult["metadata"] as JToken;
                    var code = arr["code"] as JValue;
                    var codeVal = code.Value.ToString();
                    if (codeVal.Contains("200"))
                    {
                        //jika sukses
                        var response = objResult["response"] as JToken;
                        var responseVal = response.Value<string>();
                        return new[] { "Sukses:" + responseVal };
                    }
                    else
                    {
                        //jika error
                        var msg = arr["message"] as JValue;
                        var msgVal = msg.Value.ToString();
                        return new string[] { "error:" + msgVal };
                    }
                }

                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 7.4. Data Lembar Pengajuan Klaim

        [PropertyLabel("Tanggal Masuk", "tglMasuk", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        public string[] CariLpk(string tglMasuk, string jnsPelayanan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "LPK/TglMasuk/" + tglMasuk + "/JnsPelayanan/" + jnsPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            var listResult = new List<string>();
                            var source = objResult["response"]["lpk"]["list"];
                            var objects = source.OfType<JToken>();
                            foreach (var property in objects)
                            {
                                JToken item = property;
                                try
                                {
                                    if (item is JArray)
                                    {
                                        JArray array = item as JArray;
                                        foreach (JToken token4 in array)
                                        {
                                            MappingResultJToken(listResult, token4);
                                        }
                                        listResult.Add("=============");
                                    }
                                    else
                                    {
                                        MappingResultJToken(listResult, item);
                                        listResult.Add("=============");
                                    }
                                }
                                catch (Exception e)
                                {
                                    listResult.Add("error:" + e.Message);
                                }
                            }
                            return listResult.ToArray();
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 8. Monitoring

        #region 8.1 Data Kunjungan

        [PropertyLabel("Tanggal SEP", "tglSep", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        public string[] CariKunjungan(string tglSep, string jnsPelayanan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Monitoring/Kunjungan/Tanggal/" + tglSep + "/JnsPelayanan/" + jnsPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "sep");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 8.2 Data Klaim

        [PropertyLabel("Tanggal Pulang", "tglPulang", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Jenis Pelayanan", "jnsPelayanan", "0", typeof(string))]
        [PropertyLabel("Status Klaim", "statusKlaim", "0", typeof(string))]
        public string[] CariKlaim(string tglPulang, string jnsPelayanan, string statusKlaim)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "Monitoring/Klaim/Tanggal/" + tglPulang + "/JnsPelayanan/" + jnsPelayanan + "/Status/" +
                          statusKlaim + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "klaim");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 8.3 Data History Pelayanan Peserta

        [PropertyLabel("No Kartu Peserta", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tgl. Mulai Cari", "tglMulaiCari", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Tgl. Akhir Cari", "tglAkhirCari", "2018-10-10", typeof(DateTime))]
        public string[] CariHistoryPelayananPeserta(string noKartu, string tglMulaiCari, string tglAkhirCari)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "monitoring/HistoriPelayanan/NoKartu/" + noKartu + "/tglMulai/" + tglMulaiCari + "/tglAkhir/" + tglAkhirCari + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "histori");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 8.4 Data History Klaim Jaminan Jasa Raharja

        [PropertyLabel("Tgl. Mulai Cari", "tglMulaiCari", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Tgl. Akhir Cari", "tglAkhirCari", "2018-01-01", typeof(DateTime))]
        public string[] CariHistoryKlaimJaminanJasaRaharja(string tglMulaiCari, string tglAkhirCari)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "monitoring/JasaRaharja/tglMulai/" + tglMulaiCari + "/tglAkhir/" + tglAkhirCari + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "jaminan");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 9. Referensi

        #region 9.1. Diagnosa

        [PropertyLabel("Kode / Nama Diagnosa", "kdOrNmDiagnosa", "0", typeof(string))]
        public string[] RefDiagnosa(string kdOrNmDiagnosa)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/diagnosa/" + kdOrNmDiagnosa + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "diagnosa");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.2. Poli

        [PropertyLabel("Kode / Nama Poli", "kdOrNmPoli", "0", typeof(string))]
        public string[] RefPoli(string kdOrNmPoli)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/poli/" + kdOrNmPoli + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "poli");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.3. Fasilitas Kesehatan

        [PropertyLabel("Kode / Nama Faskes", "kdOrNmFaskes", "0", typeof(string))]
        [PropertyLabel("Jenis Faskes", "jnsFaskes", "0", typeof(string))]
        public string[] RefFasilitasKesehatan(string kdOrNmFaskes, string jnsFaskes)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/faskes/" + kdOrNmFaskes + "/" + jnsFaskes + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "faskes");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.4. Procedure / Tindakan

        [PropertyLabel("Kode / Nama Prosedur (Diagnosa Tindakan)", "kdOrNmProsedur", "0", typeof(string))]
        public string[] RefProsedur(string kdOrNmProsedur)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/procedure/" + kdOrNmProsedur + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "procedure");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.5. Kelas Rawat

        public string[] RefKelasRawat()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/kelasrawat";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.6. Dokter

        [PropertyLabel("Nama Dokter", "NmDokter", "0", typeof(string))]
        public string[] RefDokter(string NmDokter)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/dokter/" + NmDokter + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.7. Spesialistik

        public string[] RefSpesialistik()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/spesialistik";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.8. Ruang Rawat

        public string[] RefRuangRawat()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/ruangrawat";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.9. Cara Keluar

        public string[] RefCaraKeluar()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/carakeluar";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.10. Pasca Pulang

        public string[] RefPascaPulang()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/pascapulang";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.11. Dokter DPJP

        public string RefDokterDpjpJsonForm(string jnsPelayanan, string tglPelayanan, string kdSpesialisSubSpesialis)
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "referensi/dokter/pelayanan/" + jnsPelayanan + "/tglPelayanan/" + tglPelayanan + "/Spesialis/" + kdSpesialisSubSpesialis;
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("Jenis Pelayanan (1. Rawat Inap, 2. Rawat Jalan)", "jnsPelayanan", "2", typeof(string))]
        [PropertyLabel("Tgl. Pelayanan / SEP  (yyyy-mm-dd)", "tglPelayanan", "2018-01-01", typeof(DateTime))]
        [PropertyLabel("Kode Spesialis/Subspesialis", "kdSpesialisSubSpesialis", "0", typeof(string))]
        public string[] RefDokterDpjp(string jnsPelayanan, string tglPelayanan, string kdSpesialisSubSpesialis)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/dokter/pelayanan/" + jnsPelayanan + "/tglPelayanan/" + tglPelayanan + "/Spesialis/" + kdSpesialisSubSpesialis;
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.12. Propinsi

        public string RefPropinsiJsonForm()
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "referensi/propinsi";
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        public string[] RefPropinsi()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/propinsi";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.13. Kota Kabupaten

        public string RefKotaKabupatenJsonForm(string kdPropinsi)
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "referensi/kabupaten/propinsi/" + kdPropinsi;
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("Kode Propinsi", "kdPropinsi", "03", typeof(string))]
        public string[] RefKotaKabupaten(string kdPropinsi)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/kabupaten/propinsi/" + kdPropinsi;
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 9.14. Kecamatan

        public string RefKecamatanJsonForm(string kdKotaKabupaten)
        {
            Send = "";
            var client = CreateWebClient();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var url = Url + "referensi/kecamatan/kabupaten/" + kdKotaKabupaten;
            UrlFinal = url;
            var result = client.DownloadString(url);
            Receive = "Receive:" + result;
            OnLogLoad(new LogEvent("Receive:" + result));
            return result;
        }

        [PropertyLabel("Kode Propinsi", "kdPropinsi", "0227", typeof(string))]
        public string[] RefKecamatan(string kdKotaKabupaten)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var url = Url + "referensi/kecamatan/kabupaten/" + kdKotaKabupaten;
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 10. Ketersediaan Kamar

        #region 10.1 Referensi Kamar

        public string[] RefKamar()
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json");
                var url = Url + "aplicaresws/rest/ref/kelas";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metadata"] as JToken;
                        if (arr == null)
                            arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("1"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 10.2 Update Ketersediaan Tempat Tidur

        [PropertyLabel("Kode PPK", "kdPpk", "0", typeof(string))]
        [PropertyLabel("Kode Kelas", "kdKelas", "VIP", typeof(string))]
        [PropertyLabel("Kode Ruangan", "kdRuangan", "RG01", typeof(string))]
        [PropertyLabel("Nama Ruangan", "namaRuangan", "Ruang Anggrek VIP", typeof(string))]
        [PropertyLabel("Kapasitas", "kapasitas", "20", typeof(string))]
        [PropertyLabel("Tersedia", "tersedia", "10", typeof(string))]
        [PropertyLabel("Tersedia Pria", "tersediaPria", "0", typeof(string))]
        [PropertyLabel("Tersedia Wanita", "tersediaWanita", "0", typeof(string))]
        [PropertyLabel("Tersedia Pria Wanita", "tersediaPriaWanita", "0", typeof(string))]
        public string[] UpdateKetersediaanTempatTidur(
            string kdPpk,
            string kdKelas,
            string kdRuangan,
            string namaRuangan,
            string kapasitas,
            string tersedia,
            string tersediaPria,
            string tersediaWanita,
            string tersediaPriaWanita)
        {
            try
            {
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json");
                var url = Url + "aplicaresws/rest/bed/update/" + kdPpk;
                UrlFinal = url;
                var json = "{" +
                           "\"kodekelas\":\"" + kdKelas + "\"," +
                           "\"koderuang\":\"" + kdRuangan + "\"," +
                           "\"namaruang\":\"" + namaRuangan + "\"," +
                           "\"kapasitas\":\"" + kapasitas + "\"," +
                           "\"tersedia\":\"" + tersedia + "\"," +
                           "\"tersediapria\":\"" + tersediaPria + "\"," +
                           "\"tersediawanita\":\"" + tersediaWanita + "\"," +
                           "\"tersediapriawanita\":\"" + tersediaPriaWanita + "\"" +
                           "}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                var result = client.UploadString(url, json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metadata"] as JToken;
                        if (arr == null)
                            arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("1"))
                        {
                            var msgVal = "Sukses";
                            var msg = arr["message"] as JValue;
                            if (msg != null)
                            {
                                msgVal = msg.Value.ToString();
                            }
                            return new[] { msgVal };
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };

            }
        }

        #endregion

        #region 10.3 Ruangan Baru

        [PropertyLabel("Kode PPK", "kdPpk", "0", typeof(string))]
        [PropertyLabel("Kode Kelas", "kdKelas", "VIP", typeof(string))]
        [PropertyLabel("Kode Ruangan", "kdRuangan", "RG01", typeof(string))]
        [PropertyLabel("Nama Ruangan", "namaRuangan", "Ruang Anggrek VIP", typeof(string))]
        [PropertyLabel("Kapasitas", "kapasitas", "20", typeof(string))]
        [PropertyLabel("Tersedia", "tersedia", "10", typeof(string))]
        [PropertyLabel("Tersedia Pria", "tersediaPria", "0", typeof(string))]
        [PropertyLabel("Tersedia Wanita", "tersediaWanita", "0", typeof(string))]
        [PropertyLabel("Tersedia Pria Wanita", "tersediaPriaWanita", "0", typeof(string))]
        public string[] InsertRuanganKamarBaru(
            string kdPpk,
            string kdKelas,
            string kdRuangan,
            string namaRuangan,
            string kapasitas,
            string tersedia,
            string tersediaPria,
            string tersediaWanita,
            string tersediaPriaWanita)
        {
            try
            {
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json");
                var url = Url + "aplicaresws/rest/bed/create/" + kdPpk;
                var json = "{" +
                           "\"kodekelas\":\"" + kdKelas + "\"," +
                           "\"koderuang\":\"" + kdRuangan + "\"," +
                           "\"namaruang\":\"" + namaRuangan + "\"," +
                           "\"kapasitas\":\"" + kapasitas + "\"," +
                           "\"tersedia\":\"" + tersedia + "\"," +
                           "\"tersediapria\":\"" + tersediaPria + "\"," +
                           "\"tersediawanita\":\"" + tersediaPriaWanita + "\"," +
                           "\"tersediapriawanita\":\"" + tersediaPriaWanita + "\"" +
                           "}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                UrlFinal = url;
                var result = client.UploadString(url, json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metadata"] as JToken;
                        if (arr == null)
                            arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("1"))
                        {
                            var msgVal = "Sukses";
                            var msg = arr["message"] as JValue;
                            if (msg != null)
                            {
                                msgVal = msg.Value.ToString();
                            }
                            return new[] { msgVal };
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 10.4 Ketersediaan Kamar RS

        [PropertyLabel("Kode PPK", "kdPpk", "0", typeof(string))]
        [PropertyLabel("Start", "start", "1", typeof(string))]
        [PropertyLabel("Limit", "limit", "1", typeof(string))]
        public string[] KetersediaanKamarRs(
            string kdPpk,
            string start,
            string limit)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Accept", "application/json");
                var url = Url + "aplicaresws/rest/bed/read/" + kdPpk + "/" + start + "/" + limit;
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metadata"] as JToken;
                        if (arr == null)
                            arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("200"))
                        {
                            //jika sukses
                            return AutoMaps(objResult, "list");
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region 10.5 Hapus Ruangan

        [PropertyLabel("Kode PPK", "kdPpk", "0", typeof(string))]
        [PropertyLabel("Kode Kelas", "kdKelas", "VIP", typeof(string))]
        [PropertyLabel("Kode Ruangan", "kdRuangan", "RG01", typeof(string))]
        public string[] HapusRuanganKamar(
            string kdPpk,
            string kdKelas,
            string kdRuangan)
        {
            try
            {
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json");
                var url = Url + "aplicaresws/rest/bed/delete/" + kdPpk;
                var json = "{" +
                           "\"kodekelas\":\"" + kdKelas + "\"," +
                           "\"koderuang\":\"" + kdRuangan + "\"" +
                           "}";
                Send = "Send:" + json;
                OnLogLoad(new LogEvent("Send:" + json));
                UrlFinal = url;
                var result = client.UploadString(url, json);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metadata"] as JToken;
                        if (arr == null)
                            arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        var codeVal = code.Value.ToString();
                        if (codeVal.Contains("1"))
                        {
                            var msgVal = "Sukses";
                            var msg = arr["message"] as JValue;
                            if (msg != null)
                            {
                                msgVal = msg.Value.ToString();
                            }
                            return new[] { msgVal };
                        }
                        else
                        {
                            //jika error
                            var msg = arr["message"] as JValue;
                            var msgVal = msg.Value.ToString();
                            return new string[] { "error:" + msgVal };
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #endregion

        #region 11. Potensi Suplesi Jasa Raharja

        [PropertyLabel("No. Kartu Peserta", "noKartu", "0", typeof(string))]
        [PropertyLabel("Tanggal Pelayanan / SEP", "tglPelayanan", "2018-01-01", typeof(DateTime))]
        public string[] SuplesiJasaRaharja(string noKartu, string tglPelayanan)
        {
            try
            {
                Send = "";
                var client = CreateWebClient();
                client.Headers.Add("Content-Type", "application/json; charset=utf-8");
                //client.Headers.Add("Content-Type", "application/json;");
                var url = Url + "sep/JasaRaharja/Suplesi/" + noKartu + "/tglPelayanan/" + tglPelayanan + "";
                UrlFinal = url;
                var result = client.DownloadString(url);
                Receive = "Receive:" + result;
                OnLogLoad(new LogEvent("Receive:" + result));
                //var result = "{\"metaData\":{\"code\":\"200\",\"message\":\"OK\"},\"response\":{\"peserta\":{\"cob\":{\"nmAsuransi\":null,\"noAsuransi\":null,\"tglTAT\":null,\"tglTMT\":null},\"hakKelas\":{\"keterangan\":\"KELAS I\",\"kode\":\"1\"},\"informasi\":{\"dinsos\":null,\"noSKTM\":null,\"prolanisPRB\":null},\"jenisPeserta\":{\"keterangan\":\"PEGAWAI SWASTA\",\"kode\":\"13\"},\"mr\":{\"noMR\":null,\"noTelepon\":null},\"nama\":\"TRI M\",\"nik\":\"3319022010810007\",\"noKartu\":\"0011336526592\",\"pisa\":\"1\",\"provUmum\":{\"kdProvider\":\"0138U020\",\"nmProvider\":\"KPRJ PALA MEDIKA\"},\"sex\":\"L\",\"statusPeserta\":{\"keterangan\":\"AKTIF\",\"kode\":\"0\"},\"tglCetakKartu\":\"2016 - 02 - 12\",\"tglLahir\":\"1981 - 10 - 10\",\"tglTAT\":\"2014 - 12 - 31\",\"tglTMT\":\"2008 - 10 - 01\",\"umur\":{\"umurSaatPelayanan\":\"35 tahun ,1 bulan ,11 hari\",\"umurSekarang\":\"35 tahun ,2 bulan ,10 hari\"}}}}";
                if (result != null)
                {
                    var objResult = JsonConvert.DeserializeObject(result) as JObject;
                    if (objResult != null)
                    {
                        var arr = objResult["metaData"] as JToken;
                        var code = arr["code"] as JValue;
                        if (code != null)
                        {
                            var codeVal = code.Value.ToString();
                            if (codeVal.Contains("200"))
                            {
                                //jika sukses
                                return AutoMaps(objResult, "jaminan");
                            }
                            else
                            {
                                //jika error
                                var msg = arr["message"] as JValue;
                                var msgVal = msg.Value.ToString();
                                return new[] { "error:" + msgVal };
                            }
                        }
                    }
                }
                return new string[] { };
            }
            catch (Exception e)
            {
                return new string[] { "error:" + e.Message + " => " + e.GetBaseException() };
            }
        }

        #endregion

        #region Functions

        private string[] AutoMaps(JObject objResult, string tag)
        {
            var listResult = new List<string>();
            var respone = objResult["response"] as JToken;
            if (!respone.HasValues) return new[] { "OK" };
            var source = objResult["response"][tag];
            var objects = source.OfType<JToken>();
            foreach (var property in objects)
            {
                var item = property;
                try
                {
                    if (item is JArray)
                    {
                        JArray array = item as JArray;
                        foreach (JToken token4 in array)
                        {
                            MappingResultJToken(listResult, token4);
                        }
                        //listResult.Add("=============");
                    }
                    else
                    {
                        MappingResultJToken(listResult, item);
                        //listResult.Add("=============");
                    }
                }
                catch (Exception e)
                {
                    listResult.Add("error:" + e.Message);
                }
            }
            return listResult.ToArray();
        }

        private WebClient CreateWebClient()
        {
            var encoding = new UTF8Encoding();
            var str = "";
            var client = new WebClient();
            if (IsByPassSsl == 1)
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                    {
                        return true;
                    };
            DateTime time;
            if (!string.IsNullOrEmpty(DefaultTime))
            {
                var dateString = DefaultTime.Split('-');
                Tahun = Convert.ToInt32(dateString[0]);
                Bulan = Convert.ToInt32(dateString[1]);
                Hari = Convert.ToInt32(dateString[2]);
                time = new DateTime(Tahun, Bulan, Hari, Jam, Menit, Detik, 0);
            }
            else
                time = new DateTime(Tahun, Bulan, Hari, Jam, Menit, Detik, 0);
            var span = (TimeSpan)(DateTime.Now - time.ToLocalTime());
            str = Convert.ToInt64(span.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            Console.Write(str + " - ");
            var s = ConsumerId + "&" + str;
            //byte[] bytes = encoding.GetBytes(s);

            var hmacsha = new HMACSHA256(encoding.GetBytes(PasswordKey));
            //string str3 = Convert.ToBase64String(hmacsha.ComputeHash(bytes));
            var str3 = Convert.ToBase64String(hmacsha.ComputeHash(Encoding.UTF8.GetBytes(s)));


            client.Headers.Add("X-cons-id", ConsumerId);
            OnXcodeLoad(new XcodeEvent("X-cons-id:" + ConsumerId));
            client.Headers.Add("X-timestamp", str);
            OnXcodeLoad(new XcodeEvent("X-timestamp:" + str));
            client.Headers.Add("X-signature", str3);
            OnXcodeLoad(new XcodeEvent("X-signature:" + str3));
            return client;
        }

        private void MappingResult(List<string> listResult, JToken data)
        {
            foreach (JProperty property in data.OfType<JProperty>())
            {
                try
                {
                    bool flag = false;
                    foreach (JProperty property2 in property.Value.OfType<JProperty>())
                    {
                        flag = true;
                        bool flag2 = false;
                        foreach (JProperty property3 in property2.Value.OfType<JProperty>())
                        {
                            flag2 = true;
                            listResult.Add(property3.Name + ":" + property3.Value.ToString());
                        }
                        if (!flag2)
                        {
                            listResult.Add(property2.Name + ":" + property2.Value.ToString());
                        }
                    }
                    if (!flag)
                    {
                        listResult.Add(property.Name + ":" + property.Value.ToString());
                    }
                }
                catch (Exception e)
                {
                    listResult.Add("error:" + e.Message);
                }
            }
        }

        private static string Parent { get; set; }
        private static void MappingResultJToken(List<string> listResult, JToken data)
        {
            foreach (var property in data)
            {
                try
                {
                    var prop = property.Parent as JProperty;
                    if (prop != null)
                    {
                        var results = prop.Value;
                        if (results.Any())
                        {
                            var parent = property.Parent as JProperty;
                            if (parent != null)
                            {
                                Parent = parent.Name;
                                //listResult.Add(">>" + parent.Name);
                            }
                            MappingResultJToken(listResult, results);
                        }
                        else
                        {
                            var name = prop.Name;
                            var values = prop.Value;
                            var join = "";
                            if (!string.IsNullOrEmpty(Parent))
                                join = (Parent + "-" + name + ":" + values.Value<string>()).ToUpper();
                            else
                                join = (name + ":" + values.Value<string>()).ToUpper();
                            listResult.Add(join);
                            //listResult.Add(name + ":" + values.Value<string>());
                        }
                    }
                    else
                    {
                        var secondProp = property as JProperty;
                        if (secondProp != null)
                        {
                            var values = secondProp.Value;
                            if (values.Any())
                            {
                                var parent = secondProp.Name;
                                if (parent != null)
                                {
                                    Parent = parent;
                                }
                                MappingResultJToken(listResult, values);
                            }
                            else
                            {
                                var name = secondProp.Name;
                                var join = "";
                                if (!string.IsNullOrEmpty(Parent))
                                    join = (Parent + "-" + name + ":" + values.Value<string>()).ToUpper();
                                else
                                    join = (name + ":" + values.Value<string>()).ToUpper();
                                listResult.Add(join);
                                //listResult.Add(name + ":" + values.Value<string>());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    listResult.Add("error:" + e.Message);
                }
            }
        }

        #endregion


    }
}
