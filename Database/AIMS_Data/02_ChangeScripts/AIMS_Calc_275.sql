------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:275 Non-Interest Earning Assets
--
--			(APPN + AGWI+AINT+SINV+SOLA+SOAT)
--
-- Author:	Aniket
-- Date:	July 13, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_275', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_275;
GO

CREATE procedure [dbo].[AIMS_Calc_275](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data
	select pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf  
	 -- inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 60			-- APPN
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 61					-- AGWI
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	  
	   
	 select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 62				-- AINT
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	  select pf.* 
	  into #D
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 69				-- SINV
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	   
	    select pf.* 
	  into #E
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 72				--SOLA
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A' 
	   
	     select pf.* 
	  into #F
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 74				-- SOAT
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   
	   
	   
	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  275 as DATA_ID										-- DATA_ID:275 Non-Interest Earning Assets
		,  (isnull(a.AMOUNT, 0.0) + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) + isnull(d.AMOUNT, 0.0) + isnull(e.AMOUNT, 0.0) + isnull(f.AMOUNT, 0.0)) as AMOUNT	-- ( 60+61+62+69+72+74) (APPN + AGWI+AINT+SINV+SOLA+SOAT)
		,  'APPN(' + CAST(isnull(a.AMOUNT, 0.0) as varchar(32)) + ') + AGWI (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + AINT (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) 
		   + ') + SINV (' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ') + SOLA (' + CAST(isnull(e.AMOUNT, 0.0) as varchar(32)) + ') + SOAT (' + CAST(isnull(f.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	  left join	#C c on c.ISSUER_ID = a.ISSUER_ID 
					and c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_TYPE = a.PERIOD_TYPE
					and c.PERIOD_YEAR = a.PERIOD_YEAR and c.FISCAL_TYPE = a.FISCAL_TYPE
					and c.CURRENCY = a.CURRENCY
	  left join	#D d on d.ISSUER_ID = a.ISSUER_ID 
					and d.DATA_SOURCE = a.DATA_SOURCE and d.PERIOD_TYPE = a.PERIOD_TYPE
					and d.PERIOD_YEAR = a.PERIOD_YEAR and d.FISCAL_TYPE = a.FISCAL_TYPE
					and d.CURRENCY = a.CURRENCY
	  left join	#E e on e.ISSUER_ID = a.ISSUER_ID 
					and e.DATA_SOURCE = a.DATA_SOURCE and e.PERIOD_TYPE = a.PERIOD_TYPE
					and e.PERIOD_YEAR = a.PERIOD_YEAR and e.FISCAL_TYPE = a.FISCAL_TYPE
					and e.CURRENCY = a.CURRENCY
	  left join	#F f on f.ISSUER_ID = a.ISSUER_ID 
					and f.DATA_SOURCE = a.DATA_SOURCE and f.PERIOD_TYPE = a.PERIOD_TYPE
					and f.PERIOD_YEAR = a.PERIOD_YEAR and f.FISCAL_TYPE = a.FISCAL_TYPE
					and f.CURRENCY = a.CURRENCY
	 			
	 where 1=1 	  
	 order by a.ISSUER_ID, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			
			 (
					-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 275 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 275 Non-Interest Earning Assets.  DATA_ID:60 APPN is missing'
			  from #B a
			  left join	#A b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  

			) union (	
			
			select GETDATE() as LOG_DATE, 275 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:61 AGWI is missing'
			  from #A a
			  left join	#B b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	
			
			select GETDATE() as LOG_DATE, 275 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:62 AINT is missing'
			  from #A a
			  left join	#C b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (
			
			select GETDATE() as LOG_DATE, 275 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:69 SINV is missing'
			  from #A a
			  left join	#D b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	
			
			select GETDATE() as LOG_DATE, 275 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:72 SOLA is missing'
			  from #A a
			  left join	#E b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (
			
			select GETDATE() as LOG_DATE, 275 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:74 SOAT is missing'
			  from #A a
			  left join	#F b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (

			
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 275 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 275 Non-Interest Earning Assets.  DATA_ID:60 APPN no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			  
			 ) union (	
			
			select GETDATE() as LOG_DATE, 275 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:61 AGWI no data' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			  
			 ) union (	

			select GETDATE() as LOG_DATE, 275 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:62 AINT no data' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z 
			  
			  ) union (	

			select GETDATE() as LOG_DATE, 275 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:69 SINV no data' as TXT
			  from (select COUNT(*) CNT from #D having COUNT(*) = 0) z
			
			
			 ) union (	

			select GETDATE() as LOG_DATE, 275 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:72 SOLA no data' as TXT
			  from (select COUNT(*) CNT from #E having COUNT(*) = 0) z 
			  
			  ) union (	

			select GETDATE() as LOG_DATE, 275 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 275 Non-Interest Earning Assets.  DATA_ID:74 SOAT no data' as TXT
			  from (select COUNT(*) CNT from #F having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
    drop table #E
    drop table #F
	
	