USE [AIMS_Main]
GO
/****** Object:  StoredProcedure [dbo].[expICProcess]    Script Date: 01/22/2014 16:51:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

ALTER procedure [dbo].[expICProcess]
as

declare @totDirtyValue decimal(22,8) 

select @totDirtyValue = sum(ch.DIRTY_VALUE_PC) from GF_COMPOSITE_LTHOLDINGS ch
	where ch.PORTFOLIO_ID = 'EQYALL'
	and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_COMPOSITE_LTHOLDINGS ch where ch.PORTFOLIO_ID = 'EQYALL')
	group by ch.PORTFOLIO_DATE

select ISSUER_ID, max(ROOT_SOURCE_DATE) as LAST_UPDATED
into #Root
from dbo.INTERNAL_STATEMENT
group by ISSUER_ID

select * INTO #compMatrix from COMPOSITE_MATRIX cm where cm.COMPOSITE != 'EQYALL'

select cm.PORTFOLIO
	, case when cmx.composite is null then 'EQYSPEC' else cmx.composite end as 'Composite'
into #fnlCompMatrix
from COMPOSITE_MATRIX cm
	left join #compMatrix cmx on cm.PORTFOLIO = cmx.portfolio
where cm.COMPOSITE = 'EQYALL' and cm.ACTIVE = 1


select sb.ASEC_SEC_COUNTRY_NAME
	, sb.ISSUER_ID
	, sb.SECURITY_ID
	, sb.ASEC_SEC_SHORT_NAME
	, sb.ISSUE_NAME
	, cm.composite
	, sb.ASHMOREEMM_PRIMARY_ANALYST
	--, ch.PORPATH
	, ch.DIRTY_VALUE_PC/@totDirtyValue as 'Portfolio%'
	, a.DATA_DESC
	, a.FV_BUY
	, a.FV_SELL
	, a.UPDATED as FV_Update
	, a.CURRENT_MEASURE_VALUE
	, a.UPSIDE
	, r.last_updated
from GF_SECURITY_BASEVIEW sb
	left join GF_COMPOSITE_LTHOLDINGS ch on sb.ASEC_SEC_SHORT_NAME = ch.ASEC_SEC_SHORT_NAME 
		and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_COMPOSITE_LTHOLDINGS ch where ch.PORTFOLIO_ID = 'EQYALL')
		and ch.PORTFOLIO_ID = 'EQYALL'
		and ch.SECURITYTHEMECODE = 'EQUITY'
	left join (select fv.*,dm.DATA_DESC from FAIR_VALUE fv, DATA_MASTER dm where fv.FV_MEASURE = dm.DATA_ID) a  on a.SECURITY_ID = sb.SECURITY_ID
	left join #Root r on r.issuer_id = ch.ISSUER_ID
	left join #fnlCompMatrix cm on ch.PORPATH = cm.PORTFOLIO
--where ch.DIRTY_VALUE_PC > 0

DROP TABLE #Root
DROP TABLE #compMatrix
DROP TABLE #fnlCompMatrix