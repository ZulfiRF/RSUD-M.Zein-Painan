using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToJKN.Models
{
    public class JadwalOprasiModel
    {
        public string NoRiwayat { get; set; }
        public string NoIbs { get; set; }
        public string TglOprasi { get; set; }
        public string JenisOprasi { get; set; }
        public string KodePli { get; set; }
        public string NamaPoli { get; set; }
        public string status { get; set; }
    }
}