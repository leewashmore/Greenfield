IF OBJECT_ID ( 'AIMS_Calc_0', 'P' ) IS NOT NULL 
DROP PROCEDURE AIMS_Calc_0;
GO
-------------------------------------------------------------------------------
-- Purpose:	This stored procedure calculates data needed for other calculations.
--			DATA_ID		Name
--			-------		------------------------
--				185		Market Capitalization
--				191		Prices
--				218		Shares Ourstanding
--			These values are calculated for all the dates currently available 
--			in the PERIOD_FINANCIALS table: Historic, CURRENT and Future.
--
-- Author:	David Muench
-------------------------------------------------------------------------------

create procedure AIMS_Calc_0 (
	@ISSUER_ID	varchar(20) = NULL		-- If NULL then process all issuers at once
,	@CALC_LOG			char		= 'Y'			-- Write errors to the log table
)
as

	declare @START		datetime		-- the time the calc starts
	set @START = GETDATE()

	--Step 1 -   Extract all securities* from SECURITY_BASEVIEW joining to tblStdCompanyInfo on ReportNumber to pull COAType.  If not found in tblStdCompanyInfo join to INTERNAL_ISSUER on ISSUER_ID  to pull COAType.   
	--*If a single issuer run, then extract only the securities for the issuer.  
print 'Extract all securities';
	select cast(sb.SECURITY_ID as varchar(20)) as SECURITY_ID
		,  sb.ISSUER_ID
		,  sb.ASEC_SEC_SHORT_NAME
		,  ISO_COUNTRY_CODE
		,  TRADING_CURRENCY
		,  ISNULL(ii.COA_TYPE, sci.COAType) as COA_TYPE
		,  sb.SHARES_PER_ADR as ADR_CONV
	  into #SB
	  from dbo.GF_SECURITY_BASEVIEW sb
	  left join dbo.INTERNAL_ISSUER ii on ii.ISSUER_ID = sb.ISSUER_ID
	  left join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = sb.REPORTNUMBER
	 where 1=1
	   and sb.ISSUER_ID = @ISSUER_ID
	;
	print 'Extract all securities' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
	

-- select 'SB' as SB, * from #SB

	-- Get a list of period end dates
print 'List of period end dates';
	select ISSUER_ID
		,  PERIOD_END_DATE
		,  DATA_SOURCE
		,  ROOT_SOURCE
		,  max(ROOT_SOURCE_DATE) as ROOT_SOURCE_DATE
		,  PERIOD_TYPE
		,  PERIOD_YEAR
		,  CURRENCY
		,  FISCAL_TYPE
		,  'PF' as tab 
	  into #PERIOD_END_DATES
	  from dbo.PERIOD_FINANCIALS
	 where ISSUER_ID in (select distinct ISSUER_ID from #SB)
	   and PERIOD_TYPE <> 'C'
	 group by ISSUER_ID
		,  PERIOD_END_DATE
		,  DATA_SOURCE
		,  ROOT_SOURCE
		,  PERIOD_TYPE
		,  PERIOD_YEAR
		,  CURRENCY
		,  FISCAL_TYPE
	print 'List of period end dates' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

print 'Add the current dates';
	insert into #PERIOD_END_DATES 
	select distinct pf.ISSUER_ID
		,  p.PRICE_DATE as PERIOD_END_DATE
		,  pf.DATA_SOURCE
		,  ' ' as ROOT_SOURCE
		,  s.SHARES_DATE as ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  pf.CURRENCY 
		,  ' ' as FISCAL_TYPE
		,  'Pa' as tab 
	  from dbo.PERIOD_FINANCIALS pf
	 inner join dbo.GF_SECURITY_BASEVIEW sb on (sb.ISSUER_ID = pf.ISSUER_ID or sb.SECURITY_ID = pf.SECURITY_ID)
	 inner join (select SECURITY_ID, MAX(PRICE_DATE) as PRICE_DATE 
				   from PRICES 
				  where SECURITY_ID in (select distinct SECURITY_ID from #SB)
				  group by SECURITY_ID
				) p on p.SECURITY_ID = sb.SECURITY_ID
	 inner join (select ISSUER_ID, MAX(SHARES_DATE) as SHARES_DATE 
				   from ISSUER_SHARES 
				  where SHARES_DATE < GETDATE() 
				    and ISSUER_ID in (select distinct ISSUER_ID from #SB)
				  group by ISSUER_ID
				) s on s.ISSUER_ID = pf.ISSUER_ID
	 where sb.ISSUER_ID in (select distinct ISSUER_ID from #SB)
	   and pf.ROOT_SOURCE <> 'CONSENSUS'
/*	 group by pf.ISSUER_ID
		,  p.PRICE_DATE
		,  pf.DATA_SOURCE
		,  pf.ROOT_SOURCE
		,  s.SHARES_DATE
		,  pf.CURRENCY
		,  pf.FISCAL_TYPE
*/
	print 'Add the current dates' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

			

-- select 'PED' as PED, * from #PERIOD_END_DATES

	-- Step 2 -   Pull source data needed and run any preliminary calculations:
print 'Step 2 - Pull source data needed and run any preliminary calculations';
	select sb.ISSUER_ID
		,  ped.PERIOD_END_DATE
		,  ped.DATA_SOURCE
		,  ped.ROOT_SOURCE
		,  ped.ROOT_SOURCE_DATE
		,  ped.PERIOD_TYPE
		,  ped.PERIOD_YEAR
		,  ped.CURRENCY
		,  ped.FISCAL_TYPE
		,  cast(sb.SECURITY_ID as varchar(20)) as SECURITY_ID
		,  p.PRICE
		,  p.PRICE_DATE
		,  sb.ADR_CONV
		,  sb.TRADING_CURRENCY
		,  sb.COA_TYPE
		,  i.SHARES_OUTSTANDING
		,  fxt.FX_RATE as PRICE_FX_RATE
		,  fxc.FX_RATE as DATA_FX_RATE
	  into #A
	  from #SB sb
	 inner join #PERIOD_END_DATES ped on ped.ISSUER_ID = sb.ISSUER_ID
	 inner join dbo.Country_Master cm on cm.COUNTRY_CODE = sb.ISO_COUNTRY_CODE
	  left join dbo.PRICES p on p.SECURITY_ID = sb.SECURITY_ID and p.PRICE_DATE = ped.PERIOD_END_DATE
	  left join dbo.ISSUER_SHARES i on i.ISSUER_ID = sb.ISSUER_ID and i.SHARES_DATE = ped.ROOT_SOURCE_DATE
	  left join dbo.FX_RATES fxt on fxt.CURRENCY = sb.TRADING_CURRENCY and fxt.FX_DATE = ped.PERIOD_END_DATE
	  left join dbo.FX_RATES fxc on fxc.CURRENCY = cm.CURRENCY_CODE and fxc.FX_DATE = ped.PERIOD_END_DATE

	print 'Step 2 - Pull source data needed and run any preliminary calculations' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()


---------------------------------
-- Store the CURRENT values 185 USD
---------------------------------
print 'Store the CURRENT values 185 USD'
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  '01/01/1900' as PERIOD_END_DATE
		,  ' ' as FISCAL_TYPE
		,  'USD' CURRENCY
		,  185 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,   ((	((PRICE * SHARES_OUTSTANDING) 
				/ case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end)
				/ PRICE_FX_RATE)) as AMOUNT
		,  'Price ('+ cast(PRICE as varchar(20))
				+ ') * Shares(' + CAST(SHARES_OUTSTANDING as varchar(20)) 
				+ ') / ADV_CONV('+ cast(case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end as varchar(20))
				+ ') / FX Rate('+ cast(PRICE_FX_RATE as varchar(20)) 
				+ ')' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
/*
	 inner join (select ISSUER_ID, MAX(PERIOD_END_DATE) as PERIOD_END_DATE
				   from #A
				  where CURRENCY = 'USD'
				    and a.PERIOD_TYPE = 'C'
				    and ADR_CONV is not null
				    and PRICE is not null
				    and SHARES_OUTSTANDING is not null
				    and PRICE_FX_RATE is not null
				  group by ISSUER_ID
				) b on b.ISSUER_ID = a.ISSUER_ID and b.PERIOD_END_DATE = a.PERIOD_END_DATE
*/
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_TYPE = 'C'
	   and a.ADR_CONV is not null
	   and a.PRICE is not null
	   and a.SHARES_OUTSTANDING is not null
	   and a.PRICE_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY
 
	print 'Store the CURRENT values 185 USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()



---------------------------------
-- Store the Historic values 185 USD
---------------------------------
print 'Store the Historic values 185 USD'
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  'USD' as CURRENCY
		,  185 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,   ((	((PRICE * SHARES_OUTSTANDING) 
				/ case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end)
				/ PRICE_FX_RATE)) as AMOUNT
		,  'Price ('+ cast(PRICE as varchar(20))
				+ ') * Shares(' + CAST(SHARES_OUTSTANDING as varchar(20)) 
				+ ') / ADV_CONV('+ cast(case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end as varchar(20))
				+ ') / FX Rate('+ cast(PRICE_FX_RATE as varchar(20)) 
				+ ')' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTMATE' as AMOUNT_TYPE
	  from #A a
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_TYPE <> 'C'
	   and a.PERIOD_END_DATE < GETDATE()
	   and a.ADR_CONV is not null
	   and a.PRICE is not null
	   and a.SHARES_OUTSTANDING is not null
	   and a.PRICE_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE

	print 'Store the Historic values 185 USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

---------------------------------
-- Store the Future values 185 USD
---------------------------------
print 'Store the Future values 185 USD'
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  'USD' as CURRENCY
		,  185 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,  c.AMOUNT
		,  'Store the Future values 185 USD  ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTIMATE' as AMOUNT_TYPE
	  from #A a
	 inner join (select SECURITY_ID, DATA_SOURCE, AMOUNT		-- All future amounts use the Current amount
				   from PERIOD_FINANCIALS
				  where DATA_ID = 185
				    and PERIOD_TYPE = 'C'
				) c on c.SECURITY_ID = a.SECURITY_ID and 'USD' = a.CURRENCY and c.DATA_SOURCE = a.DATA_SOURCE
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_END_DATE > GETDATE()
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE

	print 'Store the Future values 185 USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()


-----------------------------------------------------
-- Store the CURRENT values 185 Local Currency
-----------------------------------------------------
print 'Store the CURRENT values 185 Local Currency'
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  '01/01/1900' as PERIOD_END_DATE
		,  ' ' as FISCAL_TYPE
		,  a.CURRENCY
		,  185 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,   ((	((PRICE * SHARES_OUTSTANDING) 
				/ case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end)
				/ PRICE_FX_RATE) * DATA_FX_RATE) as AMOUNT

		,  'Price ('+ cast(PRICE as varchar(20))
				+ ') * Shares(' + CAST(SHARES_OUTSTANDING as varchar(20)) 
				+ ') / ADV_CONV('+ cast(case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end as varchar(20))
				+ ') / FX Rate('+ cast(PRICE_FX_RATE as varchar(20)) 
				+ ') * Local FX Rate('+ cast(DATA_FX_RATE as varchar(20))
				+ ')' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
/*
	 inner join (select ISSUER_ID, MAX(PERIOD_END_DATE) as PERIOD_END_DATE
				   from #A
				  where CURRENCY <> 'USD'
				    and a.PERIOD_TYPE = 'C'
				    and ADR_CONV is not null
				    and PRICE is not null
				    and SHARES_OUTSTANDING is not null
				    and PRICE_FX_RATE is not null
				    and DATA_FX_RATE is not null
				  group by ISSUER_ID
				) b on b.ISSUER_ID = a.ISSUER_ID and b.PERIOD_END_DATE = a.PERIOD_END_DATE
*/
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_TYPE = 'C'
	   and a.ADR_CONV is not null
	   and a.PRICE is not null
	   and a.SHARES_OUTSTANDING is not null
	   and a.PRICE_FX_RATE is not null
	   and a.DATA_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY
 
	print 'Store the Future values 185 USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()
 
 



-----------------------------------------------------
-- Store the Historic values 185 Local Currency
-----------------------------------------------------
print 'Store the Historic values 185 Local Currency'
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  a.CURRENCY
		,  185 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,   ((	((PRICE * SHARES_OUTSTANDING) 
				/ case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end)
				/ PRICE_FX_RATE) * DATA_FX_RATE) as AMOUNT

		,  'Price ('+ cast(PRICE as varchar(20))
				+ ') * Shares(' + CAST(SHARES_OUTSTANDING as varchar(20)) 
				+ ') / ADV_CONV('+ cast(case when ADR_CONV is NULL or ADR_CONV = 0.0 then 1 else ADR_CONV end as varchar(20))
				+ ') / FX Rate('+ cast(PRICE_FX_RATE as varchar(20)) 
				+ ') * Local FX Rate('+ cast(DATA_FX_RATE as varchar(20))
				+ ')' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_TYPE <> 'C'
	   and a.PERIOD_END_DATE < GETDATE()
	   and a.ADR_CONV is not null
	   and a.PRICE is not null
	   and a.SHARES_OUTSTANDING is not null
	   and a.PRICE_FX_RATE is not null
	   and a.DATA_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE


	print 'Store the Historic values 185 Local Currency' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

-----------------------------------------------------
-- Store the Future values 185 Local CUrrency
-----------------------------------------------------
print 'Store the Future values 185 Local CUrrency'
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  a.CURRENCY
		,  185 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,  c.AMOUNT
		,  'Store the Future values 185 Local CUrrency' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTIMATE' as AMOUNT_TYPE
	  from #A a
	 inner join (select SECURITY_ID, DATA_SOURCE, AMOUNT, CURRENCY		-- All future amounts use the Current amount
				   from PERIOD_FINANCIALS
				  where DATA_ID = 185
				    and PERIOD_TYPE = 'C'
				) c on c.SECURITY_ID = a.SECURITY_ID and c.CURRENCY = a.CURRENCY and c.DATA_SOURCE = a.DATA_SOURCE
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_END_DATE > GETDATE()
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE


	print 'Store the Future values 185 Local CUrrency' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------
------------------------------  Now store the prices ----------------------------------------------
---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------



-------------------------------------------------
-- Store the CURRENT values Prices 191 USD
-------------------------------------------------
print 'CURRENT prices'

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  '01/01/1900' as PERIOD_END_DATE
		,  ' ' as FISCAL_TYPE
		,  'USD' CURRENCY
		,  191 as DATA_ID										-- DATA_ID:191 Price
		,  (PRICE / PRICE_FX_RATE) as AMOUNT
		,  ' ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 inner join (select ISSUER_ID, MAX(PERIOD_END_DATE) as PERIOD_END_DATE
				   from #A
				  where CURRENCY = 'USD'
				    and PRICE is not null
				    and PRICE_FX_RATE is not null
				  group by ISSUER_ID
				) b on b.ISSUER_ID = a.ISSUER_ID and b.PERIOD_END_DATE = a.PERIOD_END_DATE
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_TYPE = 'C'
	   and a.PRICE is not null
	   and a.PRICE_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY
 

	print 'CURRENT prices' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()


---------------------------------------------
-- Store the Historic values Prices 191 USD
---------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  'USD' as CURRENCY
		,  191 as DATA_ID										-- DATA_ID:191 Price
		,  (PRICE / PRICE_FX_RATE) as AMOUNT
		,  ' ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_TYPE <> 'C'
	   and a.PERIOD_END_DATE < GETDATE()
	   and a.PRICE is not null
	   and a.PRICE_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE

	print 'Store the Historic values Prices 191 USD' + ' ISSUER_ID = ' +@ISSUER_ID + ' - Elapsed Time ' + 	CONVERT(varchar(40), cast(DATEDIFF(millisecond, @START, GETDATE()) as decimal) /1000)
	set @START = GETDATE()

--------------------------------------------
-- Store the Future values Prices 191 USD
--------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  'USD' as CURRENCY
		,  191 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,  c.AMOUNT
		,  ' ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTIMATE' as AMOUNT_TYPE
	  from #A a
	 inner join (select SECURITY_ID, DATA_SOURCE, AMOUNT		-- All future amounts use the Current amount
				   from PERIOD_FINANCIALS
				  where DATA_ID = 191
				    and PERIOD_TYPE = 'C'
				) c on c.SECURITY_ID = a.SECURITY_ID and 'USD' = a.CURRENCY and c.DATA_SOURCE = a.DATA_SOURCE
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_END_DATE > GETDATE()
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE



-----------------------------------------------------
-- Store the CURRENT values Prices 191 Local Currency
-----------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  '01/01/1900' as PERIOD_END_DATE
		,  ' ' as FISCAL_TYPE
		,  a.CURRENCY
		,  191 as DATA_ID										-- DATA_ID:191 Price
		,  ((PRICE / PRICE_FX_RATE) * DATA_FX_RATE) as AMOUNT
		,  ' ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 inner join (select ISSUER_ID, MAX(PERIOD_END_DATE) as PERIOD_END_DATE
				   from #A
				  where CURRENCY <> 'USD'
				    and PRICE is not null
				    and PRICE_FX_RATE is not null
				  group by ISSUER_ID
				) b on b.ISSUER_ID = a.ISSUER_ID and b.PERIOD_END_DATE = a.PERIOD_END_DATE
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_TYPE = 'C'
	   and a.PRICE is not null
	   and a.PRICE_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY
 
 
  



-----------------------------------------------------
-- Store the Historic values Prices 191 Local Currency
-----------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  a.CURRENCY
		,  191 as DATA_ID										-- DATA_ID:191 Price
		,  ((PRICE / PRICE_FX_RATE) * DATA_FX_RATE) as AMOUNT
		,  ' ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_TYPE <> 'C'
	   and a.PERIOD_END_DATE < GETDATE()
	   and a.PRICE is not null
	   and a.PRICE_FX_RATE is not null
	   and a.DATA_FX_RATE is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE


-----------------------------------------------------
-- Store the Future values Prices 191 Local CUrrency
-----------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  a.CURRENCY
		,  191 as DATA_ID										-- DATA_ID:185 Market Capitalization
		,  c.AMOUNT
		,  ' ' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTIMATE' as AMOUNT_TYPE
	  from #A a
	 inner join (select SECURITY_ID, DATA_SOURCE, AMOUNT, CURRENCY		-- All future amounts use the Current amount
				   from PERIOD_FINANCIALS
				  where DATA_ID = 191
				    and PERIOD_TYPE = 'C'
				) c on c.SECURITY_ID = a.SECURITY_ID and c.CURRENCY = a.CURRENCY and c.DATA_SOURCE = a.DATA_SOURCE
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_END_DATE > GETDATE()
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE


---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------
------------------------------  Now store the Shares Outstanding ----------------------------------
---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------



-------------------------------------------------
-- Store the CURRENT values 218 USD
-------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  '01/01/1900' as PERIOD_END_DATE
		,  ' ' as FISCAL_TYPE
		,  'USD' CURRENCY
		,  218 as DATA_ID										-- DATA_ID:218 Shares Outstanding
		,  SHARES_OUTSTANDING as AMOUNT
		,  'C1' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
/*	 inner join (select ISSUER_ID, MAX(PERIOD_END_DATE) as PERIOD_END_DATE
				   from #A
				  where CURRENCY = 'USD'
				    and PERIOD_TYPE = 'A'
				    and SHARES_OUTSTANDING is not null
				  group by ISSUER_ID
				) b on b.ISSUER_ID = a.ISSUER_ID and b.PERIOD_END_DATE = a.PERIOD_END_DATE
*/	 where a.CURRENCY = 'USD'
	   and PERIOD_TYPE = 'C'
	   and a.SHARES_OUTSTANDING is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY
 



---------------------------------------------
-- Store the Historic values 218 USD
---------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  'USD' as CURRENCY
		,  218 as DATA_ID										-- DATA_ID:218 Shares Outstanding
		,  SHARES_OUTSTANDING as AMOUNT
		,  'H1' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_END_DATE < GETDATE()
	   and a.SHARES_OUTSTANDING is not null
	   and a.PERIOD_TYPE <> 'C'
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE


--------------------------------------------
-- Store the Future values Prices 218 USD
--------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  'USD' as CURRENCY
		,  218 as DATA_ID										-- DATA_ID:218 Shares Outstanding
		,  c.AMOUNT
		,  'F1' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTIMATE' as AMOUNT_TYPE
	  from #A a
	 inner join (select SECURITY_ID, DATA_SOURCE, AMOUNT		-- All future amounts use the Current amount
				   from PERIOD_FINANCIALS
				  where DATA_ID = 218
				    and PERIOD_TYPE = 'C'
				) c on c.SECURITY_ID = a.SECURITY_ID and 'USD' = a.CURRENCY and c.DATA_SOURCE = a.DATA_SOURCE
	 where a.CURRENCY = 'USD'
	   and a.PERIOD_END_DATE > GETDATE()
	   and a.PERIOD_TYPE <> 'C'
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE



-----------------------------------------------------
-- Store the CURRENT values 218 Local Currency
-----------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  ' ' as ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  'C' as PERIOD_TYPE
		,  0 as PERIOD_YEAR
		,  '01/01/1900' as PERIOD_END_DATE
		,  ' ' as FISCAL_TYPE
		,  a.CURRENCY
		,  218 as DATA_ID										-- DATA_ID:218 Shares Outstanding
		,  SHARES_OUTSTANDING as AMOUNT
		,  'C2' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
/*	 inner join (select ISSUER_ID, MAX(PERIOD_END_DATE) as PERIOD_END_DATE
				   from #A
				  where CURRENCY <> 'USD'
				    and SHARES_OUTSTANDING is not null
				  group by ISSUER_ID
				) b on b.ISSUER_ID = a.ISSUER_ID and b.PERIOD_END_DATE = a.PERIOD_END_DATE
*/	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_TYPE = 'C'
	   and a.SHARES_OUTSTANDING is not null
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY
 
 
  



-----------------------------------------------------
-- Store the Historic values 218 Local Currency
-----------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  a.CURRENCY
		,  218 as DATA_ID										-- DATA_ID:218 Shares Outstanding
		,  SHARES_OUTSTANDING as AMOUNT
		,  'H2' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ACTUAL' as AMOUNT_TYPE
	  from #A a
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_END_DATE < GETDATE()
	   and a.SHARES_OUTSTANDING is not null
	   and a.PERIOD_TYPE <> 'C'
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE


-----------------------------------------------------
-- Store the Future values 218 Local CUrrency
-----------------------------------------------------

	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select ' ' as ISSUER_ID
		,  a.SECURITY_ID
		,  a.COA_TYPE
		,  a.DATA_SOURCE
		,  a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE
		,  a.PERIOD_TYPE
		,  a.PERIOD_YEAR
		,  a.PERIOD_END_DATE
		,  a.FISCAL_TYPE
		,  a.CURRENCY
		,  218 as DATA_ID										-- DATA_ID:218 Shares Outstanding
		,  c.AMOUNT
		,  'F2' as CALCULATION_DIAGRAM
		,  a.CURRENCY
		,  'ESTIMATE' as AMOUNT_TYPE
	  from #A a
	 inner join (select SECURITY_ID, DATA_SOURCE, AMOUNT, CURRENCY		-- All future amounts use the Current amount
				   from PERIOD_FINANCIALS
				  where DATA_ID = 218
				    and PERIOD_TYPE = 'C'
				) c on c.SECURITY_ID = a.SECURITY_ID and c.CURRENCY = a.CURRENCY and c.DATA_SOURCE = a.DATA_SOURCE
	 where a.CURRENCY <> 'USD'
	   and a.PERIOD_END_DATE > GETDATE()
	   and a.PERIOD_TYPE <> 'C'
--	 order by a.ISSUER_ID, a.COA_TYPE, a.CURRENCY, PERIOD_YEAR, PERIOD_TYPE

go

-- exec AIMS_Calc_0 182896
-- delete from PERIOD_FINANCIALS where DATA_ID in( 185, 191, 218)

