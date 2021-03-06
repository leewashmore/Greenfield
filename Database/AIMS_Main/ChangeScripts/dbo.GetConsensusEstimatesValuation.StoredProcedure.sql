SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[GetConsensusEstimatesValuation](
	@ISSUER_ID			varchar(20)					-- The company identifier		
,	@DATA_SOURCE		varchar(10)  = 'REUTERS'	-- REUTERS, PRIMARY, INDUSTRY
,	@ROOT_SOURCE		varchar(10)  = 'CONSENSUS'	-- CONSENSUS, REUTERS, PRIMARY
,	@PERIOD_TYPE		char(2) = 'A'				-- A, Q
,	@FISCAL_TYPE		char(8) = 'FISCAL'			-- FISCAL, CALENDAR
,	@CURRENCY			char(3)	= 'USD'				-- USD or the currency of the country (local)
,	@ESTIMATE_ID		integer = NULL				-- A specific ID or NULL for all.
,	@PERIOD_YEAR		integer = NULL				-- A specific year if required	
,	@Security_ID		varchar(20)					-- Security-ID of selected Security
)
as

	-- Select the Estimated data	
	select pf.ISSUER_ID
		,  pf.DATA_ID as ESTIMATE_ID
		,  dm.DATA_DESC as ESTIMATE_DESC
		,  substring(ISNULL(AMOUNT_TYPE,'ESTIMATE'),1,1)+cast(ISNULL(pf.PERIOD_YEAR,2300) as CHAR(4))+ISNULL(pf.PERIOD_TYPE,'E') as Period
		,  ISNULL(pf.AMOUNT_TYPE, 'ESTIMATE') AS AMOUNT_TYPE
		,  ISNULL(pf.PERIOD_YEAR,2300)AS PERIOD_YEAR
		,  ISNULL(pf.PERIOD_TYPE,'E') AS PERIOD_TYPE
--		,  ISNULL(pf.AMOUNT,0) AS AMOUNT
		,  case pf.DATA_ID 
			when 192 then ISNULL(pf.AMOUNT,0)*100		--DividendYield
			else ISNULL(pf.AMOUNT,0)
		 End as AMOUNT
		,  0.0 as ASHMOREEMM_AMOUNT
		,  0 as NUMBER_OF_ESTIMATES
		,  0.0 as HIGH
		,  0.0 as LOW
		,  0.0 as STANDARD_DEVIATION
		,  pf.SOURCE_CURRENCY
		,  pf.DATA_SOURCE
		,  pf.ROOT_SOURCE_DATE as DATA_SOURCE_DATE
		,  pf.SECURITY_ID
		,  case pf.DATA_ID 
			when 170 then 'P_Revenue'		--P/Revenue				 
			when 171 then 'P_CE'			--P/CE						 		
			when 166 then 'P_E'				--P/E								
			when 172 then 'P_E_To_Growth'	--P/E To Growth							
			when 164 then 'P_BV'			--P/BV						
			when 192 then 'DIV_YIELD'		--DividendYield
		 End as dp_desc	
	  into #Estimate
	  from dbo.PERIOD_FINANCIALS pf
	   Right join dbo.DATA_MASTER dm on dm.DATA_ID =pf.DATA_ID
	   where (pf.ISSUER_ID = @ISSUER_ID or pf.SECURITY_ID=@Security_ID)
	   and pf.DATA_SOURCE = @DATA_SOURCE
	   and pf.ROOT_SOURCE = @ROOT_SOURCE
	   and substring(pf.PERIOD_TYPE,1,1) = @PERIOD_TYPE
	   and pf.FISCAL_TYPE = @FISCAL_TYPE
	   and pf.CURRENCY = @CURRENCY
	   and pf.data_id in ( 170,		--P/Revenue
								166,	--P/E
								164,	--P/BV
								192,	--DividendYield
								177		--Net Income Growth
							)
	   --and (cce.ESTIMATE_ID =cce.ESTIMATE_ID)
	 order by pf.PERIOD_YEAR
	 ;
	
	 
	 Select pf.ISSUER_ID
--	 , pf.AMOUNT
	 ,  case pf.DATA_ID 
		when 192 then ISNULL(pf.AMOUNT,0)*100		--DividendYield
		else ISNULL(pf.AMOUNT,0)
		End as AMOUNT
	 , pf.AMOUNT_TYPE
	 , pf.DATA_ID as ESTIMATE_ID
	 ,pf.FISCAL_TYPE, pf.CURRENCY, pf.PERIOD_YEAR, pf.PERIOD_TYPE,
	 		case pf.data_id 
				when 170 then 'P_Revenue'		--P/Revenue				 
				when 171 then 'P_CE'			--P/CE						 		
				when 166 then 'P_E'				--P/E								
				when 172 then 'P_E_To_Growth'	--P/E To Growth							
				when 164 then 'P_BV'			--P/BV						
				when 192 then 'DIV_YIELD'		--DividendYield
			 End as dp_desc	
			 , pf.SECURITY_ID
		 into #pf1
		 from PERIOD_FINANCIALS pf
				  where 1=1
				    and pf.DATA_SOURCE = 'PRIMARY'
				    and substring(pf.PERIOD_TYPE,1,1) = @PERIOD_TYPE
				    and pf.FISCAL_TYPE = @FISCAL_TYPE
				    and pf.CURRENCY = @CURRENCY
				    and (pf.ISSUER_ID = @ISSUER_ID or pf.SECURITY_ID=@Security_ID)
					and pf.DATA_ID in( 170, --P/Revenue			
										166, --P/E
										164,  --P/BV
										192,   --DividendYield
										177   --Net Income Growth
									  )
	 

	-- Combine Actual with Estimated so that Actual is always used in case both are present.
	select e.ISSUER_ID
		,  e.ESTIMATE_ID
		,  e.ESTIMATE_DESC
		,  e.Period
		,  e.AMOUNT_TYPE
		,  e.PERIOD_YEAR
		,  e.PERIOD_TYPE
		,  e.AMOUNT
		--,  b.AMOUNT as ASHMOREEMM_AMOUNT
		,(select amount from #pf1 b where (b.issuer_id = e.issuer_id or b.security_id = e.security_id) and b.period_year = e.period_year
				and b.estimate_id = e.estimate_id 
		  ) as ASHMOREEMM_AMOUNT
		,  e.NUMBER_OF_ESTIMATES
		,  e.HIGH
		,  e.LOW
		,  e.STANDARD_DEVIATION
		,  e.SOURCE_CURRENCY
		,  e.DATA_SOURCE
		,  e.DATA_SOURCE_DATE
		,  0.0  as ACTUAL
	-- Need to get all the possible rows
	  from #Estimate e 

	---- Clean up
	drop table #Estimate;


GO
