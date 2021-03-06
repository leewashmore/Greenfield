USE [AIMS_Main]
GO
/****** Object:  StoredProcedure [dbo].[expPortfolioValuation]    Script Date: 08/22/2014 16:20:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

ALTER procedure [dbo].[expPortfolioValuation](
@PORTFOLIO varchar(20) = '',
@EFFECTIVE datetime = '2012-12-31'
--declare @METHODOLOGY varchar(20) = 'PCT_OWNED'
)
as

select pvh.PORTFOLIO_ID as PORTFOLIO, pvh.EFFECTIVE_DATE as EFFECTIVE, pvh.METHODOLOGY, pvh.DATA_ID, dm.DATA_DESC as DESCRIPTION, pvh.amount as REL0
into #REL0
from dbo.PORTFOLIO_VALUATION_HISTORY pvh
	join dbo.DATA_MASTER dm on pvh.DATA_ID = dm.DATA_ID
where pvh.PORTFOLIO_ID = @PORTFOLIO
	and pvh.EFFECTIVE_DATE = @EFFECTIVE
	--and pvh.METHODOLOGY = @METHODOLOGY
	and pvh.RELATIVE_PERIOD = 0
order by dm.data_desc, pvh.methodology

select R0.*, pvh.AMOUNT as REL1
into #REL1
from #REL0 R0
	left join dbo.PORTFOLIO_VALUATION_HISTORY pvh on R0.data_id = pvh.DATA_ID 
		and pvh.PORTFOLIO_ID = @PORTFOLIO 
		and pvh.EFFECTIVE_DATE = @EFFECTIVE 
		and pvh.RELATIVE_PERIOD = 1 
		and R0.METHODOLOGY = pvh.METHODOLOGY 
		
select R1.*, pvh.AMOUNT as REL2
into #REL2
from #REL1 R1
	left join dbo.PORTFOLIO_VALUATION_HISTORY pvh on R1.data_id = pvh.DATA_ID 
		and pvh.PORTFOLIO_ID = @PORTFOLIO 
		and pvh.EFFECTIVE_DATE = @EFFECTIVE 
		and pvh.RELATIVE_PERIOD = 2 
		and R1.METHODOLOGY = pvh.METHODOLOGY 

declare @Year0 int = year(@EFFECTIVE) 
declare @Year1 int = year(@EFFECTIVE)+1 
declare @Year2 int = year(@EFFECTIVE)+2 

declare @Y0 char(4) =   cast(@Year0 as char(4)) 
declare @Y1 char(4) =   cast(@Year1 as char(4)) 
declare @Y2 char(4) =   cast(@Year2 as char(4)) 

exec tempdb..sp_rename '#REL2.REL0',@Y0,'COLUMN'
exec tempdb..sp_rename '#REL2.REL1',@Y1,'COLUMN'
exec tempdb..sp_rename '#REL2.REL2',@Y2,'COLUMN'

select * 
from #REL2

drop table #REL0
drop table #REL1
drop table #REL2
