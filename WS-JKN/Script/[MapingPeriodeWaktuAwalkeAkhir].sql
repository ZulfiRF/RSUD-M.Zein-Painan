

/****** Object:  Table [dbo].[MapingPeriodeWaktuAwalkeAkhir]    Script Date: 23/12/2020 11:19:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MapingPeriodeWaktuAwalkeAkhir](
	[KdKelompokWaktu] [int] IDENTITY(1,1) NOT NULL,
	[KdPeriodeWaktuAwal] [varchar](5) NOT NULL,
	[KdPeriodeWaktuAkhir] [varchar](5) NOT NULL,
	[Nama] [varchar](50) NULL,
	[Keterangan] [varchar](50) NULL,
 CONSTRAINT [PK_MapingPeriodeWaktuAwalkeAkhir] PRIMARY KEY CLUSTERED 
(
	[KdKelompokWaktu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


