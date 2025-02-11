namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Reflection;
    using UnityEditor;

    internal class Property : IMemberDrawer
    {
        private readonly PropertyInfo _property;
        private readonly IVariable _value;

        public Property(
            PropertyInfo property,
            IVariable value)
        {
            _property = property;
            _value = value;
        }

        void IMemberDrawer.Draw(object target)
        {
            if (_property.CanRead)
            {
                _value.Value = _property.GetValue(target);
            }

            if (_property.CanWrite)
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    _value.Draw();
                    if (scope.changed)
                    {
                        _property.SetValue(target, _value.Value);
                    }
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    _value.Draw();
                }
            }
        }
    }

    internal class PropertyFactory
    {
        private readonly VariableFactory _variableFactory;

        public PropertyFactory(VariableFactory variableFactory)
        {
            _variableFactory = variableFactory;
        }

        public Property Create(PropertyInfo info)
            => new(
                info,
                _variableFactory.Create(
                    info.GetCustomAttribute<NameAttribute>()
                        ?.Name
                        ?? info.Name,
                    info.PropertyType));
    }
}