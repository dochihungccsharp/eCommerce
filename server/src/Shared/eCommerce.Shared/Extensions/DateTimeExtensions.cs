namespace eCommerce.Shared.Extensions;

public static class DateTimeExtensions
{
    public static long ToDifference(this DateTime issuedTime, DateTime expiredTime)
    {
        return (long)(expiredTime.Subtract(issuedTime)).TotalDays;
    }
}