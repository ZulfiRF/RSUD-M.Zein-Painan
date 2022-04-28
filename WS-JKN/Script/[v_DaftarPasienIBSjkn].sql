

/****** Object:  View [dbo].[v_DaftarPasienIBSjkn]    Script Date: 07/12/2020 10:54:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_DaftarPasienIBSjkn]
AS
SELECT        dbo.JadwalOperasi.NoIBS, dbo.JadwalOperasi.TglRencanaOperasi AS TglPendaftaran, dbo.JenisOperasi.JenisOperasi, dbo.Ruangan.KodeExternal, dbo.Ruangan.NamaExternal, dbo.PemakaianAsuransi.IdAsuransi, 
                         dbo.JadwalOperasi.TglMulaiOperasi, dbo.JadwalOperasi.TglSelesaiOperasi, dbo.Penjamin.NamaPenjamin
FROM            dbo.Ruangan INNER JOIN
                         dbo.NoKamar ON dbo.Ruangan.KdRuangan = dbo.NoKamar.KdRuangan INNER JOIN
                         dbo.JadwalOperasi INNER JOIN
                         dbo.PemakaianAsuransi ON dbo.JadwalOperasi.NoPendaftaran = dbo.PemakaianAsuransi.NoPendaftaran INNER JOIN
                         dbo.Penjamin ON dbo.PemakaianAsuransi.IdPenjamin = dbo.Penjamin.IdPenjamin INNER JOIN
                         dbo.JenisOperasi ON dbo.JadwalOperasi.KdJenisOperasi = dbo.JenisOperasi.KdJenisOperasi ON dbo.NoKamar.KdKamar = dbo.JadwalOperasi.KdKamarIBS
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Ruangan"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 271
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "NoKamar"
            Begin Extent = 
               Top = 6
               Left = 309
               Bottom = 136
               Right = 479
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "JadwalOperasi"
            Begin Extent = 
               Top = 6
               Left = 517
               Bottom = 270
               Right = 719
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "PemakaianAsuransi"
            Begin Extent = 
               Top = 6
               Left = 757
               Bottom = 136
               Right = 952
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Penjamin"
            Begin Extent = 
               Top = 6
               Left = 990
               Bottom = 166
               Right = 1182
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "JenisOperasi"
            Begin Extent = 
               Top = 6
               Left = 1220
               Bottom = 136
               Right = 1390
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         W' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'v_DaftarPasienIBSjkn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'idth = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'v_DaftarPasienIBSjkn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'v_DaftarPasienIBSjkn'
GO


