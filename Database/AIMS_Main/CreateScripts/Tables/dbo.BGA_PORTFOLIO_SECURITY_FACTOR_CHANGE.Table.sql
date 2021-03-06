/****** Object:  Table [dbo].[BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE](
	[ID] [int] NOT NULL,
	[PORTFOLIO_ID] [varchar](20) NOT NULL,
	[SECURITY_ID] [varchar](20) NOT NULL,
	[FACTOR_BEFORE] [decimal](32, 6) NULL,
	[FACTOR_AFTER] [decimal](32, 6) NULL,
	[COMMENT] [ntext] NOT NULL,
	[CHANGESET_ID] [int] NOT NULL,
 CONSTRAINT [PK_BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE]  WITH CHECK ADD  CONSTRAINT [FK_BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE_BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET] FOREIGN KEY([CHANGESET_ID])
REFERENCES [dbo].[BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET] ([ID])
GO
ALTER TABLE [dbo].[BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE] CHECK CONSTRAINT [FK_BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE_BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET]
GO
