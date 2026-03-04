create table Orders (
    OrderId varchar(128) not null primary key,
    Product varchar(50) not null,
    Amount decimal(16, 2) not null,
    Count int,
    Customer varchar(128),
    Date date not null,
    Status varchar(50)
);

create table InventoryItems (
    InventoryId varchar(128) not null primary key,
    ProductName varchar(50) not null,
    Count int
)
