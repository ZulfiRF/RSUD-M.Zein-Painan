using System;
using System.Collections.Generic;
using DbContext;
using Helper;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;

namespace Medifirst.SistemAdministrasi
{
    public enum TipeTransaksi
    {
        NoRegistrasi,
        NoMasuk,
        NoRujukan,
        NoRujukan_Intern,
        NoHasilPeriksa,
        NoStruk,
        NoBKM,
        NoBKK,
        No_Order,
        NoTerima,
        NoKirim,
        NoSJP,
        NoResep,
        NoClosing,
        NoPosting,
        NoRetur,
        NoProgram,
        NoKehamilan,
        NoPaket,
        NoProgramBT,
        NoSchedule
    }

    public class Function : INoTraksaksi, IKelompokUmur
    {
        private const int countAutoIncrement = 6;
        private static readonly object obj = new object();
        private static string tanggal;

        #region IKelompokUmur Members

        public TipeKelompokUmur GetKelompokUmur(byte KdProfile, string NoCM, DateTime TglPeriksa, DateTime TglLahir,
                                                int KelompokUmur)
        {
            int Year = DateHelper.CountDiffrenceAge(TglPeriksa, TglLahir).Year;
            int TotalHari = DateTime.Now.Subtract(new DateTime(1989, 10, 24)).Days;
            switch (KelompokUmur)
            {
                case 1:
                    return (TotalHari < 28) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                case 2:
                    return (TotalHari >= 28 && Year < 1) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                case 3:
                    //JmlUmurDlmThn >= 1 and @JmlUmurDlmThn <= 4 and @JmlUmurDlmHari>=365
                    return (Year >= 1 && Year <= 4 && TotalHari >= 365)
                               ? TipeKelompokUmur.Termasuk
                               : TipeKelompokUmur.TidakTermasuk;
                case 4:
                    return (Year >= 5 && Year <= 14) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                case 5:
                    return (Year >= 15 && Year <= 24) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                case 6:
                    return (Year >= 25 && Year <= 44) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                case 7:
                    return (Year >= 45 && Year <= 64) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                case 8:
                    return (Year > 65) ? TipeKelompokUmur.Termasuk : TipeKelompokUmur.TidakTermasuk;
                default:
                    return TipeKelompokUmur.TidakTermasuk;
            }
        }

        #endregion

        #region INoTraksaksi Members

        public string GenerateNoRegistrasi(string noCM, byte kodeProfile, string kodeDepartemenUser,
                                           DateTime tanggalStruk)
        {
            return NoRegistrasi(noCM, kodeProfile, kodeDepartemenUser, tanggalStruk);
        }

        public string GenerateNoMasuk(string noRegistrasi, byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoMasuk(noRegistrasi, kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoRujukan(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoRujukan(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoRujukanIntern(string kodeRuanganRujukan, string noCM, byte kodeProfile,
                                              DateTime tanggalStruk)
        {
            return NoRujukanIntern(kodeRuanganRujukan, noCM, kodeProfile, tanggalStruk);
        }

        public string GenerateNoHasilPeriksa(string tempNoStruk, byte kodeProfile, DateTime tanggalStruk)
        {
            return NoHasilPeriksa(tempNoStruk, kodeProfile, tanggalStruk);
        }

        public string GenerateNoStruk(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoStruk(kodeProfile, tanggalStruk);
        }

        public string GenerateNoBKM(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoBKM(kodeProfile, tanggalStruk);
        }

        public string GenerateNoBKK(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoBKK(kodeProfile, tanggalStruk);
        }

        public string GenerateNoOrder(byte kodeProfile, DateTime tanggalStruk)
        {
            return No_Order(kodeProfile, tanggalStruk);
        }

        public string GenerateNoTerima(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoTerima(kodeProfile, tanggalStruk);
        }

        public string GenerateNoKirim(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoKirim(kodeProfile, tanggalStruk);
        }

        public string GenerateNoSJP(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoSJP(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoResep(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoResep(kodeProfile, tanggalStruk);
        }

        public string GenerateNoClosing(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoClosing(kodeProfile, tanggalStruk);
        }

        public string GenerateNoPosting(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoPosting(kodeProfile, tanggalStruk);
        }

        public string GenerateNoRetur(byte kodeProfile, DateTime tanggalStruk)
        {
            return NoRetur(kodeProfile, tanggalStruk);
        }

        public string GenerateNoProgram(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoProgram(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoKehamilan(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoKehamilan(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoPaket(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoPaket(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoProgramBT(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return NoProgram(kodeProfile, noCM, tanggalStruk);
        }

        #endregion

        public static string AutoFormatNoTraksaksi(byte kodeProfile, long kodeHistroyLogin, DateTime tanggalStruk,
                                                   TipeTransaksi transaksi, string noCM, string noRegistrasi,
                                                   string kodeRuanganRujukan)
        {
            //DECLARE @TempNoStruk varchar(10)
            //DECLARE @bln char(2)
            //DECLARE @thn char(2)
            //DECLARE @x smallint
            //DECLARE @OutputNoStruk char(10)
            //DECLARE @RowData tinyint
            //DECLARE @KdJenisPerawatanPasien tinyint
            //DECLARE @KdRuanganUser varchar(3)
            //DECLARE @KdDepartemenUser char(1)
            string tempNoStruk = "";
            string kodeRuanganUser;
            string kodeDepartemenUser;
            tanggal = tanggalStruk.ToString("yyMM");
            //int x = 0;

            //SELECT @KdRuanganUser=KdRuanganUser FROM HistoryLoginModulAplikasi_S WHERE KdProfile=@KdProfile AND KdHistoryLogin=@KdHistoryLogin
            //SELECT @KdDepartemenUser=KdDepartemen FROM Ruangan_M WHERE KdProfile=@KdProfile AND KdRuangan=@KdRuanganUser

            kodeRuanganUser =
                new object().LastRecordIndex(GenOneField("HistoryLoginModulAplikasi_S", "KdRuanganUser",
                                                         new List<WhereTemp>
                                                             {
                                                                 new WhereTemp
                                                                     {Field = "KdProfile", Value = kodeProfile},
                                                                 new WhereTemp
                                                                     {
                                                                         Field = "KdHistoryLogin",
                                                                         Value = kodeHistroyLogin
                                                                     }
                                                             }));
            kodeDepartemenUser =
                new object().LastRecordIndex(GenOneField("Ruangan_M", "KdDepartemen", new List<WhereTemp>
                                                                                          {
                                                                                              new WhereTemp
                                                                                                  {
                                                                                                      Field = "KdProfile",
                                                                                                      Value = kodeProfile
                                                                                                  },
                                                                                              new WhereTemp
                                                                                                  {
                                                                                                      Field = "KdRuangan",
                                                                                                      Value =
                                                                                                          kodeRuanganUser
                                                                                                  }
                                                                                          }));
            //IF @TglStruk>GETDATE()
            //    SET @OutputNoStruk=@thn + @bln + '999999'
            //ELSE
            if (tanggalStruk > DateTime.Now)
            {
                return tanggal + "999999";
            }
            else
            {
                switch (transaksi)
                {
                    case TipeTransaksi.NoRegistrasi:
                        return NoRegistrasi(noCM, kodeProfile, kodeDepartemenUser, tanggalStruk);
                    case TipeTransaksi.NoMasuk:
                        return NoMasuk(noRegistrasi, kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoRujukan:
                        return NoRujukan(kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoRujukan_Intern:
                        return NoRujukanIntern(kodeRuanganRujukan, noCM, kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoHasilPeriksa:
                        return NoHasilPeriksa(tempNoStruk, kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoStruk:
                        return NoStruk(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoBKM:
                        return NoBKM(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoBKK:
                        return NoBKK(kodeProfile, tanggalStruk);
                    case TipeTransaksi.No_Order:
                        return No_Order(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoTerima:
                        return NoTerima(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoKirim:
                        return NoKirim(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoSJP:
                        return NoSJP(kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoResep:
                        return NoResep(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoClosing:
                        return NoClosing(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoPosting:
                        return NoPosting(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoRetur:
                        return NoRetur(kodeProfile, tanggalStruk);
                    case TipeTransaksi.NoProgram:
                        return NoProgram(kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoKehamilan:
                        return NoKehamilan(kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoPaket:
                        return NoPaket(kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoProgramBT:
                        return NoProgramBT(kodeProfile, noCM, tanggalStruk);
                    case TipeTransaksi.NoSchedule:
                        return NoSchedule(kodeProfile, tanggalStruk);
                    default:
                        break;
                }
            }
            return "";
        }

        #region SatuParameter Kode Profile

        private static string GenereteOneParameter(byte kodeProfile, DateTime tanggalStruk, string table, string field)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoPosting )) FROM Posting_T WHERE KdProfile=@KdProfile AND (LEFT(NoPosting,2)=@thn) AND (SUBSTRING(NoPosting,3,2)=@bln)
            string hasil =
                obj.LastRecordIndex(GenOneField(table, "MAX(CAST(" + field + "  AS BIGINT )) ", new List<WhereTemp>
                                                                                                    {
                                                                                                        new WhereTemp
                                                                                                            {
                                                                                                                Field =
                                                                                                                    "KdProfile",
                                                                                                                Value =
                                                                                                                    kodeProfile
                                                                                                            }
                                                                                                    },
                                                "AND LEFT(" + field + ",4)='" + tanggal + "'"));
            return FunctionAutoFormat(tanggalStruk.ToString("yyMM"), hasil);
        }

        private static string NoSchedule(byte kodeProfile, DateTime tanggalStruk)
        {
            return GenereteOneParameter(kodeProfile, tanggalStruk, "Scheduling_T", "NoSchedule ");
        }

        private static string NoPosting(byte kodeProfile, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoPosting )) FROM Posting_T WHERE KdProfile=@KdProfile AND (LEFT(NoPosting,2)=@thn) AND (SUBSTRING(NoPosting,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "Posting_T", "NoPosting ");
        }

        private static string NoClosing(byte kodeProfile, DateTime tanggalStruk)
        {
            //	SELECT @TempNoStruk=MAX(CAST(NoClosing )) FROM Closing_T WHERE KdProfile=@KdProfile AND (LEFT(NoClosing,2)=@thn) AND (SUBSTRING(NoClosing,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "Closing_T", "NoClosing ");
        }

        private static string NoKirim(byte kodeProfile, DateTime tanggalStruk)
        {
            //	SELECT @TempNoStruk=MAX(CAST(NoKirim )) FROM StrukKirim_T WHERE KdProfile=@KdProfile AND (LEFT(NoKirim,2)=@thn) AND (SUBSTRING(NoKirim,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukKirim_T", "NoKirim ");
        }

        private static string NoBKK(byte kodeProfile, DateTime tanggalStruk)
        {
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukPelayananPasien_T", "NoStruk ");
        }

        private static string NoStruk(byte kodeProfile, DateTime tanggalStruk)
        {
            //	SELECT @TempNoStruk=MAX(CAST(NoBKK )) FROM StrukBuktiKasKeluar_T WHERE KdProfile=@KdProfile AND (LEFT(NoBKK,2)=@thn) AND (SUBSTRING(NoBKK,3,2)=@bln)
            //return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukBuktiKasKeluar_T", "NoBKK ");
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukPelayanan_T", "NoStruk ");
        }

        private static string No_Order(byte kodeProfile, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(No_Order )) FROM StrukOrder_T WHERE KdProfile=@KdProfile AND (LEFT(No_Order,2)=@thn) AND (SUBSTRING(No_Order,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukOrder_T", "No_Order ");
        }

        private static string NoTerima(byte kodeProfile, DateTime tanggalStruk)
        {
            // SELECT @TempNoStruk=MAX(CAST(NoTerima )) FROM StrukTerima_T WHERE KdProfile=@KdProfile AND (LEFT(NoTerima,2)=@thn) AND (SUBSTRING(NoTerima,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukTerima_T", "NoTerima ");
        }

        private static string NoBKM(byte kodeProfile, DateTime tanggalStruk)
        {
            //  SELECT @TempNoStruk=MAX(CAST(NoBKM )) FROM StrukBuktiKasMasuk_T WHERE KdProfile=@KdProfile AND (LEFT(NoBKM,2)=@thn) AND (SUBSTRING(NoBKM,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukBuktiKasMasuk_T", "NoBKM ");
        }

        private static string NoRetur(byte kodeProfile, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoRetur )) FROM StrukRetur_T WHERE KdProfile=@KdProfile AND (LEFT(NoRetur,2)=@thn) AND (SUBSTRING(NoRetur,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukRetur_T", "NoRetur ");
        }

        private static string NoResep(byte kodeProfile, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoResep )) FROM PelayananResep_T WHERE KdProfile=@KdProfile AND (LEFT(NoResep,2)=@thn) AND (SUBSTRING(NoResep,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "StrukResep_T", "NoResep ");
        }

        #endregion SatuParameter Kode Profile

        #region Implemetasi INoTransaksi

        private static string NoPaket(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoPaket AS BIGINT)) FROM PaketKunjunganPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglAkhirPaket<@TglStruk AND QtyKunjunganKe<QtyMaksKunjungan AND (LEFT(NoPaket,2)=@thn) AND (SUBSTRING(NoPaket,3,2)=@bln)
            //IF @RowData=0
            //BEGIN

            object temp =
                new object().LastRecordIndexObject(GenOneField("PaketKunjunganPasien_T", "MAX(CAST(NoPaket AS BIGINT)) ",
                                                               new List<WhereTemp>
                                                                   {
                                                                       new WhereTemp
                                                                           {Field = "KdProfile", Value = kodeProfile},
                                                                       new WhereTemp {Field = "NoCM", Value = noCM}
                                                                   },
                                                               " TglAkhirPaket<" + "CONVERT(datetime,'" +
                                                               tanggalStruk.ToString("yyyy-MM-dd 0:00:00") + "',120)" +
                                                               " AND QtyKunjunganKe<QtyMaksKunjungan AND LEFT(NoPaket,4)='" +
                                                               tanggal + "'"));
            long count = (temp == null || temp == DBNull.Value) ? 0 : Convert.ToInt64(temp);
            //IF @RowData=0
            if (count == 0)
            {
                //SELECT @TempNoStruk=MAX(CAST(NoPaket AS BIGINT)) FROM PaketKunjunganPasien_T WHERE KdProfile=@KdProfile AND (LEFT(NoPaket,2)=@thn) AND (SUBSTRING(NoPaket,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("PaketKunjunganPasien_T", "MAX(CAST(NoPaket AS BIGINT)) ",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoPaket,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoKehamilan(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoKehamilan AS BIGINT)) FROM PemeriksaanKehamilanPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND isBersalin=0 AND (LEFT(NoKehamilan,2)=@thn) AND (SUBSTRING(NoKehamilan,3,2)=@bln)
            //IF @RowData=0
            //BEGIN
            object temp =
                new object().LastRecordIndexObject(GenOneField("PemeriksaanKehamilanPasien_T",
                                                               "MAX(CAST(NoKehamilan AS BIGINT)) ", new List<WhereTemp>
                                                                                                        {
                                                                                                            new WhereTemp
                                                                                                                {
                                                                                                                    Field
                                                                                                                        =
                                                                                                                        "KdProfile",
                                                                                                                    Value
                                                                                                                        =
                                                                                                                        kodeProfile
                                                                                                                },
                                                                                                            new WhereTemp
                                                                                                                {
                                                                                                                    Field
                                                                                                                        =
                                                                                                                        "NoCM",
                                                                                                                    Value
                                                                                                                        =
                                                                                                                        noCM
                                                                                                                }
                                                                                                        },
                                                               " AND isBersalin=0 AND LEFT(NoKehamilan,4)='" + tanggal +
                                                               "'"));
            long count = (temp == null || temp == DBNull.Value) ? 0 : Convert.ToInt64(temp);
            //IF @RowData=0
            if (count == 0)
            {
                //    SELECT @TempNoStruk=MAX(CAST(NoKehamilan AS BIGINT)) FROM PemeriksaanKehamilanPasien_T WHERE KdProfile=@KdProfile AND (LEFT(NoKehamilan,2)=@thn) AND (SUBSTRING(NoKehamilan,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("PemeriksaanKehamilanPasien_T", "MAX(CAST(NoKehamilan AS BIGINT)) ",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoKehamilan,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoProgram(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            //   SELECT @TempNoStruk=MAX(CAST(NoProgram AS BIGINT)) FROM ProgramKeluargaBerencana_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglDicabut IS NULL AND (LEFT(NoProgram,2)=@thn) AND (SUBSTRING(NoProgram,3,2)=@bln)
            object temp =
                new object().LastRecordIndexObject(GenOneField("ProgramKeluargaBerencana_T",
                                                               "MAX(CAST(NoProgram AS BIGINT)) ", new List<WhereTemp>
                                                                                                      {
                                                                                                          new WhereTemp
                                                                                                              {
                                                                                                                  Field
                                                                                                                      =
                                                                                                                      "KdProfile",
                                                                                                                  Value
                                                                                                                      =
                                                                                                                      kodeProfile
                                                                                                              },
                                                                                                          new WhereTemp
                                                                                                              {
                                                                                                                  Field
                                                                                                                      =
                                                                                                                      "NoCM",
                                                                                                                  Value
                                                                                                                      =
                                                                                                                      noCM
                                                                                                              }
                                                                                                      },
                                                               "AND TglDicabut IS NULL AND LEFT(NoProgram,4)='" +
                                                               tanggal + "'"));
            long count = (temp == null || temp == DBNull.Value) ? 0 : Convert.ToInt64(temp);
            //IF @RowData=0
            if (count == 0)
            {
                //	SELECT @TempNoStruk=MAX(CAST(NoProgram AS BIGINT)) FROM ProgramKeluargaBerencana_T WHERE KdProfile=@KdProfile AND (LEFT(NoProgram,2)=@thn) AND (SUBSTRING(NoProgram,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("ProgramKeluargaBerencana_T", "MAX(CAST(NoProgram AS BIGINT))",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoProgram,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoProgramBT(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            object temp =
                new object().LastRecordIndexObject(GenOneField("ProgramBayiTabung_T", "MAX(CAST(NoProgram AS BIGINT)) ",
                                                               new List<WhereTemp>
                                                                   {
                                                                       new WhereTemp
                                                                           {Field = "KdProfile", Value = kodeProfile},
                                                                       new WhereTemp {Field = "NoCM", Value = noCM}
                                                                   },
                                                               "AND TglProgramBayiTabung IS NULL AND LEFT(NoProgram,4)='" +
                                                               tanggal + "'"));
            long count = (temp == null || temp == DBNull.Value) ? 0 : Convert.ToInt64(temp);
            if (count == 0)
            {
                string hasil =
                    obj.LastRecordIndex(GenOneField("ProgramBayiTabung_T", "MAX(CAST(NoProgram AS BIGINT))",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoProgram,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoSJP(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoSJP AS BIGINT)) FROM PemakaianAsuransiPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglMaksBerlaku<@TglStruk AND (LEFT(NoSJP,2)=@thn) AND (SUBSTRING(NoSJP,3,2)=@bln)
            object temp =
                new object().LastRecordIndexObject(GenOneField("PemakaianAsuransiPasien_T", "MAX(CAST(NoSJP AS BIGINT))",
                                                               new List<WhereTemp>
                                                                   {
                                                                       new WhereTemp
                                                                           {Field = "KdProfile", Value = kodeProfile},
                                                                       new WhereTemp {Field = "NoCM", Value = noCM}
                                                                   },
                                                               "AND TglMaksBerlaku<" + "CONVERT(datetime,'" +
                                                               tanggalStruk.ToString("yyyy-MM-dd 0:00:00") + "',120)" +
                                                               " AND LEFT(NoSJP,4)='" + tanggal + "'"));
            long count = (temp == null || temp == DBNull.Value) ? 0 : Convert.ToInt64(temp);
            //IF @RowData=0
            if (count == 0)
            {
                //SELECT @TempNoStruk=MAX(CAST(NoSJP AS BIGINT)) FROM PemakaianAsuransiPasien_T WHERE KdProfile=@KdProfile AND (LEFT(NoSJP,2)=@thn) AND (SUBSTRING(NoSJP,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("PemakaianAsuransiPasien_T", "MAX(CAST(NoSJP AS BIGINT)) ",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoSJP,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoRujukan(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            //--Pasien tdk bisa masuk rujukan lagi jika rujukan pemeriksaan seblmnya blm kadaluarsa.
            //SELECT @RowData=COUNT(NoCM)FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglMaksBerlaku<@TglStruk AND (LEFT(NoRujukan,2)=@thn) AND (SUBSTRING(NoRujukan,3,2)=@bln)
            int count =
                Convert.ToInt16(
                    new object().LastRecordIndexObject(GenOneField("PasienDiRujukEksternal_T", "COUNT(NoCM)",
                                                                   new List<WhereTemp>
                                                                       {
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "KdProfile",
                                                                                   Value = kodeProfile
                                                                               },
                                                                           new WhereTemp {Field = "NoCM", Value = noCM}
                                                                       },
                                                                   "AND TglMaksBerlaku< " + "CONVERT(datetime,'" +
                                                                   tanggalStruk.ToString("yyyy-MM-dd 0:00:00") +
                                                                   "',120)" + " AND LEFT(NoRujukan,4)='" + tanggal + "'")));
            //IF @RowData=0
            if (count == 0)
            {
                //SELECT @TempNoStruk=MAX(CAST(NoRujukan AS BIGINT)) FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND (LEFT(NoRujukan,2)=@thn) AND (SUBSTRING(NoRujukan,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("PasienDiRujukEksternal_T", "MAX(CAST(NoRujukan AS BIGINT))",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoRujukan,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoHasilPeriksa(string tempNoStruk, byte kodeProfile, DateTime tanggalStruk)
        {
            //SELECT @TempNoStruk=MAX(CAST(NoHasilPeriksa AS BIGINT)) FROM HasilPemeriksaanPasien_T WHERE KdProfile=@KdProfile AND (LEFT(NoHasilPeriksa,2)=@thn) AND (SUBSTRING(NoHasilPeriksa,3,2)=@bln)
            return GenereteOneParameter(kodeProfile, tanggalStruk, "HasilPemeriksaanPasien_T", "NoHasilPeriksa");
        }

        private static string NoRujukanIntern(string kodeRuanganRujukan, string noCM, byte kodeProfile,
                                              DateTime tanggalStruk)
        {
            //SELECT @RowData=COUNT(NoCM)FROM PasienDiRujukInternal_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND KdRuanganRujukan=@KdRuanganRujukan AND NoMasukRujukan IS NULL AND (LEFT(NoRujukan_Intern,2)=@thn) AND (SUBSTRING(NoRujukan_Intern,3,2)=@bln)
            //IF @RowData=0
            //BEGIN
            //    SELECT @TempNoStruk=MAX(CAST(NoRujukan_Intern AS BIGINT)) FROM PasienDiRujukInternal_T WHERE KdProfile=@KdProfile AND (LEFT(NoRujukan_Intern,2)=@thn) AND (SUBSTRING(NoRujukan_Intern,3,2)=@bln)
            int count =
                Convert.ToInt16(
                    new object().LastRecordIndexObject(GenOneField("PasienDiRujukInternal_T", "COUNT(NoCM)",
                                                                   new List<WhereTemp>
                                                                       {
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "KdProfile",
                                                                                   Value = kodeProfile
                                                                               },
                                                                           new WhereTemp {Field = "NoCM", Value = noCM},
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "KdRuanganRujukan",
                                                                                   Value = kodeRuanganRujukan
                                                                               }
                                                                       },
                                                                   "AND LEFT(NoRujukan_Intern,4)='" + tanggal +
                                                                   "' AND NoMasukRujukan IS NULL ")));
            //IF @RowData=0
            if (count == 0)
            {
                //SELECT @TempNoStruk=MAX(CAST(NoRujukan AS BIGINT)) FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND (LEFT(NoRujukan,2)=@thn) AND (SUBSTRING(NoRujukan,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("PasienDiRujukInternal_T", "MAX(CAST(NoRujukan_Intern AS BIGINT))",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoRujukan_Intern,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string NoMasuk(string noRegistrasi, byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            //--Pasien tdk bisa masuk ruangan jika kunjungan seblmnya blm keluar kamar.
            //SELECT @RowData=COUNT(NoCM)FROM RegistrasiPelayananPasien_T WHERE KdProfile=@KdProfile AND NoRegistrasi=@NoRegistrasi AND NoCM=@NoCM AND TglKeluar IS NULL AND (LEFT(NoRegistrasi,2)=@thn) AND (SUBSTRING(NoRegistrasi,3,2)=@bln)
            int count =
                Convert.ToInt16(
                    new object().LastRecordIndexObject(GenOneField("RegistrasiPelayananPasien_T", "COUNT(NoCM)",
                                                                   new List<WhereTemp>
                                                                       {
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "KdProfile",
                                                                                   Value = kodeProfile
                                                                               },
                                                                           new WhereTemp {Field = "NoCM", Value = noCM},
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "NoRegistrasi",
                                                                                   Value = noRegistrasi
                                                                               }
                                                                       },
                                                                   "AND TglKeluar IS NULL AND AND LEFT(NoRegistrasi,4)='" +
                                                                   tanggal + "'")));
            //IF @RowData=0
            if (count == 0)
            {
                //    SELECT @TempNoStruk=MAX(CAST(NoMasuk AS BIGINT)) FROM RegistrasiPelayananPasien_T WHERE KdProfile=@KdProfile AND (LEFT(NoMasuk,2)=@thn) AND (SUBSTRING(NoMasuk,3,2)=@bln)
                string hasil =
                    obj.LastRecordIndex(GenOneField("RegistrasiPelayananPasien_T", "MAX(CAST(NoMasuk AS BIGINT))",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoRegistrasi,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            else
            {
                return FunctionAutoFormat(tanggal, "aaaaaa");
            }
        }

        private static string FunctionAutoFormat(string tanggal, string hasil)
        {
            if (hasil.Contains("aaaaa"))
                return hasil;
            if (hasil == string.Empty)
            {
                int value = 1;
                return tanggal + value.ToString("D6");
            }
            else
            {
                return tanggal + (Convert.ToInt16(hasil.Substring(4)) + 1).ToString("D" + (hasil.Length - 4).ToString());
            }
        }

        private static string NoRegistrasi(string noCM, byte kodeProfile, string kodeDepartemenUser,
                                           DateTime tanggalStruk)
        {
            int kodePerawatanPasien =
                Convert.ToInt16(
                    new object().LastRecordIndexObject(GenOneField("Departemen_M", "KdJenisPerawatanPasien",
                                                                   new List<WhereTemp>
                                                                       {
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "KdProfile",
                                                                                   Value = kodeProfile
                                                                               },
                                                                           new WhereTemp
                                                                               {
                                                                                   Field = "KdDepartemen",
                                                                                   Value = kodeDepartemenUser
                                                                               }
                                                                       })));
            if (kodePerawatanPasien == 1 || kodePerawatanPasien == 2)
            {
                //SELECT @RowData=COUNT(NoCM)FROM AntrianPasienDiPeriksa_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND NoMasuk IS NULL AND (LEFT(NoRegistrasi,2)=@thn) AND (SUBSTRING(NoRegistrasi,3,2)=@bln)

                int count =
                    Convert.ToInt16(
                        obj.LastRecordIndexObject(GenOneField("AntrianPasienDiPeriksa_T", "COUNT(NoCM)",
                                                              new List<WhereTemp>
                                                                  {
                                                                      new WhereTemp
                                                                          {Field = "KdProfile", Value = kodeProfile},
                                                                      new WhereTemp {Field = "NoCM", Value = noCM}
                                                                  },
                                                              " AND NoMasuk IS NULL AND LEFT(NoRegistrasi,4)='" +
                                                              tanggal + "'")));
                string WherePlus = (count == 0) ? " TglKeluar IS NULL " : " NoHasilPeriksa IS NULL ";

                //SELECT @RowData=COUNT(NoCM)FROM RegistrasiPelayananPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglKeluar IS NULL AND (LEFT(NoRegistrasi,2)=@thn) AND (SUBSTRING(NoRegistrasi,3,2)=@bln)
                count =
                    Convert.ToInt16(
                        obj.LastRecordIndexObject(GenOneField("RegistrasiPelayananPasien_T", "COUNT(NoCM)",
                                                              new List<WhereTemp>
                                                                  {
                                                                      new WhereTemp
                                                                          {Field = "KdProfile", Value = kodeProfile},
                                                                      new WhereTemp {Field = "NoCM", Value = noCM}
                                                                  },
                                                              " AND " + WherePlus + "  AND LEFT(NoRegistrasi,4)='" +
                                                              tanggal + "'")));

                string hasil =
                    obj.LastRecordIndex(GenOneField("PasienDaftar_T", "MAX(CAST(NoRegistrasi AS BIGINT))",
                                                    new List<WhereTemp>
                                                        {
                                                            new WhereTemp {Field = "KdProfile", Value = kodeProfile}
                                                        }, "AND LEFT(NoRegistrasi,4)='" + tanggal + "'"));
                return FunctionAutoFormat(tanggal, hasil);
            }
            return tanggal + "aaaaaa";
        }

        private static string GenOneField(string tableName, string Field, List<WhereTemp> where, params string[] plusWhere)
        {
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable(tableName);
            query.AddField(Field);
            foreach (WhereTemp item in where)
            {
                query.AddCriteria(query.Criteria.Equal(item.Field, item.Value));
            }

            string hasil = query.BuildQuery();
            foreach (string item in plusWhere)
            {
                hasil += " " + item;
            }
            return hasil;
        }

        #endregion Implemetasi INoTransaksi
    }

    internal class WhereTemp
    {
        public string Field { get; set; }

        public object Value { get; set; }
    }

    public class FunctionImpl : INoTraksaksi
    {
        private readonly INoTraksaksi Transaksi;

        public FunctionImpl(INoTraksaksi Transaksi)
        {
            this.Transaksi = Transaksi;
        }

        public FunctionImpl()
            : this(new Function())
        {
        }

        #region INoTraksaksi Members

        public string GenerateNoRegistrasi(string noCM, byte kodeProfile, string kodeDepartemenUser,
                                           DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoRegistrasi(noCM, kodeProfile, kodeDepartemenUser, tanggalStruk);
        }

        public string GenerateNoMasuk(string noRegistrasi, byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoMasuk(noRegistrasi, kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoRujukanIntern(string kodeRuanganRujukan, string noCM, byte kodeProfile,
                                              DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoRujukanIntern(kodeRuanganRujukan, noCM, kodeProfile, tanggalStruk);
        }

        public string GenerateNoHasilPeriksa(string tempNoStruk, byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoHasilPeriksa(tempNoStruk, kodeProfile, tanggalStruk);
        }

        public string GenerateNoStruk(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoStruk(kodeProfile, tanggalStruk);
        }

        public string GenerateNoBKM(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoBKM(kodeProfile, tanggalStruk);
        }

        public string GenerateNoBKK(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoBKK(kodeProfile, tanggalStruk);
        }

        public string GenerateNoOrder(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoOrder(kodeProfile, tanggalStruk);
        }

        public string GenerateNoTerima(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoTerima(kodeProfile, tanggalStruk);
        }

        public string GenerateNoKirim(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoKirim(kodeProfile, tanggalStruk);
        }

        public string GenerateNoSJP(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoSJP(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoResep(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoResep(kodeProfile, tanggalStruk);
        }

        public string GenerateNoClosing(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoClosing(kodeProfile, tanggalStruk);
        }

        public string GenerateNoPosting(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoPosting(kodeProfile, tanggalStruk);
        }

        public string GenerateNoRetur(byte kodeProfile, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoRetur(kodeProfile, tanggalStruk);
        }

        public string GenerateNoProgram(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoProgram(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoKehamilan(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoKehamilan(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoPaket(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoPaket(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoRujukan(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoRujukan(kodeProfile, noCM, tanggalStruk);
        }

        public string GenerateNoProgramBT(byte kodeProfile, string noCM, DateTime tanggalStruk)
        {
            return Transaksi.GenerateNoProgramBT(kodeProfile, noCM, tanggalStruk);
        }

        #endregion
    }
}