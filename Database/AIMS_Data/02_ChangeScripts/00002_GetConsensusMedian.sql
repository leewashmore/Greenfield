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

CREATE procedure [dbo].[GetConsensusEstimatesMedian](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@DATA_SOURCE		varchar(10)  = 'REUTERS'	-- REUTERS, PRIMARY, INDUSTRY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
)
as

Declare
@earnings nvarchar(20),
@netIncomeType int,
@epsktype int

SET @earnings = (Select Earnings from [Reuters].[dbo].tblCompanyInfo 
				where XRef IN (Select XRef from GF_SECURITY_BASEVIEW 
								where ISSUER_ID = @Issuer_Id));
								
SET @netIncomeType =   CASE @earnings  
  WHEN 'EPS' THEN 11  
  WHEN 'EPSREP' THEN 13   
  WHEN 'EBG' THEN 12  
  ELSE null 
  END
  
  SET @epsktype =   CASE @earnings  
  WHEN 'EPS' THEN 8  
  WHEN 'EPSREP' THEN 9   
  WHEN 'EBG' THEN 5  
  ELSE null
  END 	


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
	   and cce.ESTIMATE_ID in (17,7,@netIncomeType,@epsktype,18,19)
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ACTUAL'
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
	   and cce.ESTIMATE_ID in (17,7,@netIncomeType,@epsktype,18,19)
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ESTIMATE'
	 order by cce.PERIOD_YEAR
	 ;

	
	-- Combine Actual with Estimated so that Actual is always used in case both are present.
	select isnull(e.ISSUER_ID, a.ISSUER_ID) as ISSUER_ID
		,  isnull(e.ESTIMATE_ID, a.ESTIMATE_ID) as ESTIMATE_ID
		,  isnull(e.ESTIMATE_DESC, a.ESTIMATE_DESC) as [ESTIMATE_DESC]
		,  isnull(e.Period, a.Period) as Period
		,  isnull(e.AMOUNT_TYPE, a.AMOUNT_TYPE) as AMOUNT_TYPE
		,  isnull(e.PERIOD_YEAR, a.PERIOD_YEAR) as PERIOD_YEAR
		,  isnull(e.PERIOD_TYPE, a.PERIOD_TYPE) as PERIOD_TYPE
		,  isnull(e.AMOUNT, a.AMOUNT) as AMOUNT
		,  b.AMOUNT as ASHMOREEMM_AMOUNT
		,  isnull(e.NUMBER_OF_ESTIMATES, a.NUMBER_OF_ESTIMATES) as NUMBER_OF_ESTIMATES
		,  isnull(e.HIGH, a.HIGH) as HIGH
		,  isnull(e.LOW, a.LOW) as LOW
		,  isnull(e.STANDARD_DEVIATION, a.STANDARD_DEVIATION) as STANDARD_DEVIATION
		,  isnull(e.SOURCE_CURRENCY, a.SOURCE_CURRENCY) as SOURCE_CURRENCY
		,  isnull(e.DATA_SOURCE, a.DATA_SOURCE) as DATA_SOURCE
		,  isnull(e.DATA_SOURCE_DATE, a.DATA_SOURCE_DATE) as DATA_SOURCE_DATE
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
				 ) b on b.ISSUER_ID = ae.ISSUER_ID
					and (   (b.DATA_ID =  11 and ae.ESTIMATE_ID = 17)		-- Revenue
						 or (b.DATA_ID = 130 and ae.ESTIMATE_ID = 7)			-- EBITDA
						 or (b.DATA_ID =  44 and ae.ESTIMATE_ID in(11,12,13))-- Net Income
						 or (b.DATA_ID = 131 and ae.ESTIMATE_ID in(8,9,5))	-- EPS
						 or (b.DATA_ID = 132 and ae.ESTIMATE_ID = 18)		-- ROA
						 or (b.DATA_ID = 133 and ae.ESTIMATE_ID = 19)		-- ROE
						)
				    and b.PERIOD_YEAR = ae.PERIOD_YEAR
--				    and substring(b.PERIOD_TYPE,1,1) = @PERIOD_TYPE
--					and b.FISCAL_TYPE = @FISCAL_TYPE
--					and b.CURRENCY = @CURRENCY
--					and b.AMOUNT_TYPE = 'ESTIMATE'
		;

	-- Clean up
	drop table #Actual;
	drop table #Estimate;
GO










--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



