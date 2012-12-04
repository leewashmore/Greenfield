CREATE TABLE [dbo].[REGION_BASKET](
	[ID] [int] NOT NULL,
	[DEFINITION] [xml] NOT NULL,
 CONSTRAINT [PK_REGION_BASKET] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[REGION_BASKET]  WITH CHECK ADD  CONSTRAINT [FK_REGION_BASKET_BASKET] FOREIGN KEY([ID])
REFERENCES [dbo].[BASKET] ([ID])
GO

ALTER TABLE [dbo].[REGION_BASKET] CHECK CONSTRAINT [FK_REGION_BASKET_BASKET]
GO

insert into [BASKET] ([ID], [TYPE]) values (19, 'region')
insert into [REGION_BASKET] ([ID], [DEFINITION]) values (19, '<region-basket name="Central Europe" iso-codes="PL, HU, CZ" />')
insert into [BASKET] ([ID], [TYPE]) values (20, 'region')
insert into [REGION_BASKET] ([ID], [DEFINITION]) values (20, '<region-basket name="South Asia" iso-codes="IN, BD, LK" />')
insert into [BASKET] ([ID], [TYPE]) values (21, 'region')
insert into [REGION_BASKET] ([ID], [DEFINITION]) values (21, '<region-basket name="Africa" iso-codes="BW, CD, CI, EG, GH, KE, MU, MW, MA, NA, NG, SN, TZ, TN, ZM, ZW" />')
insert into [BASKET] ([ID], [TYPE]) values (22, 'region')
insert into [REGION_BASKET] ([ID], [DEFINITION]) values (22, '<region-basket name="Middle East" iso-codes="BH, JO, KW, LB, OM, PS, QA, SA, AE" />')
insert into [BASKET] ([ID], [TYPE]) values (23, 'region')
insert into [REGION_BASKET] ([ID], [DEFINITION]) values (23, '<region-basket name="China" iso-codes="HK, CH" />')
GO