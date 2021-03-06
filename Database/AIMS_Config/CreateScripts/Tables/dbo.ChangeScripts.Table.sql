/****** Object:  Table [dbo].[ChangeScripts]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChangeScripts](
	[ScriptVersion] [nvarchar](100) NOT NULL,
	[DateExecuted] [datetime] NOT NULL,
 CONSTRAINT [PK_ChangeScripts] PRIMARY KEY CLUSTERED 
(
	[ScriptVersion] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
