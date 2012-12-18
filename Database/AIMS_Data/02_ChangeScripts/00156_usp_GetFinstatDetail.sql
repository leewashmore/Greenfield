set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00155'
declare @CurrentScriptVersion as nvarchar(100) = '00156'

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

IF OBJECT_ID ('[dbo].[GetFinstatDetail]') IS NOT NULL
	DROP PROCEDURE [dbo].[GetFinstatDetail]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetFinstatDetail] 
@issuerID varchar(20),
@securityId varchar(20),
@dataSource varchar(10),
@fiscalType char(8),
@currency char(3)
	
AS
BEGIN
--CREATING GROUPED DATA
	--when FINSTAT_DISPLAY.DATA_ID in PF
SELECT  aa.DATA_SOURCE,
		aa.ROOT_SOURCE,
		aa.ROOT_SOURCE_DATE,
		bb.ESTIMATE_ID,
		isnull(aa.PERIOD_YEAR,2300) as PERIOD_YEAR,
		aa.DATA_ID,
		aa.AMOUNT,
		bb.MULTIPLIER,
		bb.DECIMALS,
		bb.PERCENTAGE,
		bb.BOLD_FONT,
		bb.GROUP_NAME,
		bb.SORT_ORDER,
		bb.HARMONIC,
		bb.DATA_DESC
FROM
(Select *
From PERIOD_FINANCIALS 
Where( ISSUER_ID = @issuerID  or SECURITY_ID = @securityId)
and DATA_SOURCE = @dataSource
and PERIOD_TYPE = 'A'
and FISCAL_TYPE = @fiscalType
and CURRENCY = @currency) aa
FULL JOIN
(Select a.*, b.DATA_DESC 
from FINSTAT_DISPLAY a, DATA_MASTER b
where a.COA_TYPE = (Select COA_TYPE from dbo.INTERNAL_ISSUER where ISSUER_ID = @issuerID)
and a.DATA_ID = b.DATA_ID) bb on aa.DATA_ID = bb.DATA_ID
where bb.DATA_ID IS NOT NULL
order by SORT_ORDER
END
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00156'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


