

/****** Object:  StoredProcedure [dbo].[RetrievePortfolioDetailsData]    Script Date: 12/12/2013 16:04:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RetrievePortfolioDetailsData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RetrievePortfolioDetailsData]
GO


/****** Object:  StoredProcedure [dbo].[RetrievePortfolioDetailsData]    Script Date: 12/12/2013 16:04:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[RetrievePortfolioDetailsData] 
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
		AsecSecShortName nvarchar(255),
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
	
	 declare @HoldingsDetailsData table 
	(
		FromDate datetime,
		AsecSecShortName nvarchar(255),
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
	
	select * into #PortfolioLTTemp from gf_portfolio_ltholdings where 1=0;
	select * into #BenchmarkTemp from  gf_benchmark_holdings where 1=0;
	select * into #PortfolioTemp from gf_portfolio_holdings where 1=0;
	if  @isLookThru = 0  -- no look thru
	begin
		/* no look thru*******************************/
		if @excludeCash = 1 --- exclude cash
		begin
			if (@filterType='Region' )
			begin
				insert into #PortfolioTemp  select * from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if (@filterType='Country')
			begin
				insert into #PortfolioTemp select *  from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select *   from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if	(@filterType='Industry')
			begin
				insert into #PortfolioTemp  select * from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if (@filterType='Sector') 
			begin
				insert into #PortfolioTemp  select * from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY' 	
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY' 	
			end
			else
			begin
				insert into #PortfolioTemp  select * from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
				
		end
	else  -- include cash
		begin 
			if (@filterType='Region' )
			begin
				insert into #PortfolioTemp select *  from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue 
			end
			else if (@filterType='Country')
			begin
				insert into #PortfolioTemp select *  from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue 
			end	
			else if	(@filterType='Industry')
			begin
				insert into #PortfolioTemp select *  from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue 
			end
			else if (@filterType='Sector') 
			begin
				insert into #PortfolioTemp select *  from gf_portfolio_holdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	
			end
			else
			begin
				insert into #PortfolioTemp  select *  from gf_portfolio_holdings where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date 	
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings where	benchmark_id = @benchmarkId and portfolio_date = @portfolio_date 	
			end
		end
	set @sumbenchmarkWeight = (select sum(benchmark_weight) from #BenchmarkTemp);
    insert into #PortfolioDetailsData
    (AsecSecShortName,IssueName,Ticker,PortfolioId, portfoliopath, pfcholdingportfolio,ProprietaryRegionCode,IsoCountryCode,SectorName,
    IndustryName,SubIndustryName,MarketCapUSD,SecurityType,BalanceNominal,DirtyValuePC,BenchmarkWeight,IssuerId,issuer_proxy,IssuerName,SecurityId)
    select p.asec_sec_short_name,p.issue_name,p.ticker,p.portfolio_id,p.portfolio_id,p.portfolio_id,
		p.ashemm_prop_Region_code,
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
		rtrim(ltrim(s.Issuer_Name)),
		s.Security_Id
	 from #PortfolioTemp p
	 inner join GF_SECURITY_BASEVIEW s on s.asec_Sec_Short_name = p.asec_Sec_short_name
     left outer join #BenchmarkTemp b on b.asec_Sec_short_name = p.asec_sec_short_name;	
     	/**********************End of  no Look Thru ********************************/
	end
	
	
			
	
		
		
    
	else
	begin
	/************************Look Thru ********************************/
	if @excludeCash = 1 --- exclude cash
		begin
			if (@filterType='Region' )
			begin
				insert into #PortfolioLTTemp  select * from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<> 'LOC_CCY'
			end
			else if (@filterType='Country')
			begin
				insert into #PortfolioLTTemp select *  from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue and upper(SECURITYTHEMECODE)<> 'CASH' and upper(SECURITYTHEMECODE)<> 'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select *   from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if	(@filterType='Industry')
			begin
				insert into #PortfolioLTTemp  select * from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
			else if (@filterType='Sector') 
			begin
				insert into #PortfolioLTTemp  select * from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY' 	
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY' 	
			end
			else
			begin
				insert into #PortfolioLTTemp  select * from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and upper(SECURITYTHEMECODE)<>'CASH' and upper(SECURITYTHEMECODE)<>'LOC_CCY'
			end
				
		end
	else  -- include cash
		begin 
			if (@filterType='Region' )
			begin
				insert into #PortfolioLTTemp select *  from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ASHEMM_PROP_REGION_CODE = @filterValue 
			end
			else if (@filterType='Country')
			begin
				insert into #PortfolioLTTemp select *  from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and ISO_COUNTRY_CODE = @filterValue 
			end	
			else if	(@filterType='Industry')
			begin
				insert into #PortfolioLTTemp select *  from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue 
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_INDUSTRY_NAME = @filterValue 
			end
			else if (@filterType='Sector') 
			begin
				insert into #PortfolioLTTemp select *  from gf_portfolio_ltholdings	where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings		where benchmark_id = @benchmarkId and portfolio_date = @portfolio_date and GICS_SECTOR_NAME = @filterValue	
			end
			else
			begin
				insert into #PortfolioLTTemp  select *  from gf_portfolio_ltholdings where portfolio_id = @portfolio_id and portfolio_date = @portfolio_date 	
				set @benchmarkId =  (select distinct benchmark_id from #PortfolioLTTemp)
				insert into #BenchmarkTemp select * from gf_benchmark_holdings where	benchmark_id = @benchmarkId and portfolio_date = @portfolio_date 	
			end
		end
	set @sumbenchmarkWeight = (select sum(benchmark_weight) from #BenchmarkTemp);
    insert into #PortfolioDetailsData
    (AsecSecShortName,IssueName,Ticker,PfcHoldingPortfolio,PortfolioId,PortfolioPath,ProprietaryRegionCode,IsoCountryCode,SectorName,
    IndustryName,SubIndustryName,MarketCapUSD,SecurityType,BalanceNominal,DirtyValuePC,BenchmarkWeight,AshEmmModelWeight,IssuerId,issuer_proxy,IssuerName,SecurityId)
    select p.asec_sec_short_name,p.issue_name,p.ticker,p.A_PFCHOLDINGS_PORLT,p.portfolio_id,p.porpath,
		p.ashemm_prop_Region_code,
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
		
		p.ash_emm_model_Weight,
		p.issuer_id,
		s.issuer_proxy,
		rtrim(ltrim(s.Issuer_Name)),
		s.security_id
	 from #PortfolioLTTemp p
	 inner join GF_SECURITY_BASEVIEW s on s.asec_Sec_Short_name = p.asec_Sec_short_name
     left outer join #BenchmarkTemp b on b.asec_Sec_short_name = p.asec_sec_short_name;	
			
			
	end
    --select * from #PortfolioLTTemp
    --select * from #BenchmarkTemp
    
  
     
        
    if( @includeBenchmark = 1) -- include benchmark
    begin
    if @isLookThru = 1 
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
		rtrim(ltrim(s.Issuer_Name)),
		s.security_id
     from #BenchmarkTemp b
     inner join GF_SECURITY_BASEVIEW s on s.asec_sec_short_name = b.asec_sec_short_name
    WHERE NOT EXISTS(SELECT * FROM #PortfolioLTTemp p WHERE P.ASEC_Sec_Short_name = b.asec_Sec_short_name)
    end
    else
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
		rtrim(ltrim(s.Issuer_Name)),
		s.security_id
		
     from #BenchmarkTemp b
      inner join GF_SECURITY_BASEVIEW s on s.asec_sec_short_name = b.asec_sec_short_name
    WHERE NOT EXISTS(SELECT * FROM #PortfolioTemp p WHERE P.ASEC_Sec_Short_name = b.asec_Sec_short_name)
    end
    
    end
    --exec dbo.[RetrieveExternalResearchData] @portfolio_id,@portfolio_date ,@filterType ,@filterValue,@excludeCash,@isComposite,@isLookThru ,@includeBenchmark
    exec dbo.[RetrieveExternalResearchData] 
    insert into @HoldingsDetailsData
    select * from #PortfolioDetailsData
    
    
    /*select AsecSecShortName,IssueName,Ticker,PfcHoldingPortfolio,PortfolioId,PortfolioPath,ProprietaryRegionCode,IsoCountryCode,SectorName,
    IndustryName,SubIndustryName,MarketCapUSD,SecurityType,BalanceNominal,DirtyValuePC,BenchmarkWeight,AshEmmModelWeight,IssuerId,stype,upside,marketcap,ForwardEB_EBITDA,forwardpe,Forwardpbv,RevenueGrowthCurrentYear,NetIncomeGrowthCurrentYear,ROE,NetDebtEquity,FreecashFlowMargin,RevenueGrowthNextYear,NetIncomeGrowthNextYear from #PortfolioDetailsData;*/
    select h.* from @HoldingsDetailsData h
    
	--drop table #PortfolioDetailsData


END

--select * from fair_value

--exec RetrievePortfolioDetailsData 'EMIF','10/31/2013',null,null,0,0,1
GO


