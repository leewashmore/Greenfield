create procedure updateICMonitorTable
as 
begin
	declare @buffer as decimal(32,6) = .02

	select ic.* 
	into #ic_most_recent
	from IC_TABLE ic ,
		(select ic.ISSUER_ID, MAX(ic.update_date)as update_date from IC_TABLE ic group by ic.ISSUER_ID) a
	where
		a.ISSUER_ID = ic.ISSUER_ID
		and a.update_date = ic.update_date

	select im.ISSUER_ID
		, im.SECURITY_ID
		, im.CURR_ACTION
		, pf.DATA_SOURCE
		, pf.AMOUNT
		, imr.ic_buy
		, imr.ic_sell
		, pf.ROOT_SOURCE_DATE
	into #tmp
	from IC_MONITORING im
		left join #ic_most_recent imr on imr.issuer_id = im.ISSUER_ID
		left join PERIOD_FINANCIALS pf on ( pf.SECURITY_ID = im.SECURITY_ID
				and imr.IC_MEASURE = pf.DATA_ID
				and pf.CURRENCY = 'USD'
				and pf.PERIOD_TYPE = 'C'
				and (
					isnull(im.DATA_SOURCE,'PRIMARY') =  pf.DATA_SOURCE
					OR
					isnull(im.DATA_SOURCE,'REUTERS') =  pf.DATA_SOURCE
					)
				)
	where im.UPDATE_DATE = (select MAX(im.UPDATE_DATE) from IC_MONITORING im)
	order by im.ISSUER_ID			

	insert into IC_MONITORING (ISSUER_ID,SECURITY_ID,DATA_SOURCE,MEASURE_VALUE,CURR_ACTION,UPDATE_DATE)
	select t.ISSUER_ID
		 , sb.issuer_proxy
		 , t.DATA_SOURCE
		 , t.amount as MEASURE_VALUE
		 , case 
			when t.amount is null then 'insufficient data'
			when t.CURR_ACTION = 'HOLD' then (case when t.amount > (t.ic_sell * (1-@buffer)) then 'SELL' else 'HOLD' END) 
			WHEN t.CURR_ACTION = 'BUY' then (case when t.amount > (t.ic_sell * (1-@buffer)) then 'SELL' else 'BUY' END) 
			WHEN t.CURR_ACTION = 'SELL' then (case when t.amount < (t.ic_buy * (1+@buffer)) then 'BUY' else 'SELL' END) 
			WHEN t.CURR_ACTION = 'WATCH' then (
				case when (ch.DIRTY_VALUE_PC > 0 and  t.amount < (t.ic_sell * (1-@buffer))) then 'HOLD'
					when  (ch.DIRTY_VALUE_PC > 0 and  t.amount >= (t.ic_sell * (1-@buffer))) then 'SELL'
					when  (ISNULL(ch.DIRTY_VALUE_PC,0)=0 and  t.amount <= (t.ic_buy * (1+@buffer))) then 'BUY'
					when  (ISNULL(ch.DIRTY_VALUE_PC,0)=0 and  t.amount > (t.ic_buy * (1+@buffer))) then 'WATCH'
					END)
			WHEN t.CURR_ACTION = 'insufficient data' then (
				case when (ch.DIRTY_VALUE_PC > 0 and  t.amount < (t.ic_sell * (1-@buffer))) then 'HOLD'
					when  (ch.DIRTY_VALUE_PC > 0 and  t.amount >= (t.ic_sell * (1-@buffer))) then 'SELL'
					when  (ISNULL(ch.DIRTY_VALUE_PC,0)=0 and  t.amount <= (t.ic_buy * (1+@buffer))) then 'BUY'
					when  (ISNULL(ch.DIRTY_VALUE_PC,0)=0 and  t.amount > (t.ic_buy * (1+@buffer))) then 'WATCH'
					END)
			ELSE t.CURR_ACTION end 
			as CURR_ACTION
			,case when t.ROOT_SOURCE_DATE is null then (SELECT MAX(ROOT_SOURCE_DATE) from #tmp) else t.ROOT_SOURCE_DATE end as UPDATE_DATE 
	from #tmp t
		left join GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = t.security_id
		left join GF_COMPOSITE_LTHOLDINGS ch on (sb.ASEC_SEC_SHORT_NAME = ch.ASEC_SEC_SHORT_NAME
				and ch.PORTFOLIO_ID = 'EQYALL'
				and ch.PORTFOLIO_DATE = (select MAX(ch.PORTFOLIO_DATE) from GF_COMPOSITE_LTHOLDINGS ch where ch.PORTFOLIO_ID = 'EQYALL')
				)
	--where t.curr_action = 'insufficient data'

	DROP TABLE #TMP
	DROP TABLE #ic_most_recent
end