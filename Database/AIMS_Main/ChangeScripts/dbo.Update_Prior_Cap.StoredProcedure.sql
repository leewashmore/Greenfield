IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Update_Prior_Cap]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Update_Prior_Cap
GO

/****** Object:  StoredProcedure [dbo].[Update_Prior_Cap]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[Update_Prior_Cap]

as

	--Truncate the Cap Monitoring Table 	
	truncate table .dbo.MONITORING_DAILY_CAP_CHANGE


	--Extract out Market Cap data first
	select pf.SECURITY_ID, getdate() as CURR_DATE, pf.AMOUNT as PRIOR_CAP, 0 as CURR_CAP, pf.CALCULATION_DIAGRAM as PRIOR_DIAGRAM, '' as CURR_DIAGRAM
	into #Caps
		from .dbo.PERIOD_FINANCIALS pf
		where pf.DATA_ID = 185
		and pf.DATA_SOURCE = 'PRIMARY'
		and pf.CURRENCY = 'USD'
		and pf.PERIOD_TYPE = 'C'	
	
	
	--Then Extract out Shares data 
	select pf.security_id, pf.AMOUNT as PRIOR_SHARES, 0 as CURR_SHARES
	into #Shares
		from .dbo.PERIOD_FINANCIALS pf
		where pf.DATA_ID = 218
		and pf.DATA_SOURCE = 'PRIMARY'
		and pf.CURRENCY = 'USD'
		and pf.PERIOD_TYPE = 'C'	
	
	
	BEGIN TRAN T1
	insert into MONITORING_DAILY_CAP_CHANGE(security_id,curr_date,prior_cap, curr_cap, prior_diagram, curr_diagram, prior_shares, curr_shares)
	select c.SECURITY_ID, c.CURR_DATE, c.PRIOR_CAP, c.CURR_CAP, c.PRIOR_DIAGRAM, c.CURR_DIAGRAM, s.PRIOR_SHARES, s.CURR_SHARES
		from #Caps c
		join #Shares s on c.security_id = s.security_id 

	COMMIT TRAN T1	  
	
	
	drop table #Caps
	drop table #Shares
	
	
GO
