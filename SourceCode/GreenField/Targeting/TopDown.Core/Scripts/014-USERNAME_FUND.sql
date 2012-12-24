﻿CREATE TABLE [dbo].[USERNAME_FUND](
	[USERNAME] [varchar](50) NOT NULL,
	[PORTFOLIO_ID] [varchar](20) NOT NULL,
 CONSTRAINT [PK_PORTFOLIO_ID] PRIMARY KEY CLUSTERED 
(
	[USERNAME] ASC,
	[PORTFOLIO_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'GSCF')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'AFRICA')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'MIDEAST')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'SAF')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'SICVESC')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'STARS')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('vfedonkin', 'LSCF')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'GSCF')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'AFRICA')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'MIDEAST')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'SAF')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'SICVESC')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'STARS')
INSERT INTO [dbo].[USERNAME_FUND] ([USERNAME], [PORTFOLIO_ID]) values ('bykova', 'LSCF')
