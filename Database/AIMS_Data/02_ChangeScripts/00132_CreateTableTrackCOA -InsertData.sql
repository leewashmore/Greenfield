set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00131'
declare @CurrentScriptVersion as nvarchar(100) = '00132'

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

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
INSERT INTO Track_COA (Id, COA) VALUES (1, 'RTLR')
GO

INSERT INTO Track_COA (Id, COA) VALUES (2, 'SOPI')
GO

INSERT INTO Track_COA (Id, COA) VALUES (3, 'TTAX')
GO

INSERT INTO Track_COA (Id, COA) VALUES (4, 'NINC')
GO

INSERT INTO Track_COA (Id, COA) VALUES (5, 'CIAC')
GO

INSERT INTO Track_COA (Id, COA) VALUES (6, 'SCSI')
GO

INSERT INTO Track_COA (Id, COA) VALUES (7, 'STLD')
GO

INSERT INTO Track_COA (Id, COA) VALUES (8, 'LMIN')
GO

INSERT INTO Track_COA (Id, COA) VALUES (9, 'QTLE')
GO

INSERT INTO Track_COA (Id, COA) VALUES (10, 'SDED')
GO

INSERT INTO Track_COA (Id, COA) VALUES (11, 'SOCF')
GO

INSERT INTO Track_COA (Id, COA) VALUES (12, 'OTLO')
GO

INSERT INTO Track_COA (Id, COA) VALUES (13, 'SCEX')
GO

INSERT INTO Track_COA (Id, COA) VALUES (14, 'FCDP')
GO




Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00132'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

