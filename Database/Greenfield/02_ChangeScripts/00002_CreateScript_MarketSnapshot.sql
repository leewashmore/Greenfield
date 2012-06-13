set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

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
--PUT YOUR CODE HERE:





IF OBJECT_ID ('dbo.tblMarketSnapshotPreference') IS NOT NULL
	DROP TABLE dbo.tblMarketSnapshotPreference
GO

CREATE TABLE dbo.tblMarketSnapshotPreference
	(
	SnapshotPreferenceId INT IDENTITY NOT NULL,
	SnapshotName         NVARCHAR (max) NOT NULL,
	UserId               NVARCHAR (100) NOT NULL,
	CONSTRAINT PK_tblMarketSnapshotPreference PRIMARY KEY (SnapshotPreferenceId)
	)
GO

IF OBJECT_ID ('dbo.tblMarketSnapshotGroupPreference') IS NOT NULL
	DROP TABLE dbo.tblMarketSnapshotGroupPreference
GO

CREATE TABLE dbo.tblMarketSnapshotGroupPreference
	(
	GroupPreferenceId    INT IDENTITY NOT NULL,
	SnapshotPreferenceId INT NOT NULL,
	GroupName            NVARCHAR (max) NOT NULL,
	CONSTRAINT PK_tblUserGroupPreference PRIMARY KEY (GroupPreferenceId),
	CONSTRAINT FK_tblMarketSnapshotGroupPreference_tblMarketSnapshotPreference FOREIGN KEY (SnapshotPreferenceId) REFERENCES dbo.tblMarketSnapshotPreference (SnapshotPreferenceId) ON DELETE CASCADE ON UPDATE CASCADE
	)
GO


IF OBJECT_ID ('dbo.tblMarketSnapshotEntityPreference') IS NOT NULL
	DROP TABLE dbo.tblMarketSnapshotEntityPreference
GO

CREATE TABLE dbo.tblMarketSnapshotEntityPreference
	(
	EntityPreferenceId INT IDENTITY NOT NULL,
	GroupPreferenceId  INT NOT NULL,
	EntityName         NVARCHAR (max) NOT NULL,
	EntityReturnType   NVARCHAR (50) NULL,
	EntityOrder        INT NOT NULL,
	EntityType         NVARCHAR (50) NOT NULL,
	CONSTRAINT PK_tblUserBenchmarkPreference PRIMARY KEY (EntityPreferenceId)
	)
GO

CREATE PROCEDURE [dbo].[GetMarketSnapshotPreference] 
	-- Add the parameters for the stored procedure here
	  @userid nvarchar(100),
	  @snapshotname nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select 
	tblMarketSnapshotGroupPreference.GroupName,
	tblMarketSnapshotGroupPreference.GroupPreferenceID,
    tblMarketSnapshotEntityPreference.EntityPreferenceId,
    tblMarketSnapshotEntityPreference.EntityName,
    tblMarketSnapshotEntityPreference.EntityOrder,
    tblMarketSnapshotEntityPreference.EntityReturnType,
    tblMarketSnapshotEntityPreference.EntityType
    
 from 
 tblMarketSnapshotGroupPreference LEFT OUTER JOIN tblMarketSnapshotEntityPreference
 
 ON
  tblMarketSnapshotGroupPreference.GroupPreferenceID = tblMarketSnapshotEntityPreference.GroupPreferenceID
  
  where 
  tblMarketSnapshotGroupPreference.SnapshotPreferenceId = 
 (SELECT SnapshotPreferenceId from tblMarketSnapshotPreference WHERE 
  UserId = @userid AND SnapshotName = @snapshotname)
 
END

GO


CREATE PROCEDURE [dbo].GetMarketSnapshotSelectionData 
	-- Add the parameters for the stored procedure here
	  @userId nVARCHAR(100)	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select SnapshotPreferenceId,SnapshotName from tblMarketSnapshotPreference
	WHERE UserId = @userId	
END

GO

CREATE PROCEDURE [dbo].SetMarketSnapshotPreference 
	-- Add the parameters for the stored procedure here
	  @userId nVARCHAR(100),
	  @snapshotname NVARCHAR(max)
AS
DECLARE @OrderCount int 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Insert into tblMarketSnapshotPreference (UserId,SnapshotName)	
	values (@userId , @snapshotname)
	
	Select SCOPE_IDENTITY()
	
END

GO

CREATE PROCEDURE [dbo].[SetMarketSnapshotGroupPreference] 
	-- Add the parameters for the stored procedure here
	  @snapshotpreferenceId int,
	  @groupname NVARCHAR(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
	Insert into tblMarketSnapshotGroupPreference(GroupName,SnapshotPreferenceId)	
	values (@groupname,@snapshotpreferenceId)
	
	Select SCOPE_IDENTITY()
	
END


GO


CREATE PROCEDURE [dbo].[SetMarketSnapshotEntityPreference] 
	-- Add the parameters for the stored procedure here
	  @grouppreferenceid int,
	  @entityName nvarchar(max),
	  @entityReturnType nvarchar(50),
	  @entityType nvarchar(50),
	  @entityOrder int
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
    -- Insert statements for procedure here
	Insert into tblMarketSnapshotEntityPreference(GroupPreferenceId,EntityName,EntityReturnType,EntityType,EntityOrder)
	
	Values (@grouppreferenceid,@entityName,@entityReturnType,@entityType,@entityOrder)
	
	Select SCOPE_IDENTITY()
	
END

GO

CREATE PROCEDURE [dbo].[UpdateMarketSnapshotEntityPreference] 
	-- Add the parameters for the stored procedure here
	  @grouppreferenceid int,
	  @entitypreferenceid int,
	  @entityOrder int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update tblMarketSnapshotEntityPreference
	SET GroupPreferenceId = @grouppreferenceid,
		EntityOrder = @entityOrder		
	where EntityPreferenceId = @entitypreferenceid
	Select @@ROWCOUNT
	
END


GO

CREATE PROCEDURE [dbo].[UpdateMarketSnapshotPreference] 
	-- Add the parameters for the stored procedure here
	  @userId NVARCHAR(100),
	  @snapshotname NVARCHAR(max),
	  @snapshotpreferenceid INT	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update tblMarketSnapshotPreference
	SET SnapshotName = @snapshotname		
	where UserId = @userId AND SnapshotPreferenceId = @snapshotpreferenceid
	Select @@ROWCOUNT
	
END

GO

CREATE PROCEDURE [dbo].[DeleteMarketSnapshotEntityPreference] 
	-- Add the parameters for the stored procedure here
	  @entitypreferenceid int
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from tblMarketSnapshotEntityPreference
	where EntityPreferenceId = @entitypreferenceid
	
	Select @@ROWCOUNT
    
END

GO


CREATE PROCEDURE [dbo].[DeleteMarketSnapshotGroupPreference] 
	-- Add the parameters for the stored procedure here
	  @grouppreferenceid int
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from tblMarketSnapshotGroupPreference
	where GroupPreferenceId = @grouppreferenceid
	
	Select @@ROWCOUNT
    
END


GO


CREATE PROCEDURE [dbo].DeleteMarketSnapshotPreference 
	-- Add the parameters for the stored procedure here
	  @userId nVARCHAR(100),
	  @snapshotname NVARCHAR(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from tblMarketSnapshotPreference	
	where UserId = @userId AND SnapshotName = @snapshotname	
	Select @@ROWCOUNT
	
END

GO



--END OF YOUR CODE.


--indicate thet current script is executed
if @@error = 0
begin
	declare @CurrentScriptVersion as nvarchar(100) = '00002'
	insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
end



