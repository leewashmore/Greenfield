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

CREATE TABLE [dbo].[tblPerformanceData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TO_DATE] [datetime] NULL,
	[NODE_NAME] [varchar](50) NULL,
	[PORT_GROUP] [varchar](50) NULL,
	[PORTFOLIO] [varchar](50) NULL,
	[AGG_LVL_1] [varchar](50) NULL,
	[BM1_OFFICIAL_RC_3M] [real] NULL,
	[BM1_RC_ASSET_ALLOC_YTD] [real] NULL,
	[BM1_RC_ASSET_ALLOC_1M] [real] NULL,
	[BM1_RC_ASSET_ALLOC_3M] [real] NULL,
	[BM1_RC_ASSET_ALLOC_6M] [real] NULL,
	[BM1_RC_ASSET_ALLOC_1Y] [real] NULL,
	[BM1_RC_ASSET_ALLOC_SI] [real] NULL,
	[BM1_RC_AVG_WGT_1M] [real] NULL,
	[BM1_RC_AVG_WGT_3M] [real] NULL,
	[BM1_RC_AVG_WGT_6M] [real] NULL,
	[BM1_RC_AVG_WGT_1Y] [real] NULL,
	[BM1_RC_AVG_WGT_SI] [real] NULL,
	[BM1_RC_CCY_RTN_YTD] [real] NULL,
	[BM1_RC_CCY_RTN_1M] [real] NULL,
	[BM1_RC_CCY_RTN_3M] [real] NULL,
	[BM1_RC_CCY_RTN_6M] [real] NULL,
	[BM1_RC_CCY_RTN_1Y] [real] NULL,
	[BM1_RC_CCY_RTN_SI] [real] NULL,
	[BM1_RC_CTN_YTD] [real] NULL,
	[BM1_RC_CTN_1M] [real] NULL,
	[BM1_RC_CTN_3M] [real] NULL,
	[BM1_RC_CTN_6M] [real] NULL,
	[BM1_RC_CTN_1Y] [real] NULL,
	[BM1_RC_CTN_SI] [real] NULL,
	[BM1_RC_CURR_ALLOC_YTD] [real] NULL,
	[BM1_RC_CURR_ALLOC_1M] [real] NULL,
	[BM1_RC_CURR_ALLOC_3M] [real] NULL,
	[BM1_RC_CURR_ALLOC_6M] [real] NULL,
	[BM1_RC_CURR_ALLOC_1Y] [real] NULL,
	[BM1_RC_CURR_ALLOC_SI] [real] NULL,
	[BM1_RC_EXCESS_CTN_YTD] [real] NULL,
	[BM1_RC_EXCESS_CTN_1M] [real] NULL,
	[BM1_RC_EXCESS_CTN_3M] [real] NULL,
	[BM1_RC_EXCESS_CTN_6M] [real] NULL,
	[BM1_RC_EXCESS_CTN_1Y] [real] NULL,
	[BM1_RC_EXCESS_CTN_SI] [real] NULL,
	[BM1_RC_SEC_SELEC_YTD] [real] NULL,
	[BM1_RC_SEC_SELEC_1M] [real] NULL,
	[BM1_RC_SEC_SELEC_3M] [real] NULL,
	[BM1_RC_SEC_SELEC_6M] [real] NULL,
	[BM1_RC_SEC_SELEC_1Y] [real] NULL,
	[BM1_RC_SEC_SELEC_SI] [real] NULL,
	[BM1_RC_TWR_YTD] [real] NULL,
	[BM1_RC_TWR_1M] [real] NULL,
	[BM1_RC_TWR_3M] [real] NULL,
	[BM1_RC_TWR_6M] [real] NULL,
	[BM1_RC_TWR_1Y] [real] NULL,
	[BM1_RC_TWR_SI_ANN] [real] NULL,
	[BM1_RC_WGT_EOD] [real] NULL,
	[BM1_RC_WGT_SOD] [real] NULL,
	[POR_OFFICIAL_RC_3M] [real] NULL,
	[POR_RC_AVG_WGT_1M] [real] NULL,
	[POR_RC_AVG_WGT_3M] [real] NULL,
	[POR_RC_AVG_WGT_6M] [real] NULL,
	[POR_RC_AVG_WGT_1Y] [real] NULL,
	[POR_RC_AVG_WGT_SI] [real] NULL,
	[POR_RC_EXPOSURE] [real] NULL,
	[POR_RC_MARKET_VALUE] [real] NULL,
	[POR_RC_CASH_FLOW] [real] NULL,
	[POR_RC_CCY_RTN_YTD] [real] NULL,
	[POR_RC_CCY_RTN_1M] [real] NULL,
	[POR_RC_CCY_RTN_3M] [real] NULL,
	[POR_RC_CCY_RTN_6M] [real] NULL,
	[POR_RC_CCY_RTN_1Y] [real] NULL,
	[POR_RC_CCY_RTN_SI] [real] NULL,
	[POR_RC_CTN_YTD] [real] NULL,
	[POR_RC_CTN_1M] [real] NULL,
	[POR_RC_CTN_3M] [real] NULL,
	[POR_RC_CTN_6M] [real] NULL,
	[POR_RC_CTN_1Y] [real] NULL,
	[POR_RC_CTN_SI] [real] NULL,
	[POR_RC_TWR_1M] [real] NULL,
	[POR_RC_TWR_3M] [real] NULL,
	[POR_RC_TWR_6M] [real] NULL,
	[POR_RC_TWR_1Y] [real] NULL,
	[POR_RC_TWR_SI_ANN] [real] NULL,
	[POR_RC_TWR_SI_CUM] [real] NULL,
	[POR_RC_TWR_YTD] [real] NULL,
	[POR_RC_WGT_EOD] [real] NULL,
	[POR_RC_WGT_SOD] [real] NULL,
 CONSTRAINT [PK__tblPerfo__3214EC2701142BA1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






