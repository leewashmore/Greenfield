alter table dbo.portfolio add  proxy_portfolio varchar(20)
Go

update portfolio set proxy_portfolio = 'CURIANESC' where id = 'PASERSESC'
update portfolio set proxy_portfolio = 'CURIANESC' where id = 'SICVESC'
update portfolio set proxy_portfolio = 'CURIANESC' where id = 'USESC'

Go