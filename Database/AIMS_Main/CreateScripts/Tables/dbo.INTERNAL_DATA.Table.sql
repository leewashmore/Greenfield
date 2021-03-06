/****** Object:  Table [dbo].[INTERNAL_DATA]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[INTERNAL_DATA](
	[ISSUER_ID] [varchar](20) NOT NULL,
	[REF_NO] [varchar](50) NOT NULL,
	[PERIOD_TYPE] [char](2) NOT NULL,
	[COA] [char](8) NOT NULL,
	[AMOUNT] [decimal](32, 6) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [INTERNAL_DATA_idx] ON [dbo].[INTERNAL_DATA] 
(
	[REF_NO] ASC,
	[PERIOD_TYPE] ASC,
	[COA] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
