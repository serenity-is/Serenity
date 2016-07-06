SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

CREATE TABLE DisplayOrders (
    ID int IDENTITY(1, 1) NOT NULL,
    GroupID int NULL,
    DisplayOrder int NOT NULL,
    IsActive smallint NOT NULL,
    CONSTRAINT [PK_DisplayOrders] PRIMARY KEY CLUSTERED (ID ASC) 
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
        ON [PRIMARY]
) ON [PRIMARY];

CREATE TABLE SystemLogs (
    ID int IDENTITY(1, 1) NOT NULL,
    EventDate datetime NOT NULL,
    LogLevel nvarchar(10) NULL,
    LogMessage nvarchar(MAX) NULL,
    Exception nvarchar(MAX) NULL,
    SourceType nvarchar(200) NULL
);

CREATE TABLE Int32Master (
    ID int IDENTITY(1, 1) NOT NULL,
    Name nvarchar(100) NOT NULL
);

CREATE TABLE Int32Detail (
    DetailID int IDENTITY(1, 1) NOT NULL,
    MasterID int NOT NULL,
    ProductID int NOT NULL,
    Quantity numeric(18, 2) NOT NULL
);

CREATE TABLE GuidMaster (
    ID uniqueidentifier NOT NULL default((newid())),
    Name nvarchar(100) NOT NULL
);

CREATE TABLE GuidDetail (
    DetailID uniqueidentifier NOT NULL default((newid())),
    MasterID uniqueidentifier NOT NULL,
    ProductID int NOT NULL,
    Quantity numeric(18, 2) NOT NULL
);