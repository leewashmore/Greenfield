
/****** Object:  StoredProcedure [dbo].[RetrieveExternalResearchData]    Script Date: 12/12/2013 16:06:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RetrieveExternalResearchData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RetrieveExternalResearchData]
GO



/****** Object:  StoredProcedure [dbo].[RetrieveExternalResearchData]    Script Date: 12/12/2013 16:06:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[RetrieveExternalResearchData] 
	/*(	@portfolio_id varchar(25),
		@portfolio_date datetime,
		@filterType varchar(20),
		@filterValue varchar(50),
		@excludeCash int,
		@isComposite int,
		@isLookThru int,
		@includeBenchmark int
	)*/
AS
BEGIN
 --update fair value upside 
	update #PortfolioDetailsData 
	set upside = f.upside * 100
	from #PortfolioDetailsData as por
	inner join gf_Security_baseview s on s.asec_Sec_Short_name COLLATE DATABASE_DEFAULT = por.AsecSecShortName COLLATE DATABASE_DEFAULT 
	inner join fair_Value f on f.security_id = s.security_id
	where f.value_type = 'PRIMARY'

--update ForwardEB_EBITDA forwardpb forwardpe marketcap
update #PortfolioDetailsData 
	set marketcap = x.marketcap ,
		forwardpe = x.forwardpe ,
		forwardpbv = x.forwardpbv,
		ForwardEB_EBITDA=x.ForwardEB_EBITDA
		
	from #PortfolioDetailsData as por
	inner join (
		select asec_sec_short_name,[185] as marketcap ,[187] as forwardpe, [188] as forwardpbv , [198] as ForwardEB_EBITDA from
		(	select data_id,s.asec_sec_short_name , amount from period_financials p 
			inner join gf_security_baseview s on s.security_id = p.security_id
			where  CURRENCY = 'USD'
			and PERIOD_TYPE = 'C'
			and DATA_SOURCE = 'PRIMARY'
			and DATA_ID IN (185,187,198,188)
				
		) a
		pivot 
		(
			sum(AMOUNT)
			for data_id in ([185],[187],[198],[188])
		) as p
	)x on x.asec_sec_short_name COLLATE DATABASE_DEFAULT = por.AsecSecShortName COLLATE DATABASE_DEFAULT



--update current year issuer level valuations
update #PortfolioDetailsData 
	set RevenueGrowthCurrentYear = x.RevenueGrowth *100,
		NetIncomeGrowthCurrentYear = x.NetIncomeGrowth * 100,
		ROE = x.ROE,
		NetDebtEquity=x.NetDebtEquity,
		FreecashFlowMargin = x.FreeCashflowMargin
	from #PortfolioDetailsData as por
	Inner join (
		select Issuer_Id,[178] AS RevenueGrowth ,[177] As NetIncomeGrowth   ,[133] As ROE,[149] As NetDebtEquity,[146] as FreeCashflowMargin from 
		(select data_id,issuer_id , amount from period_financials
			where CURRENCY = 'USD'
			and PERIOD_TYPE = 'A'
			and FISCAL_TYPE='CALENDAR'
			and DATA_SOURCE = 'PRIMARY'
			and DATA_ID IN (178,177,133,149,146)
			and period_year=year(getdate())
		) a
		pivot 
		(
			sum(AMOUNT)
			for data_id in ( [178] ,[177],[133] ,[149] ,[146] )
		) as p
	) x on x.issuer_id COLLATE DATABASE_DEFAULT = por.IssuerId COLLATE DATABASE_DEFAULT

--update next year issuer level valuations
update #PortfolioDetailsData 
	set RevenueGrowthNextYear = x.RevenueGrowth *100,
		NetIncomeGrowthNextYear = x.NetIncomeGrowth * 100
	from #PortfolioDetailsData as por
	Inner join (
		select Issuer_Id,[178] AS RevenueGrowth ,[177] As NetIncomeGrowth   from 
		(select data_id,issuer_id , amount from period_financials
			where CURRENCY = 'USD'
			and PERIOD_TYPE = 'A'
			and FISCAL_TYPE='CALENDAR'
			and DATA_SOURCE = 'PRIMARY'
			and DATA_ID IN (178,177)
			and period_year=year(getdate())+1
		) a
		pivot 
		(
			sum(AMOUNT)
			for data_id in ( [178] ,[177])
		) as p
	) x on x.issuer_id COLLATE DATABASE_DEFAULT = por.IssuerId COLLATE DATABASE_DEFAULT




	
END


GO


