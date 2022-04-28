

/****** Object:  Table [dbo].[DetailReservasi]    Script Date: 23/12/2020 11:15:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DetailReservasi](
	[KodeBooking] [varchar](10) NOT NULL,
	[NoCM] [nchar](12) NULL,
	[NoIdentitas] [varchar](20) NULL,
	[NamaLengkap] [varchar](50) NOT NULL,
	[Alamat] [varchar](50) NULL,
	[NoTelp] [varchar](15) NULL,
	[Email] [varchar](50) NOT NULL,
	[KdRuangan] [char](3) NULL,
	[IdPegawai] [varchar](10) NULL,
	[KdKelompokWaktu] [int] NOT NULL,
	[JenisPasien] [char](20) NULL,
	[StatusPasien] [char](1) NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Reservasi]    Script Date: 23/12/2020 11:15:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Reservasi](
	[NoReservasi] [varchar](11) NOT NULL,
	[KodeBooking] [char](10) NULL,
	[TglReservasi] [datetime] NOT NULL,
	[TglRegistrasi] [datetime] NOT NULL,
	[KdRuangan] [char](3) NOT NULL,
 CONSTRAINT [PK_Reservasi_1] PRIMARY KEY CLUSTERED 
(
	[NoReservasi] ASC,
	[TglReservasi] ASC,
	[TglRegistrasi] ASC,
	[KdRuangan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ReservasiLogin]    Script Date: 23/12/2020 11:15:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReservasiLogin](
	[NoCM] [varchar](10) NOT NULL,
	[Password] [varbinary](50) NOT NULL,
	[Email] [varchar](50) NULL,
	[NoTelp] [varchar](20) NULL,
	[StatusEnabled] [smallint] NOT NULL,
	[isVerify] [tinyint] NULL,
 CONSTRAINT [PK_ReservasiLogin] PRIMARY KEY CLUSTERED 
(
	[NoCM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ReservasiPasienReport]    Script Date: 23/12/2020 11:15:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReservasiPasienReport](
	[NoReservasi] [varchar](11) NOT NULL,
	[NoCM] [varchar](12) NULL,
	[NamaLengkap] [varchar](50) NOT NULL,
	[JenisKelamin] [char](2) NOT NULL,
	[TglLahir] [datetime] NOT NULL,
	[TglPemesanan] [smalldatetime] NOT NULL,
	[KdRuangan] [char](3) NOT NULL,
	[IdDokter] [char](10) NULL,
	[TglMasuk] [smalldatetime] NULL,
	[Keterangan] [varchar](150) NULL,
	[IdUser] [char](10) NOT NULL,
	[NoTlp] [varchar](15) NULL,
	[NoAntrian] [char](3) NOT NULL,
	[StatusDaftar] [char](1) NULL,
	[KdKelas] [char](2) NULL,
	[KdKamar] [char](4) NULL,
	[NoBed] [char](2) NULL,
	[NoPendaftaran] [char](10) NULL,
 CONSTRAINT [PK_ReservasiPasienReport_1] PRIMARY KEY CLUSTERED 
(
	[NoReservasi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReservasiPasienReport]  WITH NOCHECK ADD  CONSTRAINT [FK_ReservasiPasienReport_Ruangan] FOREIGN KEY([KdRuangan])
REFERENCES [dbo].[Ruangan] ([KdRuangan])
GO

ALTER TABLE [dbo].[ReservasiPasienReport] NOCHECK CONSTRAINT [FK_ReservasiPasienReport_Ruangan]
GO


