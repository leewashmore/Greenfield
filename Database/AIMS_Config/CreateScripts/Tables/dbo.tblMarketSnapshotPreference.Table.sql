/****** Object:  Table [dbo].[tblMarketSnapshotPreference]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMarketSnapshotPreference](
	[SnapshotPreferenceId] [int] IDENTITY(1,1) NOT NULL,
	[SnapshotName] [nvarchar](max) NOT NULL,
	[UserId] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_tblMarketSnapshotPreference] PRIMARY KEY CLUSTERED 
(
	[SnapshotPreferenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
