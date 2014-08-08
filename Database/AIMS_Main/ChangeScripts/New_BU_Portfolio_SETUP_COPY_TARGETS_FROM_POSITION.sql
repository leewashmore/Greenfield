IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[New_BU_Portfolio_SETUP_COPY_TARGETS_FROM_POSITION]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[New_BU_Portfolio_SETUP_COPY_TARGETS_FROM_POSITION]

GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


create procedure [dbo].[New_BU_Portfolio_SETUP_COPY_TARGETS_FROM_POSITION]
(
	@sourportfolioID varchar(20),
	@destinationportfolio_id varchar(20),
	@username varchar(20)
)
as

/*insert into portfolio values
(@destinationportfolio_id,@destinationportfolio_id,1,NULL)

insert into username_fund values
(@username,@destinationportfolio_id) */

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

Create table   #TargetsUnClean
(
	PORTFOLIO_ID varchar(20),
	SECURITY_ID VARCHAR(20),
	TARGET_PCT decimal(32,6)
)


/*select * into #TargetsUnClean from(
select @destinationportfolio_id as portfolio_id ,SECURITY_ID as SECURITY_ID,TARGET as target_pct,getdate() as updated
from BU_PORTFOLIO_SECURITY_TARGET where portfolio_id = @sourportfolioID) as t */


---select * from #TargetsUnClean where portfolio_id = 'LSCF'
insert into #TargetsUnClean values ('OSPREYEQ','139684',	0.0048)
insert into #TargetsUnClean values ('OSPREYEQ','378017',	0.0101)
insert into #TargetsUnClean values ('OSPREYEQ','17510',	0.0248)
insert into #TargetsUnClean values ('OSPREYEQ','461999',	0.0196)
insert into #TargetsUnClean values ('OSPREYEQ','17237',	0.0104)
insert into #TargetsUnClean values ('OSPREYEQ','17616',	0.01)
insert into #TargetsUnClean values ('OSPREYEQ','145119',	0.0342)
insert into #TargetsUnClean values ('OSPREYEQ','227250',	0.0182)
insert into #TargetsUnClean values ('OSPREYEQ','161563',	0.0132)
insert into #TargetsUnClean values ('OSPREYEQ','109291',	0.009)
insert into #TargetsUnClean values ('OSPREYEQ','16383',	0.0166)
insert into #TargetsUnClean values ('OSPREYEQ','2365',		0.0332)
insert into #TargetsUnClean values ('OSPREYEQ','17630',	0.0256)
insert into #TargetsUnClean values ('OSPREYEQ','18383',	0.0122)
insert into #TargetsUnClean values ('OSPREYEQ','66082',	0.0091)
insert into #TargetsUnClean values ('OSPREYEQ','17807',	0.0059)
insert into #TargetsUnClean values ('OSPREYEQ','303475',	0.0099)
insert into #TargetsUnClean values ('OSPREYEQ','2358',		0.0205)
insert into #TargetsUnClean values ('OSPREYEQ','225484',	0.0062)
insert into #TargetsUnClean values ('OSPREYEQ','65047',	0.0105)
insert into #TargetsUnClean values ('OSPREYEQ','18132',	0.0107)
insert into #TargetsUnClean values ('OSPREYEQ','109452',	0.0101)
insert into #TargetsUnClean values ('OSPREYEQ','178024',	0.0102)
insert into #TargetsUnClean values ('OSPREYEQ','17275',	0.0088)
insert into #TargetsUnClean values ('OSPREYEQ','298695',	0.0034)
insert into #TargetsUnClean values ('OSPREYEQ','18165',	0.0048)
insert into #TargetsUnClean values ('OSPREYEQ','139270',	0.0191)
insert into #TargetsUnClean values ('OSPREYEQ','177819',	0.0011)
insert into #TargetsUnClean values ('OSPREYEQ','144801',	0.0095)
insert into #TargetsUnClean values ('OSPREYEQ','17514',	0.004)
insert into #TargetsUnClean values ('OSPREYEQ','17613',	0.0075)
insert into #TargetsUnClean values ('OSPREYEQ','158637',	0.0094)
insert into #TargetsUnClean values ('OSPREYEQ','326019',	0.0163)
insert into #TargetsUnClean values ('OSPREYEQ','333024',	0.0064)
insert into #TargetsUnClean values ('OSPREYEQ','2947',		0.0075)
insert into #TargetsUnClean values ('OSPREYEQ','44625',	0.0125)
insert into #TargetsUnClean values ('OSPREYEQ','96581',	0.0051)
insert into #TargetsUnClean values ('OSPREYEQ','2308',		0.0218)
insert into #TargetsUnClean values ('OSPREYEQ','18279',	0.0102)
insert into #TargetsUnClean values ('OSPREYEQ','17330',	0.0104)
insert into #TargetsUnClean values ('OSPREYEQ','17405',	0.0085)
insert into #TargetsUnClean values ('OSPREYEQ','17643',	0.0194)
insert into #TargetsUnClean values ('OSPREYEQ','17539',	0.011)
insert into #TargetsUnClean values ('OSPREYEQ','136740',	0.0207)
insert into #TargetsUnClean values ('OSPREYEQ','2282',		0.0123)
insert into #TargetsUnClean values ('OSPREYEQ','6964',		0.0477)
insert into #TargetsUnClean values ('OSPREYEQ','17387',	0.0095)
insert into #TargetsUnClean values ('OSPREYEQ','18086',	0.0083)
insert into #TargetsUnClean values ('OSPREYEQ','18054',	0.0013)
insert into #TargetsUnClean values ('OSPREYEQ','177530',	0.0028)
insert into #TargetsUnClean values ('OSPREYEQ','17622',	0.0504)
insert into #TargetsUnClean values ('OSPREYEQ','17605',	0.0242)
insert into #TargetsUnClean values ('OSPREYEQ','18192',	0.006)
insert into #TargetsUnClean values ('OSPREYEQ','83895',	0.0182)
insert into #TargetsUnClean values ('OSPREYEQ','2287',		0.0288)
insert into #TargetsUnClean values ('OSPREYEQ','17233',	0.0102)
insert into #TargetsUnClean values ('OSPREYEQ','156506',	0.0051)
insert into #TargetsUnClean values ('OSPREYEQ','17464',	0.0097)
insert into #TargetsUnClean values ('OSPREYEQ','42968',	0.0057)
insert into #TargetsUnClean values ('OSPREYEQ','18201',	0.0051)
insert into #TargetsUnClean values ('OSPREYEQ','2301',	0.0179)
insert into #TargetsUnClean values ('OSPREYEQ','171073',	0.0101)
insert into #TargetsUnClean values ('OSPREYEQ','17413',	0.017)
insert into #TargetsUnClean values ('OSPREYEQ','17672',	0.0083)
insert into #TargetsUnClean values ('OSPREYEQ','17340',	0.0097)
insert into #TargetsUnClean values ('OSPREYEQ','17979',	0.0173)
insert into #TargetsUnClean values ('OSPREYEQ','104524',	0.0184)
insert into #TargetsUnClean values ('OSPREYEQ','75690',	0.0129)
insert into #TargetsUnClean values ('OSPREYEQ','181698',	0.0081)
insert into #TargetsUnClean values ('OSPREYEQ','2329',	0.0099)
insert into #TargetsUnClean values ('OSPREYEQ','17345',	0.0115)
insert into #TargetsUnClean values ('OSPREYEQ','6693',	0.0159)
insert into #TargetsUnClean values ('OSPREYEQ','20616',	0.0171)
insert into #TargetsUnClean values ('OSPREYEQ','83789',	0.0074)
insert into #TargetsUnClean values ('OSPREYEQ','17678',	0.009 )




select * into #Targets from
	( 
	select PORTFOLIO_ID, SECURITY_ID, sum(TARGET_PCT) as TARGET_PCT 
	from #TargetsUnClean where TARGET_PCT > 0 group by PORTFOLIO_ID, SECURITY_ID) as u
	
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
--exec	[New_BU_Portfolio_SETUP_COPY_TARGETS_FROM_POSITION] 'STARS','OSPREYEQ','NAZIRALIA'
