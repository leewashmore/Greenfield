------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:216 Trailing ROE Growth 
--
--	((Sum of previous 4 quarters NINC*/Previous Annual QTLE**)  / (Sum of  4 quarters NINC prior to previous***/Annual QTLE prior to Previous****)) - 1 
--
-- Author:	Shivani
-- Date:	July 17, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_216', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_216;
GO

CREATE procedure [dbo].[AIMS_Calc_216](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
-- Sum of previous 4 quarters NINC*	
	select pf.* 
	  into #A
	  from dbo.period_financials pf
	 where pf.data_id = 104 -- QTLE
	   and pf.ISSUER_ID = @ISSUER_ID
	   and period_end_date = (select max(period_end_date) from dbo.PERIOD_FINANCIALS pf  -- to find closest end_date to getdate
							   where DATA_ID = 104			
							     and pf.ISSUER_ID = @ISSUER_ID
						         and period_end_date < getdate() )
	
	 ---- Total amount for all the fiscal quarters within an year --- 
	 
	 
-- Previous Annual QTLE**
	select sum(f.amount)as AMOUNT, f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY
	into #B        
         from (select * 
                from dbo.PERIOD_FINANCIALS pf
               where pf.DATA_ID = 44   -- NINC
				 and pf.FISCAL_TYPE = 'FISCAL'
				 and pf.PERIOD_TYPE like 'Q%'
				 and pf.PERIOD_END_DATE < GETDATE()                                      -- previous quarter from today
				 and pf.PERIOD_END_DATE > DATEADD( month, -12, getdate())   -- only 4 quarters
				 and pf.ISSUER_ID = @ISSUER_ID
			  ) f
        group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4  
	
	
--- Sum of  4 quarters NINC prior to previous***							   
    
	select sum(f.amount)as AMOUNT, f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY
	into #C         
         from (select * 
				 from dbo.PERIOD_FINANCIALS pf
				where pf.DATA_ID = 44   -- NINC
				  and pf.FISCAL_TYPE = 'FISCAL'
				  and pf.PERIOD_TYPE like 'Q%'
				  and pf.PERIOD_END_DATE > DATEADD( month, -24, getdate())   -- previous quarter from today
				  and pf.PERIOD_END_DATE < DATEADD( month, -12, getdate())   -- only 4 quarters
				  and pf.ISSUER_ID = @ISSUER_ID
			   ) f
        group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4  

-- Annual QTLE prior to Previous**** 

	select pf.* 
	into #D
	from dbo.PERIOD_FINANCIALS pf
	where pf.data_id = 104
	  and pf.ISSUER_ID = @ISSUER_ID
	  and period_end_date = (select max(PERIOD_END_DATE) from PERIOD_FINANCIALS
							  where DATA_ID = 104
							    and (@ISSUER_ID is null or @ISSUER_ID = ISSUER_ID) 
							    and PERIOD_END_DATE < (select max(period_end_date) from dbo.PERIOD_FINANCIALS pf  -- to find closest end_date to getdate
														where DATA_ID = 104			
							   							  and pf.ISSUER_ID = @ISSUER_ID
														  and period_end_date < getdate()))
	 
	 	
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  216 as DATA_ID										-- DATA_ID:216 Trailing ROE Growth 
		,  ((b.AMOUNT /a.AMOUNT)/(c.amount/d.amount)) -1 as AMOUNT						-- ((Sum of previous 4 quarters NINC*/Previous Annual QTLE**)  / (Sum of  4 quarters NINC prior to previous***/Annual QTLE prior to Previous****)) - 1 
		,  '((Sum Previous 4 quarters(' + CAST(b.AMOUNT as varchar(32)) + ') / Previous Annual QTLE(' + CAST(a.AMOUNT as varchar(32)) + ') / (Sum of 4 quarters NINC prior to prev(' + CAST(c.AMOUNT as varchar(32)) + ')/Annual QTLE prior to prev (' + CAST(d.AMOUNT as varchar(32)) + ') -1)' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE					
					and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 inner join #C c on  c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE					
					and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
     inner join #D d on  d.ISSUER_ID = a.ISSUER_ID 
					and d.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE					
					and d.FISCAL_TYPE = a.FISCAL_TYPE
					and d.CURRENCY = a.CURRENCY					
					
	 where 1=1 	  
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	-- order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 216 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 216 Trailing ROE Growth. DATA_ID:104 Previous Annual QTLE is NULL or ZERO'
			  from #A a
			  where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 216 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 216 Trailing ROE Growth. DATA_ID:44 Sum of 4 quarters NINC prior to previous is NULL or ZERO'
			  from #C a
			  where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	   
			) union (	

			
				-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 216 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 216 Trailing ROE Growth .  DATA_ID:104 Previous Annual QTLE no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 216 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 216 Trailing ROE Growth.  DATA_ID:44 Sum of previous 4 quarters NINC no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			) union ( 
			select GETDATE() as LOG_DATE, 216 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 216 Trailing ROE Growth.  DATA_ID:44 Sum of 4 quarters NINC prior to previous no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			  
			) union (	

			
			select GETDATE() as LOG_DATE, 216 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 216 Trailing ROE Growth .  DATA_ID:104 Annual QTLE prior to Previous no data' as TXT
			  from (select COUNT(*) CNT from #D having COUNT(*) = 0) z
			)
		END	

	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
	



