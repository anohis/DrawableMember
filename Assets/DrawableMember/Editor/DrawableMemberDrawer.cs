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

        public DrawableMemberDrawer(IMemberDrawer[] drawers)
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

    public class MemberDrawerFactory
    {
        private readonly MethodFactory _methodFactory;
        private readonly PropertyFactory _propertyFactory;
        private readonly FieldFactory _fieldFactory;

        public MemberDrawerFactory()
        {
            var variableFactory = new VariableFactory(
                new(new()),
                new(this));

            _methodFactory = new(variableFactory);
            _propertyFactory = new(variableFactory);
            _fieldFactory = new(variableFactory);
        }

        public IMemberDrawer[] Create(Type type)
            => Create(new DrawableMemberSelector(type));

        public IMemberDrawer[] Create(IMemberSelector memberSelector)
            => Create(
                memberSelector.Fields,
                memberSelector.Properties,
                memberSelector.Methods);

        public IMemberDrawer[] Create(
            IEnumerable<FieldInfo> fields,
            IEnumerable<PropertyInfo> properties,
            IEnumerable<MethodInfo> methods)
            => fields
                .Select(_fieldFactory.Create)
                .Cast<IMemberDrawer>()
                .Concat(properties
                    .Select(_propertyFactory.Create)
                    .Cast<IMemberDrawer>())
                .Concat(methods
                    .Select(_methodFactory.Create)
                    .Cast<IMemberDrawer>())
                .ToArray();
    }

    internal sealed class DrawableMemberSelector : IMemberSelector
    {
        private readonly FieldInfo[] _fields;
        private readonly PropertyInfo[] _properties;
        private readonly MethodInfo[] _methods;

        FieldInfo[] IMemberSelector.Fields => _fields;
        PropertyInfo[] IMemberSelector.Properties => _properties;
        MethodInfo[] IMemberSelector.Methods => _methods;

        public DrawableMemberSelector(Type type)
        {
            var attributeType = typeof(DrawableAttribute);
            var bindingFlags =
                BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic;

            _fields = type
                .GetFields(bindingFlags)
                .Where(field => field.IsDefined(attributeType))
                .ToArray();
            _properties = type
                .GetProperties(bindingFlags)
                .Where(property => property.IsDefined(attributeType))
                .ToArray();
            _methods = type
                .GetMethods(bindingFlags)
                .Where(method => method.IsDefined(attributeType))
                .ToArray();
        }
    }

    internal sealed class PublicFieldAndPropertySelector : IMemberSelector
    {
        private readonly FieldInfo[] _fields;
        private readonly PropertyInfo[] _properties;

        FieldInfo[] IMemberSelector.Fields => _fields;
        PropertyInfo[] IMemberSelector.Properties => _properties;
        MethodInfo[] IMemberSelector.Methods { get; } = Array.Empty<MethodInfo>();

        public PublicFieldAndPropertySelector(Type type)
        {
            var bindingFlags =
                BindingFlags.Instance
                    | BindingFlags.Public;

            _fields = type
                .GetFields(bindingFlags)
                .ToArray();
            _properties = type
                .GetProperties(bindingFlags)
                .ToArray();
        }
    }
}