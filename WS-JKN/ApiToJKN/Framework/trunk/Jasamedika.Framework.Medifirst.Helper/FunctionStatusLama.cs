using System;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;
using DbContext;

namespace Medifirst.SistemAdministrasi
{
    public enum TipeStatusLamaBaru
    {
        KunjunganRumahSakit,
        KunjunganRuanganPelayanan,
        KasusDiagnosaPenyakitRumahSakit,
        KasusDiagnosaPenyakitRuanganPelayanan
    }
    public enum TipePasien
    {
        PasienLama,
        PasienBaru
    }
    class FunctionStatusLama : IStatusLamaBaru
    {
        #region IStatusLamaBaru Members

        public TipePasien CekPasienLamaBaruKunjunganRumahSakit(string noCm)
        {
            //    SELECT @RowData=COUNT(NoCM) FROM PasienDaftar_T WHERE NoCM=@NoCM
            //IF @RowData=0
            //    SET @StatusLamaBaru=0
            //ELSE
            //    SET @StatusLamaBaru=1    
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("PasienDaftar_T");
            query.AddField("COUNT(NoCM) ");
            query.AddCriteria(query.Criteria.Equal("NoCM", noCm));
            return (this.LastRecordIndex(query.BuildQuery()) == "0") ? TipePasien.PasienBaru : TipePasien.PasienLama;

        }

        public TipePasien CekPasienLamaBaruStatusKunjunganRuanganPelayanan(string noCm, string kodeRuangan)
        {
            //    SELECT @RowData=COUNT(NoCM) FROM RegistrasiPelayananPasien_T WHERE NoCM=@NoCM AND KdRuangan=@KdRuangan
            //IF @RowData=0
            //    SET @StatusLamaBaru=0
            //ELSE
            //    SET @StatusLamaBaru=1
            //GOTO Selesai
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("RegistrasiPelayananPasien_T");
            query.AddField("COUNT(NoCM) ");
            query.AddCriteria(query.Criteria.Equal("NoCM", noCm));
            query.AddCriteria(query.Criteria.Equal("KdRuangan", kodeRuangan));
            return (this.LastRecordIndex(query.BuildQuery()) == "0") ? TipePasien.PasienBaru : TipePasien.PasienLama;
        }

        public TipePasien CekPasienLamaBaruKasusDiagnosaPenyakitRumahSakit(string KdDiagnosa)
        {
            //    SELECT @RowData=COUNT(KdDiagnosa) FROM PemeriksaanDiagnosaPasien_T WHERE KdDiagnosa=@KdDiagnosa
            //IF @RowData=0
            //    SET @StatusLamaBaru=0
            //ELSE
            //    SET @StatusLamaBaru=1
            //GOTO Selesai
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("PemeriksaanDiagnosaPasien_T");
            query.AddField("COUNT(KdDiagnosa) ");
            query.AddCriteria(query.Criteria.Equal("KdRuangan", KdDiagnosa));
            return (this.LastRecordIndex(query.BuildQuery()) == "0") ? TipePasien.PasienBaru : TipePasien.PasienLama;
        }

        public TipePasien CekPasienLamaBaruKasusDiagnosaPenyakitRuanganPelayanan(string KdDiagnosa, string kodeRuangan)
        {
            //    SELECT @RowData=COUNT(KdDiagnosa) FROM PemeriksaanDiagnosaPasien_T WHERE KdDiagnosa=@KdDiagnosa AND KdRuangan=@KdRuangan
            //IF @RowData=0
            //    SET @StatusLamaBaru=0
            //ELSE
            //    SET @StatusLamaBaru=1
            //GOTO Selesai
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("PemeriksaanDiagnosaPasien_T");
            query.AddField("COUNT(KdDiagnosa) ");
            query.AddCriteria(query.Criteria.Equal("KdDiagnosa", KdDiagnosa));
            query.AddCriteria(query.Criteria.Equal("KdRuangan", kodeRuangan));
            return (this.LastRecordIndex(query.BuildQuery()) == "0") ? TipePasien.PasienBaru : TipePasien.PasienLama;
        }

        #endregion
    }
    public class FunctionStatusLamaImpl : IStatusLamaBaru
    {
        public IStatusLamaBaru StatusLamaBaru { get; set; }
        public FunctionStatusLamaImpl(IStatusLamaBaru StatusLamaBaru)
        {
            this.StatusLamaBaru = StatusLamaBaru;
        }
        public FunctionStatusLamaImpl()
            : this(new FunctionStatusLama())
        {

        }
        #region IStatusLamaBaru Members

        public TipePasien CekPasienLamaBaruKunjunganRumahSakit(string noCm)
        {
            return StatusLamaBaru.CekPasienLamaBaruKunjunganRumahSakit(noCm);
        }

        public TipePasien CekPasienLamaBaruStatusKunjunganRuanganPelayanan(string noCm, string kodeRuangan)
        {
            return StatusLamaBaru.CekPasienLamaBaruStatusKunjunganRuanganPelayanan(noCm, kodeRuangan);
        }

        public TipePasien CekPasienLamaBaruKasusDiagnosaPenyakitRumahSakit(string KdDiagnosa)
        {
            return StatusLamaBaru.CekPasienLamaBaruKasusDiagnosaPenyakitRumahSakit(KdDiagnosa);
        }

        public TipePasien CekPasienLamaBaruKasusDiagnosaPenyakitRuanganPelayanan(string KdDiagnosa, string kodeRuangan)
        {
            return StatusLamaBaru.CekPasienLamaBaruKasusDiagnosaPenyakitRuanganPelayanan(KdDiagnosa, kodeRuangan);
        }

        #endregion
    }
}
