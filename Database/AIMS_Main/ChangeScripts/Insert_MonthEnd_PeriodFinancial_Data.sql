/****** Object:  StoredProcedure [dbo].[Insert_MonthEnd_SecurityReference_Data]    Script Date: 05/01/2013 09:09:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Insert_MonthEnd_PeriodFinancial_Data]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Insert_MonthEnd_PeriodFinancial_Data]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-----------------------------------------------------------------------------------
-- Purpose:	Called every month and insert monthend period financial data
--
--			RUN_MODE - F (Full Mode) or I (Issuer Specific Mode)
--			ISSUER_ID - List of Issuer id
--			EffDate - Effective Date
--
-- Author:	Akhtar Nazirali 
-- Date:	April 30, 2013
-----------------------------------------------------------------------------------


create procedure [dbo].[Insert_MonthEnd_PeriodFinancial_Data](
	
	@RUN_MODE	char(1)='F'
,	@ISSUER_ID	varchar(20)	
,	@EffDate datetime

		
)
as

if @RUN_MODE = 'F'
begin
	begin tran
		Merge dbo.period_financials_history as target
		using (Select @EffDate as EffDate,  pf.issuer_id,pf.security_id,pf.data_source,pf.root_source,pf.period_type,pf.period_year,pf.period_end_Date,pf.fiscal_type,pf.currency,pf.data_id,pf.amount,pf.amount_type
		from  PERIOD_FINANCIALS pf
		join DATA_FIELDS_EXTENSION  mdf on pf.data_source =  mdf.data_source
		and pf.period_type = mdf.period_type
		and pf.period_year = case when mdf.period_year is null  then 0 else YEAR(getdate()) + mdf.PERIOD_YEAR end
		and isnull(pf.fiscal_type,'') = isnull(mdf.fiscal_type,'')
		and pf.currency = mdf.currency
		and pf.data_id = mdf.data_id) as source
		on (	cast(target.effective_date as date) = cast(source.EffDate as date)
			and isnull(target.issuer_id,'')=isnull(source.issuer_id,'')
			and isnull(target.security_id,'') = isnull(source.security_id,'')
			and target.data_source = source.data_source
			and target.root_source = source.root_source
			and target.period_type = source.period_type
			and target.period_year = source.period_year
			and target.period_end_date = source.period_end_date
			and target.fiscal_type = source.fiscal_type
			and target.currency = source.currency
			and target.data_id = source.data_id
			and target.amount_type = source.amount_type)
		when matched then
		update set target.amount = source.amount
		when not matched by target then

		insert (effective_date,issuer_id,security_id,data_source,root_source,period_type,period_year,period_end_date,fiscal_type,currency,data_id,amount,amount_type)
		values(cast(source.EFFDate as date),source.issuer_id,source.security_id,source.data_source,source.root_source,source.period_type,source.period_year, source.period_end_date, source.fiscal_type,source.currency,source.data_id,source.amount,source.amount_type)
		output $action, Inserted.*, Deleted.*;
	commit tran;
end
else if @RUN_MODE = 'I'
begin
		-- Add records for issuer id
		Merge dbo.period_financials_history as target
		using (Select @EffDate as EffDate,  pf.issuer_id,pf.security_id,pf.data_source,pf.root_source,pf.period_type,pf.period_year,pf.period_end_Date,pf.fiscal_type,pf.currency,pf.data_id,pf.amount,pf.amount_type
		from  PERIOD_FINANCIALS pf
		join DATA_FIELDS_EXTENSION  mdf on pf.data_source =  mdf.data_source
		and pf.period_type = mdf.period_type
		and pf.period_year = case when mdf.period_year is null  then 0 else YEAR(getdate()) + mdf.PERIOD_YEAR end
		and isnull(pf.fiscal_type,'') = isnull(mdf.fiscal_type,'')
		and pf.currency = mdf.currency
		and pf.data_id = mdf.data_id where issuer_id = @issuer_id) as source
		on (	cast(target.effective_date as date) = cast(source.EffDate as date)
			and isnull(target.issuer_id,'')=isnull(source.issuer_id,'')
			and isnull(target.security_id,'') = isnull(source.security_id,'')
			and target.data_source = source.data_source
			and target.root_source = source.root_source
			and target.period_type = source.period_type
			and target.period_year = source.period_year
			and target.period_end_date = source.period_end_date
			and target.fiscal_type = source.fiscal_type
			and target.currency = source.currency
			and target.data_id = source.data_id
			and target.amount_type = source.amount_type)
		when matched then
		update set target.amount = source.amount
		when not matched by target then

		insert (effective_date,issuer_id,security_id,data_source,root_source,period_type,period_year,period_end_date,fiscal_type,currency,data_id,amount,amount_type)
		values(cast(source.EFFDate as date),source.issuer_id,source.security_id,source.data_source,source.root_source,source.period_type,source.period_year, source.period_end_date, source.fiscal_type,source.currency,source.data_id,source.amount,source.amount_type)
		output $action, Inserted.*, Deleted.*;
	
	-- Add records for associated securities:
	Merge dbo.period_financials_history as target
		using (Select @EffDate as EffDate,  pf.issuer_id,pf.security_id,pf.data_source,pf.root_source,pf.period_type,pf.period_year,pf.period_end_Date,pf.fiscal_type,pf.currency,pf.data_id,pf.amount,pf.amount_type
		from  PERIOD_FINANCIALS pf
		join DATA_FIELDS_EXTENSION  mdf on pf.data_source =  mdf.data_source
		and pf.period_type = mdf.period_type
		and pf.period_year = case when mdf.period_year is null  then 0 else YEAR(getdate()) + mdf.PERIOD_YEAR end
		and isnull(pf.fiscal_type,'') = isnull(mdf.fiscal_type,'')
		and pf.currency = mdf.currency
		and pf.data_id = mdf.data_id where 
		SECURITY_ID in (select SECURITY_ID from GF_SECURITY_BASEVIEW where issuer_id = @issuer_id)) as source
		on (	cast(target.effective_date as date) = cast(source.EffDate as date)
			and isnull(target.issuer_id,'')=isnull(source.issuer_id,'')
			and isnull(target.security_id,'') = isnull(source.security_id,'')
			and target.data_source = source.data_source
			and target.root_source = source.root_source
			and target.period_type = source.period_type
			and target.period_year = source.period_year
			and target.period_end_date = source.period_end_date
			and target.fiscal_type = source.fiscal_type
			and target.currency = source.currency
			and target.data_id = source.data_id
			and target.amount_type = source.amount_type)
		when matched then
		update set target.amount = source.amount
		when not matched by target then

		insert (effective_date,issuer_id,security_id,data_source,root_source,period_type,period_year,period_end_date,fiscal_type,currency,data_id,amount,amount_type)
		values(cast(source.EFFDate as date),source.issuer_id,source.security_id,source.data_source,source.root_source,source.period_type,source.period_year, source.period_end_date, source.fiscal_type,source.currency,source.data_id,source.amount,source.amount_type)
		output $action, Inserted.*, Deleted.*;
end







	
---- exec dbo.[Insert_MonthEnd_PeriodFinancial_Data] 'F','117929','04/30/2013' 

