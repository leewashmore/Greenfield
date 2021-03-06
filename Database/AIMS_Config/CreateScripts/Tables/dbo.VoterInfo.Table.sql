/****** Object:  Table [dbo].[VoterInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VoterInfo](
	[VoterID] [bigint] IDENTITY(1,1) NOT NULL,
	[PresentationID] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Notes] [varchar](255) NULL,
	[VoteType] [varchar](50) NULL,
	[AttendanceType] [varchar](50) NULL,
	[PostMeetingFlag] [bit] NULL,
	[DiscussionFlag] [bit] NULL,
	[VoterPFVMeasure] [varchar](50) NULL,
	[VoterBuyRange] [real] NULL,
	[VoterSellRange] [real] NULL,
	[VoterRecommendation] [varchar](50) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_tblICP_VoterInfo] PRIMARY KEY CLUSTERED 
(
	[VoterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[VoterInfo]  WITH CHECK ADD  CONSTRAINT [FK_VoterInfo_PresentationInfo] FOREIGN KEY([PresentationID])
REFERENCES [dbo].[PresentationInfo] ([PresentationID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[VoterInfo] CHECK CONSTRAINT [FK_VoterInfo_PresentationInfo]
GO
