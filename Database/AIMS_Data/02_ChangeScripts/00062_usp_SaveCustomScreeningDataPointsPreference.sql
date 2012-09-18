set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00061'
declare @CurrentScriptVersion as nvarchar(100) = '00062'

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
CREATE PROCEDURE [dbo].[SaveCustomScreeningDataPointsPreference] 
	-- Add the parameters for the stored procedure here
	@userPreference varchar(MAX), 
	@username nvarchar(50)
AS
DECLARE @listName nvarchar(50);	
DECLARE @tempTable TABLE
(
UserName nvarchar(50) not null, 
ListName nvarchar(50) not null, 
ScreeningId varchar(50) not null, 
DataDescription varchar(100) not null,
DataSource varchar(50), 
PeriodType char(2),
YearType char(8),
FromDate int,
ToDate int,
DataPointsOrder int not null,
CreatedBy nvarchar(50) not null,
CreatedOn datetime not null,
ModifiedBy nvarchar(50) not null,
ModifiedOn datetime not null
)
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION

    --TSQL Script to parse update xml
		
		DECLARE @XML XML
		SELECT @XML = @userPreference
		DECLARE @idoc int
		
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML
		
		SELECT @listName = ListName FROM OPENXML(@idoc, '/Root/CreateRow', 2)
			WITH ( ListName nvarchar(50) '@ListName')
			
		INSERT INTO dbo.UserCustomisedListInfo
			(UserName, ListName, Accessibilty, CreatedOn, ModifiedBy, ModifiedOn)
			SELECT *,
			GETUTCDATE(),
			@username,
			GETUTCDATE()
			  FROM OPENXML(@idoc, '/Root/CreateRow/CreateRowEntity', 2)
			WITH (
				UserName nvarchar(50)  '@UserName', 
				ListName nvarchar(50) '@ListName', 
				Accessibilty nvarchar(10) '@Accessibilty') 
				
				IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1				
				RETURN
			END	
			
			INSERT INTO @tempTable
			SELECT *,
			@username as [CreatedBy],
			GETUTCDATE() as [CreatedOn],
			@username as [ModifiedBy],
			GETUTCDATE() as [ModifiedOn]
			FROM OPENXML(@idoc, '/Root/CreateRow/CreateRowPreference', 2)
			WITH (
				UserName nvarchar(50) '@UserName', 
				ListName nvarchar(50) '@ListName', 
				ScreeningId nvarchar(10) '@ScreeningId', 
				DataDescription nvarchar(100) '@DataDescription',
				DataSource varchar(50) '@DataSource', 
				PeriodType nvarchar(50) '@PeriodType',
				YearType nvarchar(50) '@YearType',
				FromDate int '@FromDate',
				ToDate int '@ToDate',
				DataPointsOrder int '@DataPointsOrder')
				
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
			
		SELECT 
		l.ListId,
		t.ScreeningId,
		t.DataDescription,
		t.DataSource,
		t.PeriodType,
		t.YearType,
		t.FromDate,
		t.ToDate,
		t.DataPointsOrder,
		t.CreatedBy,
		t.CreatedOn,
		t.ModifiedBy,
		t.ModifiedOn
		INTO #finalTable
		FROM
		(SELECT ListId,ListName,UserName 
		FROM UserCustomisedListInfo 
		WHERE ListName = @listName)l
		
		left join
		
		(SELECT * FROM @tempTable)t
		ON l.ListName = t.ListName AND
		l.UserName = t.UserName	
		
		INSERT INTO UserListDataPointMappingInfo
		(ListId,
		ScreeningId,
		DataDescription,
		DataSource,
		PeriodType,
		YearType,
		FromDate,
		ToDate,
		DataPointsOrder,
		CreatedBy,
		CreatedOn,
		ModifiedBy,
		ModifiedOn) 
		SELECT ListId,
		ScreeningId,
		DataDescription,
		DataSource,
		PeriodType,
		YearType,
		FromDate,
		ToDate,
		DataPointsOrder,
		CreatedBy,
		CreatedOn,
		ModifiedBy,
		ModifiedOn 
		FROM #finalTable
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
		
		DROP TABLE #finalTable;	
		--EXEC sp_xml_removedocument @idoc;
		
		COMMIT TRANSACTION;
		SELECT 0	
END


GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00062'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



