SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
----------------------------------------------------------------------------------
-- This stored procedure makes sure that there is a price for each security on 
-- every date.  Doing so makes the supporting data complete and removes issues 
-- and complexity of SQL in using the PRICES table.
----------------------------------------------------------------------------------
alter procedure [dbo].[Price_Check]
as

	-- Verify that all the dates in the FX table are filled in
	declare @PrevSecurity	char(20)			-- Previous row Security
	declare @CurrDate		datetime		-- current expected data.  
	declare @SECURITY_ID	varchar(20)		-- This row Security
	declare @PRICE_DATE		datetime		-- Date from this row
	declare @PRICE			decimal(32,6)	-- Price from this row
	declare @FX_RATE		decimal(32,6)	-- FX Rate from this row
	declare @ADR_CONV		decimal(32,6)	-- The ADR conversion just selected
	declare @Curr_ADR_CONV	decimal(32,6)	-- The ADR conversion for the current security
	
	
	-- We need a table to hold the new data while we itterate through the existing data
	create table #PRICES_NEW
		(
		SECURITY_ID		varchar(20)		NOT NULL,
		PRICE_DATE		datetime		NOT NULL,
		PRICE			decimal(32,6)	NOT NULL,
		FX_RATE			decimal(32,6)	NOT NULL,
		ADR_CONV		decimal(32,6)		NULL 
		)
	create unique index PRICES_NEW_idx on #PRICES_NEW( SECURITY_ID, PRICE_DATE)


	-- THis cursor will iterate through the FX Rates data to find missing dates
	declare Pricecheck cursor 
		for select SECURITY_ID, PRICE_DATE, PRICE, FX_RATE, ADR_CONV
			  from dbo.PRICES
			 where 1=1 --FX_DATE is null
			 order by SECURITY_ID, PRICE_DATE;

	open Pricecheck;
	fetch next from Pricecheck
	 into @SECURITY_ID, @PRICE_DATE, @PRICE, @FX_RATE, @ADR_CONV

	-- Loop through the data checking each row
	while @@FETCH_STATUS = 0
		BEGIN
			if( isnull(@PrevSecurity, ' ') <> @SECURITY_ID)
				BEGIN
					set @PrevSecurity = @SECURITY_ID;
					set @CurrDate = @PRICE_DATE;
					set @Curr_ADR_CONV = @ADR_CONV
				END
			
			-- Add rows for each date between the last row and this row
			while @CurrDate < @PRICE_DATE
				BEGIN
					-- Create a row with the rates from the previous row
					--select 'insert into #FX_RATES_NEW Values('''+@CURRENCY+''', '''+convert(varchar(32), @CurrDate)+''','+CAST(@FX_RATE as varchar(32))+', '+CAST(@AVG90D as varchar(32))+', '+CAST(@AVG12M as varchar(32))+')'
					insert into #PRICES_NEW Values(@SECURITY_ID, @CurrDate, @PRICE, @FX_RATE, @Curr_ADR_CONV)
			
					-- The next expected date is
					set @CurrDate = DATEADD( d, 1, @CurrDate);
				END
			
			-- The next expected date is
			set @CurrDate = DATEADD( d, 1, @CurrDate);

			-- Get the next row of data
			fetch next from Pricecheck
			 into @SECURITY_ID, @PRICE_DATE, @PRICE, @FX_RATE, @ADR_CONV;
		END;

	-- Clean up the cursor
	close Pricecheck;
	deallocate Pricecheck;

	-- populate the generated data into the FX_RATES table
	insert into PRICES
	select * from #PRICES_NEW ;

	-- clean up the temp table
	drop table #PRICES_NEW;
GO
