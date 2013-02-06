set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00313'
declare @CurrentScriptVersion as nvarchar(100) = '00314'

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
            IF OBJECT_ID ( 'SET_INTERIM_AMOUNTS', 'P' ) IS NOT NULL 
DROP PROCEDURE SET_INTERIM_AMOUNTS;
GO
-----------------------------------------------------------------------------------
-- Purpose:	To take the Annual data provided by the Internal Analysts and make
--			quarterly data out of it.  THe models will be supplying the data for
--			Annual as well as the percentages to distribute the Annual values
--			into quarterly data.  THis procedure creates the quarterly values.
--
-- Author:	David Muench
-- Date:	July 23, 2012
-----------------------------------------------------------------------------------

create procedure SET_INTERIM_AMOUNTS (
	@ISSUER_ID			varchar(20)		= NULL			-- The company identifier		
,	@DATA_SOURCE		varchar(10)	= 'PRIMARY'		-- Default to the primary analyst.
)
as
	declare @DIST_COUNT	integer
	
	-- delete previous data
	delete from dbo.INTERNAL_DATA 
	 where (@ISSUER_ID is NULL or ISSUER_ID = @ISSUER_ID)
	   and PERIOD_TYPE in ('Q1', 'Q2', 'Q3', 'Q4')

	-- Get the number of rows for the Issuer, there should be 1 for each quarter = total of 4
	select @DIST_COUNT = count(*) 
	  from INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION 
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_SOURCE = @DATA_SOURCE
	 
	if @DIST_COUNT = 0
		BEGIN
			-- When there is no distribution for the Issuer, put in 25% for each quarter
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q1', 0.25, GETDATE())
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q2', 0.25, GETDATE())
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q3', 0.25, GETDATE())
			insert into INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION values (@ISSUER_ID, @DATA_SOURCE, 'Q4', 0.25, GETDATE())
			set @DIST_COUNT = 4		-- The count is now 4
		END


	if @DIST_COUNT = 4
		BEGIN
			-- insert the quarterly data
			Insert into dbo.INTERNAL_DATA (ISSUER_ID, REF_NO, PERIOD_TYPE, COA, AMOUNT)
			Select id.ISSUER_ID
				,  id.REF_NO
				,  iiqd.PERIOD_TYPE						-- The Period type being distributed to
				,  id.COA
				,  id.AMOUNT * iiqd.PERCENTAGE	as AMOUNT	-- The new amount
			  from dbo.INTERNAL_DATA id
			 inner join dbo.INTERNAL_STATEMENT s on s.ISSUER_ID = id.ISSUER_ID and s.REF_NO = id.REF_NO
			 inner join INTERNAL_ISSUER_QUARTERLY_DISTRIBUTION iiqd on iiqd.ISSUER_ID = id.ISSUER_ID and iiqd.DATA_SOURCE = s.ROOT_SOURCE
			 where (@ISSUER_ID is NULL or id.ISSUER_ID = @ISSUER_ID)
			   AND iiqd.DATA_SOURCE = @DATA_SOURCE
			   and id.PERIOD_TYPE = 'A'
			 order by id.REF_NO, id.COA, id.PERIOD_TYPE, iiqd.PERIOD_TYPE
			;
		END
go

-- exec SET_INTERIM_AMOUNTS '8131602'

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00314'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())