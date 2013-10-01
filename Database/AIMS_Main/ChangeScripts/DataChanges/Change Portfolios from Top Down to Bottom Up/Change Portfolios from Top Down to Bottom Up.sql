/****** Convert all Top Down portfolios to Bottom Up  ******/
UPDATE [AIMS_Main].[dbo].[PORTFOLIO] 
   SET IS_BOTTOM_UP = 1
   where ID in ('ABPEQ','ACTINVER','AGCFE','AREF','BIRCH','CONN','EMIF','EMSF','GRD7','IOWA','KODAK','OPB','PRIT','SICVEF','USEF')

/****** Revert to original Top Down settings ******/
/*
UPDATE [AIMS_Main].[dbo].[PORTFOLIO] 
   SET IS_BOTTOM_UP = 0
   where ID in ('ABPEQ','ACTINVER','AGCFE','AREF','BIRCH','CONN','EMIF','EMSF','GRD7','IOWA','KODAK','OPB','PRIT','SICVEF','USEF')
*/