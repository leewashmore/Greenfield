
-- 1. Add PORTFOLIO
INSERT INTO [AIMS_Main].[dbo].[PORTFOLIO]
           ([ID]
           ,[NAME]
           ,[IS_BOTTOM_UP])
VALUES
           ('PASERSESC' 
           ,'PASERS SMALL CAP'
           ,1)

-- 2. Give permissions
INSERT INTO [AIMS_Main].[dbo].[USERNAME_FUND]
           ([USERNAME]
           ,[PORTFOLIO_ID])
VALUES
           ('MALLOYL'
           ,'PASERSESC')

INSERT INTO [AIMS_Main].[dbo].[USERNAME_FUND]
           ([USERNAME]
           ,[PORTFOLIO_ID])
VALUES
           ('MORROWF'
           ,'PASERSESC')
           
INSERT INTO [AIMS_Main].[dbo].[USERNAME_FUND]
           ([USERNAME]
           ,[PORTFOLIO_ID])
VALUES
           ('MACHATAJ'
           ,'PASERSESC')  