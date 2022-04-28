using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medifirst.SistemAdministrasi
{

    public interface INoTransaksiMasihBerlaku
    {
        string GenerateNoBerlaku(byte kodeProfile, string noCm, DateTime tanggalTransaksi, TipeTransaksi jenisTransaksi);
    }
}
