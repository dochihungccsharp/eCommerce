USE eCommerce
GO


CREATE PROC sp_Provinces
@Activity						NVARCHAR(50)		=		NULL,
-----------------------------------------------------------------
@ProvinceId					    UNIQUEIDENTIFIER	=		NULL,
@DistrictId					    UNIQUEIDENTIFIER	=		NULL
-----------------------------------------------------------------
AS
-----------------------------------------------------------------
IF @Activity = 'GET_ALL_P'
BEGIN
	SELECT * FROM Province	
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_D'
BEGIN
	SELECT * FROM District
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_D_BY_P_ID'
BEGIN
	SELECT * FROM District WHERE ProvinceId = @ProvinceId
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_W'
BEGIN
	SELECT * FROM Ward
END

-----------------------------------------------------------------
ELSE IF @Activity = 'GET_ALL_W_BY_D_ID'
BEGIN
	SELECT * FROM Ward WHERE DistrictId = @DistrictId
END
GO





































