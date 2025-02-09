namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class DrawableMemberDrawer
    {
        private readonly Property[] _properties;
        private readonly Method[] _methods;

        private bool _isFoldoutExpanded;

        internal DrawableMemberDrawer(
            Property[] properties,
            Method[] methods)
        {
            _properties = properties;
            _methods = methods;
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

    public class DrawableMemberDrawerFactory
    {
        private readonly ScriptableObjectFactory _scriptableObjectFactory;
        private readonly ParameterFactory _parameterFactory;
        private readonly MethodFactory _methodFactory;
        private readonly PropertyFactory _propertyFactory;

        public DrawableMemberDrawerFactory()
        {
            _scriptableObjectFactory = new();
            _parameterFactory = new(_scriptableObjectFactory);
            _methodFactory = new(_parameterFactory);
            _propertyFactory = new(_scriptableObjectFactory);
        }

        public DrawableMemberDrawer Create(Type type)
        {
            var attributeType = typeof(DrawableAttribute);

            var bindingFlags =
                BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic;

            return Create(
                type.GetProperties(bindingFlags)
                    .Where(property => property.IsDefined(attributeType)),
                type.GetMethods(bindingFlags)
                    .Where(property => property.IsDefined(attributeType)));
        }

        public DrawableMemberDrawer Create(
            IEnumerable<PropertyInfo> properties,
            IEnumerable<MethodInfo> methods)
            => new(
                properties
                    .Select(_propertyFactory.Create)
                    .ToArray(),
                methods
                    .Select(_methodFactory.Create)
                    .ToArray());
    }
}