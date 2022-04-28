using System;
using Medifirst.QueryBuilder;
using Medifirst.QueryBuilder.Impl;
using DbContext;

namespace Medifirst.SistemAdministrasi
{
    class FunctionMarginBiayaDeposit : IStatusLimit
    {
        #region IStatusLimit Members

        public TipeStatusLimit CekStatusMarginBiayaDeposit(byte kodeProfile, int kodeKelompokPasien, byte kodePenjaminPasien, string kodeKelas, int kodeHubunganPeserta, int kodeJenisTransaksi)
        {
            IQueryBuilder query = QueryBuilderDB.CreateDatabaseQuery(Database.SQLSERVER);
            int KdJenisTransaksiPM = 0;
            int KdJenisTraksaksiOA = 0;
            //SELECT TOP 1 @KdJenisTransaksiPM=KdJenisTransaksiPM,@KdJenisTransaksiOA=KdJenisTransaksiOA FROM SettingDataFixed_M WHERE KdProfile=@KdProfile
            query.SelectTable("SettingDataFixed_M");
            query.AddField("KdJenisTransaksiPM");
            query.AddField("KdJenisTransaksiOA");
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            var data = this.GetDataFromSQL(query.BuildQuery());
            while (data.Read())
            {
                KdJenisTransaksiPM = Convert.ToInt16(data[0]);
                KdJenisTraksaksiOA = Convert.ToInt16(data[1]);
                break;
            }
            query.SelectTable("PersentaseTanggunganPenjamin_M");
            query.AddField("sum(MaxTPenjamin)");
            query.AddField("avg(PersenMaxTPenjamin)");
            query.AddCriteria(query.Criteria.Equal("KdProfile", kodeProfile));
            query.AddCriteria(query.Criteria.Equal("KdKelompokPasien", kodeKelompokPasien));
            query.AddCriteria(query.Criteria.Equal("KdPenjaminPasien", kodePenjaminPasien));
            query.AddCriteria(query.Criteria.Equal("KdKelas", kodeKelas));
            query.AddCriteria(query.Criteria.Equal("KdHubunganPeserta", kodeHubunganPeserta));
            if (kodeJenisTransaksi == KdJenisTransaksiPM)
            {
                //SELECT TOP 1 @MaksTPenjamin=sum(MaxTPenjamin),@PersenMaxTPenjamin=avg(PersenMaxTPenjamin) FROM PersentaseTanggunganPenjamin_M 
                //WHERE KdProfile=@KdProfile AND KdKelompokPasien=@KdKelompokPasien AND KdPenjaminPasien=@KdPenjaminPasien AND KdKelas=@KdKelas 
                //AND KdHubunganPeserta=@KdHubunganPeserta AND KdJenisTransaksi=@KdJenisTransaksiPM             
                query.AddCriteria(query.Criteria.Equal("KdJenisTransaksi", KdJenisTransaksiPM));

            }
            else
                if (kodeJenisTransaksi == KdJenisTraksaksiOA)
                {
                    //SELECT TOP 1 @MaksTPenjamin=sum(MaxTPenjamin),@PersenMaxTPenjamin=avg(PersenMaxTPenjamin) FROM PersentaseTanggunganPenjamin_M 
                    //WHERE KdProfile=@KdProfile AND KdKelompokPasien=@KdKelompokPasien AND KdPenjaminPasien=@KdPenjaminPasien 
                    //AND KdKelas=@KdKelas AND KdHubunganPeserta=@KdHubunganPeserta AND KdJenisTransaksi=@KdJenisTransaksiOA        
                    query.AddCriteria(query.Criteria.Equal("KdJenisTransaksi", KdJenisTransaksiPM));
                }
            data = this.GetDataFromSQL(query.BuildQuery());
            while (data.Read())
            {
                if (Convert.ToInt16(data[0]) == 0 && Convert.ToInt16(data[1]) == 0)
                {
                    return TipeStatusLimit.Tidak;
                }
                else
                {
                    return TipeStatusLimit.Ya;
                }
            }
            return TipeStatusLimit.Tidak;
        }

        #endregion
    }
    public class FunctionMarginBiayaDepositImpl : IStatusLimit
    {
        public IStatusLimit StatusLimit { get; set; }

        public FunctionMarginBiayaDepositImpl(IStatusLimit StatusLimit)
        {
            this.StatusLimit = StatusLimit;
        }
        public FunctionMarginBiayaDepositImpl()
            : this(new FunctionMarginBiayaDeposit())
        {

        }
        #region IStatusLimit Members

        public TipeStatusLimit CekStatusMarginBiayaDeposit(byte kodeProfile, int kodeKelompokPasien, byte kodePenjaminPasien, string kodeKelas, int kodeHubunganPeserta, int kodeJenisTransaksi)
        {
            return StatusLimit.CekStatusMarginBiayaDeposit(kodeProfile, kodeKelompokPasien, kodePenjaminPasien, kodeKelas, kodeHubunganPeserta, kodeJenisTransaksi);
        }

        #endregion
    }

}
