use AIMS_Main
go

declare @buffer as decimal(32,6) = .02

select ic.* 
into #ic_most_recent
from IC_TABLE ic ,
	(select ic.ISSUER_ID, MAX(ic.date_presented)as date_presented from IC_TABLE ic group by ic.ISSUER_ID) a
where
	a.ISSUER_ID = ic.ISSUER_ID
	and a.date_presented = ic.DATE_PRESENTED
--	and a.date_presented in ('7/21/2014','7/14/2014')

select imr.*
	,sb.SECURITY_ID 
	,pf.AMOUNT
	, pf.DATA_SOURCE
	, pf.ROOT_SOURCE_DATE
into #tmp
from #ic_most_recent imr
	left join GF_SECURITY_BASEVIEW sb on imr.issuer_id = sb.ISSUER_ID 
	left join PERIOD_FINANCIALS pf on ( pf.SECURITY_ID = sb.issuer_proxy
			and imr.IC_MEASURE = pf.DATA_ID
			and pf.CURRENCY = 'USD'
			and pf.PERIOD_TYPE = 'C'
			)
	left join IC_MONITORING im on im.ISSUER_ID = imr.issuer_id
where sb.issuer_proxy = sb.SECURITY_ID
	and im.ISSUER_ID is null
	--and imr.update_date = '8/7/2014'
order by imr.ISSUER_ID

insert into IC_MONITORING (ISSUER_ID,SECURITY_ID,DATA_SOURCE,MEASURE_VALUE,CURR_ACTION,UPDATE_DATE)
select t.ISSUER_ID
	, t.SECURITY_ID
	, t.DATA_SOURCE
	, t.amount as MEASURE_VALUE
	, case 
		when t.amount is null then 'insufficient data'
		when t.ic_action = 'HOLD' then (case when t.amount > (t.ic_sell * (1-@buffer)) then 'SELL' else 'HOLD' END) 
		WHEN t.ic_action = 'BUY' then  (case when t.amount > (t.ic_sell * (1-@buffer)) then 'SELL' 
											 when  t.amount > (t.ic_buy * (1+@buffer)) then 
												CASE WHEN ISNULL(ch.DIRTY_VALUE_PC,0)=0 THEN 'WATCH' ELSE 'HOLD' END 
											 ELSE 'BUY' 
											 END
										) 
		WHEN t.ic_action = 'SELL' then (case when t.amount < (t.ic_buy * (1+@buffer)) then 'BUY' else 'SELL' END) 
		WHEN t.ic_action = 'WATCH' then (
			case when (ch.DIRTY_VALUE_PC > 0 and  t.amount < (t.ic_sell * (1-@buffer))) then 'HOLD'
				when  (ch.DIRTY_VALUE_PC > 0 and  t.amount >= (t.ic_sell * (1-@buffer))) then 'SELL'
				when  (ISNULL(ch.DIRTY_VALUE_PC,0)=0 and  t.amount <= (t.ic_buy * (1+@buffer))) then 'BUY'
				when  (ISNULL(ch.DIRTY_VALUE_PC,0)=0 and  t.amount > (t.ic_buy * (1+@buffer))) then 'WATCH'
				END)
		ELSE t.ic_action end 
		as CURR_ACTION
	, t.root_source_date as UPDATE_DATE
from #tmp t
	left join GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = t.security_id
	left join GF_COMPOSITE_LTHOLDINGS ch on (sb.ASEC_SEC_SHORT_NAME = ch.ASEC_SEC_SHORT_NAME
			and ch.PORTFOLIO_ID = 'EQYALL'
			and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_COMPOSITE_LTHOLDINGS ch where ch.PORTFOLIO_ID = 'EQYALL')
			)
--where t.ic_action = ''


DROP TABLE #TMP
DROP TABLE #ic_most_recent