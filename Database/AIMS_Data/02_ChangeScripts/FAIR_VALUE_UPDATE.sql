IF OBJECT_ID ( 'FAIR_VALUE_UPDATE', 'P' ) IS NOT NULL 
DROP PROCEDURE FAIR_VALUE_UPDATE;
GO
------------------------------------------------------------------------
-- Purpose:	This procedure updates the FAIR_VALUES table 
--
-- Author:	David Muench
-- Date:	August 12, 2012
------------------------------------------------------------------------
create procedure FAIR_VALUE_UPDATE (
	@ISSUER_ID			varchar(20) = NULL		-- The company identifier		
,	@VALUE_TYPE			varchar(20) = NULL		-- Default to update all datasources
)
as

	update dbo.FAIR_VALUE
	   set CURRENT_MEASURE_VALUE = pf.AMOUNT
		,  UPSIDE = (fv.FV_SELL / pf.AMOUNT)-1
	  FROM dbo.FAIR_VALUE fv 
	 inner join dbo.PERIOD_FINANCIALS pf on pf.SECURITY_ID = fv.SECURITY_ID and pf.DATA_ID = fv.FV_MEASURE
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where (@ISSUER_ID is NULL or sb.ISSUER_ID = @ISSUER_ID)
	   and (@VALUE_TYPE is NULL or fv.VALUE_TYPE = @VALUE_TYPE)
	   and pf.CURRENCY = 'USD'
	   and pf.PERIOD_TYPE = 'C'
	   and (   (fv.VALUE_TYPE <> 'INDUSTRY' and pf.DATA_SOURCE = 'PRIMARY')
		    or (fv.VALUE_TYPE =  'INDUSTRY' and pf.DATA_SOURCE = 'INDUSTRY') )
	   and isnull(pf.AMOUNT, 0.0) <> 0.0

go

-- exec FAIR_VALUE_UPDATE '223340'
