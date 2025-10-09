using System.Globalization;

namespace MyApi.Middleware
{
    public static class NameUppercasePipe
    {
        public static string ToUpperCase(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return name.ToUpper(CultureInfo.InvariantCulture);
        }
    }
}
