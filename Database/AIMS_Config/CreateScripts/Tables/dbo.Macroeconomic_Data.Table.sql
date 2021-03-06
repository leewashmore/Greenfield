/****** Object:  Table [dbo].[Macroeconomic_Data]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Macroeconomic_Data](
	[COUNTRY_CODE] [char](4) NOT NULL,
	[FIELD] [varchar](30) NOT NULL,
	[YEAR1] [int] NOT NULL,
	[VALUE] [decimal](32, 6) NULL,
	[UPDATE_DATE] [datetime] NOT NULL,
	[UPDATE_SOURCE] [varchar](20) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
