set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00002'
declare @CurrentScriptVersion as nvarchar(100) = '00003'

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

Create procedure [dbo].[GetConsensusEstimatesValuation](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@DATA_SOURCE		varchar(10)  = 'REUTERS'	-- REUTERS, PRIMARY, INDUSTRY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
,	@ESTIMATE_ID		integer = NULL				-- A specific ID or NULL for all.
,	@PERIOD_YEAR		integer = NULL				-- A specific year if required	
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
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID =cce.ESTIMATE_ID
	 where 1=1
	   and cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ACTUAL'
	   and (cce.ESTIMATE_ID =cce.ESTIMATE_ID)
	   and (@PERIOD_YEAR is NULL or (cce.PERIOD_YEAR = @PERIOD_YEAR))
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
	  left join #Actual a on a.ISSUER_ID = cce.ISSUER_ID and a.ESTIMATE_ID = cce.ESTIMATE_ID
						 and a.PERIOD_YEAR = cce.PERIOD_YEAR and a.PERIOD_TYPE = cce.PERIOD_TYPE
	 inner join dbo.CONSENSUS_MASTER cm on cm.ESTIMATE_ID =cce.ESTIMATE_ID
	 where a.ISSUER_ID is NULL
	   and cce.ISSUER_ID = @ISSUER_ID
	   and cce.DATA_SOURCE = @DATA_SOURCE
	   and substring(cce.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and cce.FISCAL_TYPE = @FISCAL_TYPE
	   and cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ESTIMATE'
	   and (cce.ESTIMATE_ID =cce.ESTIMATE_ID)
	   and (@PERIOD_YEAR is NULL or (cce.PERIOD_YEAR = @PERIOD_YEAR))
	 order by cce.PERIOD_YEAR
	 ;

	-- Combine Actual with Estimated so that Actual is always used in case both are present.
	select a.ISSUER_ID
		,  a.ESTIMATE_ID
		,  a.ESTIMATE_DESC
		,  a.Period
		,  a.AMOUNT_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_TYPE
		,  a.AMOUNT
		,  b.AMOUNT as ASHMOREEMM_AMOUNT
		,  a.NUMBER_OF_ESTIMATES
		,  a.HIGH
		,  a.LOW
		,  a.STANDARD_DEVIATION
		,  a.SOURCE_CURRENCY
		,  a.DATA_SOURCE
		,  a.DATA_SOURCE_DATE
	  from (select * from #Actual
			union
			select * from #Estimate
		   ) a
	  left join (Select pf.ISSUER_ID, pf.AMOUNT, pf.AMOUNT_TYPE, pf.DATA_ID
				   from PERIOD_FINANCIALS pf
				  where 1=1
				    and pf.DATA_SOURCE = 'PRIMARY'
				    and substring(pf.PERIOD_TYPE,1,1) = @PERIOD_TYPE
				    and pf.FISCAL_TYPE = @FISCAL_TYPE
				    and pf.CURRENCY = @CURRENCY
				    and pf.ISSUER_ID = @ISSUER_ID
				 ) b on b.ISSUER_ID = a.ISSUER_ID
					and (   (b.DATA_ID =  170 and a.ESTIMATE_ID = 170)		-- P/Revenue
						 or (b.DATA_ID = 171 and a.ESTIMATE_ID = 171)			-- P/CE
						 or (b.DATA_ID =  166 and a.ESTIMATE_ID =166)-- P/E
						 or (b.DATA_ID = 172 and a.ESTIMATE_ID =172)	-- P/E To Growth
						 or (b.DATA_ID = 164 and a.ESTIMATE_ID = 164)		-- P/BV
						 or (b.DATA_ID = 192 and a.ESTIMATE_ID = 192)		-- DividendYield
						)
		;



	-- Clean up
	drop table #Actual;
	drop table #Estimate;


GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00003'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



