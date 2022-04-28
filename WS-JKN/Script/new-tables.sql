

/****** Object:  Table [dbo].[ReservasiCheckin]    Script Date: 6/8/2021 2:07:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReservasiCheckin](
	[KodeBooking] [char](10) NOT NULL,
	[LastCheckinServer] [datetime] NOT NULL,
	[LastCheckinClient] [datetime] NULL,
 CONSTRAINT [PK_ReservasiCheckin] PRIMARY KEY CLUSTERED 
(
	[KodeBooking] ASC,
	[LastCheckinServer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ReservasiBatal]    Script Date: 6/8/2021 2:07:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReservasiBatal](
	[KodeBooking] [char](10) NOT NULL,
	[TglBatal] [datetime] NOT NULL,
	[Keterangan] [varchar](500) NULL,
 CONSTRAINT [PK_ReservasiBatal] PRIMARY KEY CLUSTERED 
(
	[KodeBooking] ASC,
	[TglBatal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


