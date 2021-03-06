/****** Object:  Table [dbo].[INTERNAL_COA_CHANGES]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[INTERNAL_COA_CHANGES](
	[ISSUER_ID] [varchar](20) NOT NULL,
	[ROOT_SOURCE] [varchar](10) NOT NULL,
	[LOAD_ID] [bigint] NOT NULL,
	[CURRENCY] [char](3) NOT NULL,
	[COA] [char](8) NOT NULL,
	[PERIOD_YEAR] [int] NOT NULL,
	[PERIOD_END_DATE] [datetime] NOT NULL,
	[START_DATE] [datetime] NOT NULL,
	[END_DATE] [datetime] NULL,
	[AMOUNT] [decimal](32, 6) NOT NULL,
	[UNITS] [char](1) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [INTERNAL_COA_CHANGES_idx] ON [dbo].[INTERNAL_COA_CHANGES] 
(
	[ISSUER_ID] ASC,
	[LOAD_ID] ASC,
	[COA] ASC,
	[CURRENCY] ASC,
	[PERIOD_END_DATE] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
