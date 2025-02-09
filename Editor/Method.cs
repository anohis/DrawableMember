namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class Method : IMemberDrawer
    {
        private readonly string _name;
        private readonly MethodInfo _method;
        private readonly Parameter[] _parameters;

        private bool _isFoldoutExpanded;

        public Method(
            string name,
            MethodInfo method,
            Parameter[] parameters)
        {
            _name = name;
            _method = method;
            _parameters = parameters;
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
                    foreach (var parameter in _parameters)
                    {
                        parameter.Draw();
                    }

                    if (GUILayout.Button("Invoke"))
                    {
                        _method.Invoke(
                            target,
                            _parameters
                                .Select(parameter => parameter.Value)
                                .ToArray());
                    }
                }
            }
        }
    }

    internal class MethodFactory
    {
        private readonly ParameterFactory _parameterFactory;

        public MethodFactory(ParameterFactory parameterFactory)
        {
            _parameterFactory = parameterFactory;
        }

        public Method Create(MethodInfo info)
            => new(
                info.GetCustomAttribute<DrawableNameAttribute>()
                    ?.Name
                    ?? info.Name,
                info,
                info.GetParameters()
                    .Select(_parameterFactory.Create)
                    .ToArray());
    }
}