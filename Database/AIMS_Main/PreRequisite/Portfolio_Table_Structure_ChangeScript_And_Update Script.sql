alter table dbo.portfolio add  proxy_portfolio varchar(20)
Go

update portfolio set proxy_portfolio = 'MUGCTM' where id = 'SICVESC'
update portfolio set proxy_portfolio = 'GSCF' where id = 'USESC'
Go