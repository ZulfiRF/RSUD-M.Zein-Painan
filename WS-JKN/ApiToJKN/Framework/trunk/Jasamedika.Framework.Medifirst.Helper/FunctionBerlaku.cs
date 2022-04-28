using System;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;
using DbContext;

namespace Medifirst.SistemAdministrasi
{
    class FunctionBerlaku : INoTransaksiMasihBerlaku
    {
        #region INoTransaksiMasihBerlaku Members

        public string GenerateNoBerlaku(byte kodeProfile, string noCm, DateTime tanggalTransaksi, TipeTransaksi jenisTransaksi)
        {
            switch (jenisTransaksi)
            {
                case TipeTransaksi.NoRujukan:
                    return NoRujukan(kodeProfile, noCm, tanggalTransaksi);
                case TipeTransaksi.NoSJP:
                    return NoSJP(kodeProfile, noCm, tanggalTransaksi);
                case TipeTransaksi.NoPaket:
                    return NoPaket(kodeProfile, noCm, tanggalTransaksi);
                case TipeTransaksi.NoProgram:
                    return NoProgram(kodeProfile, noCm, tanggalTransaksi);
                case TipeTransaksi.NoKehamilan:
                    return NoKehamilan(kodeProfile, noCm, tanggalTransaksi);
                default:
                    break;
            }
            return "";
        }

        private string NoKehamilan(byte kodeProfile, string noCm, DateTime tanggalTransaksi)
        {
            //    SELECT TOP 1 @NoTransaksi=NoKehamilan FROM PemeriksaanKehamilanPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND isBersalin=0
            //IF @@ROWCOUNT=0
            //    SET @OutputNoTransaksi=null
            //ELSE
            //    SET @OutputNoTransaksi=@NoTransaksi

            //GOTO Selesai
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("PemeriksaanKehamilanPasien_T");
            query.AddField("NoKehamilan");
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            query.AddCriteria(query.Criteria.Equal("NoCM", noCm));
            query.AddCriteria(query.Criteria.Equal("isBersalin", 0));
            string hasil = query.BuildQuery();
            var data = this.GetDataFromSQL(hasil);
            while (data.Read())
            {
                return data[0].ToString();
            }
            return "";
        }

        private string NoProgram(byte kodeProfile, string noCm, DateTime tanggalTransaksi)
        {
            //    SELECT TOP 1 @NoTransaksi=NoProgram FROM ProgramKeluargaBerencana_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglDicabut IS NULL
            //IF @@ROWCOUNT=0
            //    SET @OutputNoTransaksi=null
            //ELSE
            //    SET @OutputNoTransaksi=@NoTransaksi

            //GOTO Selesai
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("ProgramKeluargaBerencana_T");
            query.AddField("NoProgram");
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            query.AddCriteria(query.Criteria.Equal("NoCM", noCm));
            string hasil = query.BuildQuery() + " AND TglDicabut is NULL";
            var data = this.GetDataFromSQL(hasil);
            while (data.Read())
            {
                return data[0].ToString();
            }
            return "";
        }

        private string NoPaket(byte kodeProfile, string noCm, DateTime tanggalTransaksi)
        {
            //    SELECT TOP 1 @NoTransaksi=NoPaket FROM PaketKunjunganPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglAkhirPaket<=@TglTransaksi
            //IF @@ROWCOUNT=0
            //    SET @OutputNoTransaksi=null
            //ELSE
            //BEGIN
            //    SELECT @QtyMaksKunjungan=QtyMaksKunjungan,@QtyKunjunganKe=QtyKunjunganKe FROM PaketKunjunganPasien_T WHERE KdProfile=@KdProfile AND NoPaket=@NoTransaksi
            //    IF @QtyKunjunganKe<@QtyMaksKunjungan
            //        SET @OutputNoTransaksi=@NoTransaksi
            //    ELSE
            //        SET @OutputNoTransaksi=null
            //END
            //GOTO Selesai
            return GenerateQuery("PaketKunjunganPasien_T", kodeProfile, noCm, "NoSJP", "AND TglAkhirPaket <= '" + tanggalTransaksi.ToString("yyyy-MM-dd") + "'", new string[] { "QtyMaksKunjungan", "QtyKunjunganKe" });
        }

        private string NoSJP(byte kodeProfile, string noCm, DateTime tanggalTransaksi)
        {
            //    SELECT TOP 1 @NoTransaksi=NoSJP FROM PemakaianAsuransiPasien_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglMaksBerlaku<=@TglTransaksi
            //IF @@ROWCOUNT=0
            //    SET @OutputNoTransaksi=null
            //ELSE
            //BEGIN
            //    SELECT @QtyMaksKunjungan=QtyMaksKunjungan,@QtyKunjunganKe=QtyKunjunganKe FROM PemakaianAsuransiPasien_T WHERE KdProfile=@KdProfile AND NoSJP=@NoTransaksi
            //    IF @QtyKunjunganKe<@QtyMaksKunjungan
            //        SET @OutputNoTransaksi=@NoTransaksi
            //    ELSE
            //        SET @OutputNoTransaksi=null
            //END
            return GenerateQuery("PemakaianAsuransiPasien_T", kodeProfile, noCm, "NoSJP", "AND TglMaksBerlaku <= '" + tanggalTransaksi.ToString("yyyy-MM-dd") + "'", new string[] { "QtyMaksKunjungan", "QtyKunjunganKe" });
        }

        private string NoRujukan(byte kodeProfile, string noCm, DateTime tanggalTransaksi)
        {
            //    SELECT TOP 1 @NoTransaksi=NoRujukan FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglMaksBerlaku<=@TglTransaksi
            //IF @@ROWCOUNT=0
            //    SET @OutputNoTransaksi=null
            //ELSE
            //BEGIN
            //    SELECT @QtyMaksKunjungan=QtyMaksKunjungan,@QtyKunjunganKe=QtyKunjunganKe FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND NoRujukan=@NoTransaksi
            //    IF @QtyKunjunganKe<@QtyMaksKunjungan
            //        SET @OutputNoTransaksi=@NoTransaksi
            //    ELSE
            //        SET @OutputNoTransaksi=null
            //END
            //GOTO Selesai            
            return GenerateQuery("PasienDiRujukEksternal_T", kodeProfile, noCm, "NoRujukan", "AND TglMaksBerlaku <= '" + tanggalTransaksi.ToString("yyyy-MM-dd") + "'", new string[] { "QtyMaksKunjungan", "QtyKunjunganKe" });
        }

        private string GenerateQuery(string tabelName, byte kodeProfile, string noCm, string getField, string whereDateCount, string[] Qty)
        {
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            int qtyMaksimumKunjungan = 0;
            int qtyKunjunganKe = 0;
            query.SelectTable(tabelName);
            query.AddField(getField);
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            query.AddCriteria(query.Criteria.Equal("NoCM", noCm));
            //    SELECT TOP 1 @NoTransaksi=NoRujukan FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND NoCM=@NoCM AND TglMaksBerlaku<=@TglTransaksi            

            string hasil = query.BuildQuery() + " " + whereDateCount;
            hasil = this.LastRecordIndex(hasil);
            if (hasil != string.Empty)
            {
                if (Qty.Length == 2)
                {
                    //    SELECT @QtyMaksKunjungan=QtyMaksKunjungan,@QtyKunjunganKe=QtyKunjunganKe FROM PasienDiRujukEksternal_T WHERE KdProfile=@KdProfile AND NoRujukan=@NoTransaksi
                    query.SelectTable(tabelName);
                    foreach (var item in Qty)
                    {
                        query.AddField(item);
                    }
                    query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
                    query.AddCriteria(query.Criteria.Equal(getField, hasil));
                    var data = this.GetDataFromSQL(query.BuildQuery());
                    while (data.Read())
                    {
                        qtyMaksimumKunjungan = (int)data[0];
                        qtyKunjunganKe = (int)data[1];
                    }
                    if (qtyKunjunganKe < qtyMaksimumKunjungan)
                    {
                        return hasil;
                    }
                    else
                    {
                        return "";
                    }
                }

                return "";
            }
            else
                return "";
        }

        #endregion
    }

    public class FucntionBerlakuIml : INoTransaksiMasihBerlaku
    {
        public INoTransaksiMasihBerlaku Transaksi { get; set; }
        public FucntionBerlakuIml(INoTransaksiMasihBerlaku Transaksi)
        {
            this.Transaksi = Transaksi;
        }
        public FucntionBerlakuIml()
            : this(new FunctionBerlaku())
        {

        }
        #region INoTransaksiMasihBerlaku Members

        public string GenerateNoBerlaku(byte kodeProfile, string noCm, DateTime tanggalTransaksi, TipeTransaksi jenisTransaksi)
        {
            return this.Transaksi.GenerateNoBerlaku(kodeProfile, noCm, tanggalTransaksi, jenisTransaksi);
        }

        #endregion

    }
}
