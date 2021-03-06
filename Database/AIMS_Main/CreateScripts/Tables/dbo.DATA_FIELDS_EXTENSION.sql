
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DATA_FIELDS_EXTENSION]') AND type in (N'U'))
DROP TABLE [dbo].[DATA_FIELDS_EXTENSION]

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DATA_FIELDS_EXTENSION](
	[DATA_SOURCE] [varchar](10) NOT NULL,
	[PERIOD_TYPE] [char](2) NOT NULL,
	[PERIOD_YEAR] [int]  NULL,
	[FISCAL_TYPE] [char](8)  NULL,
	[CURRENCY] [char](3) NOT NULL,
	[DATA_ID] [int] NOT NULL,
	[NOTES] [varchar](255) NULL,
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


insert into [DATA_FIELDS_EXTENSION] values( 'PRIMARY','C',null,null,'USD',185,'Cap')
insert into [DATA_FIELDS_EXTENSION] values( 'PRIMARY',	'A',	-3,	'CALENDAR',	'USD',	290,	'Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-2,	'CALENDAR',	'USD',	290,	'Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	290,	'Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	290,	'Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	290,	'Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	'FISCAL',	'USD',	279,	'Forward Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	124,	'Dividends')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	124,	'Dividends')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	124,	'Dividends')

insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-2,	'CALENDAR',	'USD',	104,	'Book Value')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	104,	'Book Value')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	104,	'Book Value')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	104,	'Book Value')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	'FISCAL',	'USD',	280,	'Forward Book Value')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	166,	'P/E')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	166,	'P/E')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	166,	'P/E')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,null,		'USD',	187,	'Forward P/E')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,null,		'USD',	207,	'Trailing P/E')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	164,	'P/BV')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	164,	'P/BV')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	164,	'P/BV')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,null,		'USD',	188,	'Forward P/BV')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,null,		'USD',	209,	'Trailing P/BV')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	192,	'Dividend Yield')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	192,	'Dividend Yield')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	192,	'Dividend Yield')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,null,			'USD',	236,	'Forward Dividend Yield')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	133,	'ROE')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	133,	'ROE')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	133,	'ROE')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	'FISCAL','USD',	200,	'Forward ROE')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	'FISCAL','USD',	212,	'Trailing ROE')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	177,	'Net Income Growth')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	177,	'Net Income Growth')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	177,	'Net Income Growth')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	240,	'Net Income Growth, 2 Year (Annualized)')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	240,	'Net Income Growth, 2 Year (Annualized)')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	240,	'Net Income Growth, 2 Year (Annualized)')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	-1,	'CALENDAR',	'USD',	234,	'Net Income Growth, 2 Year (Annualized)')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	0,	'CALENDAR',	'USD',	234,	'Net Income Growth, 2 Year (Annualized)')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'A',	1,	'CALENDAR',	'USD',	234,	'Net Income Growth, 2 Year (Annualized)')

insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	NULL,	'USD',	296,	'Trailing Earnings')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	NULL,	'USD',	297,	'Trailing Dividends')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	NULL,	'USD',	300,	'Forward Dividends')
insert into [DATA_FIELDS_EXTENSION] values('PRIMARY',	'C',	NULL,	NULL,	'USD',	298,	'Trailing Book Value')
GO