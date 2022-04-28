using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToJKN.Models
{
    public class slotingModel
    {
      
            public Int32 Id { get; set; }
            public Int32 KdKelompokWaktu { get; set; }
            public Int32 KdRuangan { get; set; }
            public Int32 Sloting { get; set; }
            public string Keterangan { get; set; }
            public Int16 Flag { get; set; }
            public Int32 SisaSlotting { get; set; }
            public Int32 JumlahPasienTerdaftar { get; set; }
            public string namaRuangan { get; set; }

    }
}