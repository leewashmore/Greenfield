/****** Object:  Table [dbo].[STATEMENT_CONSENSUS_MAPPING]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[STATEMENT_CONSENSUS_MAPPING](
	[STATEMENT_TYPE] [char](3) NOT NULL,
	[ESTIMATE_ID] [int] NOT NULL,
	[DESCRIPTION] [varchar](50) NOT NULL,
	[SORT_ORDER] [int] NOT NULL,
	[EARNINGS] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
