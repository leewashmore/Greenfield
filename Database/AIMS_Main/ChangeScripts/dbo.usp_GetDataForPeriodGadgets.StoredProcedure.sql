
/****** Object:  StoredProcedure [dbo].[usp_GetDataForPeriodGadgets]    Script Date: 07/19/2013 09:53:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetDataForPeriodGadgets]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetDataForPeriodGadgets]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetDataForPeriodGadgets]    Script Date: 07/19/2013 09:53:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[usp_GetDataForPeriodGadgets]
(
@cSource Varchar(80),
@cFiscalType Varchar(80),
@cCurrency Varchar(80),
@cIssuerID Varchar(80),
@cSecurityID Varchar(80)
)
AS

BEGIN

--/*For Testing ONLY - start
--Declare @cSource Varchar(80),
--@cFiscalType Varchar(80),
--@cCurrency Varchar(80),
--@cIssuerID Varchar(80),
--@cSecurityID Varchar(80)
--set @cSource = 'PRIMARY'
--set @cFiscalType = 'FISCAL'
--set @cCurrency = 'USD'
--set @cIssuerID = '117929'
--set @cSecurityID = 6964
--/*For Testing ONLY - end

DECLARE @coatype VARCHAR(3)

SET @coatype = (SELECT TOP 1 COA_TYPE FROM INTERNAL_ISSUER WHERE ISSUER_ID = @cIssuerID)
select gt.GRID_ID as GridId, SHOW_GRID as ShowGrid, CASE WHEN CharIndex('(', GRID_DESC) <> 0
THEN SUBSTRING(GRID_DESC, 0, CharIndex('(', GRID_DESC)) 
ELSE GRID_DESC
END AS GridDesc,
iii.AMOUNT as Amount,iii.AMOUNT_TYPE as AmountType,iii.DATA_DESC as [Description],iii.DATA_SOURCE as DataSource,iii.DECIMALS as Decimals,iii.PERCENTAGE as IsPercentage,
isnull(iii.PERIOD_YEAR,2300) as PeriodYear,iii.ROOT_SOURCE as RootSource,iii.SORT_ORDER as SortOrder,Period_Type = 'A',gt.SORT_ORDER AS GridSortOrder, iii.MULTIPLIER AS MULTIPLIER	
into #temp
from dbo.GRID_TYPES gt 
 FULL JOIN ((Select a.GRID_ID
	,  a.SORT_ORDER
	,  a.DECIMALS
	,  a.PERCENTAGE
	,  a.DATA_DESC
	,  b.PERIOD_YEAR
	,  b.AMOUNT
	,  b.DATA_SOURCE
	,  b.ROOT_SOURCE
	,  b.AMOUNT_TYPE
	,  a.MULTIPLIER	
  from (Select fgd.*, dm.DATA_DESC  
  from dbo.FINANCIAL_GRIDS_DISPLAY fgd 
 inner join dbo.DATA_MASTER dm on dm.DATA_ID = fgd.DATA_ID
 where fgd.COA_TYPE = @coatype) a
 LEFT OUTER JOIN  (Select pf.* 
  from dbo.PERIOD_FINANCIALS pf
 inner join dbo.FINANCIAL_GRIDS_DISPLAY fgd 
			on fgd.DATA_ID = pf.DATA_ID 
 where (pf.ISSUER_ID = @cIssuerID OR pf.SECURITY_ID = @cSecurityID)
   and pf.PERIOD_TYPE = 'A'			-- or 'Q'
   and pf.DATA_SOURCE = @cSource		-- or 'PRIMARY' or 'INDUSTRY'
   and pf.FISCAL_TYPE = @cFiscalType		-- or 'CALENDAR'
   and pf.CURRENCY = @cCurrency
   AND fgd.COA_TYPE = @coatype) b  on b.DATA_ID = a.DATA_ID)    
   
   UNION
    
 (Select a.GRID_ID 
	,  a.SORT_ORDER
	,  a.DECIMALS
	,  a.PERCENTAGE
	,  a.DATA_DESC
	,  b.PERIOD_YEAR
	,  b.AMOUNT
	,  b.DATA_SOURCE
	,  ' ' as ROOT_SOURCE
	,  b.AMOUNT_TYPE
	,  a.MULTIPLIER
  from ((Select fgd.*, dm.DATA_DESC  
  from dbo.FINANCIAL_GRIDS_DISPLAY fgd 
 INNER JOIN dbo.DATA_MASTER dm on dm.DATA_ID = fgd.DATA_ID
 where fgd.COA_TYPE = @coatype) a
 INNER JOIN (Select cce.*   
  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
 LEFT OUTER JOIN dbo.FINANCIAL_GRIDS_DISPLAY fgd 
			on fgd.ESTIMATE_ID = cce.ESTIMATE_ID 
 where (cce.ISSUER_ID = @cIssuerID OR cce.SECURITY_ID = @cSecurityID)
   and cce.PERIOD_TYPE = 'A'				-- or 'Q'
   and cce.FISCAL_TYPE = @cFiscalType		-- or 'CALENDAR'
   and cce.CURRENCY = @cCurrency
   AND fgd.COA_TYPE = @coatype			
   and PERIOD_YEAR not in (Select pf.PERIOD_YEAR 
  from dbo.PERIOD_FINANCIALS pf
 inner join dbo.FINANCIAL_GRIDS_DISPLAY fgd 
			on fgd.DATA_ID = pf.DATA_ID 
 where (pf.ISSUER_ID = @cIssuerID OR pf.SECURITY_ID = @cSecurityID)
   and pf.PERIOD_TYPE = 'A'			-- or 'Q'
   and pf.DATA_SOURCE = @cSource		-- or 'PRIMARY' or 'INDUSTRY'
   and pf.FISCAL_TYPE = @cFiscalType
   AND fgd.COA_TYPE = @coatype
   and pf.CURRENCY = @cCurrency)) b  on b.ESTIMATE_ID = a.ESTIMATE_ID )))iii 
   on iii.GRID_ID = gt.GRID_ID
 where gt.COA_TYPE = @coatype
 --AND gt.SHOW_GRID = 'Y'
 order by gt.SORT_ORDER,iii.SORT_ORDER
 
--Select * from #temp 

 
 select  GridId,PeriodYear,[Description]
  into #temp2
 from #temp
 group by 
 GridId,[Description],PeriodYear
 having COUNT(*) = 2
 
 select a.GridId,a.PeriodYear,a.[Description],
 a.Amount,DataSource,Decimals,IsPercentage,RootSource,SortOrder,Period_Type,GridDesc,ShowGrid,AmountType,GridSortOrder, MULTIPLIER	 
 into #temp3
 from #temp a ,#temp2 b
 where a.GridId = b.GridId and  a.PeriodYear = b.PeriodYear and a.[Description] = b.[Description] and a.AmountType = 'Actual'
 
 select  GridId,PeriodYear,[Description]
 into #temp4
 from #temp
 group by 
 GridId,[Description],PeriodYear
 having COUNT(*) = 1
 
 select a.GridId,a.PeriodYear,a.[Description],
 a.Amount,DataSource,Decimals,IsPercentage,RootSource,SortOrder,Period_Type,GridDesc,ShowGrid,AmountType,GridSortOrder, MULTIPLIER	 
 into #temp5
 from #temp a ,#temp4 b
 where a.GridId = b.GridId and  isnull(a.PeriodYear,'99999') = isnull(b.PeriodYear,'99999') and a.[Description] = b.[Description]
 
select * from #temp3
union 
select * from #temp5
ORDER BY GridSortOrder,SortOrder   

  
drop table #temp,#temp2,#temp3,#temp4,#temp5

END

GO


