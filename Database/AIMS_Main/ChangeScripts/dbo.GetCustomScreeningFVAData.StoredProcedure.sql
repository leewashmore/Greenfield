SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetCustomScreeningFVAData]
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
