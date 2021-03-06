
/****** Object:  StoredProcedure [dbo].[AIMS_Calc_186]    Script Date: 01/06/2014 11:57:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
ALTER procedure [dbo].[AIMS_Calc_186](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write to the CALC_LOG table, 'N' = do not write.
,	@STAGE				char		= 'N'
)
as

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()


	  select  pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS_SECURITY_STAGE pf  with (nolock)		
	  where 1=0
	  
	  select pf.* 
	  into #B
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  select pf.* 
	  into #C
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  
	-- Get the data
	

	
	-- Get the sub query data into a temp table
	--figure out the most recent net debt date and the period type of that data id
	declare @PERIOD_TYPE varchar(2)
	declare @PERIOD_END_DATE datetime

--GH - 2013-11-19 - Start Modification 
--Replaced the following query - Request ID : 21602 RE: AIMS question: Grupo Aeromexico Upside 
	/*
	select @PERIOD_TYPE = MIN(period_type), 
		@PERIOD_END_DATE = max(period_end_date) 
	from dbo.PERIOD_FINANCIALS pf  -- to find closest end_date to getdate
	where DATA_ID = 190  --Net Debt		
	and pf.ISSUER_ID = @ISSUER_ID
	and period_end_date < getdate() 
	and pf.FISCAL_TYPE = 'FISCAL'
	and pf.AMOUNT_TYPE = 'ACTUAL'
	group by PERIOD_END_DATE
	*/
	IF @STAGE = 'Y'
	BEGIN
	
	
	select @PERIOD_TYPE = MIN(period_type), 
		@PERIOD_END_DATE = max(period_end_date) 
		from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf  with (nolock)  -- to find closest end_date to getdate 
		where DATA_ID = 190  --Net Debt
		and pf.ISSUER_ID = @ISSUER_ID
		and period_end_date < (getdate() + 365)
		and pf.FISCAL_TYPE = 'FISCAL'
		and pf.PERIOD_TYPE = 'A'
		group by PERIOD_END_DATE
--GH - 2013-11-19 - End Modification 

		insert into #A
	select distinct pf.* 
	  
	  from dbo.PERIOD_FINANCIALS_SECURITY_STAGE pf  with (nolock)
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'C'

	insert into #B
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	 where DATA_ID = 190					-- Net Debt
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = @PERIOD_TYPE
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	   and pf.FISCAL_TYPE = 'FISCAL'


	insert   into #C
	 select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	 where DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = @PERIOD_TYPE
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	   and pf.FISCAL_TYPE = 'FISCAL'
	   
	END
	ELSE
	BEGIN
		select @PERIOD_TYPE = MIN(period_type), 
		@PERIOD_END_DATE = max(period_end_date) 
		from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf  with (nolock)  -- to find closest end_date to getdate 
		where DATA_ID = 190  --Net Debt
		and pf.ISSUER_ID = @ISSUER_ID
		and period_end_date < (getdate() + 365)
		and pf.FISCAL_TYPE = 'FISCAL'
		and pf.PERIOD_TYPE = 'A'
		group by PERIOD_END_DATE
--GH - 2013-11-19 - End Modification 

	insert  into #A
	select distinct pf.* 
	 
	  from dbo.PERIOD_FINANCIALS_SECURITY_MAIN pf  with (nolock)
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'C'

	insert into #B
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 190					-- Net Debt
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = @PERIOD_TYPE
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	   and pf.FISCAL_TYPE = 'FISCAL'

	insert  into #C
	 select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = @PERIOD_TYPE
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	   and pf.FISCAL_TYPE = 'FISCAL'
	END
	   

------------------ Inserting Current Data ------------------------------	   
	-- Add the data to the table
	BEGIN TRAN T1
	IF @STAGE = 'Y'
	BEGIN
		insert into PERIOD_FINANCIALS_SECURITY_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) 
		select distinct isnull(a.ISSUER_ID, ' '), a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  186 as DATA_ID										-- DATA_ID:186 Enterprise Value
		,  (isnull(a.AMOUNT, 0.0) + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0)) as AMOUNT						-- 185 + Net Debt + LMIN
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

--Removing SINV per discussion with AG - GH 1/6/2014
--		,  (isnull(a.AMOUNT, 0.0) + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) - isnull(d.AMOUNT, 0.0)) as AMOUNT						-- 185 + Net Debt + LMIN - SINV
--		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') - 69 (' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #B b																			-- Must be one Issuer level Data item
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = b.ISSUER_ID 
	 inner join	#A a on a.SECURITY_ID = sb.SECURITY_ID and a.CURRENCY = b.CURRENCY		-- Must be a 185
					 and a.DATA_SOURCE = b.DATA_SOURCE
	  left join	#C c on c.ISSUER_ID = b.ISSUER_ID and c.CURRENCY = b.CURRENCY
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
	
	--Removing SINV per discussion with AG - GH 1/6/2014
	  --left join	#D d on d.ISSUER_ID = b.ISSUER_ID and d.CURRENCY = b.CURRENCY					
			--		and d.DATA_SOURCE = b.DATA_SOURCE and d.PERIOD_TYPE = b.PERIOD_TYPE
			--		and d.PERIOD_YEAR = b.PERIOD_YEAR and d.FISCAL_TYPE = b.FISCAL_TYPE
	
	 where 1=1 	  
	
	END
	ELSE
	BEGIN
	insert into PERIOD_FINANCIALS_SECURITY_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE) 
	select distinct isnull(a.ISSUER_ID, ' '), a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  186 as DATA_ID										-- DATA_ID:186 Enterprise Value
		,  (isnull(a.AMOUNT, 0.0) + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0)) as AMOUNT						-- 185 + Net Debt + LMIN
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

--Removing SINV per discussion with AG - GH 1/6/2014
--		,  (isnull(a.AMOUNT, 0.0) + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) - isnull(d.AMOUNT, 0.0)) as AMOUNT						-- 185 + Net Debt + LMIN - SINV
--		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') - 69 (' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #B b																			-- Must be one Issuer level Data item
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = b.ISSUER_ID 
	 inner join	#A a on a.SECURITY_ID = sb.SECURITY_ID and a.CURRENCY = b.CURRENCY		-- Must be a 185
					 and a.DATA_SOURCE = b.DATA_SOURCE
	  left join	#C c on c.ISSUER_ID = b.ISSUER_ID and c.CURRENCY = b.CURRENCY
					and c.DATA_SOURCE = b.DATA_SOURCE and c.PERIOD_TYPE = b.PERIOD_TYPE
					and c.PERIOD_YEAR = b.PERIOD_YEAR and c.FISCAL_TYPE = b.FISCAL_TYPE
	
	--Removing SINV per discussion with AG - GH 1/6/2014
	  --left join	#D d on d.ISSUER_ID = b.ISSUER_ID and d.CURRENCY = b.CURRENCY					
			--		and d.DATA_SOURCE = b.DATA_SOURCE and d.PERIOD_TYPE = b.PERIOD_TYPE
			--		and d.PERIOD_YEAR = b.PERIOD_YEAR and d.FISCAL_TYPE = b.FISCAL_TYPE
	
	 where 1=1 	  
	-- order by a.ISSUER_ID, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	END
	COMMIT TRAN T1
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
			 
			 ) 

--Removing SINV per discussion with AG - GH 1/6/2014 
			-- union (	
			
			--select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
			--	,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
			--	, 'ERROR calculating 186 Enterprise Value.  DATA_ID:51 SCSI is missing for current'
			--  from #D a
			--   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			--  left join	#A b on b.SECURITY_ID = sb.SECURITY_ID	 					
			--				and b.CURRENCY = a.CURRENCY
			--				 and b.DATA_SOURCE = a.DATA_SOURCE
			-- where 1=1 and b.ISSUER_ID is NULL
			 
			-- ) 
			 
			 union (	
			
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
			  
			 ) /*union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:51 SCSI no data for current' as TXT
			  from (select COUNT(*) CNT from #D having COUNT(*) = 0) z
			)*/
		END

	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Clean up
	drop table #A
	drop table #B
	drop table #C

--Removing SINV per discussion with AG - GH 1/6/2014
--	drop table #D
	
	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	
	--------------------- Logic for Annual Data ----------------------
print 'ANNUAL logic for Calc 186'	
	-- Get the data
	
	  select pf.* 
	  into #A1
	  from dbo.PERIOD_FINANCIALS_SECURITY_STAGE pf  with (nolock)
	  Where 1=0
	  
	  select pf.* 
	  into #B1
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	  select pf.* 
	  into #C1
	  from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	  where 1=0
	  
	
	if @stage = 'Y'
	BEGIN
		insert into #A1
		select distinct pf.* 
	    from dbo.PERIOD_FINANCIALS_SECURITY_STAGE pf  with (nolock)
	    inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	    where DATA_ID = 185			--Market Capitalization
	    and sb.ISSUER_ID = @ISSUER_ID
--	   and pf.PERIOD_TYPE = 'A'

		insert into #B1
		select pf.* 
	    from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		where DATA_ID = 190					-- Net Debt
	    and pf.ISSUER_ID = @ISSUER_ID
--	   and pf.PERIOD_TYPE = 'A'	                          
	   
  	   insert  into #C1
	   select pf.* 
	   from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
	   where DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
 --      and pf.PERIOD_TYPE = 'A'

	END
	ELSE
	BEGIN
	
	insert into #A1
	select distinct pf.* 
	  from dbo.PERIOD_FINANCIALS_SECURITY_MAIN pf  with (nolock)
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
--	   and pf.PERIOD_TYPE = 'A'

	insert into #B1	
	select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 190					-- Net Debt
	   and pf.ISSUER_ID = @ISSUER_ID
--	   and pf.PERIOD_TYPE = 'A'	                          
	   
	insert into #C1	
	 select pf.* 
	  from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
	 where DATA_ID = 92				-- LMIN
	   and pf.ISSUER_ID = @ISSUER_ID
 --      and pf.PERIOD_TYPE = 'A'
	  END
	
	   



	-- Add the data to the table
	begin tran t1
	if @stage = 'Y'
	begin
			insert into PERIOD_FINANCIALS_SECURITY_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
			select isnull(a.ISSUER_ID, ' '), a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  186 as DATA_ID										-- DATA_ID:186 Enterprise Value
		,  (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0)) as AMOUNT						-- 185 + 190 + LMIN
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

--Removing SINV per discussion with AG - GH 1/6/2014
--		,  (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) - isnull(d.AMOUNT, 0.0)) as AMOUNT						-- 185 + 190 + LMIN - SINV
--		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') - 69(' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

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

--Removing SINV per discussion with AG - GH 1/6/2014
	  --left join	#D1 d on d.ISSUER_ID = sb.ISSUER_ID and d.CURRENCY = b.CURRENCY					
			--		and d.DATA_SOURCE = b.DATA_SOURCE and d.PERIOD_TYPE = b.PERIOD_TYPE
			--		and d.PERIOD_YEAR = b.PERIOD_YEAR and d.FISCAL_TYPE = b.FISCAL_TYPE
	 where 1=1 	  
--	 order by a.ISSUER_ID, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.DATA_SOURCE, a.FISCAL_TYPE, a.CURRENCY
	
	
	end
	else
	begin
	insert into PERIOD_FINANCIALS_SECURITY_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select isnull(a.ISSUER_ID, ' '), a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE
		,  a.FISCAL_TYPE, a.CURRENCY
		,  186 as DATA_ID										-- DATA_ID:186 Enterprise Value
		,  (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0)) as AMOUNT						-- 185 + 190 + LMIN
		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

--Removing SINV per discussion with AG - GH 1/6/2014
--		,  (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0) - isnull(d.AMOUNT, 0.0)) as AMOUNT						-- 185 + 190 + LMIN - SINV
--		,  '185(' + CAST(a.AMOUNT as varchar(32)) + ') + 190 (' + CAST(isnull(b.AMOUNT, 0.0) as varchar(32)) + ') + 92 (' + CAST(isnull(c.AMOUNT, 0.0) as varchar(32)) + ') - 69(' + CAST(isnull(d.AMOUNT, 0.0) as varchar(32)) + ')' as CALCULATION_DIAGRAM

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

--Removing SINV per discussion with AG - GH 1/6/2014
	  --left join	#D1 d on d.ISSUER_ID = sb.ISSUER_ID and d.CURRENCY = b.CURRENCY					
			--		and d.DATA_SOURCE = b.DATA_SOURCE and d.PERIOD_TYPE = b.PERIOD_TYPE
			--		and d.PERIOD_YEAR = b.PERIOD_YEAR and d.FISCAL_TYPE = b.FISCAL_TYPE
	 where 1=1 	  
--	 order by a.ISSUER_ID, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.DATA_SOURCE, a.FISCAL_TYPE, a.CURRENCY
	end
	commit tran t1
	
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

			 ) 

--Removing SINV per discussion with AG - GH 1/6/2014			 
			-- union (	
			
			--select GETDATE() as LOG_DATE, 186 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
			--	,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
			--	, 'ERROR calculating 186 Enterprise Value.  DATA_ID:69 SINV is missing'
			--  from #D1 a
			--   inner join dbo.GF_SECURITY_BASEVIEW sb on a.ISSUER_ID = sb.ISSUER_ID 
			--  left join	#A1 b on b.SECURITY_ID = sb.SECURITY_ID	 					
			--				and b.CURRENCY = a.CURRENCY
			--				and b.DATA_SOURCE = a.DATA_SOURCE	
			--				and a.PERIOD_TYPE = b.PERIOD_TYPE				
			-- where 1=1 and b.ISSUER_ID is NULL

			--) 
			union (		
			 
			-- ERROR - No data at all available
				
			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 186 Enterprise Value.  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A1 having COUNT(*) = 0) z
			  
			 ) union (	
			
			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:190 Net Debt no data' as TXT
			  from (select COUNT(*) CNT from #B1 having COUNT(*) = 0) z 
			  
			  ) union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:92 LMIN no data' as TXT
			  from (select COUNT(*) CNT from #C1 having COUNT(*) = 0) z
			  
			 ) /*union (	

			select GETDATE() as LOG_DATE, 186 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'WARNING calculating 186 Enterprise Value.  DATA_ID:69 SINV no data' as TXT
			  from (select COUNT(*) CNT from #D1 having COUNT(*) = 0) z
			)*/
		END


	print ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Clean up
	drop table #A1
	drop table #B1
	drop table #C1
	
--Removing SINV per discussion with AG - GH 1/6/2014
--	drop table #D1
