namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Linq;

    internal static class IMemberSelectorExtensions
    {
        public static bool Any(this IMemberSelector memberSelector)
            => memberSelector.Fields.Any()
                || memberSelector.Properties.Any()
                || memberSelector.Methods.Any();
    }
}