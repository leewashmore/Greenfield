IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expStaleData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expStaleData]
GO

/****** Object:  StoredProcedure [dbo].[expStaleData]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expStaleData](
@Mode varchar(6) = 'NEXT'
)
as

DECLARE @WeekPriorDate DATETIME
SET @WeekPriorDate = (select dateadd(dd,-14,getdate()))

DECLARE @WeekNextDate DATETIME
SET @WeekNextDate = (select dateadd(dd,+14,getdate()))


select ISSUER_ID, max(ROOT_SOURCE_DATE) as LAST_UPDATED
into #Root
from dbo.INTERNAL_STATEMENT
group by ISSUER_ID

if @Mode = 'PRIOR'
	Begin
		--select out those companies that have expired in the last 2 weeks
		select r.ISSUER_ID, 
			MAX(gsb.issuer_name) as ISSUER_NAME, 
			MAX(gsb.ISO_COUNTRY_CODE) as COUNTRY, 
			MAX(gsb.ASHMOREEMM_PRIMARY_ANALYST) as ANALYST,
			MAX(r.LAST_UPDATED) as LAST_UPDATED,
			MAX(r.LAST_UPDATED) + 120 as EXPIRED
		into #AlreadyStale	
		from #Root r  
		join dbo.GF_SECURITY_BASEVIEW gsb on r.issuer_id = gsb.ISSUER_ID
		where r.LAST_UPDATED+120 > @WeekPriorDate and r.LAST_UPDATED+120 < GETDATE()
		group by r.ISSUER_ID

		select *
		from #AlreadyStale a
		order by a.analyst, a.expired

		drop table #AlreadyStale	
	End
else
	Begin
		--select out thoise companies that will expire in the next 2 weeks
		select r.ISSUER_ID, 
			MAX(gsb.issuer_name) as ISSUER_NAME, 
			MAX(gsb.ISO_COUNTRY_CODE) as COUNTRY, 
			MAX(gsb.ASHMOREEMM_PRIMARY_ANALYST) as ANALYST,
			MAX(r.LAST_UPDATED) as LAST_UPDATED,
			MAX(r.LAST_UPDATED) + 120 as EXPIRES
		into #AlmostStale	
		from #Root r  
		join dbo.GF_SECURITY_BASEVIEW gsb on r.issuer_id = gsb.ISSUER_ID
		where r.LAST_UPDATED+120 > getdate() and r.LAST_UPDATED+120 < @WeekNextDate
		group by r.ISSUER_ID

		select *
		from #AlmostStale a
		order by a.analyst, a.expires 

		drop table #AlmostStale	
	end

drop table #Root

go