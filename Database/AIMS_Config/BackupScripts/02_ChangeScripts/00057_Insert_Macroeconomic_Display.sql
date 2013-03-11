set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00056'
declare @CurrentScriptVersion as nvarchar(100) = '00057'

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

INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('NOMINAL_GDP', 'Country Data', 'Nominal GDP ($BN)', '', NULL, 1, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('GDP_PER_CAPITA', 'Country Data', 'GDP Per Capita ($)', '', NULL, 2, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('POPULATION', 'Country Data', 'Total Population (MN)', '', NULL, 3, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('UNEMPLOYMENT_PCT', 'Country Data', 'Unemployment %', '', NULL, 4, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('MANUFACTURING_WAGE', 'Country Data', 'Manufacturing Wage (Local/Month)', '', NULL, 5, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('ST_INTEREST_RATE', 'Country Data', 'Short Term Interest Rate', '', NULL, 6, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('REAL_GDP_GROWTH_RATE', 'Growth', 'Real GDP Growth Rate %', '', NULL, 7, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('INFLATION_PCT', 'Growth', 'Inflation %', '', NULL, 8, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('WPI_PCT', 'Growth', 'WPI %', '', NULL, 9, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('INDUSTRIAL_PRODUCTION_PCT', 'Growth', 'Industrial Production % (Local)', '', NULL, 10, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('CONSUMPTION_PCT', 'Growth', 'Consumption %', '', NULL, 11, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('INVESTMENT_PCT', 'Growth', 'Investment %', '', NULL, 12, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('EXPORT_GROWTH_RATE', 'Growth', 'Export Growth Rate % ($)', '', NULL, 13, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('IMPORT_GROWTH_RATE', 'Growth', 'Import Growth Rate % ($)', '', NULL, 14, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('POPULATION_GROWTH_RATE', 'Growth', 'Population Growth Rate %', '', NULL, 15, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('CAPITAL_INVESTMENT_PCT_GDP', 'Ratios', 'Gross Fixed Capital Investment as % GDP', '', NULL, 16, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('SAVINGS_PCT_GDP', 'Ratios', 'Gross Savings as % GDP', '', NULL, 17, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('VT_EXPENDITURES_PCT_GDP', 'Ratios', 'vernment Expenditures as % GDP', '', NULL, 18, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('VT_SURPLUS_DEFICIT_PCT_GDP', 'Ratios', 'vernment Surplus/Deficit as % GDP', '', NULL, 19, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('TRADE_SURPLUS_DEFICIT_PCT_GDP', 'Ratios', 'Trade Surplus/Deficit as % GDP', '', NULL, 20, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('CURRENT_ACCOUNT_PCT_GDP', 'Ratios', 'Current Account as % GDP', '', NULL, 21, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('EXPORTS_PCT_GDP', 'Ratios', 'Exports as % GDP', '', NULL, 22, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('IMPORTS_PCT_GDP', 'Ratios', 'Imports as % GDP', '', NULL, 23, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('FOREIGN_DEBT_PCT_GDP', 'Ratios', 'Foreign Debt as % GDP', '', NULL, 24, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('DEBT_SERVICE_RATIO_PCT_EXPORTS', 'Ratios', 'Debt Service Ratio as % of Exports', '', NULL, 25, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('FX_RESERVES_MONTHS_IMPORTS', 'Ratios', 'Foreign Exchange Reserves/Months Imports', '', NULL, 26, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('EXPORTS', 'External Accounts', 'Exports ($BN)', '', NULL, 27, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('IMPORTS', 'External Accounts', 'Imports ($BN)', '', NULL, 28, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('TRADE_ACCOUNT', 'External Accounts', 'Trade Account ($BN)', '', NULL, 29, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('CURRENT_ACCOUNT', 'External Accounts', 'Current Account ($BN)', '', NULL, 30, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('FX_RESERVES ', 'External Accounts', 'Foreign Exchange Reserves ($BN)', '', NULL, 31, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('FOREIGN_DEBT', 'External Accounts', 'Foreign Debt ($BN)', '', NULL, 32, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('TOURIST_ARRIVALS', 'External Accounts', 'Tourist Arrivals (YOY %)', '', NULL, 33, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('FOREIGN_INVESTMENT', 'External Accounts', 'Foreign Investment ($BN)', '', NULL, 34, '', 'CTYSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('REAL_GDP_GROWTH_RATE', 'Growth', 'Real GDP Growth Rate %', '', NULL, 1, '', 'EMSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('INFLATION_PCT', 'Growth', 'Inflation %', '', NULL, 2, '', 'EMSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('CURRENT_ACCOUNT_PCT_GDP', 'Ratios', 'Current Account as % GDP', '', NULL, 3, '', 'EMSUMMARY')


INSERT INTO Macroeconomic_Display (FIELD, CATEGORY_NAME, DESCRIPTION, DATATYPE, DECIMALS, SORT_ORDER, HELP_TEXT, DISPLAY_TYPE) VALUES ('ST_INTEREST_RATE', 'Country Data', 'Short Term Interest Rate', '', NULL, 4, '', 'EMSUMMARY')

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00057'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())