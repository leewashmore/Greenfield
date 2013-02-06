set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00133'
declare @CurrentScriptVersion as nvarchar(100) = '00134'

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

IF OBJECT_ID ('[dbo].[SetFairValue]') IS NOT NULL
	DROP PROCEDURE [dbo].[SetFairValue]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalCommodityAssumptions]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SetFairValue] 
	(
		@SECURITY_ID VARCHAR(20),
		@PFV_MEASURE VARCHAR(255),
		@PFV_MEASURE_VALUE DECIMAL(32,6),
		@FV_BUY DECIMAL(32,6),
		@FV_SELL DECIMAL(32,6)
	)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @PFV_MEASURE_ID INT
	SELECT TOP(1) @PFV_MEASURE_ID = [DATA_ID] FROM DATA_MASTER 
	WHERE [DATA_DESC] = @PFV_MEASURE
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -1
		RETURN
	END
	
	DECLARE @UPSIDE DECIMAL(32,6)
	SELECT @UPSIDE = CASE WHEN @PFV_MEASURE_VALUE = 0 THEN 0 ELSE ((@FV_SELL / @PFV_MEASURE_VALUE) - 1) END
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -2
		RETURN
	END
	
	DECLARE @RECORD_COUNT INT	
	SET @RECORD_COUNT = (SELECT COUNT(*) FROM FAIR_VALUE 
	WHERE [SECURITY_ID] = '2282' AND [VALUE_TYPE] = 'IC')
	
	IF @RECORD_COUNT = 0
	BEGIN
		INSERT INTO FAIR_VALUE ( [VALUE_TYPE], [SECURITY_ID], [FV_MEASURE], [FV_BUY], [FV_SELL], [CURRENT_MEASURE_VALUE], [UPSIDE], [UPDATED] )
		VALUES ( 'IC', @SECURITY_ID, @PFV_MEASURE_ID, @FV_BUY, @FV_SELL, @PFV_MEASURE_VALUE, @UPSIDE, GETUTCDATE() )	
	END
	ELSE
	BEGIN
		UPDATE FAIR_VALUE SET
		[FV_MEASURE] = @PFV_MEASURE_ID,
		[FV_BUY] = @FV_BUY,
		[FV_SELL] = @FV_SELL,
		[CURRENT_MEASURE_VALUE] = @PFV_MEASURE_VALUE,
		[UPSIDE] = @UPSIDE,
		[UPDATED] = GETUTCDATE()
		WHERE [SECURITY_ID] = @SECURITY_ID AND [VALUE_TYPE] = 'IC'
	END
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -3
		RETURN
	END
	
	SELECT 0
	
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00134'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

