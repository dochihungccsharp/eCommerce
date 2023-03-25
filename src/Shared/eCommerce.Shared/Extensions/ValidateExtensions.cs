namespace eCommerce.Shared.Extensions;

public static class ValidateExtensions
{
    public static bool ValidateInputIsOfTypeGuid(this string[] input)
    {
        if (input == null && !input.Any())
        {
            return false;
        }
        
        Guid guidResult = new Guid();

        foreach (string guidStr in input)
        {
            if (!Guid.TryParse(guidStr.Trim(), out guidResult))
            {
                return false;
            }
        }
        return true;
    }
}