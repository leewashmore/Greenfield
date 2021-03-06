set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00058'
declare @CurrentScriptVersion as nvarchar(100) = '00059'

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

INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('ALUMINUM', 2094.469970703125, 2163.4599609375, 2646, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('COALSTEAM', 107, 116.37999725341797, 91, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('COPPER', 8145.56982421875, 8017.47998046875, 6615, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('LD', 1626.9300537109375, 1608.0699462890625, 1250, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('IRONORE', 150.5, 145, 110, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('LEAD', NULL, NULL, 90, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('NICKEL', 17894.30078125, 17373, 19845, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('PALLADIUM', NULL, NULL, 600, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('PALMOIL', 969, 950, 1180, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('PLATINUM', 1538.3199462890625, 1505.5999755859375, 1650, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('SILVER', 30.370000839233398, 29.420000076293945, 15, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('STEELUSHR', 769, 744, 791, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('TIN', NULL, NULL, 587, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('URANIUM', 58, 68, 50, '2012-05-17 11:47:32.273')


INSERT INTO COMMODITY_FORECASTS (COMMODITY_ID, CURRENT_YEAR_END, NEXT_YEAR_END, LONG_TERM, LASTUPDATE) VALUES ('ZINC', 1973.6099853515625, 1968.6099853515625, 2205, '2012-05-17 11:47:32.273')

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00059'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())