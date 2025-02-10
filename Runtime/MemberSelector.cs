namespace DrawableMember.Runtime
{
    using System.Reflection;

    public interface IMemberSelector
    {
        FieldInfo[] Fields { get; }
        PropertyInfo[] Properties { get; }
        MethodInfo[] Methods { get; }
    }
}