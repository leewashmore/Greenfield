set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00037'
declare @CurrentScriptVersion as nvarchar(100) = '00038'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

IF OBJECT_ID ('[dbo].[Get_PRevenue]') IS NOT NULL
	DROP PROCEDURE [dbo].[Get_PRevenue]
GO

CREATE PROCEDURE [dbo].[Get_PRevenue](@securityId varchar(20),@issuerId varchar(20),@chartTitle varchar(20))	
AS
BEGIN
	SET NOCOUNT ON;
	

-- Extract Price, Shares, FX Rates and Financial Data needed for the charts
Select a.SECURITY_ID, a.PRICE_DATE
		, (a.price/case when a.ADR_CONV = 0.0 then 1.0 else a.ADR_CONV end)/ fx.FX_RATE as USDPrice
		, fx.CURRENCY
		, b.SHARES_OUTSTANDING 
into #SupportData
from PRICES a
		inner join GF_SECURITY_BASEVIEW sb			on sb.SECURITY_ID = a.SECURITY_ID
		inner join FX_RATES fx						on fx.CURRENCY = sb.TRADING_CURRENCY and fx.FX_DATE = a.PRICE_DATE
		inner join ISSUER_SHARES b					on b.SHARES_DATE = fx.FX_DATE and b.ISSUER_ID = sb.ISSUER_ID
where sb.SECURITY_ID = @securityId


-- Financial Data
-- *Extract quarter values from Primary Analyst source first

Select * 
into #PrimaryFinancials
from PERIOD_FINANCIALS 
where (@issuerId is null or ISSUER_ID = @issuerId)
		and PERIOD_END_DATE > dateadd (year, -10, getdate())
		and PERIOD_END_DATE < dateadd (year, 6, getdate())
		and DATA_SOURCE = 'PRIMARY' 
		and FISCAL_TYPE = 'FISCAL'
		and Currency = 'USD'
		and PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
order by ISSUER_ID, DATA_ID, PERIOD_YEAR

-- *next pull quarter estimated values for periods after the oldest PeriodEndDate in the select above
Select * 
into #ConsensusFinancials
from CURRENT_CONSENSUS_ESTIMATES cce
where (@issuerId is null or ISSUER_ID = @issuerId)
		and PERIOD_END_DATE > (select max(PERIOD_END_DATE) as PERIOD_END_DATE from #PrimaryFinancials)
		and DATA_SOURCE = 'REUTERS' 
		and FISCAL_TYPE = 'FISCAL'
		and Currency = 'USD'
		and PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
order by ISSUER_ID, estimate_id, PERIOD_YEAR

   --Joining tables depending upon the chart title
     
  IF(@chartTitle = 'P/E')
			BEGIN
				DECLARE @Earnings varchar(10),@nEstID Integer
					SELECT @Earnings = t.Earnings
					FROM 
					GF_SECURITY_BASEVIEW sb
					inner join [Reuters].[dbo].tblCompanyInfo t
					ON t.XRef = sb.XREF
					WHERE sb.SECURITY_ID = @securityId			
					
					SELECT	@nEstID=
						CASE @Earnings		
							WHEN 'EPS'		THEN  11
							WHEN 'EPSREP'	THEN 13
							WHEN 'EBG'		THEN 12			
						END
					
					--JOIN ABOVE TWO EXTRACTS ORDERED BY PERIODENDDATES INTO “REVENUEVALUES”
					select *
					into #RevenueValuesP_E
					from (
							(Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
									, PERIOD_END_DATE
									, AMOUNT, PERIOD_TYPE
									, PERIOD_YEAR
							from #PrimaryFinancials
							where DATA_ID = 44
							) 
							UNION
							(Select a.PERIOD_TYPE + ' ' + CAST(a.PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
									, a.PERIOD_END_DATE
									, a.AMOUNT, a.PERIOD_TYPE
									, a.PERIOD_YEAR
							from #ConsensusFinancials a
							left join 
							(Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
												, PERIOD_END_DATE
												, AMOUNT, PERIOD_TYPE
												, PERIOD_YEAR
										from #PrimaryFinancials
										where DATA_ID = 44
										) z 
										on z.PERIOD_YEAR = a.PERIOD_YEAR 
											and z.PERIOD_TYPE = a.PERIOD_TYPE 
							where ESTIMATE_ID = @nEstID
								and z.PERIOD_YEAR is null -- Only where we don't find a matching ACTUAL
							) 
						) z	
						
					--JOIN REVENUEVALUES TO SUPPORTDATA ON PERIODENDDATES
					
					SELECT  a.*, b.USDPrice, b.SHARES_OUTSTANDING ,b.PRICE_DATE
					INTO #RevenueSupportP_E
					FROM #RevenueValuesP_E a
					left join #SupportData b ON a.PERIOD_END_DATE = b.PRICE_DATE
					order by PERIOD_YEAR,PERIOD_TYPE
					
					--SELECTING CALCULATED DATA
					SELECT * FROM #RevenueSupportP_E
					
					--Dropping above tables
					DROP TABLE #RevenueValuesP_E
					DROP TABLE #RevenueSupportP_E		
			END
			
  ELSE IF(@chartTitle = 'P/CE')
			BEGIN
			SELECT *
				  INTO #RevenueValuesP_CE
				  FROM (
							Select a.PERIOD_END_DATE
									, b.PERIOD_TYPE
									, b.PERIOD_YEAR 
									, b.PeriodLabel
									,b.AMOUNT
							from #PrimaryFinancials a
							inner join
							   (SELECT PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
									,PERIOD_TYPE
									,PERIOD_YEAR
									,sum(Amount) AS AMOUNT									
								FROM #PrimaryFinancials 
								WHERE DATA_ID = 44 or DATA_ID = 24
								group by PERIOD_TYPE, PERIOD_YEAR) b
								ON a.PERIOD_TYPE = b.PERIOD_TYPE
									and a.PERIOD_YEAR = b.PERIOD_YEAR
									and a.PERIOD_TYPE + ' ' + CAST(a.PERIOD_YEAR AS VARCHAR(4)) = b.PeriodLabel
						) p_ce
						
					--Join RevenueValues to SupportData on PeriodEndDates		
					SELECT  a.*, b.USDPrice, b.SHARES_OUTSTANDING 
					INTO #RevenueSupportP_CE
					FROM #RevenueValuesP_CE a
					left join #SupportData b 
					ON a.PERIOD_END_DATE = b.PRICE_DATE -- Multiple rows for one period label
					ORDER BY PERIOD_YEAR ,PERIOD_TYPE
					
					--SELECTING CALCULATED DATA
					SELECT * FROM #RevenueSupportP_CE
					
					--Dropping above tables
					DROP TABLE #RevenueValuesP_CE
					DROP TABLE #RevenueSupportP_CE		
			
			END
			
  ELSE IF(@chartTitle = 'P/BV')
			BEGIN
					DECLARE @ReutersShares decimal(18,0)
					
					SELECT @ReutersShares = t.OutstandingShares
					FROM 
					GF_SECURITY_BASEVIEW sb
					inner join [Reuters].[dbo].tblCompanyInfo t
					ON t.XRef = sb.XREF
					WHERE sb.SECURITY_ID = @securityId
					
					--print 'ReutersChares	' +Convert(varchar(25),@ReutersShares)
					
					
					--JOIN ABOVE TWO EXTRACTS ORDERED BY PERIOD END DATES INTO “REVENUEVALUES”
					select *
					into #RevenueValuesP_BV
					from (
							(Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
									, PERIOD_END_DATE
									, AMOUNT, PERIOD_TYPE
									, PERIOD_YEAR
							from #PrimaryFinancials
							where DATA_ID = 105
							) 
							UNION
							(Select a.PERIOD_TYPE + ' ' + CAST(a.PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
									, a.PERIOD_END_DATE
									, a.AMOUNT
									, a.PERIOD_TYPE
									, a.PERIOD_YEAR
							from #ConsensusFinancials a
							left join 
							(Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
												,  PERIOD_END_DATE
												,  Amount * @ReutersShares as AMOUNT
												,  PERIOD_TYPE
												,  PERIOD_YEAR
										from #PrimaryFinancials
										where DATA_ID = 105
										) z 
										on z.PERIOD_YEAR = a.PERIOD_YEAR 
											and z.PERIOD_TYPE = a.PERIOD_TYPE 
							where ESTIMATE_ID = 1
								and z.PERIOD_YEAR is null -- Only where we don't find a matching ACTUAL
							) 
						) p_bv	
						
					--JOIN REVENUEVALUES TO SUPPORTDATA ON PERIODENDDATES
					SELECT  a.*, b.USDPrice, b.SHARES_OUTSTANDING 
					INTO #RevenueSupportP_BV
					FROM #RevenueValuesP_BV a
					left join #SupportData b ON a.PERIOD_END_DATE = b.PRICE_DATE -- Multiple rows for one period label
					ORDER BY PERIOD_YEAR ,PERIOD_TYPE
					
					--SELECTING CALCULATED DATA
					SELECT * FROM #RevenueSupportP_BV
					
					--Dropping above tables
					DROP TABLE #RevenueValuesP_BV
					DROP TABLE #RevenueSupportP_BV		
			END	
			
	ELSE IF(@chartTitle = 'FCF Yield')
			BEGIN
			SELECT *
				  INTO #RevenueValuesFCFYield
				  FROM (
							SELECT PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
							, PERIOD_END_DATE, sum(Amount) as Amount, PERIOD_TYPE, PERIOD_YEAR 
							FROM #PrimaryFinancials 
							WHERE DATA_ID = 44 or DATA_ID = 24
							group by PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE
							) fcfYield
							
					--Join RevenueValues to SupportData on PeriodEndDates	
						
					SELECT  a.*, b.USDPrice, b.SHARES_OUTSTANDING 
					INTO #RevenueSupportFCFYield
					FROM #RevenueValuesFCFYield a
					left join #SupportData b ON a.PERIOD_END_DATE = b.PRICE_DATE 
					ORDER BY PERIOD_YEAR ,PERIOD_TYPE
					

					--SELECTING CALCULATED DATA
					
					SELECT * FROM #RevenueSupportFCFYield
					
					--Dropping above tables
					DROP TABLE #RevenueValuesFCFYield
					DROP TABLE #RevenueSupportFCFYield		
			
			END
	ELSE IF(@chartTitle = 'Dividend Yield')
			BEGIN
					DECLARE @ReutersShares2 decimal(18,0)
					
					SELECT @ReutersShares2 = t.OutstandingShares
					FROM 
					GF_SECURITY_BASEVIEW sb
					inner join [Reuters].[dbo].tblCompanyInfo t
					ON t.XRef = sb.XREF
					WHERE sb.SECURITY_ID = @securityId
					
					
					--JOIN ABOVE TWO EXTRACTS ORDERED BY PERIODENDDATES INTO “REVENUEVALUES”
				  SELECT *
				  INTO #RevenueValuesDividend
				  FROM (
						   (Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
									,  PERIOD_END_DATE
									,  AMOUNT
									,  PERIOD_TYPE
									,  PERIOD_YEAR
							from #PrimaryFinancials
							where DATA_ID =	124
							) 
					 union
						   (Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
								,  PERIOD_END_DATE
								,  Amount * @ReutersShares2
								,  PERIOD_TYPE
								,  PERIOD_YEAR
							  from #ConsensusFinancials
							 where ESTIMATE_ID = 4
							) 
						) dividend
						
					--JOIN REVENUEVALUES TO SUPPORTDATA ON PERIODENDDATES
					SELECT  a.*, b.USDPrice, b.SHARES_OUTSTANDING 
					INTO #RevenueSupportDividend
					FROM #RevenueValuesDividend a
					left join #SupportData b ON a.PERIOD_END_DATE = b.PRICE_DATE -- Multiple rows for one period label
					ORDER BY PERIOD_YEAR ,PERIOD_TYPE
					
					--SELECTING CALCULATED DATA
					SELECT * FROM #RevenueSupportDividend
					
					--Dropping above tables
					DROP TABLE #RevenueValuesDividend
					DROP TABLE #RevenueSupportDividend		
			END

  ELSE IF(@chartTitle = 'P/Revenue')
			BEGIN
			  -- *Extract the list of quarter periods based on Financial Data point availability

			SELECT *
			INTO #RevenueValues
			from (
					(Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
							, PERIOD_END_DATE
							, AMOUNT, PERIOD_TYPE
							, PERIOD_YEAR
					from #PrimaryFinancials
					where DATA_ID = 11
					) 
					UNION
					(Select a.PERIOD_TYPE + ' ' + CAST(a.PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
							, a.PERIOD_END_DATE
							, a.AMOUNT, a.PERIOD_TYPE
							, a.PERIOD_YEAR
					from #ConsensusFinancials a
					left join 
					(Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS VARCHAR(4)) as PeriodLabel
							, PERIOD_END_DATE
							, AMOUNT, PERIOD_TYPE
							, PERIOD_YEAR
					from #PrimaryFinancials
					where DATA_ID = 11
					) z on z.PERIOD_YEAR = a.PERIOD_YEAR and z.PERIOD_TYPE = a.PERIOD_TYPE 
					where ESTIMATE_ID = 17
					and z.PERIOD_YEAR is null -- Only where we don't find a matching ACTUAL
					) 
				) z
			
			-- *Join RevenueValues to SupportData on PeriodEndDates 
			Select b.SECURITY_ID, a.*, b.USDPrice, b.SHARES_OUTSTANDING 
			into #RevenueSupport
			from #RevenueValues a
			left join #SupportData b on a.PERIOD_END_DATE = b.PRICE_DATE
			order by  a.PERIOD_YEAR, a.PERIOD_TYPE
				
				select * from #RevenueSupport
				--Dropping above tables
				drop table #RevenueValues
				drop table #RevenueSupport
			END 
	
	--Dropping temporary tables
	drop table #SupportData
	drop table #PrimaryFinancials
	drop table #ConsensusFinancials	

END

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00038'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())