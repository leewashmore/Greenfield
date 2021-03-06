/****** Object:  Table [dbo].[tblMarketSnapshotEntityPreference]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMarketSnapshotEntityPreference](
	[EntityPreferenceId] [int] IDENTITY(1,1) NOT NULL,
	[GroupPreferenceId] [int] NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[EntityReturnType] [nvarchar](50) NULL,
	[EntityOrder] [int] NOT NULL,
	[EntityType] [nvarchar](50) NOT NULL,
	[EntityId] [nvarchar](50) NOT NULL,
	[EntityNodeType] [nvarchar](50) NULL,
	[EntityNodeValueCode] [nvarchar](50) NULL,
	[EntityNodeValueName] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblMarketSnapshotEntityPreference] PRIMARY KEY CLUSTERED 
(
	[EntityPreferenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblMarketSnapshotEntityPreference]  WITH CHECK ADD  CONSTRAINT [FK_tblMarketSnapshotEntityPreference_tblMarketSnapshotGroupPreference] FOREIGN KEY([GroupPreferenceId])
REFERENCES [dbo].[tblMarketSnapshotGroupPreference] ([GroupPreferenceId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblMarketSnapshotEntityPreference] CHECK CONSTRAINT [FK_tblMarketSnapshotEntityPreference_tblMarketSnapshotGroupPreference]
GO
