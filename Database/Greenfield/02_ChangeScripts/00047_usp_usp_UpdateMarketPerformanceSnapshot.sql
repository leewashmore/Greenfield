--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00046'
declare @CurrentScriptVersion as nvarchar(100) = '00047'
--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 return
--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
 RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
 return
end

IF OBJECT_ID ('[dbo].[usp_UpdateMarketPerformanceSnapshot]') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_UpdateMarketPerformanceSnapshot]
GO

CREATE PROCEDURE [dbo].[usp_UpdateMarketPerformanceSnapshot]  
	@snapshotPreferenceId INT, 
	@updateXML VARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
		
	--TSQL Script to parse update xml
	BEGIN TRANSACTION
		
		DECLARE @snapshotPreferenceCount INT
		SELECT @snapshotPreferenceCount = COUNT(*) FROM dbo.tblMarketSnapshotPreference
		WHERE SnapshotPreferenceId = @snapshotPreferenceId
		
		IF @snapshotPreferenceCount = 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN			
			END
			
		DECLARE @XML XML
		SELECT @XML = @updateXML
		DECLARE @idoc int
		
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML
		
		--CREATE GROUP
		DECLARE @CreateGroupCount INT
		SELECT @CreateGroupCount = COUNT(*) FROM OPENXML(@idoc, '/Root/CreateGroup', 1)
			WITH ( GroupName VARCHAR(50))
		
		
		
		IF @CreateGroupCount <> 0
			BEGIN
				INSERT INTO dbo.tblMarketSnapshotGroupPreference (SnapshotPreferenceId, GroupName)
				SELECT SnapshotPreferenceId = @snapshotPreferenceId, GroupName 
				FROM OPENXML(@idoc, '/Root/CreateGroup', 1)
					WITH ( GroupName VARCHAR(50))
				
				IF @@ROWCOUNT <> @CreateGroupCount
					BEGIN
						ROLLBACK TRANSACTION
						SELECT 1
						RETURN
					END
			END
						
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 1
				RETURN
			END					
		
		--CREATE GROUP ENTITIES
		IF @CreateGroupCount <> 0
			BEGIN
				DECLARE @CreateGroupEntityCount INT
				SELECT @CreateGroupEntityCount = COUNT(*)
				FROM OPENXML(@idoc, '/Root/CreateGroup/CreateGroupEntity', 2)
				WITH (
					GroupName VARCHAR(50) '../@GroupName', 
					EntityName VARCHAR(50) '@EntityName', 
					EntityReturnType VARCHAR(50) '@EntityReturnType', 
					EntityOrder INT '@EntityOrder', 
					EntityType VARCHAR(50) '@EntityType')
				
				IF @CreateGroupEntityCount = 0
					BEGIN
						ROLLBACK TRANSACTION
						SELECT 2
						RETURN
					END
					
				SELECT GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
				INTO #CreateGroupUnion
				FROM (
					SELECT GroupPreferenceId, GroupName 
					FROM dbo.tblMarketSnapshotGroupPreference
					WHERE GroupName IN (
							SELECT * FROM OPENXML(@idoc, '/Root/CreateGroup', 1)
							WITH ( GroupName VARCHAR(50)))
						AND SnapshotPreferenceId = @snapshotPreferenceId) T1
				INNER JOIN (
						SELECT * FROM OPENXML(@idoc, '/Root/CreateGroup/CreateGroupEntity', 2)
						WITH (
							GroupName VARCHAR(50) '../@GroupName', 
							EntityName VARCHAR(50) '@EntityName', 
							EntityReturnType VARCHAR(50) '@EntityReturnType', 
							EntityOrder INT '@EntityOrder', 
							EntityType VARCHAR(50) '@EntityType')) T2
				ON T1.GroupName = T2.GroupName
						
				DECLARE @CreateGroupUnionCount INT
				SELECT @CreateGroupUnionCount = COUNT(*) FROM #CreateGroupUnion
						
				IF @CreateGroupUnionCount = 0
					BEGIN
						ROLLBACK TRANSACTION
						SELECT 2
						RETURN
					END
							
				INSERT INTO dbo.tblMarketSnapshotEntityPreference
				(GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType)
				SELECT GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
				FROM #CreateGroupUnion
						
				IF @@ROWCOUNT <> @CreateGroupUnionCount
					BEGIN
						ROLLBACK TRANSACTION
						SELECT 2
						RETURN
					END
			END
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 2
				RETURN
			END					
		
		
		--DELETE GROUP
		DECLARE @DeleteGroupCount INT
		SELECT @DeleteGroupCount = COUNT(*) 
			FROM OPENXML(@idoc, '/Root/DeleteGroup', 1)
			WITH (GroupPreferenceId INT)
			
		IF @DeleteGroupCount <> 0
			BEGIN
				DELETE FROM dbo.tblMarketSnapshotGroupPreference
				WHERE GroupPreferenceId	IN
					(SELECT * FROM OPENXML(@idoc, '/Root/DeleteGroup', 1)
						WITH (GroupPreferenceId INT))
				
				IF @@ROWCOUNT <> @DeleteGroupCount
					BEGIN
						ROLLBACK TRANSACTION
						SELECT 3
						RETURN
					END
			END
				
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 3
				RETURN
			END
		
		--CREATE ENTITY
		DECLARE @CreateEntityCount INT
		SELECT @CreateEntityCount = COUNT(*)
		FROM OPENXML(@idoc, '/Root/CreateEntity', 1)
			WITH (
				GroupPreferenceId INT, 
				EntityName VARCHAR(50), 
				EntityReturnType VARCHAR(50), 
				EntityOrder INT, 
				EntityType VARCHAR(50))
		
		IF @CreateEntityCount <> 0
		BEGIN
			INSERT INTO dbo.tblMarketSnapshotEntityPreference
			(GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType)
			SELECT * FROM OPENXML(@idoc, '/Root/CreateEntity', 1)
			WITH (
				GroupPreferenceId INT, 
				EntityName VARCHAR(50), 
				EntityReturnType VARCHAR(50), 
				EntityOrder INT, 
				EntityType VARCHAR(50))
			
			IF @@ROWCOUNT <> @CreateEntityCount
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 4
				RETURN
			END
		END		
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 4
				RETURN
			END	
			
		--DELETE ENTITY
		DECLARE @DeleteEntityCount INT
		SELECT @DeleteEntityCount = COUNT(*) 
			FROM OPENXML(@idoc, '/Root/DeleteEntity', 1)
			WITH (EntityPreferenceId INT)
		
		IF @DeleteEntityCount <> 0
			BEGIN		
				DELETE FROM dbo.tblMarketSnapshotEntityPreference
				WHERE EntityPreferenceId IN (
					SELECT * FROM OPENXML(@idoc, '/Root/DeleteEntity', 1)
					WITH (EntityPreferenceId INT))
				
				IF @@ROWCOUNT <> @DeleteEntityCount
					BEGIN
						ROLLBACK TRANSACTION
						SELECT 5
						RETURN
					END
			END		
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 5
				RETURN
			END
			
		--UPDATE ENTITY
		DECLARE @UpdateEntityCount INT
		SELECT @UpdateEntityCount = COUNT (*)
		FROM OPENXML(@idoc, '/Root/UpdateEntity', 1)
		WITH (
			GroupPreferenceId INT,
			EntityPreferenceId INT, 
			EntityOrder INT)
			
		IF @UpdateEntityCount <> 0
		BEGIN
			UPDATE dbo.tblMarketSnapshotEntityPreference
			SET
				dbo.tblMarketSnapshotEntityPreference.GroupPreferenceId = TEMP.GroupPreferenceId,
				dbo.tblMarketSnapshotEntityPreference.EntityOrder = TEMP.EntityOrder
			FROM dbo.tblMarketSnapshotEntityPreference MSEP
			INNER JOIN
			(SELECT * FROM OPENXML(@idoc, '/Root/UpdateEntity', 1)
				WITH (
					GroupPreferenceId INT,
					EntityPreferenceId INT, 
					EntityOrder INT)) TEMP
			ON MSEP.EntityPreferenceId = TEMP.EntityPreferenceId
			
			IF @@ROWCOUNT <> @UpdateEntityCount
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 6
				RETURN
			END
		END
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT 6
				RETURN
			END
			
		COMMIT TRANSACTION
		SELECT 0
END

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00047'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

