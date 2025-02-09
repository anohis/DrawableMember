namespace DrawableMember.Editor
{
    using System;

    internal static class TypeExtensions
    {
        public static bool IsInheritsFrom(this Type derived, Type @base)
            => derived.IsSubclassOf(@base)
                || @base.IsAssignableFrom(derived);
    }
}