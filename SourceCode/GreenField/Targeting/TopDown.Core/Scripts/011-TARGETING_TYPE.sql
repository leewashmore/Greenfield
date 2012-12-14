﻿CREATE TABLE [dbo].[TARGETING_TYPE](
	[ID] [int] NOT NULL,
	[TARGETING_TYPE_GROUP_ID] [int] NOT NULL,
	[TAXONOMY_ID][int] NOT NULL,
	[NAME] [varchar](200) NOT NULL,
	[BENCHMARK_ID] [varchar](100) NULL
 CONSTRAINT [PK_TARGETING_TYPE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TARGETING_TYPE]  WITH CHECK ADD  CONSTRAINT [FK_TARGETING_TYPE_TARGETING_TYPE_GROUP] FOREIGN KEY([TARGETING_TYPE_GROUP_ID])
REFERENCES [dbo].[TARGETING_TYPE_GROUP] ([ID])
GO

ALTER TABLE [dbo].[TARGETING_TYPE] CHECK CONSTRAINT [FK_TARGETING_TYPE_TARGETING_TYPE_GROUP]
GO

ALTER TABLE [dbo].[TARGETING_TYPE]  WITH CHECK ADD  CONSTRAINT [FK_TARGETING_TYPE_TAXONOMY] FOREIGN KEY([TAXONOMY_ID])
REFERENCES [dbo].[TAXONOMY] ([ID])
GO

ALTER TABLE [dbo].[TARGETING_TYPE] CHECK CONSTRAINT [FK_TARGETING_TYPE_TAXONOMY]
GO



INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (0, 0, 2, 'Targeting type A', 'MSCI EM NET')
INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (1, 0, 1, 'Targeting type B', 'MSCI EM IMI NET')
--INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (1,1,0,'Targeting type B','MSCI EM IMI NET')
--INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (2,1,0,'Targeting type C','MSCI EM LATAM SC NET')
--INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (3,2,2,'Targeting type D','MSCI EM IN SC NET')
--INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (4,1,0,'Targeting type E','MSCI EM IMI NET')
--INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (5,0,1,'Targeting type F','MSCI EM GROSS')
--INSERT INTO [TARGETING_TYPE]([ID],[TARGETING_TYPE_GROUP_ID],[TAXONOMY_ID],[NAME],[BENCHMARK_ID]) VALUES (6,1,0,'Targeting type G',null)

GO

