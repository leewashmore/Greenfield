--these 4 updates per JM 
--AIMS - Company Snap Shot Issue List - Change ID : 2632
update dbo.FINANCIAL_GRIDS_DISPLAY
set DECIMALS = 3 where DATA_ID = 191
	  
update dbo.FINANCIAL_GRIDS_DISPLAY
set PERCENTAGE = 'N' where DATA_ID = 150
	
update dbo.FINANCIAL_GRIDS_DISPLAY
set PERCENTAGE = 'N' where DATA_ID = 151

update dbo.FINANCIAL_GRIDS_DISPLAY
set PERCENTAGE = 'N' where DATA_ID = 152



--add a field for the Multiplier functionality
--AIMS - Company Snap Shot Issue List - Change ID : 2632
ALTER TABLE FINANCIAL_GRIDS_DISPLAY
ADD MULTIPLIER dec(32,6) NOT NULL DEFAULT(1.000000)
GO


--Update the multiplier field
--AIMS - Company Snap Shot Issue List - Change ID : 2632
UPDATE FINANCIAL_GRIDS_DISPLAY 
SET    FINANCIAL_GRIDS_DISPLAY.MULTIPLIER = ISNULL((SELECT DISTINCT MULTIPLIER
                            FROM   FINSTAT_DISPLAY 
                            WHERE  FINSTAT_DISPLAY.DATA_ID = FINANCIAL_GRIDS_DISPLAY.DATA_ID),1)  