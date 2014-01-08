

/****** Object:  StoredProcedure [dbo].[RetrieveCompositeDetailsData]    Script Date: 12/12/2013 16:05:16 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RetrieveCompositeDetailsData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RetrieveCompositeDetailsData]
GO




/****** Object:  StoredProcedure [dbo].[RetrieveCompositeDetailsData]    Script Date: 12/12/2013 16:05:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[RetrieveCompositeDetailsData] 
	(
		@portfolio_id varchar(25),
		@portfolio_date datetime,
		@filterType varchar(20),
		@filterValue varchar(50),
		@excludeCash int,
		@isLookThru int,
		@includeBenchmark int
	)
AS
BEGIN
	declare @benchmarkId varchar(50);
	declare @sumbenchmarkWeight decimal(22,8);
	Create table #PortfolioDetailsData
	(
		FromDate datetime,
		AsecSecShortName nvarchar(16),
		IssueName nvarchar(255),
		Issuer_Proxy varchar(20),
		PortfolioPath varchar(500),
		PfcHoldingPortfolio varchar(10),
		PortfolioId varchar(10),
		IsExpanded int,
		Ticker nvarchar(255),
		SecurityThemeCode varchar(10),
		A_Sec_Instr_Type nvarchar(255),
		SecurityType varchar(10),
		ProprietaryRegionCode varchar(10),
		IsoCountryCode varchar(3),
		SectorName varchar(60),
		IndustryName varchar(60),
		SubIndustryName varchar(50),
		BalanceNominal decimal(22,8),
		DirtyValuePC decimal(22,8),
		PortfolioDirtyValuePC decimal(22,8),
		TradingCurrency varchar(3),
		AshEmmModelWeight decimal(22,8),
		PortfolioWeight decimal(22,8),
		BenchmarkWeight decimal(22,8),
		MarketCapUSD decimal(22,8),
		ActivePosition decimal(22,8),
		ReAshEmmModelWeight decimal(22,8),
		RePortfolioWeight decimal(22,8),
		ReBenchmarkWeight decimal(22,8),
		SType varchar(50),
		SecurityId varchar(20),
		MarketCap decimal(22,8),
		Upside decimal(22,8),
		ForwardPE decimal(22,8),
		ForwardPBV decimal(22,8),
		ForwardEB_EBITDA decimal(22,8),
		RevenueGrowthCurrentYear decimal(22,8),
		RevenueGrowthNextYear decimal(22,8),
		NetIncomeGrowthCurrentYear decimal(22,8),
		NetIncomeGrowthNextYear decimal(22,8),
		ROE decimal(22,8),
		NetDebtEquity decimal(22,8),
		FreecashFlowMargin decimal(22,8),
		IssuerId varchar(20),
		IssuerName nvarchar(255),
		FairValue decimal(22,8)
	)
	Declare @HoldingsDetailsData table
	(
		FromDate datetime,
		AsecSecShortName nvarchar(16),
		IssueName nvarchar(255),
		Issuer_Proxy varchar(20),
		PortfolioPath varchar(500),
		PfcHoldingPortfolio varchar(10),
		PortfolioId varchar(10),
		IsExpanded int,
		Ticker nvarchar(255),
		SecurityThemeCode varchar(10),
		A_Sec_Instr_Type nvarchar(255),
		SecurityType varchar(10),
		ProprietaryRegionCode varchar(10),
		IsoCountryCode varchar(3),
		SectorName varchar(60),
		IndustryName varchar(60),
		SubIndustryName varchar(50),
		BalanceNominal decimal(22,8),
		DirtyValuePC decimal(22,8),
		PortfolioDirtyValuePC decimal(22,8),
		TradingCurrency varchar(3),
		AshEmmModelWeight decimal(22,8),
		PortfolioWeight decimal(22,8),
		BenchmarkWeight decimal(22,8),
		MarketCapUSD decimal(22,8),
		ActivePosition decimal(22,8),
		ReAshEmmModelWeight decimal(22,8),
		RePortfolioWeight decimal(22,8),
		ReBenchmarkWeight decimal(22,8),
		SType varchar(50),
		SecurityId varchar(20),
		MarketCap decimal(22,8),
		Upside decimal(22,8),
		ForwardPE decimal(22,8),
		ForwardPBV decimal(22,8),
		ForwardEB_EBITDA decimal(22,8),
		RevenueGrowthCurrentYear decimal(22,8),
		RevenueGrowthNextYear decimal(22,8),
		NetIncomeGrowthCurrentYear decimal(22,8),
		NetIncomeGrowthNextYear decimal(22,8),
		ROE decimal(22,8),
		NetDebtEquity decimal(22,8),
		FreecashFlowMargin decimal(22,8),
		IssuerId varchar(20),
		IssuerName nvarchar(255),
		FairValue decimal(22,8)
	)
	
	
	select * into #CompositeLTTemp from gf_composite_ltholdings where 1=0;
	select * into #BenchmarkTemp from  gf_benchmark_holdings where 1=0;
	
	
	
	if @excludeCash = 1 --- exclude cash
		begin
			if (@filterType='Region' )
			begin
				insert into #CompositeLTTemp  select * from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if (@filterType='Country')
			begin
				insert into #CompositeLTTemp select *  from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select *   from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if	(@filterType='Industry')
			begin
				insert into #CompositeLTTemp  select * from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if (@filterType='Sector') 
			begin
				insert into #CompositeLTTemp  select * from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY' 	
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY' 	
			end
			else
			begin
				insert into #CompositeLTTemp  select * from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
				
		end
	else  -- include cash
		begin 
			if (@filterType='Region' )
			begin
				insert into #CompositeLTTemp select *  from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue 
			end
			else if (@filterType='Country')
			begin
				insert into #CompositeLTTemp select *  from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue 
			end	
			else if	(@filterType='Industry')
			begin
				insert into #CompositeLTTemp select *  from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue 
			end
			else if (@filterType='Sector') 
			begin
				insert into #CompositeLTTemp select *  from gf_composite_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	
			end
			else
			begin
				insert into #CompositeLTTemp  select *  from gf_composite_ltholdings where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date 	
				set @benchmarkId =  (select distinct benchmark_id from #CompositeLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings where	benchmark_id = @benchmarkId and portfolio_date = @portfolio_date 	
			end
		end
	set @sumbenchmarkWeight = (select sum(benchmark_weight) from #BenchmarkTemp);
    insert into #PortfolioDetailsData
    (AsecSecShortName,IssueName,Ticker,PortfolioId,portfoliopath,pfcholdingportfolio,ProprietaryRegionCode,IsoCountryCode,SectorName,
    IndustryName,SubIndustryName,MarketCapUSD,SecurityType,BalanceNominal,DirtyValuePC,BenchmarkWeight,IssuerId,issuer_proxy,IssuerName,SecurityId)
    select p.asec_sec_short_name,p.issue_name,p.ticker,p.portfolio_id,p.portfolio_id,p.portfolio_id,p.ashemm_prop_Region_code,
		p.iso_country_code,
		p.gics_sector_name,
		p.gics_industry_name,
		p.gics_sub_industry_name,
		p.market_Cap_in_usd,
		p.security_type,
		p.balance_nominal,
		p.dirty_Value_pc,
		(case when @filterType is null or @filterType = 'Show Everything' then isnull(b.benchmark_weight,0)
			else
				 CASE WHEN @sumbenchmarkWeight = 0.0 THEN 0
					ELSE  isnull(b.benchmark_weight,0)*100/@sumbenchmarkWeight
				 END
		
		end) benchmarkweight,
		p.issuer_id,
		s.issuer_proxy,
		ltrim(rtrim(s.issuer_name)),
		s.security_id
	 from #CompositeLTTemp p
	 inner join gf_Security_baseview s on s.asec_Sec_short_name  = p.asec_sec_short_name 
     left outer join #BenchmarkTemp b on b.asec_Sec_short_name = p.asec_sec_short_name ;	
			
			
	
    --select * from #PortfolioLTTemp
    --select * from #BenchmarkTemp
    
  
     
        
    if( @includeBenchmark = 1) -- include benchmark
    begin
  
    insert into #PortfolioDetailsData
    (AsecSecShortName,IssueName,Ticker,ProprietaryRegionCode,IsoCountryCode,SectorName,
    IndustryName,SubIndustryName,MarketCapUSD,SecurityType,BalanceNominal,DirtyValuePC,BenchmarkWeight,stype,IssuerId,issuer_proxy,IssuerName,SecurityId)
     select b.asec_sec_short_name,b.issue_name,b.ticker,
		b.ashemm_prop_Region_code,
		b.iso_country_code,
		b.gics_sector_name,
		b.gics_industry_name,
		b.gics_sub_industry_name,
		b.market_Cap_in_usd,
		b.security_type,
		b.balance_nominal,
		b.dirty_Value_pc,
		
		(case when @filterType is null or @filterType = 'Show Everything' then isnull(b.benchmark_weight,0)
			else
				 CASE WHEN @sumbenchmarkWeight = 0.0 THEN 0
					ELSE  isnull(b.benchmark_weight,0)*100/@sumbenchmarkWeight
				 END
		
		end) benchmarkweight,
		
		'BENCHMARK',
		b.issuer_id,
		s.issuer_proxy,
		s.issuer_name,
		s.security_id
     from #BenchmarkTemp b
     inner join gf_Security_baseview s on s.asec_sec_short_name  = b.asec_sec_short_name 
    WHERE NOT EXISTS(SELECT * FROM #CompositeLTTemp p WHERE p.asec_sec_short_name  = b.asec_sec_short_name  )
    end
	
	exec dbo.[RetrieveExternalResearchData] 
	
	insert into @HoldingsDetailsData
	select * from #PortfolioDetailsData
    select * from @HoldingsDetailsData
    drop table #PortfolioDetailsData
END



--exec [RetrieveCompositeDetailsData] 'EQYALL','07/31/2013',null,null,0,0,1
GO


