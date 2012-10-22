------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:280 Forward Book Value

-- Sum of next 4 quarters QTLE**
--
-- Author:	Aniket
-- Date:	August 06, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_280', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_280;
GO

CREATE procedure [dbo].[AIMS_Calc_280](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data


	select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY), DATEPART(YYYY, GETDATE()) as Current_Year
	into #A    
         from (select * 
                       from dbo.PERIOD_FINANCIALS pf
                      where pf.DATA_ID = 104  --QTLE
                        and pf.FISCAL_TYPE = 'FISCAL'
                        and pf.PERIOD_TYPE like 'Q%'
                        and pf.PERIOD_END_DATE > GETDATE()                         -- next quarter from today
                        and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())   -- only 4 quarters
					    and pf.ISSUER_ID = @ISSUER_ID
                       -- order by pf.PERIOD_END_DATE  desc           
                     ) f
        group by f.issuer_id , f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
		 having count(distinct PERIOD_TYPE) = 4   
        
   
    
 -- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE,PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, '', a.COA_TYPE, a.DATA_SOURCE, a.DATA_SOURCE
		, '','C', 0, '01/01/1900'	
		,  a.FISCAL_TYPE, a.CURRENCY
		,  280 as DATA_ID							-- DATA_ID:280 
		,  (a.AMOUNT) as AMOUNT		-- Sum of next 4 quarters QTLE**
		,  '(Next 4 Qtr(' + CAST(a.AMOUNT as varchar(32)) + '))' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL'
	  from #A a
	
---	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.FISCAL_TYPE, a.CURRENCY
	


	if @CALC_LOG = 'Y'
		BEGIN	
		-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
				
			-- ERROR - No data at all available or one of the next quarter is missing
			select GETDATE() as LOG_DATE, 280 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 280 Forward Book Value .  DATA_ID:104 No data or missing Next quarters' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z 
		END	
	 
	 
	-- Clean up
	drop table #A
	
	
	



