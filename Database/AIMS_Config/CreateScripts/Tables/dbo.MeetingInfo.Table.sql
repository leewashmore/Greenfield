/****** Object:  Table [dbo].[MeetingInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeetingInfo](
	[MeetingID] [bigint] IDENTITY(1,1) NOT NULL,
	[MeetingDateTime] [datetime] NOT NULL,
	[MeetingClosedDateTime] [datetime] NOT NULL,
	[MeetingVotingClosedDateTime] [datetime] NOT NULL,
	[MeetingDescription] [varchar](255) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_tblICP_MeetingInfo] PRIMARY KEY CLUSTERED 
(
	[MeetingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
