using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medifirst.SistemAdministrasi
{
    public interface INoTraksaksi
    {
        string GenerateNoRegistrasi(string noCM, byte kodeProfile, string kodeDepartemenUser, DateTime tanggalStruk);
        string GenerateNoMasuk(string noRegistrasi, byte kodeProfile, string noCM, DateTime tanggalStruk);
        string GenerateNoRujukan(byte kodeProfile, string noCM, DateTime tanggalStruk);
        string GenerateNoRujukanIntern(string kodeRuanganRujukan, string noCM, byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoHasilPeriksa(string tempNoStruk, byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoStruk(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoBKM(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoBKK(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoOrder(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoTerima(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoKirim(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoSJP(byte kodeProfile, string noCM, DateTime tanggalStruk);
        string GenerateNoResep(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoClosing(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoPosting(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoRetur(byte kodeProfile, DateTime tanggalStruk);
        string GenerateNoProgram(byte kodeProfile, string noCM, DateTime tanggalStruk);
        string GenerateNoKehamilan(byte kodeProfile, string noCM, DateTime tanggalStruk);
        string GenerateNoPaket(byte kodeProfile, string noCM, DateTime tanggalStruk);
        string GenerateNoProgramBT(byte kodeProfile, string noCM, DateTime tanggalStruk);
    }
}
