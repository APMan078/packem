using System;
using System.ComponentModel;
using System.Globalization;

namespace Packem.Domain.Common.ExtensionMethods
{
    /// <summary>
    /// Defines extension methods for all Enums
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Pretty-prints an enum name in Title case with spaces instead of underscores
        /// </summary>
        public static string ToLabel(this Enum e)
        {
            if (e == null) return null;

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(e.ToString().ToLower().Replace('_', ' '));
        }

        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }

        public static int ToInt(this Enum e)
        {
            return Convert.ToInt32(e);
        }

        public static bool IsValueExistInEnum(this Enum e)
        {
            return Enum.IsDefined(e.GetType(), e);
        }
    }
}