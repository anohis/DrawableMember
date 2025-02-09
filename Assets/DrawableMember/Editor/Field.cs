namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class Field : IMemberDrawer
    {
        private readonly string _name;
        private readonly FieldInfo _field;
        private readonly Parameter _getter;
        private readonly Parameter _setter;

        private bool _isFoldoutExpanded;

        public Field(
            string name,
            FieldInfo field,
            Parameter getter,
            Parameter setter)
        {
            _name = name;
            _field = field;
            _getter = getter;
            _setter = setter;
        }

        public void Draw(object target)
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
                    using (new EditorGUI.DisabledScope(true))
                    {
                        _getter.Value = _field.GetValue(target);
                        _getter.Draw();
                    }

                    _setter.Draw();
                    if (GUILayout.Button("Set Value"))
                    {
                        _field.SetValue(
                           target,
                           _setter.Value);
                    }
                }
            }
        }
    }

    internal class FieldFactory
    {
        private readonly ScriptableObjectFactory _scriptableObjectFactory;

        public FieldFactory(ScriptableObjectFactory scriptableObjectFactory)
        {
            _scriptableObjectFactory = scriptableObjectFactory;
        }

        public Field Create(FieldInfo info)
        {
            var attribute = info.GetCustomAttribute<DrawableTypeAttribute>();

            return new(
                info.GetCustomAttribute<DrawableNameAttribute>()
                    ?.Name
                    ?? info.Name,
                info,
                new(
                  _scriptableObjectFactory.Create(info.FieldType),
                  "current value"),
                new(
                  _scriptableObjectFactory.Create(
                      attribute != null
                          && attribute.Type.IsInheritsFrom(info.FieldType)
                          ? attribute.Type
                          : info.FieldType),
                  "new value"));
        }
    }
}