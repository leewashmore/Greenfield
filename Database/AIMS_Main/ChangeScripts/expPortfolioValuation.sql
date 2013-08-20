IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expPortfolioValuation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expPortfolioValuation]
GO

/****** Object:  StoredProcedure [dbo].[expPortfolioValuation]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expPortfolioValuation](
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
left join dbo.PORTFOLIO_VALUATION_HISTORY pvh on R0.data_id = pvh.DATA_ID and pvh.PORTFOLIO_ID = @PORTFOLIO and pvh.EFFECTIVE_DATE = @EFFECTIVE and pvh.RELATIVE_PERIOD = 1 and R0.METHODOLOGY = pvh.METHODOLOGY 

declare @Year0 int = year(@EFFECTIVE) 
declare @Year1 int = year(@EFFECTIVE)+1 

declare @Y0 char(4) =   cast(@Year0 as char(4)) 
declare @Y1 char(4) =   cast(@Year1 as char(4)) 

exec tempdb..sp_rename '#REL1.REL0',@Y0,'COLUMN'
exec tempdb..sp_rename '#REL1.REL1',@Y1,'COLUMN'

select * 
from #REL1

drop table #REL0
drop table #REL1
