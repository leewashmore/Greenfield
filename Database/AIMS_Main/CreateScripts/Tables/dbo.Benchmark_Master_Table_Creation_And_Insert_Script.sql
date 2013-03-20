/****** Object:  Table [dbo].[BENCHMARK_MASTER]    Script Date: 03/15/2013 11:47:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BENCHMARK_MASTER]') AND type in (N'U'))
DROP TABLE [dbo].[BENCHMARK_MASTER]
GO
/****** Object:  Table [dbo].[BENCHMARK_MASTER]    Script Date: 03/15/2013 11:47:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[BENCHMARK_MASTER](
	[BENCHMARK_ID] [varchar](100) NOT NULL,
	[BENCHMARK_NAME] [varchar](100) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- Insert statements to populate the data in the benchmark master table. The list can be added later.

insert into dbo.BENCHMARK_MASTER values('SP AFRICA N CST',	'S&P PAN AFRICA EX SOUTH AFRICA NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI LA SC NET CST',	'MSCI EM LATAM SC NET');
insert into dbo.BENCHMARK_MASTER values('MSCI EM GROSS CST',	'MSCI EM GROSS');
insert into dbo.BENCHMARK_MASTER values('MSCI IMI MY TH CST',	'MSCI EM IMI Malay/Thai Custom Sector');
insert into dbo.BENCHMARK_MASTER values ('MSCI LA NET CST',	'MSCI EM LATAM NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI EM NET CST',	'MSCI EM NET');
insert into dbo.BENCHMARK_MASTER values ('SP MID EAST N CST',	'S&P AEMM MIDDLE EAST NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI IN SC NET CST',	'MSCI EM IN SC NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI FRONTIER N CST',	'MSCI FRONTIER NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI EM IMI NET CST',	'MSCI EM IMI NET');
insert into dbo.BENCHMARK_MASTER values ('GIC CUSTOM NET CST',	'GIC CUSTOM NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI IN NET CST',	'MSCI EM IN NET');
insert into dbo.BENCHMARK_MASTER values ('MSCI EM SC NET CST',	'MSCI EM SC NET');

