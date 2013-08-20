IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expPortfolioSecurityValuation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expPortfolioSecurityValuation]
GO

/****** Object:  StoredProcedure [dbo].[expPortfolioSecurityValuation]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expPortfolioSecurityValuation](
@PORTFOLIO varchar(20) = '',
@EFFECTIVE datetime = '2012-12-31'
)
as

declare @VALUE decimal(22,8) 

declare @RelPeriodM1 int = year(@EFFECTIVE)-1
declare @RelPeriod int = year(@EFFECTIVE)
declare @RelPeriod1 int = year(@EFFECTIVE)+1

select max(plh.PORTFOLIO_DATE) as EFFECTIVE, 
	max(plh.PORTFOLIO_ID) as PORTFOLIO_ID, 
	plh.issuer_id as ISSUER_ID,
	--'' as SHORT_NAME, --plh.ASEC_SEC_SHORT_NAME as SHORT_NAME, 
	max(sbh.issuer_proxy) as SECURITY_ID,
	--max(sbh.ISSUER_NAME) as NAME, 
	MAX(plh.SECURITYTHEMECODE) as SECURITYTHEMECODE, 
	-- as TICKER,
	--max(sbh.ISO_COUNTRY_CODE as COUNTRY, 
	--sbh.ASHMOREEMM_PRIMARY_ANALYST as ANALYST,
	sum(plh.DIRTY_VALUE_PC) as DIRTY_VALUE_PC
into #PH
from dbo.GF_PORTFOLIO_LTHOLDINGS_HISTORY plh
left join dbo.GF_SECURITY_BASEVIEW_HISTORY sbh on plh.ASEC_SEC_SHORT_NAME = sbh.ASEC_SEC_SHORT_NAME and sbh.EFFECTIVE_DATE = @EFFECTIVE
where plh.PORTFOLIO_ID = @PORTFOLIO
and plh.PORTFOLIO_DATE = @EFFECTIVE
group by plh.issuer_id


select ph.EFFECTIVE, 
	ph.PORTFOLIO_ID, 
	ph.ISSUER_ID,
	sbh.ASEC_SEC_SHORT_NAME as SHORT_NAME, 
	ph.SECURITY_ID,
	sbh.ISSUER_NAME as NAME, 
	ph.SECURITYTHEMECODE, 
	sbh.TICKER,
	sbh.ISO_COUNTRY_CODE as COUNTRY, 
	sbh.ASHMOREEMM_PRIMARY_ANALYST as ANALYST,
	ph.DIRTY_VALUE_PC
into #PHIssuer
from #PH ph
left join dbo.GF_SECURITY_BASEVIEW_HISTORY sbh on ph.SECURITY_ID = sbh.SECURITY_ID and sbh.EFFECTIVE_DATE = @EFFECTIVE
order by sbh.SECURITY_ID


select @VALUE = SUM(dirty_value_pc)
from #PHIssuer
group by PORTFOLIO_ID

select ph.*, 
	ph.DIRTY_VALUE_PC/@VALUE as WEIGHT
into #PH1
from #PHIssuer ph
order by WEIGHT desc


--Extract period_financial data for securities/issuers in portfolio
select pfh.*
into #PFH
from dbo.PERIOD_FINANCIALS_HISTORY pfh
where pfh.DATA_SOURCE = 'PRIMARY'
and pfh.CURRENCY = 'USD' 
and pfh.EFFECTIVE_DATE = @EFFECTIVE
and (pfh.SECURITY_ID in (select SECURITY_ID from #PH1) or pfh.ISSUER_ID in (select ISSUER_ID from #PH1))


--Select "Current" fields
select * 
into #Current
from #PFH pfh
where pfh.DATA_ID in (185,207,209,212,301,304,306,308,309)

--Select security based fields
select * 
into #Security
from #PFH pfh
where pfh.DATA_ID in (166,164,192)

--Select issuer based fields
select * 
into #Issuer
from #PFH pfh
where pfh.DATA_ID in (104,133,149,177,124,290)


--Pivot data
select p.*, 
	c1.amount as MktCap, 
	c2.amount as TrailingPE,
	c3.amount as TrailingPBV,
--	c4.amount as TrailingROE,
	c5.amount as ForwardPE,
	c6.amount as ForwardPBV,
--	c7.amount as ForwardROE,
	s1.amount as PEM1,
	s2.amount as PE,
	s3.amount as PE1,	
	s4.amount as PBVM1,
	s5.amount as PBV,
	s6.amount as PBV1,	
	s7.amount as DYM1,
	s8.amount as DY,
	s9.amount as DY1,
	i1.amount as ROEM1,	
	i2.amount as ROE,	
	i3.amount as ROE1,	
	i4.amount as EGM1,	
	i5.amount as EG,	
	i6.amount as EG1,
	c7.amount as ForwardEarn,
	c8.amount as ForwardBV,
	i7.amount as EARN,
	i8.amount as EARN1,
	i9.amount as BV,
	i10.amount as BV1,
	i11.amount as DIV,
	i12.amount as DIV1,
	i13.amount as DEBTEQYM1,
	i14.amount as DEBTEQY,
	i15.amount as DEBTEQY1	
into #PortfolioHistory	
from #PH1 p
left join #Current c1 on p.security_id = c1.security_id and c1.data_id = 185  --Market Cap
left join #Current c2 on p.security_id = c2.security_id and c2.data_id = 207  --Trailing P/E
left join #Current c3 on p.security_id = c3.security_id and c3.data_id = 209  --Trailing P/BV
--left join #Current c4 on p.security_id = c4.security_id and c4.data_id = 212  --Trailing ROE
left join #Current c5 on p.security_id = c5.security_id and c5.data_id = 308  --Forward P/E
left join #Current c6 on p.security_id = c6.security_id and c6.data_id = 306  --Forward P/BV
left join #Current c7 on p.issuer_id = c7.issuer_id and c7.data_id = 304  --Forward Earnings
left join #Current c8 on p.issuer_id = c8.issuer_id and c8.data_id = 301  --Forward BV
--left join #Current c7 on p.security_id = c7.security_id and c7.data_id = 309  --Forward ROE
left join #Security s1 on p.security_id = s1.security_id and s1.data_id = 166 and s1.period_year = @RelPeriodM1  --PE Year-1  
left join #Security s2 on p.security_id = s2.security_id and s2.data_id = 166 and s2.period_year = @RelPeriod  --PE Year
left join #Security s3 on p.security_id = s3.security_id and s3.data_id = 166 and s3.period_year = @RelPeriod1 --PE Year+1 
left join #Security s4 on p.security_id = s4.security_id and s4.data_id = 164 and s4.period_year = @RelPeriodM1  --PBV Year-1  
left join #Security s5 on p.security_id = s5.security_id and s5.data_id = 164 and s5.period_year = @RelPeriod  --PBV Year
left join #Security s6 on p.security_id = s6.security_id and s6.data_id = 164 and s6.period_year = @RelPeriod1 --PBV Year+1 
left join #Security s7 on p.security_id = s7.security_id and s7.data_id = 192 and s7.period_year = @RelPeriodM1  --DY Year-1  
left join #Security s8 on p.security_id = s8.security_id and s8.data_id = 192 and s8.period_year = @RelPeriod  --DY Year
left join #Security s9 on p.security_id = s9.security_id and s9.data_id = 192 and s9.period_year = @RelPeriod1 --DY Year+1 
left join #Issuer i1 on p.issuer_id = i1.issuer_id and i1.data_id = 133 and i1.period_year = @RelPeriodM1  --ROE Year-1  
left join #Issuer i2 on p.issuer_id = i2.issuer_id and i2.data_id = 133 and i2.period_year = @RelPeriod  --ROE Year 
left join #Issuer i3 on p.issuer_id = i3.issuer_id and i3.data_id = 133 and i3.period_year = @RelPeriod1  --ROE Year+1  
left join #Issuer i4 on p.issuer_id = i4.issuer_id and i4.data_id = 177 and i4.period_year = @RelPeriodM1  --Earn Gr Year-1  
left join #Issuer i5 on p.issuer_id = i5.issuer_id and i5.data_id = 177 and i5.period_year = @RelPeriod  --Earn Gr Year 
left join #Issuer i6 on p.issuer_id = i6.issuer_id and i6.data_id = 177 and i6.period_year = @RelPeriod1  --Earn Gr Year+1  
left join #Issuer i7 on p.issuer_id = i7.issuer_id and i7.data_id = 290 and i7.period_year = @RelPeriod  --Earning Year  
left join #Issuer i8 on p.issuer_id = i8.issuer_id and i8.data_id = 290 and i8.period_year = @RelPeriod1  --Earning Year+1  
left join #Issuer i9 on p.issuer_id = i9.issuer_id and i9.data_id = 104 and i9.period_year = @RelPeriod  --BV Year  
left join #Issuer i10 on p.issuer_id = i10.issuer_id and i10.data_id = 104 and i10.period_year = @RelPeriod1  --BV Year+1  
left join #Issuer i11 on p.issuer_id = i11.issuer_id and i11.data_id = 124 and i11.period_year = @RelPeriod  --DIV Year  
left join #Issuer i12 on p.issuer_id = i12.issuer_id and i12.data_id = 124 and i12.period_year = @RelPeriod1  --DIV Year+1  
left join #Issuer i13 on p.issuer_id = i13.issuer_id and i13.data_id = 149 and i13.period_year = @RelPeriodM1  --Debt/Equity Year-1  
left join #Issuer i14 on p.issuer_id = i14.issuer_id and i14.data_id = 149 and i14.period_year = @RelPeriod  --Debt/Equity Year  
left join #Issuer i15 on p.issuer_id = i15.issuer_id and i15.data_id = 149 and i15.period_year = @RelPeriod1  --Debt/Equity Year+1  


declare @PEM1 char(6) =   'PE' + cast(@RelPeriodM1 as char(4)) 
declare @PE char(6) =   'PE' + cast(@RelPeriod as char(4))
declare @PE1 char(6) =   'PE' + cast(@RelPeriod1 as char(4)) 
declare @PBVM1 char(7) =   'PBV' + cast(@RelPeriodM1 as char(4)) 
declare @PBV char(7) =   'PBV' + cast(@RelPeriod as char(4))
declare @PBV1 char(7) =   'PBV' + cast(@RelPeriod1 as char(4)) 
declare @DYM1 char(6) =   'DY' + cast(@RelPeriodM1 as char(4)) 
declare @DY char(6) =   'DY' + cast(@RelPeriod as char(4))
declare @DY1 char(6) =   'DY' + cast(@RelPeriod1 as char(4)) 
declare @ROEM1 char(7) =   'ROE' + cast(@RelPeriodM1 as char(4)) 
declare @ROE char(7) =   'ROE' + cast(@RelPeriod as char(4))
declare @ROE1 char(7) =   'ROE' + cast(@RelPeriod1 as char(4))
declare @EGM1 char(10) =   'EARNGR' + cast(@RelPeriodM1 as char(4)) 
declare @EG char(10) =   'EARNGR' + cast(@RelPeriod as char(4))
declare @EG1 char(10) =   'EARNGR' + cast(@RelPeriod1 as char(4))
declare @EARN char(8) =   'EARN' + cast(@RelPeriod as char(4))
declare @EARN1 char(8) =   'EARN' + cast(@RelPeriod1 as char(4))
declare @BV char(6) =   'BV' + cast(@RelPeriod as char(4))
declare @BV1 char(6) =   'BV' + cast(@RelPeriod1 as char(4))
declare @DIV char(7) =   'DIV' + cast(@RelPeriod as char(4))
declare @DIV1 char(7) =   'DIV' + cast(@RelPeriod1 as char(4))
declare @DEBTEQYM1 char(11) =   'DEBTEQY' + cast(@RelPeriodM1 as char(4))
declare @DEBTEQY char(11) =   'DEBTEQY' + cast(@RelPeriod as char(4))
declare @DEBTEQY1 char(11) =   'DEBTEQY' + cast(@RelPeriod1 as char(4))

exec tempdb..sp_rename '#PortfolioHistory.PEM1',@PEM1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.PE',@PE,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.PE1',@PE1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.PBVM1',@PBVM1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.PBV',@PBV,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.PBV1',@PBV1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DYM1',@DYM1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DY',@DY,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DY1',@DY1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.ROEM1',@ROEM1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.ROE',@ROE,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.ROE1',@ROE1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.EGM1',@EGM1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.EG',@EG,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.EG1',@EG1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.EARN',@EARN,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.EARN1',@EARN1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.BV',@BV,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.BV1',@BV1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DIV',@DIV,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DIV1',@DIV1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DEBTEQYM1',@DEBTEQYM1,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DEBTEQY',@DEBTEQY,'COLUMN'
exec tempdb..sp_rename '#PortfolioHistory.DEBTEQY1',@DEBTEQY1,'COLUMN'

select * 
from #PortfolioHistory
order by weight desc

drop table #PortfolioHistory

drop table #PH
drop table #PH1
drop table #PFH

drop table #Current
drop table #Security
drop table #Issuer

go
