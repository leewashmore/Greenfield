SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[GetTargetPrice](@XRef nvarchar(15)) as

Select a.Xref, a.Ticker, a.CurrentPrice, a.CurrentPriceDate, a.Currency, b.StartDate, b.Median
	,  b.Currency as TargetCurrency, b.NumOfEsts, b.High, b.Low, b.StdDev, c.MeanLabel
  from tblCompanyInfo a
  left join tblCETargetPrice b on b.XREF = a.XREF and b.ExpirationDate is NULL 
  left join tblConsensusRecommendation c on c.XREF = a.XREF and c.ExpirationDate is NULL
 where 1=1
   and a.XRef = @XRef
GO
