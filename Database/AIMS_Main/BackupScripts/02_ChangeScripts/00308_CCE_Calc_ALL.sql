set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00307'
declare @CurrentScriptVersion as nvarchar(100) = '00308'

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
-- The seven CCE calculations have been pulled together into one stored procedure
-- This single procedure executes more quickly then the individual procedures.
------------------------------------------------------------------------
--
--
-- Purpose:	This procedure calculates the value for DATA_ID:164 P/BV 
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
--
--
-- Author:	David Muench
-- Date:	Sept 6, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'CCE_Calc_ALL', 'P' ) IS NOT NULL 
DROP PROCEDURE CCE_Calc_ALL;
GO

CREATE procedure [dbo].[CCE_Calc_ALL](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Used for calculating elapsed time
	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

	-- Get the data
	select pf.* 
	  into #A164
	  from dbo.PERIOD_FINANCIALS pf  
	   inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and pf.PERIOD_TYPE = 'A'
	 

	select cce.* 
	  into #B164
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 1			-- BVPS
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  b.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  164 as ESTIMATE_ID										-- DATA_ID:166 P/E 
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 185/NINC  
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A164 a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B164 b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	

	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 164 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 164 P/BV .  ESTIMATE_ID:1 is NULL or ZERO'
			  from #B164 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 164 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 164 P/BV .  DATA_ID:185 is missing' as TXT
			  from #A164 a
			  left join	#B164 b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 164 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 164 P/BV .  ESTIMATE_ID:1 is missing' as TXT
			  from #B164 a
			  left join	#A164 b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 164 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 164 P/BV .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A164 having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 164 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 164 P/BV .  ESTIMATE_ID:1 no data' as TXT
			  from (select COUNT(*) CNT from #B164 having COUNT(*) = 0) z
			)
		END
	-- Clean up
	drop table #A164
	drop table #B164
	

------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:166 P/E 
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
------------------------------------------------------------------------
	-- Get the data
	select pf.* 
	  into #A166
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and pf.PERIOD_TYPE = 'A'

	select cce.* 
	  into #B166
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 11			-- Net Income
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  b.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  166 as ESTIMATE_ID										-- DATA_ID:166 P/E 
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 185/NINC  
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A166 a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B166 b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY

	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 166 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 166 P/E  .  ESTIMATE_ID:11 is NULL or ZERO'
			  from #B166 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error	
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 166 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 166 P/E  .  DATA_ID:185 is missing' as TXT
			  from #A166 a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B166 b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 166 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 166 P/E  .  ESTIMATE_ID:11 is missing' as TXT
			  from #B166 a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#A166 b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 166 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 166 P/E  .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A166 having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 166 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 166 P/E  .  ESTIMATE_ID:11 no data' as TXT
			  from (select COUNT(*) CNT from #B166 having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A166
	drop table #B166

------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:170 P/Revenue  
--
--			185/RTLR
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
------------------------------------------------------------------------

	-- Get the data
	select pf.* 
	  into #A170
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and pf.PERIOD_TYPE = 'A'

	select cce.* 
	  into #B170
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 17 			-- Revenue
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  b.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  170 as ESTIMATE_ID										-- DATA_ID:170 P/Revenue  
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 185/NINC  
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A170 a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B170 b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE 
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation

	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 170 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 170 P/Revenue.  ESTIMATE_ID:17 Revenue is NULL or ZERO'
			  from #B170 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'

			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 170 as DATA_ID, sb.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 170 P/Revenue.  ESTIMATE_ID:17 Revenue is missing' as TXT
			  from #A170 a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B170 b on b.ISSUER_ID = sb.ISSUER_ID  --and b.AMOUNT_TYPE = a.AMOUNT_TYPE
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 170 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 170 P/Revenue.  DATA_ID:185 is missing' as TXT
			  from #B170 a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = a.ISSUER_ID   
			  left join	#A170 b on b.SECURITY_ID = sb.SECURITY_ID --and b.AMOUNT_TYPE = a.AMOUNT_TYPE
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 170 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 170 P/Revenue.  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A170 having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 170 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 170 P/Revenue.  ESTIMATE_ID:17 Revenue no data' as TXT
			  from (select COUNT(*) CNT from #B170 having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A170
	drop table #B170
	
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:171 P/CE 
--
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
--
-- Author:	David Muench
-- Date:	Sept 6, 2012
------------------------------------------------------------------------

	-- Get the data
	select pf.* 
	  into #A171
	  from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and pf.PERIOD_TYPE = 'A'

	select cce.* 
	  into #B171
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 3 			-- CFPS
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  b.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  171 as ESTIMATE_ID										-- DATA_ID:171
		,  a.AMOUNT / b.AMOUNT as AMOUNT			
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A171 a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B171 b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 171 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 171 P/CE .  ESTIMATE_ID:3 CFPS is NULL or ZERO'
			  from #B171 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 171 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 171 P/CE .  DATA_ID:185 is missing' as TXT
			  from #A171 a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B171 b on b.ISSUER_ID = sb.ISSUER_ID  
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 171 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 171 P/CE .  ESTIMATE_ID:3 CFPS is missing' as TXT
			  from #B171 a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#A171 b on b.ISSUER_ID = sb.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 171 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 171 P/CE .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A171 having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 171 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 171 P/CE .  ESTIMATE_ID:3 CFPS no data' as TXT
			  from (select COUNT(*) CNT from #B171 having COUNT(*) = 0) z
			)
		END
		
	-- Clean up
	drop table #A171
	drop table #B171
	
	
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:177 Net Income Growth (YOY) 
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
------------------------------------------------------------------------

	-- Get the data
	select cce.* 
	  into #A177
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 11			-- Net Income
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	select cce.PERIOD_YEAR+1 as PERIOD_YEAR, cce.CURRENCY, cce.DATA_SOURCE, cce.AMOUNT_TYPE
		,  cce.FISCAL_TYPE, cce.PERIOD_TYPE, cce.ISSUER_ID, cce.AMOUNT, cce.PERIOD_END_DATE
	  into #B177
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 11			-- Net Income
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  a.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  177 as ESTIMATE_ID										-- DATA_ID:166 P/E 
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 185/NINC  
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A177 a
	 inner join	#B177 b on b.ISSUER_ID = a.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
	   and b.AMOUNT > 0					-- must not be negative
	   and a.AMOUNT >= 0				-- must not be negative

	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 is NULL or ZERO for prior period'
			  from #B177 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error
	 			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 is missing for the year' as TXT
			  from #A177 a
			  left join	#B177 b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
	  			) union (	

			-- Error conditions - missing data for prior period
			select GETDATE() as LOG_DATE, 177 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 is missing for the prior period' as TXT
			  from #B177 a
			  left join	#A177 b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
	  			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 177 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 177 Net Income Growth (YOY) .  ESTIMATE_ID:11 no data' as TXT
			  from (select COUNT(*) CNT from #A177 having COUNT(*) = 0) z
			) 
		END

	-- Clean up
	drop table #A177
	drop table #B177
	
------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:172 P/E to Growth (PEG) 
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
------------------------------------------------------------------------

	-- Get the data
	select cce.* 
	  into #A172
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 166
	   and @ISSUER_ID = cce.ISSUER_ID
	   and cce.PERIOD_TYPE = 'A'

	print '172 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	select cce.* 
	  into #B172
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = cce.SECURITY_ID
	 where cce.ESTIMATE_ID = 177
	   and @ISSUER_ID = sb.ISSUER_ID
	   and cce.PERIOD_TYPE = 'A'

	print '172 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select a.ISSUER_ID, a.SECURITY_ID, b.DATA_SOURCE
		,  b.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  172 as ESTIMATE_ID										
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A172 a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B172 b on b.ISSUER_ID = sb.ISSUER_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation

	print '172 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 172 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 172 P/E to Growth (PEG) .  ESTIMATE_ID:177 is NULL or ZERO'
			  from #B172 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			 and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 172 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 172 P/E to Growth (PEG) .  ESTIMATE_ID:166 is missing' as TXT
			  from #A172 a
			  left join	#B172 b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 172 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 172 P/E to Growth (PEG) .  ESTIMATE_ID:177 is missing' as TXT
			  from #B172 a
			  left join	#A172 b on b.ISSUER_ID = a.ISSUER_ID 
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 172 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 172 P/E to Growth (PEG) .  ESTIMATE_ID:166 no data' as TXT
			  from (select COUNT(*) CNT from #A172 having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 172 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 172 P/E to Growth (PEG) .  ESTIMATE_ID:177 no data' as TXT
			  from (select COUNT(*) CNT from #B172 having COUNT(*) = 0) z
			)
		END

	print '172 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
		
	-- Clean up
	drop table #A172
	drop table #B172

------------------------------------------------------------------------
-- Purpose:	This procedure calculates the value for DATA_ID:192 Dividend Yield
--			This calculation is for CURRENCT_CONSENSUS_ESTIMATES
--
------------------------------------------------------------------------

	-- Get the data
	select cce.* 
	  into #A192
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce 
	 where cce.ESTIMATE_ID = 4		-- DPS
	   and (@ISSUER_ID is null or @ISSUER_ID = cce.ISSUER_ID)
	   and cce.PERIOD_TYPE = 'A'

	print '192 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	select pf.* 
	  into #B192
	   from dbo.PERIOD_FINANCIALS pf  
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and (@ISSUER_ID is null or @ISSUER_ID = sb.ISSUER_ID)
	   and pf.PERIOD_TYPE = 'A'

	print '192 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

	-- Add the data to the table
	insert into CURRENT_CONSENSUS_ESTIMATES(ISSUER_ID, SECURITY_ID, DATA_SOURCE
		  , DATA_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , ESTIMATE_ID, AMOUNT, SOURCE_CURRENCY, AMOUNT_TYPE, NUMBER_OF_ESTIMATES, HIGH
		  , LOW, STANDARD_DEVIATION )
	select b.ISSUER_ID, b.SECURITY_ID, b.DATA_SOURCE
		,  a.DATA_SOURCE_DATE, b.PERIOD_TYPE, b.PERIOD_YEAR, b.PERIOD_END_DATE
		,  b.FISCAL_TYPE, b.CURRENCY
		,  192 as ESTIMATE_ID										
		,  a.AMOUNT / b.AMOUNT as AMOUNT						-- 
		,  b.SOURCE_CURRENCY
		,  b.AMOUNT_TYPE
		,  1 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
	  from #A192 a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = a.ISSUER_ID
	 inner join	#B192 b on b.SECURITY_ID = sb.SECURITY_ID and b.DATA_SOURCE = a.DATA_SOURCE 
					and b.PERIOD_TYPE = a.PERIOD_TYPE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
					and b.AMOUNT_TYPE = a.AMOUNT_TYPE
	 where 1=1 	
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation

	print '192 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into dbo.CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 192 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 192 Dividend Yield.  DATA_ID:185 is NULL or ZERO'
			  from #B192 a
			 where isnull(a.AMOUNT, 0.0) = 0.0	-- Data error
			  and a.PERIOD_TYPE = 'A'
			) union (	
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 192 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 192 Dividend Yield.  DATA_ID:124 is missing' as TXT
			  from #A192 a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#B192 b on b.ISSUER_ID = sb.ISSUER_ID
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 192 as DATA_ID, a.ISSUER_ID, a.PERIOD_TYPE
				,  a.PERIOD_YEAR,  a.PERIOD_END_DATE,  a.FISCAL_TYPE,  a.CURRENCY
				, 'ERROR calculating 192 Dividend Yield.  DATA_ID:185 is missing' as TXT
			  from #B192 a
			  inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			  left join	#A192 b on b.ISSUER_ID = sb.ISSUER_ID  
							and b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_TYPE = a.PERIOD_TYPE
							and b.PERIOD_YEAR = a.PERIOD_YEAR and b.FISCAL_TYPE = a.FISCAL_TYPE
							and b.CURRENCY = a.CURRENCY
			 where 1=1 and b.ISSUER_ID is NULL
			   and a.PERIOD_TYPE = 'A'
			) union (	
			 
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 192 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 192 Dividend Yield.  DATA_ID:124 no data' as TXT
			  from (select COUNT(*) CNT from #A192 having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 192 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 192 Dividend Yield.  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #B192 having COUNT(*) = 0) z
			)
		END

	print '192 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
		
	-- Clean up
	drop table #A192
	drop table #B192
	
	print '192 - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00308'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())