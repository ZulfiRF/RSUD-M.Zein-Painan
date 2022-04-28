

/****** Object:  UserDefinedFunction [dbo].[Ambil_JmlAntrianTerlayani]    Script Date: 23/12/2020 11:47:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE FUNCTION [dbo].[Ambil_JmlAntrianTerlayani]
(@Tgl datetime)  
RETURNS integer
AS  
BEGIN 
  declare @Jml integer
 
SELECT   @Jml =  count(Reservasi.TglReservasi)
FROM         DetailReservasi INNER JOIN
                      Reservasi ON DetailReservasi.KdRuangan = Reservasi.KdRuangan AND DetailReservasi.KodeBooking = Reservasi.KodeBooking
WHERE     (DetailReservasi.StatusPasien = 'Y') and   Convert(varchar(10),Reservasi.TglReservasi,126) = @Tgl

  return @Jml

END
GO


