CREATE TABLE [dbo].[PORTFOLIO](
	[ID] [varchar](20) NOT NULL,
	[NAME] [varchar](200) NOT NULL,
	[IS_BOTTOM_UP] [int] NOT NULL,
 CONSTRAINT [PK_PORTFOLIO] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_A', 'PORT_A', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_B', 'PORT_B', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_C', 'PORT_C', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_D', 'PORT_D', 0)
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_E', 'PORT_E')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_F', 'PORT_F')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_G', 'PORT_G')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_H', 'PORT_H')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_J', 'PORT_J')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_I', 'PORT_I')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_K', 'PORT_K')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_L', 'PORT_L')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_M', 'PORT_M')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_N', 'PORT_N')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_O', 'PORT_O')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_P', 'PORT_P')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_Q', 'PORT_Q')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_R', 'PORT_R')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_S', 'PORT_S')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_T', 'PORT_T')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_U', 'PORT_U')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_V', 'PORT_V')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_W', 'PORT_W')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_X', 'PORT_X')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_Y', 'PORT_Y')
--INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PORT_Z', 'PORT_Z')

INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('AFRICA', 'AFRICA', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('SAF', 'SAF', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('GSCF', 'GSCF', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('LSCF', 'LSCF', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('MIDEAST', 'MIDEAST', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('STARS', 'STARS', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('AP1F', 'AP1F', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('APG60', 'APG60', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('NOMPOOL', 'NOMPOOL', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('SICVESC', 'SICVESC', 1)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('SICVFEF', 'SICVFEF', 0)

INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('USESC', 'USESC', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('ABP', 'ABP', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('BIRCH', 'BIRCH', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('CONN', 'CONN', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('EMIF', 'EMIF', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('EMSF', 'EMSF', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('GRD7', 'GRD7', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('IOWA', 'IOWA', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('KODAK', 'KODAK', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('OPB', 'OPB', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('PRIT', 'PRIT', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('SICVEF', 'SICVEF', 0)
INSERT INTO [dbo].[PORTFOLIO] ([ID], [NAME]) values ('USEF', 'USEF', 0)
GO

ALTER TABLE [dbo].[PORTFOLIO] ADD  CONSTRAINT [DF_PORTFOLIO_IS_BOTTOM_UP]  DEFAULT ((0)) FOR [IS_BOTTOM_UP]
GO
