USE eCommerce
GO

-- + + + + + INIT DATA TABLE BRAND + + + + + --
INSERT INTO Brand (Id, Name, LogoURL, Description, Status, Created, Modified, IsDeleted)
SELECT TOP 10 NEWID(), 
       CONCAT('Brand ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       CONCAT('http://example.com/logo/', NEWID(), '.jpg'), 
       CONCAT('Description for Brand ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       1,
       GETDATE(),
       NULL,
       0
FROM sys.columns c1
CROSS JOIN sys.columns c2
OPTION (MAXDOP 1);
GO

INSERT INTO Supplier (Id, Name, Description, Address, Phone, Email, ContactPerson, Status, Created, Modified, IsDeleted)
SELECT TOP 10 NEWID(), 
       CONCAT('Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       CONCAT('Description for Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       CONCAT('Address for Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       '0976580418',
       CONCAT('supplier', ROW_NUMBER() OVER(ORDER BY (SELECT NULL)), '@example.com'), 
       CONCAT('Contact Person for Supplier ', ROW_NUMBER() OVER(ORDER BY (SELECT NULL))), 
       1,
       GETDATE(),
       NULL,
       0
FROM sys.columns c1
CROSS JOIN sys.columns c2
OPTION (MAXDOP 1);
GO

DECLARE @counter INT = 1;
WHILE @counter <= 10
	BEGIN
		INSERT INTO Category (Id, [Name], [Description], ImageUrl, [Status], Created, Modified, IsDeleted, ParentId)
		VALUES
		(NEWID(), 'Category ' + CAST(@counter AS NVARCHAR(10)), 'This is category ' + CAST(@counter AS NVARCHAR(10)), 'https://example.com/category' + CAST(@counter AS NVARCHAR(10)) + '.jpg', @counter%2, GETDATE(), NULL, 0, NULL)
		SET @counter += 1
	END
GO
--++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
-- CREATE USER
DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserAddressId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [User] 
(Id, Username, Fullname, Email, EmailConfirmed, PasswordHash, PhoneNumber, Avatar, [Address], [TotalAmountOwed], UserAddressId, [Status], Created, IsDeleted)
VALUES 
(@UserId, 'dohung.csharp@gmail.com', N'Đỗ Chung', 'dohung.csharp@gmail.com', 1, '00a666c06d72c25ab9d74d1fa687d2b8', '0999999999', NULL, N'Đông Kết, Khoái Châu, Hưng Yên', 999999999, @UserAddressId, 1, GETDATE(), 0);

-- CREATE USER ADDRESS
INSERT INTO UserAddress
(Id, [Name], [UserId], [DeliveryAddress], Telephone, Active, Created, IsDeleted )
VALUES
(@UserAddressId, N'Đ', @UserId, N'Quan Hoa, Cầu Giấy, Hà Nội', '0989700110', 1, GETDATE(), 0);

-- CREATE ROLE
DECLARE @RoleId UNIQUEIDENTIFIER = NEWID();
INSERT INTO [Role]
(Id, [Name], [Description], Created, IsDeleted)
VALUES
(@RoleId, 'Admin', 'Admin', GETDATE(), 0);

-- ADD ROLE USER
INSERT INTO [UserRole] (UserId, RoleId)
VALUES (@UserId, @RoleId);
