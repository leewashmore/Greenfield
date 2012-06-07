
/****** Object:  StoredProcedure [dbo].[usp_UpdateMarketPerformanceSnapshot]    Script Date: 06/05/2012 11:01:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rahul Vig
-- Create date: 28-05-2012
-- Description:	Post updates to market performance snapshot
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateMarketPerformanceSnapshot] 
	@snapshotPreferenceId INT, 
	@updateXML VARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
		
	--TSQL Script to parse update xml
	BEGIN TRANSACTION
			
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
						WITH ( GroupName VARCHAR(50)))) T1
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

GO

