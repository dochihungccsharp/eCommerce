USE eCommerce
GO

--CREATE TYPE CategoryProductExclusion AS TABLE
--(
--	CategoryId UNIQUEIDENTIFIER,
--	ProductId UNIQUEIDENTIFIER
--)
--GO

ALTER PROC [dbo].[sp_CategoryDiscount]
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@PageIndex						INT					=		1,
@PageSize						INT					=		10,
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
@CategoryProductExclusions CategoryProductExclusion READONLY,
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
-- Percentage discount
IF(@DiscountType != 'PERCENT' OR @DiscountType != 'FIXED')
	THROW 400, 'Discount type is invalid', 1

IF(@DiscountType = 'PERCENT' AND (@DiscountValue <= 0 OR @DiscountValue >= 100))
	THROW 400, 'Discount type and Discount value is invalid', 1
	
IF(@EndDate >= @StartDate)
	THROW 400, 'Invalid discount period', 1

IF(@EndDate < GETDATE())
	THROW 400, 'Invalid discount period', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404, 'User is not found', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM Category WHERE Id = @CategoryId)
	THROW 404, 'Category is not found', 1

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
			THROW 400, 'Category product exclusion is invalid', 1

		INSERT INTO CategoryProductExclusion (Id, CategoryDiscountId, CategoryId, ProductId)
		VALUES (NEWID(), @Id, @CategoryId, @ProductId);

		SET @Index = @Index + 1;
	END
END

INSERT INTO CategoryDiscount (Id, UserId, CategoryId, DiscountType, DiscountValue, IsActive, StartDate, EndDate, Created)
VALUES(@Id, @UserId, @CategoryId, @DiscountType, @DiscountValue, @IsActive, @StartDate, @EndDate, GETDATE())

COMMIT TRANSACTION

END TRY
BEGIN CATCH
	THROW 400, 'Inseart discount category fail', 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'UPDATE'
BEGIN
BEGIN TRANSACTION
BEGIN TRY
-- Percentage discount
IF(@DiscountType != 'PERCENT' OR @DiscountType != 'FIXED')
	THROW 400, 'Discount type is invalid', 1

IF(@DiscountType = 'PERCENT' AND (@DiscountValue <= 0 OR @DiscountValue >= 100))
	THROW 400, 'Discount type and Discount value is invalid', 1
	
IF(@EndDate >= @StartDate)
	THROW 400, 'Invalid discount period', 1

IF(@EndDate < GETDATE())
	THROW 400, 'Invalid discount period', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM [User] WHERE Id = @UserId)
	THROW 404, 'User is not found', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM Category WHERE Id = @CategoryId)
	THROW 404, 'Category is not found', 1

IF NOT EXISTS (SELECT TOP 1 1 FROM CategoryDiscount WHERE Id = @Id AND StartDate < GETDATE())
	THROW 400, 'discount has started, cannot be edited', 1

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
			(SELECT TOP 1 1 FROM Product AS p LEFT JOIN Category AS c ON p.CategoryId = c.Id WHERE p.Id = @ProductId AND c.Id = @CategoryId)
			THROW 400, 'Category product exclusion is invalid', 1

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
	THROW 400, @ErrorMessage , 1
	ROLLBACK TRANSACTION
END CATCH
END
-----------------------------------------------------------------
ELSE IF @Activity = 'GET_BY_ID'
BEGIN 
	SELECT cd.Id, cd.UserId,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM [User] AS u WHERE u.Id = cd.UserId FOR JSON PATH), '$[0]')) AS _User,
	(SELECT JSON_QUERY((SELECT TOP(1) * FROM Category AS s WHERE s.Id = cd.CategoryId FOR JSON PATH), '$[0]')) AS _Category,
	(
		SELECT cpe.Id, cpe.CategoryDiscountId, cpe.CategoryId, cpe.ProductId
		FROM CategoryProductExclusion AS cpe
		LEFT JOIN Product AS p ON p.Id = cpe.ProductId
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