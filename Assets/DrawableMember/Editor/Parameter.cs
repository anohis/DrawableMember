namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class Parameter : IVariable
    {
        private static readonly GUIContent NOT_SERIALIZABLE_LABEL = new GUIContent("is not serializable");

        private readonly ScriptableObject _target;
        private readonly FieldInfo _valueField;
        private readonly SerializedObject _so;
        private readonly SerializedProperty _sp;
        private readonly GUIContent _label;

        public object Value
        {
            set
            {
                _valueField.SetValue(_target, value);
                _so.Update();
            }
            get
            {
                if (_so.hasModifiedProperties)
                {
                    _so.ApplyModifiedPropertiesWithoutUndo();
                }

                return _valueField.GetValue(_target);
            }
        }

        public Parameter(string name, ScriptableObject target)
        {
            _target = target;
            _valueField = target
                .GetType()
                .GetField("Value", BindingFlags.Public | BindingFlags.Instance);
            _so = new SerializedObject(target);
            _sp = _so.FindProperty("Value");
            _label = new GUIContent(name);
        }

        void IVariable.Draw()
        {
            if (_sp == null)
            {
                EditorGUILayout.LabelField(_label, NOT_SERIALIZABLE_LABEL);
            }
            else
            {
                EditorGUILayout.PropertyField(_sp, _label);
            }
        }
    }

    internal class ParameterFactory
    {
        private readonly ScriptableObjectFactory _scriptableObjectFactory;

        public ParameterFactory(ScriptableObjectFactory scriptableObjectFactory)
        {
            _scriptableObjectFactory = scriptableObjectFactory;
        }

        public Parameter Create(ParameterInfo info)
        {
            var overrideType = info.GetCustomAttribute<TypeAttribute>()?.Type;

            var parameterType =
                (overrideType?.IsInheritsFrom(info.ParameterType)
                    ?? false)
                    && (!info.HasDefaultValue
                        || info.DefaultValue
                            .GetType()
                            .IsInheritsFrom(overrideType))
                ? overrideType
                : info.ParameterType;

            var parameter = Create(
                info.GetCustomAttribute<NameAttribute>()
                    ?.Name
                    ?? info.Name,
                parameterType);

            if (info.HasDefaultValue)
            {
                parameter.Value = info.DefaultValue;
            }

            return parameter;
        }

        public Parameter Create(string name, Type type)
            => new(
                name,
                _scriptableObjectFactory.Create(type));
    }
}