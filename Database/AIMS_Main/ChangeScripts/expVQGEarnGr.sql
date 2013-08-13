IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expVQGEarnGr]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expVQGEarnGr]
GO

/****** Object:  StoredProcedure [dbo].[expVQGEarnGr]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expVQGEarnGr]
as

select *
into #fwdE 
from dbo.PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'FISCAL'
and pf.PERIOD_TYPE = 'C'
and pf.CURRENCY != 'USD'
and pf.DATA_ID = '279'
and pf.DATA_SOURCE = 'PRIMARY'
--and pf.PERIOD_YEAR = '2011'
order by pf.ISSUER_ID

select *
into #e2012 
from dbo.PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'FISCAL'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY != 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = '2012'
order by pf.ISSUER_ID

select *
into #e2013 
from dbo.PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'FISCAL'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY != 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = '2013'
order by pf.ISSUER_ID

select *
into #e2014 
from dbo.PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'FISCAL'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY != 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = '2014'
order by pf.ISSUER_ID

select *
into #e2015 
from dbo.PERIOD_FINANCIALS pf
where pf.FISCAL_TYPE = 'FISCAL'
and pf.PERIOD_TYPE = 'A'
and pf.CURRENCY != 'USD'
and pf.DATA_ID = '290'
and pf.DATA_SOURCE = 'PRIMARY'
and pf.PERIOD_YEAR = '2015'
order by pf.ISSUER_ID

select 
	sb.TICKER + ' Equity' as Ticker
	,sb.SECURITY_ID
	, sb.ISSUER_ID
	, sb.ISIN
	, sb.MSCI
	, e2012.amount as e2012
	, e2012.period_end_date as ped2012
	, e2013.amount as e2013
	, e2013.period_end_date as ped2013
	, e2014.amount as e2014
	, e2014.period_end_date as ped2014
	, e2015.amount as e2015
	, e2015.period_end_date as ped2015
from dbo.GF_SECURITY_BASEVIEW sb
	left join #e2012 e2012 on sb.ISSUER_ID = e2012.ISSUER_ID
	left join #e2013 e2013 on sb.ISSUER_ID = e2013.ISSUER_ID
	left join #e2014 e2014 on sb.ISSUER_ID = e2014.ISSUER_ID
	left join #e2015 e2015 on sb.ISSUER_ID = e2015.ISSUER_ID
--where sb.issuer_id = '157633'
order by ped2013 desc

drop table #e2012
drop table #e2013
drop table #e2014
drop table #e2015
drop table #fwdE

--select * from #fwdE order by root_source_date desc
Go