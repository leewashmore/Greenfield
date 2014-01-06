
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc]') AND type in (N'U'))
DROP TABLE [dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc]
GO


CREATE TABLE [dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc](
	[ASEC_SEC_SHORT_NAME] [nvarchar](255) NULL,
	[XREF] [nvarchar](12) NULL,
	[REPORTNUMBER] [nvarchar](5) NULL,
) 

