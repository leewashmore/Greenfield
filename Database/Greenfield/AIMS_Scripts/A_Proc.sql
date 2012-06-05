USE [AshmoreEMMPOC]
GO

/****** Object:  StoredProcedure [dbo].[A_PROC]    Script Date: 06/05/2012 10:32:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE  procedure [dbo].[A_PROC]
 (@SEC_SECSHORT_List VARCHAR(500))
AS DECLARE @Index INT 
   DECLARE @RecordCnt INT
   DECLARE  @TEMPORARY_TAB TABLE( 
								iSNo INT IDENTITY(1,1), 
								XREF VARCHAR(9), SEC_SECSHORT VARCHAR(30) )
   					   								    
							    			    
BEGIN
       
SET NOCOUNT OFF;
SET FMTONLY OFF;

CREATE TABLE #TempList
	(	iSNo INT IDENTITY(1,1),     
		SEC_SECSHORT varchar(50)
	)
	CREATE TABLE #TEMPORARY_MAIN
	(	    
		XREF VARCHAR(9),
		YEAR2009 REAL,
		YEAR2010 REAL,
		YEAR2011 REAL,
		YEAR2012 REAL,
		YEAR2013 REAL,
		SEC_SECSHORT varchar(50)
	)
DECLARE @SEC_SECSHORT varchar(50), @Pos int

SET @SEC_SECSHORT_List = LTRIM(RTRIM(@SEC_SECSHORT_List))+ ','
	SET @Pos = CHARINDEX(',', @SEC_SECSHORT_List, 1)

	IF REPLACE(@SEC_SECSHORT_List, ',', '') <>''
	BEGIN
		WHILE @Pos > 0
		BEGIN
		
			SET @SEC_SECSHORT = LTRIM(RTRIM(LEFT(@SEC_SECSHORT_List, @Pos - 1)))
			IF @SEC_SECSHORT <> ''
			BEGIN
			
				INSERT INTO #TempList (SEC_SECSHORT) VALUES (CAST(@SEC_SECSHORT AS varchar(50))) 
			
			END
			SET @SEC_SECSHORT_List = RIGHT(@SEC_SECSHORT_List, LEN(@SEC_SECSHORT_List) - @Pos)
			SET @Pos = CHARINDEX(',', @SEC_SECSHORT_List, 1)

		END
	    END	
             
              INSERT INTO @TEMPORARY_TAB(XREF,SEC_SECSHORT) SELECT A.XREF,T.SEC_SECSHORT FROM  AGG_TABLE AS A JOIN #TempList T ON A.SEC_SECSHORT=T.SEC_SECSHORT
              SELECT @Index = 1
              SELECT @RecordCnt = COUNT(iSNo) FROM @TEMPORARY_TAB
              
              WHILE @Index <= @RecordCnt 
              BEGIN 
            
                        
             INSERT INTO #TEMPORARY_MAIN (XREF)
             SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index
             
			 UPDATE  #TEMPORARY_MAIN
			 SET YEAR2011 =
		     (SELECT TOP 1 Median  FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)
             AND fYearEnd = '201112' AND ExpirationDate = (SELECT MAX(ExpirationDate) FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND (XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)) AND fYearEnd = '201112')),
             
             YEAR2012 = 
             (SELECT TOP 1 Median FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index) 
             AND (fYearEnd = '201212') AND ExpirationDate = (SELECT MAX(ExpirationDate) FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND (XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)) AND fYearEnd = '201212')),
              
             YEAR2013 = 
             (SELECT TOP 1 Median FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)
             AND (fYearEnd = '201312') AND ExpirationDate = (SELECT MAX(ExpirationDate) FROM tblConsensusEstimate
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND (XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)) AND fYearEnd = '201312')),
			 		 
           
             YEAR2009 = 
             (SELECT TOP 1 ActualValue  FROM tblActual
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index) AND fYearEnd = '200912' ),
			
			YEAR2010 = (SELECT TOP 1 ActualValue FROM tblActual
             WHERE EstimateType = 'NTP' AND PeriodType = 'A' AND XRef=(SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index) AND fYearEnd = '201012' ),
             
             SEC_SECSHORT = (SELECT SEC_SECSHORT FROM @TEMPORARY_TAB WHERE iSNo=@Index) 
             
             WHERE
             XREF = (SELECT XREF FROM @TEMPORARY_TAB WHERE iSNo=@Index)
             
            
             SET @Index=@Index+1
             END 
             
            SELECT * FROM #TEMPORARY_MAIN
        END
GO

