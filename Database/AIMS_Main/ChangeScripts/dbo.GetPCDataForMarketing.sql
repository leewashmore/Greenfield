
/****** Object:  StoredProcedure [dbo].[GetPCDataForMarketing]    Script Date: 05/07/2013 14:19:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPCDataForMarketing]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPCDataForMarketing]
GO


/****** Object:  StoredProcedure [dbo].[GetPCDataForMarketing]    Script Date: 05/07/2013 14:19:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

------------------------------------------------------------------------
-- Purpose:	Retrieve list of Portfolios
--
-- Author:	Akhtar Nazirali
-- Date:	05-06-2013
------------------------------------------------------------------------
CREATE procedure [dbo].[GetPCDataForMarketing]
@effective_date datetime,
@portfolio_id varchar(20) = null

As
begin

if @portfolio_id = null
begin 
	select portfolio_id, issuer_id,asec_sec_short_name ,dirty_price , dirty_value_pc from 
	dbo.GF_PORTFOLIO_LTHOLDINGS_HISTORY where portfolio_date = @effective_date
end
else
begin
	select portfolio_id, issuer_id,asec_sec_short_name ,dirty_price , dirty_value_pc from 
	dbo.GF_PORTFOLIO_LTHOLDINGS_HISTORY where portfolio_date = @effective_date and portfolio_id = @portfolio_id
end


end
--exec [dbo].GetPCDataForMarketing '04/30/2013'
GO


