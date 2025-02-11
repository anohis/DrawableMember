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
        AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class TypeAttribute : Attribute
    {
        public readonly Type Type;

        public TypeAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(
        AttributeTargets.Parameter
            | AttributeTargets.Property
            | AttributeTargets.Method
            | AttributeTargets.Field,
        AllowMultiple = false)]
    public sealed class NameAttribute : Attribute
    {
        public readonly string Name;

        public NameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(
       AttributeTargets.Class
            | AttributeTargets.Struct,
       AllowMultiple = false)]
    public sealed class MemberSelectorAttribute : Attribute
    {
        public readonly Type SelectorType;

        public MemberSelectorAttribute(Type selectorType)
        {
            SelectorType = selectorType;
        }
    }
}