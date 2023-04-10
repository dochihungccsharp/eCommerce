USE eCommerce
GO


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



