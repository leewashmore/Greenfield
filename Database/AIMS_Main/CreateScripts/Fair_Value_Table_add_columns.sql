alter table dbo.fair_value
add PERIOD_TYPE char(2)

alter table dbo.fair_value
add PERIOD_YEAR int

go

update dbo.FAIR_VALUE
set PERIOD_TYPE= 'C' where 1=1

update dbo.FAIR_VALUE
set PERIOD_YEAR= 0 where 1=1

/*
alter table dbo.fair_value
ALTER COLUMN PERIOD_TYPE char(2) not null


alter table dbo.fair_value
ALTER COLUMN PERIOD_YEAR int NOT null
*/
