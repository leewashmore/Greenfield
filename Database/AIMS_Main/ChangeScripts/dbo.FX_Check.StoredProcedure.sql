SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
----------------------------------------------------------------------------------
-- This stored procedure makes sure that there is an FX Rate for currency on 
-- every date.  Doing so makes the supporting data complete and removes issues 
-- and complexity of SQL in using the FX_RATES table.
----------------------------------------------------------------------------------
alter procedure [dbo].[FX_Check]
as

	-- Verify that all the dates in the FX table are filled in
	declare @PrevCurr	char(3)			-- Previous row Currency
	declare @CurrDate	datetime		-- current expected data.  
	declare @CURRENCY	char(3)			-- This row Currency
	declare @FX_DATE	datetime		-- Date from this row
	declare @FX_RATE	decimal(32,6)	-- Rate from this row
	declare @AVG90D		decimal(32,6)	-- Average 90 day rate from this row
	declare @AVG12M		decimal(32,6)	-- Average 12 month rate from this row
	
	-- We need a table to hold the new data while we itterate through the existing data
	create table #FX_RATES_NEW
		(
		CURRENCY		char(3)			NOT NULL,
		FX_DATE			datetime		NOT NULL,
		FX_RATE			decimal(32,6)	NOT NULL,
		AVG90DAYRATE	decimal(32,6)	    NULL,
		AVG12MonthRATE	decimal(32,6)	    NULL
		)
	create unique index FX_RATES_NEW_idx on #FX_RATES_NEW( CURRENCY, FX_DATE)


	-- THis cursor will iterate through the FX Rates data to find missing dates
	declare FXcheck cursor 
		for select CURRENCY, FX_DATE, FX_RATE, AVG90DAYRATE, AVG12MonthRATE
			  from dbo.FX_RATES
			 where 1=1 --FX_DATE is null
			 order by CURRENCY, FX_DATE;
			 
	open FXcheck;
	fetch next from FXcheck
	 into @CURRENCY, @FX_DATE, @FX_RATE, @AVG90D, @AVG12M;

	-- Loop through the data checking each row
	while @@FETCH_STATUS = 0
		BEGIN
			if( isnull(@PrevCurr, ' ') <> @CURRENCY)
				BEGIN
					set @PrevCurr = @CURRENCY;
					set @CurrDate = @FX_DATE;							
				END
			
			-- Add rows for each date between the last row and this row
			while @CurrDate < @FX_DATE
				BEGIN
					-- Create a row with the rates from the previous row
					--select 'insert into #FX_RATES_NEW Values('''+@CURRENCY+''', '''+convert(varchar(32), @CurrDate)+''','+CAST(@FX_RATE as varchar(32))+', '+CAST(@AVG90D as varchar(32))+', '+CAST(@AVG12M as varchar(32))+')'
					insert into #FX_RATES_NEW Values(@CURRENCY, @CurrDate, @FX_RATE, @AVG90D, @AVG12M)
			
					-- The next expected date is
					set @CurrDate = DATEADD( d, 1, @CurrDate);
				END
			
			-- The next expected date is
			set @CurrDate = DATEADD( d, 1, @CurrDate);

			-- Get the next row of data
			fetch next from FXcheck
			 into @CURRENCY, @FX_DATE, @FX_RATE, @AVG90D, @AVG12M;
		END;

	-- Clean up the cursor
	close FXcheck;
	deallocate FXcheck;


	-- Make sure the USD FX Rates for the future are populated
	-- Get the most recent date
	select @CurrDate = MAX(FX_DATE)
	  from FX_RATES
	 where CURRENCY = 'USD'
	 
	-- Add dates to 10 years in the future.
	declare @MaxDate datetime
	set @MaxDate = dateadd( year, 10, GETDATE())

	while @CurrDate < @MaxDate
		BEGIN
			-- The next expected date is
			set @CurrDate = DATEADD( d, 1, @CurrDate);
			
			-- Create a row with USD rates
			insert into #FX_RATES_NEW Values('USD', @CurrDate, 1.0, 1.0, 1.0)
	
		END
	
	-- populate the generated data into the FX_RATES table
	insert into FX_RATES
	select * from #FX_RATES_NEW ;

	-- clean up the temp table
	drop table #FX_RATES_NEW;
GO
