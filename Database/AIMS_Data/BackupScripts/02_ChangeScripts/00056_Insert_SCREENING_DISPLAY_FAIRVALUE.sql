set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00055'
declare @CurrentScriptVersion as nvarchar(100) = '00056'

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

INSERT INTO SCREENING_DISPLAY_FAIRVALUE (SCREENING_ID, DATA_DESC, SHORT_COLUMN_DESC, LONG_DESC, TABLE_COLUMN, DATA_TYPE, MULTIPLIER, DECIMAL, PERCENTAGE) VALUES ('FVA001', 'Fair Value Measure', 'FV Measure', 'Valuation measure chosen to express the fair value buy/sell range (e.g. Forward P/BV)', 'DATA_DESC', 'STRING', NULL, NULL, NULL)


INSERT INTO SCREENING_DISPLAY_FAIRVALUE (SCREENING_ID, DATA_DESC, SHORT_COLUMN_DESC, LONG_DESC, TABLE_COLUMN, DATA_TYPE, MULTIPLIER, DECIMAL, PERCENTAGE) VALUES ('FVA002', 'Buy', 'FV Buy', 'Buy value corresponding to the Fair Value Measure chosen', 'FV_BUY', 'DECIMAL', 1, 2, 'N')


INSERT INTO SCREENING_DISPLAY_FAIRVALUE (SCREENING_ID, DATA_DESC, SHORT_COLUMN_DESC, LONG_DESC, TABLE_COLUMN, DATA_TYPE, MULTIPLIER, DECIMAL, PERCENTAGE) VALUES ('FVA003', 'Sell', 'FV Sell', 'Sell value corresponding to the Fair Value Measure chosen', 'FV_SELL', 'DECIMAL', 1, 2, 'N')


INSERT INTO SCREENING_DISPLAY_FAIRVALUE (SCREENING_ID, DATA_DESC, SHORT_COLUMN_DESC, LONG_DESC, TABLE_COLUMN, DATA_TYPE, MULTIPLIER, DECIMAL, PERCENTAGE) VALUES ('FVA004', 'Current Measure Value', 'FV Measure Value', 'Current value corresponding to the Fair Value Measure chosen', 'CURRENT_MEASURE_VALUE', 'DECIMAL', 1, 2, 'N')


INSERT INTO SCREENING_DISPLAY_FAIRVALUE (SCREENING_ID, DATA_DESC, SHORT_COLUMN_DESC, LONG_DESC, TABLE_COLUMN, DATA_TYPE, MULTIPLIER, DECIMAL, PERCENTAGE) VALUES ('FVA005', 'Upside', 'Upside', 'Derived upside based on the Sell and Current value of the Fair Value Measure chosen.Calculation = (Sell value/Current value)-1', 'UPSIDE', 'DECIMAL', 100, 1, 'Y')
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00056'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

