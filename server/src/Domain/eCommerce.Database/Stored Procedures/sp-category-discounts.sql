USE eCommerce
GO

CREATE TYPE CategoryProductExclusionsTableType AS TABLE
(
	CategoryId UNIQUEIDENTIFIER,
	ProductId UNIQUEIDENTIFIER
)
GO

ALTER PROC [dbo].[sp_CategoryDiscount]
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

