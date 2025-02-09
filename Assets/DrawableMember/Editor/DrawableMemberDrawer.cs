namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public interface IMemberDrawer
    {
        void Draw(object target);
    }

    public class DrawableMemberDrawer : IMemberDrawer
    {
        private readonly IMemberDrawer[] _drawers;

        private bool _isFoldoutExpanded;

        internal DrawableMemberDrawer(IMemberDrawer[] drawers)
        {
            _drawers = drawers;
        }

        void IMemberDrawer.Draw(object target)
        {
            if (!_drawers.Any())
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
                foreach (var drawer in _drawers)
                {
                    drawer.Draw(target);
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
        private readonly FieldFactory _fieldFactory;

        public DrawableMemberDrawerFactory()
        {
            _scriptableObjectFactory = new();
            _parameterFactory = new(_scriptableObjectFactory);
            _methodFactory = new(_parameterFactory);
            _propertyFactory = new(_scriptableObjectFactory);
            _fieldFactory = new(_scriptableObjectFactory);
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
                type.GetFields(bindingFlags)
                    .Where(field => field.IsDefined(attributeType)),
                type.GetProperties(bindingFlags)
                    .Where(property => property.IsDefined(attributeType)),
                type.GetMethods(bindingFlags)
                    .Where(method => method.IsDefined(attributeType)));
        }

        public DrawableMemberDrawer Create(
            IEnumerable<FieldInfo> fields,
            IEnumerable<PropertyInfo> properties,
            IEnumerable<MethodInfo> methods)
            => new(
                fields
                    .Select(_fieldFactory.Create)
                    .Cast<IMemberDrawer>()
                    .Concat(properties
                        .Select(_propertyFactory.Create)
                        .Cast<IMemberDrawer>())
                    .Concat(methods
                        .Select(_methodFactory.Create)
                        .Cast<IMemberDrawer>())
                    .ToArray());
    }
}