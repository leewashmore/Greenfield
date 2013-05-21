IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMS_Universe_Monitoring]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AIMS_Universe_Monitoring]
GO

/****** Object:  StoredProcedure [dbo].[AIMS_Universe_Monitoring]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[AIMS_Universe_Monitoring]
as


declare @PortDate datetime

select @PortDate = MAX(portfolio_date)
from .dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.PORTFOLIO_ID = 'EQYALL'


select max(@PortDate) as Date, max('HOLDING') as Type, gcl.ASEC_SEC_SHORT_NAME, max(gcl.ISSUE_NAME) as ISSUE_NAME, max(gcl.TICKER) as TICKER
into #Hold
from .dbo.GF_COMPOSITE_LTHOLDINGS gcl
where gcl.ASEC_SEC_SHORT_NAME not in (select distinct ASEC_SEC_SHORT_NAME from .dbo.GF_SECURITY_BASEVIEW)
and gcl.A_SEC_INSTR_TYPE in ('Equity','GDR/ADR') 
group by gcl.ASEC_SEC_SHORT_NAME


select max(@PortDate) as Date, max('BENCHMARK') as Type, bm.ASEC_SEC_SHORT_NAME, max(bm.ISSUE_NAME) as ISSUE_NAME, max(bm.TICKER) as TICKER
into #Benchmark
from .dbo.GF_BENCHMARK_HOLDINGS bm 
where bm.ASEC_SEC_SHORT_NAME not in (select distinct ASEC_SEC_SHORT_NAME from .dbo.GF_SECURITY_BASEVIEW)
and bm.ASEC_SEC_SHORT_NAME not in (select distinct ASEC_SEC_SHORT_NAME from #Hold)
group by bm.ASEC_SEC_SHORT_NAME


select *
from #Hold
union
select *
from #Benchmark 

drop table #Hold
drop table #Benchmark

