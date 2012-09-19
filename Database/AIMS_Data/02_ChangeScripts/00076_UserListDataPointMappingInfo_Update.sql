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

IF OBJECT_ID ('dbo.UserListDataPointMappingInfo') IS NOT NULL
	DROP TABLE dbo.UserListDataPointMappingInfo
GO

CREATE TABLE dbo.UserListDataPointMappingInfo
	(
	DataPointId     BIGINT IDENTITY NOT NULL,
	ListId          BIGINT NOT NULL,
	ScreeningId     VARCHAR (50) NOT NULL,
	DataDescription NVARCHAR (max) NOT NULL,
	DataSource      VARCHAR (50) NULL,
	PeriodType      VARCHAR (10) NULL,
	YearType        CHAR (8) NULL,
	FromDate        INT NULL,
	ToDate          INT NULL,
	DataPointsOrder INT NOT NULL,
	CreatedBy       NVARCHAR (50) NOT NULL,
	CreatedOn       DATETIME NOT NULL,
	ModifiedBy      NVARCHAR (50) NOT NULL,
	ModifiedOn      DATETIME NOT NULL,
	CONSTRAINT PK_tblUserListDataPointMappingInfo PRIMARY KEY (DataPointId),
	CONSTRAINT FK_UserListDataPointMappingInfo_UserCustomisedListInfo FOREIGN KEY (ListId) REFERENCES dbo.UserCustomisedListInfo (ListId) ON DELETE CASCADE ON UPDATE CASCADE
	)

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00076'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




