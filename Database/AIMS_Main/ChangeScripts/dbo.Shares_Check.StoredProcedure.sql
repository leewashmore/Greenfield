SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
----------------------------------------------------------------------------------
-- This stored procedure makes sure that there is an Issuer Shares value for each 
-- Issuer on every date.  Doing so makes the supporting data complete and removes issues 
-- and complexity of SQL in using the ISSUER_SHARES table.
----------------------------------------------------------------------------------
alter procedure [dbo].[Shares_Check]
	@ISSUER		varchar(20) = NULL		-- choose a specific Issuer to run
as

	-- Verify that all the dates in the FX table are filled in
	declare @PrevISSUER_ID	varchar(20)	-- Previous row ISSUER_ID
	declare @CurrDate	datetime		-- current expected data.  
	declare @ISSUER_ID	varchar(20)		-- This row Currency
	declare @SHARE_DATE	datetime		-- Date from this row
	declare @SHARE_OUT	decimal(32,6)	-- AMOUNT from this row
	
	-- We need a table to hold the new data while we itterate through the existing data
	create table #ISSUER_SHARES_NEW
		(
		ISSUER_ID			varchar(20)		NOT NULL,
		SHARES_DATE			datetime		NOT NULL,
		SHARES_OUTSTANDING	decimal(32,2)	NOT NULL,
		)
	create unique index ISSUER_SHARES_NEW_idx on #ISSUER_SHARES_NEW( ISSUER_ID, SHARES_DATE)


	-- THis cursor will iterate through the FX Rates data to find missing dates
	declare Sharescheck cursor 
		for select ISSUER_ID, SHARES_DATE, SHARES_OUTSTANDING
			  from dbo.ISSUER_SHARES
			 where 1=1 
			   and (@ISSUER is NULL or @ISSUER = ISSUER_ID)
			 order by ISSUER_ID, SHARES_DATE;
			 
	open Sharescheck;
	fetch next from Sharescheck
	 into @ISSUER_ID, @SHARE_DATE, @SHARE_OUT

	-- Loop through the data checking each row
	while @@FETCH_STATUS = 0
		BEGIN
			if( isnull(@PrevISSUER_ID, ' ') <> @ISSUER_ID)
				BEGIN
					set @PrevISSUER_ID = @ISSUER_ID;
					set @CurrDate = @SHARE_DATE;							
				END
			
			-- Add rows for each date between the last row and this row
			while @CurrDate < @SHARE_DATE
				BEGIN
					-- Create a row with the rates from the previous row
					--select 'insert into #FX_RATES_NEW Values('''+@CURRENCY+''', '''+convert(varchar(32), @CurrDate)+''','+CAST(@FX_RATE as varchar(32))+', '+CAST(@AVG90D as varchar(32))+', '+CAST(@AVG12M as varchar(32))+')'
					insert into #ISSUER_SHARES_NEW Values(@ISSUER_ID, @CurrDate, @Share_OUT)
			
					-- The next expected date is
					set @CurrDate = DATEADD( d, 1, @CurrDate);
				END
			
			-- The next expected date is
			set @CurrDate = DATEADD( d, 1, @CurrDate);

			-- Get the next row of data
	fetch next from Sharescheck
	 into @ISSUER_ID, @SHARE_DATE, @SHARE_OUT
		END;

	-- Clean up the cursor
	close Sharescheck;
	deallocate Sharescheck;

	
	-- populate the generated data into the FX_RATES table
	insert into ISSUER_SHARES
	select * from #ISSUER_SHARES_NEW ;

	-- clean up the temp table
	drop table #ISSUER_SHARES_NEW;
GO
