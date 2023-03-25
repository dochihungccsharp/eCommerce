using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace eCommerce.Shared.Extensions;

public static class StringExtensions
{
    public static string HashMD5(this string text)
    {
        StringBuilder sBuilder = new StringBuilder();
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
        }
        return sBuilder.ToString();
    }
    
    public static string Base64Encode(this string plainText)
    {
        Base64UrlEncoder.Encode(plainText);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64UrlEncode(this string plainText)
    {
        return Base64UrlEncoder.Encode(plainText);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string Base64UrlDecode(this string base64EncodedData)
    {
        return Base64UrlEncoder.Decode(base64EncodedData);
    }

    public static string TrimAndToLower(this string str)
    {
        return str.Trim().ToLower();
    }
    
    public static string RemoveVietnameseSigns(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        text = text.Trim().ToLower();
        text = Regex.Replace(text, @"[áàảãạăắằẳẵặâấầẩẫậ]", "a");
        text = Regex.Replace(text, @"[éèẻẽẹêếềểễệ]", "e");
        text = Regex.Replace(text, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
        text = Regex.Replace(text, @"[íìỉĩị]", "i");
        text = Regex.Replace(text, @"[úùủũụưứừửữự]", "u");
        text = Regex.Replace(text, @"[ýỳỷỹỵ]", "y");
        text = Regex.Replace(text, @"đ", "d");

        return text;
    }
    
    public static string ConvertToSlug(this string input)
    {
        string slug = input.ToLower().RemoveVietnameseSigns().Replace(" ", "-").Replace(",", "").Replace(".", "");
        
        slug = Regex.Replace(slug, @"[^0-9a-z-]+", "");

        return slug;
    }

}