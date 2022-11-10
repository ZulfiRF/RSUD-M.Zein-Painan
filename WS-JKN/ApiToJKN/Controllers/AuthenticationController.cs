using ApiToJKN.Models;
using ApiToJKN.Models.Requests;
using ApiToJKN.Models.Responses;
using ApiToJKN.Utilities;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using Core.Framework.Model.Attr;
using Core.Framework.Model.Contract;
using Core.Framework.Model.Error;
using Core.Framework.Model.Impl;
using Core.Framework.Model.Impl.Postgresql;
using Core.Framework.Model.Impl.SqlServer;
using Core.Framework.Model;
using Core.Framework.Helper;
using System.Collections.Generic;
using System.Globalization;
using Core.Framework.Helper.Extention;
using Core.Framework.Helper.Date;

namespace ApiToJKN.Controllers
{
    public class AuthenticationController : ApiController
    {
        [HttpGet]
        public JsonResult<AuthenticationResponse> Authentication()
        {
            AuthenticationResponse result;
            var connection = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            using (var db = new DbMedifirstDataContext(connection))
            {

                IEnumerable<string> usernames;
                IEnumerable<string> passwords;
                var isExistUsename = Request.Headers.TryGetValues("x-username", out usernames);
                if (!isExistUsename)
                    throw new Exception("x-username tidak ditemukan");
                var isExistPassword = Request.Headers.TryGetValues("x-password", out passwords);
                if (!isExistPassword)
                    throw new Exception("x-password tidak ditemukan");

                var username = usernames.FirstOrDefault();
                var password = passwords.FirstOrDefault();

                var getUser = db.UserApiJkns
                    .FirstOrDefault(n => n.userName == username && n.password == password);
                if (getUser != null)
                {

                    var token = Helper.CreateToken(getUser.userName, getUser.password);
                    result = new AuthenticationResponse()
                    {
                        Response = new Response()
                        {
                            Token = token
                        },
                        Metadata = new Metadata()
                        {
                            Message = "Ok",
                            Code = 200
                        }
                    };
                    return Json(result);

                }
                result = new AuthenticationResponse()
                {
                    Response = new Response()
                    {
                        Token = null
                    },
                    Metadata = new Metadata()
                    {
                        Message = "user tidak ditemukan",
                        Code = 401
                    }
                };
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult<AuthenticationResponse> Authentication([FromBody] AuthenticationRequest request)
        {
            AuthenticationResponse result;
            var connection = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            using (var db = new DbMedifirstDataContext(connection))
            {

                var getUser = db.UserApiJkns
                    .FirstOrDefault(n => n.userName == request.Username && n.password == request.Password);
                if (getUser != null)
                {

                    var token = Helper.CreateToken(getUser.userName, getUser.password);
                    result = new AuthenticationResponse()
                    {
                        Response = new Response()
                        {
                            Token = token
                        },
                        Metadata = new Metadata()
                        {
                            Message = "Ok",
                            Code = 200
                        }
                    };
                    return Json(result);

                }
                result = new AuthenticationResponse()
                {
                    Response = new Response()
                    {
                        Token = null
                    },
                    Metadata = new Metadata()
                    {
                        Message = "user tidak ditemukan",
                        Code = 401
                    }
                };
            }
            return Json(result);
        }



        [Route("GetRekapAntrian")]
        [HttpPost]
        public JsonResult<SlotingResponse> CheckDataSloting([FromBody] ChekSlotingReques request)
        {
            DateTime LastUpdate = DateTime.Now;
            var connection = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            var sloting = new slotingModel();
            var pasien = new PasienModel();
            var lastupdateUse = "";
            var tglReservasi = request.TanggalPeriksa;
            var tglValidasi = request.TanggalPeriksa.Substring(8, 2);
            Int32 thnV = Convert.ToInt32(request.TanggalPeriksa.Substring(0, 4));
            Int32 blnV = Convert.ToInt32(request.TanggalPeriksa.Substring(5, 2));
            var token = Request.Headers.GetValues("x-token").FirstOrDefault();
            if (ConfigurationManager.AppSettings["polieksekutif"] != "1")
            {
                if (request.PoliEksekutif == "1")
                {
                    var error = new SlotingResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Tidak Ada polieksekutif",
                            Code = 401
                        }
                    };
                    return Json(error);

                }
                else
                {
                    goto nextPoli;
                }

            }
        nextPoli:;
            if (Convert.ToInt32(tglValidasi) > DateTime.DaysInMonth(thnV, blnV))
            {
                var error = new SlotingResponse()
                {

                    Metadata = new Metadata()
                    {
                        Message = "Tgl Tidak Bisa Diproses",
                        Code = 401
                    }
                };
                return Json(error);
            }
            if (token == null)
            {
                var error = new SlotingResponse()
                {

                    Metadata = new Metadata()
                    {
                        Message = "token kosong",
                        Code = 401
                    }
                };
                return Json(error);
            }

            var validateToken = Helper.ValidateToken(token);
            if (validateToken != null)
            {
                var user = validateToken.UserName;
                var valid = new userApiModel();
                var sql1 = "select * from UserApiJkn where userName ='" + validateToken.UserName + "' and password='" + validateToken.password + "'";
                var conn = new ContextManager(connection);
                var reader01 = conn.ExecuteQuery(sql1);

                while (reader01.Read())
                {
                    valid.status = reader01["status"].ToString();
                }
                if (valid != null)
                {
                    goto nextStep;
                }
                else
                {
                    var error = new SlotingResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Data Tidak Ditemukan",
                            Code = 401
                        }
                    };
                    return Json(error);
                }

            }

        nextStep:
            SlotingResponse result;
            var kdKelompokWaktu = 1;
            //if (Convert.ToInt32(DateTime.Now.Hour) >= 12)
            //{
            //    kdKelompokWaktu = 2;
            //}
            //else
            //{
            //    kdKelompokWaktu = 1;
            //}



            using (var db = new ContextManager(connection))
            {
                if (request.KodePoli == "" || request.KodePoli == null)
                {
                    var error = new SlotingResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Pilih Ruangan Tujuan",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                var ruanganUse = "";

                var chek = "select KdRuangan from ruangan where KodeExternal='" + request.KodePoli + "'";
                var readerChek = db.ExecuteQuery(chek);
                while (readerChek.Read())
                {
                    ruanganUse = readerChek["KdRuangan"].ToString();
                }
                if (ruanganUse == "")
                {
                    var error = new SlotingResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Ruangan Tujuan tidak Ada",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                var query =
                "SELECT    ruangan.namaruangan, dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi + "', " + kdKelompokWaktu + ", '" + ruanganUse + "') AS JumlahPasienTerdaftar, Sloting.KdKelompokWaktu, Sloting.KdRuangan, Sloting.Slotting,   dbo.Ambil_JmlAntrianTerlayani('" + tglReservasi + "') AS SisaSloting FROM" +
                " reservasi INNER JOIN Ruangan ON reservasi.KdRuangan = Ruangan.KdRuangan INNER JOIN Instalasi ON Ruangan.KdInstalasi = Instalasi.KdInstalasi INNER JOIN " +
                "Sloting ON Ruangan.KdRuangan = Sloting.KdRuangan INNER JOIN MapingPeriodeWaktuAwalkeAkhir " +
                "ON Sloting.KdKelompokWaktu = MapingPeriodeWaktuAwalkeAkhir.KdKelompokWaktu WHERE " +
                " (Sloting.KdRuangan = '" + ruanganUse + "') Group by Sloting.KdKelompokWaktu,Sloting.KdRuangan,Sloting.Slotting,ruangan.namaruangan";
                var reader = db.ExecuteQuery(query);
                while (reader.Read())
                {
                    sloting.Sloting = Convert.ToInt32(reader["Slotting"]);
                    sloting.SisaSlotting = Convert.ToInt32(reader["SisaSloting"]);
                    sloting.JumlahPasienTerdaftar = Convert.ToInt32(reader["JumlahPasienTerdaftar"]);
                    sloting.namaRuangan = reader["NamaRuangan"].ToString();
                }
                if (sloting != null)
                {
                    lastupdateUse = (Convert.ToString(Core.Framework.Helper.Date.DateHelper.ToInteger(DateTime.Now)));
                    result = new SlotingResponse()
                    {
                        Response = new ResponseChekSloting
                        {
                            NamaPoli = sloting.namaRuangan,
                            TotalAntrean = sloting.JumlahPasienTerdaftar,
                            JumlahTerlayani = sloting.SisaSlotting,

                            lastupdate = Convert.ToInt64(lastupdateUse)

                        },
                        Metadata = new Metadata()
                        {
                            Message = "ok",
                            Code = 200
                        }

                    };
                    return Json(result);

                }
                else
                {
                    result = new SlotingResponse()
                    {




                        Metadata = new Metadata()
                        {
                            Message = "Error",
                            Code = 401
                        }

                    };
                    return Json(result);

                }

            }
        }

        private string UserKey { get; set; } = ConfigurationManager.AppSettings["UserKey"];

        private string IsEncrypt { get; set; } = ConfigurationManager.AppSettings["IsEncrypt"];

        private Dictionary<string, object> dict;

        [Route("GetAntrian")]
        [HttpPost]
        public JsonResult<GetNoAntrianResponse> DaftarReservasi([FromBody] DaftarAntean request)
        {


            var list = new List<string[]>();
            var temp = new List<string>();

            Jasamedika.Sdk.Vclaim.ContextVclaim context = new Jasamedika.Sdk.Vclaim.ContextVclaim();
            context.ConsumerId = ConfigurationManager.AppSettings["ConsumerId"];
            context.PasswordKey = ConfigurationManager.AppSettings["PasswordKey"];
            context.Url = ConfigurationManager.AppSettings["Url"];
            context.UserKey = UserKey;
            context.IsEncrypt = IsEncrypt.ToInt16();
            context.IsByPassSsl = 1;
            var url = ConfigurationManager.AppSettings["Url"];


            DateTime LastUpdate = DateTime.Now;
            var pasien = new PasienModel();
            var IdPenjamin = ConfigurationManager.AppSettings["KodeJenisPasien"];
            var connection = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            var sloting = new slotingModel();


            var tglReservasi = Convert.ToDateTime(request.TanggalPeriksa);

            var isTrue = new PasienDaftarModel();
            GetNoAntrianResponse result;
            int jsnReq = Convert.ToInt32(request.JenisRequest);

            if (request.JenisReferensi != "2" && request.JenisReferensi != "1")
            {
                var error = new GetNoAntrianResponse()
                {

                    Metadata = new Metadata()
                    {
                        Message = "Jenis Referensi Tidak Sesuai",
                        Code = 401
                    }
                };
                return Json(error);
            }

            if (request.JenisRequest != "2" && request.JenisRequest != "1")
            {
                var error = new GetNoAntrianResponse()
                {

                    Metadata = new Metadata()
                    {
                        Message = "Jenis Request Tidak Sesuai",
                        Code = 401
                    }
                };
                return Json(error);
            }

            if (Convert.ToInt32(ConfigurationManager.AppSettings["Rujukan"].ToString()) != 0)
            {
                if (Convert.ToInt32(request.JenisReferensi) == 2)
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Jenis Referensi Tidak Aktif",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
            }
            if (Convert.ToInt32(ConfigurationManager.AppSettings["Kontrol"].ToString()) != 0)
            {
                if (Convert.ToInt32(request.JenisReferensi) == 1)
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Jenis Referensi Tidak Aktif",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
            }
            if (ConfigurationManager.AppSettings["polieksekutif"] != "1")
            {
                if (request.PoliEksekutif == "1")
                {
                    var error = new GetNoAntrianResponse()
                    {
                        Metadata = new Metadata()
                        {
                            Message = "Tidak Ada polieksekutif",
                            Code = 401
                        }
                    };
                    return Json(error);

                }
                else
                {
                    goto nextPoli;
                }

            }
        nextPoli:;

            var kdKelompokWaktu = ConfigurationManager.AppSettings["KodeKelompokWaktu"].ToString();

            if (jsnReq > 2)
            {
                var error = new GetNoAntrianResponse()
                {

                    Metadata = new Metadata()
                    {
                        Message = "Jenis Reques Tidak Tersedia",
                        Code = 402
                    }
                };
                return Json(error);
            }


            using (var db = new ContextManager(connection))
            {

                var token = Request.Headers.GetValues("x-token").FirstOrDefault();
                if (token == null)
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "token kosong",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                var validateToken = Helper.ValidateToken(token);
                if (validateToken != null)
                {

                    var user = validateToken.UserName;
                    var valid = new userApiModel();
                    var sql1 = "select * from UserApiJkn where userName ='" + validateToken.UserName + "' and password='" + validateToken.password + "'";
                    var conn = new ContextManager(connection);
                    var reader01 = conn.ExecuteQuery(sql1);
                    while (reader01.Read())
                    {
                        valid.status = reader01["status"].ToString();
                    }
                    if (valid != null)
                    {
                        goto nextStep;
                    }
                    if (pasien != null)
                    {
                        goto nextStep;
                    }
                    else
                    {
                        var error = new GetNoAntrianResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "Data Tidak Ditemukan",
                                Code = 401
                            }
                        };
                        return Json(error);
                    }

                }

            nextStep:

                var hariReq = Convert.ToInt32(Convert.ToDateTime(request.TanggalPeriksa).DayOfWeek);
                var ambil = new TanggalViewModel();
                if (hariReq != 0 && hariReq != 7 && hariReq != 6)
                {
                    goto nextStep2;
                }
                else
                {

                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Maaf Hari ini sedang libur ",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
            nextStep2:
                var tglSkrg = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var tglYgDipilih = Convert.ToDateTime(request.TanggalPeriksa);
                if (tglYgDipilih >= tglSkrg.Add(TimeSpan.FromDays(1)) && tglYgDipilih <= tglSkrg.Add(TimeSpan.FromDays(7)))
                {
                    goto nextStep1;
                }
                else if (request.TanggalPeriksa == "")
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Tgl pendaftaran tidak boleh kosong ",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                else
                {
                    //responEror result1;
                    var result1 = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Tgl pendaftaran tidak boleh kurang dari  +1 hari dan lebih dari +7 hari ",
                            Code = 401
                        }
                    };
                    return Json(result1);

                }
            nextStep1:

                foreach (var str in context.CariPesertaByNoKartuBpjs(request.NoKartu, DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    if (str.Contains("=============") || str.Contains(">>"))
                    {
                        list.Add(temp.ToArray());
                        //temp = new List<string>();
                    }
                    else if (str.Contains(">>"))
                    {
                        list.Add(temp.ToArray());
                    }
                    else
                    {
                        temp.Add(str);
                    }
                }
                dict = new Dictionary<string, object>();
                var item = list.FirstOrDefault();
                foreach (var n in temp)
                {
                    if (n.Contains(":"))
                    {
                        object obj;
                        if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                        {
                            dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                        }
                    }
                }
                var validasi = dict.Keys.FirstOrDefault().ToString();
                var validasi1 = dict.Values.FirstOrDefault().ToString();
                if (validasi == "error")
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = validasi1,
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                else
                {
                    temp = new List<string>();
                    list = new List<string[]>();
                    if (request.NomorReferensi != "")
                    {
                        foreach (var str in context.RujukanPcareByNoRujukan(request.NomorReferensi))
                        {
                            if (str.Contains("=============") || str.Contains(">>"))
                            {
                                list.Add(temp.ToArray());
                                //temp = new List<string>();
                            }
                            else if (str.Contains(">>"))
                            {
                                list.Add(temp.ToArray());
                            }
                            else
                            {
                                temp.Add(str);
                            }
                        }
                        dict = new Dictionary<string, object>();

                        foreach (var n in temp)
                        {
                            if (n.Contains(":"))
                            {
                                object obj;
                                if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                                {
                                    dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                                }
                            }
                        }

                        if (dict.Count < 10)
                        {
                            foreach (var str in context.RujukanRsByNoRujukan(request.NomorReferensi))
                            {
                                if (str.Contains("=============") || str.Contains(">>"))
                                {
                                    list.Add(temp.ToArray());
                                    //temp = new List<string>();
                                }
                                else if (str.Contains(">>"))
                                {
                                    list.Add(temp.ToArray());
                                }
                                else
                                {
                                    temp.Add(str);
                                }
                            }
                            dict = new Dictionary<string, object>();

                            foreach (var n in temp)
                            {
                                if (n.Contains(":"))
                                {
                                    object obj;
                                    if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                                    {
                                        dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                                    }
                                }
                            }

                            if (dict.Count < 10)
                            {
                                var error = new GetNoAntrianResponse()
                                {

                                    Metadata = new Metadata()
                                    {
                                        Message = "Rujukan Tidak Ada",
                                        Code = 401
                                    }
                                };
                                return Json(error);
                            }
                        }
                    }
                    else
                    {
                        temp = new List<string>();
                        list = new List<string[]>();
                        foreach (var str in context.RujukanPcareByNoKartu(request.NoKartu))
                        {
                            if (str.Contains("=============") || str.Contains(">>"))
                            {
                                list.Add(temp.ToArray());
                                //temp = new List<string>();
                            }
                            else if (str.Contains(">>"))
                            {
                                list.Add(temp.ToArray());
                            }
                            else
                            {
                                temp.Add(str);
                            }
                        }
                        dict = new Dictionary<string, object>();

                        foreach (var n in temp)
                        {
                            if (n.Contains(":"))
                            {
                                object obj;
                                if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                                {
                                    dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                                }
                            }
                        }
                        if (dict.Count <= 23)
                        {

                            foreach (var str in context.RujukanRsByNoKartu(request.NoKartu))
                            {
                                if (str.Contains("=============") || str.Contains(">>"))
                                {
                                    list.Add(temp.ToArray());
                                    //temp = new List<string>();
                                }
                                else if (str.Contains(">>"))
                                {
                                    list.Add(temp.ToArray());
                                }
                                else
                                {
                                    temp.Add(str);
                                }
                            }
                            dict = new Dictionary<string, object>();

                            foreach (var n in temp)
                            {
                                if (n.Contains(":"))
                                {
                                    object obj;
                                    if (!dict.TryGetValue(n.Split(':')[0].ToLower(), out obj))
                                    {
                                        dict.Add(n.Split(':')[0].ToLower(), n.Split(':')[1]);
                                    }
                                }
                            }

                            if (dict.Count <= 23)
                            {
                                var error = new GetNoAntrianResponse()
                                {

                                    Metadata = new Metadata()
                                    {
                                        Message = "Rujukan Tidak Ada",
                                        Code = 401
                                    }
                                };
                                return Json(error);
                            }
                        }
                    }
                    var validasi2 = dict.Keys.FirstOrDefault().ToString();
                    var validasi3 = dict.Values.FirstOrDefault().ToString();
                    var dictNoKartu = dict.Keys.FirstOrDefault(n => n.Contains("nokartu"));
                    var dictTglkunjungan = dict.Keys.FirstOrDefault(n => n.Contains("tglkunjungan"));

                    if (validasi2 == "error")
                    {
                        var error = new GetNoAntrianResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = validasi3,
                                Code = 401
                            }
                        };
                        return Json(error);
                    }
                    else if (dict[dictNoKartu].ToString() != request.NoKartu.ToString())
                    {
                        var error = new GetNoAntrianResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "No Rujukan Tidak Sesuai Dengan No Kartu",
                                Code = 401
                            }
                        };
                        return Json(error);
                    }

                    else if (Convert.ToInt32(DateTime.Now.Subtract(Convert.ToDateTime(dict[dictTglkunjungan].ToString())).TotalDays) <= 90)
                    {
                        goto LolosRUjukan;

                    }
                    else
                    {
                        var error = new GetNoAntrianResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "Tgl Rujukan Sudak Tidak Aktif",
                                Code = 401
                            }
                        };
                        return Json(error);
                    }

                }
            LolosRUjukan:;
                var chekPesertaByNoKartu = "select NoCm from PemakaianAsuransi where  IdAsuransi = '" + request.NoKartu + "'";
                var readerChekPeserta = db.ExecuteQuery(chekPesertaByNoKartu);
                while (readerChekPeserta.Read())
                {
                    request.NomorRm = readerChekPeserta["NoCm"].ToString();
                }

                if (request.KodePoli == "" || request.KodePoli == null)
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Pilih Ruangan Tujuan",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                var ruanganUse = "";

                var chek = "select KdRuangan from ruangan where KodeExternal='" + request.KodePoli + "'";
                var readerChek = db.ExecuteQuery(chek);
                while (readerChek.Read())
                {
                    ruanganUse = readerChek["KdRuangan"].ToString();
                    request.KodePoli = readerChek["KdRuangan"].ToString();
                }
                if (ruanganUse == "")
                {
                    var error = new GetNoAntrianResponse()
                    {
                        Metadata = new Metadata()
                        {
                            Message = "Ruangan Tujuan tidak Ada",
                            Code = 401
                        }
                    };
                    return Json(error);
                }

                var query3 = "";
                if (request.NomorRm == "" || request.NomorRm == null)
                {
                    query3 = "select *  FROM         Reservasi INNER JOIN " +
                                " DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking " +
                                 " where DetailReservasi.noidentitas='" + request.Nik + "' AND StatusPasien <> 'B' and month(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("MM") + "' and  year(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("yyyy") + "' and  day(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("dd") + "'";
                }
                else
                {
                    query3 = "select *  FROM         Reservasi INNER JOIN " +
                               " DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking " +
                                " where DetailReservasi.nocm='" + request.NomorRm + "' AND StatusPasien <> 'B' and month(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("MM") + "' and  year(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("yyyy") + "' and  day(Reservasi.TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).ToString("dd") + "'";

                }

                var reader4 = db.ExecuteQuery(query3);

                while (reader4.Read())
                {
                    isTrue.NoCm = reader4["NoCm"].ToString();
                }

                if (isTrue.NoCm == null)
                {

                    var query = "SELECT    ruangan.namaruangan, dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi.ToString("yyyy-MM-dd HH:mm:ss") + "', " + kdKelompokWaktu + ", '" + request.KodePoli + "') AS JumlahPasienTerdaftar, Sloting.KdKelompokWaktu, Sloting.KdRuangan, Sloting.Slotting,  Sloting.Slotting - dbo.Jumlah_Pasien_Terdaftar('" + tglReservasi.ToString("yyyy-MM-dd HH:mm:ss") + "', " + kdKelompokWaktu + ", '" + request.KodePoli + "') AS SisaSloting FROM" +
                                " PasienDaftar INNER JOIN Ruangan ON PasienDaftar.KdRuanganAkhir = Ruangan.KdRuangan INNER JOIN Instalasi ON Ruangan.KdInstalasi = Instalasi.KdInstalasi INNER JOIN " +
                                " Sloting ON Ruangan.KdRuangan = Sloting.KdRuangan INNER JOIN MapingPeriodeWaktuAwalkeAkhir " +
                                " ON Sloting.KdKelompokWaktu = MapingPeriodeWaktuAwalkeAkhir.KdKelompokWaktu WHERE " +
                                " (Sloting.KdRuangan = '" + request.KodePoli + "') Group by Sloting.KdKelompokWaktu,Sloting.KdRuangan,Sloting.Slotting,ruangan.namaruangan";

                    var reader = db.ExecuteQuery(query);
                    while (reader.Read())
                    {
                        sloting.Sloting = Convert.ToInt32(reader["Slotting"]);
                        sloting.SisaSlotting = Convert.ToInt32(reader["SisaSloting"]);
                        sloting.JumlahPasienTerdaftar = Convert.ToInt32(reader["JumlahPasienTerdaftar"]);
                        sloting.namaRuangan = reader["NamaRuangan"].ToString();
                    }
                    var sql1 = "select * from pasien where nocm ='" + request.NomorRm + "'";

                    var reader1 = db.ExecuteQuery(sql1);
                    while (reader1.Read())
                    {
                        pasien.NoCM = reader1["NoCM"].ToString();
                        pasien.NamaLengkap = reader1["NamaLengkap"].ToString();
                        pasien.Alamat = reader1["Alamat"].ToString();
                        pasien.NoTelp = reader1["Telepon"].ToString();
                    }
                    var NoCm = "";
                    if (pasien.NoCM == null)
                    {
                        NoCm = request.NomorRm;
                    }
                    else
                    {
                        NoCm = pasien.NoCM;
                    }

                    if (string.IsNullOrEmpty(NoCm))
                    {
                        var error = new GetNoAntrianResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "Pasien belum terdaftar di Rumah Sakit",
                                Code = 401
                            }
                        };
                        return Json(error);
                    }

                    if (sloting != null)
                    {
                        var countNoantrian = GetNoReservasi(Convert.ToDateTime(tglReservasi), request.KodePoli);
                        var kodeBooking = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                        var TotalDaftar = "";
                        var sql5 = "select top 1 TglReservasi from Reservasi where month(TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).Month + "' and year(TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).Year + "' and day(TglReservasi) ='" + Convert.ToDateTime(request.TanggalPeriksa).Day + "' order by TglReservasi desc";

                        var reader5 = db.ExecuteQuery(sql5);
                        while (reader5.Read())
                        {
                            TotalDaftar = reader5["TglReservasi"].ToString();
                        }
                        if (TotalDaftar != "")
                        {
                            tglReservasi = Convert.ToDateTime(TotalDaftar).AddMinutes(10);
                        }
                        else
                        {
                            tglReservasi = Convert.ToDateTime(tglReservasi).AddHours(8);

                        }
                        var sql = "insert into Reservasi(NoReservasi,KodeBooking,TglReservasi,TglRegistrasi,KdRuangan)values('" + countNoantrian + "','" + kodeBooking + "','" + tglReservasi.ToString("yyyy/MM/dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + request.KodePoli + "')";
                        db.ExecuteQuery(sql);

                        var idPegawai = ConfigurationManager.AppSettings["IdPegawai"].ToString();
                        var query2 =
                            "insert into DetailReservasi(KodeBooking,NoCM,NoIdentitas,NamaLengkap,Alamat,NoTelp,Email,KdRuangan,IdPegawai,KdKelompokWaktu,JenisPasien,StatusPasien) values " +
                            "('" + kodeBooking + "'," +
                            "'" + NoCm + "'," +
                            "'" + request.Nik + "'," +
                            "'" + pasien.NamaLengkap + "'," +
                            "'" + pasien.Alamat + "'," +
                            "'" + request.NoTelp + "'," +
                            "'" + null + "'," +
                            "'" + request.KodePoli + "'," +
                            "'" + idPegawai + "'," +
                            "" + kdKelompokWaktu + "," +
                            "'" + IdPenjamin + "'," +
                            "'T')\n";
                        query2 += "insert into ReservasiMobileJKN values ('" + kodeBooking + "')";
                        db.ExecuteQuery(query2);
                        request.NomorReferensi = kodeBooking;
                        var antrean = new DaftarAntrianModel();
                        var query1 = "SELECT     NoReservasi, Reservasi.KodeBooking, Reservasi.TglReservasi, Reservasi.TglRegistrasi, Reservasi.KdRuangan, Ruangan.NamaRuangan " +
                                      " FROM         Reservasi INNER JOIN " +
                                      " DetailReservasi ON Reservasi.KdRuangan = DetailReservasi.KdRuangan AND Reservasi.KodeBooking = DetailReservasi.KodeBooking INNER JOIN " +
                                      " Ruangan ON Reservasi.KdRuangan = Ruangan.KdRuangan where Reservasi.KodeBooking ='" + kodeBooking + "'";

                        var reader2 = db.ExecuteQuery(query1);
                        while (reader2.Read())
                        {
                            antrean.NomorAntrean = reader2["NoReservasi"].ToString();
                            antrean.NamaPoli = reader2["NamaRuangan"].ToString();
                            //var tanggalNow = reader2["TglReservasi"];
                            antrean.estimasidilayani = Convert.ToString(DateHelper.ToInteger(Convert.ToDateTime(reader2["TglReservasi"].ToString())));


                        }
                        if (antrean.NomorAntrean != null)
                        {

                            result = new GetNoAntrianResponse()
                            {


                                Response = new DaftarAntrean()
                                {
                                    NomorAntrean = antrean.NomorAntrean,
                                    KodeBooking = request.NomorReferensi,
                                    JenisAntrean = Convert.ToInt32(request.JenisRequest),
                                    estimasidilayani = Convert.ToInt64(antrean.estimasidilayani),
                                    NamaPoli = antrean.NamaPoli,
                                    NamaDokter = ""

                                },
                                Metadata = new Metadata()
                                {
                                    Message = "ok",
                                    Code = 200
                                }

                            };
                            return Json(result);
                        }
                        else
                        {
                            var error = new GetNoAntrianResponse()
                            {
                                Metadata = new Metadata()
                                {
                                    Message = "Error",
                                    Code = 401
                                }

                            };
                            return Json(error);

                        }
                    }
                    else
                    {
                        var error = new GetNoAntrianResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "Error",
                                Code = 401
                            }

                        };
                        return Json(error);

                    }
                }

                else
                {
                    var error = new GetNoAntrianResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Data Sudah Terdaftar",
                            Code = 401
                        }

                    };
                    return Json(error);

                }

            }
        }
        [Route("GetJadwalOprasi")]
        [HttpPost]
        public JsonResult<GetJadwalOprasiResponse> CheckDataSlotingOprasi([FromBody] getJadwalOprasi request)
        {
            GetJadwalOprasiResponse result;
            var pasien = new PasienModel();
            DateTime LastUpdate = DateTime.Now;
            var PasienDaftar = new PasienDaftarModel();
            var jadwal = new JadwalOprasiResponse();
            var connection = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            var kdRuanganBedah = ConfigurationManager.AppSettings["KodeRuanganBedah"];
            var list = new List<JadwalOprasiResponse>();

            var PenjaminBpjs = ConfigurationManager.AppSettings["KodeJenisPasien"];

            using (var db = new ContextManager(connection))
            {

                var chek = "select  top 1  NoCM, NoPendaftaran from  PemakaianAsuransi where IdAsuransi ='" + request.NoPeserta + "' order by TglSJP desc";
                var readerChek = db.ExecuteQuery(chek);
                while (readerChek.Read())
                {
                    PasienDaftar.NoCm = readerChek["NoCM"].ToString();
                    PasienDaftar.NoPendaftara = readerChek["NoPendaftaran"].ToString();
                }

                if (PasienDaftar.NoCm == null)
                {
                    var error = new GetJadwalOprasiResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Data Tidak Ditemukan",
                            Code = 401
                        }
                    };
                    return Json(error);
                }

                var token = Request.Headers.GetValues("x-token").FirstOrDefault();
                if (token == null)
                {
                    var error = new GetJadwalOprasiResponse()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "token kosong",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                var validateToken = Helper.ValidateToken(token);
                if (validateToken != null)
                {
                    var user = validateToken.UserName;
                    var valid = new userApiModel();
                    var sql1 = "select * from UserApiJkn where userName ='" + validateToken.UserName + "' and password='" + validateToken.password + "'";
                    var conn = new ContextManager(connection);
                    var reader01 = conn.ExecuteQuery(sql1);
                    while (reader01.Read())
                    {
                        valid.status = reader01["status"].ToString();
                    }
                    if (valid != null)
                    {
                        goto nextStep;
                    }
                    else
                    {
                        var error = new GetJadwalOprasiResponse()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "Data Tidak Ditemukan",
                                Code = 401
                            }
                        };
                        return Json(error);
                    }

                }
            nextStep:

                //var chekJadwal = " SELECT    NamaExternal,KodeExternal, NoIBS,NoPendaftaran, JenisOperasi, NamaRuangan, case when StatusOperasi = 'Sudah' then 1 else 0 end as StatusOperasi, TglPendaftaran, KdRuangan,IdAsuransi " +
                //                                " FROM         v_DaftarPasienIBSjkn  where IdAsuransi='" + request.NoPeserta + "' and StatusOperasi = 'Belum'";
                //karena di Malingping, tidak memiliki table rencanaoperasipasien, semua di tampung ditable jadwaloperasi
                var chekJadwal = " SELECT * FROM  v_DaftarPasienIBSjkn  where IdAsuransi='" + request.NoPeserta + "'";

                var readerChekJadwal = db.ExecuteQuery(chekJadwal);
                while (readerChekJadwal.Read())
                {
                    jadwal = new JadwalOprasiResponse();
                    jadwal.KodeBooking = readerChekJadwal["NoIBS"].ToString();
                    jadwal.TanggalOprasi = Convert.ToDateTime(readerChekJadwal["TglPendaftaran"]).ToString("yyyy-MM-dd");
                    jadwal.JenisTindakan = readerChekJadwal["JenisOperasi"].ToString();
                    jadwal.KodePoli = readerChekJadwal["KodeExternal"].ToString();
                    jadwal.NamaPoli = readerChekJadwal["NamaExternal"].ToString();
                    jadwal.Terlaksana = Convert.ToInt32(readerChekJadwal["StatusOperasi"]);
                    ////karena menurut excel, hanya yang belum terlaksana yang harus ditampilkan, jadi saya patok 0
                    //jadwal.Terlaksana = 0;

                    list.Add(jadwal);

                }

                if (readerChekJadwal != null)
                {
                    result = new GetJadwalOprasiResponse()
                    {

                        Response = new ListJadwalOprasiResponse()
                        {
                            List = list
                        },
                        Metadata = new Metadata()
                        {
                            Message = "ok",
                            Code = 200
                        }

                    };
                    return Json(result);

                }
                else
                {
                    result = new GetJadwalOprasiResponse()
                    {



                        Metadata = new Metadata()
                        {
                            Message = "Error",
                            Code = 401
                        }

                    };
                    return Json(result);

                }

            }
        }

        [Route("GetJadwalOprasiAll")]
        [HttpPost]
        public JsonResult<GetJadwalOprasiResponseAll> CheckDataSlotingOprasiList([FromBody] getAllJadwalOprasi request)
        {
            GetJadwalOprasiResponseAll result;
            var pasien = new PasienModel();
            DateTime LastUpdate = DateTime.Now;
            var PasienDaftar = new PasienDaftarModel();
            var jadwal = new JadwalOprasiResponseall();
            var connection = ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
            var kdRuanganBedah = ConfigurationManager.AppSettings["KodeRuanganBedah"];
            var PenjaminBpjs = ConfigurationManager.AppSettings["KodeJenisPasien"];
            var list = new List<JadwalOprasiResponseall>();

            using (var db = new ContextManager(connection))
            {
                var token = Request.Headers.GetValues("x-token").FirstOrDefault();


                if (token == null)
                {
                    var error = new GetJadwalOprasiResponseAll()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "token kosong",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                if (Convert.ToDateTime(request.TglAkhir) < Convert.ToDateTime(request.TglAwal))
                {
                    var error = new GetJadwalOprasiResponseAll()
                    {

                        Metadata = new Metadata()
                        {
                            Message = "Tgl Awal Tidak Boleh Lebih Besar Dari Tgl Akhir",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
                var validateToken = Helper.ValidateToken(token);
                if (validateToken != null)
                {
                    var user = validateToken.UserName;
                    var valid = new userApiModel();
                    var sql1 = "select * from UserApiJkn where userName ='" + validateToken.UserName + "' and password='" + validateToken.password + "'";
                    var conn = new ContextManager(connection);
                    var reader01 = conn.ExecuteQuery(sql1);
                    while (reader01.Read())
                    {
                        valid.status = reader01["status"].ToString();
                    }
                    if (valid != null)
                    {
                        goto nextStep;
                    }
                    else
                    {
                        var error = new GetJadwalOprasiResponseAll()
                        {

                            Metadata = new Metadata()
                            {
                                Message = "Data Tidak Ditemukan",
                                Code = 401
                            }
                        };
                        return Json(error);
                    }

                }
            nextStep:
                //var chekJadwal = " SELECT    NamaExternal,KodeExternal, NoIBS,NoPendaftaran, JenisOperasi, NamaRuangan, case when StatusOperasi = 'Sudah' then 1 else 0 end as StatusOperasi, TglPendaftaran, KdRuangan,IdAsuransi " +
                //                 " FROM         v_DaftarPasienIBSjkn  where NamaPenjamin  ='" + PenjaminBpjs + "' and  TglPendaftaran between  '" + request.TglAwal + "' and '" + request.TglAkhir + "' ";

                var chekJadwal = " SELECT    NamaExternal,KodeExternal, NoIBS, JenisOperasi, StatusOperasi, TglPendaftaran, IdAsuransi  " +
                    "FROM         v_DaftarPasienIBSjkn  " +
                    "where NamaPenjamin  ='" + PenjaminBpjs + "' and  TglPendaftaran between  '" + request.TglAwal + "' and '" + request.TglAkhir + "' Order by TglPendaftaran Desc";

                var readerChekJadwal = db.ExecuteQuery(chekJadwal);
                while (readerChekJadwal.Read()) 
                {
                    jadwal = new JadwalOprasiResponseall();
                    jadwal.KodeBooking = readerChekJadwal["NoIBS"].ToString();
                    jadwal.TanggalOprasi = Convert.ToDateTime(readerChekJadwal["TglPendaftaran"]).ToString("yyyy-MM-dd");
                    jadwal.JenisTindakan = readerChekJadwal["JenisOperasi"].ToString();
                    jadwal.KodePoli = readerChekJadwal["KodeExternal"].ToString();
                    jadwal.NamaPoli = readerChekJadwal["NamaExternal"].ToString();
                    jadwal.Terlaksana = Convert.ToInt32(readerChekJadwal["StatusOperasi"]);
                    jadwal.NoPeserta = readerChekJadwal["IdAsuransi"].ToString();
                    jadwal.LastUpdate = Convert.ToInt64(Convert.ToString(DateHelper.ToInteger(DateTime.Now)));

                    list.Add(jadwal);

                }

                if (readerChekJadwal != null)
                {
                    result = new GetJadwalOprasiResponseAll()
                    {

                        Response = new ListJadwalOprasiResponseAll()
                        {
                            List = list
                        },
                        Metadata = new Metadata()
                        {
                            Message = "ok",
                            Code = 200
                        }

                    };
                    return Json(result);

                }
                else
                {
                    //var masagge = ResponErroSemua("ERROR");

                    // var responseError = ResponErroSemua("Tgl pendaftaran tidak boleh kurang dari  +1 hari dan lebih dari +7 hari ");
                    var error = new GetJadwalOprasiResponseAll()
                    {
                        Metadata = new Metadata()
                        {
                            Message = "ERROR",
                            Code = 401
                        }
                    };
                    return Json(error);
                }
            }


        }
        private JsonResult<responEror> ResponErroSemua(string masagge)
        {
            try
            {
                responEror result;
                result = new responEror()
                {
                    Metadata = new Metadata()
                    {
                        Message = masagge,
                        Code = 401
                    }

                };
                return Json(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private int GetNoReservasi(DateTime? tglReservasi, string kdRuangan)
        {
            try
            {
                var tgl = DateTime.Now.ToString("yyyy-MM-dd 00.00.00");
                var tglAwal = tglReservasi.Value.ToString("yyyy-MM-dd 00:00:00");
                var tglAkhir = tglReservasi.Value.ToString("yyyy-MM-dd 23:59:59");
                using (var context = new ContextManager(ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString))
                {
                    var query = "select max(NoReservasi) as NoAntrian from reservasi " +
                        "where TglReservasi between '" + tglAwal + "' " +
                        "and '" + tglAkhir + "'";
                    var reader = context.ExecuteQuery(query);
                    while (reader.Read())
                    {
                        int final;
                        var noAntrian = reader["NoAntrian"].ToInt32();
                        if (noAntrian != 0)
                        //if (!string.IsNullOrEmpty(noAntrian))
                        {
                            var last = Convert.ToInt32(noAntrian);//.Substring(8, 3));
                            last += 1;
                            final = last;// "RO" + Convert.ToString(tglReservasi.Value.ToString("yyMMdd")) + last.ToString("D3");
                        }
                        else
                        {
                            final = 1;
                        }
                        return final;
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }
}
