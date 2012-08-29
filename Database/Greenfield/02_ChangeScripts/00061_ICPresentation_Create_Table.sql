--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00060'
declare @CurrentScriptVersion as nvarchar(100) = '00061'
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
--Script for IC Presentation Create Table

IF OBJECT_ID ('dbo.PresentationInfo') IS NOT NULL
	DROP TABLE dbo.PresentationInfo
GO

CREATE TABLE dbo.PresentationInfo
	(
	PresentationID                 BIGINT IDENTITY NOT NULL,
	Presenter                      VARCHAR (50) NOT NULL,
	StatusType                     VARCHAR (50) NOT NULL,
	SecurityTicker                 VARCHAR (50) NULL,
	SecurityName                   VARCHAR (50) NULL,
	SecurityCountry                VARCHAR (50) NULL,
	SecurityCountryCode            VARCHAR (50) NULL,
	SecurityIndustry               VARCHAR (50) NULL,
	SecurityCashPosition           REAL NULL,
	SecurityPosition               BIGINT NULL,
	SecurityMSCIStdWeight          REAL NULL,
	SecurityMSCIIMIWeight          REAL NULL,
	SecurityGlobalActiveWeight     REAL NULL,
	SecurityLastClosingPrice       REAL NULL,
	SecurityMarketCapitalization   REAL NULL,
	SecurityPFVMeasure             VARCHAR (50) NULL,
	SecurityBuyRange               REAL NULL,
	SecuritySellRange              REAL NULL,
	SecurityRecommendation         VARCHAR (50) NULL,
	InvestmentThesis               NVARCHAR (max) NULL,
	Background                     NVARCHAR (max) NULL,
	Valuations                     NVARCHAR (max) NULL,
	EarningsOutlook                NVARCHAR (max) NULL,
	CompetitiveAdvantage           NVARCHAR (max) NULL,
	CompetitiveDisadvantage        NVARCHAR (max) NULL,
	CommitteePFVMeasure            VARCHAR (255) NULL,
	CommitteeBuyRange              REAL NULL,
	CommitteeSellRange             REAL NULL,
	CommitteeRecommendation        VARCHAR (50) NULL,
	CommitteeRangeEffectiveThrough DATE NULL,
	AcceptWithoutDiscussionFlag    BIT NULL,
	AdminNotes                     VARCHAR (255) NULL,
	CreatedBy                      VARCHAR (50) NOT NULL,
	CreatedOn                      DATETIME NOT NULL,
	ModifiedBy                     VARCHAR (50) NOT NULL,
	ModifiedOn                     DATETIME NOT NULL,
	Analyst                        VARCHAR (50) NULL,
	Price                          VARCHAR (50) NULL,
	FVCalc                         VARCHAR (50) NULL,
	SecurityBuySellvsCrnt          VARCHAR (50) NULL,
	CurrentHoldings                VARCHAR (50) NULL,
	PercentEMIF                    VARCHAR (50) NULL,
	SecurityBMWeight               VARCHAR (50) NULL,
	SecurityActiveWeight           VARCHAR (50) NULL,
	YTDRet_Absolute                VARCHAR (50) NULL,
	YTDRet_RELtoLOC                VARCHAR (50) NULL,
	YTDRet_RELtoEM                 VARCHAR (50) NULL,
	CONSTRAINT PK_tblICP_PresentationInfo PRIMARY KEY (PresentationID)
	)
GO




IF OBJECT_ID ('dbo.VoterInfo') IS NOT NULL
	DROP TABLE dbo.VoterInfo
GO

CREATE TABLE dbo.VoterInfo
	(
	VoterID             BIGINT IDENTITY NOT NULL,
	PresentationID      BIGINT NOT NULL,
	Name                VARCHAR (50) NOT NULL,
	Notes               VARCHAR (255) NULL,
	VoteType            VARCHAR (50) NULL,
	AttendanceType      VARCHAR (50) NULL,
	PostMeetingFlag     BIT NULL,
	DiscussionFlag      BIT NULL,
	VoterPFVMeasure     VARCHAR (50) NULL,
	VoterBuyRange       REAL NULL,
	VoterSellRange      REAL NULL,
	VoterRecommendation VARCHAR (50) NULL,
	CreatedBy           VARCHAR (50) NOT NULL,
	CreatedOn           DATETIME NOT NULL,
	ModifiedBy          VARCHAR (50) NOT NULL,
	ModifiedOn          DATETIME NOT NULL,
	CONSTRAINT PK_tblICP_VoterInfo PRIMARY KEY (VoterID),
	CONSTRAINT FK_VoterInfo_PresentationInfo FOREIGN KEY (PresentationID) REFERENCES dbo.PresentationInfo (PresentationID) ON DELETE CASCADE ON UPDATE CASCADE
	)
GO

IF OBJECT_ID ('dbo.FileMaster') IS NOT NULL
	DROP TABLE dbo.FileMaster
GO

CREATE TABLE dbo.FileMaster
	(
	FileID         BIGINT IDENTITY NOT NULL,
	Name           VARCHAR (255) NULL,
	SecurityName   VARCHAR (255) NULL,
	SecurityTicker VARCHAR (50) NULL,
	Location       VARCHAR (255) NULL,
	MetaTags       VARCHAR (255) NULL,
	Type           VARCHAR (50) NULL,
	CreatedBy      VARCHAR (50) NULL,
	CreatedOn      DATETIME NULL,
	ModifiedBy     VARCHAR (50) NULL,
	ModifiedOn     DATETIME NULL,
	CONSTRAINT PK_FileMaster PRIMARY KEY (FileID)
	)
GO

IF OBJECT_ID ('dbo.MeetingConfigurationSchedule') IS NOT NULL
	DROP TABLE dbo.MeetingConfigurationSchedule
GO

CREATE TABLE dbo.MeetingConfigurationSchedule
	(
	PresentationDateTime                 DATETIME NOT NULL,
	PresentationTimeZone                 VARCHAR (50) NOT NULL,
	PresentationDeadline                 DATETIME NOT NULL,
	PreMeetingVotingDeadline             DATETIME NOT NULL,
	CreatedBy                            VARCHAR (50) NOT NULL,
	CreatedOn                            DATETIME NOT NULL,
	ModifiedBy                           VARCHAR (50) NOT NULL,
	ModifiedOn                           DATETIME NOT NULL,
	ConfigurablePresentationDeadline     DECIMAL (18,2) NULL,
	ConfigurablePreMeetingVotingDeadline DECIMAL (18,2) NULL
	)
GO

IF OBJECT_ID ('dbo.MeetingInfo') IS NOT NULL
	DROP TABLE dbo.MeetingInfo
GO

CREATE TABLE dbo.MeetingInfo
	(
	MeetingID                   BIGINT IDENTITY NOT NULL,
	MeetingDateTime             DATETIME NOT NULL,
	MeetingClosedDateTime       DATETIME NOT NULL,
	MeetingVotingClosedDateTime DATETIME NOT NULL,
	MeetingDescription          VARCHAR (255) NULL,
	CreatedBy                   VARCHAR (50) NOT NULL,
	CreatedOn                   DATETIME NOT NULL,
	ModifiedBy                  VARCHAR (50) NOT NULL,
	ModifiedOn                  DATETIME NOT NULL,
	CONSTRAINT PK_tblICP_MeetingInfo PRIMARY KEY (MeetingID)
	)
GO

IF OBJECT_ID ('dbo.MeetingAttachedFileInfo') IS NOT NULL
	DROP TABLE dbo.MeetingAttachedFileInfo
GO

CREATE TABLE dbo.MeetingAttachedFileInfo
	(
	ID         BIGINT IDENTITY NOT NULL,
	FileID     BIGINT NOT NULL,
	MeetingID  BIGINT NOT NULL,
	CreatedBy  VARCHAR (50) NOT NULL,
	CreatedOn  DATETIME NOT NULL,
	ModifiedBy VARCHAR (50) NOT NULL,
	ModifiedOn DATETIME NOT NULL,
	CONSTRAINT PK_MeetingAttachedFileInfo_1 PRIMARY KEY (ID),
	CONSTRAINT FK_MeetingAttachedFileInfo_MeetingInfo FOREIGN KEY (MeetingID) REFERENCES dbo.MeetingInfo (MeetingID) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_MeetingAttachedFileInfo_FileMaster FOREIGN KEY (FileID) REFERENCES dbo.FileMaster (FileID) ON DELETE CASCADE ON UPDATE CASCADE
	)
GO

IF OBJECT_ID ('dbo.MeetingPresentationMappingInfo') IS NOT NULL
	DROP TABLE dbo.MeetingPresentationMappingInfo
GO

CREATE TABLE dbo.MeetingPresentationMappingInfo
	(
	MappingID      BIGINT IDENTITY NOT NULL,
	MeetingID      BIGINT NOT NULL,
	PresentationID BIGINT NOT NULL,
	CreatedBy      VARCHAR (50) NOT NULL,
	CreatedOn      DATETIME NOT NULL,
	ModifedBy      VARCHAR (50) NOT NULL,
	ModifiedOn     DATETIME NOT NULL,
	CONSTRAINT PK_tblICP_MeetingPresentationMappingInfo PRIMARY KEY (MappingID),
	CONSTRAINT FK_MeetingPresentationMappingInfo_MeetingInfo FOREIGN KEY (MeetingID) REFERENCES dbo.MeetingInfo (MeetingID) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_MeetingPresentationMappingInfo_PresentationInfo FOREIGN KEY (PresentationID) REFERENCES dbo.PresentationInfo (PresentationID) ON DELETE CASCADE ON UPDATE CASCADE
	)
GO


IF OBJECT_ID ('dbo.PresentationAttachedFileInfo') IS NOT NULL
	DROP TABLE dbo.PresentationAttachedFileInfo
GO

CREATE TABLE dbo.PresentationAttachedFileInfo
	(
	ID             BIGINT IDENTITY NOT NULL,
	FileID         BIGINT NOT NULL,
	PresentationID BIGINT NOT NULL,
	CreatedBy      VARCHAR (50) NOT NULL,
	CreatedOn      DATETIME NOT NULL,
	ModifiedBy     VARCHAR (50) NOT NULL,
	ModifiedOn     DATETIME NOT NULL,
	CONSTRAINT PK_PresentationAttachedFileInfo PRIMARY KEY (ID),
	CONSTRAINT FK_AttachedFileInfo_PresentationInfo FOREIGN KEY (PresentationID) REFERENCES dbo.PresentationInfo (PresentationID),
	CONSTRAINT FK_PresentationAttachedFileInfo_FileMaster FOREIGN KEY (FileID) REFERENCES dbo.FileMaster (FileID) ON DELETE CASCADE ON UPDATE CASCADE
	)
GO

IF OBJECT_ID ('dbo.CommentInfo') IS NOT NULL
	DROP TABLE dbo.CommentInfo
GO

CREATE TABLE dbo.CommentInfo
	(
	CommentID      BIGINT IDENTITY NOT NULL,
	PresentationID BIGINT NULL,
	FileID         BIGINT NULL,
	Comment        VARCHAR (255) NULL,
	CommentBy      VARCHAR (50) NULL,
	CommentOn      DATETIME NULL,
	CONSTRAINT PK_CommentInfo PRIMARY KEY (CommentID),
	CONSTRAINT FK_CommentInfo_PresentationInfo FOREIGN KEY (PresentationID) REFERENCES dbo.PresentationInfo (PresentationID) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_CommentInfo_FileMaster FOREIGN KEY (FileID) REFERENCES dbo.FileMaster (FileID) ON DELETE CASCADE ON UPDATE CASCADE
	)
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00061'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

