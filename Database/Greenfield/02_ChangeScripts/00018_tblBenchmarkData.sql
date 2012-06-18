set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblBenchmarkData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PORTFOLIO_DATE] [datetime] NOT NULL,
	[BENCHMARK_ID] [varchar](50) NULL,
	[PORTFOLIO_THEME_SUBGROUP_CODE] [varchar](50) NULL,
	[PORTFOLIO_CURRENCY] [varchar](50) NULL,
	[ISSUER_ID] [int] NOT NULL,
	[ASEC_SEC_SHORT_NAME] [varchar](50) NULL,
	[ISSUE_NAME] [varchar](255) NULL,
	[TICKER] [varchar](50) NULL,
	[SECURITYTHEMECODE] [varchar](50) NULL,
	[ASEC_INSTR_TYPE] [varchar](50) NULL,
	[SECURITY_TYPE] [varchar](50) NULL,
	[BALANCE_NOMINAL] [real] NULL,
	[DIRTY_PRICE] [real] NULL,
	[TRADING_CURRENCY] [varchar](50) NULL,
	[DIRTY_VALUE_PC] [real] NULL,
	[BENCHMARK_WEIGHT] [real] NULL,
	[ASH_EMM_MODEL_WEIGHT] [real] NULL,
	[ASHEMM_PROPRIETARY_REGION_CODE] [varchar](50) NULL,
	[ASHEMM_PROP_REGION_NAME] [varchar](255) NULL,
	[ISO_COUNTRY_CODE] [varchar](50) NULL,
	[COUNTRYNAME] [varchar](255) NULL,
	[GICS_SECTOR] [int] NULL,
	[GICS_SECTOR_NAME] [varchar](255) NULL,
	[GICS_INDUSTRY] [int] NULL,
	[GICS_INDUSTRY_NAME] [varchar](255) NULL,
	[GICS_SUB_INDUSTRY] [int] NULL,
	[GICS_SUB_INDUSTRY_NAME] [varchar](255) NULL,
	[LOOK_THRU_FUND] [varchar](50) NULL,
	[BARRA_RISK_FACTOR_MOMENTUM] [real] NULL,
	[BARRA_RISK_FACTOR_VOLATILITY] [real] NULL,
	[BARRA_RISK_FACTOR_VALUE] [real] NULL,
	[BARRA_RISK_FACTOR_SIZE] [real] NULL,
	[BARRA_RISK_FACTOR_SIZE_NONLINEAR] [real] NULL,
	[BARRA_RISK_FACTOR_GROWTH] [real] NULL,
	[BARRA_RISK_FACTOR_LIQUIDITY] [real] NULL,
	[BARRA_RISK_FACTOR_LEVERAGE] [real] NULL,
	[BARRA_RISK_FACTOR_PBETEWLD] [int] NULL,
 CONSTRAINT [PK__tblBench__3214EC276E01572D] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






