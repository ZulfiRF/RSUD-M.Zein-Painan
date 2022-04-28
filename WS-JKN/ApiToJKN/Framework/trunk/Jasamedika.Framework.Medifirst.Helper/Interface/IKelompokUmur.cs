using System;

namespace Medifirst.SistemAdministrasi
{
    public enum TipeKelompokUmur
    {
        TidakTermasuk,
        Termasuk
    }
    public interface IKelompokUmur
    {
        TipeKelompokUmur GetKelompokUmur(byte KdProfile, string NoCM, DateTime TglPeriksa, DateTime TglLahir, int KelompokUmur);
    }
}
