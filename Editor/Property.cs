namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class Property
    {
        private readonly PropertyInfo _property;
        private readonly Parameter _getter;
        private readonly Parameter _setter;

        private bool _isFoldoutExpanded;

        public Property(
            PropertyInfo property,
            Parameter getter,
            Parameter setter)
        {
            _property = property;
            _getter = getter;
            _setter = setter;
        }

        public void Draw(object target)
        {
            _isFoldoutExpanded = EditorGUILayout.Foldout(_isFoldoutExpanded, _property.Name);

            if (!_isFoldoutExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                if (_getter != null)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        _getter.Value = _property.GetValue(target);
                        _getter.Draw();
                    }
                }

                if (_setter != null)
                {
                    _setter.Draw();
                    if (GUILayout.Button("Set Value"))
                    {
                        _property.SetValue(
                           target,
                           _setter.Value);
                    }
                }
            }
        }
    }

    internal class PropertyFactory
    {
        private readonly ScriptableObjectFactory _scriptableObjectFactory;

        public PropertyFactory(ScriptableObjectFactory scriptableObjectFactory)
        {
            _scriptableObjectFactory = scriptableObjectFactory;
        }

        public Property Create(PropertyInfo info)
        {
            var attribute = info.GetCustomAttribute<DrawableTypeAttribute>();

            return new(
                info,
                info.CanRead
                    ? new(
                        _scriptableObjectFactory.Create(info.PropertyType),
                        "current value")
                    : null,
                info.CanWrite
                    ? new(
                        _scriptableObjectFactory.Create(
                            attribute != null
                                && attribute.Type.IsInheritsFrom(info.PropertyType)
                                ? attribute.Type
                                : info.PropertyType),
                        "new value")
                    : null);
        }
    }
}