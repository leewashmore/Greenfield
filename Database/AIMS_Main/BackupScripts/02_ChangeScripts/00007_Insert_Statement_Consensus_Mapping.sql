set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00006'
declare @CurrentScriptVersion as nvarchar(100) = '00007'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('BAL', 1, 'Book Value Per Share', 20, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('BAL', 10, 'Net Debt', 10, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('BAL', 18, 'ROA', 30, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('BAL', 19, 'ROE', 40, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('CAS', 2, 'Capital Expenditures', 10, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('CAS', 3, 'Cash Flow Per Share', 30, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('CAS', 4, 'Dividends Per Share', 20, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 5, 'Earnings Per Share (Pre Exceptional)', 60, 'EBG')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 6, 'EBIT', 30, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 7, 'EBITDA', 20, NULL)
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 8, 'Earnings Per Share (Pre Exceptional)', 60, 'EPS')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 9, 'Earnings Per Share (Pre Exceptional)', 60, 'EPSREP')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 11, 'Net Income', 50, 'EPS')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 12, 'Net Income', 50, 'EBG')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 13, 'Net Income', 50, 'EPSREP')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 14, 'Income Before Tax', 40, 'EPS')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 15, 'Income Before Tax', 40, 'EBG')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 16, 'Income Before Tax', 40, 'EPSREP')
GO

INSERT INTO STATEMENT_CONSENSUS_MAPPING (STATEMENT_TYPE, ESTIMATE_ID, DESCRIPTION, SORT_ORDER, EARNINGS) VALUES ('INC', 17, 'Revenue', 10, NULL)
GO



--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00007'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



