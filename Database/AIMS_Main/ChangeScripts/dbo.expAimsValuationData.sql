

/****** Object:  StoredProcedure [dbo].[expAimsMktCap]    Script Date: 12/23/2013 12:54:03 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expAimsValuationData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expAimsValuationData]
GO


/****** Object:  StoredProcedure [dbo].[expAimsMktCap]    Script Date: 12/23/2013 12:54:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO




CREATE procedure [dbo].[expAimsValuationData]
as
begin


SET NOCOUNT ON
declare @effectiveDate datetime
declare @findstr varchar(20)


select convert(varchar(10),Root_Source_Date,101) as Root_Source_Date, g.ASEC_SEC_SHORT_NAME,data_id,pf.amount  into #Temp from period_financials pf inner join gf_Security_baseview g on g.security_id = pf.security_id
where pf.data_id in( 185,308,306,302)  and pf.period_type = 'C' and pf.data_source = 'PRIMARY'
and pf.Currency = 'USD'
 
 select  @effectiveDate = max(root_source_Date) from #Temp

update #Temp set root_source_date = convert(varchar(10),@effectiveDate,101)

insert into #Temp (Root_source_Date,asec_sec_short_name,data_id,amount)
select convert(varchar(10),@effectiveDate,101) as Root_Source_Date, g.ASEC_SEC_SHORT_NAME,data_id,pf.amount   from period_financials pf inner join gf_Security_baseview g on g.issuer_id = pf.issuer_id
where pf.data_id in( 309,304)  and pf.period_type = 'C' and pf.data_source = 'PRIMARY'
and pf.Currency = 'USD'

set @findstr= '<FIND>' + SPACE(20-LEN('<FIND>'))
select @findstr,p.Root_Source_date,rtrim(p.asec_sec_short_name),isnull(rtrim(convert(varchar,p.mktcap)),'') as mktcap,
rtrim(isnull(convert(varchar,p.ForwardPE),'')) as ForwardPE,
rtrim(isnull(convert(varchar,p.ForwardPB),'')) as ForwardPB,
rtrim(isnull(convert(varchar,p.ForwardDY),''))  as ForwardDY,
rtrim(isnull(convert(varchar,p.ForwardROE),''))  as ForwardROE,
rtrim(isnull(convert(varchar,p.ForwardEarnings),''))  as ForwardEarnings
 from (
	select Root_Source_date,rtrim(asec_sec_short_name) as asec_sec_short_name,[185] as 'MktCap',
	   	   [308] as 'ForwardPE',
		   [306]as 'ForwardPB',
		   [302]as 'ForwardDY',
		   [309] as 'ForwardROE',
		   [304] as 'ForwardEarnings' from 

	(	select 
			root_source_date,
			asec_sec_short_name,
			data_id,
			amount 
		from #Temp
	)a
	pivot
	(
		sum(AMOUNT)
		for data_id in  ([185] ,
						 [308],
						 [306],
						 [302],
						 [309],
						 [304])
	)as p1
	)as p;

end

--exec [dbo].[expAimsValuationData] 'c:\temp\temp.csv'
GO


