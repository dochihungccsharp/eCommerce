-- Create database with database name eCommerce
CREATE DATABASE eCommerce
GO

-- Use database
USE eCommerce
GO



CREATE TYPE PurchaseOrderDetailsTableType AS TABLE
(
   ProductId UNIQUEIDENTIFIER,
   Quantity INT,
   Price DECIMAL(18, 2)
);
GO

CREATE TABLE [dbo].[User] (
    [Id]                   UNIQUEIDENTIFIER   NOT NULL,
    [Username]             NVARCHAR (256)     NOT NULL,
    [Fullname]             NVARCHAR (512)     NULL,
    [Email]                NVARCHAR (256)     NOT NULL,
    [EmailConfirmed]       BIT                DEFAULT 0,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (50)      NULL,
    [Avatar]               NVARCHAR (MAX)     NULL,
    [Address]              NVARCHAR (MAX)     NULL,
    TotalAmountOwed        DECIMAL            DEFAULT 0,
    UserAddressId          UNIQUEIDENTIFIER   NULL,
    [Status]               BIT                DEFAULT 1,
    [Created]              DATETIME           NOT NULL,
    [Modified]             DATETIME           NULL,
    [IsDeleted]            BIT                DEFAULT 0,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[Role] (
  [Id]                   UNIQUEIDENTIFIER   NOT NULL,
  [Name]                 NVARCHAR (256)     NOT NULL,
  [Description]          NVARCHAR (MAX)         NULL,
  [Created]              DATETIME           NOT NULL,
  [Modified]             DATETIME           NULL,
  [IsDeleted]            BIT                DEFAULT 0,
  CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);


-- CREATE TABLE USER ROLE
CREATE TABLE [UserRole] (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED (UserId ASC, RoleId ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY (RoleId) REFERENCES [Role] (Id) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY (UserId) REFERENCES [User] (Id) ON DELETE CASCADE,
);



-- CREATE TABLE USER ADDRESS
CREATE TABLE UserAddress
(
    Id UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR,
    [UserId] UNIQUEIDENTIFIER,
    DeliveryAddress NVARCHAR(255) NOT NULL,
    Telephone VARCHAR(10),
    Active BIT,

    Created DATETIME NULL,
    Modified DATETIME NULL,
	IsDeleted BIT DEFAULT 0,

    CONSTRAINT [PK_UserAddress] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [FK_UserAddress_UserId] FOREIGN KEY([UserId]) REFERENCES [User](Id) ON DELETE CASCADE
)
GO

-- ADD FK USER
--ALTER TABLE [User]
--ADD CONSTRAINT FK_User_UserAddressId
--FOREIGN KEY (UserAddressId)
--REFERENCES UserAddress(Id);

-- CREATE TABLE USER PAYMENT
CREATE TABLE UserPayment
(
    Id UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER,
    PaymentType VARCHAR(100),
    [Provider] VARCHAR(100),
    AccountNo INT,
    Expiry DATETIME,

    CONSTRAINT [Pk_UserPayment] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [Fk_UserPayment_UserId] FOREIGN KEY([UserId]) REFERENCES [User](Id) ON DELETE CASCADE
)
GO

-- 1. CREATE TABLE BRAND
CREATE TABLE Brand
(
	Id UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	LogoURL NVARCHAR(MAX), 
	[Description] NVARCHAR(MAX),
	[Status] BIT NULL,

	Created DATETIME NULL,
    Modified DATETIME NULL,

	IsDeleted BIT DEFAULT 0,

	CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED ([Id] ASC ) 
) 
GO


-- 2. CREATE TABLE SUPPLIER
CREATE TABLE Supplier
(
	Id UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
	[Address] NVARCHAR(255),
	[Phone] VARCHAR(10),
	Email NVARCHAR(100),
	ContactPerson NVARCHAR(255) NULL,
	TotalAmountOwed DECIMAL(18, 2) DEFAULT 0,
	
    [Status] BIT NULL,
    Created DATETIME NULL,
    Modified DATETIME NULL,
    IsDeleted BIT DEFAULT 0,

	CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([Id] ASC ) 

) 
GO


-- 3. CREATE TABLE CATEGORY
CREATE TABLE Category (
    Id UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(150) NULL,
    [Description] NVARCHAR(255) NULL,
	ImageUrl NVARCHAR(MAX) NULL,

	[Status] BIT NULL,
	Created DATETIME NULL,
    Modified DATETIME NULL,
	IsDeleted BIT DEFAULT 0,

	ParentId UNIQUEIDENTIFIER NULL,

    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC ),

	CONSTRAINT Fk_Category_CategoryParentId FOREIGN KEY (ParentId) REFERENCES Category(Id)
)
GO


-- 4. CREATE TABLE INVENTORY
CREATE TABLE Inventory
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL,

    CONSTRAINT [PK_ProductInventory] PRIMARY KEY CLUSTERED ([Id] ASC ) 
)
GO



-- 5. CREATE PRODUCT
CREATE TABLE Product
(
    Id UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
	Slug NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
	ImageUrl NVARCHAR(MAX) NULL,
	OriginalPrice DECIMAL(18, 2) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
	QuantitySold INT DEFAULT 0,

	[Status] BIT DEFAULT 1,
	IsBestSelling BIT DEFAULT 0,
	IsNew BIT DEFAULT 0,

    CategoryId UNIQUEIDENTIFIER NULL,
	SupplierId UNIQUEIDENTIFIER NULL,
	BrandId UNIQUEIDENTIFIER NULL, 
    InventoryId UNIQUEIDENTIFIER NULL,

    Created DATETIME NULL,
    Modified DATETIME NULL,
    IsDeleted BIT DEFAULT 0,

    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC ),

    CONSTRAINT [Fk_Product_CategoryId] FOREIGN KEY (CategoryId) REFERENCES Category(Id),

    CONSTRAINT [Fk_Product_InventoryId] FOREIGN KEY (InventoryId) REFERENCES Inventory(Id),

	CONSTRAINT [Fk_Product_SupplierId] FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),

	CONSTRAINT [Fk_Product_BrandId] FOREIGN KEY (BrandId) REFERENCES Brand(Id)

)
GO



-- CREATE TABLE PURCHASE ORDER
CREATE TABLE PurchaseOrder
(
	Id UNIQUEIDENTIFIER NOT NULL,
	SupplierId UNIQUEIDENTIFIER NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	TotalMoney DECIMAL(18, 2) NOT NULL,
	Note NVARCHAR(MAX),
    PurchaseOrderStatus VARCHAR(20),
	PaymentStatus VARCHAR(20),
	TotalPaymentAmount DECIMAL(18, 2) DEFAULT 0,

	Created DATETIME NULL,
    Modified DATETIME NULL,
	IsDeleted BIT DEFAULT 0,

	CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED ([Id] ASC ),

	CONSTRAINT [Fk_PurchaseOrder_SupplierId] FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
	CONSTRAINT [Fk_PurchaseOrder_CreatorId] FOREIGN KEY (UserId) REFERENCES [User](Id)

)
GO

-- CREATE TABLE PURCHASE ORDER DETAIL
CREATE TABLE PurchaseOrderDetail
(
	PurchaseOrderId UNIQUEIDENTIFIER NOT NULL,
	ProductId UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	Price DECIMAL(18, 2) NOT NULL,

	CONSTRAINT [Fk_PurchaseOrderDetail_PurchaseInvoiceId] FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrder(Id) ON DELETE CASCADE,

	CONSTRAINT [Fk_PurchaseOrderDetail_ProductId] FOREIGN KEY (ProductId) REFERENCES Product(Id),

	CONSTRAINT [PK_PurchaseOrderDetail] PRIMARY KEY CLUSTERED (ProductId ASC, PurchaseOrderId ASC)
)
GO


-- CREATE TABLE SHOPPING SESSION
CREATE TABLE Shopping
(
    Id UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER,
    Total DECIMAL(18, 2),
    Created DATETIME NULL,
    Modified DATETIME NULL,

    CONSTRAINT [Pk_Shopping] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [Fk_Shopping_UserId] FOREIGN KEY([UserId]) REFERENCES [User](id)
)
GO


-- CREATE TABLE CART ITEM
CREATE TABLE CartItem
(
    Id UNIQUEIDENTIFIER NOT NULL,
    ShoppingId UNIQUEIDENTIFIER,
    ProductId UNIQUEIDENTIFIER,
    Quantity INT,
    Created DATETIME,
    Modified DATETIME,
	
    CONSTRAINT [Pk_CartItem] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [Fk_CartItem_ShoppingSessionId] FOREIGN KEY(ShoppingId) REFERENCES ShoppingSession(Id),

    CONSTRAINT [Fk_CartItem_ProductId] FOREIGN KEY(ProductId) REFERENCES Product(Id)
)
GO


-- CREATE TABLE ORDER DETAIL
CREATE TABLE OrderDetail
(
    Id UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER,
    ToTal DECIMAL(18, 2),
    PaymentId UNIQUEIDENTIFIER,
    Created DATETIME NULL,
    Modified DATETIME NULL,
	IsDeleted BIT,

    CONSTRAINT [Pk_OrderDetail] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [Fk_OrderDetail_UserId] FOREIGN KEY(UserId) REFERENCES [User](Id),
)
GO

-- CREATE TABLE PAYMENT DETAIL
CREATE TABLE PaymentDetail
(
    Id UNIQUEIDENTIFIER NOT NULL,
    OrderId UNIQUEIDENTIFIER,
    Amount INT,
    [Provider] varchar(100),
    [Status] varchar(100),
    Created DATETIME NULL,
    Modified DATETIME NULL,
    IsDeleted BIT,

    CONSTRAINT [Pk_PaymentDetail] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [Fk_PaymentDetail_OrderId] FOREIGN KEY(OrderId) REFERENCES OrderDetail(Id)
)
GO

ALTER TABLE OrderDetail ADD CONSTRAINT [Fk_OrderDetail_PaymentId] FOREIGN KEY(PaymentId) REFERENCES PaymentDetail(Id)
GO

-- CREATE TABLE ORDER DETAIL
CREATE TABLE OrderItem
(
    Id UNIQUEIDENTIFIER NOT NULL,
    OrderId UNIQUEIDENTIFIER,
    ProductId UNIQUEIDENTIFIER,
    Quantity INT,
    Created DATETIME NULL,
    Modified DATETIME NULL,
    IsDeleted BIT,

    CONSTRAINT [Pk_OrderItem] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [Fk_OrderItem_OrderId]
        FOREIGN KEY(OrderId) REFERENCES OrderDetail(Id),

    CONSTRAINT [Fk_OrderItem_ProductId]
        FOREIGN KEY(ProductId) REFERENCES Product(id)
)
GO

-- CREATE TABLE PROVINCE
CREATE TABLE Province(
	ProvinceId UNIQUEIDENTIFIER PRIMARY KEY,
	Name NVARCHAR(MAX),
	Status BIT DEFAULT 1
)
GO

-- CREATE TABLE DISTRICT
CREATE TABLE District(
	DistrictId UNIQUEIDENTIFIER PRIMARY KEY,
	ProvinceId UNIQUEIDENTIFIER,
	Name NVARCHAR(MAX),
	Status BIT DEFAULT 1
)
GO
-- CREATE TABLE WARD
CREATE TABLE Ward(
	WardId UNIQUEIDENTIFIER PRIMARY KEY,
	DistrictId UNIQUEIDENTIFIER,
	Name NVARCHAR(MAX),
	Status BIT DEFAULT 1
)
GO


-- + + + + + FUNCTION + + + + + --
CREATE FUNCTION fn_GetStringByToken
(
	@String VARCHAR(MAX),
	@Delimiter CHAR(1),
    @Position INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN 
	-- Khai báo biến StartIndex INT, @EndIndex INT
	DECLARE @StartIndex INT = 1, @EndIndex INT

	-- Tìm vị trí bắt đầu
	DECLARE @Index INT = 1;
	IF(@Position = 1) SET @StartIndex = 0
	ELSE
		BEGIN
			WHILE (@Index < @Position)
				BEGIN
					SET @StartIndex = CHARINDEX(@Delimiter, @String, @StartIndex) + 1;
					-- Nếu từ  @StartIndex mà không tìm thấy @Delimiter trong @String thì retrun
					IF(CHARINDEX(@Delimiter, @String, @StartIndex) = 0) RETURN '';
					SET @Index = @Index + 1;
				END
		END
	-- Tìm vị trí kết thúc
	SET @EndIndex = CHARINDEX(@Delimiter, @String, @StartIndex);

	IF(@EndIndex <> 0)
		RETURN SUBSTRING(@String, @StartIndex , @EndIndex - @StartIndex)
	RETURN ''
END
GO


CREATE FUNCTION fn_GetStringByTokenUseStringSplit
(
	@String VARCHAR(MAX),
	@Delimiter CHAR(1),
    @Position INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN 
	DECLARE @Result VARCHAR(MAX)
	IF (@Position <= (SELECT COUNT(*) FROM STRING_SPLIT(@String, @Delimiter)))
		BEGIN
			SELECT @Result = value
			FROM STRING_SPLIT(@String, @Delimiter)
			ORDER BY (SELECT NULL)
			OFFSET @Position - 1 ROWS FETCH NEXT 1 ROW ONLY;
			RETURN TRIM(@Result);
		END
	RETURN '';
END
GO


-- + + + + + INIT DATA TABLE + + + + + --
INSERT INTO Brand (Id, Name, LogoURL, Description, Status, CreatedTime, CreatorId, ModifiedTime, ModifierId, IsDeleted)
SELECT TOP 100 NEWID(), 
       CONCAT('Brand ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       CONCAT('http://example.com/logo/', NEWID(), '.jpg'), 
       CONCAT('Description for Brand ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       1,
       GETDATE(),
       NEWID(),
       NULL,
       NULL,
       0
FROM sys.columns c1
CROSS JOIN sys.columns c2
OPTION (MAXDOP 1);
GO

INSERT INTO Supplier (Id, Name, Description, Address, Phone, Email, ContactPerson, Status, CreatedTime, CreatorId, ModifiedTime, ModifierId, IsDeleted)
SELECT TOP 100 NEWID(), 
       CONCAT('Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       CONCAT('Description for Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       CONCAT('Address for Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       '0976580418',
       CONCAT('supplier', ROW_NUMBER() OVER(ORDER BY (SELECT NULL)), '@example.com'), 
       CONCAT('Contact Person for Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       1,
       GETDATE(),
       NEWID(),
       NULL,
       NULL,
       0
FROM sys.columns c1
CROSS JOIN sys.columns c2
OPTION (MAXDOP 1);
GO

DECLARE @counter INT = 1;
WHILE @counter <= 100
	BEGIN
		INSERT INTO Category (Id, [Name], [Description], ImageUrl, [Status], CreatedTime, CreatorId, ModifiedTime, ModifierId, IsDeleted, CategoryParentId)
		VALUES
		(NEWID(), 'Category ' + CAST(@counter AS NVARCHAR(10)), 'This is category ' + CAST(@counter AS NVARCHAR(10)), 'https://example.com/category' + CAST(@counter AS NVARCHAR(10)) + '.jpg', @counter%2, GETDATE(), NULL, NULL, NULL, 0, NULL)
		SET @counter += 1
	END