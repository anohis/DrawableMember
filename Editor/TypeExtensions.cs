namespace DrawableMember.Editor
{
    using System;
    using System.Linq;

    internal static class TypeExtensions
    {
        public static bool IsInheritsFrom(this Type derived, Type @base)
            => derived.IsSubclassOf(@base)
                || @base.IsAssignableFrom(derived);

        public static object CreateInstance(this Type type)
        {
            var constructor = type
                .GetConstructors()
                .First();
            var parameters = constructor
                .GetParameters()
                .Select(parameter => parameter.HasDefaultValue
                    ? parameter.DefaultValue
                    : parameter.ParameterType.GetDefault())
                .ToArray();
            return constructor.Invoke(parameters);
        }

        public static object GetDefault(this Type type)
            => type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
    }
}