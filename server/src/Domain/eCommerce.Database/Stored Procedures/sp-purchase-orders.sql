USE eCommerce
GO

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


