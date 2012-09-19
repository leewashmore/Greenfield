set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00076'
declare @CurrentScriptVersion as nvarchar(100) = '00077'

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
CREATE PROCEDURE [dbo].[UpdateCustomScreeningDataPointsPreference] 
	-- Add the parameters for the stored procedure here
	@userPreference varchar(MAX), 
	@username nvarchar(50),
	@existingListname nvarchar(50),
	@newListname nvarchar(50),
	@accessibility nvarchar(10)
AS
DECLARE @tempTable TABLE
(
UserName nvarchar(50) not null, 
ListName nvarchar(50) not null, 
ScreeningId varchar(50) not null, 
DataDescription nvarchar(MAX) not null,
DataSource varchar(50), 
PeriodType varchar(10),
YearType char(8),
FromDate int,
ToDate int,
DataPointsOrder int not null,
CreatedBy nvarchar(50) not null,
CreatedOn datetime not null,
ModifiedBy nvarchar(50) not null,
ModifiedOn datetime not null
)
DECLARE @listId bigint;
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION
SET @listId = (select ListId from UserCustomisedListInfo where ListName = @existingListname);

    --TSQL Script to parse update xml
		
		DECLARE @XML XML
		SELECT @XML = @userPreference
		DECLARE @idoc int
		
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML
		
		DELETE from UserListDataPointMappingInfo 
		where ListId = @listId		
			
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
				DataDescription nvarchar(MAX) '@DataDescription',
				DataSource varchar(50) '@DataSource', 
				PeriodType varchar(10) '@PeriodType',
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
		WHERE ListName = @existingListname)l
		
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
				DROP TABLE #finalTable;				
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
		
	Update UserCustomisedListInfo
	SET ListName = @newListname,
	Accessibilty = @accessibility
	where ListId = @listId
	
	IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
		
		COMMIT TRANSACTION;
		SELECT 1;	
END


GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00077'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


