/****** Object:  StoredProcedure [dbo].[GetCompositePortfolios]    Script Date: 04/18/2013 15:07:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
------------------------------------------------------------------------
-- Purpose:	Retrieve list of portfolios in the composite that are active
--
-- Author:	Krish J
-- Date:	04-08-2013
------------------------------------------------------------------------
ALTER procedure [dbo].[GetCompositePortfolios] 
@compositeID varchar(100)
	
AS
BEGIN

SELECT	COMPOSITE, PORTFOLIO      
FROM	[COMPOSITE_MATRIX]
WHERE	COMPOSITE = @compositeID and 
		ACTIVE = 1
  
END
