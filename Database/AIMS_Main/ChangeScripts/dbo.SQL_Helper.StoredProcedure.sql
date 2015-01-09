/****** Object:  StoredProcedure [dbo].[SQL_Helper]    Script Date: 01/09/2015 09:37:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SQL_Helper]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SQL_Helper]
GO

USE [AIMS_Main]
GO

/****** Object:  StoredProcedure [dbo].[SQL_Helper]    Script Date: 01/09/2015 09:37:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SQL_Helper] (
	@task varchar(25) -- Task to perf
)
as

If @task  = 'PORTFOLIOS' 
BEGIN
     select distinct PORTFOLIO_ID from PORTFOLIO_VALUATION_HISTORY order by PORTFOLIO_ID
END

Else If @task = 'EFFECTIVE_DATES'
BEGIN
    select distinct EFFECTIVE_DATE from PORTFOLIO_VALUATION_HISTORY order by EFFECTIVE_DATE
END


GO


