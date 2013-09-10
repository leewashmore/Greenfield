IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expTearPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expTearPage]
GO

/****** Object:  StoredProcedure [dbo].[expTearPage]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expTearPage](
	@SECURITY_ID			varchar(20) = NULL			-- The company identifier		
)
as

declare @ISSUER_ID varchar(20)
declare @ISO_COUNTRY_CODE	varchar(3)

select @ISSUER_ID = gsb.ISSUER_ID,
	@ISO_COUNTRY_CODE = gsb.ISO_COUNTRY_CODE
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_ID = @SECURITY_ID

--Compile Year Specific Data Points
select pf.DATA_ID, pf.CURRENCY, pf.PERIOD_YEAR, pf.AMOUNT, pf.PERIOD_END_DATE, dm.DATA_DESC
into #TearPage
from dbo.PERIOD_FINANCIALS pf
left join dbo.DATA_MASTER dm on pf.data_id = dm.DATA_ID
where pf.ISSUER_ID = @ISSUER_ID
and pf.DATA_SOURCE = 'PRIMARY'
and pf.FISCAL_TYPE = 'FISCAL'
and pf.PERIOD_TYPE = 'A'
and pf.DATA_ID in ('37','51','92','104','116','118','130','133','140','141','144','157','162','178','190','220','225','290')


--Add Current Period Based Fields
insert into #TearPage 
select pf.DATA_ID, pf.CURRENCY, pf.PERIOD_YEAR, pf.AMOUNT, pf.PERIOD_END_DATE, dm.DATA_DESC
from dbo.PERIOD_FINANCIALS pf
left join dbo.DATA_MASTER dm on pf.data_id = dm.DATA_ID
where pf.SECURITY_ID = @SECURITY_ID
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_TYPE = 'C'
and pf.DATA_ID in ('191','218','185')
and pf.CURRENCY = 'USD'

--Add Prices from price table in Trading Currency
insert into #TearPage
select 191 as DATA_ID, gsb.TRADING_CURRENCY as CURRENCY, 0 as PERIOD_YEAR, p.PRICE as AMOUNT, p.PRICE_DATE as PERIOD_END_DATE, 'Price' as DATA_DESC
from dbo.PRICES p
left join dbo.GF_SECURITY_BASEVIEW gsb on p.SECURITY_ID = gsb.SECURITY_ID
where p.SECURITY_ID = @SECURITY_ID 
and p.PRICE_DATE = (select MAX(PRICE_DATE) from dbo.PRICES where p.SECURITY_ID = @SECURITY_ID)
--and gsb.TRADING_CURRENCY <> 'USD'


--Add Beta, Mkt Risk Premium, Risk Free Rate and LT GDP Growth with fictional data ids
--Beta
insert into #TearPage
select 991 as DATA_ID, 'USD' as CURRENCY, 0 as PERIOD_YEAR, isnull(gsb.beta,0) as AMOUNT, GETDATE()-1 as PERIOD_END_DATE, 'Beta' as DATA_DESC
from dbo.GF_SECURITY_BASEVIEW gsb
where gsb.SECURITY_ID = @SECURITY_ID

--Mkt Risk Premium
insert into #TearPage
select 992 as DATA_ID, 'USD' as CURRENCY, 0 as PERIOD_YEAR, isnull(MIC.RISK_PREM,0) as AMOUNT, GETDATE()-1 as PERIOD_END_DATE, 'Market Risk Premium' as DATA_DESC
from dbo.MODEL_INPUTS_CTY MIC
where mic.COUNTRY_CODE = @ISO_COUNTRY_CODE

--Risk Free Rate
insert into #TearPage
select 993 as DATA_ID, 'USD' as CURRENCY, 0 as PERIOD_YEAR, isnull(MIC.RISK_FREE_RATE,0) as AMOUNT, GETDATE()-1 as PERIOD_END_DATE, 'Risk Free Rate' as DATA_DESC
from dbo.MODEL_INPUTS_CTY MIC
where mic.COUNTRY_CODE = @ISO_COUNTRY_CODE

--LT GDP Growth
insert into #TearPage
select 994 as DATA_ID, 'USD' as CURRENCY, 0 as PERIOD_YEAR, isnull(MIC.LONG_TERM_GDP_GR,0) as AMOUNT, GETDATE()-1 as PERIOD_END_DATE, 'LT GDP Growth Rate' as DATA_DESC
from dbo.MODEL_INPUTS_CTY MIC
where mic.COUNTRY_CODE = @ISO_COUNTRY_CODE


select *
from #TearPage tp
order by tp.data_id, tp.period_year

drop table #TearPage

go