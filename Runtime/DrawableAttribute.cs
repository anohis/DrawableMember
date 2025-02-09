namespace DrawableMember.Runtime
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class DrawableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DrawableTypeAttribute : Attribute
    {
        public readonly Type Type;

        public DrawableTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}