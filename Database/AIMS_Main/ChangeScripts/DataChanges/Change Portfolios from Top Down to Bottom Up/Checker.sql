/****** Script for SelectTopNRows command from SSMS  ******/
SELECT [ID]
      ,[NAME]
      ,[IS_BOTTOM_UP]
      ,[proxy_portfolio]
  FROM [AIMS_Main].[dbo].[PORTFOLIO] where IS_BOTTOM_UP = 0

/****** Script for SelectTopNRows command from SSMS  ******/
SELECT [ID]
      ,[USERNAME]
      ,[TIMESTAMP]
      ,[CALCULATION_ID]
  FROM [AIMS_Main].[dbo].[BU_PORTFOLIO_SECURITY_TARGET_CHANGESET] where USERNAME = 'MIGRATE'
  
declare  @ChangeSetID int --unique to Portfolio
select @ChangesetID = min( ID )from [AIMS_Main].[dbo].[BU_PORTFOLIO_SECURITY_TARGET_CHANGESET] where USERNAME = 'MIGRATE'

  /****** Script for SelectTopNRows command from SSMS  ******/
SELECT [ID]
      ,[STATUS_CODE]
      ,[QUEUED_ON]
      ,[STARTED_ON]
      ,[FINISHED_ON]
      ,[LOG]
  FROM [AIMS_Main].[dbo].[TARGETING_CALCULATION] where FINISHED_ON > dateadd(minute, -2, getdate()) 
  
/****** Script for SelectTopNRows command from SSMS  ******/
  SELECT *
  FROM [AIMS_Main].[dbo].[BU_PORTFOLIO_SECURITY_TARGET_CHANGE] where CHANGESET_ID >= @ChangesetID