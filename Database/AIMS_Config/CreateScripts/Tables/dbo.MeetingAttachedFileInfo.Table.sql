/****** Object:  Table [dbo].[MeetingAttachedFileInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeetingAttachedFileInfo](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FileID] [bigint] NOT NULL,
	[MeetingID] [bigint] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_MeetingAttachedFileInfo_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[MeetingAttachedFileInfo]  WITH CHECK ADD  CONSTRAINT [FK_MeetingAttachedFileInfo_FileMaster] FOREIGN KEY([FileID])
REFERENCES [dbo].[FileMaster] ([FileID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MeetingAttachedFileInfo] CHECK CONSTRAINT [FK_MeetingAttachedFileInfo_FileMaster]
GO
ALTER TABLE [dbo].[MeetingAttachedFileInfo]  WITH CHECK ADD  CONSTRAINT [FK_MeetingAttachedFileInfo_MeetingInfo] FOREIGN KEY([MeetingID])
REFERENCES [dbo].[MeetingInfo] ([MeetingID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MeetingAttachedFileInfo] CHECK CONSTRAINT [FK_MeetingAttachedFileInfo_MeetingInfo]
GO
