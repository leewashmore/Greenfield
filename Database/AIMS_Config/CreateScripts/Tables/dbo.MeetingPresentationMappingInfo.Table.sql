/****** Object:  Table [dbo].[MeetingPresentationMappingInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeetingPresentationMappingInfo](
	[MappingID] [bigint] IDENTITY(1,1) NOT NULL,
	[MeetingID] [bigint] NOT NULL,
	[PresentationID] [bigint] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_tblICP_MeetingPresentationMappingInfo] PRIMARY KEY CLUSTERED 
(
	[MappingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[MeetingPresentationMappingInfo]  WITH CHECK ADD  CONSTRAINT [FK_MeetingPresentationMappingInfo_MeetingInfo] FOREIGN KEY([MeetingID])
REFERENCES [dbo].[MeetingInfo] ([MeetingID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MeetingPresentationMappingInfo] CHECK CONSTRAINT [FK_MeetingPresentationMappingInfo_MeetingInfo]
GO
ALTER TABLE [dbo].[MeetingPresentationMappingInfo]  WITH CHECK ADD  CONSTRAINT [FK_MeetingPresentationMappingInfo_PresentationInfo] FOREIGN KEY([PresentationID])
REFERENCES [dbo].[PresentationInfo] ([PresentationID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MeetingPresentationMappingInfo] CHECK CONSTRAINT [FK_MeetingPresentationMappingInfo_PresentationInfo]
GO
