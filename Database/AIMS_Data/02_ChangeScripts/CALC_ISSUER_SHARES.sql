----------------------------------------------------------------------------------
-- This stored procedure makes sure that there is an FX Rate for currency on 
-- every date.  Doing so makes the supporting data complete and removes issues 
-- and complexity of SQL in using the FX_RATES table.
----------------------------------------------------------------------------------
alter procedure CALC_ISSUER_SHARES(
	@ISSUER_ID			integer		= NULL			-- The company identifier		
)
as

	-- Get a list of Issuers and their outstanding shares.
	select cast(sb.ISSUER_ID as varchar(20)), a.PERIOD_END_DATE, sb.SHARES_OUTSTANDING
	  into #ISSUER_SHARES
	  from (select sb.ISSUER_ID, sum(sb.SHARES_OUTSTANDING) as SHARES_OUTSTANDING
			  from dbo.GF_SECURITY_BASEVIEW sb
			 inner join dbo.ISSUER_SHARES_COMPOSITION isc on isc.SECURITY_ID = sb.SECURITY_ID
			 where (@ISSUER_ID is NULL or @ISSUER_ID = isc.ISSUER_ID)
			 group by sb.ISSUER_ID) sb
	 inner join (select distinct PERIOD_END_DATE from dbo.PERIOD_FINANCIALS) a on 1=1
	 where sb.SHARES_OUTSTANDING is not NULL
	   and sb.ISSUER_ID is not null

	-- Delete the existing rows for the issuers
	delete from dbo.ISSUER_SHARES
	 where ISSUER_ID in (select distinct ISSUER_ID from #ISSUER_SHARES)
	
	-- Insert new rows for the Issuer that does not have them for this date.
	insert into dbo.ISSUER_SHARES (ISSUER_ID, SHARES_DATE, SHARES_OUTSTANDING)


GO

-- exec FX_Check