using System;

namespace Packem.Domain.Common.ExtensionMethods
{
    public static class DateTimeExtension
    {
		/// <summary>
		/// Method checks if passed string is datetime
		/// </summary>
		/// <param name="text">string text for checking</param>
		/// <returns>bool - if text is datetime return true, else return false</returns>
		public static bool IsDateTime(this string text)
		{
            // Check for empty string.
            if (string.IsNullOrEmpty(text))
			{
				return false;
			}

            bool isDateTime = DateTime.TryParse(text, out _);

            return isDateTime;
		}

		public static DateTime ToDateTime(this string text)
		{
			return DateTime.Parse(text);
		}
	}
}
