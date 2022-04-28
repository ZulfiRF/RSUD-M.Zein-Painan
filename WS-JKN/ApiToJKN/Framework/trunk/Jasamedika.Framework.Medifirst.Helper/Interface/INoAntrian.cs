using System;

namespace Medifirst.SistemAdministrasi
{
    public interface INoAntrian
    {
        int GenerateNoAntrianRegistrasi(int kodeProfile, DateTime tanggalAntrian);
        int GenerateNoAntrianDiPeriksa(int kodeProfile, DateTime tanggalAntrian, string kodeRuangan);
    }
}
