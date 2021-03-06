/****** Object:  StoredProcedure [dbo].[GetUnlistedSecurityShares]    Script Date: 04/19/2013 11:09:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-- Variables to be passed in as paramaters
alter procedure [dbo].[GetUnlistedSecurityShares](
	@startDateParam DATETIME						
	, @endDateParam DATETIME
	, @ISSUER_ID VARCHAR(20))
as


-- For Testing
/*
DECLARE @startDateParam DATETIME
SET @startDateParam = DATEADD(Day, 0, DATEDIFF(Day, 0, GetDate()))-101;

DECLARE @endDateParam DATETIME
SET @endDateParam = DATEADD(Day, 0, DATEDIFF(Day, 0, GetDate()));

DECLARE @ISSUER_ID VARCHAR(20)
SET @ISSUER_ID = '10054858';  -- This is a VarChar... but really muse be INT, maybe update table?
*/

-- Variables
Declare @SECURITY_ID VARCHAR(20);
Declare @SHARES_OUTSTANDING INT;
Declare @BEGINDATE Date;
Declare @ENDDATE Date;


-- Preclean
IF OBJECT_ID('tempdb..#results', 'U') IS NOT NULL
DROP TABLE #results;
IF OBJECT_ID('tempdb..#UnlistedSecurities', 'U') IS NOT NULL
DROP TABLE #UnlistedSecurities;

-- Results Table
CREATE TABLE #results (date DATETIME 
						,ISSUER_ID VARCHAR(100)
						,SECURITY_ID VARCHAR(100)
						,SHARES_OUTSTANDING INT);
						
-- Pick up the Unlisted Securities Records we want to process (date checking within loop)
SELECT ISSUER_ID
      ,SECURITY_ID
      ,SHARES_OUTSTANDING
      ,0 AS PROCESSED
      ,BEGINDATE
      ,ENDDATE
  into #UnlistedSecurities
  FROM SECURITY_SHARES_UNLISTED
  Where ISSUER_ID = @ISSUER_ID ;

-- Loop through Unlisted Securities
While (Select Count(*) From #UnlistedSecurities Where Processed = 0) > 0
Begin
			
	Select Top 1 
		@SECURITY_ID = SECURITY_ID 
		,@ISSUER_ID = ISSUER_ID
		,@SHARES_OUTSTANDING = SHARES_OUTSTANDING
		,@BEGINDATE = BEGINDATE
		,@ENDDATE = ENDDATE
		From #UnlistedSecurities 
		Where Processed = 0;
		
-- Kill the temp dates table
	IF OBJECT_ID('tempdb..#dates', 'U') IS NOT NULL
	DROP TABLE #dates;

-- Table of Dates in the period 
/*	--this way does not work with 100+ records
	WITH records AS (
	  SELECT @startDateParam AS date
	  UNION ALL
	  SELECT DATEADD(dd, 1, date)
		FROM records R
	   WHERE DATEADD(dd, 1, date) <= @endDateParam)
	SELECT * into #dates
	  FROM records;
*/	  
	CREATE TABLE #dates (date DATETIME)  
	DECLARE @workingDateParam DATETIME
	SET @workingDateParam = @startDateParam;
	While (DATEADD(dd, 1,@endDateParam) <> @workingDateParam) 
	Begin
		INSERT INTO #dates
		select @workingDateParam as date;
		SET @workingDateParam = DATEADD(dd, 1, @workingDateParam);
	End	  
	  

-- remove any dates beyond the end date specified in SECURITY_SHARES_UNLISTED
	DELETE FROM #dates
	 WHERE date>@ENDDATE;

-- remove any dates prior to the begin date specified in SECURITY_SHARES_UNLISTED
	DELETE FROM #dates
	 WHERE date<@BEGINDATE;

-- put together the results	 
	INSERT INTO #results (date)  
	select date from #dates;
	
	UPDATE #results
	SET
		ISSUER_ID = @ISSUER_ID
		,SECURITY_ID = @SECURITY_ID 
		,SHARES_OUTSTANDING = @SHARES_OUTSTANDING
	WHERE 
		SECURITY_ID IS NULL;

--Flag record as processed			
	Update #UnlistedSecurities Set Processed = 1 Where SECURITY_ID = @SECURITY_ID;
End
  
-- Return Results double check
select * from #results
  
-- Cleanup
IF OBJECT_ID('tempdb..#UnlistedSecurities', 'U') IS NOT NULL
DROP TABLE #UnlistedSecurities;
IF OBJECT_ID('tempdb..#dates', 'U') IS NOT NULL
DROP TABLE #dates;
IF OBJECT_ID('tempdb..#results', 'U') IS NOT NULL
DROP TABLE #results;

