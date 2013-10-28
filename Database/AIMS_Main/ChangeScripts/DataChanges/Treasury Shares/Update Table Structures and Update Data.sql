/*Request ID : 20231  Net Treasury Shares as part of AIMS Valuations */

/* Update the Country_Master table with a flag telling us when we should remove Treasury Shares from Issuers Total Outstanding Shares */
ALTER TABLE [Country_Master]
	ADD remove_t_shares BIT DEFAULT 0 NOT NULL;

/* Update based on list from BB */
UPDATE [Country_Master]
	SET remove_t_shares = 1
	WHERE COUNTRY_CODE not in ('AR','BS','BD','BB','BM','CA','CN','GE','IN','IE','IL','JM','MY','PK','PA','PH','SG','LK','TH','TT','GB','VN')
	
/* Update the GF_Security_Baseview table with field to hold Treasury Shares from Issuers Total Outstanding Shares */
ALTER TABLE [GF_Security_Baseview]
	ADD remove_t_shares BIT DEFAULT 0 NOT NULL;