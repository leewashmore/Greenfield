
/****** Object:  View [dbo].[Portfolio_Security_Target_Baseview]    Script Date: 03/08/2013 13:32:58 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Portfolio_Security_Target_Baseview]'))
DROP VIEW [dbo].[Portfolio_Security_Target_Baseview]
GO


/****** Object:  View [dbo].[Portfolio_Security_Target_Baseview]    Script Date: 03/08/2013 13:33:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[Portfolio_Security_Target_Baseview]
AS
SELECT     dbo.Portfolio_Security_Targets_Union.PORTFOLIO_ID, dbo.Portfolio_Security_Targets_Union.TARGET_PCT, dbo.Portfolio_Security_Targets_Union.SECURITY_ID, 
                      dbo.GF_SECURITY_BASEVIEW.ASEC_SEC_SHORT_NAME, dbo.GF_SECURITY_BASEVIEW.SECURITY_TYPE, 
                      dbo.GF_SECURITY_BASEVIEW.ASHEMM_PROPRIETARY_REGION_CODE,dbo.GF_SECURITY_BASEVIEW.ISO_COUNTRY_CODE, dbo.GF_SECURITY_BASEVIEW.GICS_SECTOR, 
                      dbo.GF_SECURITY_BASEVIEW.GICS_SECTOR_NAME, dbo.GF_SECURITY_BASEVIEW.GICS_INDUSTRY, dbo.GF_SECURITY_BASEVIEW.GICS_INDUSTRY_NAME, 
                      dbo.GF_SECURITY_BASEVIEW.GICS_SUB_INDUSTRY, dbo.GF_SECURITY_BASEVIEW.GICS_SUB_INDUSTRY_NAME,dbo.GF_SECURITY_BASEVIEW.LOOK_THRU_FUND,dbo.GF_SECURITY_BASEVIEW.ISSUER_NAME
FROM         dbo.Portfolio_Security_Targets_Union LEFT OUTER JOIN
                      dbo.GF_SECURITY_BASEVIEW ON dbo.Portfolio_Security_Targets_Union.SECURITY_ID = dbo.GF_SECURITY_BASEVIEW.SECURITY_ID 






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
         Begin Table = "Portfolio_Security_Targets_Union"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 273
               Right = 202
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "GF_SECURITY_BASEVIEW"
            Begin Extent = 
               Top = 6
               Left = 240
               Bottom = 272
               Right = 526
            End
            DisplayFlags = 280
            TopColumn = 51
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Portfolio_Security_Target_Baseview'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Portfolio_Security_Target_Baseview'
GO


