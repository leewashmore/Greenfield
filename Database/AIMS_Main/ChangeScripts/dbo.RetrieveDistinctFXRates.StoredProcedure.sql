SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[RetrieveDistinctFXRates]
AS
BEGIN
select Distinct(Currency) from FX_RATES
END
GO
