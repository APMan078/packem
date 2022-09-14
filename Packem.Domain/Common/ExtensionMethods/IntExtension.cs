namespace Packem.Domain.Common.ExtensionMethods
{
    public static class IntExtension
    {
        public static bool IsNumeric(this string s)
            => int.TryParse(s, out _);

        public static int? ToNullableInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public static bool IsPositive(this int number)
        {
            return number > 0;
        }

        public static bool IsNegative(this int number)
        {
            return number < 0;
        }

        public static bool IsZero(this int number)
        {
            return number == 0;
        }
    }
}
