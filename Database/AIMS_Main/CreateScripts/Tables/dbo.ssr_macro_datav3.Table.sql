/****** Object:  Table [dbo].[ssr_macro_datav3]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ssr_macro_datav3](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[COUNTRY_CODE] [nvarchar](255) NULL,
	[FIELD] [nvarchar](255) NULL,
	[YEAR] [float] NULL,
	[VALUE] [float] NULL,
	[update_dt] [datetime] NULL,
	[update_source] [nvarchar](255) NULL
) ON [PRIMARY]
GO
