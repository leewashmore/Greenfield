set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00059'
declare @CurrentScriptVersion as nvarchar(100) = '00060'

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

CREATE TABLE [dbo].[UserListDataPointMappingInfo](
	[DataPointId] [bigint] IDENTITY(1,1) NOT NULL,
	[ListId] [bigint] NOT NULL,
	[ScreeningId] [varchar](50) NOT NULL,
	[DataDescription] [varchar](100) NOT NULL,
	[DataSource] [varchar](50) NULL,
	[PeriodType] [char](2) NULL,
	[YearType] [char](8) NULL,
	[FromDate] [int] NULL,
	[ToDate] [int] NULL,
	[DataPointsOrder] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_tblUserListDataPointMappingInfo] PRIMARY KEY CLUSTERED 
(
	[DataPointId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[UserListDataPointMappingInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserListDataPointMappingInfo_UserCustomisedListInfo] FOREIGN KEY([ListId])
REFERENCES [dbo].[UserCustomisedListInfo] ([ListId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserListDataPointMappingInfo] CHECK CONSTRAINT [FK_UserListDataPointMappingInfo_UserCustomisedListInfo]
GO
--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00060'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

