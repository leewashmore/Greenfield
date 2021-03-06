/****** Object:  Table [dbo].[PresentationInfo]    Script Date: 03/08/2013 10:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PresentationInfo](
	[PresentationID] [bigint] IDENTITY(1,1) NOT NULL,
	[Presenter] [varchar](50) NOT NULL,
	[StatusType] [varchar](50) NOT NULL,
	[SecurityTicker] [varchar](50) NULL,
	[SecurityName] [varchar](50) NULL,
	[SecurityCountry] [varchar](50) NULL,
	[SecurityCountryCode] [varchar](50) NULL,
	[SecurityIndustry] [varchar](50) NULL,
	[SecurityCashPosition] [real] NULL,
	[SecurityPosition] [bigint] NULL,
	[SecurityMSCIStdWeight] [real] NULL,
	[SecurityMSCIIMIWeight] [real] NULL,
	[SecurityGlobalActiveWeight] [real] NULL,
	[SecurityLastClosingPrice] [real] NULL,
	[SecurityMarketCapitalization] [real] NULL,
	[SecurityPFVMeasure] [varchar](50) NULL,
	[SecurityBuyRange] [real] NULL,
	[SecuritySellRange] [real] NULL,
	[SecurityRecommendation] [varchar](50) NULL,
	[CommitteePFVMeasure] [varchar](255) NULL,
	[CommitteeBuyRange] [real] NULL,
	[CommitteeSellRange] [real] NULL,
	[CommitteeRecommendation] [varchar](50) NULL,
	[CommitteeRangeEffectiveThrough] [datetime] NULL,
	[AcceptWithoutDiscussionFlag] [bit] NULL,
	[AdminNotes] [varchar](255) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[Analyst] [varchar](50) NULL,
	[Price] [varchar](50) NULL,
	[FVCalc] [varchar](50) NULL,
	[SecurityBuySellvsCrnt] [varchar](50) NULL,
	[CurrentHoldings] [varchar](50) NULL,
	[PercentEMIF] [varchar](50) NULL,
	[SecurityBMWeight] [varchar](50) NULL,
	[SecurityActiveWeight] [varchar](50) NULL,
	[YTDRet_Absolute] [varchar](50) NULL,
	[YTDRet_RELtoLOC] [varchar](50) NULL,
	[YTDRet_RELtoEM] [varchar](50) NULL,
	[SecurityPFVMeasureValue] [decimal](18, 0) NULL,
	[CommitteePFVMeasureValue] [decimal](18, 0) NULL,
	[PortfolioId] [varchar](50) NULL,
 CONSTRAINT [PK_tblICP_PresentationInfo] PRIMARY KEY CLUSTERED 
(
	[PresentationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
