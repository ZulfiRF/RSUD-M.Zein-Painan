
/****** Object:  Table [dbo].[Sloting]    Script Date: 23/12/2020 11:17:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sloting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KdKelompokWaktu] [int] NOT NULL,
	[KdRuangan] [varchar](50) NOT NULL,
	[Slotting] [int] NULL,
	[Keterangan] [varchar](50) NULL,
	[Flag] [int] NOT NULL,
 CONSTRAINT [PK_Sloting_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


