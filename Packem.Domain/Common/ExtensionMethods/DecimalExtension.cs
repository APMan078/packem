namespace Packem.Domain.Common.ExtensionMethods
{
    public static class DecimalExtension
    {
        public static bool IsInteger(this decimal number)
        {
            return (number % 1) == 0;
        }
    }
}