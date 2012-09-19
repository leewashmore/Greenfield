set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00075'
declare @CurrentScriptVersion as nvarchar(100) = '00076'

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

UPDATE [dbo].[UserListDataPointMappingInfo]
   SET [ListId] = <ListId, bigint,>
      ,[ScreeningId] = <ScreeningId, varchar(50),>
      ,[DataDescription] = <DataDescription, nvarchar(max),>
      ,[DataSource] = <DataSource, varchar(50),>
      ,[PeriodType] = <PeriodType, varchar(10),>
      ,[YearType] = <YearType, char(8),>
      ,[FromDate] = <FromDate, int,>
      ,[ToDate] = <ToDate, int,>
      ,[DataPointsOrder] = <DataPointsOrder, int,>
      ,[CreatedBy] = <CreatedBy, nvarchar(50),>
      ,[CreatedOn] = <CreatedOn, datetime,>
      ,[ModifiedBy] = <ModifiedBy, nvarchar(50),>
      ,[ModifiedOn] = <ModifiedOn, datetime,>
 WHERE <Search Conditions,,>
GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00076'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




