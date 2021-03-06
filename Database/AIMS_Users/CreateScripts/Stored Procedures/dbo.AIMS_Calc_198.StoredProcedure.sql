/****** Object:  StoredProcedure [dbo].[AIMS_Calc_198]    Script Date: 03/08/2013 11:20:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[AIMS_Calc_198](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write errors to the CALC_LOG table.
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 186			--Enterprise Value
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and period_type = 'C'

 ---- Total amount for all the fiscal quarters within an year --- 

	select sum(f.amount)as AMOUNT, f.ISSUER_ID,f.FISCAL_TYPE,f.COA_TYPE,f.DATA_SOURCE,f.CURRENCY,datepart(year,getdate())as current_year 
	  into #B         
	  from (select * 
			  from dbo.PERIOD_FINANCIALS pf
			 where pf.DATA_ID = 130 --EBITDA
			   and pf.FISCAL_TYPE = 'FISCAL'
			   and pf.PERIOD_TYPE like 'Q%'
			   and pf.PERIOD_END_DATE > GETDATE()                                      -- previous quarter from today
			   and pf.PERIOD_END_DATE < DATEADD( month, 12, getdate())   -- only 4 quarters
			   and (@ISSUER_ID is null or @ISSUER_ID = pf.ISSUER_ID)                 
			) f
	 group by f.issuer_id, f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
	having count(distinct PERIOD_TYPE) = 4  

	
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  198 as DATA_ID										-- DATA_ID:198 Forward EV/EBITDA
		,  a.AMOUNT /b.AMOUNT as AMOUNT						-- Current Enterprise Value* / Sum of next 4 quarters EBITDA (130)**
		,  'EntVal(' + CAST(a.AMOUNT as varchar(32)) + ') / sum of 4 quarters(' + CAST(b.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE					
					and b.CURRENCY = a.CURRENCY
	 where 1=1 	  
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	-- order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY


	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 197 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 198 Forward EV/EBITDA. DATA_ID:130 EBITDA is NULL or ZERO'
			  from #B a
			  where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	  

			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 198 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 198 Forward EV/EBITDA.  DATA_ID:130 EBITDA is missing' as TXT
			   from #A a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE					
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  

			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 198 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 198 Forward EV/EBITDA.  DATA_ID:186 is missing' as TXT
			   from #B a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = a.ISSUER_ID
			 inner join	#A b on b.SECURITY_ID = sb.SECURITY_ID
							and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE					
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 198 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 198 Forward EV/EBITDA .  DATA_ID:186 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z

			) union (	

			select GETDATE() as LOG_DATE, 198 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 198 Forward EV/EBITDA.  DATA_ID:130 EBITDA  no data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
GO
