set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00317'
declare @CurrentScriptVersion as nvarchar(100) = '00318'

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
            ------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:165 Adjusted P/BV
--
-- (185) / (104 - [259 x (1-155) x (1-232)]) 
--
-- Author:	Justin Machata
-- Date:	January 23, 2013
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_165', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_165;
GO

create procedure [dbo].[AIMS_Calc_165](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as
	
	-- Get the data
		select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'


	select *
	  into #B104
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 104				-- QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select *
	  into #B259
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 259
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select *
	  into #B155
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 155
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select *
	  into #B232
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 232				-- 
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'



	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		  , a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
		  , a.DATA_ID, (a.AMOUNT - (isnull(b.AMOUNT, 0.0) * (1-isnull(c.AMOUNT, 0.0)) * (1-isnull(d.AMOUNT, 0.0)))) as AMOUNT
		  , ' ' as CALCULATION_DIAGRAM, a.SOURCE_CURRENCY, a.AMOUNT_TYPE
	  into #B
	  from #B104 a
	   left join #B259 b on b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_YEAR = a.PERIOD_YEAR 
						and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
						and b.PERIOD_END_DATE = a.PERIOD_END_DATE
	   left join #B155 c on c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_YEAR = a.PERIOD_YEAR 
						and c.FISCAL_TYPE = a.FISCAL_TYPE and c.CURRENCY = a.CURRENCY
						and c.PERIOD_END_DATE = a.PERIOD_END_DATE
	   left join #B232 d on d.DATA_SOURCE = a.DATA_SOURCE and d.PERIOD_YEAR = a.PERIOD_YEAR 
						and d.FISCAL_TYPE = a.FISCAL_TYPE and d.CURRENCY = a.CURRENCY
						and d.PERIOD_END_DATE = a.PERIOD_END_DATE


	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  165 as DATA_ID										-- DATA_ID:165 Adjusted P/BV 
		,  a.AMOUNT /b.AMOUNT as AMOUNT						-- 185/(104 - [259 x (1-155) x (1-232)])   
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') / QTLE - [NPL/Total Loans * (1-Provisions/NPL) * (1-Tax Rate)](' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation


	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 165 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 165 Adjusted P/BV.  DATA_ID:104 QTLE is NULL or ZERO'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error	
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 165 as DATA_ID, sb.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 165 Adjusted P/BV.  DATA_ID:104 QTLE is missing' as TXT
			  from #A a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 165 as DATA_ID, sb.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 165 Adjusted P/BV.  DATA_ID:185 Market Cap is missing' as TXT
			  from #B a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#A b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 165 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 165 Adjusted P/BV  .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 165 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 165 Adjusted P/BV.  DATA_ID:104 QTLE no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END



		
	-- Clean up
	drop table #A
	drop table #B
	drop table #B104	
	drop table #B259
	drop table #B155
	drop table #B232

GO




--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00318'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())