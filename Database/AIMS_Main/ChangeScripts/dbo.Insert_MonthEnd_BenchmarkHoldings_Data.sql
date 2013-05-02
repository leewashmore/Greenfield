

/****** Object:  StoredProcedure [dbo].[Insert_MonthEnd_SecurityReference_Data]    Script Date: 05/01/2013 09:09:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Insert_MonthEnd_BenchmarkHoldings_Data]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Insert_MonthEnd_BenchmarkHoldings_Data]
GO



/****** Object:  StoredProcedure [dbo].[Insert_MonthEnd_BenchmarkHoldings_Data]    Script Date: 05/01/2013 09:09:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

-----------------------------------------------------------------------------------
-- Purpose:	Called every month and insert monthend benchmark holdings data
--
--			EffDate - Effective Date
--
-- Author:	Akhtar Nazirali 
-- Date:	May 1, 2013
-----------------------------------------------------------------------------------


CREATE procedure [dbo].[Insert_MonthEnd_BenchmarkHoldings_Data](
	

	@EffDate datetime

		
)
as
print @EffDate

	begin tran
		Merge dbo.GF_BENCHMARK_HOLDINGS_HISTORY as target
		using (select  b.* from dbo.GF_BENCHMARK_HOLDINGS b where portfolio_date = @EffDate) as source
		on (	cast(target.effective_date as date) = cast(source.portfolio_Date as date)
			and target.benchmark_id = source.benchmark_id
			and isnull(target.issuer_id,'')=isnull(source.issuer_id,'')
			and isnull(target.asec_sec_short_name,'') = isnull(source.asec_sec_short_name,'')
			)
		when matched then
			update set 
			target.[BENCHMARK_WEIGHT] = source.[BENCHMARK_WEIGHT]
		
		when not matched  then
		
			INSERT(EFFECTIVE_DATE, 
			BENCHMARK_ID,
			ISSUER_ID,
			ASEC_SEC_SHORT_NAME,
			BENCHMARK_WEIGHT
			)
			VALUES
			(	cast(source.portfolio_Date as date),
				source.benchmark_id,
				source.issuer_id,
				source.asec_sec_short_name,
				source.benchmark_weight
			)
		output $action, Inserted.*, Deleted.*;
	commit tran;
	
	
-- exec dbo.[Insert_MonthEnd_BenchmarkHoldings_Data] '04/30/2013' 



