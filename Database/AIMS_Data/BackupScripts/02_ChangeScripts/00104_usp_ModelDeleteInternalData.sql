set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00103'
declare @CurrentScriptVersion as nvarchar(100) = '00104'

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

IF OBJECT_ID ('[dbo].[ModelDeleteInternalData]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelDeleteInternalData]
GO

/****** Object:  StoredProcedure [dbo].[ModelDeleteInternalData]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelDeleteInternalData] 
	(
		@ISSUER_ID VARCHAR(50),
		@REF_NO VARCHAR(20)
	)
AS
BEGIN	
	
	SET NOCOUNT ON;
	
	DELETE FROM INTERNAL_DATA 
	WHERE ISSUER_ID=@ISSUER_ID 
	AND REF_NO=@REF_NO
    
END

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00104'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

