SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetFreeCashFlows] (@IssuerId varchar(20))	
AS
BEGIN
	
	SET NOCOUNT ON;

-------------------REVENUE GROWTH-------------
select PERIOD_YEAR, AMOUNT, DATA_ID
into #RevenueGrowthTab
from period_financials
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
period_year <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and
DATA_ID = 178 

ALTER table #RevenueGrowthTab
ADD FIELD_NAME varchar(20)

Update #RevenueGrowthTab
set FIELD_NAME = 'Revenue Growth' 
where DATA_ID = 178

------------------EBITDA Margins-----------------
insert into #RevenueGrowthTab(PERIOD_YEAR,AMOUNT,DATA_ID)
select PERIOD_YEAR, AMOUNT, DATA_ID
from period_financials
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
period_year <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and 
DATA_ID = 144


Update #RevenueGrowthTab
set FIELD_NAME = 'EBITDA Margins' 
where DATA_ID = 144


----------EBITDA------------------
insert into #RevenueGrowthTab(PERIOD_YEAR,AMOUNT,DATA_ID)
select PERIOD_YEAR, AMOUNT, DATA_ID
from period_financials
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
period_year <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and 
DATA_ID = 130


Update #RevenueGrowthTab
set FIELD_NAME = 'EBITDA' 
where DATA_ID = 130
 
------------Taxes------------------
insert into #RevenueGrowthTab(PERIOD_YEAR,AMOUNT,DATA_ID)
select PERIOD_YEAR, AMOUNT, DATA_ID
from period_financials
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
period_year <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and 
DATA_ID =37


Update #RevenueGrowthTab
set FIELD_NAME = 'Taxes' 
where DATA_ID = 37

----------------Change in Working Capital----------
insert into #RevenueGrowthTab(PERIOD_YEAR,AMOUNT,DATA_ID)
select PERIOD_YEAR, AMOUNT, DATA_ID
from period_financials
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
PERIOD_YEAR <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and 
DATA_ID = 116

Update #RevenueGrowthTab
set FIELD_NAME = 'Change in WC' 
where DATA_ID = 116

-----------------Capital Expenditures
insert into #RevenueGrowthTab(PERIOD_YEAR,AMOUNT,DATA_ID)
select PERIOD_YEAR, AMOUNT, DATA_ID
from period_financials
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
PERIOD_YEAR <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and 
DATA_ID = 118

Update #RevenueGrowthTab
set FIELD_NAME = 'Capex' 
where DATA_ID = 118

-----Free Cash Flows--------------
insert into #RevenueGrowthTab(PERIOD_YEAR,AMOUNT,DATA_ID)
Select PERIOD_YEAR, AMOUNT,DATA_ID
from PERIOD_FINANCIALS
where ISSUER_ID = @IssuerId and
DATA_SOURCE = 'PRIMARY' and
PERIOD_TYPE = 'A'and
PERIOD_YEAR >= datepart(year,getdate()) and 
PERIOD_YEAR <= datepart(year,dateadd(year,9,getdate())) and
CURRENCY = 'USD' and 
FISCAL_TYPE = 'CALENDAR' and
DATA_ID = 157

Update #RevenueGrowthTab
set FIELD_NAME = 'Free Cash Flow' 
where DATA_ID = 157
 
select * from #RevenueGrowthTab order by FIELD_NAME ,PERIOD_YEAR desc 
--Dropping temporary tables
drop table #RevenueGrowthTab

END
GO
