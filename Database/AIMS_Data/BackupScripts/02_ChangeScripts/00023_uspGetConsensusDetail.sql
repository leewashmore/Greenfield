set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00022'
declare @CurrentScriptVersion as nvarchar(100) = '00023'

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

IF OBJECT_ID ('[dbo].[Get_Consensus_Detail]') IS NOT NULL
	DROP PROCEDURE [dbo].[Get_Consensus_Detail]
GO

/****** Object:  StoredProcedure [dbo].[Get_Consensus_Detail]    Script Date: 08/23/2012 20:01:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






--------------------------------------------------------------------------------------
-- Name:	Get_Consensus
--
-- Purpose:	Get the consensus data to display the statement in the form that the 
--			user needs it.  The Actual data must be combined with the estimated  data
--			so that Actual is always used in case both are present.
--
--------------------------------------------------------------------------------------
CREATE procedure [dbo].[Get_Consensus_Detail](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@DATA_SOURCE		varchar(10)  = 'REUTERS'	-- REUTERS, PRIMARY, INDUSTRY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
)
as

	-- Select the Actual data 
	select cce.ISSUER_ID
		,  cce.ESTIMATE_ID
		,  cm.ESTIMATE_DESC
		,  substring(AMOUNT_TYPE,1,1)+cast(cce.PERIOD_YEAR as CHAR(4))+cce.PERIOD_TYPE as Period
		,  cce.AMOUNT_TYPE
		,  cce.PERIOD_YEAR
		,  cce.PERIOD_TYPE
		,  cce.AMOUNT
		,  0.0 as ASHMOREEMM_AMOUNT
		,  cce.NUMBER_OF_ESTIMATES
		,  cce.HIGH
		,  cce.LOW
		,  cce.STANDARD_DEVIATION
		,  cce.SOURCE_CURRENCY
		,  cce.DATA_SOURCE
		,  cce.DATA_SOURCE_DATE
	  into #Actual
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID = cce.ESTIMATE_ID
	 where 1=1
	   and cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ACTUAL'
	   and cce.ESTIMATE_ID in (17,7,8,11,18,19)
	 order by cce.PERIOD_YEAR
	 ;

	-- Select the Estimated data
	select cce.ISSUER_ID
		,  cce.ESTIMATE_ID
		,  cm.ESTIMATE_DESC
		,  substring(cce.AMOUNT_TYPE,1,1)+cast(cce.PERIOD_YEAR as CHAR(4))+cce.PERIOD_TYPE as Period
		,  cce.AMOUNT_TYPE
		,  cce.PERIOD_YEAR
		,  cce.PERIOD_TYPE
		,  cce.AMOUNT
		,  0.0 as ASHMOREEMM_AMOUNT
		,  cce.NUMBER_OF_ESTIMATES
		,  cce.HIGH
		,  cce.LOW
		,  cce.STANDARD_DEVIATION
		,  cce.SOURCE_CURRENCY
		,  cce.DATA_SOURCE
		,  cce.DATA_SOURCE_DATE
	  into #Estimate
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID = cce.ESTIMATE_ID
	 where 1=1
	   and cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ESTIMATE'
	   and cce.ESTIMATE_ID in (17,7,8,11,18,19)
	 order by cce.PERIOD_YEAR
	 ;

	
	-- Combine Actual with Estimated so that Actual is always used in case both are present.
	select isnull(a.ISSUER_ID, e.ISSUER_ID) as ISSUER_ID
		,  isnull(a.ESTIMATE_ID, e.ESTIMATE_ID) as ESTIMATE_ID
		,  isnull(a.ESTIMATE_DESC, e.ESTIMATE_DESC) as ESTIMATE_DESC
		,  isnull(a.Period, e.Period) as PERIOD
		,  isnull(a.AMOUNT_TYPE, e.AMOUNT_TYPE) as AMOUNT_TYPE
		,  isnull(a.PERIOD_YEAR, e.PERIOD_YEAR) as PERIOD_YEAR
		,  isnull(a.PERIOD_TYPE, e.PERIOD_TYPE) as PERIOD_TYPE
		,  isnull(a.AMOUNT, e.AMOUNT) as AMOUNT
		,  b.AMOUNT as ASHMOREEMM_AMOUNT
		,  isnull(a.NUMBER_OF_ESTIMATES, e.NUMBER_OF_ESTIMATES) as NUMBER_OF_ESTIMATES
		,  isnull(a.HIGH, e.HIGH) as HIGH
		,  isnull(a.LOW, e.LOW) as LOW
		,  isnull(a.STANDARD_DEVIATION, e.STANDARD_DEVIATION) as STANDARD_DEVIATION
		,  isnull(a.SOURCE_CURRENCY, e.SOURCE_CURRENCY) as SOURCE_CURRENCY
		,  isnull(a.DATA_SOURCE, e.DATA_SOURCE) as DATA_SOURCE
		,  isnull(a.DATA_SOURCE_DATE, e.DATA_SOURCE_DATE) as DATA_SOURCE_DATE
		,  a.AMOUNT  as ACTUAL
		
	-- Need to get all the possible rows
	  from (select distinct * 
			 from (		  select ISSUER_ID, DATA_SOURCE, ESTIMATE_ID, PERIOD_YEAR, SOURCE_CURRENCY 
							from #Actual 
					union 
						  select ISSUER_ID, DATA_SOURCE, ESTIMATE_ID, PERIOD_YEAR, SOURCE_CURRENCY
							from #Estimate
				  ) u
			) ae
	  left join #Estimate e on e.ISSUER_ID = ae.ISSUER_ID and e.DATA_SOURCE = ae.DATA_SOURCE 
				and e.ESTIMATE_ID = ae.ESTIMATE_ID
				and e.PERIOD_YEAR = ae.PERIOD_YEAR and e.SOURCE_CURRENCY = ae.SOURCE_CURRENCY
	  left join #Actual a on a.ISSUER_ID = ae.ISSUER_ID and a.DATA_SOURCE = ae.DATA_SOURCE 
				and a.ESTIMATE_ID = ae.ESTIMATE_ID
				and a.PERIOD_YEAR = ae.PERIOD_YEAR and a.SOURCE_CURRENCY = ae.SOURCE_CURRENCY
	  left join (Select pf.ISSUER_ID, pf.AMOUNT, pf.AMOUNT_TYPE, pf.DATA_ID
					,   pf.FISCAL_TYPE, pf.CURRENCY, pf.PERIOD_YEAR, pf.PERIOD_TYPE
				   from PERIOD_FINANCIALS pf
				  where 1=1
				    and pf.DATA_SOURCE = 'PRIMARY'
				    and substring(pf.PERIOD_TYPE,1,1) = @PERIOD_TYPE
				    and pf.FISCAL_TYPE = @FISCAL_TYPE
				    and pf.CURRENCY = @CURRENCY
				    and pf.ISSUER_ID = @ISSUER_ID
				 ) b on b.ISSUER_ID = a.ISSUER_ID
					and (   (b.DATA_ID =  11 and a.ESTIMATE_ID = 17)		-- Revenue
						 or (b.DATA_ID = 130 and a.ESTIMATE_ID = 7)			-- EBITDA
						 or (b.DATA_ID =  44 and a.ESTIMATE_ID = 11)		-- Net Income
						 or (b.DATA_ID = 131 and a.ESTIMATE_ID = 8)			-- EPS
						 or (b.DATA_ID = 132 and a.ESTIMATE_ID = 18)		-- ROA
						 or (b.DATA_ID = 133 and a.ESTIMATE_ID = 19)		-- ROE
						)
				    and substring(b.PERIOD_TYPE,1,1) = @PERIOD_TYPE
					and b.FISCAL_TYPE = @FISCAL_TYPE
					and b.CURRENCY = @CURRENCY
					and b.AMOUNT_TYPE = 'ESTIMATE'
		;



	-- Clean up
	drop table #Actual;
	drop table #Estimate;

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00023'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


