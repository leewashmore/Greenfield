set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00132'
declare @CurrentScriptVersion as nvarchar(100) = '00133'

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

IF OBJECT_ID ('[dbo].[InsertIntoBenchmarkNodeFinancials]') IS NOT NULL
	DROP PROCEDURE [dbo].[InsertIntoBenchmarkNodeFinancials]
GO

/****** Object:  StoredProcedure [dbo].[InsertIntoBenchmarkNodeFinancials]	 Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertIntoBenchmarkNodeFinancials]	
	@xmlScript NVARCHAR(MAX)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
		
		DECLARE @XML XML
		
		SELECT @XML = @xmlScript
		DECLARE @idoc int
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML	   
		
		DECLARE @InsertionBenchmarkNodeFinancialCount INT
		SELECT @InsertionBenchmarkNodeFinancialCount = COUNT(*) FROM OPENXML(@idoc, '/Root/GroupedBenchmarkNodeData', 1)			
			
		IF @InsertionBenchmarkNodeFinancialCount <> 0
		BEGIN   												
			SELECT _MMVI.[BenchmarkID],
				   _MMVI.[NodeName1],
				   _MMVI.[NodeID1],
				   _MMVI.[NodeName2],
				   _MMVI.[NodeID2],
				   _MMVI.[DataID],
				   _MMVI.[PeriodType],
				   _MMVI.[PeriodYear],
				   _MMVI.[Currency],
				   _MMVI.[Amount],
				   _MMVI.[UpdateDate]				   
			INTO #BenchmarkNodeFinancialData 
			FROM OPENXML(@idoc, '/Root/GroupedBenchmarkNodeData', 1)
				WITH ( 
					[BenchmarkID]		VARCHAR(50),
					[NodeName1]			VARCHAR(50),
					[NodeID1]			VARCHAR(50),
					[NodeName2]		    VARCHAR(50),
					[NodeID2]    	    VARCHAR(50),
					[DataID]            INT, 
					[PeriodType]        CHAR(2),
					[PeriodYear]        INT,
					[Currency]          CHAR(3),
					[Amount]            DECIMAL(32,6),
					[UpdateDate]        [DATETIME]
					) _MMVI	
		
		    MERGE INTO dbo.BENCHMARK_NODE_FINANCIALS _BN		    
			USING #BenchmarkNodeFinancialData _BNF			
			ON _BN.[BENCHMARK_ID] = _BNF.[BenchmarkID]
		    AND _BN.[NODE_NAME1] = _BNF.[NodeName1]
		    AND _BN.[NODE_ID1] = _BNF.[NodeID1]
		    AND _BN.[NODE_NAME2] = _BNF.[NodeName2]
		    AND _BN.[NODE_ID2] = _BNF.[NodeID2]
		    AND _BN.[DATA_ID]  = _BNF.[DataID]
		    AND _BN.[PERIOD_TYPE] = _BNF.[PeriodType]
		    AND _BN.[PERIOD_YEAR] = _BNF.[PeriodYear]
		    AND _BN.[CURRENCY] = _BNF.[Currency]
		    AND _BN.[UPDATE_DATE] = _BNF.[UpdateDate]	  		      
			WHEN MATCHED THEN
			UPDATE SET
				[AMOUNT] = _BNF.[Amount]	,
                                                       [UPDATE_DATE] = _BNF.[UpdateDate]			
			WHEN NOT MATCHED THEN
			INSERT (BENCHMARK_ID, NODE_NAME1, NODE_ID1, NODE_NAME2, NODE_ID2,
			 DATA_ID,PERIOD_TYPE, PERIOD_YEAR,CURRENCY,AMOUNT,UPDATE_DATE)
			VALUES (_BNF.[BenchmarkID], _BNF.[NodeName1],_BNF.[NodeID1], _BNF.[NodeName2],
			_BNF.[NodeID2],_BNF.[DataID],_BNF.[PeriodType],_BNF.[PeriodYear],_BNF.[Currency],
			_BNF.[Amount],_BNF.[UpdateDate]);
			
			DROP TABLE #BenchmarkNodeFinancialData
		END

COMMIT TRANSACTION

END	

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00133'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())