/****** Object:  Table [dbo].[CommentInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CommentInfo](
	[CommentID] [bigint] IDENTITY(1,1) NOT NULL,
	[PresentationID] [bigint] NULL,
	[FileID] [bigint] NULL,
	[Comment] [varchar](255) NULL,
	[CommentBy] [varchar](50) NULL,
	[CommentOn] [datetime] NULL,
 CONSTRAINT [PK_CommentInfo] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CommentInfo]  WITH CHECK ADD  CONSTRAINT [FK_CommentInfo_FileMaster] FOREIGN KEY([FileID])
REFERENCES [dbo].[FileMaster] ([FileID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommentInfo] CHECK CONSTRAINT [FK_CommentInfo_FileMaster]
GO
ALTER TABLE [dbo].[CommentInfo]  WITH CHECK ADD  CONSTRAINT [FK_CommentInfo_PresentationInfo] FOREIGN KEY([PresentationID])
REFERENCES [dbo].[PresentationInfo] ([PresentationID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommentInfo] CHECK CONSTRAINT [FK_CommentInfo_PresentationInfo]
GO
