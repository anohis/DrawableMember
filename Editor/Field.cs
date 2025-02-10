namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Reflection;

    internal class Field : IMemberDrawer
    {
        private readonly FieldInfo _field;
        private readonly IVariable _value;

        public Field(
            FieldInfo field,
            IVariable value)
        {
            _field = field;
            _value = value;
        }

        void IMemberDrawer.Draw(object target)
        {
            _value.Value = _field.GetValue(target);
            _value.Draw();
            _field.SetValue(target, _value.Value);
        }
    }

    internal class FieldFactory
    {
        private readonly VariableFactory _variableFactory;

        public FieldFactory(VariableFactory variableFactory)
        {
            _variableFactory = variableFactory;
        }

        public Field Create(FieldInfo info)
            => new(
                info,
                _variableFactory.Create(
                    info.GetCustomAttribute<NameAttribute>()
                        ?.Name
                        ?? info.Name,
                    info.FieldType,
                    info.GetCustomAttribute<MemberSelectorAttribute>()?.SelectorType));
    }
}