namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class DrawableMemberDrawer
    {
        private readonly Method[] _methods;
        private readonly Property[] _properties;

        private bool _isFoldoutExpanded;

        public DrawableMemberDrawer(Type type)
        {
            var scriptableObjectFactory = new ScriptableObjectFactory();
            var parameterFactory = new ParameterFactory(scriptableObjectFactory);
            var methodFactory = new MethodFactory(parameterFactory);
            var propertyFactory = new PropertyFactory(scriptableObjectFactory);

            var attributeType = typeof(DrawableAttribute);

            var bindingFlags =
                BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic;

            _methods = type
                .GetMethods(bindingFlags)
                .Where(method => method.IsDefined(attributeType))
                .Select(methodFactory.Create)
                .ToArray();

            _properties = type
                .GetProperties(bindingFlags)
                .Where(property => property.IsDefined(attributeType))
                .Select(propertyFactory.Create)
                .ToArray();
        }

        public void Draw(object target)
        {
            if (!_methods.Any() && !_properties.Any())
            {
                return;
            }

            var style = new GUIStyle(EditorStyles.foldout);
            style.font = EditorStyles.boldFont;
            _isFoldoutExpanded = EditorGUILayout.Foldout(_isFoldoutExpanded, "Drawable Members", style);

            if (!_isFoldoutExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var property in _properties)
                {
                    property.Draw(target);
                }

                foreach (var method in _methods)
                {
                    method.Draw(target);
                }
            }
        }
    }
}