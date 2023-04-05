-- Create database with database name eCommerce
CREATE DATABASE eCommerce
GO

-- Use database
USE eCommerce
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
    [Name] NVARCHAR(255),
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
    OrderStatus VARCHAR(20),
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

	CONSTRAINT [Fk_PurchaseOrderDetail_PurchaseOrderId] FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrder(Id) ON DELETE CASCADE,

	CONSTRAINT [Fk_PurchaseOrderDetail_ProductId] FOREIGN KEY (ProductId) REFERENCES Product(Id),

	CONSTRAINT [PK_PurchaseOrderDetail] PRIMARY KEY CLUSTERED (ProductId ASC, PurchaseOrderId ASC)
)
GO


-- CREATE TABLE CART ITEM
CREATE TABLE CartItem
(
	Id UNIQUEIDENTIFIER,
    UserId UNIQUEIDENTIFIER,
    ProductId UNIQUEIDENTIFIER,
    Quantity INT,
    Created DATETIME,
    Modified DATETIME,
	
    CONSTRAINT [Pk_CartItem] PRIMARY KEY CLUSTERED (UserId ASC, ProductId ASC),
	CONSTRAINT [Fk_CartItem_User] FOREIGN KEY(UserId) REFERENCES [User](Id),
    CONSTRAINT [Fk_CartItem_ProductId] FOREIGN KEY(ProductId) REFERENCES Product(Id)
)
GO

-- CREATE TABLE CATEGORY DISCOUNT
CREATE TABLE CategoryDiscount (
	Id UNIQUEIDENTIFIER,
	UserId UNIQUEIDENTIFIER NOT NULL,
	CategoryId UNIQUEIDENTIFIER NOT NULL,
	Code VARCHAR(50) NOT NULL,
	DiscountType VARCHAR(10) NOT NULL,
	DiscountValue DECIMAL(10,2) NOT NULL,
	Created DATETIME NOT NULL,
	Modified DATETIME NULL,
	IsActive BIT NOT NULL DEFAULT 0,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,

	CONSTRAINT [Pk_CategoryDiscount] PRIMARY KEY CLUSTERED (Id ASC),

	CONSTRAINT [Fk_User_UserId] FOREIGN KEY(UserId) REFERENCES [User](Id),

	CONSTRAINT [Fk_Category_CategoryId] FOREIGN KEY(CategoryId) REFERENCES Category(Id)
);

-- CREATE TABLE CATEGORY DISCOUNT Exclusion
CREATE TABLE CategoryProductExclusion (
    Id UNIQUEIDENTIFIER,
    CategoryDiscountId UNIQUEIDENTIFIER NOT NULL,
	CategoryId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_CategoryProductExclusion] PRIMARY KEY CLUSTERED (Id ASC),

    CONSTRAINT [FK_CategoryProductExclusion_CategoryDiscountId] FOREIGN KEY (CategoryDiscountId) REFERENCES CategoryDiscount(Id),

    CONSTRAINT [FK_CategoryProductExclusion_ProductId] FOREIGN KEY (ProductId) REFERENCES Product(Id)
);

-- CREATE TABLE PROMOTION
CREATE TABLE Promotion (
	Id UNIQUEIDENTIFIER,
	UserId UNIQUEIDENTIFIER NOT NULL,
	Code VARCHAR(50) NOT NULL,
	DiscountType VARCHAR(10) NOT NULL,
	DiscountValue DECIMAL(10,2) NOT NULL,
	MinimumOrderAmount DECIMAL(10,2) NOT NULL,
	Created DATETIME NOT NULL,
	Modified DATETIME NULL,
	IsActive BIT NOT NULL DEFAULT 0,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,

	CONSTRAINT [Pk_Promotion] PRIMARY KEY CLUSTERED (Id ASC),

	CONSTRAINT [Fk_Promotion_UserId] FOREIGN KEY(UserId) REFERENCES [User](Id)
);



-- CREATE TABLE ORDER
CREATE TABLE [Order]
(
    Id UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    PaymentId UNIQUEIDENTIFIER NULL,
    PromotionId UNIQUEIDENTIFIER NULL,
	PaymentStatus NVARCHAR(20), -- PAID, UNPAID
    PaymentMethod NVARCHAR(20), -- COD, VNPAY
	OrderStatus NVARCHAR(20),  -- Pending, Processing, Shipped, Delivered, Cancelled
    ToTal DECIMAL(18, 2),
	Note NVARCHAR(MAX) NULL,
    Created DATETIME NULL,
    Modified DATETIME NULL,
	IsCancelled BIT DEFAULT 0, -- đã hủy
	IsDeleted BIT,

    CONSTRAINT [Pk_Order] PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT [Fk_Order_UserId] FOREIGN KEY(UserId) REFERENCES [User](Id),
    CONSTRAINT [Fk_Order_PromotionId] FOREIGN KEY(PromotionId) REFERENCES [Promotion](Id)
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

    CONSTRAINT [Fk_PaymentDetail_OrderId] FOREIGN KEY(OrderId) REFERENCES [Order](Id)
)
GO


ALTER TABLE [Order] ADD CONSTRAINT [Fk_OrderDetail_PaymentId] FOREIGN KEY(PaymentId) REFERENCES PaymentDetail(Id)
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
        FOREIGN KEY(OrderId) REFERENCES [Order](Id),

    CONSTRAINT [Fk_Product_ProductId]
        FOREIGN KEY(ProductId) REFERENCES Product(id)
)
GO

-- CREATE TABLE PROVINCE
CREATE TABLE Province(
	ProvinceId UNIQUEIDENTIFIER PRIMARY KEY,
	[Name] NVARCHAR(MAX),
	[Status] BIT DEFAULT 1
)
GO

-- CREATE TABLE DISTRICT
CREATE TABLE District(
	DistrictId UNIQUEIDENTIFIER PRIMARY KEY,
	ProvinceId UNIQUEIDENTIFIER,
	[Name] NVARCHAR(MAX),
	[Status] BIT DEFAULT 1
)
GO
-- CREATE TABLE WARD
CREATE TABLE Ward(
	WardId UNIQUEIDENTIFIER PRIMARY KEY,
	DistrictId UNIQUEIDENTIFIER,
	[Name] NVARCHAR(MAX),
	[Status] BIT DEFAULT 1
)
GO

