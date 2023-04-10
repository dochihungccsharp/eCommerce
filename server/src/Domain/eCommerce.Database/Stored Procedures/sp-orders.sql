USE eCommerce
GO

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