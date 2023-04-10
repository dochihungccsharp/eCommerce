USE eCommerce
GO


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

