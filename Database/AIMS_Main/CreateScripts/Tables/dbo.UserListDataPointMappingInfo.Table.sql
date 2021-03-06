/****** Object:  Table [dbo].[UserListDataPointMappingInfo]    Script Date: 03/08/2013 11:10:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserListDataPointMappingInfo](
	[DataPointId] [bigint] IDENTITY(1,1) NOT NULL,
	[ListId] [bigint] NOT NULL,
	[ScreeningId] [varchar](50) NOT NULL,
	[DataDescription] [nvarchar](max) NOT NULL,
	[DataSource] [varchar](50) NULL,
	[PeriodType] [varchar](10) NULL,
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
