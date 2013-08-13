IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expSummaryFX]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expSummaryFX]
GO

/****** Object:  StoredProcedure [dbo].[expFXForecasts]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expSummaryFX]
as

select distinct fxr.CURRENCY 
into #CurrencyList
from dbo.FX_RATES fxr
where year(fxr.FX_DATE) in (
YEAR(getdate()),
YEAR(getdate())+1
)
order by fxr.CURRENCY

declare @Q1Y0 char(10) = '03-31-' + cast(year(getdate()) as char(4))
declare @Q2Y0 char(10) = '06-30-' + cast(year(getdate()) as char(4))
declare @Q3Y0 char(10) = '09-30-' + cast(year(getdate()) as char(4))
declare @Q4Y0 char(10) = '12-31-' + cast(year(getdate()) as char(4))

declare @Q1Y1 char(10) = '03-31-' + cast(year(getdate())+1 as char(4))
declare @Q2Y1 char(10) = '06-30-' + cast(year(getdate())+1 as char(4))
declare @Q3Y1 char(10) = '09-30-' + cast(year(getdate())+1 as char(4))
declare @Q4Y1 char(10) = '12-31-' + cast(year(getdate())+1 as char(4))

select cm.COUNTRY_CODE,
cm.COUNTRY_NAME,
cl.CURRENCY, 
fxr10.FX_RATE as 'Q1Y0', 
fxr20.FX_RATE as 'Q2Y0',
fxr30.FX_RATE as 'Q3Y0', 
fxr40.FX_RATE as 'Q4Y0',
fxr11.FX_RATE as 'Q1Y1', 
fxr21.FX_RATE as 'Q2Y1',
fxr31.FX_RATE as 'Q3Y1', 
fxr41.FX_RATE as 'Q4Y1'
from #CurrencyList cl
left join dbo.Country_Master cm on cl.currency = cm.CURRENCY_CODE
left join dbo.FX_RATES fxr10 on cl.CURRENCY = fxr10.CURRENCY and fxr10.FX_DATE = @Q1Y0
left join dbo.FX_RATES fxr20 on cl.CURRENCY = fxr20.CURRENCY and fxr20.FX_DATE = @Q2Y0
left join dbo.FX_RATES fxr30 on cl.CURRENCY = fxr30.CURRENCY and fxr30.FX_DATE = @Q3Y0
left join dbo.FX_RATES fxr40 on cl.CURRENCY = fxr40.CURRENCY and fxr40.FX_DATE = @Q4Y0
left join dbo.FX_RATES fxr11 on cl.CURRENCY = fxr11.CURRENCY and fxr11.FX_DATE = @Q1Y1
left join dbo.FX_RATES fxr21 on cl.CURRENCY = fxr21.CURRENCY and fxr21.FX_DATE = @Q2Y1
left join dbo.FX_RATES fxr31 on cl.CURRENCY = fxr31.CURRENCY and fxr31.FX_DATE = @Q3Y1
left join dbo.FX_RATES fxr41 on cl.CURRENCY = fxr41.CURRENCY and fxr41.FX_DATE = @Q4Y1
where cm.COUNTRY_CODE is not null


drop table #CurrencyList



go