alter table dbo.portfolio add  proxy_portfolio varchar(20)
Go

update portfolio set proxy_portfolio = 'CURIANESC' where id = 'PASERSESC'
update portfolio set proxy_portfolio = 'CURIANESC' where id = 'SICVESC'
update portfolio set proxy_portfolio = 'CURIANESC' where id = 'USESC'

Go


delete from dbo.username_fund where portfolio_id in (
select id from portfolio where proxy_portfolio is not null)

Go