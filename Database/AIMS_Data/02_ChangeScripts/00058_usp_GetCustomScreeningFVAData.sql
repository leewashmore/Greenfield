set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00057'
declare @CurrentScriptVersion as nvarchar(100) = '00058'

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

CREATE PROCEDURE [dbo].[GetCustomScreeningFVAData]
(
@securityIdsList varchar(max),
@dataSource varchar(20)
)
AS

SET FMTONLY OFF

BEGIN
DECLARE @tempTable TABLE
(
SECURITY_ID varchar(20) not null,
DATA_DESC varchar(100) not null,
FV_BUY decimal(32,6) not null,
FV_SELL decimal(32,6) not null,
CURRENT_MEASURE_VALUE decimal(32,6) not null,
UPSIDE decimal(32,6) not null
)
DECLARE @sqlquery varchar(max);

if @securityIdsList is not null
begin 

set @sqlquery = 'Select a.SECURITY_ID,b.DATA_DESC,a.FV_BUY,a.FV_SELL,a.CURRENT_MEASURE_VALUE,a.UPSIDE
	from FAIR_VALUE a,  DATA_MASTER b 
	where a.FV_MEASURE = b.DATA_ID
	and a.VALUE_TYPE = ''' +@dataSource+'''  
	and a.SECURITY_ID IN ('+@securityIdsList+')'

end

INSERT INTO @tempTable  EXECUTE(@sqlquery)

Select * from @tempTable;
END 

GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00058'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())




