﻿CREATE TABLE [dbo].[BGA_PORTFOLIO_SECURITY_TARGET](
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

ALTER TABLE [dbo].[BGA_PORTFOLIO_SECURITY_TARGET] ADD  CONSTRAINT [DF_BGA_PORTFOLIO_SECURITY_TARGET_UPDATED]  DEFAULT (getdate()) FOR [UPDATED]
GO