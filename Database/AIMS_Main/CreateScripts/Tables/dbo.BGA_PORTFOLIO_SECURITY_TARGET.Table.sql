/****** Object:  Table [dbo].[BGA_PORTFOLIO_SECURITY_TARGET]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BGA_PORTFOLIO_SECURITY_TARGET](
	[BGA_PORTFOLIO_ID] [varchar](20) NOT NULL,
	[SECURITY_ID] [varchar](20) NOT NULL,
	[TARGET] [decimal](32, 9) NOT NULL,
	[UPDATED] [datetime] NOT NULL,
 CONSTRAINT [PK_BGA_PORTFOLIO_SECURITY_TARGET] PRIMARY KEY CLUSTERED 
(
	[BGA_PORTFOLIO_ID] ASC,
	[SECURITY_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
