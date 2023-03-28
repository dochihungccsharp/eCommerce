USE eCommerce
GO

--=========================== START STORED PROC PROVINCE ==============================
CREATE PROC sp_Provinces
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@ProvinceId					    UNIQUEIDENTIFIER	=		NULL,
@DistrictId					    UNIQUEIDENTIFIER	=		NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'GET_ALL_P'
BEGIN
	SELECT * FROM Province	
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_D'
BEGIN
	SELECT * FROM District
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_D_BY_P_ID'
BEGIN
	SELECT * FROM District WHERE ProvinceId = @ProvinceId
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_W'
BEGIN
	SELECT * FROM Ward
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_W_BY_D_ID'
BEGIN
	SELECT * FROM Ward WHERE DistrictId = @DistrictId
END

GO
--=========================== END STORED PROC PROVINCE ==============================

--=========================== START STORED PROC USER ==============================
CREATE TYPE RolesTableType AS TABLE
(
  [Id]                   UNIQUEIDENTIFIER   NOT NULL,
  [Name]                 NVARCHAR (256)     NOT NULL
);
GO

CREATE PROC sp_Users
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@Id                             UNIQUEIDENTIFIER    =       NULL,
@Username                       NVARCHAR(256)       =       NULL,
@Fullname                       NVARCHAR(512)       =       NULL,
@Email                          NVARCHAR(256)       =       NULL,
@EmailConfirmed                 BIT                 =       NULL,
@PasswordHash                   NVARCHAR (MAX)      =       NULL,
@PhoneNumber                    NVARCHAR (50)       =       NULL,
@Avatar                         NVARCHAR (MAX)      =       NULL,
@Address                        NVARCHAR (MAX)      =       NULL,
@TotalAmountOwed                DECIMAL             =       NULL,
@UserAddressId                  UNIQUEIDENTIFIER    =       NULL,
@Status                         BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted                      BIT                 =       NULL,
-----------------------------------------------------------------
@Roles                          RolesTableType     READONLY ,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)      =        NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS 
-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		BEGIN

			-- ADD USER
			INSERT INTO [User] 
			([Id], [UserName], [Fullname], [Email], [EmailConfirmed], [PasswordHash], [PhoneNumber], [Avatar], [Address], [TotalAmountOwed],[UserAddressId], [Status], [Created], [IsDeleted])
			VALUES
			(@Id, @Username, @Fullname, @Email, @EmailConfirmed, @PasswordHash, @PhoneNumber, @Avatar, @Address, 0, NULL, 1, GETDATE(), 0)


			-- ADD USER ROLE
			DECLARE @RoleId UNIQUEIDENTIFIER;
			DECLARE @Index INT = 1;

			DECLARE @RowCount INT = (SELECT COUNT(*) FROM @Roles);
			IF @RowCount > 0
					WHILE @Index <= @RowCount
						BEGIN
							SELECT @RoleId = Id
							FROM @Roles
							ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
							INSERT INTO [UserRole] (UserId, RoleId)
							VALUES (@Id, @RoleId)
							SET @Index = @Index + 1
						END
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH

END

---------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		BEGIN
			-- UPDATE USER	
			UPDATE [User] SET
				[Username] = ISNULL(@Username, [Username]),
				[Fullname] = ISNULL(@Fullname, [Fullname]),
				[Email] = ISNULL(@Email, [Email]),
				[EmailConfirmed] = ISNULL(@EmailConfirmed, [EmailConfirmed]),
				[PasswordHash] = ISNULL(@PasswordHash, [PasswordHash]),
				[PhoneNumber] = ISNULL(@PhoneNumber, [PhoneNumber]),
				[Avatar] = ISNULL(@Avatar, [Avatar]),
				[Address] = ISNULL(@Address, [Address]),
				[TotalAmountOwed] = ISNULL(@TotalAmountOwed, [TotalAmountOwed]),
				[UserAddressId] = ISNULL(@UserAddressId, [UserAddressId]),
				[Status] = ISNULL(@Status, [Status]),
				[Modified] = GETDATE()
			WHERE Id = @Id

	        -- DELETE ALL USER ROLE
			DELETE FROM [UserRole] WHERE UserId = @Id;

			DECLARE @RoleId_ UNIQUEIDENTIFIER;
			DECLARE @Index_ INT = 1;

			DECLARE @RowCount_ INT = (SELECT COUNT(*) FROM @Roles);
			IF @RowCount_ > 0
				BEGIN
					-- ADD ROLE ROLE
					WHILE @Index_ <= @RowCount_
						BEGIN
							SELECT @RoleId_ = Id
							FROM @Roles
							ORDER BY (SELECT NULL) OFFSET @Index_ - 1 ROWS FETCH NEXT 1 ROWS ONLY;

							INSERT INTO [UserRole] (UserId, RoleId)
							VALUES (@Id, @RoleId_)

							SET @Index_ += 1
						END
				END
			COMMIT TRANSACTION
		END
	END TRY
	BEGIN CATCH
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END

---------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE [User] SET [IsDeleted] = 1 WHERE [Id] = @Id
END

---------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM [User] (NOLOCK)
	WHERE  (([Username] = @Username) 
	OR  ([Email] = @Email) 
	OR  ([PhoneNumber] = @PhoneNumber)) 
	AND [IsDeleted] = 0
END

---------------------------------------------------------------
ELSE IF @Activity = 'FIND_BY_EMAIL'
BEGIN
	SELECT * FROM [User] WHERE [Email] = @Email AND [IsDeleted] = 0
END

---------------------------------------------------------------
ELSE IF @Activity = 'FIND_BY_ID'
BEGIN
	SELECT * FROM [User] WHERE Id = @Id AND [IsDeleted] = 0
END

---------------------------------------------------------------
ELSE IF @Activity = 'FIND_BY_NAME'
BEGIN
	SELECT * FROM [User] WHERE [Username] = @Username AND [IsDeleted] = 0
END

---------------------------------------------------------------
ELSE IF @Activity = 'GET_PROFILE_BY_ID'
BEGIN
	SELECT u.Id, u.Username, u.Fullname, u.Email, u.[EmailConfirmed], u.PhoneNumber, u.Avatar, u.[Address], u.TotalAmountOwed, u.UserAddressId, u.[Status], u.Created, u.Modified,
	(SELECT r.Id, r.[Name], r.[Description] FROM [Role]  AS r 
	INNER JOIN [UserRole] AS ur ON ur.RoleId = r.Id
	WHERE ur.UserId = u.Id
	FOR JSON PATH
	) AS _Roles,
	(SELECT * FROM UserAddress AS ua WHERE ua.Id = u.UserAddressId FOR JSON PATH) AS _UserAddresses
	FROM [User] AS u
	WHERE u.Id = @Id AND u.IsDeleted = 0
END

---------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH UserTemp AS(
		SELECT * 
		FROM [User](NOLOCK) u
		WHERE (@SearchString IS NULL OR u.[Fullname] LIKE N'%'+@SearchString+'%' 
		OR u.[Username] LIKE N'%'+@SearchString+'%' 
		OR  u.[Email] LIKE N'%'+@SearchString+'%'
		OR  u.[PhoneNumber] LIKE N'%'+@SearchString+'%') AND [IsDeleted] = 0
	)

	SELECT u.Id, u.Username, u.Fullname, u.Email, u.[EmailConfirmed] ,u.PhoneNumber, u.Avatar, u.[Address], u.TotalAmountOwed, u.UserAddressId, u.[Status], u.Created, u.Modified
	FROM UserTemp AS ut
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM UserTemp
	) as RecordCount
	INNER JOIN [User] (NOLOCK) u ON u.Id = ut.Id
	ORDER BY u.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY

END

GO
--=========================== END STORED PROC USER ==============================

--=========================== START STORED PROC ROLE ==============================
CREATE PROC sp_Roles
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(256)		=		NULL,
@Description					NVARCHAR(MAX)		=		NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted                      BIT                 =       0   ,

@UserId						    UNIQUEIDENTIFIER	=		NULL
-----------------------------------------------------------------
AS

-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO [Role] ([Id], [Name], [Description], [Created], [Modified], [IsDeleted])
	VALUES (@Id, @Name, @Description, GETDATE(), NULL, 0)
	SELECT CAST(SCOPE_IDENTITY() AS INT)
END

-----------------------------------------------------------------
IF @Activity = 'UPDATE'
BEGIN
	UPDATE [Role] SET
		[Name] = ISNULL(@Name, [Name]),
		[Description] = ISNULL(@Description, [Description])
		WHERE Id = @Id
END

-----------------------------------------------------------------
IF @Activity = 'DELETE'
BEGIN
	DELETE FROM [Role] WHERE [Id] = @Id
END

-----------------------------------------------------------------
IF @Activity = 'FIND_ROLE_BY_ID'
BEGIN
	SELECT TOP 1 * FROM [dbo].[Role]
	WHERE [Id] = @Id
END

-----------------------------------------------------------------
IF @Activity = 'FIND_ROLE_BY_NAME'
BEGIN
	SELECT TOP 1 * FROM [dbo].[Role]
	WHERE [Name] = @Name
END

-----------------------------------------------------------------
IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM [Role] (NOLOCK)
	WHERE  (@Name IS NULL OR [Name] = @Name) 
	AND (@Id IS NULL OR Id <> @Id)
END

-----------------------------------------------------------------
IF @Activity = 'GET_ALL'
BEGIN
	SELECT * FROM [dbo].[Role]
END

-----------------------------------------------------------------
IF @Activity = 'GET_ROLES_BY_USER_ID'
BEGIN

	SELECT r.[Id] ,r.[Name], r.[Description], r.Created FROM [Role] r 
	INNER JOIN [UserRole] ur ON ur.[RoleId] = r.Id 
	WHERE ur.UserId = @UserId
END
GO
--=========================== END STORED PROC ROLE ==============================

--=========================== START STORED PROC BRAND ==============================
CREATE PROC [dbo].[sp_Brands]
@Activity						NVARCHAR(50)		=		NULL,
@SearchString					NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		1,
@PageSize						INT					=		10,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@LogoURL                        NVARCHAR(MAX)       =       NULL,
@Description					NVARCHAR(255)		=		NULL,
@Status                         BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO Brand(Id, [Name], [Description], LogoURL, [Status], Created, IsDeleted) 
	VALUES (@Id, @Name, @Description, @LogoURL, 1, GETDATE(), 0)
END

-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	UPDATE Brand
	SET 
		[Name] = ISNULL(@Name, [Name]),
		[Description] = ISNULL(@Description, [Description]),
		LogoURL = ISNULL(@LogoURL, LogoURL),
		[Status] = ISNULL(@Status, [Status]),
		Modified = GETDATE()
	WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE Brand SET IsDeleted = 1 WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM Brand (NOLOCK)
	WHERE [Name] = @Name AND (@Id IS NULL OR Id <> @Id) AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS'
BEGIN
	UPDATE Brand SET [Status] = ~[Status] WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT Id, [Name], [Description], LogoURL, [Status]
	FROM Brand AS b (NOLOCK)
	WHERE b.Id = @Id AND b.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT TOP(1) b.Id, b.[Name], b.LogoURL, b.[Description], b.[Status], b.Created, b.Modified
	FROM Brand AS b WHERE b.Id = @Id AND b.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH BrandsTemp AS (
		SELECT b.Id
		FROM Brand (NOLOCK) b
		WHERE (@SearchString IS NULL OR @SearchString = '' OR b.[Name] LIKE N'%'+@SearchString+'%' OR  b.[Description] LIKE N'%'+@SearchString+'%') AND b.IsDeleted = 0
	)
	SELECT b.Id, b.[Name], b.[Description], b.LogoURL, b.[Status], RecordCount.TotalRows as TotalRows
	FROM BrandsTemp AS bt 
		CROSS JOIN 
		(
			SELECT COUNT(*) AS TotalRows
			FROM BrandsTemp
		) as RecordCount
		INNER JOIN Brand (NOLOCK) b ON b.Id = bt.Id
	ORDER BY b.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

--=========================== END STORED PROC BRAND ==============================

--=========================== START STORED PROC SUPPLIER ==============================
CREATE PROC [dbo].[sp_Suppliers]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@Description					NVARCHAR(255)		=		NULL,
@Address                        NVARCHAR(255)       =       NULL,
@Phone                          VARCHAR(10)         =       NULL,
@Email                          NVARCHAR(100)       =       NULL,
@ContactPerson                  NVARCHAR(255)       =       NULL,
@Status                         BIT                 =       NULL,
@CreatedTime                    DATETIME            =       NULL,
@CreatorId                      UNIQUEIDENTIFIER    =       NULL,
@ModifiedTime                   DATETIME            =       NULL,
@ModifierId                     UNIQUEIDENTIFIER    =       NULL,
@IsDeleted						BIT                 =       0
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO Supplier(Id, [Name], [Description], [Address], Phone, Email, ContactPerson, [Status], TotalAmountOwed, Created, IsDeleted) 
	VALUES (@Id, @Name, @Description, @Address, @Phone, @Email, @ContactPerson, 1, 0, GETDATE(), 0)
END

-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	UPDATE Supplier
	SET 
		[Name] = ISNULL(@Name, [Name]),
		[Description] = ISNULL(@Description, [Description]),
		[Address] = ISNULL(@Address, [Address]),
		Phone = ISNULL(@Phone, Phone),
		Email = ISNULL(@Email, Email),
		ContactPerson = ISNULL(@ContactPerson, ContactPerson),
		[Status] = ISNULL(@Status, [Status]),
		Modified = GETDATE()
	WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS'
BEGIN
	UPDATE Supplier
	SET [Status] = ~[Status] WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE Supplier SET IsDeleted = 1 WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM Supplier (NOLOCK)
	WHERE ([Name] = @Name OR Phone = @Phone OR Email = @Email) AND (@Id IS NULL OR Id <> @Id) AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT Id, [Name], [Description], [Address], [Phone], Email, ContactPerson, [Status], TotalAmountOwed
	FROM Supplier AS s (NOLOCK)
	WHERE s.Id = @Id AND s.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT Id, [Name], [Description], [Address], [Phone], Email, ContactPerson, [Status], TotalAmountOwed, Created, Modified
	FROM Supplier AS s (NOLOCK)
	WHERE s.Id = @Id AND s.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH SupplierTemp AS (
		SELECT s.Id
		FROM Supplier (NOLOCK) s
		WHERE (@SearchString IS NULL 
		OR @SearchString = '' 
		OR s.[Name] LIKE N'%'+@SearchString+'%' 
		OR  s.[Description] LIKE N'%'+@SearchString+'%'
		OR  s.[Phone] LIKE N'%'+@SearchString+'%'
		OR  s.Email LIKE N'%'+@SearchString+'%' 
		OR  s.ContactPerson LIKE N'%'+@SearchString+'%') 
		AND s.IsDeleted = 0
	)
	SELECT s.Id, s.[Name], s.[Description], s.[Address], s.Phone, s.Email, s.ContactPerson, s.[Status], s.TotalAmountOwed, RecordCount.TotalRows as TotalRows
	FROM SupplierTemp AS st
		CROSS JOIN 
		(
			SELECT COUNT(*) AS TotalRows
			FROM SupplierTemp
		) as RecordCount
		INNER JOIN Supplier (NOLOCK) s ON s.Id = st.Id
	ORDER BY s.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
--=========================== END STORED PROC SUPPLIER ==============================

--=========================== START STORED PROC CATEGORY ==============================
CREATE PROC [dbo].[sp_Categories]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@Description					NVARCHAR(255)		=		NULL,
@ImageUrl                       NVARCHAR(MAX)       =       NULL,
@Status                         BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
@ParentId                       UNIQUEIDENTIFIER    =       NULL,
@ListId							VARCHAR(MAX)        =       NULL
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO Category (Id, [Name], [Description], ImageUrl, [Status], Created, IsDeleted, ParentId) 
	VALUES (@Id, @Name, @Description, @ImageUrl, 1, GETDATE(), 0, @ParentId)
END

-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	UPDATE Category
	SET 
		[Name] = ISNULL(@Name, [Name]),
		[Description] = ISNULL(@Description, [Description]),
		ImageUrl = ISNULL(@ImageUrl, ImageUrl),
		[Status] = ISNULL(@Status, [Status]),
		Modified = GETDATE(),
		ParentId = ISNULL(@ParentId, ParentId)
	WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE Category SET IsDeleted = 1 WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS'
BEGIN
	UPDATE Category SET [Status] = ~[Status] WHERE Id = @Id AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM Category (NOLOCK)
	WHERE [Name] = @Name AND (@Id IS NULL OR Id <> @Id) AND IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT Id, [Name], [Description], ImageUrl, [Status]
	FROM Category AS Cate (NOLOCK)
	WHERE Cate.Id = @Id AND Cate.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.Created, c.Modified, c.ParentId, 
	(SELECT JSON_QUERY((SELECT TOP(1) Id, [Name], [Description], ImageUrl, [Status] FROM Category AS ct WHERE ct.Id = c.ParentId FOR JSON PATH), '$[0]')) AS _Category
	FROM Category AS c (NOLOCK)
	WHERE c.Id = @Id AND c.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH CategoriesTemp AS (
		SELECT cate.Id
		FROM Category (NOLOCK) cate
		WHERE (@SearchString IS NULL OR @SearchString = '' OR cate.[Name] LIKE N'%'+@SearchString+'%' OR  cate.[Description] LIKE N'%'+@SearchString+'%') AND Cate.IsDeleted = 0
	)
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, C.[Status], RecordCount.TotalRows as TotalRows
	FROM CategoriesTemp AS ct 
		CROSS JOIN 
		(
			SELECT COUNT(*) AS TotalRows
			FROM CategoriesTemp
		) as RecordCount
		INNER JOIN Category (NOLOCK) c ON c.Id = ct.Id
	ORDER BY c.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
--=========================== END STORED PROC CATEGORY ==============================

--=========================== START STORED PROC PRODUCT ==============================

CREATE PROC sp_Products
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		1,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
@FromPrice                      DECIMAL             =       NULL,
@ToPrice                        DECIMAL             =       NULL,
@FromTime                       DATETIME            =       NULL,
@ToTime                         DATETIME            =       NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@Slug							NVARCHAR(150)		=		NULL,
@Description					NVARCHAR(255)		=		NULL,
@ImageUrl                       NVARCHAR(MAX)       =       NULL,
@OriginalPrice                  DECIMAL             =       NULL,
@Price                          DECIMAL             =       NULL,
@QuantitySold                   INT                 =       0,

@CategoryId                     UNIQUEIDENTIFIER    =       NULL,
@SupplierId                     UNIQUEIDENTIFIER    =       NULL,
@BrandId                        UNIQUEIDENTIFIER    =       NULL,
@InventoryId                    UNIQUEIDENTIFIER    =       NULL,

@Status                         BIT                 =       NULL,
@IsBestSelling                  BIT                 =       NULL,
@IsNew                          BIT                 =       NULL,

@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted                      BIT                 =       0   ,

@ListId							VARCHAR(MAX)        =       NULL,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)      =        NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS

-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
	INSERT INTO Product (Id, [Name], Slug, [Description], ImageUrl, OriginalPrice, Price, QuantitySold, CategoryId, SupplierId, BrandId, [Status], IsBestSelling, IsNew, Created)
	VALUES (@Id, @Name, @Slug, @Description, @ImageUrl, @OriginalPrice, @Price, 0, @CategoryId, @SupplierId, @BrandId, 1, 0, 0, GETDATE())
END

-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	UPDATE Product
	SET [Name] = ISNULL(@Name, [Name]),
		Slug = ISNULL(@Slug, Slug),
		[Description] = ISNULL(@Description, [Description]),
		ImageUrl = ISNULL(@ImageUrl, ImageUrl),
		OriginalPrice = ISNULL(@OriginalPrice, OriginalPrice),
		Price = ISNULL(@Price, Price),
		CategoryId  = ISNULL(@CategoryId, CategoryId),
		SupplierId = ISNULL(@SupplierId, SupplierId),
		BrandId = ISNULL(@BrandId, BrandId),
		[Status] = ISNULL(@Status, [Status]),
		IsBestSelling = ISNULL(@IsBestSelling, IsBestSelling),
		IsNew = ISNULL(@IsNew, IsBestSelling),
		Modified = GETDATE()
	WHERE Id = @Id  AND @IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE Product SET IsDeleted = 1 WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE_LIST'
BEGIN
	BEGIN TRANSACTION;
	DECLARE @CurrentPosition INT
	SET @CurrentPosition = 1

	WHILE (dbo.fn_GetStringByTokenUseStringSplit(@ListId, ',', @CurrentPosition) <> '')
	BEGIN
		SET @Id = CONVERT(UNIQUEIDENTIFIER, dbo.fn_GetStringByTokenUseStringSplit(@ListId, ',', @CurrentPosition))
		IF EXISTS (SELECT * FROM Product WHERE Id = @Id)
			BEGIN
				EXEC sp_Products @Activity = N'DELETE', -- NVARCHAR(50)
							@Id = @Id -- UNIQUEIDENTIFIER
				SET @CurrentPosition += 1;
			END
		ELSE
			BEGIN
			SELECT 
				@ErrorMessage = ERROR_MESSAGE(),
				@ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState = ERROR_STATE();
				RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);

				ROLLBACK TRANSACTION
			END
	END 
	COMMIT TRANSACTION
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM Product (NOLOCK)
	WHERE [Name] = @Name AND (@Id IS NULL OR Id <> @Id) AND @IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS_IS_BESTSELLING'
BEGIN
	UPDATE Product SET IsBestSelling = ~IsBestSelling WHERE Id = @Id AND @IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS_IS_NEW'
BEGIN
	UPDATE Product SET IsNew = ~IsNew WHERE Id = @Id AND @IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHANGE_STATUS'
BEGIN
	UPDATE Product SET [Status] = ~[Status] WHERE Id = @Id AND @IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice, p.Price, i.Quantity,p.QuantitySold, p.[Status], p.IsBestSelling, p.IsNew
	FROM Product AS p (NOLOCK)
	LEFT JOIN Inventory (NOLOCK) i ON i.Id = p.InventoryId
	WHERE p.Id = @Id AND P.IsDeleted = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice,  p.Price, p.QuantitySold, p.[Status], p.IsBestSelling, p.IsNew,
	p.CategoryId, p.InventoryId, p.BrandId, p.SupplierId, p.Created, p.Modified,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Category AS c WHERE c.Id = p.CategoryId FOR JSON PATH), '$[0]')) AS _Category,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Brand AS b WHERE b.Id = p.BrandId FOR JSON PATH), '$[0]')) AS _Brand,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Supplier AS s WHERE s.Id = p.SupplierId FOR JSON PATH), '$[0]')) AS _Supplier,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Inventory AS i WHERE i.Id = p.InventoryId FOR JSON PATH), '$[0]')) AS _Inventory
	FROM Product AS p (NOLOCK)
	WHERE p.Id = @Id AND P.IsDeleted = 0

END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH ProductTemp AS (
		SELECT p.Id
		FROM Product (NOLOCK) p
		WHERE (@SearchString IS NULL OR p.[Name] LIKE N'%'+@SearchString+'%' OR  p.[Description] LIKE N'%'+@SearchString+'%') 
		AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
		AND (@BrandId IS NULL OR p.BrandId = @BrandId)
		AND ((@FromPrice IS NULL OR @ToPrice IS NULL) OR (p.Price >= @FromPrice AND p.Price <= @ToPrice))
		AND ((@FromTime IS NULL OR @ToTime IS NULL) OR (p.Created >= @FromTime AND p.Created <= @ToTime))
		AND (@IsBestSelling IS NULL OR p.IsBestSelling = @IsBestSelling)
		AND (@IsNew IS NULL OR p.IsNew = @IsNew)
		AND p.IsDeleted = 0
	)
	SELECT p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice, p.Price, i.Quantity,p.QuantitySold, p.[Status], p.IsBestSelling, p.IsNew,
	RecordCount.TotalRows as TotalRows
	FROM ProductTemp AS pt 
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM ProductTemp
	) as RecordCount
	INNER JOIN Product (NOLOCK) p ON p.Id = pt.Id
	LEFT JOIN Inventory (NOLOCK) i ON i.Id = p.InventoryId
	ORDER BY p.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
--=========================== END STORED PROC PRODUCT ==============================

--=========================== START STORED PROC PURCHASE ORDER ==============================
CREATE TYPE PurchaseOrderDetailsTableType AS TABLE
(
   ProductId UNIQUEIDENTIFIER,
   Quantity INT,
   Price DECIMAL(18, 2)
);
GO



CREATE PROC [dbo].[sp_PurchaseOrders]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		0,
@PageSize						INT					=		10,
@SearchString					NVARCHAR(MAX)		=		NULL,
@FromPrice                      DECIMAL             =       NULL,
@ToPrice                        DECIMAL             =       NULL,
@FromTime                       DATETIME            =       NULL,
@ToTime                         DATETIME            =       NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId                         UNIQUEIDENTIFIER    =       NULL,
@SupplierId				        UNIQUEIDENTIFIER	=		NULL,
@TotalMoney						DECIMAL     		=		NULL,
@Note       					NVARCHAR(MAX)		=		NULL,
@OrderStatus                    NVARCHAR(20)        =       NULL,
@PaymentStatus                  NVARCHAR(20)        =       NULL,
@TotalPaymentAmount             DECIMAL(18, 2)      =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
-----------------------------------------------------------------
@PurchaseOrderDetails           PurchaseOrderDetailsTableType READONLY,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =       NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		BEGIN
			-- CREATE PURCHASE ORDER
			INSERT INTO PurchaseOrder (Id, SupplierId, UserId, TotalMoney, Note, OrderStatus, PaymentStatus, TotalPaymentAmount, Created)
			VALUES (@Id, @SupplierId, @UserId, @TotalMoney, @Note, @OrderStatus, @PaymentStatus, @TotalPaymentAmount, GETDATE())

			DECLARE @RowCount INT = (SELECT COUNT(*) FROM @PurchaseOrderDetails);
			IF @RowCount > 0
				BEGIN
				PRINT @RowCount;
					DECLARE @ProductId UNIQUEIDENTIFIER;
					DECLARE @Quantity INT;
					DECLARE @Price DECIMAL(18,2);
					DECLARE @Index INT = 1;

					-- DRAFT_INVOICE
					IF @OrderStatus = 'DRAFT_INVOICE'
						BEGIN
							WHILE @Index <= @RowCount
								BEGIN
									SELECT @ProductId = ProductId, @Quantity = Quantity, @Price = Price
									FROM @PurchaseOrderDetails
									ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

									BEGIN
										SELECT 
											@ErrorMessage = 'Product in purchase order does not exist', -- Sản phẩm trong purchase order không tồn tại
											@ErrorSeverity = ERROR_SEVERITY(),
											@ErrorState = ERROR_STATE();
										RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
									END

									-- CREATE PURCHASE ORDER DETAIL
									INSERT INTO PurchaseOrderDetail (PurchaseOrderId, ProductId, Quantity, Price) 
									VALUES (@Id, @ProductId, @Quantity, @Price)

									SET @Index = @Index + 1;
								END
						END
					-- PURCHASE_INVOICE
					ELSE IF @OrderStatus = 'PURCHASE_INVOICE'
						BEGIN
							WHILE @Index <= @RowCount
							BEGIN
								SELECT @ProductId = ProductId, @Quantity = Quantity, @Price = Price
								FROM @PurchaseOrderDetails
								ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

								-- CHECK THE PRODUCT EXISTENCE
								IF NOT EXISTS (SELECT * FROM Product WHERE Id = @ProductId)
									BEGIN
										SELECT 
											@ErrorMessage = 'Product in purchase order does not exist', -- Sản phẩm trong purchase order không tồn tại
											@ErrorSeverity = ERROR_SEVERITY(),
											@ErrorState = ERROR_STATE();
										RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
									END

								-- CREATE PURCHASE ORDER DETAIL
								INSERT INTO PurchaseOrderDetail (PurchaseOrderId, ProductId, Quantity, Price) 
								VALUES (@Id, @ProductId, @Quantity, @Price)

								-- UPDATE INVENTORY WITH PRODUCT ID
								DECLARE @InventoryId UNIQUEIDENTIFIER;
								SELECT @InventoryId = p.InventoryId FROM Product AS p WHERE p.Id = @ProductId
								IF(@InventoryId IS NULL)
									BEGIN
										-- CREATE INVENTORY IF INVENTORY NULL
										SET @InventoryId = NEWID()
										INSERT INTO Inventory (Id, Quantity)
										VALUES (@InventoryId, @Quantity)

										-- SET InventoryId, OriginalPrice FOR PRODUCT
										UPDATE Product SET InventoryId = @InventoryId, OriginalPrice = @Price
										WHERE Id = @ProductId
									END
								ELSE
									BEGIN
										DECLARE @CurrentQuantity INT;
										DECLARE @CurrentOriginalPrice DECIMAL(18, 2);
								
										-- GET CurrentQuantity, CurrentOriginalPrice
										SELECT 
											@CurrentQuantity = COALESCE(i.Quantity, 0), 
											@CurrentOriginalPrice = p.OriginalPrice
										FROM Product AS p
										LEFT JOIN Inventory i ON i.Id = p.InventoryId
										WHERE p.Id = @ProductId

										-- UPDATE OriginalPrice FOR Product
										UPDATE Product 
										SET OriginalPrice = (@CurrentQuantity * @CurrentOriginalPrice + @Quantity * @Price)/(@CurrentQuantity + @Quantity) 
										WHERE Id = @ProductId

										-- UPDATE Quantity Inventory
										UPDATE Inventory 
										SET Quantity = Quantity + @Quantity
										WHERE Id = @InventoryId;
									END
								SET @Index = @Index + 1;
							END
						END
					ELSE
						BEGIN
							SELECT 
							@ErrorMessage = 'Order status invalid',
							@ErrorSeverity = ERROR_SEVERITY(),
							@ErrorState = ERROR_STATE();
							RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
						END
				END
			ELSE 
				BEGIN
					SELECT 
					@ErrorMessage = 'Your order has no products?',
					@ErrorSeverity = ERROR_SEVERITY(),
					@ErrorState = ERROR_STATE();
					RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
				END
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT 
			@ErrorMessage =  ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		ROLLBACK TRANSACTION
	END CATCH
END
---------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
	DELETE FROM PurchaseOrder WHERE Id = @Id;
			
	-- CREATE Purchase Order
	DECLARE @NewId UNIQUEIDENTIFIER;
	EXEC [dbo].[sp_PurchaseOrders] 
		@Activity               = 'INSERT',
		@Id                     = @Id,
		@UserId                 = @UserId,
		@SupplierId				= @SupplierId,
		@TotalMoney				= @TotalMoney,
		@Note       			= @Note,
		@OrderStatus            = @OrderStatus,
		@PaymentStatus          = @PaymentStatus,
		@TotalPaymentAmount     = @TotalPaymentAmount,
		@Created                = @Created,
		@Modified               = @Modified,
		@IsDeleted				= @IsDeleted,
		@PurchaseOrderDetails   =  @PurchaseOrderDetails;
END
-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	DELETE FROM PurchaseOrder WHERE Id = @Id;
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	SELECT  po.Id, po.TotalMoney, po.Note, po.OrderStatus, po.PaymentStatus, po.Created, s.[Name] as SupplierName, u.Fullname as UserName
	FROM PurchaseOrder AS po
	LEFT JOIN Supplier (NOLOCK) s ON s.Id = po.SupplierId
	LEFT JOIN [User] (NOLOCK) u ON u.Id = po.UserId
	WHERE @Id = po.Id AND po.IsDeleted = 0
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_DETAILS_BY_ID'
BEGIN
	SELECT po.Id, po.SupplierId, po.TotalMoney, po.Note, po.OrderStatus, po.PaymentStatus, po.Created, po.UserId, po.Modified,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = po.UserId FOR JSON PATH), '$[0]')) AS _User,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Supplier AS s WHERE s.Id = po.SupplierId FOR JSON PATH), '$[0]')) AS _Supplier,
	
	(
		SELECT pod.PurchaseOrderId, pod.ProductId, pod.Price, pod.Quantity, p.[Name] AS ProductName
		FROM PurchaseOrderDetail AS pod 
		LEFT JOIN Product AS p ON p.Id = pod.ProductId
		WHERE pod.PurchaseOrderId = po.Id FOR JSON PATH
	) AS _PurchaseOrderDetails

	FROM PurchaseOrder AS po (NOLOCK)
	WHERE po.Id = @Id AND po.IsDeleted = 0
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH PurchaseOrderTemp AS
	(
		SELECT po.Id
		FROM PurchaseOrder (NOLOCK) po
		LEFT JOIN [User] AS u ON u.Id = po.UserId
		LEFT JOIN [Supplier] AS s ON s.Id = po.SupplierId
 		WHERE (@SearchString IS NULL OR po.Note LIKE N'%'+@SearchString+'%' OR  s.[Name] LIKE N'%'+@SearchString+'%' OR  u.[Fullname] LIKE N'%'+@SearchString+'%')
		AND (@SupplierId IS NULL OR po.SupplierId = @SupplierId)
		AND (@UserId IS NULL OR po.UserId = @UserId)
		AND (@OrderStatus IS NULL OR po.OrderStatus = @OrderStatus)
		AND (@PaymentStatus IS NULL OR po.PaymentStatus = @PaymentStatus)
		AND ((@FromPrice IS NULL OR @ToPrice IS NULL) OR (po.TotalMoney >= @FromPrice AND po.TotalMoney <= @ToPrice))
		AND ((@FromTime IS NULL OR @ToTime IS NULL) OR (po.Created >= @FromTime AND po.Created <= @ToTime))
		AND po.IsDeleted = 0
	)

	SELECT po.Id, po.SupplierId, po.UserId, po.TotalMoney, po.Note, po.OrderStatus, po.PaymentStatus, po.Created, s.[Name] as SupplierName, u.Fullname as CreatorName,
	RecordCount.TotalRows as TotalRows
	FROM PurchaseOrderTemp AS pot
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM PurchaseOrderTemp
	) as RecordCount
	LEFT JOIN PurchaseOrder (NOLOCK) po ON po.Id = pot.Id
	LEFT JOIN Supplier (NOLOCK) s ON s.Id = po.SupplierId
	LEFT JOIN [User] (NOLOCK) u ON u.Id = po.UserId
	ORDER BY po.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
--=========================== END STORED PROC PURCHASE ORDER ==============================