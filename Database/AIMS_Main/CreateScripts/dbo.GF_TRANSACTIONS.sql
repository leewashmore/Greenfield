
/****** Object:  Table [dbo].[GF_TRANSACTIONS]    Script Date: 11/27/2013 11:51:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GF_TRANSACTIONS]') AND type in (N'U'))
DROP TABLE [dbo].[GF_TRANSACTIONS]
GO



/****** Object:  Table [dbo].[GF_TRANSACTIONS]    Script Date: 11/27/2013 11:51:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GF_TRANSACTIONS](
	[GF_ID] BIGINT NOT NULL,
	[A_TRANSACTIONS_TRANSACTION] [decimal](14, 0) NULL,
	[ASEC_SEC_SHORT_NAME] [nvarchar](16) NULL,
	[CUSTODIAN] [nvarchar](16) NULL,
	[FX_RATE_QC_PC] [decimal](16, 8) NULL,
	[FX_RATE_QC_SC] [decimal](16, 8) NULL,
	[ISIN] [nvarchar](12) NULL,
	[LEG_NO] [int] NULL,
	[NOMINAL] [decimal](21, 7) NULL,
	[PAYMENT_VALUE_PC] [decimal](32,6) NULL,
	[PAYMENT_VALUE_QC] [decimal](32,6) NULL,
	[PAYMENT_VALUE_SC] [decimal](32,6) NULL,
	[PORTFOLIO_ID] [nvarchar](10) NULL,
	[PORTFOLIO_NAME] [nvarchar](50) NULL,
	[PORTFOLIO_THEME_SUBGROUP_CODE] [nvarchar](50) NULL,
	[PRICE] [decimal](17, 9) NULL,
	[SEC_NAME] [nvarchar](50) NULL,
	[SEC_TYPE] [nvarchar](10) NULL,
	[SETTLEMENT_CURRENCY] [nvarchar](3) NULL,
	[SETTLEMENT_DATE] [datetime] NULL,
	[TRADE_DATE] [datetime] NULL,
	[TRANSACTION_CODE] [nvarchar](30) NULL,
	[VALUE_PC] [decimal](32,6) NULL,
	[VALUE_QC] [decimal](32,6) NULL,
	[VALUE_SC] [decimal](32,6) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GF_TRANSACTIONS] ADD  CONSTRAINT [PK_GF_TRANSACTIONS] PRIMARY KEY CLUSTERED 
(
	[GF_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

