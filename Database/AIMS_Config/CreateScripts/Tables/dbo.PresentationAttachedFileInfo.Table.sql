/****** Object:  Table [dbo].[PresentationAttachedFileInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PresentationAttachedFileInfo](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FileID] [bigint] NOT NULL,
	[PresentationID] [bigint] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_PresentationAttachedFileInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[PresentationAttachedFileInfo]  WITH CHECK ADD  CONSTRAINT [FK_AttachedFileInfo_PresentationInfo] FOREIGN KEY([PresentationID])
REFERENCES [dbo].[PresentationInfo] ([PresentationID])
GO
ALTER TABLE [dbo].[PresentationAttachedFileInfo] CHECK CONSTRAINT [FK_AttachedFileInfo_PresentationInfo]
GO
ALTER TABLE [dbo].[PresentationAttachedFileInfo]  WITH CHECK ADD  CONSTRAINT [FK_PresentationAttachedFileInfo_FileMaster] FOREIGN KEY([FileID])
REFERENCES [dbo].[FileMaster] ([FileID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PresentationAttachedFileInfo] CHECK CONSTRAINT [FK_PresentationAttachedFileInfo_FileMaster]
GO
