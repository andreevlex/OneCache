using System;
using System.Linq;

namespace OneCache
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }

    public class LocationAttribute : Attribute
    {
        internal LocationAttribute(string location)
        {
            this.Location = location;
        }

        public string Location { get; private set; }
    }
        
    public enum TypeCache
    {
        [Location("LOCALAPPDATA")]
        Metadata,
        [Location("APPDATA")]
        Settings
    }
}