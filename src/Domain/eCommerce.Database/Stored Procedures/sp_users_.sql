USE eCommerce
GO

--CREATE TYPE RoleIdsTableType AS TABLE
--(
--   Id UNIQUEIDENTIFIER
--);
--GO

ALTER PROC sp_Users
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
@RoleIds                        RoleIdsTableType     READONLY 
-----------------------------------------------------------------
AS 
-----------------------------------------------------------------
IF @Activity = 'INSERT'
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		BEGIN
			INSERT INTO [User] 
			([Id], [UserName], [Fullname], [Email], [EmailConfirmed], [PasswordHash], [PhoneNumber], [Avatar], [Address], [TotalAmountOwed],[UserAddressId], [Status], [Created], [IsDeleted])
			VALUES
			(@Id, @Username, @Fullname, @Email, @EmailConfirmed, @PasswordHash, @PhoneNumber, @Avatar, @Address, @TotalAmountOwed, @UserAddressId, 1, GETDATE(), 0)

			DECLARE @RoleId UNIQUEIDENTIFIER;
			DECLARE @Index INT = 1;

			DECLARE @RowCount INT = (SELECT COUNT(*) FROM @RoleIds);
			IF @RowCount > 0
					WHILE @Index <= @RowCount
						BEGIN
							SELECT @RoleId = Id
							FROM @RoleIds
							ORDER BY (SELECT NULL) OFFSET @Index-1 ROWS FETCH NEXT 1 ROWS ONLY;
							INSERT INTO [UserRole] (UserId, RoleId)
							VALUES (@Id, @RoleId)
							SET @Index = @Index + 1
						END
		END
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
	PRINT '1111111'
		RAISERROR('Error: No details provided', 16, 1)
		ROLLBACK TRANSACTION
	END CATCH

END

-----------------------------------------------------------------
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
				[Modified] = GETDATE(),
				[IsDeleted] = ISNULL(@IsDeleted, [IsDeleted])
			WHERE Id = @Id
	
			-- DELETE ALL USER ROLE
			DELETE FROM [UserRole] WHERE UserId = @Id;

			-- ADD ROLE ROLE
			DECLARE @RoleId_ UNIQUEIDENTIFIER;
			DECLARE @Index_ INT = 1;

			DECLARE @RowCount_ INT = (SELECT COUNT(*) FROM @RoleIds);
			IF @RowCount_ > 0
				BEGIN
					WHILE @Index_ <= @RowCount_
						BEGIN
							SELECT @RoleId_ = Id
							FROM @RoleIds
							ORDER BY (SELECT NULL) OFFSET @Index_-1 ROWS FETCH NEXT 1 ROWS ONLY;

							INSERT INTO [UserRole] (UserId, RoleId)
							VALUES (@Id, @RoleId)

							SET @Index_ += 1
						END
				END
			COMMIT TRANSACTION
		END
	END TRY
	BEGIN CATCH
		RAISERROR('Error: No details provided', 16, 1)
		ROLLBACK TRANSACTION
	END CATCH
END

-----------------------------------------------------------------
ELSE IF @Activity = 'DELETE'
BEGIN
	UPDATE [User] SET [IsDeleted] = 1 WHERE [Id] = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'CHECK_DUPLICATE'
BEGIN
	SELECT TOP 1 1
	FROM [User] (NOLOCK)
	WHERE  (@Username IS NULL OR [Username] = @Username) 
	AND  (@Email IS NULL OR [Email] = @Email) 
	AND  (@PhoneNumber IS NULL OR [PhoneNumber] = @PhoneNumber) 
	AND (@Id IS NULL OR Id <> @Id)
	AND [IsDeleted] = 0
END

-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_BY_EMAIL'
BEGIN
	SELECT * FROM [User] WHERE [Email] = @Email
END

-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_BY_ID'
BEGIN
	SELECT * FROM [User] WHERE Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'FIND_BY_NAME'
BEGIN
	SELECT * FROM [User] WHERE [Username] = @Username
END

-----------------------------------------------------------------
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
	WHERE u.Id = @Id
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL'
BEGIN
	;WITH UserTemp AS(
		SELECT * 
		FROM [User](NOLOCK) u
		WHERE (@SearchString IS NULL OR u.[Fullname] LIKE N'%'+@SearchString+'%' 
		OR u.[Username] LIKE N'%'+@SearchString+'%' 
		OR  u.[Email] LIKE N'%'+@SearchString+'%'
		OR  u.[PhoneNumber] LIKE N'%'+@SearchString+'%') 
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

EXEC sp_Users @Activity = 'CHECK_DUPLICATE', @Username = 'dochihung492002@gmai.com'

