SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[A] as

declare @ISSUER_ID varchar(20)
set @ISSUER_ID = '117621'

	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		  , a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
		  , a.DATA_ID, (a.AMOUNT - b.AMOUNT - c.AMOUNT) as AMOUNT, ' ' as CALCULATION_DIAGRAM, a.SOURCE_CURRENCY, a.AMOUNT_TYPE
	  into #A
	  from (select * from dbo.PERIOD_FINANCIALS pf 
			 where DATA_ID = 130					-- 
			   and pf.ISSUER_ID = @ISSUER_ID
			   and pf.PERIOD_TYPE = 'A'
			) a
	  left join (select * from dbo.PERIOD_FINANCIALS pf 
				  where DATA_ID = 291					-- 
				    and pf.ISSUER_ID = @ISSUER_ID
				    and pf.PERIOD_TYPE = 'A'
				 ) b on b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_YEAR = a.PERIOD_YEAR 
					and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
	  left join (select * from dbo.PERIOD_FINANCIALS pf 
				  where DATA_ID = 37					-- TTAX
				    and pf.ISSUER_ID = @ISSUER_ID
				    and pf.PERIOD_TYPE = 'A'
				 ) c on c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_YEAR = a.PERIOD_YEAR 
					and c.FISCAL_TYPE = a.FISCAL_TYPE and c.CURRENCY = a.CURRENCY

select 'calc_162 #A' as A, * from #A where currency = 'USD' and data_source = 'PRIMARY' and FISCAL_TYPE = 'FISCAL' Order by PERIOD_YEAR, PERIOD_TYPE


	select a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		  , a.ROOT_SOURCE_DATE, a.PERIOD_TYPE, a.PERIOD_YEAR, a.PERIOD_END_DATE, a.FISCAL_TYPE, a.CURRENCY
		  , 0 as DATA_ID, (a.AMOUNT + isnull(b.AMOUNT, 0.0) + isnull(c.AMOUNT, 0.0)) as AMOUNT, ' ' as CALCULATION_DIAGRAM
		  , a.SOURCE_CURRENCY, a.AMOUNT_TYPE
		  , a.AMOUNT as AMOUNT104, b.AMOUNT as AMOUNT92, c.AMOUNT as AMOUNT190
	  into #B
	  from (select *
			  from dbo.PERIOD_FINANCIALS pf 
			 where pf.DATA_ID = 104				-- QTLE
			   and pf.ISSUER_ID = @ISSUER_ID
			   and pf.PERIOD_TYPE = 'A'
			) a
	   left join
			(select *
			  from dbo.PERIOD_FINANCIALS pf 
			 where pf.DATA_ID = 92				-- LMIN
			   and pf.ISSUER_ID = @ISSUER_ID
			   and pf.PERIOD_TYPE = 'A'
			) b on b.DATA_SOURCE = a.DATA_SOURCE and b.PERIOD_YEAR = a.PERIOD_YEAR 
				and b.FISCAL_TYPE = a.FISCAL_TYPE and b.CURRENCY = a.CURRENCY
	   left join
			(select *
			  from dbo.PERIOD_FINANCIALS pf 
			 where pf.DATA_ID = 190
			   and pf.ISSUER_ID = @ISSUER_ID
			   and pf.PERIOD_TYPE = 'A'
			) c on c.DATA_SOURCE = a.DATA_SOURCE and c.PERIOD_YEAR = a.PERIOD_YEAR 
				and c.FISCAL_TYPE = a.FISCAL_TYPE and c.CURRENCY = a.CURRENCY

select 'calc_162 #B' as B, * from #B where currency = 'USD' and data_source = 'PRIMARY' and FISCAL_TYPE = 'FISCAL' Order by PERIOD_YEAR, PERIOD_TYPE


-- exec A
GO
