set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00238'
declare @CurrentScriptVersion as nvarchar(100) = '00239'

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
-- Purpose:	This procedure calculates the value for DATA_ID:213 Trailing Revenue Growth

-- (Sum of previous 4 quarters RTLR*/ Sum of 4 quarters RTLR prior to previous 4**) -1
--
-- Author:	Aniket
-- Date:	July 20, 2012
------------------------------------------------------------------------
IF OBJECT_ID ( 'AIMS_Calc_213', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_213;
GO

CREATE procedure [dbo].[AIMS_Calc_213](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as

	-- Get the data

	select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY), DATEPART(YYYY, GETDATE()) as Current_Year
	into #A    
         from (select * 
                       from dbo.PERIOD_FINANCIALS pf
                      where pf.DATA_ID = 11   -- RTLR
                        and pf.FISCAL_TYPE = 'FISCAL'
                        and pf.PERIOD_TYPE like 'Q%'
                        and pf.PERIOD_END_DATE < GETDATE()                         -- previous quarter from today
                        and pf.PERIOD_END_DATE > DATEADD( month, -12, getdate())   -- only 4 quarters
					    and pf.ISSUER_ID = @ISSUER_ID
                       -- order by pf.PERIOD_END_DATE  desc           
                     ) f
        group by f.issuer_id , f.FISCAL_TYPE, f.COA_TYPE, f.DATA_SOURCE, f.CURRENCY
		 having count(distinct PERIOD_TYPE) = 4   
 
 
 select sum(f.amount)as AMOUNT, f.ISSUER_ID,(f.FISCAL_TYPE),(f.COA_TYPE),(f.DATA_SOURCE),(f.CURRENCY) ,DATEPART(YYYY, GETDATE()) as Current_Year
	into #B   
         from (select * 
                       from dbo.PERIOD_FINANCIALS pf
                      where pf.DATA_ID = 11   -- RTLR
                        and pf.FISCAL_TYPE = 'FISCAL'
                        and pf.PERIOD_TYPE like 'Q%'
                        and pf.PERIOD_END_DATE < DATEADD( month, -12, getdate())   -- previous to previous quarter from today
                        and pf.PERIOD_END_DATE > DATEADD( month, -24, getdate())   -- only 4 quarters
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
		,  '', 'C', 0, '01/01/1900'	-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  213 as DATA_ID							-- DATA_ID:213 
		,  ((a.AMOUNT/b.Amount)-1) as AMOUNT		--(Sum of previous 4 quarters RTLR*/ Sum of 4 quarters RTLR prior to previous 4**) -1
		,  '((Prev 4 Qtr(' + CAST(a.AMOUNT as varchar(32)) + ') / Prev Prev 4 Qtr(' + CAST(b.AMOUNT as varchar(32)) + '))-1)' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL'
	  from #A a
	 inner join	#B b on b.ISSUER_ID = a.ISSUER_ID 
					and b.DATA_SOURCE = a.DATA_SOURCE --and b.PERIOD_TYPE = a.PERIOD_TYPE
					and b.Current_Year = a.Current_Year and b.FISCAL_TYPE = a.FISCAL_TYPE
					and b.CURRENCY = a.CURRENCY
	 where 1=1 
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	 order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.FISCAL_TYPE, a.CURRENCY
			


	if @CALC_LOG = 'Y'
		BEGIN	
		-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 213 as DATA_ID, a.ISSUER_ID, 'C' as PERIOD_TYPE
				, 0, '1/1/1900' as PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 213 Trailing Revenue Growth . DATA_ID:11 is NULL or ZERO'
			  from #B a
			where 
			 isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 --DATA_ID = 11 -- RTLR
				 
			) union (	
			-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 213 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 213 Trailing Revenue Growth .  DATA_ID:11  No data or missing Prev quarters' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	
		-- ERROR - No data at all available or one of the previous quarter is missing
			select GETDATE() as LOG_DATE, 213 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 213 Trailing Revenue Growth .  DATA_ID:11  No data or missing Prev to Prev quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END

	-- Clean up
	drop table #A
	drop table #B
	
	




--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00239'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())