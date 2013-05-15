IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPortfolioList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPortfolioList]
GO
GO
/****** Object:  StoredProcedure [dbo].[GetComposites]    Script Date: 05/06/2013 10:59:31 ******/
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
CREATE procedure [dbo].[GetPortfolioList]

As


select distinct portfolio_id,BENCHMARK_ID 
from dbo.GF_PORTFOLIO_LTHOLDINGS_HISTORY

