

/****** Object:  UserDefinedFunction [dbo].[Jumlah_Pasien_Terdaftar]    Script Date: 23/12/2020 11:21:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE FUNCTION [dbo].[Jumlah_Pasien_Terdaftar](@tanggal varchar(10),@KdKelompokWaktu integer,@kdRuangan varchar(3))  
RETURNS integer
AS  
BEGIN 
	declare @Jml integer
			select @jml= count(Reservasi.KodeBooking) FROM         DetailReservasi INNER JOIN
                      Reservasi ON DetailReservasi.KodeBooking = Reservasi.KodeBooking
                     where Convert(varchar(10),Reservasi.TglReservasi,126) = @tanggal and DetailReservasi.KdKelompokWaktu = @kdKelompokWaktu and DetailReservasi.KdRuangan=@KdRuangan                                         
			return @jml	
END


GO


