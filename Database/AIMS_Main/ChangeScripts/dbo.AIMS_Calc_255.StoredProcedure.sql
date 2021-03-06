SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Calc_255]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Calc_255]
GO


/****** Object:  StoredProcedure [dbo].[AIMS_Calc_255]    Script Date: 03/18/2014 10:02:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE procedure [dbo].[AIMS_Calc_255](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- Write to the CALC_LOG table, 'N' = do not write.
,	@STAGE				char		= 'N'
)
as

	-- Get the data
		select pf.* 
		into #A
		from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		where 1=0
	
	
	declare @PERIOD_TYPE varchar(2)
	declare @PERIOD_END_DATE datetime

if @stage = 'Y'
Begin
		select @PERIOD_TYPE = MIN(period_type), 
			@PERIOD_END_DATE = max(period_end_date) 
		from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf  with (nolock) -- to find closest end_date to getdate
		where DATA_ID = 51  --Cash and ST Investments		
		and pf.ISSUER_ID = @ISSUER_ID
		and period_end_date < getdate() 
		and pf.FISCAL_TYPE = 'FISCAL'
		and pf.AMOUNT_TYPE = 'ACTUAL'
		group by PERIOD_END_DATE

		insert 	 into #A
		select pf.* 
		from dbo.PERIOD_FINANCIALS_ISSUER_STAGE pf with (nolock)
		where DATA_ID = 51			--Cash and ST Investments	
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = @PERIOD_TYPE
		and pf.FISCAL_TYPE = 'FISCAL'
		and period_end_date = @PERIOD_END_DATE	
	ENd
	else
	begin
		select @PERIOD_TYPE = MIN(period_type), 
		@PERIOD_END_DATE = max(period_end_date) 
		from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf  with (nolock) -- to find closest end_date to getdate
		where DATA_ID = 51  --Cash and ST Investments		
		and pf.ISSUER_ID = @ISSUER_ID
		and period_end_date < getdate() 
		and pf.FISCAL_TYPE = 'FISCAL'
		and pf.AMOUNT_TYPE = 'ACTUAL'
		group by PERIOD_END_DATE

		insert 		into #A
		select pf.* 
		from dbo.PERIOD_FINANCIALS_ISSUER_MAIN pf with (nolock)
		where DATA_ID = 51			--Cash and ST Investments	
		and pf.ISSUER_ID = @ISSUER_ID
		and pf.PERIOD_TYPE = @PERIOD_TYPE
		and pf.FISCAL_TYPE = 'FISCAL'
		and period_end_date = @PERIOD_END_DATE	
	end
 
	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	BEGIN TRAN T1
	if @stage = 'Y'
	Begin
	insert into PERIOD_FINANCIALS_ISSUER_STAGE(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  '' as FISCAL_TYPE, a.CURRENCY
		,  255 as DATA_ID										-- 255 Trailing Cash and ST Investments
		,  a.AMOUNT as AMOUNT						
		,  'Trailing Cash and ST Investments(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a	
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
end
else
begin
	insert into PERIOD_FINANCIALS_ISSUER_MAIN(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  '' as FISCAL_TYPE, a.CURRENCY
		,  255 as DATA_ID										-- 255 Trailing Cash and ST Investments
		,  a.AMOUNT as AMOUNT						
		,  'Trailing Cash and ST Investments(' + CAST(a.AMOUNT as varchar(32)) + ')' as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a	
	 where 1=1 	  
	   and isnull(a.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
end

	COMMIT TRAN T1
	
	if @CALC_LOG = 'Y'
		BEGIN	
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 255 as DATA_ID, a.ISSUER_ID, 'C'
				,  a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 255 Trailing Cash and ST Investments. DATA_ID:51 is NULL or ZERO'
			  from #A a
			where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			
			-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 255 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 255 Trailing Cash and ST Investments.  DATA_ID:51 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) 
		END
		
	-- Clean up
	drop table #A
GO
