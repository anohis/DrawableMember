namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class DynamicVariable : IVariable
    {
        private static readonly GUIContent NOT_SERIALIZABLE_LABEL = new GUIContent("value is null");

        private readonly string _name;
        private readonly Type _type;
        private readonly IMemberDrawer[] _memberDrawers;

        private bool _isFoldoutExpanded;

        public object Value { get; set; }

        public DynamicVariable(
            string name,
            Type type,
            IMemberDrawer[] memberDrawers)
        {
            _name = name;
            _type = type;
            _memberDrawers = memberDrawers;
        }

        void IVariable.Draw()
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
                    if (Value == null)
                    {
                        EditorGUILayout.LabelField(NOT_SERIALIZABLE_LABEL);
                        if (GUILayout.Button("new instance"))
                        {
                            Value = _type.CreateInstance();
                        }
                        return;
                    }

                    foreach (var drawer in _memberDrawers)
                    {
                        drawer.Draw(Value);
                    }
                }
            }
        }
    }

    internal class DynamicVariableFactory
    {
        private readonly MemberDrawerFactory _memberDrawerFactory;

        public DynamicVariableFactory(MemberDrawerFactory memberDrawerFactory)
        {
            _memberDrawerFactory = memberDrawerFactory;
        }

        public DynamicVariable Create(
            string name,
            Type type,
            IMemberSelector memberSelector)
            => new(
                name,
                type,
                _memberDrawerFactory.Create(memberSelector));
    }
}