namespace DrawableMember.Runtime
{
    using System;

    [AttributeUsage(
        AttributeTargets.Method
            | AttributeTargets.Property
            | AttributeTargets.Field)]
    public sealed class DrawableAttribute : Attribute
    {
    }

    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property,
        AllowMultiple = false)]
    public sealed class DrawableTypeAttribute : Attribute
    {
        public readonly Type Type;

        public DrawableTypeAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(
        AttributeTargets.Parameter
            | AttributeTargets.Property
            | AttributeTargets.Method
        , AllowMultiple = false)]
    public sealed class DrawableNameAttribute : Attribute
    {
        public readonly string Name;

        public DrawableNameAttribute(string name)
        {
            Name = name;
        }
    }
}