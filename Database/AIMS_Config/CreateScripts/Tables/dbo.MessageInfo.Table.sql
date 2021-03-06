/****** Object:  Table [dbo].[MessageInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MessageInfo](
	[EmailId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmailTo] [varchar](max) NULL,
	[EmailCc] [varchar](max) NULL,
	[EmailSubject] [varchar](255) NULL,
	[EmailMessageBody] [varchar](max) NULL,
	[EmailAttachment] [varchar](max) NULL,
	[EmailSent] [bit] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_MessageInfo] PRIMARY KEY CLUSTERED 
(
	[EmailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
