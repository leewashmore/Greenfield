------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:293 Reuters Total Shares Oustanding
--
--			If including Preferred shares, then (QTCO + QTPO), otherwise QTCO.
--
-- Author:	David Muench
-- Date:	October 16, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_293', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_293;
GO

create procedure [dbo].[AIMS_Calc_293](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as
	declare @PREFERRED char
	select @PREFERRED = PREFERRED
	  from ISSUER_SHARES_COMPOSITION
	 where ISSUER_ID = @ISSUER_ID
	

	-- Get the data
	select pf.* 
	  into #A
	   from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 106			-- QTCO
	   and pf.ISSUER_ID = @ISSUER_ID
	 order by PERIOD_YEAR

	select pf.* 
	  into #B
	   from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 107			-- QTPO
	   and pf.ISSUER_ID = @ISSUER_ID
	 order by PERIOD_YEAR


	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  293 as DATA_ID							-- DATA_ID:293
		,  case when @PREFERRED = 'Y' 
				then a.AMOUNT + ISNULL(b.AMOUNT, 0.0)
				else a.AMOUNT end as AMOUNT						-- 
		,  case when @PREFERRED = 'Y' 
				then 'QTCO(' + cast(a.AMOUNT as varchar(32)) + ') + QTPO(' + cast(ISNULL(b.AMOUNT, 0.0) as varchar(32)) + ')'
				else 'QTCO(' + cast(a.AMOUNT as varchar(32)) + ')' end as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR+1 = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 293 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 293 Net Income Growth (YOY). DATA_ID:106 QTCO is missing' as TXT
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR+1 = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

  			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 293 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 293 Reuters Total Shares Oustanding. DATA_ID:107 QTPO is missing' as TXT
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR+1 and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL

  			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 293 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 293 Reuters Total Shares Oustanding DATA_ID:106 QTCO no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
	drop table #B
	



