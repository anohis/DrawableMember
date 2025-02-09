namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal class Parameter
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

        public Parameter(ScriptableObject target, string name)
        {
            _target = target;
            _valueField = target
                .GetType()
                .GetField("Value", BindingFlags.Public | BindingFlags.Instance);
            _so = new SerializedObject(target);
            _sp = _so.FindProperty("Value");
            _label = new GUIContent(name);
        }

        public void Draw()
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
            var attribute = info.GetCustomAttribute<DrawableTypeAttribute>();

            var parameterType =
                attribute != null
                    && attribute.Type.IsInheritsFrom(info.ParameterType)
                    && (!info.HasDefaultValue
                        || info.DefaultValue
                            .GetType()
                            .IsInheritsFrom(attribute.Type))
                ? attribute.Type
                : info.ParameterType;

            var parameter = new Parameter(
                _scriptableObjectFactory.Create(parameterType),
                info.GetCustomAttribute<DrawableNameAttribute>()
                    ?.Name
                    ?? info.Name);

            if (info.HasDefaultValue)
            {
                parameter.Value = info.DefaultValue;
            }

            return parameter;
        }
    }
}