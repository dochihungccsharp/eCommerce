USE eCommerce
GO

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

--=========================================================================================
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
--=========================================================================================

CREATE PROC sp_StatisticsECommerce
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Quantity				        INT        		    =		10,
-----------------------------------------------------------------
@FirstDayOfMonth				DATETIME    		=		NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'GET_MONTHLY_STATISTICS'
BEGIN
	DECLARE @TotalRevenue DECIMAL(18, 2) = 0;
	DECLARE @TotalOrders INT = 0;
	DECLARE @TotalUsers INT = 0;
	DECLARE @CanceledOrderTotal INT = 0;

	SET @FirstDayOfMonth = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

	SELECT @TotalRevenue = SUM(Total) FROM [Order] WHERE Created >= @FirstDayOfMonth AND IsCancelled = 0;
	SELECT @TotalOrders = COUNT(1) FROM [Order] WHERE Created >= @FirstDayOfMonth AND IsCancelled = 0;
	SELECT @TotalUsers = COUNT(1) FROM [User] WHERE Created >= @FirstDayOfMonth;
	SELECT @CanceledOrderTotal = SUM(1) FROM [Order] WHERE Created >= @FirstDayOfMonth AND IsCancelled = 1;

	SELECT @TotalRevenue AS TotalRevenue, 
		@TotalOrders AS TotalOrders,
		@TotalUsers AS TotalUsers, 
		@CanceledOrderTotal AS CanceledOrderTotal;

END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_TOP_CATEGORIES_OF_CURRENT_MONTHLY'
BEGIN
	SET @FirstDayOfMonth = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

	SELECT TOP (@Quantity)
	    c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.Created,
	    SUM(oi.Quantity) AS QuantitySold
	FROM OrderItem AS oi
	LEFT JOIN Product AS p ON oi.ProductId = p.Id
	LEFT JOIN Category AS c ON p.CategoryId = c.Id
	WHERE oi.Created >= @FirstDayOfMonth
	GROUP BY c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.Created
	ORDER BY
		QuantitySold DESC;
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_TOP_USERS_OF_CURRENT_MONTHLY'
BEGIN

	SET @FirstDayOfMonth = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

	SELECT TOP(@Quantity) 
	u.Id, u.Username, u.Fullname, u.Email, u.[EmailConfirmed] ,u.PhoneNumber, u.Avatar, u.[Address], u.UserAddressId, u.[Status], u.Created, u.Modified,
	SUM(o.ToTal) AS TotalAmountOwed
	FROM [Order] AS o
	LEFT JOIN [User] AS u ON u.Id = o.UserId
	WHERE o.Created >= @FirstDayOfMonth AND o.IsCancelled = 0
	GROUP BY u.Id, u.Username, u.Fullname, u.Email, u.[EmailConfirmed] ,u.PhoneNumber, u.Avatar, u.[Address], u.UserAddressId, u.[Status], u.Created, u.Modified
	ORDER BY TotalAmountOwed DESC;

END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_TOP_PRODUCTS_OF_CURRENT_MONTHLY'
BEGIN
	SET @FirstDayOfMonth = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

	SELECT TOP (@Quantity)
	    p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice, p.Price, p.[Status], p.IsBestSelling, p.IsNew,
		p.CategoryId, p.InventoryId, p.BrandId, p.SupplierId,
	    SUM(oi.Quantity) AS QuantitySold
	FROM OrderItem AS oi
	LEFT JOIN Product AS p ON oi.ProductId = p.Id
	WHERE oi.Created >= @FirstDayOfMonth
	GROUP BY
		p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice, p.Price, p.[Status], p.IsBestSelling, p.IsNew,
		p.CategoryId, p.InventoryId, p.BrandId, p.SupplierId
	ORDER BY
		QuantitySold DESC;
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_TOP_ORDERS_OF_CURRENT_MONTHLY'
BEGIN
	SET @FirstDayOfMonth = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

	SELECT TOP (@Quantity) o.Id, o.UserId, o.PaymentId, o.PromotionId, o.OrderStatus, o.Total, o.Note, o.Created, o.Modified, o.IsCancelled,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = o.UserId FOR JSON PATH), '$[0]')) AS _User
	FROM [Order] AS o
	WHERE o.Created >= @FirstDayOfMonth AND o.IsCancelled = 0
	ORDER BY Total DESC;
END
GO
--=========================================================================================
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
--=========================================================================================
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
	SELECT b.Id, b.[Name], b.[Description], b.LogoURL, b.[Status], b.Created, RecordCount.TotalRows as TotalRows
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
--=========================================================================================
CREATE TYPE CartItemsTableType AS TABLE
(
	Id UNIQUEIDENTIFIER
);
GO



CREATE PROC sp_Carts
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id					    		UNIQUEIDENTIFIER	=		NULL,
@UserId							UNIQUEIDENTIFIER	=		NULL,
@ProductId						UNIQUEIDENTIFIER	=		NULL,
@Quantity                       INT                 =       NULL,
-----------------------------------------------------------------
@CartItems                      CartItemsTableType      READONLY,
-----------------------------------------------------------------
@RowCount                       INT                 =       NULL,
@Index                          INT                 =       NULL,
@Price                          DECIMAL             =       NULL,
@Total                          DECIMAL             =       NULL


AS
-----------------------------------------------------------------
IF @Activity = 'ADD_OR_UPDATE_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		-- CHECK THE PRODUCT EXISTENCE
		IF NOT EXISTS (SELECT TOP 1 1 FROM Product WHERE Id = @ProductId)
			THROW 400000, 'Product in cart does not exist', 1


		-- ADD OR UPDATE CART ITEM
		IF EXISTS (SELECT TOP 1 1 FROM CartItem WHERE UserId = @UserId AND ProductId = @ProductId)
			BEGIN
				UPDATE CartItem SET Quantity = @Quantity, Modified = GETDATE() WHERE UserId = @UserId AND ProductId = @ProductId
			END
		ELSE
			BEGIN
				INSERT INTO CartItem (Id, UserId, ProductId, Quantity, Created)
				VALUES (NEWID(), @UserId, @ProductId, @Quantity, GETDATE())
			END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		;THROW 400000, 'Add or update cart fail', 1;
		ROLLBACK TRANSACTION
	END CATCH
END
--------------------------------------------
ELSE IF @Activity = 'REMOVE_CART_ITEM'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE Id = @Id OR (UserId = @UserId AND ProductId = @ProductId))
			BEGIN
				;THROW 400000, 'Product in cart does not exist', 1;
			END

		-- Remove Cart Item
		DELETE CartItem WHERE Id = @Id OR (UserId = @UserId AND ProductId = @ProductId)
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		;THROW 400000, 'Remove cart item fail', 1;
		ROLLBACK TRANSACTION
	END CATCH
END

--------------------------------------------
ELSE IF @Activity = 'REMOVE_CART_ITEMS'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		SET @RowCount = (SELECT COUNT(1) FROM @CartItems);
		SET @Index = 1;
		WHILE @Index <= @RowCount
			BEGIN
				
				SELECT @Id = Id
				FROM @CartItems
				ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

				EXEC sp_Carts @Activity = 'REMOVE_CART_ITEM', @Id = @Id

				SET @Index = @Index + 1;
			END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		;THROW 400000, 'Remove list cart item fail', 1;
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_CART_ITEM_BY_ID'
BEGIN
	SELECT * FROM CartItem WHERE Id = @Id;
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_CART_ITEMS_BY_USER_ID'
BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		BEGIN
			IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE UserId = @UserId)
				THROW 400000, 'User is cart does not exist', 1;

			SET @RowCount = (SELECT COUNT(1) FROM @CartItems);
			SET @Index = 1;

			WHILE @Index <= @RowCount
				BEGIN

					print @RowCount;
					print @Index;
					-- GET @Id OF @CartItems
					SELECT @Id = Id
					FROM @CartItems
					ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

					IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE Id = @Id)
						THROW 400000, 'Cart item does not exist', 1;
					
					-- GET @ProductId OF CartItem
					SELECT @ProductId = ProductId, @Quantity = Quantity FROM CartItem WHERE Id = @Id

					-- GET Price Product
					 SELECT @Price = Price FROM Product WHERE Id = @ProductId

					-- SET Total
					 SET @Total = COALESCE(@Total, 0) + @Quantity * @Price;

					 SET @Index = @Index + 1;
				END


			SELECT  @Total as Total,
			(SELECT *, (SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS s WHERE s.Id = c.ProductId FOR JSON PATH), '$[0]')) AS _Product
			FROM CartItem AS c WHERE UserId = @UserId AND Id IN (SELECT Id FROM @CartItems) FOR JSON PATH) AS _CartItems
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		DECLARE @Message NVARCHAR(MAX) =  ERROR_MESSAGE();
		;THROW 400000, @Message, 1;
		ROLLBACK TRANSACTION
	END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_CART_BY_USER_ID'
BEGIN
	IF NOT EXISTS (SELECT TOP 1 1 FROM CartItem WHERE UserId = @UserId)
	BEGIN
		;THROW 99001, 'User is cart does not exist', 1;
	END

	SET @RowCount = (SELECT COUNT(1) FROM CartItem WHERE UserId = @UserId);
	SET @Index = 1;

	WHILE @Index <= @RowCount
		BEGIN
			-- GET @ProductId, @Quantity 
			SELECT @ProductId = ProductId, @Quantity = Quantity
			FROM CartItem
			ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
			
			-- GET Price Product
			SELECT @Price = Price FROM Product WHERE Id = @ProductId

			-- SET Total
			SET @Total = COALESCE(@Total, 0) + @Quantity * @Price;

			SET @Index = @Index + 1;
		END


	SELECT  @Total as Total,
	(SELECT *, (SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS s WHERE s.Id = c.ProductId FOR JSON PATH), '$[0]')) AS _Product
	FROM CartItem AS c WHERE UserId = @UserId FOR JSON PATH) AS _CartItems
END
GO
GO
--=========================================================================================
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
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, C.[Status], c.Created, RecordCount.TotalRows as TotalRows
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
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_ROOT'
BEGIN
	SELECT c.Id, c.[Name], c.[Description], c.ImageUrl, c.[Status], c.Created, c.Modified, c.ParentId
	FROM Category AS c (NOLOCK)
	WHERE c.Id = @Id AND c.IsDeleted = 0 AND c.ParentId IS NULL
END
GO
GO

--=========================================================================================
CREATE TYPE CategoryProductExclusionsTableType AS TABLE
(
	CategoryId UNIQUEIDENTIFIER,
	ProductId UNIQUEIDENTIFIER
)
GO

CREATE PROC [dbo].[sp_CategoryDiscount]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		1,
@PageSize						INT					=		10,
@SearchString				    NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId						    UNIQUEIDENTIFIER	=		NULL,
@CategoryId						UNIQUEIDENTIFIER	=		NULL,
@Code							NVARCHAR(150)		=		NULL,
@DiscountType		     		NVARCHAR(150)		=		NULL,
@DiscountValue					DECIMAL     		=		NULL,
@StartDate                      DATETIME            =       NULL,
@EndDate                        DATETIME            =       NULL,
@IsActive                       BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
-----------------------------------------------------------------
@CategoryProductExclusions CategoryProductExclusionsTableType READONLY,
-----------------------------------------------------------------
@RowCount						INT                 =       0,
@Index  						INT                 =       0,
@CateId						    UNIQUEIDENTIFIER	=		NULL,
@ProductId						UNIQUEIDENTIFIER	=		NULL,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =       NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404000, 'User is not found', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM Category WHERE Id = @CategoryId)
	THROW 404000, 'Category is not found', 1

IF EXISTS (SELECT TOP 1 1 FROM CategoryDiscount WHERE Code = @Code)
	THROW 404000, 'Category discount is duplicate', 1

INSERT INTO CategoryDiscount (Id, UserId, Code, CategoryId, DiscountType, DiscountValue, IsActive, StartDate, EndDate, Created)
VALUES(@Id, @UserId, @Code, @CategoryId, @DiscountType, @DiscountValue, @IsActive, @StartDate, @EndDate, GETDATE())

SET @RowCount = (SELECT COUNT(1) FROM @CategoryProductExclusions);
SET @Index = 1;

IF @RowCount > 0
BEGIN
	WHILE @Index <= @RowCount
	BEGIN
		SELECT @CateId = CategoryId, @ProductId = ProductId
		FROM @CategoryProductExclusions
		ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
		
		IF NOT EXISTS 
			(SELECT TOP 1 1 FROM Product p LEFT JOIN Category AS c ON p.CategoryId = c.Id WHERE p.Id = @ProductId AND c.Id = @CategoryId)
			THROW 400000, 'Category product exclusion is invalid', 1

		INSERT INTO CategoryProductExclusion (Id, CategoryDiscountId, CategoryId, ProductId)
		VALUES (NEWID(), @Id, @CategoryId, @ProductId);

		SET @Index = @Index + 1;
	END
END

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404000, 'User is not found', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM Category WHERE Id = @CategoryId)
	THROW 404000, 'Category is not found', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM CategoryDiscount WHERE Id = @Id)
	THROW 400000, 'discount is not found', 1

SET @RowCount = (SELECT COUNT(1) FROM @CategoryProductExclusions);
SET @Index = 1;

DELETE CategoryProductExclusion WHERE CategoryDiscountId = @Id;
IF @RowCount > 0
BEGIN
	WHILE @Index <= @RowCount
	BEGIN
		SELECT @CateId = CategoryId, @ProductId = ProductId
		FROM @CategoryProductExclusions
		ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
		
		IF NOT EXISTS 
			(SELECT TOP 1 1 FROM Product AS p LEFT JOIN Category AS c ON p.CategoryId = c.Id WHERE p.Id = @ProductId AND c.Id = @CategoryId)
			THROW 400000, 'Category product exclusion is invalid', 1

		INSERT INTO CategoryProductExclusion (Id, CategoryDiscountId, CategoryId, ProductId)
		VALUES (NEWID(), @Id, @CategoryId, @ProductId);

		SET @Index = @Index + 1;
	END
END
UPDATE CategoryDiscount
	SET 
		[UserId] = ISNULL(@UserId, UserId),
		CategoryId = ISNULL(@CategoryId, CategoryId),
		Code = ISNULL(@Code, Code),
		[DiscountType] = ISNULL(@DiscountType, DiscountType),
		DiscountValue = ISNULL(@DiscountValue, DiscountValue),
		IsActive = ISNULL(@IsActive, IsActive),
		[StartDate] = ISNULL(@StartDate, [StartDate]),
		[EndDate] = ISNULL(@EndDate, [EndDate]),
		Modified = GETDATE()
	WHERE Id = @Id

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
IF @Activity = 'DELETE'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF NOT EXISTS (SELECT TOP 1 1 FROM CategoryDiscount WHERE Id = @Id)
		THROW 400000, 'discount is not found', 1
		
	DELETE CategoryDiscount WHERE Id = @Id;

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN 
	SELECT cd.Id, cd.UserId, cd.CategoryId, cd.Code, cd.DiscountType, cd.DiscountValue, cd.StartDate, cd.EndDate, cd.Created, cd.Modified, cd.IsActive,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = cd.UserId FOR JSON PATH), '$[0]')) AS _User,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Category AS s WHERE s.Id = cd.CategoryId FOR JSON PATH), '$[0]')) AS _Category,
	(
		SELECT cpe.Id, cpe.CategoryDiscountId, cpe.CategoryId, cpe.ProductId,
		(SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS p WHERE p.Id = cpe.ProductId FOR JSON PATH), '$[0]')) AS _Product
		FROM CategoryProductExclusion AS cpe
		WHERE cpe.CategoryDiscountId = cd.Id FOR JSON PATH
	) AS _CategoryProductExclusions
	FROM CategoryDiscount AS cd WHERE cd.Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	SELECT *, RecordCount.TotalRows as TotalRows
	FROM CategoryDiscount AS cd
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM CategoryDiscount
	) as RecordCount
	ORDER BY cd.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END

GO
--=========================================================================================
CREATE TYPE OrderItemsTableType AS TABLE
(
   ProductId UNIQUEIDENTIFIER,
   Quantity INT
);
GO


CREATE PROC [dbo].[sp_Orders]
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
@PaymentId                      UNIQUEIDENTIFIER    =       NULL,
@PromotionId                    UNIQUEIDENTIFIER    =       NULL,
@Total						    DECIMAL     		=		NULL,
@Note       					NVARCHAR(MAX)		=		NULL,
@OrderStatus                    NVARCHAR(20)        =       NULL,
@PaymentStatus                  NVARCHAR(20)        =       NULL,
@PaymentMethod                  NVARCHAR(20)        =       NULL,
@IsCancelled                    BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
-----------------------------------------------------------------
@OrderItems                     OrderItemsTableType READONLY,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =       NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL,
-----------------------------------------------------------------
@ProductId                      UNIQUEIDENTIFIER    =       NULL,
@InventoryId                    UNIQUEIDENTIFIER    =       NULL,
@Quantity                       INT                 =       NULL,
@Price                          DECIMAL             =       NULL,
@TotalTemp                      DECIMAL             =       NULL,
@RowCount                       INT                 =       NULL,
@Index                          INT                 =       NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404000, 'User is not found', 1

IF (@PaymentId IS NOT NULL)
	IF NOT EXISTS (SELECT TOP 1 1 FROM [PaymentDetail] WHERE Id = @PaymentId)
		THROW 404000, 'Payment detail is not found', 1

IF (@PromotionId IS NOT NULL)
	IF NOT EXISTS (SELECT TOP 1 1 FROM Promotion WHERE Id = @PromotionId AND IsActive = 1)
		THROW 404000, 'Promotion is not found', 1

INSERT INTO [Order] (Id, [UserId], PaymentId, PromotionId, PaymentStatus, PaymentMethod, OrderStatus, ToTal, Note, Created, IsCancelled, IsDeleted)
VALUES (@Id, @UserId, @PaymentId, @PromotionId, @PaymentStatus, @PaymentMethod, 'PENDING', @Total, @Note, GETDATE(), 0, 0)

SET @RowCount = (SELECT COUNT(*) FROM @OrderItems);
SET @Index = 1;
WHILE @Index <= @RowCount
BEGIN
	SELECT @ProductId = ProductId, @Quantity = Quantity
	FROM @OrderItems
	ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

	IF NOT EXISTS (SELECT TOP 1 1 FROM Product WHERE Id = @ProductId)
		THROW 400000, 'Product in order item does not exist', 1;

	SELECT @Price = Price, @InventoryId = InventoryId FROM Product WHERE Id = @ProductId ;

	IF NOT EXISTS (SELECT TOP 1 1 FROM Inventory WHERE Id = @InventoryId AND @Quantity <= Quantity)
		THROW 400000, 'The number of products in stock is not enough', 1;

	UPDATE Product SET QuantitySold = (COALESCE(QuantitySold, 0) + @Quantity) WHERE Id = @ProductId;
			
	--- calculate total product
	SET @TotalTemp = COALESCE(@TotalTemp, 0) + @Price * @Quantity

	--- update Inventory
	UPDATE Inventory SET Quantity = (Quantity - @Quantity) WHERE Id = @InventoryId

	-- remove cart 
	DELETE CartItem WHERE UserId = @UserId AND ProductId = @ProductId;

	-- CREATE ORDER ITEM
	INSERT INTO OrderItem (Id, OrderId, ProductId, Quantity, Created) 
	VALUES (NEWID(), @Id, @ProductId,  @Quantity, GETDATE())

	SET @Index = @Index + 1;
END

 -- Promotion
IF (@PromotionId IS NOT NULL)
BEGIN
	IF NOT EXISTS (SELECT TOP 1 1 FROM [Promotion] WHERE Id = @PromotionId AND IsActive = 1)
		THROW 404000, 'Promotion is not found', 1;

	IF NOT EXISTS (SELECT TOP 1 1 FROM [Promotion] WHERE Id = @PromotionId AND GETDATE() BETWEEN StartDate AND EndDate)
		THROW 404000, 'Promotion has expired or not started', 1;

	IF NOT EXISTS (SELECT TOP 1 1 FROM [Promotion] WHERE Id = @PromotionId AND @TotalTemp >= MinimumOrderAmount)
		THROW 404000, 'promotion is not valid', 1;

	SELECT @TotalTemp = 
		CASE 
			WHEN pr.DiscountType = 'PERCENT' 
				THEN ROUND(@TotalTemp * (1 - pr.DiscountValue / 100), 2)
			WHEN pr.DiscountType = 'FIXED' 
				THEN ROUND(@TotalTemp - pr.DiscountValue, 2)
			ELSE @Total
		END
	FROM Promotion AS pr 
	WHERE pr.Id = @PromotionId AND pr.IsActive = 1 
		AND GETDATE() BETWEEN pr.StartDate AND pr.EndDate AND @TotalTemp >= MinimumOrderAmount;
END

SELECT @Total = FLOOR(@Total), @TotalTemp = FLOOR(@TotalTemp)

IF (@Total != @TotalTemp)
	THROW 404000, 'Invalid total payment', 1;


COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'CANCEL_ORDER'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

SELECT @Id = Id, @OrderStatus = OrderStatus FROM [Order] WHERE Id = @Id ;
IF (@Id IS NULL)
	THROW 400000, 'Order is not found', 1;
	
IF (@OrderStatus != 'PENDING')
	THROW 400000, 'Can not cancel order', 1;

SET @RowCount = (SELECT COUNT(1) FROM OrderItem WHERE OrderId = @Id);
SET @Index = 1;
WHILE @Index <= @RowCount
BEGIN
	SELECT @ProductId = ProductId, @Quantity = Quantity
	FROM OrderItem
	ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

	SELECT @InventoryId = InventoryId FROM Product WHERE Id = @ProductId ;

	UPDATE Inventory SET Quantity = (Quantity + @Quantity) WHERE Id = @InventoryId

	SET @Index = @Index + 1;
END

UPDATE [Order] SET IsCancelled = 1 WHERE Id = @Id;

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

SELECT @Id = Id, @IsCancelled = IsCancelled FROM [Order] WHERE Id = @Id ;
IF (@Id IS NULL)
	THROW 400000, 'Order is not found', 1;
	
IF (@IsCancelled = 1) --Cancelled
	THROW 400000, 'Order has been canceled and cannot be updated', 1;

UPDATE [Order] 
	SET OrderStatus = ISNULL(@OrderStatus, OrderStatus),
		Note = ISNULL(@Note, Note),
		Modified = GETDATE()
	WHERE Id = @Id;

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
;WITH OrderTemp AS
	(
		SELECT po.Id
		FROM [Order] (NOLOCK) po
		LEFT JOIN [User] AS u ON u.Id = po.UserId
 		WHERE (@SearchString IS NULL OR po.Note LIKE N'%'+@SearchString+'%' OR  u.[Fullname] LIKE N'%'+@SearchString+'%')
		AND (@UserId IS NULL OR po.UserId = @UserId)
		AND (@OrderStatus IS NULL OR po.OrderStatus = @OrderStatus)
		AND (@PaymentStatus IS NULL OR po.PaymentStatus = @PaymentStatus)
		AND ((@FromPrice IS NULL OR @ToPrice IS NULL) OR (po.Total >= @FromPrice AND po.Total <= @ToPrice))
		AND ((@FromTime IS NULL OR @ToTime IS NULL) OR (po.Created >= @FromTime AND po.Created <= @ToTime))
		AND (@IsCancelled IS NULL OR po.IsCancelled = @IsCancelled)
	)

	SELECT o.Id, o.UserId, o.PaymentId, o.PromotionId, o.OrderStatus, o.Total, o.Note, o.Created, o.Modified, o.IsCancelled,
	RecordCount.TotalRows as TotalRows
	FROM OrderTemp AS ot
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM OrderTemp
	) as RecordCount
	LEFT JOIN [Order] (NOLOCK) o ON o.Id = ot.Id
	ORDER BY o.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
	IF NOT EXISTS (SELECT TOP 1 1 FROM [Order] WHERE Id = @Id)
		THROW 404000, 'Order is not found', 1;
	
	SELECT  o.Id, o.UserId, o.PaymentId, o.PromotionId, o.OrderStatus, o.Total, o.Note, o.Created, o.Modified, o.IsCancelled,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = o.UserId FOR JSON PATH), '$[0]')) AS _User,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM PaymentDetail AS pd WHERE pd.Id = o.PaymentId FOR JSON PATH), '$[0]')) AS _PaymentDetail,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Promotion AS pr WHERE pr.Id = o.PromotionId FOR JSON PATH), '$[0]')) AS _Promotion,
	(
		SELECT oi.Id, oi.OrderId, oi.ProductId, oi.Quantity,
		(SELECT JSON_QUERY((SELECT TOP(1) * FROM Product AS s WHERE s.Id = oi.ProductId FOR JSON PATH), '$[0]')) AS _Product
		FROM OrderItem AS oi
		WHERE oi.OrderId = o.Id FOR JSON PATH
	) AS _OrderItems

	FROM [Order] AS o (NOLOCK)
	WHERE o.Id = @Id
END
GO
--=========================================================================================
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
	VALUES (@Id, @Name, @Slug, @Description, @ImageUrl, @OriginalPrice, @Price, 0, @CategoryId, @SupplierId, @BrandId, @Status, @IsBestSelling, @IsNew, GETDATE())
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
BEGIN TRANSACTION
BEGIN TRY
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
			THROW 404000, 'Product is not found', 1;

	END 
COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
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
	SELECT p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice, p.Price, i.Quantity,p.QuantitySold, p.[Status], p.IsBestSelling, p.IsNew,
	p.CategoryId, p.InventoryId, p.BrandId, p.SupplierId
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
		AND (@SupplierId IS NULL OR p.SupplierId = @SupplierId)
		AND ((@FromPrice IS NULL OR @ToPrice IS NULL) OR (p.Price >= @FromPrice AND p.Price <= @ToPrice))
		AND ((@FromTime IS NULL OR @ToTime IS NULL) OR (p.Created >= @FromTime AND p.Created <= @ToTime))
		AND (@IsBestSelling IS NULL OR p.IsBestSelling = @IsBestSelling)
		AND (@IsNew IS NULL OR p.IsNew = @IsNew)
		AND (@Status IS NULL OR p.[Status] = @Status)
		AND p.IsDeleted = 0
	)
	SELECT p.Id, p.[Name], p.Slug, p.[Description], p.ImageUrl, p.OriginalPrice, p.Price, i.Quantity,p.QuantitySold, p.[Status], p.IsBestSelling, p.IsNew,
	p.CategoryId, p.InventoryId, p.BrandId, p.SupplierId,
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
GO

--=========================================================================================
CREATE PROC [dbo].[sp_Promotions]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		1,
@PageSize						INT					=		10,
@SearchString				    NVARCHAR(MAX)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId						    UNIQUEIDENTIFIER	=		NULL,
@Code							NVARCHAR(150)		=		NULL,
@DiscountType		     		NVARCHAR(150)		=		NULL,
@DiscountValue					DECIMAL     		=		NULL,
@MinimumOrderAmount             DECIMAL     		=		NULL,
@StartDate                      DATETIME            =       NULL,
@EndDate                        DATETIME            =       NULL,
@IsActive                       BIT                 =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =       NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404000, 'User is not found', 1

IF EXISTS (SELECT TOP 1 1 FROM Promotion WHERE Code = @Code)
	THROW 404000, 'Promotion is duplicate', 1

INSERT INTO Promotion (Id, UserId, Code, DiscountType, DiscountValue, MinimumOrderAmount,IsActive, StartDate, EndDate, Created)
VALUES(@Id, @UserId, @Code, @DiscountType, @DiscountValue, @MinimumOrderAmount, @IsActive, @StartDate, @EndDate, GETDATE())

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
BEGIN TRANSACTION
BEGIN TRY

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404000, 'User is not found', 1

UPDATE Promotion
	SET 
		[UserId] = ISNULL(@UserId, UserId),
		Code = ISNULL(@Code, Code),
		[DiscountType] = ISNULL(@DiscountType, DiscountType),
		DiscountValue = ISNULL(@DiscountValue, DiscountValue),
		MinimumOrderAmount = ISNULL(@MinimumOrderAmount, MinimumOrderAmount),
		IsActive = ISNULL(@IsActive, IsActive),
		[StartDate] = ISNULL(@StartDate, [StartDate]),
		[EndDate] = ISNULL(@EndDate, [EndDate]),
		Modified = GETDATE()
	WHERE Id = @Id

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF NOT EXISTS (SELECT TOP 1 1 FROM Promotion WHERE Id = @Id)
	THROW 400000, 'promotion is not found', 1
		
	DELETE Promotion WHERE Id = @Id;
COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF NOT EXISTS (SELECT TOP 1 1 FROM Promotion WHERE Id = @Id)
	THROW 400000, 'promotion is not found', 1
		
	SELECT p.Id, p.UserId, p.Code, p.DiscountType, p.DiscountValue, p.MinimumOrderAmount, p.StartDate, p.EndDate, p.Created, p.Modified, p.IsActive,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = p.UserId FOR JSON PATH), '$[0]')) AS _User
	FROM Promotion AS p WHERE p.Id = @Id
COMMIT TRANSACTION

END TRY
BEGIN CATCH
	SET @ErrorMessage = ERROR_MESSAGE();
	THROW 400000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
SELECT *, RecordCount.TotalRows as TotalRows
	FROM Promotion AS p
	CROSS JOIN 
	(
		SELECT COUNT(*) AS TotalRows
		FROM Promotion
	) as RecordCount
	ORDER BY p.Created DESC
	OFFSET ((@PageIndex - 1) * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END

GO
--=========================================================================================
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
@ErrorState                     INT                 =       NULL,
-----------------------------------------------------------------
@ProductId                      UNIQUEIDENTIFIER    =       NULL,
@Quantity                       INT                 =       NULL,
@Price                          DECIMAL             =       NULL,
@RowCount                       INT                 =       NULL,
@Index                          INT                 =       NULL
-----------------------------------------------------------------
AS
IF @Activity = 'INSERT'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
-- CREATE PURCHASE ORDER
INSERT INTO PurchaseOrder (Id, SupplierId, UserId, TotalMoney, Note, OrderStatus, PaymentStatus, TotalPaymentAmount, Created)
VALUES (@Id, @SupplierId, @UserId, @TotalMoney, @Note, @OrderStatus, @PaymentStatus, @TotalPaymentAmount, GETDATE())

SET @RowCount = (SELECT COUNT(*) FROM @PurchaseOrderDetails);
IF @RowCount > 0
	BEGIN
	SET @Index = 1;

	-- DRAFT_INVOICE
	IF @OrderStatus = 'DRAFT_INVOICE'
		BEGIN
		WHILE @Index <= @RowCount
			BEGIN
				SELECT @ProductId = ProductId, @Quantity = Quantity, @Price = Price
				FROM @PurchaseOrderDetails
				ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;

				-- CHECK THE PRODUCT EXISTENCE
				IF NOT EXISTS (SELECT TOP 1 1 FROM Product WHERE Id = @ProductId)
					THROW 400000, 'Product in purchase order does not exist', 1;

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
				THROW 400000, 'Product in purchase order does not exist', 1;
										

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
		THROW 400000, 'Order status invalid', 1
	END
ELSE 
	THROW 400000, 'Your order has no products?', 1
	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	SELECT @ErrorMessage =  ERROR_MESSAGE();
	THROW 500, @ErrorMessage , 1
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
--=========================================================================================
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
	SELECT s.Id, s.[Name], s.[Description], s.[Address], s.Phone, s.Email, s.ContactPerson, s.[Status], s.TotalAmountOwed, s.Created,RecordCount.TotalRows as TotalRows
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

--=========================================================================================
CREATE PROC [dbo].[sp_UserAddresses]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@Id						        UNIQUEIDENTIFIER	=		NULL,
@UserId						    UNIQUEIDENTIFIER	=		NULL,
@Name							NVARCHAR(150)		=		NULL,
@DeliveryAddress                NVARCHAR(MAX)       =       NULL,
@Telephone   					NVARCHAR(20)		=		NULL,
@Active                         BIT                  =       NULL,
@Created                        DATETIME            =       NULL,
@Modified                       DATETIME            =       NULL,
@IsDeleted						BIT                 =       0,
-----------------------------------------------------------------
@ErrorMessage                   NVARCHAR(MAX)       =       NULL,
@ErrorSeverity                  INT                 =       NULL,
@ErrorState                     INT                 =       NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'GET_USER_ADDRESSES_BY_USER_ID'
BEGIN
	SELECT * FROM UserAddress WHERE UserId = @UserId
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_USER_ADDRESS_BY_USER_ID'
BEGIN
	SELECT TOP(1) * FROM UserAddress WHERE UserId = @UserId AND Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CREATE_USER_ADDRESS'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF NOT EXISTS (SELECT TOP 1 1 FROM UserAddress AS ua (NOLOCK) WHERE ua.DeliveryAddress = @DeliveryAddress)
		THROW 400000, 'User addresss with the same address already exits.',  1

	IF(@Active = 1)
		BEGIN
			UPDATE UserAddress SET Active = 1 WHERE 1 = 1 AND UserId = @UserId
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END
	INSERT INTO UserAddress (Id, UserId, [Name], DeliveryAddress, Telephone, Active, Created)
	VALUES (@Id, @UserId, @Name, @DeliveryAddress, @Telephone, @Active, GETDATE())
	
	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	SELECT @ErrorMessage =  ERROR_MESSAGE();
	THROW 500000, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END 
-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE_USER_ADDRESS'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	IF(@Active = 1)
		BEGIN
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END
	UPDATE UserAddress
	SET 
		UserId = ISNULL(@UserId, UserId),
		[Name] = ISNULL(@Name, [Name]),
		DeliveryAddress = ISNULL(@DeliveryAddress, DeliveryAddress),
		Telephone = ISNULL(@Telephone, Telephone),
		Active = ISNULL(@Active, Active),
		Modified = GETDATE()
	WHERE Id = @Id

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
	THROW 50000, 'Update user address fail', 1
	ROLLBACK TRANSACTION
END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE_USER_ADDRESS'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
	DELETE UserAddress WHERE Id = @Id AND UserId = @UserId
	
	SELECT TOP 1 @Id = Id FROM UserAddress 
	IF(@Id IS NULL)
		BEGIN
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END
	ELSE
		BEGIN
			UPDATE UserAddress SET Active = 1 WHERE Id = @Id
			UPDATE [User] SET UserAddressId = @Id WHERE 1 = 1 AND Id = @UserId
		END

	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	THROW 500, 'Delete user address fail', 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'SET_DEFAULT_ADDRESS_FOR_USER'
BEGIN
BEGIN TRANSACTION
BEGIN TRY	
	BEGIN
		UPDATE UserAddress SET Active = 1 WHERE Id = @Id
		UPDATE [User] SET UserAddressId = @Id WHERE Id = @UserId
		COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
	THROW 500, 'Set default user address for user fail', 1
	ROLLBACK TRANSACTION
END CATCH
END
GO
