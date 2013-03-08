set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00191'
declare @CurrentScriptVersion as nvarchar(100) = '00192'

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
-- Purpose:	This procedure calculates the value for DATA_ID:162 ROIC
--
-- (130 � 291 � TTAX) / (QTLE + LMIN + 190 for Year + QTLE + LMIN + 190 for Prior Year)/2), 
--
-- Author:	Prerna 
-- Date:	July 16, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_162', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_162;
GO

create procedure [dbo].[AIMS_Calc_162](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as
	
	-- Get the data
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		  , a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
		  , a.DATA_ID, (a.AMOUNT - b.AMOUNT - c.AMOUNT) as AMOUNT, ' ' as CALCULATION_DIAGRAM, a.SOURCE_CURRENCY, a.AMOUNT_TYPE
		 
	  into #A
	  from (select * from dbo.PERIOD_FINANCIALS pf 
			 where DATA_ID = 130					-- 
			   and pf.ISSUER_ID = @ISSUER_ID
			   and pf.PERIOD_TYPE = 'A'
			) a
	  left join (select * from dbo.PERIOD_FINANCIALS pf 
				  where DATA_ID = 291					-- 
				    and pf.ISSUER_ID = @ISSUER_ID
				    and pf.PERIOD_TYPE = 'A'
				 ) b on b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
	  left join (select * from dbo.PERIOD_FINANCIALS pf 
				  where DATA_ID = 37					-- TTAX
				    and pf.ISSUER_ID = @ISSUER_ID
				    and pf.PERIOD_TYPE = 'A'
				 ) c on c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_YEAR = a.PERIOD_YEAR 
					and c.FISCAL_TYPE = a.FISCAL_TYPE and c.CURRENCY = a.CURRENCY

--select 'calc_162 #A' as A, * from #A where currency = 'USD' and data_source = 'PRIMARY' and FISCAL_TYPE = 'CALENDAR' Order by PERIOD_YEAR, PERIOD_TYPE
	select *
	  into #B190
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 190
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select *
	  into #B104
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 104				-- QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select *
	  into #B92
	  from dbo.PERIOD_FINANCIALS pf 
	 where pf.DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		  , a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
		  , a.DATA_ID, (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0)) as AMOUNT, ' ' as CALCULATION_DIAGRAM
		  , a.SOURCE_CURRENCY, a.AMOUNT_TYPE
		  , a.AMOUNT as AMOUNT104, b.AMOUNT as AMOUNT92, c.AMOUNT as AMOUNT190
	  into #B
	  from #B104 a
	   left join #B92 b on b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_YEAR = a.PERIOD_YEAR 
						and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
						and b.PERIOD_END_DATE = a.PERIOD_END_DATE
	   left join #B190 c on c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_YEAR = a.PERIOD_YEAR 
						and c.FISCAL_TYPE = a.FISCAL_TYPE and c.CURRENCY = a.CURRENCY
						and c.PERIOD_END_DATE = a.PERIOD_END_DATE





	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  162 as DATA_ID										-- DATA_ID:162 ROIC
		,  (isnull(a.AMOUNT, 0.0) /  (((isnull(b.AMOUNT, 0.0)) + c.AMOUNT)/2)) as AMOUNT			-- (NINC-FCDP)/QTEL
		,  '(130 � 291 � TTAX)(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') /  (QTLE + LMIN + 190 for Year(' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ')+(QTLE + LMIN + 190 for Prior Year('  + CAST(c.AMOUNT as varchar(32)) + ')/2) ' as CALCULATION_DIAGRAM
		,  c.SOURCE_CURRENCY
		,  c.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	  left join	#B c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR+1 = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(c.AMOUNT, 0.0) <> 0.0	-- Data validation


	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or ZERO data
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
								, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT)
			(
			select GETDATE() as LOG_DATE, 162 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 162:ROIC.  DATA_ID:105 QTEL is NULL or ZERO'
			  from #A a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 162 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 162:ROIC.  DATA_ID:(130 � 291 � TTAX) is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			 
			 -- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 162 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 162:ROIC.  DATA_ID:(QTLE + LMIN + 190) is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			) union (
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 162 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 162:ROIC.  DATA_ID:(130 � 291 � TTAX) no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (
			select GETDATE() as LOG_DATE, 162 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 162:ROIC.  DATA_ID:(QTLE + LMIN + 190) no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #B190
	drop table #B104
	drop table #B92
GO


--exec AIMS_Calc_162 '223340'
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00192'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())