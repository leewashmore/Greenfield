/****** Object:  Table [dbo].[tblDashboardPreference]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDashboardPreference](
	[PreferenceID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[GadgetViewClassName] [varchar](100) NOT NULL,
	[GadgetViewModelClassName] [varchar](100) NOT NULL,
	[GadgetName] [varchar](50) NOT NULL,
	[GadgetState] [varchar](50) NOT NULL,
	[GadgetPosition] [int] NOT NULL,
	[PreferenceGroupID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblDashboardPreference] PRIMARY KEY CLUSTERED 
(
	[PreferenceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
