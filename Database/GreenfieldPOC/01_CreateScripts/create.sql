
CREATE DATABASE [GreenfieldPOC] ON  PRIMARY 
( NAME = N'GreenfieldPOC', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS2\MSSQL\DATA\GreenfieldPOC.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'GreenfieldPOC_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS2\MSSQL\DATA\GreenfieldPOC_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO





USE [GreenfieldPOC]
GO
/****** Object:  Table [dbo].[UserDashboardPreferance]    Script Date: 03/22/2012 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserDashboardPreferance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[UserPref] [nvarchar](max) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStdIssue]    Script Date: 03/22/2012 15:17:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStdIssue](
	[ReportNumber] [varchar](5) NOT NULL,
	[IssueID] [varchar](10) NOT NULL,
	[IssueType] [char](1) NULL,
	[IssueDesc] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[Ticker] [varchar](15) NULL,
	[CUSIP] [varchar](9) NULL,
	[MXSecurityID] [varchar](12) NULL,
	[Valoren] [varchar](12) NULL,
	[ISIN] [varchar](12) NULL,
	[RIC] [varchar](20) NULL,
	[DisplayRIC] [varchar](20) NULL,
	[SEDOL] [varchar](7) NULL,
	[Active] [bit] NULL,
	[ListingType] [varchar](10) NULL,
	[ExchangeCode] [varchar](50) NULL,
	[Country] [char](3) NULL,
	[Region] [char](2) NULL,
	[MostRecentSplit] [real] NULL,
	[MostRecentSplitDate] [datetime] NULL,
	[ParCurrency] [char](3) NULL,
	[ParValue] [real] NULL,
	[SharesAuthorized] [real] NULL,
	[SharesOut] [real] NULL,
	[FloatingShares] [real] NULL,
	[Votes] [real] NULL,
	[ConversionFactor] [real] NULL,
 CONSTRAINT [PK_tblStdAnnIssue] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[IssueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStdInterimRef]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStdInterimRef](
	[ReportNumber] [varchar](5) NOT NULL,
	[RefNo] [varchar](10) NOT NULL,
	[FiscalYear] [int] NULL,
	[InterimNumber] [smallint] NULL,
	[UpdateType] [varchar](10) NULL,
	[UpdateDate] [datetime] NULL,
	[CompleteStatement] [bit] NULL,
	[PeriodLengthCode] [char](1) NULL,
	[PeriodLength] [int] NULL,
	[PeriodEndDate] [datetime] NULL,
	[Source] [varchar](50) NULL,
	[SourceDate] [datetime] NULL,
	[ReportedAccountingStandardCode] [varchar](20) NULL,
	[ReportedAccountingStandard] [varchar](100) NULL,
	[STECFlag] [bit] NULL,
	[CurrencyConvertedTo] [char](3) NULL,
	[CurrencyReported] [char](3) NULL,
	[RepToConvExRate] [real] NULL,
	[UnitConvertedTo] [char](3) NULL,
	[UnitReported] [char](3) NULL,
	[SystemDate] [datetime] NULL,
 CONSTRAINT [PK_tblStdInterimRef] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[RefNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStdInterimIssue]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStdInterimIssue](
	[ReportNumber] [varchar](5) NOT NULL,
	[IssueID] [varchar](10) NOT NULL,
	[IssueType] [char](1) NULL,
	[IssueDesc] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[Ticker] [varchar](15) NULL,
	[CUSIP] [varchar](9) NULL,
	[MXSecurityID] [varchar](12) NULL,
	[Valoren] [varchar](12) NULL,
	[ISIN] [varchar](12) NULL,
	[RIC] [varchar](20) NULL,
	[DisplayRIC] [varchar](20) NULL,
	[SEDOL] [varchar](7) NULL,
	[Active] [bit] NULL,
	[ListingType] [varchar](10) NULL,
	[ExchangeCode] [varchar](50) NULL,
	[Country] [char](3) NULL,
	[Region] [char](2) NULL,
	[MostRecentSplit] [real] NULL,
	[MostRecentSplitDate] [datetime] NULL,
	[ParCurrency] [char](3) NULL,
	[ParValue] [real] NULL,
	[SharesAuthorized] [real] NULL,
	[SharesOut] [real] NULL,
	[FloatingShares] [real] NULL,
	[Votes] [real] NULL,
	[ConversionFactor] [real] NULL,
 CONSTRAINT [PK_tblStdInterimIssue] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[IssueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStdInterimCompanyInfo]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStdInterimCompanyInfo](
	[ReportNumber] [varchar](5) NOT NULL,
	[Name] [varchar](50) NULL,
	[Active] [bit] NULL,
	[CoType] [char](3) NULL,
	[IRSNo] [char](10) NULL,
	[CIKNo] [char](10) NULL,
	[LatestFinancialsAnnual] [datetime] NULL,
	[LatestFinancialsInterim] [datetime] NULL,
	[Currency] [char](3) NULL,
	[CurrentExchangeRate] [real] NULL,
	[CurrentExchangeRateDate] [datetime] NULL,
	[AuditorCode] [varchar](10) NULL,
	[AuditorName] [varchar](100) NULL,
	[COAType] [char](3) NULL,
	[BalanceSheetDisplayCode] [char](3) NULL,
	[CashflowMethod] [char](3) NULL,
 CONSTRAINT [PK_tblStdInterimCompanyInfo] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStdInterim]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStdInterim](
	[ReportNumber] [varchar](5) NOT NULL,
	[ReportYear] [int] NOT NULL,
	[COA] [varchar](10) NOT NULL,
	[RefNo] [varchar](10) NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_tblStdInterim] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[ReportYear] ASC,
	[COA] ASC,
	[RefNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStdCompanyInfo]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStdCompanyInfo](
	[ReportNumber] [varchar](5) NOT NULL,
	[Name] [varchar](50) NULL,
	[Active] [bit] NULL,
	[CoType] [char](3) NULL,
	[IRSNo] [char](10) NULL,
	[CIKNo] [char](10) NULL,
	[LatestFinancialsAnnual] [datetime] NULL,
	[LatestFinancialsInterim] [datetime] NULL,
	[Currency] [char](3) NULL,
	[CurrentExchangeRate] [real] NULL,
	[CurrentExchangeRateDate] [datetime] NULL,
	[AuditorCode] [varchar](10) NULL,
	[AuditorName] [varchar](100) NULL,
	[COAType] [char](3) NULL,
	[BalanceSheetDisplayCode] [char](3) NULL,
	[CashflowMethod] [char](3) NULL,
 CONSTRAINT [PK_tblStdCompanyInfo] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStd]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStd](
	[ReportNumber] [varchar](5) NOT NULL,
	[ReportYear] [int] NOT NULL,
	[COA] [varchar](10) NOT NULL,
	[RefNo] [varchar](10) NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_tblStd] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[ReportYear] ASC,
	[COA] ASC,
	[RefNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblStatementRef]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblStatementRef](
	[ReportNumber] [varchar](5) NOT NULL,
	[RefNo] [varchar](10) NOT NULL,
	[UpdateType] [varchar](10) NULL,
	[UpdateDate] [datetime] NULL,
	[CompleteStatement] [bit] NULL,
	[PeriodLengthCode] [char](1) NULL,
	[PeriodLength] [int] NULL,
	[PeriodEndDate] [datetime] NULL,
	[Source] [varchar](50) NULL,
	[SourceDate] [datetime] NULL,
	[ReportedAccountingStandardCode] [varchar](20) NULL,
	[ReportedAccountingStandard] [varchar](100) NULL,
	[STECFlag] [bit] NULL,
	[AuditorCode] [varchar](50) NULL,
	[AuditOpinion] [varchar](10) NULL,
	[CurrencyConvertedTo] [char](3) NULL,
	[CurrencyReported] [char](3) NULL,
	[RepToConvExRate] [real] NULL,
	[UnitConvertedTo] [char](3) NULL,
	[UnitReported] [char](3) NULL,
	[SystemDate] [datetime] NULL,
 CONSTRAINT [PK_tblStatementRef] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[RefNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDetailedRecommendation]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDetailedRecommendation](
	[XRef] [varchar](9) NOT NULL,
	[BrokerID] [varchar](12) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[OriginalDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[STDOpinionCode] [varchar](15) NULL,
	[STDOpinion] [varchar](50) NULL,
	[BrokerOpinionCode] [varchar](15) NULL,
	[BrokerOpinion] [varchar](50) NULL,
	[ConfirmationDate] [datetime] NULL,
	[SuppressStartDate] [datetime] NULL,
	[SuppressEndDate] [datetime] NULL,
	[SuppressOriginalDate] [datetime] NULL,
	[SuppressExpirationDate] [datetime] NULL,
 CONSTRAINT [PK_tblDetailedRecommendation] PRIMARY KEY CLUSTERED 
(
	[XRef] ASC,
	[BrokerID] ASC,
	[StartDate] ASC,
	[OriginalDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDetailedEstimate]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDetailedEstimate](
	[XRef] [varchar](9) NOT NULL,
	[fYearEnd] [char](6) NOT NULL,
	[fPeriodEnd] [char](6) NULL,
	[PeriodEndDate] [datetime] NOT NULL,
	[BrokerID] [varchar](10) NOT NULL,
	[PeriodType] [varchar](2) NOT NULL,
	[EstimateType] [varchar](20) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[OriginalDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[ConfirmationDate] [datetime] NULL,
	[SuppressStartDate] [datetime] NULL,
	[SuppressEndDate] [datetime] NULL,
	[SuppressOriginalDate] [datetime] NULL,
	[SuppressExpirationDate] [datetime] NULL,
	[Unit] [varchar](2) NULL,
	[Amount] [real] NULL,
	[Currency] [char](3) NULL,
 CONSTRAINT [PK_tblDetailedEstimate] PRIMARY KEY CLUSTERED 
(
	[XRef] ASC,
	[fYearEnd] ASC,
	[PeriodEndDate] ASC,
	[BrokerID] ASC,
	[PeriodType] ASC,
	[EstimateType] ASC,
	[StartDate] ASC,
	[OriginalDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDashboardPreference]    Script Date: 03/22/2012 15:17:05 ******/
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
/****** Object:  Table [dbo].[tblConsensusRecommendation]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblConsensusRecommendation](
	[XRef] [varchar](9) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[OriginalDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[MeanRating] [real] NULL,
	[MeanLabel] [varchar](20) NULL,
 CONSTRAINT [PK_tblConsensusRecommendation] PRIMARY KEY CLUSTERED 
(
	[XRef] ASC,
	[StartDate] ASC,
	[OriginalDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblConsensusEstimate]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblConsensusEstimate](
	[XRef] [varchar](9) NOT NULL,
	[PeriodEndDate] [smalldatetime] NOT NULL,
	[fYearEnd] [char](6) NOT NULL,
	[fPeriodEnd] [char](6) NULL,
	[EstimateType] [varchar](20) NOT NULL,
	[PeriodType] [varchar](2) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[OriginalDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[Unit] [varchar](2) NULL,
	[NumOfEsts] [int] NULL,
	[High] [real] NULL,
	[Low] [real] NULL,
	[Mean] [real] NULL,
	[StdDev] [real] NULL,
	[Median] [real] NULL,
	[Currency] [char](3) NULL,
 CONSTRAINT [PK_tblConsensusEstimate] PRIMARY KEY CLUSTERED 
(
	[XRef] ASC,
	[PeriodEndDate] ASC,
	[fYearEnd] ASC,
	[EstimateType] ASC,
	[PeriodType] ASC,
	[StartDate] ASC,
	[OriginalDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCompanyURL]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCompanyURL](
	[ReportNumber] [varchar](5) NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[URL] [varchar](225) NULL,
	[LastModified] [datetime] NULL,
 CONSTRAINT [PK_tblCompanyURL] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[Category] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCompanySummary]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCompanySummary](
	[ReportNumber] [varchar](5) NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[Summary] [varchar](2000) NULL,
	[LastModified] [datetime] NULL,
	[FilingDate] [datetime] NULL,
	[FilingType] [varchar](20) NULL,
 CONSTRAINT [PK_tblCompanySummary] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC,
	[Category] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCompanyIssue]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCompanyIssue](
	[ReportNumber] [varchar](5) NOT NULL,
	[IssueOrder] [smallint] NOT NULL,
	[IssueType] [char](1) NULL,
	[IssueDesc] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[Ticker] [varchar](15) NULL,
	[CUSIP] [varchar](9) NULL,
	[MXSecurityID] [varchar](12) NULL,
	[Valoren] [varchar](12) NULL,
	[ISIN] [varchar](12) NULL,
	[RIC] [varchar](20) NULL,
	[SEDOL] [varchar](7) NULL,
	[ExchangeCode] [char](10) NULL,
	[Country] [char](3) NULL,
	[MostRecentSplit] [real] NULL,
	[MostRecentSplitDate] [datetime] NULL,
	[GlobalListingType] [varchar](5) NULL,
	[SharesPerListing] [real] NULL,
	[DisplayRIC] [nchar](20) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCompanyInfo]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCompanyInfo](
	[XRef] [varchar](9) NOT NULL,
	[ReportNumber] [varchar](5) NULL,
	[Name] [varchar](80) NULL,
	[Country] [char](3) NULL,
	[Ticker] [varchar](15) NULL,
	[CUSIP] [varchar](12) NULL,
	[ISIN] [varchar](12) NULL,
	[SEDOL] [varchar](12) NULL,
	[VALOREN] [varchar](12) NULL,
	[RIC] [varchar](21) NULL,
	[DisplayRIC] [varchar](21) NULL,
	[RecentSplitDate] [datetime] NULL,
	[RecentCapitalAdjDate] [datetime] NULL,
	[CodeSector] [varchar](20) NULL,
	[CodeIndustry] [varchar](20) NULL,
	[CodePrimary] [varchar](20) NULL,
	[RBSSEconomicSector] [varchar](10) NULL,
	[RBSSBusinessSector] [varchar](10) NULL,
	[RBSSIndustryGroup] [varchar](10) NULL,
	[RBSSIndustry] [varchar](10) NULL,
	[Consensus] [varchar](20) NULL,
	[Estimate] [varchar](10) NULL,
	[Earnings] [varchar](10) NULL,
	[Periodicity] [varchar](2) NULL,
	[GeneralNote] [varchar](1200) NULL,
	[CurrentPeriod] [char](2) NULL,
	[pEndDate] [smalldatetime] NULL,
	[fYearEnd] [char](6) NULL,
	[fPeriodEnd] [char](6) NULL,
	[CurrentPrice] [real] NULL,
	[CurrentPriceDate] [smalldatetime] NULL,
	[Currency] [char](3) NULL,
	[OutstandingShares] [decimal](18, 0) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_tblCompanyInfo] PRIMARY KEY CLUSTERED 
(
	[XRef] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCompany]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCompany](
	[ReportNumber] [varchar](5) NOT NULL,
	[Addr1] [varchar](100) NULL,
	[Addr2] [varchar](100) NULL,
	[Addr3] [varchar](100) NULL,
	[City] [varchar](60) NULL,
	[State] [varchar](100) NULL,
	[Zip] [varchar](50) NULL,
	[ContactName] [varchar](100) NULL,
	[ContactTitle] [varchar](100) NULL,
	[Email] [varchar](250) NULL,
	[MainPhone] [varchar](25) NULL,
	[Fax] [varchar](25) NULL,
	[ContactPhone] [varchar](25) NULL,
	[PublicSince] [varchar](20) NULL,
	[Employees] [int] NULL,
	[EmployeeReportDate] [datetime] NULL,
	[SharesOut] [real] NULL,
	[FloatingShares] [real] NULL,
	[ShareReportDate] [datetime] NULL,
	[CommonShareholders] [int] NULL,
	[ShareholdersReportDate] [datetime] NULL,
	[IncorporatedIn] [varchar](20) NULL,
	[Currency] [varchar](3) NULL,
	[CoStatus] [bit] NULL,
	[CoType] [varchar](3) NULL,
	[MostRecentExchange] [real] NULL,
	[MostRecentExchangeDate] [datetime] NULL,
	[LatestAvailableAnnual] [datetime] NULL,
	[LatestAvailableInterim] [datetime] NULL,
 CONSTRAINT [PK_tblCompany] PRIMARY KEY CLUSTERED 
(
	[ReportNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblActual]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblActual](
	[XRef] [varchar](9) NOT NULL,
	[PeriodType] [varchar](2) NOT NULL,
	[EstimateType] [varchar](20) NOT NULL,
	[fYearEnd] [char](6) NOT NULL,
	[fPeriodEnd] [char](6) NOT NULL,
	[PeriodEndDate] [smalldatetime] NOT NULL,
	[Unit] [varchar](2) NULL,
	[ActualValue] [real] NULL,
	[AnnouncementDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
	[Currency] [varchar](3) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AGG_TABLE]    Script Date: 03/22/2012 15:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AGG_TABLE](
	[SEC_SECSHORT] [varchar](50) NULL,
	[ISIN] [varchar](50) NULL,
	[NAME] [varchar](50) NULL,
	[XREF] [varchar](9) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[A_PROC]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  procedure [dbo].[A_PROC]
 (@SEC_SECSHORT_List VARCHAR(500))
AS DECLARE @Index INT 
   DECLARE @RecordCnt INT
   DECLARE  @TEMPORARY_TAB TABLE( 
								iSNo INT IDENTITY(1,1), 
								XREF VARCHAR(9), SEC_SECSHORT VARCHAR(30) )
   					   								    
							    			    
BEGIN
       
SET NOCOUNT OFF;
SET FMTONLY OFF;

CREATE TABLE #TempList
	(	iSNo INT IDENTITY(1,1),     
		SEC_SECSHORT varchar(50)
	)
	CREATE TABLE #TEMPORARY_MAIN
	(	    
		XREF VARCHAR(9),
		YEAR2009 REAL,
		YEAR2010 REAL,
		YEAR2011 REAL,
		YEAR2012 REAL,
		YEAR2013 REAL,
		SEC_SECSHORT varchar(50)
	)
DECLARE @SEC_SECSHORT varchar(50), @Pos int

SET @SEC_SECSHORT_List = LTRIM(RTRIM(@SEC_SECSHORT_List))+ ','
	SET @Pos = CHARINDEX(',', @SEC_SECSHORT_List, 1)

	IF REPLACE(@SEC_SECSHORT_List, ',', '') <>''
	BEGIN
		WHILE @Pos > 0
		BEGIN
		
			SET @SEC_SECSHORT = LTRIM(RTRIM(LEFT(@SEC_SECSHORT_List, @Pos - 1)))
			IF @SEC_SECSHORT <> ''
			BEGIN
			
				INSERT INTO #TempList (SEC_SECSHORT) VALUES (CAST(@SEC_SECSHORT AS varchar(50))) 
			
			END
			SET @SEC_SECSHORT_List = RIGHT(@SEC_SECSHORT_List, LEN(@SEC_SECSHORT_List) - @Pos)
			SET @Pos = CHARINDEX(',', @SEC_SECSHORT_List, 1)

		END
	    END	
             
              INSERT INTO @TEMPORARY_TAB(XREF,SEC_SECSHORT) SELECT A.XREF,T.SEC_SECSHORT FROM  AGG_TABLE AS A JOIN #TempList T ON A.SEC_SECSHORT=T.SEC_SECSHORT
              SELECT @Index = 1
              SELECT @RecordCnt = COUNT(iSNo) FROM @TEMPORARY_TAB
              
              WHILE @Index <= @RecordCnt 
              BEGIN 
            
                        
             INSERT INTO #TEMPORARY_MAIN (XREF)
             SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index
             
			 UPDATE  #TEMPORARY_MAIN
			 SET YEAR2011 =
		     (SELECT TOP 1 Median  FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)
             AND fYearEnd = '201112' AND ExpirationDate = (SELECT MAX(ExpirationDate) FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND (XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)) AND fYearEnd = '201112')),
             
             YEAR2012 = 
             (SELECT TOP 1 Median FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index) 
             AND (fYearEnd = '201212') AND ExpirationDate = (SELECT MAX(ExpirationDate) FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND (XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)) AND fYearEnd = '201212')),
              
             YEAR2013 = 
             (SELECT TOP 1 Median FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)
             AND (fYearEnd = '201312') AND ExpirationDate = (SELECT MAX(ExpirationDate) FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND (XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)) AND fYearEnd = '201312')),
			 		 
           
             YEAR2009 = 
             (SELECT TOP 1 ActualValue  FROM tblActual
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index) AND fYearEnd = '200912' ),
			
			YEAR2010 = (SELECT TOP 1 ActualValue FROM tblActual
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index) AND fYearEnd = '201012' ),
             
             SEC_SECSHORT = (SELECT SEC_SECSHORT FROM @TEMPORARY_TAB WHERE iSNo=@Index) 
             
             WHERE
             XREF = (SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)
             
            
             SET @Index=@Index+1
             END 
             
            SELECT * FROM #TEMPORARY_MAIN
        END
GO
/****** Object:  StoredProcedure [dbo].[StoreDashboardInfo]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[StoreDashboardInfo] 

      -- Add the parameters for the stored procedure here

      @UserId varchar(100) , 

      @StoredValue varchar(max)  

AS

BEGIN

      -- SET NOCOUNT ON added to prevent extra result sets from

      -- interfering with SELECT statements.

      SET NOCOUNT ON;

 

    -- Insert statements for procedure here

      if exists(select * from UserDashboardPreferance where UserID LIKE @UserId )

      Begin

      Update UserDashboardPreferance set UserPref=@StoredValue where UserID LIKE @UserId

      End

      Else

      Insert into UserDashboardPreferance values(@UserId,@StoredValue)

END
GO
/****** Object:  StoredProcedure [dbo].[SetDashBoardPreference]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetDashBoardPreference] 
	-- Add the parameters for the stored procedure here
	@UserName VARCHAR(50), 
	@GadgetViewClassName VARCHAR(100),
	@GadgetViewModelClassName VARCHAR(100),
	@GadgetName VARCHAR(50),
	@GadgetState VARCHAR(50),
	@PreferenceGroupID VARCHAR(50),
	@GadgetPosition INT
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM tblDashboardPreference WHERE UserName LIKE @UserName AND PreferenceGroupID NOT LIKE @PreferenceGroupID)
		BEGIN
			DELETE FROM tblDashboardPreference WHERE UserName LIKE @UserName
		END
	
	BEGIN
		INSERT INTO tblDashboardPreference 
		( UserName, GadgetViewClassName, GadgetViewModelClassName,  GadgetName, GadgetState, GadgetPosition, PreferenceGroupID )
		VALUES 
		( @UserName, @GadgetViewClassName, @GadgetViewModelClassName, @GadgetName, @GadgetState, @GadgetPosition, @PreferenceGroupID )			
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetDashBoardPreferenceByUserName]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDashBoardPreferenceByUserName] 
	-- Add the parameters for the stored procedure here
	@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM tblDashboardPreference WHERE UserName = @UserName
END
GO
/****** Object:  StoredProcedure [dbo].[GetDashboardData]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDashboardData] 

      -- Add the parameters for the stored procedure here

      @UserID nvarchar(100)      

AS

BEGIN

      -- SET NOCOUNT ON added to prevent extra result sets from

      -- interfering with SELECT statements.

      SET NOCOUNT ON;

 

    -- Insert statements for procedure here

      Select * from UserDashboardPreferance where UserId LIKE @UserID 

END
GO
/****** Object:  StoredProcedure [dbo].[GetCompanies]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------------
-- Purpose:	Retreive a list of the companies available in the database.
--
-- How to run this procedure:
--			exec dbo.GetCompanies null, null	-- All companies
--			exec dbo.GetCompanies 'BRA', null	-- Only from Brazil
--			exec dbo.GetCompanies null, 'USD'	-- Only US Dollar
--------------------------------------------------------------------------------
CREATE procedure [dbo].[GetCompanies]
		@Country			varchar(9)	
	,	@Currency			varchar(50)	
AS
BEGIN

SET NOCOUNT OFF;
SET FMTONLY OFF;

		
	-- Get the Company data.
	select	ci.Name
		,	ci.XRef
		,	ci.ReportNumber
		,	ci.Country
		,	ci.Currency
		,	ci.CurrentPeriod
		,	ci.Earnings
		,	ci.Active
		,	ci.CodeIndustry
		,	ci.CodeSector
	  from dbo.tblCompanyInfo ci
	 where 1=1
	   and (@Country is NULL or ci.Country = @Country)
	   and (@Currency is NULL or ci.Currency = @Currency)
	 ;

END;
GO
/****** Object:  StoredProcedure [dbo].[DetailedEstimates]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------------
-- Purpose:	Define a report that uses the available data and produces output
--			similar to the Reuters Detailed Estimates report.
--
-- How to run this procedure:
--			exec dbo.DetailedEstimates 100013798, null, null, null
--			exec dbo.DetailedEstimates null, 'Petroleo Brasileiro SA - Petrobras', null, 'NTP'
--			exec dbo.DetailedEstimates null, null, null, null
--------------------------------------------------------------------------------
CREATE procedure [dbo].[DetailedEstimates]
		@Xref				varchar(9)	
	,	@Name				varchar(50)	
	,	@PeriodType			varchar(2)	
	,	@EstimateType		varchar(20)	
AS
BEGIN

SET NOCOUNT OFF;
SET FMTONLY OFF;

Declare @FirstPeriodEndDate	datetime

	-- Set the parameter values to defaults if not provided

	if( (@Xref is NULL) and (@Name is NULL) )
		select @Name = 'Petroleo Brasileiro SA - Petrobras';

	if( (@Xref is NULL) and (@Name is not NULL) )
		select @Xref = Xref from dbo.tblCompanyInfo where Name = @Name;

	if( @PeriodType is NULL )
		set @PeriodType = 'A';

	if( @EstimateType is NULL )
		set @EstimateType = 'EPS';

	if( @FirstPeriodEndDate is NULL )
		set @FirstPeriodEndDate = '2009-12-31 00:00:00.000';

		
	-- Get the data needed for the first part.
	select	ci.Name
		,	ce.EstimateType
		,	'FY'+left(ce.fPeriodEnd, 4) as Period
		,	ce.PeriodEndDate
		,	ce.OriginalDate
		,	ce.ExpirationDate
		,	ce.PeriodType
		,	ce.Currency
		,	ce.Mean 
		,	ce.Median
		,	ce.NumOfEsts
		,	ce.High
		,	ce.Low
		,	ce.StdDev
		,	act.Actual
	  into	#est
	  from dbo.tblCompanyInfo ci
	 inner Join dbo.tblConsensusEstimate ce on ce.Xref = ci.Xref
	 inner join	(select Xref, PeriodEndDate, MAX(OriginalDate) as OriginalDate 
				  from dbo.tblConsensusEstimate 
				 where PeriodType = @PeriodType
				   and EstimateType = @EstimateType
				   and XRef = 100033558
				 group by Xref, PeriodEndDate
				) latest on latest.XRef = ce.XRef and latest.PeriodEndDate = ce.PeriodEndDate and latest.OriginalDate = ce.OriginalDate
				
	  left join	(select	a.XRef
					,	a.EstimateType
					,	a.PeriodEndDate
					,	a.PeriodType
					,	a.Currency
					,	a.ActualValue as Actual
				  from dbo.tblCompanyInfo ci
				 inner Join dbo.tblActual a on a.Xref = ci.Xref
				 where a.EstimateType = @EstimateType
				   and a.PeriodType = @PeriodType
				   and a.XRef = @Xref
				) act on act.XRef = ci.Xref and act.PeriodEndDate = ce.PeriodEndDate and act.Currency = ce.Currency
	 where 1=1
	   and ce.PeriodType = @PeriodType
	   and ce.EstimateType = @EstimateType
	   and (@FirstPeriodEndDate is NULL or ce.PeriodEndDate >= @FirstPeriodEndDate)
	   and ce.XRef = 100033558
	 ;



	-- PIVOT the data and put it into correct rows.
	select	Name
		,	EstimateType
		,	srt
		,	AmtType
		,	max([FY2009]) as FY2009
		,	max([FY2010]) as FY2010
		,	max([FY2011]) as FY2011
		,	max([FY2012]) as FY2012
		,	max([FY2013]) as FY2013
		,	max([FY2014]) as FY2014
		,	max([FY2015]) as FY2015
	 from (
			select pvt.Name, EstimateType, 1 as Srt, 'Mean' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Mean)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 2 as Srt, 'Median' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Median)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 3 as Srt, 'NumOfEsts' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(NumOfEsts)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 4 as Srt, 'High' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(High)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 5 as Srt, 'Low' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Low)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 6 as Srt, 'StdDev' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(StdDev)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
		union
			select pvt.Name, EstimateType, 7 as Srt, 'Actual' as AmtType, pvt.[FY2009], pvt.[FY2010], pvt.[FY2011], pvt.[FY2012], pvt.[FY2013], pvt.[FY2014], pvt.[FY2015]
			  from #est e
			 pivot 
			 ( max(Actual)
			 for Period in ([FY2009], [FY2010], [FY2011], [FY2012], [FY2013], [FY2014], [FY2015])
			 ) as pvt
			) a
	 group by Name, EstimateType, Srt, AmtType
	 order by Srt
	;

	-- Clean up the temp table
--	drop table #est;
END;
GO
/****** Object:  StoredProcedure [dbo].[ConsensusEstimates]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------------
-- Purpose:	Define a report that uses the available data and produces output
--			similar to the Reuters Detailed Estimates report.
--
-- How to run this procedure:
--			exec dbo.ConsensusEstimates 100013798, null, null
--			exec dbo.ConsensusEstimates null, 'Petroleo Brasileiro SA - Petrobras', null
--			exec dbo.ConsensusEstimates null, null, null
--------------------------------------------------------------------------------
CREATE procedure [dbo].[ConsensusEstimates]
		@Xref				varchar(9)	
	,	@Name				varchar(50)	
	,	@PeriodType			varchar(2)	
AS
BEGIN
Declare @FirstPeriodEndDate	datetime

SET NOCOUNT OFF;
SET FMTONLY OFF;

	-- Set the parameter values to defaults if not provided

	if( (@Xref is NULL) and (@Name is NULL) )
		select @Name = 'Petroleo Brasileiro SA - Petrobras';

	if( (@Xref is NULL) and (@Name is not NULL) )
		select @Xref = Xref from dbo.tblCompanyInfo where Name = @Name;

	if( @PeriodType is NULL )
		set @PeriodType = 'A';

	if( @FirstPeriodEndDate is NULL )
		set @FirstPeriodEndDate = '2009-12-31 00:00:00.000';



	-- Create a list of all the years needed.
	select 'A200912' as  sPeriodEnd, 'ADec2009' as Period into #years
	insert #years values ('A201012', 'ADec2010')
	insert #years values ('E200912', 'EDec2010')
	insert #years values ('E201012', 'EDec2010')
	insert #years values ('E201112', 'EDec2011')
	insert #years values ('E201212', 'EDec2012')
	insert #years values ('E201312', 'EDec2013')
	insert #years values ('E201412', 'EDec2014')
	insert #years values ('E201512', 'EDec2015')

	-- Get a list of all the Estimate types needed
	select distinct EstimateType 
	  into #esttype
	  from  (
				select distinct EstimateType
				  from dbo.tblActual 
				 where 1=1
				   and XRef = @Xref
				   and PeriodEndDate >= @FirstPeriodEndDate
				   and PeriodType = @PeriodType
			union
				select distinct ce.EstimateType
				  from dbo.tblConsensusEstimate ce
				 where 1=1
				   and XRef = @Xref
				   and PeriodType = @PeriodType
				   and fPeriodEnd >= '200912'
			) a


	-- Get the data
	select @XRef as Xref, y.period, y.EstimateType, a.Amount as Amount, a.NumOfEsts, a.High, a.Low, a.StdDev, a.AnnouncementDate
	  into #est
	  from  (
			select y.Period, et.EstimateType, y.sPeriodEnd
			  from #years y inner join #esttype et on 1=1
			) y
	  left join ( 
				-- Get the actual data for 2009 & 2010
				select XRef, 'A'+fPeriodEnd as sPeriodEnd, EstimateType, ActualValue as Amount, 0 as NumOfEsts, '' as High, '' as Low, '' as StdDev, AnnouncementDate
				  from dbo.tblActual 
				 where XRef = @Xref
				   and PeriodEndDate >= @FirstPeriodEndDate
				   and PeriodType = @PeriodType
			union
				-- Get the Estimated data
				select ce.XRef, 'E'+ce.fPeriodEnd as sPeriodEnd, ce.EstimateType, ce.Mean as Amount, NumOfEsts, ce.High, ce.Low, ce.StdDev, '' as AnnouncementDate
				  from dbo.tblConsensusEstimate ce
				 inner join (
							select XRef, fPeriodEnd, max(OriginalDate) as OriginalDate
							  from dbo.tblConsensusEstimate
							 where 1=1
							   and XRef = 100013798
							   and PeriodType = @PeriodType
							   and fPeriodEnd >= '200912'
							 group by XRef, fPeriodEnd
							 ) ceX on ceX.XRef = ce.XRef and ceX.fPeriodEnd = ce.fPeriodEnd and ceX.OriginalDate = ce.OriginalDate
				 where 1=1
				   and ce.XRef = 100013798
	--			   and ce.EstimateType = 'ROE'
				   and ce.PeriodType = @PeriodType
				   and ce.PeriodEndDate >= @FirstPeriodEndDate
				) a on a.sPeriodEnd = y.sPeriodEnd and a.EstimateType = y.EstimateType
			
		 
		 
	-- PIVOT the data and put it into correct rows.
	select	max(Xref) as Xref
		,	EstimateType
		,	srt
		,	AmtType
		,	max([ADec2009]) as Actual_Dec_2009
		,	max([ADec2010]) as Actual_Dec_2010
		,	max([EDec2009]) as Estimated_Dec_2009
		,	max([EDec2010]) as Estimated_Dec_2010
		,	max([EDec2011]) as Estimated_Dec_2011
		,	max([EDec2012]) as Estimated_Dec_2012
		,	max([EDec2013]) as Estimated_Dec_2013
		,	max([EDec2014]) as Estimated_Dec_2014
		,	max([EDec2015]) as Estimated_Dec_2015
	 from (
			select pvt.Xref, EstimateType, '0' as Srt, 'Amount' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(Amount) 
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '1' as Srt, 'NumOfEsts' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(NumOfEsts)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '2' as Srt, 'High' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(High)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '3' as Srt, 'Low' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(Low)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '4' as Srt, 'StdDev' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(StdDev)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
		union
			select pvt.Xref, EstimateType, '5' as Srt, 'AnnouncementDate' as AmtType
				,	case when pvt.[ADec2009] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[ADec2009], 101) else '' end as ADec2009
				,	case when pvt.[ADec2010] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[ADec2010], 101) else '' end as ADec2010 
				,	case when pvt.[EDec2009] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2009], 101) else '' end as ADec2009
				,	case when pvt.[EDec2010] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2010], 101) else '' end as ADec2010
				,	case when pvt.[EDec2011] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2011], 101) else '' end as ADec2011
				,	case when pvt.[EDec2012] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2012], 101) else '' end as ADec2012
				,	case when pvt.[EDec2013] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2013], 101) else '' end as ADec2013 
				,	case when pvt.[EDec2014] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2014], 101) else '' end as ADec2014
				,	case when pvt.[EDec2015] <> 'Jan  1 1900 12:00AM' then convert(varchar(40), pvt.[EDec2015], 101) else '' end as ADec2015 
			  from #est e 
			 pivot 
			 ( max(AnnouncementDate)
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
			) a
	 group by EstimateType, Srt, AmtType
	 order by EstimateType, Srt, AmtType
	;

	-- Clean up
	drop table #est;
	drop table #years;
	drop table #esttype;

END;
GO
/****** Object:  StoredProcedure [dbo].[Aggregation1]    Script Date: 03/22/2012 15:17:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------------
-- Purpose:	Define a report that returns the data needed for the Aggregation 
--			process, based on the list of keys provided from the Oracle data.
--
-- How to run this procedure:
--			exec dbo.Aggregation1 '1234,5678,90'
--------------------------------------------------------------------------------
CREATE procedure [dbo].[Aggregation1]
		@keylist			varchar(1000)	
AS
BEGIN
Declare @FirstPeriodEndDate	datetime;
Declare @PeriodType 	varchar(2);
Declare @EstimateType	Varchar(10);
Declare @Sec_Secshort	Varchar(1000);
declare	@Xref			varchar(9)	
declare @x				int;
declare @y				int;

SET NOCOUNT OFF;
SET FMTONLY OFF;

	-- Set the parameter values to defaults if not provided

	if( @keylist is NULL )
		return;

	select @PeriodType = 'A';
	
	set @FirstPeriodEndDate = '2009-12-31 00:00:00.000';
	
	-- Create a list of all the years needed.
	select 'A200912' as  sPeriodEnd, 'ADec2009' as Period into #years
	insert #years values ('A201012', 'ADec2010')
	insert #years values ('E200912', 'EDec2010')
	insert #years values ('E201012', 'EDec2010')
	insert #years values ('E201112', 'EDec2011')
	insert #years values ('E201212', 'EDec2012')
	insert #years values ('E201312', 'EDec2013')
	insert #years values ('E201412', 'EDec2014')
	insert #years values ('E201512', 'EDec2015')

	-- Get a list of all the Estimate types needed
	select 'NTP' estimatetype
	  into #esttype;


	-- Create a table of the SEC_SECSHORT key values
	select @Sec_SecShort = @keylist
	select @x = 0;
	select @y = 1;
	
	create table #SecShortList ( sec_secshort char(20) NOT NULL, Xref char(9) );
	
	while LEN(@Sec_SecShort) > 0
		BEGIN
			select @x =  charindex( ',', @Sec_SecShort, 0 ) - 1;
			if (@x <= 0) select @x = LEN( @Sec_SecShort ), @y = 0;
			insert into #SecShortList select SUBSTRING( @sec_Secshort, 1, @x - @y) as sec_secshort, '100013798' as Xref;
			select @Sec_SecShort = right( @Sec_SecShort, LEN(@Sec_SecShort) - @x - @y);
		END;

							 
	-- Get the data
	select ss.Xref, ss.sec_secshort, y.period, y.EstimateType, a.Amount as Amount, a.NumOfEsts, a.High, a.Low, a.StdDev, a.AnnouncementDate
	  into #est
	  from  (
			select y.Period, et.EstimateType, y.sPeriodEnd
			  from #years y inner join #esttype et on 1=1
			) y
	  left join ( 
				-- Get the actual data for 2009 & 2010
				select XRef, 'A'+fPeriodEnd as sPeriodEnd, EstimateType, ActualValue as Amount, 0 as NumOfEsts, '' as High, '' as Low, '' as StdDev, AnnouncementDate
				  from dbo.tblActual 
				 where XRef = @Xref
				   and PeriodEndDate >= @FirstPeriodEndDate
				   and PeriodType = @PeriodType
			union
				-- Get the Estimated data
				select ce.XRef, 'E'+ce.fPeriodEnd as sPeriodEnd, ce.EstimateType, ce.Mean as Amount, NumOfEsts, ce.High, ce.Low, ce.StdDev, '' as AnnouncementDate
				  from dbo.tblConsensusEstimate ce
				 inner join (
							select XRef, fPeriodEnd, max(OriginalDate) as OriginalDate
							  from dbo.tblConsensusEstimate
							 where 1=1
							   and XRef in (select distinct XRef from #SecShortList)
							   and PeriodType = @PeriodType
							   and fPeriodEnd >= '200912'
							 group by XRef, fPeriodEnd
							 ) ceX on ceX.XRef = ce.XRef and ceX.fPeriodEnd = ce.fPeriodEnd and ceX.OriginalDate = ce.OriginalDate
				 where 1=1
--				   and ce.XRef in (select distinct XRef from #SecShortList)
--				   and ce.XRef = 100013798
	--			   and ce.EstimateType = 'ROE'
				   and ce.PeriodType = @PeriodType
				   and ce.PeriodEndDate >= @FirstPeriodEndDate
				) a on a.sPeriodEnd = y.sPeriodEnd and a.EstimateType = y.EstimateType
	 inner join #SecShortList ss on ss.Xref = a.XRef
			
		 
		 
	-- PIVOT the data and put it into correct rows.
	select	a.Xref as Xref
		,	sec_secshort as SEC_SECSHORT
		,	ci.Name as CompanyName
		,	ci.Country
		,	ci.ISIN
		,	EstimateType
		,	srt
		,	AmtType
		,	max([ADec2009]) as Actual_Dec_2009
		,	max([ADec2010]) as Actual_Dec_2010
		,	max([EDec2009]) as Estimated_Dec_2009
		,	max([EDec2010]) as Estimated_Dec_2010
		,	max([EDec2011]) as Estimated_Dec_2011
		,	max([EDec2012]) as Estimated_Dec_2012
		,	max([EDec2013]) as Estimated_Dec_2013
		,	max([EDec2014]) as Estimated_Dec_2014
		,	max([EDec2015]) as Estimated_Dec_2015
	 from (
			select pvt.Xref, sec_secshort,  EstimateType, '0' as Srt, 'Amount' as AmtType
				,	cast(pvt.[ADec2009] as varchar(40)) as ADec2009
				,	cast(pvt.[ADec2010] as varchar(40)) as ADec2010
				,	cast(pvt.[EDec2009] as varchar(40)) as EDec2009
				,	cast(pvt.[EDec2010] as varchar(40)) as EDec2010
				,	cast(pvt.[EDec2011] as varchar(40)) as EDec2011
				,	cast(pvt.[EDec2012] as varchar(40)) as EDec2012
				,	cast(pvt.[EDec2013] as varchar(40)) as EDec2013
				,	cast(pvt.[EDec2014] as varchar(40)) as EDec2014
				,	cast(pvt.[EDec2015] as varchar(40)) as EDec2015
			  from #est e 
			 pivot 
			 ( max(Amount) 
			 for Period in ([ADec2009], [ADec2010], [EDec2009], [EDec2010], [EDec2011], [EDec2012], [EDec2013], [EDec2014], [EDec2015])
			 ) as pvt
			) a
	 inner join dbo.tblCompanyInfo ci on ci.XRef = a.Xref
	 group by a.XRef, SEC_SECSHORT, ci.Name, Country, ISIN, EstimateType, Srt, AmtType
	 order by a.XRef, SEC_SECSHORT, ci.Name, Country, ISIN, EstimateType, Srt, AmtType
	;

	-- Clean up
	drop table #est;
	drop table #years;
	drop table #esttype;
	drop table #SecShortList;
	
END;
GO
/****** Object:  Default [DF_tblCompanyInfo_Active]    Script Date: 03/22/2012 15:17:05 ******/
ALTER TABLE [dbo].[tblCompanyInfo] ADD  CONSTRAINT [DF_tblCompanyInfo_Active]  DEFAULT ((1)) FOR [Active]
GO
