USE eCommerce
GO


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


