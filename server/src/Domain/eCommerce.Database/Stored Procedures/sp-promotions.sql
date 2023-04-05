USE eCommerce
GO

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
