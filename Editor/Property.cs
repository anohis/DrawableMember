namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class Property : IMemberDrawer
    {
        private readonly string _name;
        private readonly PropertyInfo _property;
        private readonly Parameter _getter;
        private readonly Parameter _setter;

        private bool _isFoldoutExpanded;

        public Property(
            string name,
            PropertyInfo property,
            Parameter getter,
            Parameter setter)
        {
            _name = name;
            _property = property;
            _getter = getter;
            _setter = setter;
        }

        void IMemberDrawer.Draw(object target)
        {
            _isFoldoutExpanded = EditorGUILayout.Foldout(_isFoldoutExpanded, _name);

            if (!_isFoldoutExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space(EditorGUI.indentLevel * 15, false);

                using (new EditorGUI.IndentLevelScope(-1))
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
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
                info.GetCustomAttribute<DrawableNameAttribute>()
                    ?.Name
                    ?? info.Name,
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