IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expFXForecasts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expFXForecasts]
GO

/****** Object:  StoredProcedure [dbo].[expFXForecasts]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expFXForecasts](
@Month int = 12
)
as

declare @MonthEnd char(6) = case when @Month = 12 then '12-31-' 
	when @Month = 3 then '03-31-' 
	when @Month = 6 then '06-30-'
	when @Month = 9 then '09-30-' 
	else '12-31-' 
	end

declare @YearM3 int = year(getdate())-3 
declare @YearM2 int = year(getdate())-2 
declare @YearM1 int = year(getdate())-1 
declare @Year int = year(getdate()) 
declare @Year1 int = year(getdate())+1 
declare @Year2 int = year(getdate())+2 
declare @Year3 int = year(getdate())+3 

select distinct fxr.CURRENCY 
into #CurrencyList
from dbo.FX_RATES fxr
where year(fxr.FX_DATE) in (
@YearM3,
@YearM2,
@YearM1,
@Year,
@Year1,
@Year2,
@Year3
)
order by fxr.CURRENCY

declare @YM3 char(10) = @MonthEnd + cast(@YearM3 as char(4))
declare @YM2 char(10) = @MonthEnd + cast(@YearM2 as char(4))
declare @YM1 char(10) = @MonthEnd + cast(@YearM1 as char(4))
declare @Y char(10) = @MonthEnd + cast(@Year as char(4))
declare @Y1 char(10) = @MonthEnd + cast(@Year1 as char(4))
declare @Y2 char(10) = @MonthEnd + cast(@Year2 as char(4))
declare @Y3 char(10) = @MonthEnd + cast(@Year3 as char(4))

declare @YM3S char(10) =   cast(@YearM3 as char(4)) + ' Spot' 
declare @YM3A char(10) =   cast(@YearM3 as char(4)) + ' Avg'
declare @YM2S char(10) =   cast(@YearM2 as char(4)) + ' Spot' 
declare @YM2A char(10) =   cast(@YearM2 as char(4)) + ' Avg'
declare @YM1S char(10) =   cast(@YearM1 as char(4)) + ' Spot' 
declare @YM1A char(10) =   cast(@YearM1 as char(4)) + ' Avg'
declare @YS char(10) =   cast(@Year as char(4)) + ' Spot' 
declare @YA char(10) =   cast(@Year as char(4)) + ' Avg'
declare @Y1S char(10) =   cast(@Year1 as char(4)) + ' Spot' 
declare @Y1A char(10) =   cast(@Year1 as char(4)) + ' Avg'
declare @Y2S char(10) =   cast(@Year2 as char(4)) + ' Spot' 
declare @Y2A char(10) =   cast(@Year2 as char(4)) + ' Avg'
declare @Y3S char(10) =   cast(@Year3 as char(4)) + ' Spot' 
declare @Y3A char(10) =   cast(@Year3 as char(4)) + ' Avg'


select cm.ASHEMM_PROPRIETARY_REGION_CODE AS REGION,
cm.COUNTRY_CODE,
cm.COUNTRY_NAME,
cl.CURRENCY, 
fxrM3.FX_RATE as YM3S,
fxrM2.FX_RATE as YM2S,
fxrM1.FX_RATE as YM1S,
fxr.FX_RATE as YS,
fxr1.FX_RATE as Y1S,
fxr2.FX_RATE as Y2S,
fxr3.FX_RATE as Y3S,
fxrM3.AVG12MonthRATE as YM3A,
fxrM2.AVG12MonthRATE as YM2A,
fxrM1.AVG12MonthRATE as YM1A,
fxr.AVG12MonthRATE as YA,
fxr1.AVG12MonthRATE as Y1A,
fxr2.AVG12MonthRATE as Y2A,
fxr3.AVG12MonthRATE as Y3A
into #Results
from #CurrencyList cl
left join dbo.Country_Master cm on cl.currency = cm.CURRENCY_CODE
left join dbo.FX_RATES fxrM3 on cl.CURRENCY = fxrM3.CURRENCY and fxrM3.FX_DATE = @YM3
left join dbo.FX_RATES fxrM2 on cl.CURRENCY = fxrM2.CURRENCY and fxrM2.FX_DATE = @YM2
left join dbo.FX_RATES fxrM1 on cl.CURRENCY = fxrM1.CURRENCY and fxrM1.FX_DATE = @YM1
left join dbo.FX_RATES fxr on cl.CURRENCY = fxr.CURRENCY and fxr.FX_DATE = @Y
left join dbo.FX_RATES fxr1 on cl.CURRENCY = fxr1.CURRENCY and fxr1.FX_DATE = @Y1
left join dbo.FX_RATES fxr2 on cl.CURRENCY = fxr2.CURRENCY and fxr2.FX_DATE = @Y2
left join dbo.FX_RATES fxr3 on cl.CURRENCY = fxr3.CURRENCY and fxr3.FX_DATE = @Y3
where cm.COUNTRY_CODE is not null
order by cm.ASHEMM_PROPRIETARY_REGION_CODE, cm.COUNTRY_NAME

exec tempdb..sp_rename '#Results.YM3S',@YM3S,'COLUMN'
exec tempdb..sp_rename '#Results.YM3A',@YM3A,'COLUMN'
exec tempdb..sp_rename '#Results.YM2S',@YM2S,'COLUMN'
exec tempdb..sp_rename '#Results.YM2A',@YM2A,'COLUMN'
exec tempdb..sp_rename '#Results.YM1S',@YM1S,'COLUMN'
exec tempdb..sp_rename '#Results.YM1A',@YM1A,'COLUMN'
exec tempdb..sp_rename '#Results.YS',@YS,'COLUMN'
exec tempdb..sp_rename '#Results.YA',@YA,'COLUMN'
exec tempdb..sp_rename '#Results.Y1S',@Y1S,'COLUMN'
exec tempdb..sp_rename '#Results.Y1A',@Y1A,'COLUMN'
exec tempdb..sp_rename '#Results.Y2S',@Y2S,'COLUMN'
exec tempdb..sp_rename '#Results.Y2A',@Y2A,'COLUMN'
exec tempdb..sp_rename '#Results.Y3S',@Y3S,'COLUMN'
exec tempdb..sp_rename '#Results.Y3A',@Y3A,'COLUMN'

select * 
from #Results


drop table #CurrencyList
drop table #Results


go