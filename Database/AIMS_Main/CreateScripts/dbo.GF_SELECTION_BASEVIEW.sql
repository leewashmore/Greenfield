
/****** Object:  Table [dbo].[GFQ_SELECTION_BASEVIEW]    Script Date: 11/27/2013 13:35:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GF_SELECTION_BASEVIEW]') AND type in (N'U'))
DROP TABLE [dbo].[GF_SELECTION_BASEVIEW]
GO


/****** Object:  Table [dbo].[GFQ_SELECTION_BASEVIEW]    Script Date: 11/27/2013 13:35:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[GF_SELECTION_BASEVIEW](
	[GF_ID] BIGINT NOT NULL,
	[SHORT_NAME] [nvarchar](30) NULL,
	[LONG_NAME] [nvarchar](50) NULL,
	[INSTRUMENT_ID] [varchar](40) NULL,
	[TYPE] [varchar](9) NULL,
	[SECURITY_TYPE] [nvarchar](10) NULL,
	[AIMS_COMMODITY_ID] [nvarchar](100) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[GF_SELECTION_BASEVIEW] ADD  CONSTRAINT [PK_GF_SELECTION_BASEVIEW] PRIMARY KEY CLUSTERED 
(
	[GF_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
