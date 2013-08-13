IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[expCtyTargets]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[expCtyTargets]
GO

/****** Object:  StoredProcedure [dbo].[expCtyTargets]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[expCtyTargets]
as

select isnull(convert(varchar(200),cb.ISO_COUNTRY_CODE),convert(varchar(200),rb.DEFINITION)) as COUNTRY, 'STD' as BENCHMARK, tbv.BASKET_ID, tbv.BASE_VALUE
from dbo.TARGETING_TYPE_BASKET_BASE_VALUE tbv
left join dbo.COUNTRY_BASKET cb on tbv.BASKET_ID = cb.ID
left join dbo.REGION_BASKET rb on tbv.BASKET_ID = rb.ID
where tbv.TARGETING_TYPE_ID = 0	  



go