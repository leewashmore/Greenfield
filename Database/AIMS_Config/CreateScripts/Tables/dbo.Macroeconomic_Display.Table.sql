/****** Object:  Table [dbo].[Macroeconomic_Display]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Macroeconomic_Display](
	[FIELD] [varchar](30) NOT NULL,
	[CATEGORY_NAME] [varchar](30) NOT NULL,
	[DESCRIPTION] [varchar](50) NOT NULL,
	[DATATYPE] [varchar](10) NOT NULL,
	[DECIMALS] [int] NULL,
	[SORT_ORDER] [int] NOT NULL,
	[HELP_TEXT] [varchar](255) NULL,
	[DISPLAY_TYPE] [varchar](30) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
