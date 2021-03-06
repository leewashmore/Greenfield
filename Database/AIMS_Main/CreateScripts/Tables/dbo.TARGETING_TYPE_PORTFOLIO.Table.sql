/****** Object:  Table [dbo].[TARGETING_TYPE_PORTFOLIO]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TARGETING_TYPE_PORTFOLIO](
	[TARGETING_TYPE_ID] [int] NOT NULL,
	[PORTFOLIO_ID] [varchar](20) NOT NULL,
 CONSTRAINT [PK_TARGETING_TYPE_PORTFOLIO] PRIMARY KEY CLUSTERED 
(
	[TARGETING_TYPE_ID] ASC,
	[PORTFOLIO_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[TARGETING_TYPE_PORTFOLIO]  WITH CHECK ADD  CONSTRAINT [FK_TARGETING_TYPE_PORTFOLIO_TARGETING_TYPE] FOREIGN KEY([TARGETING_TYPE_ID])
REFERENCES [dbo].[TARGETING_TYPE] ([ID])
GO
ALTER TABLE [dbo].[TARGETING_TYPE_PORTFOLIO] CHECK CONSTRAINT [FK_TARGETING_TYPE_PORTFOLIO_TARGETING_TYPE]
GO
