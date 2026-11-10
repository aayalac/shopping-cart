namespace ShoppingCart.Api.Shared.Extensions;

public static class StringExtensions
{
    public static string EncodeForLike(this string search)
    {
        return search.Replace("[", "[]]").Replace("%", "[%]");
    }
}

