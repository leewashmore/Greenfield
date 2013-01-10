------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:186 Enterprise Value
--
--			185 + STLD + LMIN –SCSI
--
-- Author:	Shivani
-- Date:	July 13, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_186', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_186;
GO

CREATE procedure [dbo].[AIMS_Calc_186](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write to the CALC_LOG table, 'N' = do not write.
)
as

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

	-- Get the data
	select distinct pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'C'

	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Get the sub query data into a temp table
	select issuer_id, PERIOD_TYPE, FISCAL_TYPE, CURRENCY, DATA_SOURCE, MAX(PERIOD_END_DATE) as PERIOD_END_DATE 
	  into #Bb
	  from PERIOD_FINANCIALS 
     where DATA_ID = 90					-- STLD
	   and PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	and ISSUER_ID = @ISSUER_ID
	
	  group by issuer_id, PERIOD_TYPE, FISCAL_TYPE, CURRENCY, DATA_SOURCE
	select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join #Bb a on a.ISSUER_ID = pf.ISSUER_ID 
					and a.PERIOD_TYPE = pf.PERIOD_TYPE and a.CURRENCY = pf.CURRENCY
					and a.DATA_SOURCE = pf.DATA_SOURCE and a.PERIOD_END_DATE = pf.PERIOD_END_DATE
	 where DATA_ID = 90					-- STLD
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and pf.FISCAL_TYPE = 'FISCAL'
       and pf.AMOUNT_TYPE = 'ACTUAL'	                          
	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Get the sub-query data into a temp table
	select issuer_id, PERIOD_TYPE, FISCAL_TYPE, CURRENCY, DATA_SOURCE, MAX(PERIOD_END_DATE) as PERIOD_END_DATE 
	  into #Cc
	  from PERIOD_FINANCIALS 
     where DATA_ID = 92					-- STLD
	   and PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and AMOUNT_TYPE = 'ACTUAL'	                          
	   and ISSUER_ID = @ISSUER_ID
	 group by issuer_id, PERIOD_YEAR, PERIOD_TYPE, FISCAL_TYPE, CURRENCY, DATA_SOURCE
	 
	 
	 select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join #Cc a on a.ISSUER_ID = pf.ISSUER_ID 
					and a.PERIOD_TYPE = pf.PERIOD_TYPE and a.CURRENCY = pf.CURRENCY
					and a.DATA_SOURCE = pf.DATA_SOURCE and a.PERIOD_END_DATE = pf.PERIOD_END_DATE
	 where DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and pf.FISCAL_TYPE = 'FISCAL'
	   and pf.AMOUNT_TYPE = 'ACTUAL'	
	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Get the sub-query intoa temp table
	select issuer_id, PERIOD_TYPE, FISCAL_TYPE, CURRENCY, DATA_SOURCE, MAX(PERIOD_END_DATE) as PERIOD_END_DATE 
	  into #Dd
	  from PERIOD_FINANCIALS 
     where DATA_ID = 51					-- SCSI
	   and PERIOD_TYPE = 'A'
	   and ISSUER_ID = @ISSUER_ID
	 group by issuer_id, PERIOD_TYPE, FISCAL_TYPE, CURRENCY, DATA_SOURCE
	 
	  select pf.* 
	  into #D
	  from dbo.PERIOD_FINANCIALS pf 
	 inner join #Dd a on a.ISSUER_ID = pf.ISSUER_ID 
					and a.PERIOD_TYPE = pf.PERIOD_TYPE and a.CURRENCY = pf.CURRENCY
					and a.DATA_SOURCE = pf.DATA_SOURCE and a.PERIOD_END_DATE = pf.PERIOD_END_DATE
	 where DATA_ID = 51				
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
--       and AMOUNT_TYPE = 'ACTUAL'	
	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

------------------ Inserting Current Data ------------------------------	   
	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct isnull(a.ISSUER_ID, ' '), a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  186 as DATA_ID										-- DATA_ID:186 Enterprise Value
		,  (isnull(a.AMOUNT, 0.0) + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) - isnull(d.AMOUNT, 0.0)) as AMOUNT						-- 185 + STLD + LMIN –SCSI
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + STLD (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + LMIN (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') - SCSI (' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #B b																			-- Must be one Issuer level Data item
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = b.ISSUER_ID 
	 inner join	#A a on a.SECURITY_ID = sb.SECURITY_ID and a.CURRENCY = b.CURRENCY		-- Must be a 185
					 and a.DATA_SOURCE = a.DATA_SOURCE
	  left join	#C c on c.ISSUER_ID = b.ISSUER_ID and c.CURRENCY = b.CURRENCY
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
	  left join	#D d on d.ISSUER_ID = b.ISSUER_ID and d.CURRENCY = b.CURRENCY					
					and d.DATA_SOURCE = b.DATA_SOURCE and d.PERIOD_TYPE = b.PERIOD_TYPE
					and d.PERIOD_YEAR = b.PERIOD_YEAR and d.FISCAL_TYPE = b.FISCAL_TYPE
	 where 1=1 	  
	-- order by a.ISSUER_ID, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(			

			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID: 92 LMIN is missing for current'
			  from #C a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			  left join	#A b on b.SECURITY_ID = sb.SECURITY_ID					
							and b.CURRENCY = a.CURRENCY
							 and b.DATA_SOURCE = a.DATA_SOURCE
			 where 1=1 and b.ISSUER_ID is NULL
			 
			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID:51 SCSI is missing for current'
			  from #D a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			  left join	#A b on b.SECURITY_ID = sb.SECURITY_ID	 					
							and b.CURRENCY = a.CURRENCY
							 and b.DATA_SOURCE = a.DATA_SOURCE
			 where 1=1 and b.ISSUER_ID is NULL
			 
			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID:90 STLD is missing for current'
			  from #B a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			  left join	#A b on b.SECURITY_ID = sb.SECURITY_ID	 					
							and b.CURRENCY = a.CURRENCY
							 and b.DATA_SOURCE = a.DATA_SOURCE
			 where 1=1 and b.ISSUER_ID is NULL

			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:90 STLD no data for current' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z 
			  
			  ) union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:92 LMIN no data for current' as TXT
			  from (select COUNT(*) CNT from #C having COUNT(*) = 0) z
			  
			 ) union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:51 SCSI no data for current' as TXT
			  from (select COUNT(*) CNT from #D having COUNT(*) = 0) z
			)
		END

	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Clean up
	drop table #A
	drop table #B
	drop table #C
	drop table #D
	drop table #Bb
	drop table #Cc
	drop table #Dd
	
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	
	--------------------- Logic for Annual Data ----------------------
print 'ANNUAL logic for Calc 186'	
	-- Get the data
	select distinct pf.* 
	  into #A1
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	select pf.* 
	  into #B1
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 90					-- STLD
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'	                          
	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	 select pf.* 
	  into #C1
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
       and pf.PERIOD_TYPE = 'A'
	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	  select pf.* 
	  into #D1
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 51				-- SCSI
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()


	  select pf.* 
	  into #E1
	  from dbo.PERIOD_FINANCIALS pf 
	 where DATA_ID = 69				-- SINV
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'

	   
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()



	-- Add the data to the table
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select isnull(a.ISSUER_ID, ' '), a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  186 as DATA_ID										-- DATA_ID:186 Enterprise Value
		,  (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) - isnull(d.AMOUNT, 0.0) - isnull(e.AMOUNT, 0.0)) as AMOUNT						-- 185 + STLD + LMIN –SCSI
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + STLD (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + LMIN (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') - SCSI (' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ') - SINV(' + CAST(isnull(e.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A1 a																			-- Must be one Issuer level Data item
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B1 b on b.ISSUER_ID = sb.ISSUER_ID and b.CURRENCY = a.CURRENCY		-- Must be a 185
					and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
	  left join	#C1 c on c.ISSUER_ID = sb.ISSUER_ID and c.CURRENCY = b.CURRENCY
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
	  left join	#D1 d on d.ISSUER_ID = sb.ISSUER_ID and d.CURRENCY = b.CURRENCY					
					and d.DATA_SOURCE = b.DATA_SOURCE and d.PERIOD_TYPE = b.PERIOD_TYPE
					and d.PERIOD_YEAR = b.PERIOD_YEAR and d.FISCAL_TYPE = b.FISCAL_TYPE
	  left join	#E1 e on e.ISSUER_ID = sb.ISSUER_ID and e.CURRENCY = b.CURRENCY					
					and e.DATA_SOURCE = b.DATA_SOURCE and e.PERIOD_TYPE = b.PERIOD_TYPE
					and e.PERIOD_YEAR = b.PERIOD_YEAR and e.FISCAL_TYPE = b.FISCAL_TYPE
	 where 1=1 	  
--	 order by a.ISSUER_ID, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.DATA_SOURCE, a.FISCAL_TYPE, a.CURRENCY

	
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(	
			
			-- Error conditions - missing data 	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID: 185 Market Cap is missing'
			  from #B1 a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			  left join	#A1 b on b.SECURITY_ID = sb.SECURITY_ID					
							and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE	
							and b.PERIOD_TYPE = a.PERIOD_TYPE
			 where 1=1 and b.ISSUER_ID is NULL
			 
			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID: 92 LMIN is missing'
			  from #C1 a
			  left join	#B1 b on b.ISSUER_ID = a.ISSUER_ID
							and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE	
							and b.PERIOD_TYPE = a.PERIOD_TYPE
			 where 1=1 and b.ISSUER_ID is NULL
			 
			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID: 92 LMIN is missing'
			  from #A1 a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B1 b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE	
							and b.PERIOD_TYPE = a.PERIOD_TYPE
			 where 1=1 and b.ISSUER_ID is NULL

			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID:51 SCSI is missing'
			  from #D1 a
			   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			  left join	#A1 b on b.SECURITY_ID = sb.SECURITY_ID	 					
							and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE	
							and a.PERIOD_TYPE = b.PERIOD_TYPE				
			 where 1=1 and b.ISSUER_ID is NULL

			) union (		
			 
			-- ERROR - No data at all available
				
			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A1 having COUNT(*) = 0) z
			  
			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:90 STLD no data' as TXT
			  from (select COUNT(*) CNT from #B1 having COUNT(*) = 0) z 
			  
			  ) union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:92 LMIN no data' as TXT
			  from (select COUNT(*) CNT from #C1 having COUNT(*) = 0) z
			  
			 ) union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:51 SCSI no data' as TXT
			  from (select COUNT(*) CNT from #D1 having COUNT(*) = 0) z
			)
		END


	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Clean up
	drop table #A1
	drop table #B1
	drop table #C1
	drop table #D1
	drop table #E1

-- exec AIMS_Calc_186 182896, 'N'