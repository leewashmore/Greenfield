/*Request ID : 20231  Net Treasury Shares as part of AIMS Valuations */

/* Update the Country_Master table with a flag telling us when we should remove Treasury Shares from Issuers Total Outstanding Shares */
ALTER TABLE [Country_Master]
	ADD remove_t_shares BIT DEFAULT 0 NOT NULL;


/* Update based on list from BB */
EXEC('
UPDATE [Country_Master]
	SET remove_t_shares = 1
	WHERE COUNTRY_CODE not in 
	(''AR'',''BS'',''BD'',''BB'',''BM'',''CA'',''CN'',''GE'',''IN''
	,''IE'',''IL'',''JM'',''MT'',''MY'',''PK'',''PA'',''PH'',''SG''
	,''LK'',''TH'',''TT'',''GB'',''VN'')
');

	
/* Back Out Changes */
declare @Command  nvarchar(1000)
select @Command = 'ALTER TABLE ' + 'Country_Master' + ' drop constraint ' + d.name
 from sys.tables t   
  join    sys.default_constraints d       
   on d.parent_object_id = t.object_id  
  join    sys.columns c      
   on c.object_id = t.object_id      
    and c.column_id = d.parent_column_id
 where t.name = 'Country_Master'
  and c.name = 'remove_t_shares'
  print @Command
execute (@Command)
ALTER TABLE [dbo].[Country_Master] DROP COLUMN remove_t_shares

