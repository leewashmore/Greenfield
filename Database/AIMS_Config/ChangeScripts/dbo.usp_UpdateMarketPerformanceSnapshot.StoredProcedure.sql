SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[usp_UpdateMarketPerformanceSnapshot]  
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
		
		--CREATE SNAPSHOT
		DECLARE @SnapshotPreferenceId INT
		SELECT @SnapshotPreferenceId = SnapshotPreferenceId FROM OPENXML(@idoc, '/Root', 1)
			WITH ( SnapshotPreferenceId VARCHAR(50))
		
		IF @SnapshotPreferenceId IS NULL
			BEGIN
				DECLARE @SnapshotName VARCHAR(50)
				SELECT @SnapshotName = SnapshotName FROM OPENXML(@idoc, '/Root', 1)
					WITH ( SnapshotName VARCHAR(50))
					
				DECLARE @UserName VARCHAR(50)
				SELECT @UserName = UserName FROM OPENXML(@idoc, '/Root', 1)
					WITH ( UserName VARCHAR(50))
				
				IF @SnapshotName IS NOT NULL AND @UserName IS NOT NULL
					BEGIN
						INSERT INTO dbo.tblMarketSnapshotPreference	( SnapshotName, UserId )
						VALUES (@SnapshotName, @UserName)
						
						SELECT @SnapshotPreferenceId = @@IDENTITY
						
						IF @@ROWCOUNT <> 1
							BEGIN
								ROLLBACK TRANSACTION
								SELECT -10
								RETURN
							END
					END
				ELSE
					BEGIN
						ROLLBACK TRANSACTION
						SELECT -11
						RETURN
					END
			END
		
		DECLARE @snapshotPreferenceCount INT
		SELECT @snapshotPreferenceCount = COUNT(*) FROM dbo.tblMarketSnapshotPreference
		WHERE SnapshotPreferenceId = @snapshotPreferenceId
		
		IF @snapshotPreferenceCount = 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -12
				RETURN			
			END

		
		--CREATE GROUP
		DECLARE @CreateGroupCount INT
		SELECT @CreateGroupCount = COUNT(*) FROM OPENXML(@idoc, '/Root/CreateGroup', 1)
			WITH ( GroupName VARCHAR(50))
		
		
		
		IF @CreateGroupCount <> 0
			BEGIN
				INSERT INTO dbo.tblMarketSnapshotGroupPreference (SnapshotPreferenceId, GroupName)
				SELECT SnapshotPreferenceId = @SnapshotPreferenceId, GroupName 
				FROM OPENXML(@idoc, '/Root/CreateGroup', 1)
					WITH ( GroupName VARCHAR(50))
				
				IF @@ROWCOUNT <> @CreateGroupCount
					BEGIN
						ROLLBACK TRANSACTION
						SELECT -20
						RETURN
					END
			END
						
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -21
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
					EntityType VARCHAR(50) '@EntityType',
					EntityId VARCHAR(50) '@EntityId',
					EntityNodeType VARCHAR(50) '@EntityNodeType',
					EntityNodeValueCode VARCHAR(50) '@EntityNodeValueCode',
					EntityNodeValueName VARCHAR(50) '@EntityNodeValueName')
				
				IF @CreateGroupEntityCount = 0
					BEGIN
						ROLLBACK TRANSACTION
						SELECT -30
						RETURN
					END
					
				SELECT GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
				, EntityId, EntityNodeType, EntityNodeValueCode, EntityNodeValueName
				INTO #CreateGroupUnion
				FROM (
					SELECT GroupPreferenceId, GroupName 
					FROM dbo.tblMarketSnapshotGroupPreference
					WHERE GroupName IN (
							SELECT * FROM OPENXML(@idoc, '/Root/CreateGroup', 1)
							WITH ( GroupName VARCHAR(50)))
						AND SnapshotPreferenceId = @SnapshotPreferenceId) T1
				INNER JOIN (
						SELECT * FROM OPENXML(@idoc, '/Root/CreateGroup/CreateGroupEntity', 2)
						WITH (
							GroupName VARCHAR(50) '../@GroupName', 
							EntityName VARCHAR(50) '@EntityName', 
							EntityReturnType VARCHAR(50) '@EntityReturnType', 
							EntityOrder INT '@EntityOrder', 
							EntityType VARCHAR(50) '@EntityType',
							EntityId VARCHAR(50) '@EntityId',
							EntityNodeType VARCHAR(50) '@EntityNodeType',
							EntityNodeValueCode VARCHAR(50) '@EntityNodeValueCode',
							EntityNodeValueName VARCHAR(50) '@EntityNodeValueName')) T2
				ON T1.GroupName = T2.GroupName
						
				DECLARE @CreateGroupUnionCount INT
				SELECT @CreateGroupUnionCount = COUNT(*) FROM #CreateGroupUnion
						
				IF @CreateGroupUnionCount = 0
					BEGIN
						ROLLBACK TRANSACTION
						SELECT -31
						RETURN
					END
				--SELECT GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
				--, EntityId, EntityNodeType, EntityNodeValueCode, EntityNodeValueName
				--FROM #CreateGroupUnion
					
				INSERT INTO dbo.tblMarketSnapshotEntityPreference
				(GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
				, EntityId, EntityNodeType, EntityNodeValueCode, EntityNodeValueName)
				SELECT GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
				, EntityId, EntityNodeType, EntityNodeValueCode, EntityNodeValueName
				FROM #CreateGroupUnion
				
				IF @@ROWCOUNT <> @CreateGroupUnionCount
					BEGIN
						ROLLBACK TRANSACTION
						SELECT @@ROWCOUNT
						SELECT -32
						RETURN
					END
					
				DROP TABLE #CreateGroupUnion
			END
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -33
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
						SELECT -40
						RETURN
					END
			END
				
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -41
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
				EntityType VARCHAR(50),
				EntityId VARCHAR(50),
				EntityNodeType VARCHAR(50),
				EntityNodeValueCode VARCHAR(50),
				EntityNodeValueName VARCHAR(50))
		
		IF @CreateEntityCount <> 0
		BEGIN
			INSERT INTO dbo.tblMarketSnapshotEntityPreference
			(GroupPreferenceId, EntityName, EntityReturnType, EntityOrder, EntityType
			, EntityId, EntityNodeType, EntityNodeValueCode, EntityNodeValueName)
			SELECT * FROM OPENXML(@idoc, '/Root/CreateEntity', 1)
			WITH (
				GroupPreferenceId INT, 
				EntityName VARCHAR(50), 
				EntityReturnType VARCHAR(50), 
				EntityOrder INT, 
				EntityType VARCHAR(50),
				EntityId VARCHAR(50),
				EntityNodeType VARCHAR(50),
				EntityNodeValueCode VARCHAR(50),
				EntityNodeValueName VARCHAR(50))
			
			IF @@ROWCOUNT <> @CreateEntityCount
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -50
				RETURN
			END
		END		
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -51
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
						SELECT -60
						RETURN
					END
			END		
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -61
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
				SELECT -70
				RETURN
			END
		END
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -71
				RETURN
			END
			
		COMMIT TRANSACTION
		SELECT @SnapshotPreferenceId
END
GO
