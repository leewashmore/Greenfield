/****** Object:  Table [dbo].[CURRENT_CONSENSUS_ESTIMATES]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CURRENT_CONSENSUS_ESTIMATES](
	[ISSUER_ID] [varchar](20) NOT NULL,
	[SECURITY_ID] [varchar](20) NOT NULL,
	[DATA_SOURCE] [varchar](10) NOT NULL,
	[DATA_SOURCE_DATE] [datetime] NOT NULL,
	[PERIOD_TYPE] [char](2) NOT NULL,
	[PERIOD_YEAR] [int] NOT NULL,
	[PERIOD_END_DATE] [datetime] NOT NULL,
	[FISCAL_TYPE] [char](8) NOT NULL,
	[ESTIMATE_ID] [int] NOT NULL,
	[CURRENCY] [char](3) NOT NULL,
	[AMOUNT] [decimal](32, 6) NOT NULL,
	[NUMBER_OF_ESTIMATES] [int] NOT NULL,
	[HIGH] [decimal](32, 6) NOT NULL,
	[LOW] [decimal](32, 6) NOT NULL,
	[SOURCE_CURRENCY] [char](3) NOT NULL,
	[STANDARD_DEVIATION] [decimal](32, 6) NOT NULL,
	[AMOUNT_TYPE] [char](10) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_idx2] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] 
(
	[ISSUER_ID] ASC,
	[SECURITY_ID] ASC,
	[DATA_SOURCE] ASC,
	[FISCAL_TYPE] ASC,
	[CURRENCY] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_idx3] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] 
(
	[ISSUER_ID] ASC,
	[ESTIMATE_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CURRENT_CONSENSUS_ESTIMATES_idx4] ON [dbo].[CURRENT_CONSENSUS_ESTIMATES] 
(
	[SECURITY_ID] ASC,
	[ESTIMATE_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
