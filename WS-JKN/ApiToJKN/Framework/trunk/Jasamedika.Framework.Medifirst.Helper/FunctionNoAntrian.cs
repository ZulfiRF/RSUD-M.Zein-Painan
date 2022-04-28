using System;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;
using DbContext;

namespace Medifirst.SistemAdministrasi
{
    public class FunctionNoAntrian : INoAntrian
    {
        #region INoAntrian Members

        public int GenerateNoAntrianRegistrasi(int kodeProfile, DateTime tanggalAntrian)
        {
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("AntrianPasienRegistrasi_T");
            query.AddField("MAX(NoAntrian) ");
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            query.AddCriteria(query.Criteria.LikeDay("TglAntrian", tanggalAntrian));
            query.AddCriteria(query.Criteria.LikeYear("TglAntrian", tanggalAntrian));
            query.AddCriteria(query.Criteria.LikeMonth("TglAntrian", tanggalAntrian));
            string hasil = this.LastRecordIndex(query.BuildQuery());
            byte no = 1;
            return (hasil == string.Empty) ? no : Convert.ToInt16((Convert.ToInt16(hasil) + 1));
        }

        public int GenerateNoAntrianDiPeriksa(int kodeProfile, DateTime tanggalAntrian, string kodeRuangan)
        {
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            query.SelectTable("AntrianPasienDiPeriksa_T");
            query.AddField("MAX(NoAntrian) ");
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            query.AddCriteria(query.Criteria.Equal("KdRuangan", kodeRuangan));
            query.AddCriteria(query.Criteria.LikeDay("TglRegistrasi", tanggalAntrian));
            query.AddCriteria(query.Criteria.LikeYear("TglRegistrasi", tanggalAntrian));
            query.AddCriteria(query.Criteria.LikeMonth("TglRegistrasi", tanggalAntrian));
            string hasil = this.LastRecordIndex(query.BuildQuery());
            byte no = 1;
            return (hasil == string.Empty) ? no : Convert.ToInt16((Convert.ToInt16(hasil) + 1));
        }

        #endregion

    }

    public class FunctionNoAntrianImpl : INoAntrian
    {
        public INoAntrian NoAntrian { get; set; }

        public FunctionNoAntrianImpl(INoAntrian NoTransaksi)
        {
            this.NoAntrian = NoTransaksi;
        }
        public FunctionNoAntrianImpl()
            : this(new FunctionNoAntrian())
        {
        }

        #region INoAntrian Members

        public int GenerateNoAntrianRegistrasi(int kodeProfile, DateTime tanggalAntrian)
        {
            return NoAntrian.GenerateNoAntrianRegistrasi(kodeProfile, tanggalAntrian);
        }

        public int GenerateNoAntrianDiPeriksa(int kodeProfile, DateTime tanggalAntrian, string kodeRuangan)
        {
            return NoAntrian.GenerateNoAntrianDiPeriksa(kodeProfile, tanggalAntrian, kodeRuangan);
        }

        #endregion
    }
}

