set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00053'
declare @CurrentScriptVersion as nvarchar(100) = '00054'

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

IF OBJECT_ID ('dbo.SCREENING_DISPLAY_CURRENT') IS NOT NULL
	DROP TABLE dbo.SCREENING_DISPLAY_CURRENT
GO

CREATE TABLE dbo.SCREENING_DISPLAY_CURRENT
	(
	SCREENING_ID VARCHAR (50) NOT NULL,
	DATA_ID      INT NOT NULL,
	ESTIMATE_ID  INT NULL,
	MULTIPLIER   DECIMAL (32,6) NULL,
	[DECIMAL]    INT NULL,
	PERCENTAGE   CHAR (10) NULL
	)
GO

IF OBJECT_ID ('dbo.SCREENING_DISPLAY_FAIRVALUE') IS NOT NULL
	DROP TABLE dbo.SCREENING_DISPLAY_FAIRVALUE
GO

CREATE TABLE dbo.SCREENING_DISPLAY_FAIRVALUE
	(
	SCREENING_ID      VARCHAR (50) NOT NULL,
	DATA_DESC         VARCHAR (100) NOT NULL,
	SHORT_COLUMN_DESC VARCHAR (255) NOT NULL,
	LONG_DESC         VARCHAR (max) NOT NULL,
	TABLE_COLUMN      VARCHAR (50) NOT NULL,
	DATA_TYPE         VARCHAR (50) NOT NULL,
	MULTIPLIER        DECIMAL (32,6) NULL,
	[DECIMAL]         INT NULL,
	PERCENTAGE        CHAR (10) NULL
	)
GO

IF OBJECT_ID ('dbo.SCREENING_DISPLAY_PERIOD') IS NOT NULL
	DROP TABLE dbo.SCREENING_DISPLAY_PERIOD
GO

CREATE TABLE dbo.SCREENING_DISPLAY_PERIOD
	(
	SCREENING_ID VARCHAR (50) NOT NULL,
	DATA_ID      INT NOT NULL,
	ESTIMATE_ID  INT NULL,
	MULTIPLIER   DECIMAL (32,6) NULL,
	[DECIMAL]    INT NULL,
	PERCENTAGE   VARCHAR (1) NULL
	)
GO

IF OBJECT_ID ('dbo.SCREENING_DISPLAY_REFERENCE') IS NOT NULL
	DROP TABLE dbo.SCREENING_DISPLAY_REFERENCE
GO

CREATE TABLE dbo.SCREENING_DISPLAY_REFERENCE
	(
	SCREENING_ID      VARCHAR (50) NOT NULL,
	DATA_DESC         VARCHAR (100) NOT NULL,
	SHORT_COLUMN_DESC VARCHAR (255) NOT NULL,
	LONG_DESC         VARCHAR (max) NOT NULL,
	TABLE_COLUMN      VARCHAR (50) NOT NULL,
	DATA_TYPE         VARCHAR (50) NOT NULL,
	MULTIPLIER        DECIMAL (32,6) NULL,
	[DECIMAL]         INT NULL,
	PERCENTAGE        CHAR (10) NULL
	)
GO



--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00054'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



