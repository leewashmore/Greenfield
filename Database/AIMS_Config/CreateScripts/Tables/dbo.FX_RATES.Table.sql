/****** Object:  Table [dbo].[FX_RATES]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FX_RATES](
	[CURRENCY] [char](3) NOT NULL,
	[FX_DATE] [datetime] NOT NULL,
	[FX_RATE] [decimal](32, 6) NOT NULL,
	[AVG90DAYRATE] [decimal](32, 6) NULL,
	[AVG12MonthRATE] [decimal](32, 6) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
