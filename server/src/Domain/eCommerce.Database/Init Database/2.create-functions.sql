-- + + + + + FUNCTION + + + + + --
CREATE FUNCTION fn_GetStringByToken
(
	@String VARCHAR(MAX),
	@Delimiter CHAR(1),
    @Position INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN 
	-- Khai báo biến StartIndex INT, @EndIndex INT
	DECLARE @StartIndex INT = 1, @EndIndex INT

	-- Tìm vị trí bắt đầu
	DECLARE @Index INT = 1;
	IF(@Position = 1) SET @StartIndex = 0
	ELSE
		BEGIN
			WHILE (@Index < @Position)
				BEGIN
					SET @StartIndex = CHARINDEX(@Delimiter, @String, @StartIndex) + 1;
					-- Nếu từ  @StartIndex mà không tìm thấy @Delimiter trong @String thì retrun
					IF(CHARINDEX(@Delimiter, @String, @StartIndex) = 0) RETURN '';
					SET @Index = @Index + 1;
				END
		END
	-- Tìm vị trí kết thúc
	SET @EndIndex = CHARINDEX(@Delimiter, @String, @StartIndex);

	IF(@EndIndex <> 0)
		RETURN SUBSTRING(@String, @StartIndex , @EndIndex - @StartIndex)
	RETURN ''
END
GO


CREATE FUNCTION fn_GetStringByTokenUseStringSplit
(
	@String VARCHAR(MAX),
	@Delimiter CHAR(1),
    @Position INT
)
RETURNS VARCHAR(MAX)
AS
BEGIN 
	DECLARE @Result VARCHAR(MAX)
	IF (@Position <= (SELECT COUNT(*) FROM STRING_SPLIT(@String, @Delimiter)))
		BEGIN
			SELECT @Result = value
			FROM STRING_SPLIT(@String, @Delimiter)
			ORDER BY (SELECT NULL)
			OFFSET @Position - 1 ROWS FETCH NEXT 1 ROW ONLY;
			RETURN TRIM(@Result);
		END
	RETURN '';
END
GO
