IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc]') AND type in (N'U'))
DROP TABLE [dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc](
	[ID] [int] NOT NULL,
	[ASEC_SEC_SHORT_NAME] [nvarchar](255) NULL,
	[XREF] [nvarchar](12) NULL,
	[REPORTNUMBER] [nvarchar](5) NULL,
) 
SET ANSI_PADDING ON

/****** Object:  Index [PK_GF_SECURITY_BASEVIEW]    Script Date: 01/06/2014 12:43:00 ******/
ALTER TABLE [dbo].[GF_SECURITY_BASEVIEW_Reuters_Adhoc] ADD  CONSTRAINT [PK_GF_SECURITY_BASEVIEW_Reuters_Adhoc] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

