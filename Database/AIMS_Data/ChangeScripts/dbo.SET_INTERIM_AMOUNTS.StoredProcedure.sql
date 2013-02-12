SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-----------------------------------------------------------------------------------
-- Purpose:	To take the Annual data provided by the Internal Analysts and make
--			quarterly data out of it.  THe models will be supplying the data for
--			Annual as well as the percentages to distribute the Annual values
--			into quarterly data.  THis procedure creates the quarterly values.
--
-- Author:	David Muench
-- Date:	July 23, 2012
-----------------------------------------------------------------------------------

alter procedure [dbo].[SET_INTERIM_AMOUNTS] (
	@ISSUER_ID			varchar(20)		= NULL			-- The company identifier		
,	@DATA_SOURCE		varchar(10)	= 'PRIMARY'		-- Default to the primary analyst.
)
as
	declare @DIST_COUNT	integer
	declare @ReutersCount integer
	declare @Q1Percent decimal(32,6)
	declare @Q2Percent decimal(32,6)
	declare @Q3Percent decimal(32,6)
	declare @Q4Percent decimal(32,6)
	declare @ReutersSum decimal(32,6)
	

	-- delete previous data
	delete from dbo.INTERNAL_DATA 
	 where (@ISSUER_ID is NULL or ISSUER_ID = @ISSUER_ID)
	   and PERIOD_TYPE in ('Q1', 'Q2', 'Q3', 'Q4')

	-- Get the number of rows for the Issuer, there should be 1 for each quarter = total of 4
	select @DIST_COUNT = count(*) 
	  from INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION 
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_SOURCE = @DATA_SOURCE
	 
	 
	if @DIST_COUNT = 0
		BEGIN
			--Get the most recent 8 quarterly statements prior to today
			select top 8 *
			into #QtrBreakdown
			from PERIOD_FINANCIALS pf
			where pf.ISSUER_ID = @ISSUER_ID
			and pf.PERIOD_TYPE in('Q1','Q2','Q3','Q4')
			and pf.DATA_SOURCE = 'REUTERS'
			and pf.CURRENCY <> 'USD'
			and pf.DATA_ID = 11
			and pf.PERIOD_END_DATE < getdate()
			order by PERIOD_END_DATE desc

			--calculate qtr averages
			select @ReutersSum = SUM(amount)
			from #QtrBreakdown 
			

			select qb.period_type, SUM(qb.amount)/@ReutersSum as QtrAmount, COUNT(*) as tally
			into #QtrPercent
			from #QtrBreakdown qb
			group by qb.period_type

			--check to make sure we have two quarters in each period
			select @ReutersCount = COUNT(*) 
				from  #QtrPercent 
				where tally = 2


			if @ReutersCount = 4
				BEGIN
					--Update distribution table with Reuters trend
					select @Q1Percent = QtrAmount 
						from #QtrPercent
						where PERIOD_TYPE = 'Q1'
					select @Q2Percent = QtrAmount 
						from #QtrPercent
						where PERIOD_TYPE = 'Q2'
					select @Q3Percent = QtrAmount 
						from #QtrPercent
						where PERIOD_TYPE = 'Q3'
					select @Q4Percent = QtrAmount 
						from #QtrPercent
						where PERIOD_TYPE = 'Q4'
			
					insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q1', @Q1Percent, GETDATE())
					insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q2', @Q2Percent, GETDATE())
					insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q3', @Q3Percent, GETDATE())
					insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q4', @Q4Percent, GETDATE())	
					 set @DIST_COUNT = 4		-- The count is now 4
				END

			drop table #QtrPercent
			drop table #QtrBreakdown
		END
	 
	 
	if @DIST_COUNT = 0
		BEGIN
			-- When there is no distribution for the Issuer, put in 25% for each quarter
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q1', 0.25, GETDATE())
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q2', 0.25, GETDATE())
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q3', 0.25, GETDATE())
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q4', 0.25, GETDATE())
			set @DIST_COUNT = 4		-- The count is now 4
		END


	if @DIST_COUNT = 4
		BEGIN
			-- insert the quarterly data
			Insert into dbo.INTERNAL_DATA (ISSUER_ID, REF_NO, PERIOD_TYPE, COA, AMOUNT)
			Select id.ISSUER_ID
				,  id.REF_NO
				,  iiqd.PERIOD_TYPE						-- The Period type being distributed to
				,  id.COA
				,  id.AMOUNT * iiqd.PERCENTAGE	as AMOUNT	-- The new amount
			  from dbo.INTERNAL_DATA id
			 inner join dbo.INTERNAL_STATEMENT s on s.ISSUER_ID = id.ISSUER_ID and s.REF_NO = id.REF_NO
			 inner join INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION iiqd on iiqd.ISSUER_ID = id.ISSUER_ID and iiqd.DATA_SOURCE = s.ROOT_SOURCE
			 where (@ISSUER_ID is NULL or id.ISSUER_ID = @ISSUER_ID)
			   AND iiqd.DATA_SOURCE = @DATA_SOURCE
			   and id.PERIOD_TYPE = 'A'
			   and s.AMOUNT_TYPE = 'ESTIMATE'  -- only calculate quarterly data for estimates
			 order by id.REF_NO, id.COA, id.PERIOD_TYPE, iiqd.PERIOD_TYPE
			;
		END
GO
