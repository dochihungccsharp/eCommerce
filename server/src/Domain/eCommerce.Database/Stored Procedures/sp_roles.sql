USE eCommerce
GO

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
