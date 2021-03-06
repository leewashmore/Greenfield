SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[GetFinstatDetail] 
@issuerID varchar(20),
@securityId varchar(20),
@dataSource varchar(10),
@fiscalType char(8),
@currency char(3)
	
AS
BEGIN
--CREATING GROUPED DATA
	--when FINSTAT_DISPLAY.DATA_ID in PF
SELECT  aa.DATA_SOURCE,
		aa.ROOT_SOURCE,
		aa.ROOT_SOURCE_DATE,
		bb.ESTIMATE_ID,
		isnull(aa.PERIOD_YEAR,2300) as PERIOD_YEAR,
		aa.DATA_ID,
		aa.AMOUNT,
		bb.MULTIPLIER,
		bb.DECIMALS,
		bb.PERCENTAGE,
		bb.BOLD_FONT,
		bb.GROUP_NAME,
		bb.SORT_ORDER,
		bb.HARMONIC,
		bb.DATA_DESC
into #Prelim
FROM
(Select *
From PERIOD_FINANCIALS 
Where( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and PERIOD_TYPE = 'A'
and FISCAL_TYPE = @fiscalType
and CURRENCY = @currency) aa
FULL JOIN
(Select a.*, b.DATA_DESC 
from FINSTAT_DISPLAY a, DATA_MASTER b
where a.COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
and a.DATA_ID = b.DATA_ID) bb on aa.DATA_ID = bb.DATA_ID
where bb.DATA_ID IS NOT NULL
order by SORT_ORDER

--Mod (JM) 09/11/13 to manually insert Blended Forward section after pricing section
declare @Year int
declare @GroupStart int
declare @BFRoot12 varchar(10)
declare @BFRoot24 varchar(10)

set @Year = year(getdate())

select @GroupStart = MAX(sort_order)
from #Prelim 
where group_name = 'Pricing'

set @GroupStart = @GroupStart + 1

select @BFRoot12 = MAX(ROOT_SOURCE)
from #Prelim
where PERIOD_YEAR = @Year

select @BFRoot24 = MAX(ROOT_SOURCE)
from #Prelim
where PERIOD_YEAR = @Year+1

--Insert BF PE 12 and BF 24 lines
insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot12 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
1 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart as SORT_ORDER,
'N' as HARMONIC,
'P/E BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 308  -- BF PE 12

insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot24 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year+1 as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
1 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart as SORT_ORDER,
'N' as HARMONIC,
'P/E BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 187  -- BF PE 24


--Insert BF P/BV 12 and BF 24 lines
insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot12 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
1 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart+1 as SORT_ORDER,
'N' as HARMONIC,
'P/BV BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 306  -- BF P/BV 12

insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot24 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year+1 as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
1 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart+1 as SORT_ORDER,
'N' as HARMONIC,
'P/BV BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 188  -- BF P/BV 24


--Insert BF Earnings 12 and BF 24 lines
insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot12 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
0 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart+2 as SORT_ORDER,
'N' as HARMONIC,
'Earnings BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 304  -- BF Earnings 12

insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot24 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year+1 as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
0 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart+2 as SORT_ORDER,
'N' as HARMONIC,
'Earnings BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 279  -- BF Earnings 24


--Insert BF Book Value 12 and BF 24 lines
insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot12 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
0 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart+3 as SORT_ORDER,
'N' as HARMONIC,
'Total Equity BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 301  -- BF Equity 12

insert into #Prelim
select pf.DATA_SOURCE,
@BFRoot24 as ROOT_SOURCE,
pf.ROOT_SOURCE_DATE,
NULL as ESTIMATE_ID,
@Year+1 as PERIOD_YEAR,
pf.DATA_ID,
pf.AMOUNT,
1 as MULTIPLIER,
0 as DECIMALS,
'N' as PERCENTAGE,
'N' as BOLD_FONT,
'Blended Forward' as GROUP_NAME,
@GroupStart+3 as SORT_ORDER,
'N' as HARMONIC,
'Total Equity BF' as DATA_DESC
from dbo.PERIOD_FINANCIALS pf
where ( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and CURRENCY = @currency
and DATA_ID = 280  -- BF Equity 24


select *
from #Prelim 
order by sort_order 
END


GO
