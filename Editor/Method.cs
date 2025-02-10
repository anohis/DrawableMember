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
        private readonly IVariable[] _parameters;

        private bool _isFoldoutExpanded;

        public Method(
            string name,
            MethodInfo method,
            IVariable[] parameters)
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
        private readonly VariableFactory _variableFactory;

        public MethodFactory(VariableFactory variableFactory)
        {
            _variableFactory = variableFactory;
        }

        public Method Create(MethodInfo info)
            => new(
                info.GetCustomAttribute<NameAttribute>()
                    ?.Name
                    ?? info.Name,
                info,
                info.GetParameters()
                    .Select(parameter =>
                    {
                        var overrideType = parameter.GetCustomAttribute<TypeAttribute>()?.Type;

                        var parameterType =
                            (overrideType?.IsInheritsFrom(parameter.ParameterType)
                                ?? false)
                                && (!parameter.HasDefaultValue
                                    || parameter.DefaultValue
                                        .GetType()
                                        .IsInheritsFrom(overrideType))
                            ? overrideType
                            : parameter.ParameterType;

                        return _variableFactory
                            .Create(
                                parameter.GetCustomAttribute<NameAttribute>()
                                    ?.Name
                                    ?? parameter.Name,
                                parameterType,
                                parameter.GetCustomAttribute<MemberSelectorAttribute>()?.SelectorType,
                                parameter.HasDefaultValue,
                                parameter.DefaultValue);
                    })
                    .ToArray());
    }
}