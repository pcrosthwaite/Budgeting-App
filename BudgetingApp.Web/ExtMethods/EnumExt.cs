using System.ComponentModel;

namespace System
{
    internal static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value != null)
            {
                var field = value.GetType().GetField(value.ToString());

                if (field == null)
                {
                    return string.Empty;
                }

                var attribute =
                    Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                return attribute == null ? value.ToString() : attribute.Description;
            }

            return string.Empty;
        }
    }
}