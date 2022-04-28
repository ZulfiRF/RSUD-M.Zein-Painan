SDk VClaim v1.0
================================
PT. Jasamedika Saranatama
Medifirst2000
================================
Lengkapnya Kunjungi --> http://dvlp.bpjs-kesehatan.go.id:8081/VClaim-Katalog/
================================


1. Pembuatan SEP
1.1. Insert SEP
{
           "request": {
              "t_sep": {
                 "noKartu": "{nokartu BPJS}",
                 "tglSep": "{tanggal penerbitan sep format yyyy-mm-dd}",
                 "ppkPelayanan": "{kode faskes pemberi pelayanan}",
                 "jnsPelayanan": "{jenis pelayanan = 1. r.inap 2. r.jalan}",
                 "klsRawat": "{kelas rawat 1. kelas 1, 2. kelas 2 3.kelas 3}",
                 "noMR": "{nomor medical record RS}",
                 "rujukan": {
                    "asalRujukan": "{asal rujukan ->1.Faskes 1, 2. Faskes 2(RS)}",
                    "tglRujukan": "{tanggal rujukan format: yyyy-mm-dd}",
                    "noRujukan": "{nomor rujukan}",
                    "ppkRujukan": "{kode faskes rujukam -> baca di referensi faskes}"
                 },
                 "catatan": "{catatan peserta}",
                 "diagAwal": "{diagnosa awal ICD10 -> baca di referensi diagnosa}",
                 "poli": {
                    "tujuan": "{kode poli -> baca di referensi poli}",
                    "eksekutif": "{poli eksekutif -> 0. Tidak 1.Ya}"
                 },
                 "cob": {
                    "cob": "{coba -> 0.Tidak 1. Ya}"
                 },
                 "jaminan": {
                    "lakaLantas": "{kejadian lakalantas -> 0. Tidak 1. Ya}",
                    "penjamin": "{penjamin lakalantas -> 1=Jasa raharja PT, 2=BPJS Ketenagakerjaan, 3=TASPEN PT, 4=ASABRI PT} jika lebih dari 1 isi -> 1,2 (pakai delimiter koma)",
                    "lokasiLaka": "{diisi lokasi kejadian kecelakaan}"
                 },
                 "noTelp": "{nomor telepon peserta/pasien}",
                 "user": "{user pembuat SEP}"
              }
           }
        }     
1.2. Update SEP
 {
       "request": {
          "t_sep": {
             "noSep": "{nomor sep}",
             "klsRawat": "kelas rawat 1. kelas 1, 2. kelas 2 3.kelas 3",
             "noMR": "{nomor medical record RS}",
             "rujukan": {
                "asalRujukan": "{asal rujukan ->1.Faskes 1, 2. Faskes 2(RS)}",
                "tglRujukan": "{tanggal rujukan format: yyyy-mm-dd}",
                "noRujukan": "{nomor rujukan}",
                "ppkRujukan": "{kode faskes rujukam -> baca di referensi faskes}"
             },
             "catatan": "{catatan peserta}",
             "diagAwal": "{diagnosa awal ICD10 -> baca di referensi diagnosa}",
             "poli": {
                "eksekutif": "{poli eksekutif -> 0. Tidak 1.Ya}"
             },
             "cob": {
                "cob": "{coba -> 0.Tidak 1. Ya}"
             },
             "jaminan": {
                "lakaLantas": "{kejadian lakalantas -> 0. Tidak 1. Ya}",
                "penjamin": "{penjamin lakalantas -> 1. Jasa Raharja, 2. BPJS Ketenagakerjaan 3. TASPEN 4.ASABRI}",
                "lokasiLaka": "{diisi lokasi kejadian kecelakaan}"
             },
             "noTelp": "{nomor telepon peserta/pasien}",
             "user": "{user pembuat SEP}"
          }
       }
    }        
1.3. Delete SEP
{
       "request": {
          "t_sep": {
             "noSep": "{nomor SEP}",
             "user": "{user pengguna SEP}"
          }
       }
    }
1.4. Cari SEP
{nomor SEP}

2. Approval Penjaminan SEP
2.1. Pengajuan
 {
       "request": {
          "t_sep": {
             "noKartu": "0003814312013",
             "tglSep": "2017-10-26",
             "jnsPelayanan": "1",
             "keterangan": "Hari libur",
             "user": "Coba Ws"
          }
       }
    }        
2.2. Aproval Pengajuan SEP
{
       "request": {
          "t_sep": {
             "noKartu": "0003814312013",
             "tglSep": "2017-10-26",
             "jnsPelayanan": "1",
             "keterangan": "Hari libur",
             "user": "Coba Ws"
          }
       }
    }   

3. Update Tgl Pulang SEP
 {  
            "request": 
                {    
                "t_sep":
                    {
                        noSep":"0301R00105160000569",
                        "tglPlg":"2016-06-12",
                        "ppkPelayanan":"0301R001"
                    }
                }
        }                  
           	     
4. Peserta
4.1. No.Kartu BPJS
{nomor kartu}				         
{tanggal sep}				         

4.2. NIK
{NIK}				         
{tanggal sep}				         

5. Cari Rujukan
5.1. Rujukan Berdasarkan Nomor Rujukan (PCare)
{nomor rujukan}
5.2. Rujukan Berdasarkan Nomor Rujukan (RS)
{nomor rujukan}
5.1. Rujukan Berdasarkan Nomor Kartu (PCare)
{nomor kartu}
5.2. Rujukan Berdasarkan Nomor Kartu (RS)
{nomor kartu}

6. Pembuatan Rujukan
6.1. Insert Rujukan
{
       "request": {
          "t_rujukan": {
             "noSep": "{nomor sep}",
             "tglRujukan": "{tanggal rujukan format : yyyy-mm-dd}",
             "ppkDirujuk": "{faskes dirujuk -> data di referensi faskes}",
             "jnsPelayanan": "{jenis pelayanan -> 1.R.Inap 2.R.Jalan}",
             "catatan": "{catatan rujukan}",
             "diagRujukan": "{kode diagnosa rujukan -> data di referensi diagnosa}",
             "tipeRujukan": "{tipe rujukan -> 0.penuh, 1.Partial 2.rujuk balik}",
             "poliRujukan": "{kode poli rujukan -> data di referensi poli}",
             "user": "{user pemakai}"
          }
       }
    }    
6.2. Update Rujukan
{
       "request": {
          "t_rujukan": {
             "noRujukan": "{nomor rujukan}",
             "ppkDirujuk": "{faskes dirujuk -> data di referensi faskes}",
             "tipe": "{tipe rujukan -> 0.penuh, 1.Partial 2.rujuk balik}",
             "jnsPelayanan": "{jenis pelayanan -> 1.R.Inap 2.R.Jalan}",
             "catatan": "{catatan rujukan}",
             "diagRujukan": "{kode diagnosa rujukan -> data di referensi diagnosa}",
             "tipeRujukan": "{tipe rujukan -> 0.penuh, 1.Partial 2.rujuk balik}",
             "poliRujukan": "{kode poli rujukan -> data di referensi poli}",
             "user": "{user pemakai}"
          }
       }
    }   
6.3. Delete Rujukan
 {
       "request": {
          "t_rujukan": {
             "noRujukan": "{nomor rujukan}",
             "user": "{user pemakai}"
          }
       }
    }             
	
7. Lembar Pengajuan Klaim
7.1. Insert LPK	           
{
       "request": {
          "t_lpk": {
             "noSep": "{nomor sep}",
             "tglMasuk": "{tanggal masuk format yyyy-mm-dd}",
             "tglKeluar": "{tanggal keluar format yyyy-mm-dd}",
             "jaminan": "{penjamin -> 1. JKN}",
             "poli": {
                "poli": "{kode poli -> data di referensi poli}"
             },
             "perawatan": {
                "ruangRawat": "{ruang rawat -> data di referensi ruang rawat}",
                "kelasRawat": "{kelas rawat -> data di referensi kelas rawat}",
                "spesialistik": "{spesialistik -> data di referensi spesialistik}",
                "caraKeluar": "{cara keluar -> data di referensi cara keluar}",
                "kondisiPulang": "{kondisi pulang -> data di referensi kondisi pulang}"
             },
             "diagnosa": [
                {
                   "kode": "{kode diagnosa  -> data di referensi diagnosa}",
                   "level": "{level diagnosa -> 1.Primer 2.Sekunder}"
                },
                {
                   "kode": "{kode diagnosa  -> data di referensi diagnosa}",
                   "level": "{level diagnosa -> 1.Primer 2.Sekunder}"
                }
             ],
             "procedure": [
                {
                   "kode": "{kode procedure -> data di referensi procedure/tindakan}"
                },
                {
                   "kode": "{kode procedure -> data di referensi procedure/tindakan}"
                }
             ],
             "rencanaTL": {
                "tindakLanjut": "{tindak lanjut -> 1:Diperbolehkan Pulang, 2:Pemeriksaan Penunjang, 3:Dirujuk Ke, 4:Kontrol Kembali}",
                "dirujukKe": {
                   "kodePPK": "{kode faskes -> data di referensi faskes}"
                },
                "kontrolKembali": {
                   "tglKontrol": "{tanggal kontrol kembali format : yyyy-mm-dd}",
                   "poli": "{kode poli -> data di referensi poli}"
                }
             },
             "DPJP": "{kode dokter dpjp -> data di referensi dokter}",
             "user": "{user pemakai}"
          }
       }
    }           

	*) Contoh memasukan Diagnosa dari VB6 
		--> A01:1;Z00:2;Z01:2; 
		--> KdDiagnosa:Level;KdDiagnosa:Level;

	*) Contoh memasukan Prosedur dari VB6 
		--> 00.82;00.83
		--> KdDiagnosa;KdDiagnosa;

7.2. Update LPK
{
       "request": {
          "t_lpk": {
             "noSep": "{nomor sep}",
             "tglMasuk": "{tanggal masuk format yyyy-mm-dd}",
             "tglKeluar": "{tanggal keluar format yyyy-mm-dd}",
             "jaminan": "{penjamin -> 1. JKN}",
             "poli": {
                "poli": "{kode poli -> data di referensi poli}"
             },
             "perawatan": {
                "ruangRawat": "{ruang rawat -> data di referensi ruang rawat}",
                "kelasRawat": "{kelas rawat -> data di referensi kelas rawat}",
                "spesialistik": "{spesialistik -> data di referensi spesialistik}",
                "caraKeluar": "{cara keluar -> data di referensi cara keluar}",
                "kondisiPulang": "{kondisi pulang -> data di referensi kondisi pulang}"
             },
             "diagnosa": [
                {
                   "kode": "{kode diagnosa  -> data di referensi diagnosa}",
                   "level": "{level diagnosa -> 1.Primer 2.Sekunder}"
                },
                {
                   "kode": "{kode diagnosa  -> data di referensi diagnosa}",
                   "level": "{level diagnosa -> 1.Primer 2.Sekunder}"
                }
             ],
             "procedure": [
                {
                   "kode": "{kode procedure -> data di referensi procedure/tindakan}"
                },
                {
                   "kode": "{kode procedure -> data di referensi procedure/tindakan}"
                }
             ],
             "rencanaTL": {
                "tindakLanjut": "{tindak lanjut -> 1:Diperbolehkan Pulang, 2:Pemeriksaan Penunjang, 3:Dirujuk Ke, 4:Kontrol Kembali}",
                "dirujukKe": {
                   "kodePPK": "{kode faskes -> data di referensi faskes}"
                },
                "kontrolKembali": {
                   "tglKontrol": "{tanggal kontrol kembali format : yyyy-mm-dd}",
                   "poli": "{kode poli -> data di referensi poli}"
                }
             },
             "DPJP": "{kode dokter dpjp -> data di referensi dokter}",
             "user": "{user pemakai}"
          }
       }
    }

	*) Contoh memasukan Diagnosa dari VB6 
		--> A01:1;Z00:2;Z01:2; 
		--> KdDiagnosa:Level;KdDiagnosa:Level;

	*) Contoh memasukan Prosedur dari VB6 
		--> 00.82;00.83
		--> KdDiagnosa;KdDiagnosa;

7.3. Delete LPK
{
       "request": {
          "t_lpk": {
             "noSep": "{nomor sep}"
          }
       }
    }                  
       
7.4. Data Lembar Pengajuan Klaim
{Tanggal Masuk}
{Jenis Pelayanan}

8. Monitoring
8.1. Data Kunjungan
{Tanggal Sep}
{Jenis Pelayanan}

8.2. Data Klaim
{Tanggal Pulang}
{Jenis Pelayanan}
{Status Klaim}

9. Referensi
9.1. Diagnosa
{Kode atau Nama Diagnosa}         
9.2. Poli
{Kode atau Nama Poli}         
9.3. Fasilitas Kesehatan
{Kode atau Nama Faskes}         
{Jenis Faskes (1. Faskes 1, 2. Faskes 2/RS)}        
9.4. Procedure / Tindakan
{Kode atau Nama Procedure / Tindakan}        
9.5. Kelas Rawat
9.6. Dokter
{Nama Dokter / DPJP}
9.7. Spesialistik
9.8. Ruang Rawat
9.9. Cara Keluar
9.10. Pasca Pulang