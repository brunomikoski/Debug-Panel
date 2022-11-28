using System.Reflection;

namespace BrunoMikoski.DebugTools
{
    public static class FieldInfoExtensions
    {
        public static bool TryGetAttribute<T>(this FieldInfo targetFieldInfo, out T resultAttribute)
        {
            object[] attributes = targetFieldInfo.GetCustomAttributes(typeof(T), false);
            if (attributes.Length <= 0)
            {
                resultAttribute = default(T);
                return false;
            }

            resultAttribute = (T)attributes[0];
            return true;
        }

        public static bool HasAttribute<T>(this FieldInfo targetFieldInfo)
        {
            return targetFieldInfo.TryGetAttribute<T>(out _);
        }
    }
}
