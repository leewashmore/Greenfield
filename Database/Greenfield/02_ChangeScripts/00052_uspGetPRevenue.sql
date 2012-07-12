set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00051'
declare @CurrentScriptVersion as nvarchar(100) = '00052'

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
CREATE PROCEDURE [dbo].[Get_PRevenue](@ISSUER_ID varchar(20))
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--EXTRACT QUARTER VALUES FROM PRIMARY ANALYST SOURCE FIRST
	
	Select * into #PrimaryFinancials
	from PERIOD_FINANCIALS 
	where 
	PERIOD_TYPE= 'A'
	and ISSUER_ID  = @ISSUER_ID --'8677772' ,'7602506' ,'157902'
	and PERIOD_END_DATE > (DATEADD(year, -10, GETDATE())) 
	and PERIOD_END_DATE < (DATEADD(year, 6, GETDATE())) 
	and DATA_SOURCE = 'PRIMARY' 
	and FISCAL_TYPE = 'FISCAL'
	and Currency = 'USD'		
	and PERIOD_TYPE in ('Q1','Q2','Q3','Q4')
	select * from #PrimaryFinancials
	declare @MAX_PERIOD_END_DATE datetime 
	select @MAX_PERIOD_END_DATE=MAX(PERIOD_END_DATE) from #PrimaryFinancials
	select @MAX_PERIOD_END_DATE 
	
	--PULL QUARTER ESTIMATED VALUES FOR PERIODS AFTER THE OLDEST PERIODENDDATE IN THE SELECT ABOVE
	
	Select * into #ConsensusFinancials
	from CURRENT_CONSENSUS_ESTIMATES 
	where ISSUER_ID  = @ISSUER_ID 
	and PERIOD_END_DATE > @MAX_PERIOD_END_DATE 
	and DATA_SOURCE = 'REUTERS' --Source is DATA_SOURCE--Confirmed
	and FISCAL_TYPE = 'FISCAL'
	and Currency = 'USD' 
	and PERIOD_TYPE in ('Q1','Q2','Q3','Q4') 
	select * from #ConsensusFinancials
	
	--EXTRACT THE LIST OF QUARTER PERIODS BASED ON FINANCIAL DATA POINT AVAILABILITY
	
	Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS varchar(10)) as PERIOD_LABEL, 
	PERIOD_END_DATE, AMOUNT, PERIOD_TYPE, PERIOD_YEAR 
	into #Extract1
	from #PrimaryFinancials  
	where DATA_ID = 11
	select * from #Extract1
	
	Select PERIOD_TYPE + ' ' + CAST(PERIOD_YEAR AS varchar(10)) as PERIOD_LABEL, 
	PERIOD_END_DATE, AMOUNT, PERIOD_TYPE, PERIOD_YEAR 
	into #Extract2
	from #ConsensusFinancials 
	where ESTIMATE_ID = 17
	select * from #Extract2
	--JOIN ABOVE TWO EXTRACTS ORDERED BY PERIODENDDATES INTO “REVENUEVALUES”
	
	SELECT E1.PERIOD_LABEL,E1.PERIOD_END_DATE, E1.AMOUNT, E1.PERIOD_TYPE, E1.PERIOD_YEAR INTO #RevenueValues
	FROM #Extract1 E1
	join #Extract2 E2
	on E1.PERIOD_LABEL = E2.PERIOD_LABEL
	SELECT * from #RevenueValues
	
	--Dropping temporary tables
	drop table #PrimaryFinancials
	drop table #ConsensusFinancials
	drop table #Extract1
	drop table #Extract2
	drop table #RevenueValues

END
GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00052'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())