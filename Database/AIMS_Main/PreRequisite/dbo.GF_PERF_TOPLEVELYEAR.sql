USE [AIMS_Main_Dev]
GO

/****** Object:  Table [dbo].[GFQ_PERF_TOPLEVELYEAR]    Script Date: 11/27/2013 12:20:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GF_PERF_TOPLEVELYEAR]') AND type in (N'U'))
DROP TABLE [dbo].[GF_PERF_TOPLEVELYEAR]
GO

USE [AIMS_Main_Dev]
GO

/****** Object:  Table [dbo].[GFQ_PERF_TOPLEVELYEAR]    Script Date: 11/27/2013 12:21:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[GF_PERF_TOPLEVELYEAR](
	[GF_ID] BIGINT NOT NULL,
	[INDE] decimal(32,6) NULL,
	[TO_DATE] [varchar](50) NULL,
	[PORTFOLIOGROUP] [varchar](50) NULL,
	[PORTFOLIO] [varchar](50) NULL,
	[PORTFOLIOCODE] [varchar](50) NULL,
	[CURRENCY] [varchar](50) NULL,
	[RETURN_TYPE] [varchar](50) NULL,
	[POR_INCEPTION_DATE] [varchar](50) NULL,
	[RC_TWR_YTD] decimal(32,6) NULL,
	[BM1ID] [varchar](50) NULL,
	[BM1NAME] [varchar](250) NULL,
	[BM1_RC_TWR_YTD] decimal(32,6) NULL,
	[EXCESSRETURN1] decimal(32,6) NULL,
	[BM2ID] [varchar](50) NULL,
	[BM2NAME] [varchar](250) NULL,
	[BM2_RC_TWR_YTD] decimal(32,6) NULL,
	[EXCESSRETURN2] decimal(32,6) NULL,
	[BM3ID] [varchar](50) NULL,
	[BM3NAME] [varchar](250) NULL,
	[BM3_RC_TWR_YTD] decimal(32,6) NULL,
	[EXCESSRETURN3] decimal(32,6) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[GF_PERF_TOPLEVELYEAR] ADD  CONSTRAINT [PK_GF_PERF_TOPLEVELYEAR] PRIMARY KEY CLUSTERED 
(
	[GF_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


