namespace Medifirst.SistemAdministrasi
{
    public enum TipeStatusLimit
    {
        Tidak,
        Ya
    }

    public interface IStatusLimit
    {
        //        @KdProfile smallint,
        //@KdKelompokPasien tinyint,
        //@KdPenjaminPasien smallint,
        //@KdKelas varchar(2),
        //@KdHubunganPeserta tinyint,
        //@KdJenisTransaksi tinyint
        TipeStatusLimit CekStatusMarginBiayaDeposit(byte kodeProfile, int kodeKelompokPasien, byte kodePenjaminPasien, string kodeKelas, int kodeHubunganPeserta, int kodeJenisTransaksi);
    }
}
