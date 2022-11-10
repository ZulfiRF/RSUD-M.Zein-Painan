using ApiToJKN.Models;
using ApiToJKN.Models.Requests;
using ApiToJKN.Models.Responses;
using ApiToJKN.Utilities;
using Core.Framework.Helper.Date;
using Core.Framework.Helper.Extention;
using Core.Framework.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using NLog;
using Jasamedika.Sdk.Vclaim;

namespace ApiToJKN.Controllers
{
    /// <inheritdoc/>>
    public class WsRsV2Controller : ApiController
    {
        private string noAntrianJoin;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WsRsV2Controller()
        {
        }

        /// <summary>
        /// digunakan untuk checkin antrean
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("api/V2/CheckinAntrean")]
        [HttpPost]
        public JsonResult<Metadata> Checkin([FromBody] CheckinRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.KodeBooking) ||
                    string.IsNullOrWhiteSpace(request.KodeBooking))
                    throw new Exception("isi kode booking");

                VerifyToken();

                var sql = "select * from Reservasi " +
                    "where KodeBooking = '" + request.KodeBooking + "'";
                using (var db = new ContextManager(Connection))
                {
                    var readReservasi = db.ExecuteQuery(sql);
                    var isKodeBookingExist = false;
                    while (readReservasi.Read())
                    {
                        isKodeBookingExist = true;
                    }

                    if (!isKodeBookingExist)
                        throw new Exception("kode booking tidak ditemukan");
                    else
                    {
                        sql = "select top 1 * from ReservasiCheckin " +
                            "where KodeBooking = '" + request.KodeBooking + "'";
                        var readLastCheckin = db.ExecuteQuery(sql);
                        var isExistLastCheckin = false;
                        while (readLastCheckin.Read())
                        {
                            isExistLastCheckin = true;
                        }

                        if (isExistLastCheckin)
                            sql = "update ReservasiCheckin set " +
                                "LastCheckinServer = GetDate(), " +
                                "LastCheckinClient = '" + request.Waktu.ToDatetTime().ToString("yyy-MM-dd HH:mm:ss") + "' " +
                                "where KodeBooking = '" + request.KodeBooking + "'";
                        else
                            sql = "insert into ReservasiCheckin (KodeBooking, LastCheckinServer, LastCheckinClient) " +
                                "values ('" + request.KodeBooking + "', GetDate(), '" + request.Waktu.ToDatetTime().ToString("yyy-MM-dd HH:mm:ss") + "')";
                        db.ExecuteNonQuery(sql);
                    }
                }
                return Result("OK", 200);
            }
            catch (Exception e)
            {
                return Result(e.Message, 201);
            }
        }

        /// <summary>
        /// digunakan untuk membatalkan antrean  per poli
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("api/V2/BatalAntrean")]
        [HttpPost]
        public JsonResult<BatalResponse> BatalAntrian([FromBody] BatalAntrianRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.KodeBooking) ||
                    string.IsNullOrWhiteSpace(request.KodeBooking))
                    throw new Exception("isi kode booking");

                VerifyToken();

                var sql = "select * from Reservasi " +
                    "where KodeBooking = '" + request.KodeBooking + "'";
                using (var db = new ContextManager(Connection))
                {
                    var readReservasi = db.ExecuteQuery(sql);
                    var isKodeBookingExist = false;
                    while (readReservasi.Read())
                    {
                        isKodeBookingExist = true;
                    }

                    if (!isKodeBookingExist)
                        throw new Exception("kode booking tidak ditemukan");
                    else
                    {
                        sql = "select top 1 * from ReservasiBatal " +
                            "where KodeBooking = '" + request.KodeBooking + "'";
                        var readLastCheckin = db.ExecuteQuery(sql);
                        var isExistLastCheckin = false;
                        while (readLastCheckin.Read())
                        {
                            isExistLastCheckin = true;
                        }

                        if (isExistLastCheckin)
                            sql = "update ReservasiBatal set " +
                                "TglBatal = GetDate(), " +
                                "Keterangan = '" + request.Keterangan + "' " +
                                "where KodeBooking = '" + request.KodeBooking + "'";
                        else
                            sql = "insert into ReservasiBatal (KodeBooking, TglBatal, Keterangan) " +
                                "values ('" + request.KodeBooking + "', GetDate(), '" + request.Keterangan + "')";
                        db.ExecuteNonQuery(sql);

                        //[20220118] ER
                        sql = "UPDATE DetailReservasi SET StatusPasien='B' WHERE KodeBooking='" + request.KodeBooking + "'";
                        db.ExecuteNonQuery(sql);
                    }
                }

                return ResultBatal("OK", 200);
            }
            catch (Exception e)
            {
                return ResultBatal(e.Message, 201);
            }
        }

        /// <summary>
        /// digunakan untuk mendapatkan status antrean
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("api/V2/StatusAntrean")]
        [HttpPost]
        public JsonResult<StatusAntrianRespone> StatusAntrian([FromBody] StatusAntrianRequest request)
        {
            try
            {
                VerifyToken();

                if (string.IsNullOrEmpty(request.KodePoli) || string.IsNullOrWhiteSpace(request.KodePoli))
                    throw new Exception("isi kode poli");
                if (string.IsNullOrEmpty(request.TanggalPeriksa) || string.IsNullOrWhiteSpace(request.TanggalPeriksa))
                    throw new Exception("isi tanggal periksa");

                using (var db = new ContextManager(Connection))
                {
                    var kdRuangan = "";
                    var namaPoli = "";
                    var sql = "select top 1 KdRuangan,NamaRuangan from ruangan where KodeExternal='" + request.KodePoli + "'";
                    var readerRuangan = db.ExecuteQuery(sql);
                    while (readerRuangan.Read())
                    {
                        kdRuangan = readerRuangan["KdRuangan"].ToString();
                        namaPoli = readerRuangan["NamaRuangan"].ToString();
                    }
                    if (string.IsNullOrEmpty(kdRuangan))
                    {
                        throw new Exception("kode poli tidak sesuai");
                    }

                    var kuota = 0;
                    var sisaKuota = 0;
                    var totalAntrian = 0;

                    //[20211228] ER
                    var idDokter = "";
                    var namaDokter = "";
                    sql = "SELECT IdPegawai,NamaDokterHFIZ FROM DokterHFIZ WHERE KdDokterHFIZ='" + request.KodeDokter + "'";
                    var readerDokter = db.ExecuteQuery(sql);
                    while (readerDokter.Read())
                    {
                        idDokter = readerDokter["IdPegawai"].ToString();
                        namaDokter = readerDokter["NamaDokterHFIZ"].ToString();
                    }

                    //sql = "SELECT    ruangan.namaruangan, dbo.Jumlah_Pasien_Terdaftar('" + request.TanggalPeriksa + "', " + 1 + ", '" + request.KodePoli + "') AS JumlahPasienTerdaftar, " +
                    //      "Sloting.KdKelompokWaktu, Sloting.KdRuangan, Sloting.Slotting,   dbo.Ambil_JmlAntrianTerlayani('" + request.TanggalPeriksa + "') AS SisaSloting, " +
                    //      "DataPegawai.NamaLengkap " +
                    //      @"FROM            Instalasi INNER JOIN
                    //         Ruangan ON Instalasi.KdInstalasi = Ruangan.KdInstalasi INNER JOIN
                    //         Sloting ON Ruangan.KdRuangan = Sloting.KdRuangan LEFT OUTER JOIN
                    //         MapingPeriodeWaktuAwalkeAkhir ON Sloting.KdKelompokWaktu = MapingPeriodeWaktuAwalkeAkhir.KdKelompokWaktu LEFT OUTER JOIN
                    //         DataPegawai INNER JOIN
                    //         DetailReservasi INNER JOIN
                    //         Reservasi ON DetailReservasi.KodeBooking = Reservasi.KodeBooking ON DataPegawai.IdPegawai = DetailReservasi.IdPegawai ON Ruangan.KdRuangan = Reservasi.KdRuangan " +
                    //      "WHERE (Sloting.KdRuangan = '" + kdRuangan + "') AND DetailReservasi.StatusPasien<>'B' ";

                    //if (!string.IsNullOrEmpty(request.KodeDokter) || !string.IsNullOrWhiteSpace(request.KodeDokter))
                    //    sql += "and DetailReservasi.IdPegawai = '" + IdDokter + "' AND Sloting.KdKelompokWaktu='" + KodeKelompokWaktu + "' ";

                    //sql += "Group by Sloting.KdKelompokWaktu,Sloting.KdRuangan,Sloting.Slotting,ruangan.namaruangan,DataPegawai.NamaLengkap";
                    //var reader = db.ExecuteQuery(sql);
                    //while (reader.Read())
                    //{
                    //    kuota = Convert.ToInt32(reader["Slotting"]);
                    //    sisaKuota = Convert.ToInt32(reader["SisaSloting"]);
                    //    totalAntrian = Convert.ToInt32(reader["JumlahPasienTerdaftar"]);
                    //    namaPoli = reader["NamaRuangan"].ToString();
                    //    //namaDokter = reader["NamaLengkap"].ToString();                        
                    //}

                    var query = "SELECT Slotting FROM Sloting WHERE KdRuangan='" + kdRuangan + "' AND KdKelompokWaktu='" + KodeKelompokWaktu + "'";
                    var reader = db.ExecuteQuery(query);
                    while (reader.Read())
                    {
                        kuota = Convert.ToInt32(reader["Slotting"]);
                    }

                    //var sqlTotal = "SELECT dbo.Jumlah_Pasien_Terdaftar('" + request.TanggalPeriksa + "'," + KodeKelompokWaktu + ",'" + kdRuangan + "') AS JumlahPasienTerdaftar";
                    var sqlTotal = "SELECT dbo.Jumlah_Pasien_Terdaftar_v2('" + request.TanggalPeriksa + "'," + KodeKelompokWaktu + ",'" + kdRuangan + "','" + idDokter + "') AS JumlahPasienTerdaftar";
                    var readerTotal = db.ExecuteQuery(sqlTotal);
                    while (readerTotal.Read())
                    {
                        totalAntrian = Convert.ToInt32(readerTotal["JumlahPasienTerdaftar"]);
                    }

                    sisaKuota = kuota - totalAntrian;

                    //var listAntrian = new List<AntrianPasienRegistrasiModel>();
                    //sql = "select AntrianPasienRegistrasi.*, AntrianLoket.[Call] " +
                    //    "from AntrianPasienRegistrasi  " +
                    //    "inner join AntrianLoket on AntrianPasienRegistrasi.JenisPasien = AntrianLoket.NamaLoket  " +
                    //    "WHERE TglAntrian > '" + request.TanggalPeriksa + "'";
                    //var readerAntrian = db.ExecuteQuery(sql);
                    //while (readerAntrian.Read())
                    //{
                    //    listAntrian.Add(new AntrianPasienRegistrasiModel()
                    //    {
                    //        KdAntrian = readerAntrian["KdAntrian"].ToInt32(),
                    //        NoLoketCounter = readerAntrian["NoLoketCounter"].ToByte(),
                    //        NoAntrian = readerAntrian["NoAntrian"].ToInt32(),
                    //        Call = readerAntrian["Call"].ToString()
                    //    });
                    //}
                    //var antrianDipanggil = listAntrian.LastOrDefault(n => n.NoLoketCounter != 0);
                    //var jmlAntrianDipanggil = antrianDipanggil != null ? antrianDipanggil.NoAntrian : 0;
                    //var sisaAntrian = totalAntrian - jmlAntrianDipanggil;

                    var antrianDipanggil = 0;
                    var sisaAntrian = 0;
                    sql = "SELECT COUNT(dbo.Reservasi.KodeBooking) AS JmlDipanggil FROM dbo.Reservasi INNER JOIN dbo.DetailReservasi ON " +
                          "dbo.Reservasi.KodeBooking = dbo.DetailReservasi.KodeBooking AND dbo.Reservasi.KdRuangan = dbo.DetailReservasi.KdRuangan " +
                          "WHERE YEAR(TglReservasi)= '" + Convert.ToDateTime(request.TanggalPeriksa).Year + "' " +
                          "AND MONTH(TglReservasi)= '" + Convert.ToDateTime(request.TanggalPeriksa).Month + "' " +
                          "AND DAY(TglReservasi)= '" + Convert.ToDateTime(request.TanggalPeriksa).Day + "' " +
                          "AND StatusPasien = 'Y' " +
                          "AND DetailReservasi.IdPegawai = '" + IdPegawai + "'";
                    var readerAntrian = db.ExecuteQuery(sql);
                    while (readerAntrian.Read())
                    {
                        antrianDipanggil = Convert.ToInt32(readerAntrian["JmlDipanggil"]);
                    }

                    sisaAntrian = totalAntrian - antrianDipanggil;

                    var result = new StatusAntrianRespone()
                    {
                        Response = new StatusAntrianDetail()
                        {
                            NamaPoli = namaPoli,
                            NamaDokter = namaDokter,
                            TotalAntrian = totalAntrian,
                            SisaAntrian = sisaAntrian,
                            AntrianDipanggil = Convert.ToString(antrianDipanggil), //antrianDipanggil != null ? antrianDipanggil.Call + "-" + antrianDipanggil.NoAntrian : "-",
                            SisaKuotaJkn = sisaKuota,
                            KuotaJkn = kuota,
                            SisaKuotaNonJkn = sisaKuota,
                            KuotaNonJkn = kuota,
                            Keterangan = "-"
                        },
                        Metadata = new Metadata()
                        {
                            Code = 200,
                            Message = "Ok"
                        }
                    };
                    return Json(result);
                }
            }
            catch (Exception e)
            {
                return ResultStatus(e.Message, 201);
            }
        }

        /// <summary>
        /// digunakan untuk mengambil antrian 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/V2/AmbilAntrean")]
        public JsonResult<AmbilAntrianRespone> AmbilAntrian([FromBody] AmbilAntrianRequest request)
        {
            var kodeBooking = "";
            try
            {
                VerifyToken();
                //var nokartu = property.nokartu.ToLower();
                //var nik = property.nik.ToLower();
                //if (nokartu == null or nik == null ){
                //    throw new Exception("" + "Pasien Baru Mohon Mendaftar Langsung ke RSUD M.Zein Painan untuk Keperluan Administrasi");
                //}

                var getProperties = request.GetType().GetProperties();
                foreach (var property in getProperties)
                {
                    var name = property.Name.ToLower();
                    if (name == "norm") continue;
                    if (name == "nomorreferensi") continue;
                    var isi = property.GetValue(request);
                    if (isi == null)
                        throw new Exception("isi " + name);
                    if (string.IsNullOrEmpty(isi.ToString()) || string.IsNullOrWhiteSpace(isi.ToString()))
                        throw new Exception("isi " + name);

                }

                if (string.IsNullOrEmpty(request.NomorKartu) || string.IsNullOrEmpty(request.Nik))
                    throw new Exception("Pasien baru mohon mendaftar langsung ke RSUD untuk keperluan administrasi");

                var pasienBaru = true;
                using (var db = new ContextManager(Connection))
                {
                    var cek = $"select top 1 * from PemakaianAsuransi where IdAsuransi = '{request.NomorKartu}'";
                    var readerCek = db.ExecuteQuery(cek);
                    while (readerCek.Read())
                    {
                        pasienBaru = false;
                    }

                    var cek2 = $"select top 1 * from Pasien where NoIdentitas = '{request.Nik}'";
                    var readerCek2 = db.ExecuteQuery(cek2);
                    while (readerCek2.Read())
                    {
                        pasienBaru = false;
                    }
                }

                if (pasienBaru)
                    throw new Exception("Pasien baru mohon mendaftar langsung ke RSUD untuk keperluan administrasi");

                var context = new ContextVclaim
                {
                    ConsumerId = ConsumerId,
                    PasswordKey = PasswordKey,
                    Url = UrlHost,
                    UserKey = UserKey,
                    IsEncrypt = IsEncrypt.ToInt16(),
                    IsByPassSsl = 1
                };

                var IdPenjamin = KodeJenisPasien;
                var tglReservasi = Convert.ToDateTime(request.TanggalPeriksa);

                if (!new List<int>() { 1, 2, 3, 4 }.Contains(request.JenisKunjungan))
                    throw new Exception("jenis kunjungan tidak sesuai");

                if (string.IsNullOrEmpty(request.KodePoli) || string.IsNullOrWhiteSpace(request.KodePoli))
                    throw new Exception("isi kode poli");

                var hariReq = Convert.ToInt32(Convert.ToDateTime(request.TanggalPeriksa).DayOfWeek);
                if (hariReq == 0 || hariReq == 7 || hariReq == 6)
                    throw new Exception("maaf hari ini sedang libur");

                if (string.IsNullOrEmpty(request.TanggalPeriksa) || string.IsNullOrWhiteSpace(request.TanggalPeriksa))
                    throw new Exception("isi tanggal periksa");

                var tglSkrg = DateTime.Now;
                //var tglskrgmundur = DateTime.Now.AddDays(-1);
                var tglYgDipilih = Convert.ToDateTime(request.TanggalPeriksa);
                    if (tglYgDipilih < tglSkrg)
                    {
                        var msg = ConfigurationManager.AppSettings["MessageMinimalAntrian"];
                        throw new Exception(msg);
                    }
                    if (tglYgDipilih > tglSkrg.AddDays(7))
                    {
                        var msg = ConfigurationManager.AppSettings["MessageMaksimalAntrian"];
                        throw new Exception(msg);
                    }

                var temp = new List<string>();
                var list = new List<string[]>();
                var dict = new Dictionary<string, object>();

                #region comment


                //foreach (var str in context.CariPesertaByNoKartuBpjs(request.NomorKartu, DateTime.Now.ToString("yyyy-MM-dd")))
                //{
                //    if (str.Contains("=============") || str.Contains(">>"))
                //    {
                //        list.Add(temp.ToArray());
                //    }
                //    else if (str.Contains(">>"))
                //    {
                //        list.Add(temp.ToArray());
                //    }
                //    else
                //    {
                //        temp.Add(str);
                //    }
                //}

                //dict = new Dictionary<string, object>();
                //var item = list.FirstOrDefault();
                //foreach (var n in temp)
                //{
                //    if (n.Contains(":"))
                //    {
                //        object obj;
                //        if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                //        {
                //            dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                //        }
                //    }
                //}

                //var validasiKey = dict.Keys.FirstOrDefault().ToString();
                //var validasiValue = dict.Values.FirstOrDefault().ToString();
                //if (validasiKey == "error")
                //{
                //    throw new Exception(validasiValue);
                //}
                //else
                {
                    //    temp = new List<string>();
                    //    list = new List<string[]>();
                    //    //kalau nomor rujukan / nomor referensi nya terisi
                    //    if (!string.IsNullOrEmpty(request.NomorReferensi))
                    //    {
                    //        //cari rujukan pcare by nomor rujukan
                    //        foreach (var str in context.RujukanPcareByNoRujukan(request.NomorReferensi))
                    //        {
                    //            if (str.Contains("=============") || str.Contains(">>"))
                    //            {
                    //                list.Add(temp.ToArray());
                    //            }
                    //            else if (str.Contains(">>"))
                    //            {
                    //                list.Add(temp.ToArray());
                    //            }
                    //            else
                    //            {
                    //                temp.Add(str);
                    //            }
                    //        }

                    //        dict = new Dictionary<string, object>();
                    //        foreach (var n in temp)
                    //        {
                    //            if (n.Contains(":"))
                    //            {
                    //                object obj;
                    //                if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                    //                {
                    //                    dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                    //                }
                    //            }
                    //        }

                    //        if (dict.Count < 10)
                    //        {
                    //            //cari rujukan rs by nomor rujukan
                    //            foreach (var str in context.RujukanRsByNoRujukan(request.NomorReferensi))
                    //            {
                    //                if (str.Contains("=============") || str.Contains(">>"))
                    //                {
                    //                    list.Add(temp.ToArray());
                    //                }
                    //                else if (str.Contains(">>"))
                    //                {
                    //                    list.Add(temp.ToArray());
                    //                }
                    //                else
                    //                {
                    //                    temp.Add(str);
                    //                }
                    //            }

                    //            dict = new Dictionary<string, object>();
                    //            foreach (var n in temp)
                    //            {
                    //                if (n.Contains(":"))
                    //                {
                    //                    object obj;
                    //                    if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                    //                    {
                    //                        dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                    //                    }
                    //                }
                    //            }

                    //            if (dict.Count < 10)
                    //            {
                    //                throw new Exception("rujukan tidak ditemukan");
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        temp = new List<string>();
                    //        list = new List<string[]>();
                    //        //cari rujukan pcare by nomor kartu bpjs
                    //        foreach (var str in context.RujukanPcareByNoKartu(request.NomorKartu))
                    //        {
                    //            if (str.Contains("=============") || str.Contains(">>"))
                    //            {
                    //                list.Add(temp.ToArray());
                    //            }
                    //            else if (str.Contains(">>"))
                    //            {
                    //                list.Add(temp.ToArray());
                    //            }
                    //            else
                    //            {
                    //                temp.Add(str);
                    //            }
                    //        }

                    //        dict = new Dictionary<string, object>();
                    //        foreach (var n in temp)
                    //        {
                    //            if (n.Contains(":"))
                    //            {
                    //                object obj;
                    //                if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                    //                {
                    //                    dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                    //                }
                    //            }
                    //        }
                    //        if (dict.Count <= 23)
                    //        {
                    //            //cari rujukan rs by nomor kartu bpjs
                    //            foreach (var str in context.RujukanRsByNoKartu(request.NomorKartu))
                    //            {
                    //                if (str.Contains("=============") || str.Contains(">>"))
                    //                {
                    //                    list.Add(temp.ToArray());
                    //                }
                    //                else if (str.Contains(">>"))
                    //                {
                    //                    list.Add(temp.ToArray());
                    //                }
                    //                else
                    //                {
                    //                    temp.Add(str);
                    //                }
                    //            }

                    //            dict = new Dictionary<string, object>();
                    //            foreach (var n in temp)
                    //            {
                    //                if (n.Contains(":"))
                    //                {
                    //                    object obj;
                    //                    if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                    //                    {
                    //                        dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                    //                    }
                    //                }
                    //            }

                    //            if (dict.Count <= 23)
                    //            {
                    //                throw new Exception("rujukan tidak ditemukan");
                    //            }
                    //        }
                    //    }

                    //var validasiRujukanKey = dict.Keys.FirstOrDefault().ToString();
                    //var validasiRujukanValue = dict.Values.FirstOrDefault().ToString();
                    //var dictNoKartu = dict.Keys.FirstOrDefault(n => n.Contains("nokartu"));
                    //var dictTglkunjungan = dict.Keys.FirstOrDefault(n => n.Contains("tglkunjungan"));

                    //if (validasiRujukanKey == "error")
                    //{
                    //    throw new Exception(validasiRujukanValue);
                    //}
                    //else if (dict[dictNoKartu].ToString() != request.NomorKartu.ToString())
                    //{
                    //    throw new Exception("norujukan tidak sesuai dengan nomorkartu");
                    //}

                    //else if (Convert.ToInt32(DateTime.Now.Subtract(Convert.ToDateTime(dict[dictTglkunjungan].ToString())).TotalDays) >= 90)
                    //{
                    //    throw new Exception("tanggal rujukan tidak aktif / lebih dari 90 hari");
                    //}
                    #endregion

                    using (var db = new ContextManager(Connection))
                    {
                        var chekPesertaByNoKartu = "select top 1 NoCm from PemakaianAsuransi where IdAsuransi = '" + request.NomorKartu + "'";
                        var readerChekPeserta = db.ExecuteQuery(chekPesertaByNoKartu);
                        while (readerChekPeserta.Read())
                        {
                            request.NoRm = readerChekPeserta["NoCm"].ToString();
                        }

                        var chekPesertaByNik = "select top 1 NoCm from Pasien where NoIdentitas = '" + request.Nik + "'";
                        var readerChekPesertanik = db.ExecuteQuery(chekPesertaByNik);
                        while (readerChekPesertanik.Read())
                        {
                            request.NoRm = readerChekPesertanik["NoCm"].ToString();
                        }

                        //[20211228] ER
                        var ruanganUse = "";
                        var KdRuanganPoli = "";
                        var namaPoli = "";
                        var chek = "select KdRuangan,NamaRuangan from ruangan where KodeExternal='" + request.KodePoli + "'";
                        var readerChek = db.ExecuteQuery(chek);
                        while (readerChek.Read())
                        {
                            ruanganUse = readerChek["KdRuangan"].ToString();
                            KdRuanganPoli = readerChek["KdRuangan"].ToString();
                            namaPoli = readerChek["NamaRuangan"].ToString();
                        }

                        if (string.IsNullOrEmpty(ruanganUse))
                        {
                            throw new Exception("ruangan tujuan tidak sesuai");
                        }

                        var sqlReservasi = "";
                        if (string.IsNullOrEmpty(request.NoRm))
                        {
                            sqlReservasi = "select *  FROM         Reservasi INNER JOIN " +
                                         " DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking " +
                                         " where DetailReservasi.noidentitas='" + request.Nik + "' AND StatusPasien<>'B' " +
                                         "and month(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("MM") + "' " +
                                         "and  year(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("yyyy") + "' " +
                                         "and  day(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("dd") + "'";
                        }
                        else
                        {
                            sqlReservasi = "select *  FROM         Reservasi INNER JOIN " +
                                        " DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking " +
                                        " where DetailReservasi.nocm='" + request.NoRm + "' AND StatusPasien<>'B' " +
                                        "and month(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("MM") + "' " +
                                        "and  year(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("yyyy") + "' " +
                                        "and  day(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("dd") + "'";

                        }

                        var isPasienExist = false;
                        var reader4 = db.ExecuteQuery(sqlReservasi);
                        while (reader4.Read())
                        {
                            isPasienExist = true;
                        }

                        if (isPasienExist)
                        {
                            throw new Exception("data pasien sudah terdaftar");
                        }
                        else
                        {
                            //var query = "SELECT    ruangan.namaruangan, " +
                            //    "dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi + "', " + KodeKelompokWaktu + ", '" + request.KodePoli + "') AS JumlahPasienTerdaftar, " +
                            //    "Sloting.KdKelompokWaktu, " +
                            //    "Sloting.KdRuangan, " +
                            //    "Sloting.Slotting,  " +
                            //    "Sloting.Slotting - dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi + "', " + KodeKelompokWaktu + ", '" + request.KodePoli + "') AS SisaSloting, " +
                            //    "DataPegawai.NamaLengkap " +
                            //    "FROM PasienDaftar INNER JOIN Ruangan ON PasienDaftar.KdRuanganAkhir = Ruangan.KdRuangan " +
                            //    "INNER JOIN Instalasi ON Ruangan.KdInstalasi = Instalasi.KdInstalasi " +
                            //    "INNER JOIN Sloting ON Ruangan.KdRuangan = Sloting.KdRuangan " +
                            //    "INNER JOIN MapingPeriodeWaktuAwalkeAkhir ON Sloting.KdKelompokWaktu = MapingPeriodeWaktuAwalkeAkhir.KdKelompokWaktu " +
                            //    "INNER JOIN PasienMasukRumahSakit ON PasienDaftar.NoPendaftaran = PasienMasukRumahSakit.NoPendaftaran " +
                            //    "INNER JOIN DataPegawai ON PasienMasukRumahSakit.IdDokter = DataPegawai.IdPegawai " +
                            //    "WHERE (Sloting.KdRuangan = '" + request.KodePoli + "') " +
                            //    "Group by Sloting.KdKelompokWaktu,Sloting.KdRuangan,Sloting.Slotting,ruangan.namaruangan,NamaLengkap";

                            var kuota = 0;
                            var sisaKuota = 0;
                            var totalAntrian = 0;
                            //var namaPoli = "";
                            //var namaDokter = "";

                            var query = "SELECT Slotting FROM Sloting WHERE KdRuangan='" + KdRuanganPoli + "' AND KdKelompokWaktu='" + KodeKelompokWaktu + "'";
                            var reader = db.ExecuteQuery(query);
                            while (reader.Read())
                            {
                                kuota = Convert.ToInt32(reader["Slotting"]);
                                //sisaKuota = Convert.ToInt32(reader["SisaSloting"]);
                                //totalAntrian = Convert.ToInt32(reader["JumlahPasienTerdaftar"]);
                                //namaPoli = reader["NamaRuangan"].ToString();
                                //namaDokter = reader["NamaLengkap"].ToString();
                            }

                            //[20220118] ER
                            //var sqlTotal = "SELECT dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi + "'," + KodeKelompokWaktu + ",'" + request.KodePoli + "') AS JumlahPasienTerdaftar";
                            var sqlTotal = "SELECT dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi.ToString("yyyy/MM/dd HH:mm:ss") + "'," + KodeKelompokWaktu + ",'" + KdRuanganPoli + "') AS JumlahPasienTerdaftar";
                            var readerTotal = db.ExecuteQuery(sqlTotal);
                            while (readerTotal.Read())
                            {
                                totalAntrian = Convert.ToInt32(readerTotal["JumlahPasienTerdaftar"]);
                            }

                            sisaKuota = kuota - totalAntrian;

                            var noCm = "";
                            var namaPasien = "";
                            var alamat = "";
                            var noTelp = "";

                            var sql = "select * from pasien where nocm ='" + request.NoRm + "'";
                            var readerPasien = db.ExecuteQuery(sql);
                            while (readerPasien.Read())
                            {
                                noCm = readerPasien["NoCM"].ToString();
                                namaPasien = readerPasien["NamaLengkap"].ToString();
                                alamat = readerPasien["Alamat"].ToString();
                                noTelp = readerPasien["Telepon"].ToString();
                            }

                            var noRm = "";
                            var PasienBaru = "0";
                            if (string.IsNullOrEmpty(noCm))
                            {
                                //noRm = request.NoRm;
                                noRm = "000000";
                            }
                            else
                            {
                                noRm = noCm;
                            }

                            //pasien baru bisa reservasi, tapi daftar dulu di loket pendaftaran
                            //if (string.IsNullOrEmpty(noRm))
                            //{                                
                            //    throw new Exception("pasien belum terdaftar di rumah sakit");
                            //}

                            var countNoantrian = GetNoReservasi(Convert.ToDateTime(tglReservasi));
                            kodeBooking = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                            var totalDaftar = "";

                            sql = "select top 1 TglReservasi from Reservasi " +
                                "where month(TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).Month + "' " +
                                "and year(TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).Year + "' " +
                                "and day(TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).Day + "' " +
                                "order by TglReservasi desc";

                            var readerReservasi = db.ExecuteQuery(sql);
                            while (readerReservasi.Read())
                            {
                                totalDaftar = readerReservasi["TglReservasi"].ToString();
                            }

                            if (!string.IsNullOrEmpty(totalDaftar))
                                tglReservasi = Convert.ToDateTime(totalDaftar).AddMinutes(10);
                            else
                                tglReservasi = Convert.ToDateTime(tglReservasi).AddHours(8);

                            //[20211228] ER
                            var IdDokter = IdPegawai;
                            var namaDokter = "";
                            sql = "SELECT IdPegawai,NamaDokterHFIZ FROM DokterHFIZ WHERE KdDokterHFIZ='" + request.KodeDokter + "'";
                            var readerDokter = db.ExecuteQuery(sql);
                            while (readerDokter.Read())
                            {
                                IdDokter = readerDokter["IdPegawai"].ToString();
                                namaDokter = readerDokter["NamaDokterHFIZ"].ToString();
                            }

                            sql = "insert into Reservasi(NoReservasi,KodeBooking,TglReservasi,TglRegistrasi,KdRuangan) " +
                                "values('" + countNoantrian + "'," +
                                "'" + kodeBooking + "'," +
                                "'" + tglReservasi.ToString("yyyy/MM/dd HH:mm:ss") + "'," +
                                "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'," +
                                "'" + KdRuanganPoli + "')";
                            db.ExecuteNonQuery(sql);

                            logger.Info($"INSERT Reservasi --> KodeBooking {kodeBooking} Ok");

                            sql =
                                "insert into DetailReservasi(KodeBooking,NoCM,NoIdentitas,NamaLengkap,Alamat,NoTelp,Email,KdRuangan,IdPegawai,KdKelompokWaktu,JenisPasien,StatusPasien) values " +
                                "('" + kodeBooking + "'," +
                                "'" + noRm + "'," +
                                "'" + request.Nik + "'," +
                                "'" + namaPasien + "'," +
                                "'" + alamat + "'," +
                                "'" + noTelp + "'," +
                                "'" + "-" + "'," +
                                "'" + KdRuanganPoli + "'," +
                                "'" + IdDokter + "'," +
                                "" + KodeKelompokWaktu + "," +
                                "'" + IdPenjamin + "'," +
                                "'T')\n";

                            db.ExecuteNonQuery(sql);

                            logger.Info($"INSERT DetailReservasi --> KodeBooking {kodeBooking} Ok");

                            //[20211228] ER
                            sql = "INSERT INTO ReservasiMobileJKN VALUES ('" + kodeBooking + "')";
                            db.ExecuteNonQuery(sql);

                            logger.Info($"INSERT ReservasiMobileJKN --> KodeBooking {kodeBooking} Ok");

                            //request.NomorReferensi = kodeBooking;

                            ////sql = "SELECT     Substring(Reservasi.NoReservasi,7, 3) as NoReservasi, Reservasi.KodeBooking, " +
                            ////             "Reservasi.TglReservasi, Reservasi.TglRegistrasi, Reservasi.KdRuangan, Ruangan.NamaRuangan " +
                            ////             "FROM         Reservasi " +
                            ////             "INNER JOIN DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking " +
                            ////             "INNER JOIN Ruangan ON Reservasi.KdRuangan = Ruangan.KdRuangan where Reservasi.KodeBooking ='" + kodeBooking + "'";

                            sql = @"SELECT        RIGHT(Reservasi.NoReservasi, 3) AS NoReservasi, Reservasi.KodeBooking, Reservasi.TglReservasi, Reservasi.TglRegistrasi, Reservasi.KdRuangan, Ruangan.NamaRuangan, DetailReservasi.IdPegawai, 
                                 AliasDokter.AliasDok FROM Reservasi INNER JOIN DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking INNER JOIN
                                 Ruangan ON Reservasi.KdRuangan = Ruangan.KdRuangan INNER JOIN AliasDokter ON DetailReservasi.IdPegawai = AliasDokter.IdDokter where Reservasi.KodeBooking ='" + kodeBooking + "'";
                            var noAntrian = "";
                            var estimasi = "";
                            var aliasDokter = "";
                            var readerReservasiAll = db.ExecuteQuery(sql);
                            while (readerReservasiAll.Read())
                            {
                                noAntrian = readerReservasiAll["NoReservasi"].ToString();
                                namaPoli = readerReservasiAll["NamaRuangan"].ToString();
                                estimasi = Convert.ToString(DateHelper.ToInteger(Convert.ToDateTime(readerReservasiAll["TglReservasi"].ToString()))) + "000";
                                aliasDokter = readerReservasiAll["AliasDok"].ToString();
                            }

                            if (!string.IsNullOrEmpty(noAntrian))
                            {
                                //Format {KodePoli}-{NoAntrian}
                                //Sample : MAT-001
                                //D3 adalah banyaknya digit angka maksimal bisa diisi 0
                                //Contoh : 000/001/012/045 = D3
                                //Contoh : 0000/0001/0012/0045 = D4
                                //  var noAntrianJoin = request.KodePoli + "-" + Convert.ToInt32(noAntrian).ToString("D3");

                                var hrIni = Convert.ToDateTime(request.TanggalPeriksa);
                                short noAntrianTemp = 0;
                                var kdAntrian = 0;
                                var statusPasien = "0";
                                if (!string.IsNullOrEmpty(noCm))
                                    statusPasien = "1";

                                var queryAntrian = @"select * from antrianpasienregistrasi 
                                    where cast(tglantrian as date) = '" + hrIni.ToString("yyy-MM-dd") + "' and kddokterorder = '" + IdDokter + "'";
                                var readerAntrian = db.ExecuteQuery(queryAntrian);
                                while (readerAntrian.Read())
                                {
                                    noAntrianTemp = Convert.ToInt16(readerAntrian["NoAntrian"]);
                                }

                                queryAntrian = @"select MAX(KdAntrian) KdAntrian from antrianpasienregistrasi";
                                readerAntrian = db.ExecuteQuery(queryAntrian);
                                while (readerAntrian.Read())
                                {
                                    kdAntrian = readerAntrian["KdAntrian"].ToInt32();
                                }

                                var noAntrianIncrement = noAntrianTemp + 1;
                                var noAntrianJoin = aliasDokter + "-" + noAntrianIncrement.ToString("D3");
                                //var noAntrianJoin = aliasDokter + "-" + Convert.ToInt32(noAntrian).ToString("D3");

                                //if (string.IsNullOrEmpty(request.NoRm))
                                //{
                                //    var jenisPasien = "A";
                                //    if (!string.IsNullOrEmpty(request.NomorKartu))
                                //        jenisPasien = "B";
                                //    noAntrianJoin = jenisPasien + "-" + noAntrianIncrement.ToString("D3");
                                //}

                                logger.Info($"Nomor Antrian : {noAntrianJoin} --> KodeBooking {kodeBooking}");

                                var kdAntrianIncrement = kdAntrian + 1;
                                var queryAddAntrian = @"INSERT INTO [dbo].[AntrianPasienRegistrasi]
                                           ([KdAntrian]
                                           ,[TglAntrian]
                                           ,[NoAntrian]
                                           ,[StatusPasien]
                                           ,[NoCM]
                                           ,[KdRuangan]
                                           ,[KdKelas]
                                           ,[KdKelompokPasien]
                                           ,[KdDokterOrder]
                                           ,[NoAntrianOrder]
                                           ,[KeteranganOrder]
                                           ,[NoReturOrder]
                                           ,[StatusEnabled]
                                           ,[IdUser]
                                           ,[NoLoketCounter]
                                           ,[NoPendaftaran]
                                           ,[JenisPasien])
                                     VALUES
                                           (" + kdAntrianIncrement + "" +
                                           ",'" + hrIni.ToString("yyy-MM-dd HH:mm:ss") + "'" +
                                           "," + noAntrianIncrement + "" +
                                           "," + (string.IsNullOrEmpty(noCm) ? "0" : "1") + "" +
                                           ",'" + request.NoRm + "'" +
                                           ",'" + KdRuanganPoli + "'" +
                                           ",'02'" +
                                           ",'" + (string.IsNullOrEmpty(request.NomorKartu) ? "02" : "01") + "'" +
                                           ",'" + IdDokter + "'" +
                                           ",''" +
                                           ",'" + kodeBooking + "'" +
                                           ",''" +
                                           ",1" +
                                           ",'" + IdDokter + "'" +
                                           ",null" +
                                           ",null" +
                                           ",'" + (string.IsNullOrEmpty(request.NomorKartu) ? "UMUM" : "JKN") + "')";
                                db.ExecuteNonQuery(queryAddAntrian);

                                logger.Info($"INSERT AntrianPasienRegistrasi --> KodeBooking {kodeBooking} Ok");

                                var result = new AmbilAntrianRespone()
                                {
                                    Response = new AmbilAntrianDetail()
                                    {
                                        NomorAntrian = noAntrianJoin,
                                        AngkaAntrian = Convert.ToInt32(noAntrianIncrement),
                                        KodeBooking = kodeBooking,
                                        EstimasiDilayani = Convert.ToInt64(estimasi),
                                        NamaPoli = namaPoli,
                                        NamaDokter = namaDokter,
                                        KuotaJkn = Convert.ToInt32(kuota),
                                        KuotaNonJkn = Convert.ToInt32(kuota),
                                        SisaKuotaJkn = Convert.ToInt32(sisaKuota),
                                        SisaKuotaNonJkn = Convert.ToInt32(sisaKuota),
                                        NoRm = noRm,
                                        //PasienBaru = Convert.ToInt32(PasienBaru),//string.IsNullOrEmpty(noCm) ? 1 : 0,
                                        Keterangan = "peserta harap 60 menit lebih awal guna pencatatan administrasi"
                                    },
                                    Metadata = new Metadata()
                                    {
                                        Message = "Ok",
                                        Code = 200
                                    }
                                };


                                //[20211228] ER
                                sql = "INSERT INTO AntrianOnline (KodeBooking,JenisPasien,NoKartu,NIK,NoHP,KodePoli,NamaPoli,PasienBaru,NoRM, " +
                                      "TglPeriksa,KodeDokter,NamaDokter,JamPraktek,JenisKunjungan,NomorReferensi,NoAntrian, " +
                                      "AngkaAntrian,EstimasiDilayani,SisaKuotaJKN,KuotaJKN,SisaKuotaNonJKN,KuotaNonJKN,Keterangan,NoPendaftaran,TaskID) " +
                                      "VALUES ('" + kodeBooking + "','JKN','" + request.NomorKartu + "','" + request.Nik + "','" + request.NoHp + "'," +
                                      "'" + request.KodePoli + "','" + namaPoli + "','" + (string.IsNullOrEmpty(noCm) ? "0" : "1") + "','" + noRm + "','" + tglReservasi.ToString("yyyy/MM/dd HH:mm:ss") + "'," +
                                      "'" + request.KodeDokter + "','" + namaDokter + "','" + request.JamPraktek + "','" + request.JenisKunjungan + "'," +
                                      "'" + request.NomorReferensi + "','" + noAntrianJoin + "','" + Convert.ToInt32(noAntrianIncrement) + "','" + estimasi + "','" + sisaKuota + "'," +
                                      "'" + kuota + "','" + sisaKuota + "','" + kuota + "','peserta harap 60 menit lebih awal guna pencatatan administrasi',NULL,NULL)";
                                db.ExecuteNonQuery(sql);

                                logger.Info($"INSERT AntrianOnline --> KodeBooking {kodeBooking} Ok");

                                if (!string.IsNullOrEmpty(noCm))
                                {
                                    var kdSubInst = "";
                                    var qSubInstalasi = $@"select * from SubInstalasiRuangan where KdRuangan = '{KdRuanganPoli}'";
                                    var readerSubInst = db.ExecuteQuery(qSubInstalasi);
                                    while (readerSubInst.Read())
                                    {
                                        kdSubInst = readerSubInst["KdSubInstalasi"].ToString();
                                    }

                                    var nonKelas = ConfigurationManager.AppSettings["NonKelas"];
                                    var idUser = ConfigurationManager.AppSettings["IdPegawai"];
                                    var kelompokPasienBpjs = ConfigurationManager.AppSettings["KelompokPasienBpjs"];
                                    var kelompokPasienNonBpjs = ConfigurationManager.AppSettings["KelompokPasienNonBpjs"];
                                    var rujukanAsalSendiri = ConfigurationManager.AppSettings["RujukanAsalSendiri"];
                                    var rujukanAsalPuskesmas = ConfigurationManager.AppSettings["RujukanAsalPuskesmas"];
                                    var kdDetailJenisJasaPelayanan = ConfigurationManager.AppSettings["KdDetailJenisJasaPelayanan"];

                                    var registrasiMrs = new List<SqlParameter>(){
                                        new SqlParameter("@NoCM", SqlDbType.VarChar, 12){Value = noRm},
                                        new SqlParameter("@KdSubInstalasi", SqlDbType.Char, 3){Value = kdSubInst},
                                        new SqlParameter("@KdRuangan", SqlDbType.Char, 3){Value = KdRuanganPoli},
                                        new SqlParameter("@TglPendaftaran", SqlDbType.DateTime){Value = request.TanggalPeriksa},
                                        new SqlParameter("@TglMasuk", SqlDbType.DateTime){Value = request.TanggalPeriksa},
                                        new SqlParameter("@KdKelas", SqlDbType.Char, 2){Value = nonKelas},
                                        new SqlParameter("@KdKelompokPasien", SqlDbType.Char, 2){Value = string.IsNullOrEmpty(request.NomorKartu) ? kelompokPasienNonBpjs:kelompokPasienBpjs},
                                        new SqlParameter("@IdPegawai", SqlDbType.Char, 10){Value = idUser},
                                        new SqlParameter("@OutputNoPendaftaran", SqlDbType.Char, 10){Direction = ParameterDirection.Output},
                                        new SqlParameter("@OutputNoAntrian", SqlDbType.Char, 3){Direction = ParameterDirection.Output},
                                        new SqlParameter("@KdDetailJenisJasaPelayanan", SqlDbType.Char, 2){Value = kdDetailJenisJasaPelayanan},
                                        new SqlParameter("@KdPaket", SqlDbType.VarChar, 3){Value = DBNull.Value},
                                        new SqlParameter("@KdRujukanAsal", SqlDbType.Char, 2){Value = string.IsNullOrEmpty(request.NomorKartu) ? rujukanAsalSendiri:rujukanAsalPuskesmas},
                                        new SqlParameter("@IdDokter", SqlDbType.Char, 10){Value = IdDokter},
                                    };

                                    var conn = new SqlConnection(Connection);
                                    conn.Open();
                                    var trans = conn.BeginTransaction();
                                    var comm = conn.CreateCommand();
                                    comm.CommandType = CommandType.StoredProcedure;
                                    comm.CommandText = "Add_RegistrasiPasienMRS";
                                    comm.Connection = conn;
                                    comm.Parameters.AddRange(registrasiMrs.ToArray());
                                    comm.Transaction = trans;
                                    comm.CommandTimeout = 0;
                                    comm.ExecuteNonQuery();
                                    trans.Commit();
                                    conn.Close();

                                    var noPendaftaran = "";
                                    var outputNoPendaftaran = registrasiMrs.FirstOrDefault(n => n.ParameterName == "@OutputNoPendaftaran");
                                    if (outputNoPendaftaran != null)
                                    {
                                        noPendaftaran = outputNoPendaftaran.Value.ToString();
                                    }

                                    var biaya = new List<SqlParameter>(){
                                        new SqlParameter("@NoPendaftaran", SqlDbType.VarChar, 10){Value = noPendaftaran},
                                        new SqlParameter("@KdSubInstalasi", SqlDbType.Char, 3){Value = kdSubInst},
                                        new SqlParameter("@KdRuangan", SqlDbType.Char, 3){Value = KdRuanganPoli},
                                        new SqlParameter("@TglMasuk", SqlDbType.DateTime){Value = request.TanggalPeriksa},
                                        new SqlParameter("@KdKelas", SqlDbType.Char, 2){Value = nonKelas},
                                        new SqlParameter("@KdKelasPel", SqlDbType.Char, 2){Value = nonKelas},
                                        new SqlParameter("@NoLab_Rad", SqlDbType.Char, 10){Value = DBNull.Value},
                                        new SqlParameter("@IdPegawai", SqlDbType.Char, 10){Value = idUser},
                                        new SqlParameter("@Status", SqlDbType.Char, 1){Value = "AL"},
                                    };

                                    conn = new SqlConnection(Connection);
                                    conn.Open();
                                    trans = conn.BeginTransaction();
                                    comm = conn.CreateCommand();
                                    comm.CommandType = CommandType.StoredProcedure;
                                    comm.CommandText = "Add_BiayaPelayananOtomatis";
                                    comm.Connection = conn;
                                    comm.Parameters.AddRange(biaya.ToArray());
                                    comm.Transaction = trans;
                                    comm.CommandTimeout = 0;
                                    comm.ExecuteNonQuery();
                                    trans.Commit();
                                    conn.Close();

                                    var update = $"update AntrianOnline set NoPendaftaran = '{noPendaftaran}' where kodeBooking = '{kodeBooking}'";
                                    db.ExecuteNonQuery(update);

                                    var addMap = $"insert into MapAntrianToAntrianOnline values ('{kdAntrianIncrement}','{kodeBooking}','MobileJkn')";
                                    db.ExecuteNonQuery(addMap);
                                }

                                return Json(result);
                            }
                            else
                            {
                                return ResultAmbil("Error", 201);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error($"Error KodeBooking {kodeBooking} --> {e.Message}");
                logger.Error(e);
                return ResultAmbil(e.Message, 201);
            }
        }

        /// <summary>
        /// digunakan untuk mendapatkan sisa antrian per pasien berdasarkan kode booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("api/V2/SisaAntrean")]
        [HttpPost]
        public JsonResult<SisaAntrianRespone> SisaAntrian([FromBody] SisaAntrianRequest request)
        {
            try
            {
                VerifyToken();

                if (string.IsNullOrEmpty(request.KodeBooking) || string.IsNullOrWhiteSpace(request.KodeBooking))
                    throw new Exception("isi kode booking");

                using (var db = new ContextManager(Connection))
                {
                    var kdRuangan = "";
                    var cekKodeBooking = "";
                    var tglReservasi = "";
                    var noAntrian = "";
                    var sql = "select top 1 * from Reservasi where KodeBooking='" + request.KodeBooking + "'";
                    var readerRuangan = db.ExecuteQuery(sql);
                    while (readerRuangan.Read())
                    {
                        kdRuangan = readerRuangan["KdRuangan"].ToString();
                        tglReservasi = Convert.ToDateTime(readerRuangan["TglReservasi"]).ToString("yyyy/MM/dd HH:mm:ss");
                        //ROYYMMDD000
                        //noAntrian = readerRuangan["NoReservasi"].ToString().Substring(7, 3);
                        noAntrian = readerRuangan["NoReservasi"].ToString();
                        cekKodeBooking = readerRuangan["KodeBooking"].ToString();
                    }

                    if (string.IsNullOrEmpty(cekKodeBooking))
                    {
                        throw new Exception("kode booking tidak ditemukan");
                    }

                    if (string.IsNullOrEmpty(kdRuangan))
                    {
                        throw new Exception("kode poli tidak sesuai");
                    }

                    //var kuota = 0;
                    //var sisaKuota = 0;
                    //var totalAntrian = 0;
                    //var namaPoli = "";
                    //var namaDokter = "";

                    //sql = "SELECT    ruangan.namaruangan, dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi + "', " + 1 + ", '" + kdRuangan + "') AS JumlahPasienTerdaftar, " +
                    //      "Sloting.KdKelompokWaktu, Sloting.KdRuangan, Sloting.Slotting,   dbo.Ambil_JmlAntrianTerlayani('" + tglReservasi + "') AS SisaSloting, " +
                    //      "DataPegawai.NamaLengkap " +
                    //      @"FROM            Instalasi INNER JOIN
                    //         Ruangan ON Instalasi.KdInstalasi = Ruangan.KdInstalasi INNER JOIN
                    //         Sloting ON Ruangan.KdRuangan = Sloting.KdRuangan LEFT OUTER JOIN
                    //         MapingPeriodeWaktuAwalkeAkhir ON Sloting.KdKelompokWaktu = MapingPeriodeWaktuAwalkeAkhir.KdKelompokWaktu LEFT OUTER JOIN
                    //         DataPegawai INNER JOIN
                    //         DetailReservasi INNER JOIN
                    //         Reservasi ON DetailReservasi.KodeBooking = Reservasi.KodeBooking ON DataPegawai.IdPegawai = DetailReservasi.IdPegawai ON Ruangan.KdRuangan = Reservasi.KdRuangan " +
                    //      "WHERE (Sloting.KdRuangan = '" + kdRuangan + "') AND DetailReservasi.StatusPasien<>'B' ";
                    //sql += "Group by Sloting.KdKelompokWaktu,Sloting.KdRuangan,Sloting.Slotting,ruangan.namaruangan,DataPegawai.NamaLengkap";
                    //var reader = db.ExecuteQuery(sql);
                    //while (reader.Read())
                    //{
                    //    kuota = Convert.ToInt32(reader["Slotting"]);
                    //    sisaKuota = Convert.ToInt32(reader["SisaSloting"]);
                    //    totalAntrian = Convert.ToInt32(reader["JumlahPasienTerdaftar"]);
                    //    namaPoli = reader["NamaRuangan"].ToString();
                    //    namaDokter = reader["NamaLengkap"].ToString();
                    //}

                    //var listAntrian = new List<AntrianPasienRegistrasiModel>();
                    //sql = "select AntrianPasienRegistrasi.*, AntrianLoket.[Call] " +
                    //    "from AntrianPasienRegistrasi  " +
                    //    "inner join AntrianLoket on AntrianPasienRegistrasi.JenisPasien = AntrianLoket.NamaLoket  " +
                    //    "where TglAntrian > '" + tglReservasi + "'";
                    //var readerAntrian = db.ExecuteQuery(sql);
                    //while (readerAntrian.Read())
                    //{
                    //    listAntrian.Add(new AntrianPasienRegistrasiModel()
                    //    {
                    //        KdAntrian = readerAntrian["KdAntrian"].ToInt32(),
                    //        NoLoketCounter = readerAntrian["NoLoketCounter"].ToByte(),
                    //        NoAntrian = readerAntrian["NoAntrian"].ToInt32(),
                    //        Call = readerAntrian["Call"].ToString()
                    //    });
                    //}
                    //var sisaAntrian = listAntrian.Count(n => n.NoLoketCounter == 0);
                    //var antrianDipanggil = listAntrian.LastOrDefault(n => n.NoLoketCounter != 0);

                    //var waktuTunggu = Math.Round((Convert.ToDateTime(tglReservasi) - DateTime.Now).TotalSeconds, 0);


                    var kuota = 0;
                    var sisaKuota = 0;
                    var totalAntrian = 0;
                    var namaPoli = "";
                    var namaDokter = "";
                    var IdPegawai = "";

                    var sqlView = "SELECT NamaRuangan,NamaDokter, IdPegawai FROM V_DetailReservasi WHERE KodeBooking='" + request.KodeBooking + "'";
                    var readerView = db.ExecuteQuery(sqlView);
                    while (readerView.Read())
                    {
                        namaPoli = readerView["NamaRuangan"].ToString();
                        namaDokter = readerView["NamaDokter"].ToString();
                        IdPegawai = readerView["IdPegawai"].ToString();
                    }

                    var query = "SELECT Slotting FROM Sloting WHERE KdRuangan='" + kdRuangan + "' AND KdKelompokWaktu='" + KodeKelompokWaktu + "'";
                    var reader = db.ExecuteQuery(query);
                    while (reader.Read())
                    {
                        kuota = Convert.ToInt32(reader["Slotting"]);
                    }

                    var sqlTotal = "SELECT dbo.Jumlah_Pasien_Terdaftar_v2('" + tglReservasi + "'," + KodeKelompokWaktu + ",'" + kdRuangan + "','" + IdPegawai + "') AS JumlahPasienTerdaftar";
                    var readerTotal = db.ExecuteQuery(sqlTotal);
                    while (readerTotal.Read())
                    {
                        totalAntrian = Convert.ToInt32(readerTotal["JumlahPasienTerdaftar"]);
                    }

                    sisaKuota = kuota - totalAntrian;

                    var antrianDipanggil = 0;
                    var sisaAntrian = 0;
                    sql = "SELECT COUNT(dbo.Reservasi.KodeBooking) AS JmlDipanggil FROM dbo.Reservasi INNER JOIN dbo.DetailReservasi ON " +
                          "dbo.Reservasi.KodeBooking = dbo.DetailReservasi.KodeBooking AND dbo.Reservasi.KdRuangan = dbo.DetailReservasi.KdRuangan " +
                          "WHERE YEAR(TglReservasi)= '" + Convert.ToDateTime(tglReservasi).Year + "' " +
                          "AND MONTH(TglReservasi)= '" + Convert.ToDateTime(tglReservasi).Month + "' " +
                          "AND DAY(TglReservasi)= '" + Convert.ToDateTime(tglReservasi).Day + "' " +
                          "AND StatusPasien = 'Y'" +
                          "AND DetailReservasi.IdPegawai = '" + IdPegawai + "'";
                    var readerAntrian = db.ExecuteQuery(sql);
                    while (readerAntrian.Read())
                    {
                        antrianDipanggil = Convert.ToInt32(readerAntrian["JmlDipanggil"]);
                    }

                    sisaAntrian = totalAntrian - antrianDipanggil;

                    var waktuTunggu = sisaAntrian * 480;

                    var result = new SisaAntrianRespone()
                    {
                        Response = new SisaAntrianDetail()
                        {
                            NamaPoli = namaPoli,
                            NamaDokter = namaDokter,
                            SisaAntrian = sisaAntrian,
                            Keterangan = "-",
                            NomorAntrian = noAntrian,
                            WaktuTunggu = waktuTunggu,
                            AntrianDipanggil = Convert.ToString(antrianDipanggil) //antrianDipanggil != null ? antrianDipanggil.Call + "-" + antrianDipanggil.NoAntrian : "-"
                        },
                        Metadata = new Metadata()
                        {
                            Code = 200,
                            Message = "Ok"
                        }
                    };
                    return Json(result);
                }
            }
            catch (Exception e)
            {
                return ResultSisa(e.Message, 201);
            }
        }

        /// <summary>
        /// digunakan untuk membuat pasien baru dan mendapatkan nomor rekam medis baru
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("api/V2/PasienBaru")]
        [HttpPost]
        public JsonResult<PasienBaruRespone> PasienBaru([FromBody] PasienBaruRequest request)
        {
            try
            {
                VerifyToken();

                var getProperties = request.GetType().GetProperties();
                foreach (var property in getProperties)
                {
                    var name = property.Name.ToLower();
                    var isi = property.GetValue(request);
                    if (isi == null)
                        throw new Exception("isi " + name);
                    if (string.IsNullOrEmpty(isi.ToString()) || string.IsNullOrWhiteSpace(isi.ToString()))
                        throw new Exception("isi " + name);
                }

                var isPasienExist = false;
                var noRm = "";
                var sql = "";

                using (var db = new ContextManager(Connection))
                {
                    sql = "select top 1 * from Pasien where NoIdentitas = '" + request.Nik + "'";
                    var reader = db.ExecuteQuery(sql);
                    while (reader.Read())
                    {
                        isPasienExist = true;
                        noRm = reader["NoCM"].ToString();
                    }
                }

                if (!isPasienExist)
                {
                    var conn = new SqlConnection(Connection);
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var comm = conn.CreateCommand();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = "AU_Pasien";
                    comm.Connection = conn;
                    var listPasien = new List<SqlParameter>()
                    {
                        new SqlParameter("@NoCM", SqlDbType.VarChar, 12){Value = DBNull.Value},
                        new SqlParameter("@NoIdentitas", SqlDbType.VarChar, 30){Value = request.Nik},
                        new SqlParameter("@TglDaftarMembership", SqlDbType.DateTime){Value = DateTime.Now},
                        new SqlParameter("@TitlePasien", SqlDbType.VarChar, 4){Value = ""},
                        new SqlParameter("@NamaLengkap", SqlDbType.VarChar, 100){Value = request.Nama},
                        new SqlParameter("@NamaPanggilan", SqlDbType.VarChar, 100){Value = request.Nama},
                        new SqlParameter("@TempatLahir", SqlDbType.VarChar, 25){Value =""},
                        new SqlParameter("@TglLahir", SqlDbType.DateTime){Value = request.TanggalLahir},
                        new SqlParameter("@JenisKelamin", SqlDbType.Char, 1){Value = request.JenisKelamin},
                        new SqlParameter("@Alamat", SqlDbType.VarChar, 100){Value = request.Alamat},
                        new SqlParameter("@Telepon", SqlDbType.VarChar, 20){Value = request.NoHp},
                        new SqlParameter("@Propinsi", SqlDbType.VarChar, 50){Value = request.NamaProp},
                        new SqlParameter("@Kota", SqlDbType.VarChar, 50){Value = request.NamaDati2},
                        new SqlParameter("@Kecamatan", SqlDbType.VarChar, 50){Value = request.NamaKec},
                        new SqlParameter("@Kelurahan", SqlDbType.VarChar, 50){Value = request.NamaKel},
                        new SqlParameter("@RTRW", SqlDbType.VarChar, 20){Value = request.Rt +"/"+ request.Rw},
                        new SqlParameter("@KodePos", SqlDbType.VarChar, 5){Value = ""},
                        new SqlParameter("@OutputNoCM", SqlDbType.VarChar, 12){Direction = ParameterDirection.Output},
                        new SqlParameter("@IdUser", SqlDbType.VarChar, 10){Value = IdPegawai}
                    };
                    comm.Parameters.AddRange(listPasien.ToArray());
                    comm.Transaction = trans;
                    comm.CommandTimeout = 0;
                    comm.ExecuteNonQuery();
                    trans.Commit();
                    noRm = listPasien.FirstOrDefault(n => n.ParameterName == "@OutputNoCM").Value.ToString();
                    comm.Dispose();
                    conn.Close();

                    conn.Open();
                    trans = conn.BeginTransaction();
                    comm = conn.CreateCommand();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = "AU_DetailPasien";
                    comm.Connection = conn;
                    var listDetailPasien = new List<SqlParameter>()
                    {
                        new SqlParameter("@NoCM", SqlDbType.VarChar, 12){Value = noRm},
                        new SqlParameter("@NamaKeluarga", SqlDbType.VarChar, 100){Value = ""},
                        new SqlParameter("@WargaNegara", SqlDbType.Char, 1){Value = ""},
                        new SqlParameter("@GolDarah", SqlDbType.VarChar, 2){Value = ""},
                        new SqlParameter("@Rhesus", SqlDbType.Char, 1){Value = ""},
                        new SqlParameter("@StatusNikah", SqlDbType.VarChar, 50){Value = ""},
                        new SqlParameter("@Pekerjaan", SqlDbType.VarChar, 50){Value =""},
                        new SqlParameter("@Agama", SqlDbType.VarChar, 20){Value = ""},
                        new SqlParameter("@Suku", SqlDbType.VarChar, 20){Value = ""},
                        new SqlParameter("@Pendidikan", SqlDbType.VarChar, 25){Value = ""},
                        new SqlParameter("@NamaAyah", SqlDbType.VarChar, 100){Value =""},
                        new SqlParameter("@NamaIbu", SqlDbType.VarChar, 100){Value = ""},
                        new SqlParameter("@NamaIstriSuami", SqlDbType.VarChar, 100){Value = ""},
                        new SqlParameter("@NoKK", SqlDbType.VarChar, 30){Value = request.NomorKk},
                        new SqlParameter("@NamaKepalaKeluarga", SqlDbType.VarChar, 100){Value = ""}
                    };
                    comm.Parameters.AddRange(listDetailPasien.ToArray());
                    comm.Transaction = trans;
                    comm.CommandTimeout = 0;
                    comm.ExecuteNonQuery();
                    trans.Commit();
                    comm.Dispose();
                    conn.Close();

                    conn.Open();
                    trans = conn.BeginTransaction();
                    comm = conn.CreateCommand();
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = "Add_AsuransiPasien";
                    comm.Connection = conn;
                    var listAsuransi = new List<SqlParameter>()
                    {
                        new SqlParameter("@IdPenjamin", SqlDbType.Char, 10){Value = IdPenjamin},
                        new SqlParameter("@IdAsuransi", SqlDbType.VarChar, 25){Value = request.NomorKartu},
                        new SqlParameter("@NoCM", SqlDbType.VarChar, 12){Value = noRm},
                        new SqlParameter("@KdHubKeluarga", SqlDbType.Char, 2){Value = ""},
                        new SqlParameter("@NamaPeserta", SqlDbType.VarChar, 50){Value = request.Nama},
                        new SqlParameter("@IDPeserta", SqlDbType.VarChar, 16){Value = request.Nik},
                        new SqlParameter("@KdGolongan", SqlDbType.Char, 2){Value =""},
                        new SqlParameter("@TglLahir", SqlDbType.DateTime){Value = request.TanggalLahir},
                        new SqlParameter("@Alamat", SqlDbType.VarChar, 100){Value = request.Alamat},
                        new SqlParameter("@KdInstitusiAsal", SqlDbType.VarChar, 4){Value = ""}
                    };
                    comm.Parameters.AddRange(listAsuransi.ToArray());
                    comm.Transaction = trans;
                    comm.CommandTimeout = 0;
                    comm.ExecuteNonQuery();
                    trans.Commit();
                    comm.Dispose();
                    conn.Close();
                }
                else
                {
                    throw new Exception("pasien sudah terdaftar dengan nomor rekam medis " + noRm);
                }

                var result = new PasienBaruRespone()
                {
                    Response = new PasienBaruDetail()
                    {
                        NoRm = noRm
                    },
                    Metadata = new Metadata()
                    {
                        Code = 200,
                        Message = "Harap datang ke admisi untuk melengkapi data rekam medis"
                    }
                };
                return Json(result);
            }
            catch (Exception e)
            {
                return ResultPasienBaru(e.Message, 201);
            }
        }

        private void VerifyToken()
        {
            if (!IsTokenExist)
                throw new Exception("token tidak ditemukan");
            if (!IsUsernameExist)
                throw new Exception("username tidak ditemukan");

            var token = Request.Headers.GetValues("x-token").FirstOrDefault();
            var deskripsiToken = Helper.ValidateToken(token);
            if (deskripsiToken != null)
            {
                var sql = "select * from UserApiJkn where userName ='" + deskripsiToken.UserName + "' and password='" + deskripsiToken.password + "'";
                var conn = new ContextManager(Connection);
                var isTokenVerify = false;
                var readToken = conn.ExecuteQuery(sql);
                while (readToken.Read())
                {
                    isTokenVerify = true;
                }

                if (!isTokenVerify)
                {
                    throw new Exception("token tidak sesuai");
                }
            }
        }

        private JsonResult<Metadata> Result(string message, int code)
        {
            return Json(new Metadata()
            {
                Message = message,
                Code = code
            });
        }

        private JsonResult<BatalResponse> ResultBatal(string message, int code)
        {
            return Json(new BatalResponse()
            {
                Metadata = new Metadata()
                {
                    Message = message,
                    Code = code
                }
            });
        }

        private JsonResult<StatusAntrianRespone> ResultStatus(string message, int code)
        {
            return Json(new StatusAntrianRespone()
            {
                Metadata = new Metadata()
                {
                    Message = message,
                    Code = code
                }
            });
        }

        private JsonResult<AmbilAntrianRespone> ResultAmbil(string message, int code)
        {
            return Json(new AmbilAntrianRespone()
            {
                Metadata = new Metadata()
                {
                    Message = message,
                    Code = code
                }
            });
        }

        private JsonResult<SisaAntrianRespone> ResultSisa(string message, int code)
        {
            return Json(new SisaAntrianRespone()
            {
                Metadata = new Metadata()
                {
                    Message = message,
                    Code = code
                }
            });
        }

        private JsonResult<PasienBaruRespone> ResultPasienBaru(string message, int code)
        {
            return Json(new PasienBaruRespone()
            {
                Metadata = new Metadata()
                {
                    Message = message,
                    Code = code
                }
            });
        }

        private bool IsTokenExist
        {
            get
            {
                IEnumerable<string> tokens;
                var isExist = Request.Headers.TryGetValues("x-token", out tokens);
                if (!isExist) return false;
                var token = tokens.FirstOrDefault();
                if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                    return false;
                return true;
            }
        }

        private bool IsUsernameExist
        {
            get
            {
                IEnumerable<string> usernames;
                var isExist = Request.Headers.TryGetValues("x-username", out usernames);
                if (!isExist) return false;
                var username = usernames.FirstOrDefault();
                if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                    return false;
                return true;
            }
        }

        private string Connection { get; } = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;

        private string ConsumerId { get; } = ConfigurationManager.AppSettings["ConsumerId"];

        private string PasswordKey { get; } = ConfigurationManager.AppSettings["PasswordKey"];

        private string UserKey { get; set; } = ConfigurationManager.AppSettings["UserKey"];

        private string IsEncrypt { get; set; } = ConfigurationManager.AppSettings["IsEncrypt"];

        private string UrlHost { get; } = ConfigurationManager.AppSettings["Url"];

        private string KodeJenisPasien { get; } = ConfigurationManager.AppSettings["KodeJenisPasien"];

        private string KodeKelompokWaktu { get; } = ConfigurationManager.AppSettings["KodeKelompokWaktu"];

        private string IdPenjamin { get; } = ConfigurationManager.AppSettings["IdPenjamin"];

        private string IdPegawai { get; } = ConfigurationManager.AppSettings["IdPegawai"];
        public string IdDokter { get; private set; }

        private string GetNoReservasi(DateTime? tglReservasi)
        {
            try
            {
                var tgl = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                var tglAwal = tglReservasi.Value.ToString("yyyy-MM-dd 00:00:00");
                var tglAkhir = tglReservasi.Value.ToString("yyyy-MM-dd 23:59:59");
                using (var context = new ContextManager(Connection))
                {
                    var query = "select max(NoReservasi) as NoAntrian from reservasi " +
                        "where TglReservasi between '" + tglAwal + "' " +
                        "and '" + tglAkhir + "'";
                    var reader = context.ExecuteQuery(query);
                    while (reader.Read())
                    {
                        string final;
                        var noAntrian = reader["NoAntrian"].ToString();

                        if (!string.IsNullOrEmpty(noAntrian))
                        {
                            var last = Convert.ToInt32(noAntrian.Substring(8, 3));
                            last += 1;
                            final = "RO" + Convert.ToString(tglReservasi.Value.ToString("yyMMdd")) + last.ToString("D3");
                        }
                        else
                        {
                            final = "RO" + Convert.ToString(tglReservasi.Value.ToString("yyMMdd")) + "001";
                        }
                        return final;
                    }
                    return "";
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}