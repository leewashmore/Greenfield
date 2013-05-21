
/****** Object:  StoredProcedure [dbo].[SaveUpdatedFairValueMeasures]    Script Date: 05/09/2013 11:05:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveUpdatedPortfolioValuation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].SaveUpdatedPortfolioValuation
GO



/****** Object:  StoredProcedure [dbo].[SaveUpdatedFairValueMeasures]    Script Date: 05/09/2013 11:05:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].SaveUpdatedPortfolioValuation
	@effective_date datetime,
	@portfolio_id VARCHAR(20),
	@method varchar(20),
	@relativePeriod int,
	@dataId int,
	@value decimal(32,6)
AS
BEGIN
	declare @cnt int=0;
	declare @myerror int;
	SET NOCOUNT ON;
	SELECT @cnt=count(*) from dbo.portfolio_Valuation_history where 
	EFFECTIVE_DATE = @effective_date
	and PORTFOLIO_ID = @portfolio_id
	and METHODOLOGY = @method
	and DATA_ID = @dataId
	and RELATIVE_PERIOD = @relativePeriod
	
	
	if @cnt=0 
	begin
		insert into dbo.portfolio_Valuation_history
		values(@effective_date,@portfolio_id,@method,@dataId,@value,@relativePeriod)
		set @myerror = @@ERROR;
	end
	else
		begin
			update dbo.portfolio_Valuation_history set amount = @value
			where EFFECTIVE_DATE = @effective_date 
			  and PORTFOLIO_ID = @portfolio_id
			  and METHODOLOGY = @method
			  and RELATIVE_PERIOD = @relativePeriod
			  and DATA_ID = @dataId;
			  set @myerror = @@ERROR;
		end
    
    return @myerror
END

GO
--select * from dbo.portfolio_valuation_history

--exec SaveUpdatedPortfolioValuation '04/30/2013','MIDEAST','PCT_OWNED',166,'1020.00'