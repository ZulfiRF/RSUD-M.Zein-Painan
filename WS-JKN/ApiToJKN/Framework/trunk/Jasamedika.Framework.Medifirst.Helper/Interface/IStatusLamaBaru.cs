namespace Medifirst.SistemAdministrasi
{
    public interface IStatusLamaBaru
    {
        TipePasien CekPasienLamaBaruKunjunganRumahSakit(string noCm);
        TipePasien CekPasienLamaBaruStatusKunjunganRuanganPelayanan(string noCm, string kodeRuangan);
        TipePasien CekPasienLamaBaruKasusDiagnosaPenyakitRumahSakit(string KdDiagnosa);
        TipePasien CekPasienLamaBaruKasusDiagnosaPenyakitRuanganPelayanan(string KdDiagnosa, string kodeRuangan);
    }
}
