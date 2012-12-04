CREATE TABLE [dbo].[COUNTRY_BASKET](
	[ID] [int] NOT NULL,
	[ISO_COUNTRY_CODE] [char](2) NOT NULL,
 CONSTRAINT [PK_COUNTRY_BASKET] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[COUNTRY_BASKET]  WITH CHECK ADD  CONSTRAINT [FK_COUNTRY_BASKET_BASKET] FOREIGN KEY([ID])
REFERENCES [dbo].[BASKET] ([ID])
GO

ALTER TABLE [dbo].[COUNTRY_BASKET] CHECK CONSTRAINT [FK_COUNTRY_BASKET_BASKET]
GO


CREATE UNIQUE NONCLUSTERED INDEX [IDX_COUNTRY_BASKET_ISO_COUNTRY_CODE] ON [dbo].[COUNTRY_BASKET]
(
	[ISO_COUNTRY_CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

insert into [BASKET] ([ID], [TYPE]) values (01, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (01, 'CN')
insert into [BASKET] ([ID], [TYPE]) values (02, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (02, 'HK')
insert into [BASKET] ([ID], [TYPE]) values (03, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (03, 'KR')
insert into [BASKET] ([ID], [TYPE]) values (04, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (04, 'TW')
insert into [BASKET] ([ID], [TYPE]) values (05, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (05, 'BR')
insert into [BASKET] ([ID], [TYPE]) values (06, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (06, 'MX')
insert into [BASKET] ([ID], [TYPE]) values (07, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (07, 'CL')
insert into [BASKET] ([ID], [TYPE]) values (08, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (08, 'PA')
insert into [BASKET] ([ID], [TYPE]) values (09, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (09, 'PE')
insert into [BASKET] ([ID], [TYPE]) values (10, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (10, 'CO')
insert into [BASKET] ([ID], [TYPE]) values (11, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (11, 'RU')
insert into [BASKET] ([ID], [TYPE]) values (12, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (12, 'TR')
insert into [BASKET] ([ID], [TYPE]) values (13, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (13, 'KZ')
insert into [BASKET] ([ID], [TYPE]) values (14, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (14, 'ZA')
insert into [BASKET] ([ID], [TYPE]) values (15, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (15, 'ID')
insert into [BASKET] ([ID], [TYPE]) values (16, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (16, 'TH')
insert into [BASKET] ([ID], [TYPE]) values (17, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (17, 'MY')
insert into [BASKET] ([ID], [TYPE]) values (18, 'country')
insert into [COUNTRY_BASKET] ([ID], [ISO_COUNTRY_CODE]) values (18, 'PH')
GO