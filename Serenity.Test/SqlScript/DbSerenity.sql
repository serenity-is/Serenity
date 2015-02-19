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