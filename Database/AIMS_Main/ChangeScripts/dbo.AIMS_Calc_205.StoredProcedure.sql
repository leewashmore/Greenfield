SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_205](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to CALC_LOG table
,	@STAGE				CHAR		= 'N'
)
as

	  select AMOUNT, ISSUER_ID,FISCAL_TYPE,COA_TYPE,DATA_SOURCE,CURRENCY, DATEPART(YYYY, GETDATE()) as Current_Year
	  into #A   
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  WHERE 1=0
	  
	  select  AMOUNT, ISSUER_ID,FISCAL_TYPE,COA_TYPE,DATA_SOURCE,CURRENCY ,DATEPART(YYYY, GETDATE()) as Current_Year
	  into #B 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  WHERE 1=0


IF @STAGE ='Y'
BEGIN
	-- Get the data
		INSERT into #A  
     	select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY), DATEPART(YYYY, GETDATE()) as Current_Year
	    from (select * 
			  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
			 where pf.DATA_ID = 157   -- Free Cash Flow
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()                         -- Next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())    -- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
            -- order by pf.PERIOD_END_DATE  desc           
			) f
	 group by f.issuer_id , f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4   
 
	INSERT into #B 
	select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY) ,DATEPART(YYYY, GETDATE()) as Current_Year
	  from (select * 
			  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
			 where pf.DATA_ID = 11   -- RTLR
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()                         -- Next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())    -- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			-- order by pf.PERIOD_END_DATE  desc           
			) f
	 group by f.issuer_id , f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4  
END
ELSE
BEGIN
	-- Get the data
	INSERT into #A
	select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY), DATEPART(YYYY, GETDATE()) as Current_Year
	  from (select * 
			  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
			 where pf.DATA_ID = 157   -- Free Cash Flow
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()                         -- Next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())    -- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
            -- order by pf.PERIOD_END_DATE  desc           
			) f
	 group by f.issuer_id , f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4   
 

	INSERT  into #B 
	select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY) ,DATEPART(YYYY, GETDATE()) as Current_Year
	  from (select * 
			  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
			 where pf.DATA_ID = 11   -- RTLR
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()                         -- Next quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())    -- only 4 quarters
			   and pf.ISSUER_ID = @ISSUER_ID
			-- order by pf.PERIOD_END_DATE  desc           
			) f
	 group by f.issuer_id , f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4  
   END   
   
    
 -- Add the data to the table
 BEGIN TRAN T1
 if @stage ='Y'
 BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE,PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, '', a.COA_TYPE, a.DATA_SOURCE, a.DATA_SOURCE
		,  '', 'C', 0, '01/01/1900'	-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  205 as DATA_ID							-- DATA_ID:205 
		,  (a.AMOUNT/b.Amount) as AMOUNT		    -- Sum of next 4 quarters Free Cash Flow (157)**/Next 4 quarters Revenue (RTLR)*
		,  '(Next 4 Qtr(Free cash flow)(' + CAST(a.AMOUNT as varchar(32)) + ') / Next 4 Qtr(RTLR)(' + CAST(b.AMOUNT as varchar(32)) + '))' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL'
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY  
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.Current_Year = a.Current_Year and b.FISCAL_TYPE = a.FISCAL_TYPE
	 where 1=1 
	 and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE,a.FISCAL_TYPE, a.CURRENCY
END
ELSE
BEGIN
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE,PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, '', a.COA_TYPE, a.DATA_SOURCE, a.DATA_SOURCE
		,  '', 'C', 0, '01/01/1900'	-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  205 as DATA_ID							-- DATA_ID:205 
		,  (a.AMOUNT/b.Amount) as AMOUNT		    -- Sum of next 4 quarters Free Cash Flow (157)**/Next 4 quarters Revenue (RTLR)*
		,  '(Next 4 Qtr(Free cash flow)(' + CAST(a.AMOUNT as varchar(32)) + ') / Next 4 Qtr(RTLR)(' + CAST(b.AMOUNT as varchar(32)) + '))' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL'
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID and b.CURRENCY = a.CURRENCY  
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.Current_Year = a.Current_Year and b.FISCAL_TYPE = a.FISCAL_TYPE
	 where 1=1 
	 and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE,a.FISCAL_TYPE, a.CURRENCY
END
	COMMIT TRAN T1

	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 205 as DATA_ID, a.ISSUER_ID, 'C' as PERIOD_TYPE
				, 0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 205 Forward Free Cash Flow Margin . DATA_ID:11 RTLR is NULL or ZERO'
			  from #B a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
				 
			) union (
					
			-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 205 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 205 Forward Free Cash Flow Margin .  DATA_ID:157 Free Cash Flow  No data or missing Next quarters' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z

			) union (	

			-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 205 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 205 Forward Free Cash Flow Margin .  DATA_ID:11 RTLR  No data or missing Next quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
GO
