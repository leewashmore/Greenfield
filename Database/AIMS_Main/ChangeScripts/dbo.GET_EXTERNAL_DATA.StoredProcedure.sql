SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-----------------------------------------------------------------------------
-- Purpose:	Get the External data portion of the statement gadget.
-- Author:	David Muench
-- Date:	19-July-2012
-----------------------------------------------------------------------------

alter procedure [dbo].[GET_EXTERNAL_DATA] (
	@ISSUER_ID			varchar(20),
	@STATEMENT_TYPE		char(10),
	@PERIOD_TYPE		char(2),
	@FISCAL_TYPE		varchar(8),
	@CURRENCY			char(3)
) as

	-- Create a temp table to hold the possible values
	create table #t (
		STATEMENT_TYPE	char(3)		NOT NULL, 
		ESTIMATE_ID		integer		NOT NULL,
		NAME			varchar(50)	NOT NULL,
		SORT_ORDER		integer		NOT NULL
	);  
	-- Insert the possible values
	insert into #t values('INC', 17, 'Revenue', 10);
	insert into #t values('INC',  7, 'EBITDA', 20);
	insert into #t values('INC',  6, 'EBIT', 30);
	insert into #t values('INC', 16, 'Income Before Tax', 40);
	insert into #t values('INC', 13, 'Net Income', 50);
	insert into #t values('INC',  8, 'Earnings Per Share (Pre Exceptional)', 60);

	insert into #t values('BAL', 10, 'Net Debt', 10);
	insert into #t values('BAL',  1, 'Book Value Per Share', 20);
	insert into #t values('BAL', 18, 'ROA', 30);
	insert into #t values('BAL', 19, 'ROE', 40);

	insert into #t values('CAS',  2, 'Capital Expenditures', 10);
	insert into #t values('CAS',  4, 'Dividends Per Share', 20);
	insert into #t values('CAS',  3, 'Cash Flow Per Share', 30);


	-- Get the data
	select t.NAME, t.SORT_ORDER, cce.PERIOD_YEAR, cce.PERIOD_TYPE, cce.AMOUNT, cce.AMOUNT_TYPE
	  from dbo.CURRENT_CONSENSUS_ESTIMATES cce
	 inner join #t t on t.ESTIMATE_ID = cce.ESTIMATE_ID
	 where t.STATEMENT_TYPE = @STATEMENT_TYPE
	   AND cce.ISSUER_ID = @ISSUER_ID
	   AND cce.PERIOD_TYPE = @PERIOD_TYPE
	   AND cce.FISCAL_TYPE = @FISCAL_TYPE
	   AND cce.CURRENCY = @CURRENCY
	   and cce.AMOUNT_TYPE = 'ESTIMATE'
	 order by t.SORT_ORDER
GO
