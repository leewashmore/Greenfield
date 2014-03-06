IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[New_BU_Portfolio_SETUP_COPY_TARGETS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[New_BU_Portfolio_SETUP_COPY_TARGETS]

GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


create procedure [dbo].[New_BU_Portfolio_SETUP_COPY_TARGETS]
(
	@sourportfolioID varchar(20),
	@destinationportfolio_id varchar(20),
	@username varchar(20)
)
as

insert into portfolio values
(@destinationportfolio_id,@destinationportfolio_id,1,NULL)

insert into username_fund values
(@username,@destinationportfolio_id)

--Cleanup
if @sourportfolioID is not null 
begin

--Variables and temp tables
CREATE TABLE #ID(ID int identity, Val int)

declare  @CalcID int
declare  @ChangeSetID int --unique to Portfolio
declare  @ChangeID int --unique to Portfolio and Security pair
declare  @PortfolioID char( 20 ) --PorfolioID used for loop
declare  @SecurityID char( 20 ) --SecurityID used for loop 
declare  @Target dec(32,6) -- actual target


--Go get targets (code comes from view Portfolio_Security_Targets_Union) 



select * into #TargetsUnClean from(
select @destinationportfolio_id as portfolio_id ,SECURITY_ID as SECURITY_ID,TARGET as target_pct,getdate() as updated
from BU_PORTFOLIO_SECURITY_TARGET where portfolio_id = @sourportfolioID) as t


---select * from #TargetsUnClean where portfolio_id = 'LSCF'

select * into #Targets from
	( select PORTFOLIO_ID, SECURITY_ID, sum(TARGET_PCT) as TARGET_PCT from #TargetsUnClean where TARGET_PCT > 0 group by PORTFOLIO_ID, SECURITY_ID) as u
--select * from #Targets

--Loop through Portfolios
--SELECT @PortfolioID = min( NAME ) from [AIMS_Main].[dbo].[PORTFOLIO] where IS_BOTTOM_UP = 0
SELECT @PortfolioID = @destinationportfolio_id



	--Go Get CalcID and put it into TARGETING_CALCULATION (log for calculation; iterates by 1;will also be populated later in script with start and stop times)
	INSERT INTO #ID
	exec CLAIM_IDS @sequenceName=N'TARGETING_CALCULATION',@howMany=1
	select top 1 @CalcID = Val
	from #ID order by ID desc
	--select @CalcID as CalcID

	insert into  [TARGETING_CALCULATION] (  [ID], [STATUS_CODE], [QUEUED_ON], [STARTED_ON], [FINISHED_ON], [LOG]) values (@CalcID, 1, GETDATE(), NULL, NULL, NULL)
	truncate table #ID

	--Go Get @ChangeSetID and put it into BU_PORTFOLIO_SECURITY_TARGET_CHANGESET (log for changeset; this will iterates ID by 1)
	INSERT INTO #ID
	exec CLAIM_IDS @sequenceName=N'BU_PORTFOLIO_SECURITY_TARGET_CHANGESET',@howMany=1
	select @ChangeSetID = Val
	from #ID order by ID desc
	--select @ChangeSetID as ChangeSetID

	insert into [BU_PORTFOLIO_SECURITY_TARGET_CHANGESET] (  [ID], [USERNAME], [TIMESTAMP], [CALCULATION_ID]) values (@ChangeSetID, 'test6', GETDATE(), @CalcID)
	truncate table #ID

	--Loop through Securities
	SELECT @SecurityID = min( SECURITY_ID ) from #Targets where PORTFOLIO_ID = @PortfolioID

	while @SecurityID is not null
	begin
		
		--Get Target Details
		select @Target = TARGET_PCT from #Targets where PORTFOLIO_ID = @PortfolioID and SECURITY_ID = @SecurityID
		--Select @PortfolioID as PortfolioID
		--Select @SecurityID as SecurityID
		--select @Target as Target
		
		--Go Get @ChangeID: Unique to Portfolio and Security Change
		INSERT INTO #ID
		exec CLAIM_IDS @sequenceName=N'BU_PORTFOLIO_SECURITY_TARGET_CHANGE',@howMany=1
		select top 1 @ChangeID = Val
		from #ID order by ID desc
		--select @ChangeID as ChangeID

		--Push In Portfolio Changes
		insert into [BU_PORTFOLIO_SECURITY_TARGET_CHANGE] values (@ChangeID, @ChangeSetID, @PortfolioID,@SecurityID,@Target,@Target,'Migration from Topdown to Bottomup')
		delete [BU_PORTFOLIO_SECURITY_TARGET]  where  PORTFOLIO_ID = @PortfolioID and SECURITY_ID = @SecurityID -- Remove the current target if it exists
		insert into [BU_PORTFOLIO_SECURITY_TARGET] values (@PortfolioID,@SecurityID,@Target,@ChangeID)

	--End of Loop through Securities
		SELECT @SecurityID = min( SECURITY_ID ) from #Targets where SECURITY_ID > @SecurityID and PORTFOLIO_ID = @PortfolioID
	end

	--Update TARGETING_CALCULATION (log for calculation) with start and stop times
	update [TARGETING_CALCULATION] set  [STATUS_CODE] = 1,  [STARTED_ON] = GETDATE() where  [ID]= @CalcID
	update [TARGETING_CALCULATION] set  [STATUS_CODE] = 1, [FINISHED_ON] = GETDATE(), [LOG] = 'Migrated from topdown to bottom up' where  [ID]= @CalcID


--Loop through Portfolios
	--select @PortfolioID = min( NAME ) from [AIMS_Main].[dbo].[PORTFOLIO] where NAME > @PortfolioID and IS_BOTTOM_UP = 0
	
	


drop table #ID
drop table #TargetsUnClean
drop table #Targets

end
--exec	[New_BU_Portfolio_SETUP_COPY_TARGETS] 'STARS','SICVISCF','NAZIRALIA'

 

--select * from portfolio where id = 'SICVESC'

/*insert into username_fund values
('NAZIRALIA','SICVESC')*/



select * from [BU_PORTFOLIO_SECURITY_TARGET] where portfolio_id ='CURIANESC'
and security_id not in 
(select security_id from [BU_PORTFOLIO_SECURITY_TARGET] where portfolio_id = 'SICVESC')
