set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00101'
declare @CurrentScriptVersion as nvarchar(100) = '00102'

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

IF OBJECT_ID ('[dbo].[ModelDeleteInteralStatement]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelDeleteInteralStatement]
GO

/****** Object:  StoredProcedure [dbo].[ModelDeleteInteralStatement]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModelDeleteInteralStatement] 
	(
	@ISSUER_ID VARCHAR(50),
	@ROOT_SOURCE VARCHAR(10)
	)
AS
BEGIN

	SET NOCOUNT ON;

Select REF_NO 
into #REF_LIST
from INTERNAL_STATEMENT
WHERE ISSUER_ID=@ISSUER_ID AND ROOT_SOURCE=@ROOT_SOURCE

Delete from INTERNAL_STATEMENT 
where ISSUER_ID=@ISSUER_ID and ROOT_SOURCE=@ROOT_SOURCE

SELECT * 
FROM #REF_LIST

DROP TABLE #REF_LIST

END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00102'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

