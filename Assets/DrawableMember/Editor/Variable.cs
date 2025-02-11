namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using System.Reflection;

    internal interface IVariable
    {
        object Value { get; set; }

        void Draw();
    }

    internal class VariableFactory
    {
        private readonly ParameterFactory _parameterFactory;
        private readonly DynamicVariableFactory _dynamicVariableFactory;

        public VariableFactory(
            ParameterFactory parameterFactory,
            DynamicVariableFactory dynamicVariableFactory)
        {
            _parameterFactory = parameterFactory;
            _dynamicVariableFactory = dynamicVariableFactory;
        }

        public IVariable Create(
            string name,
            Type type,
            bool hasDefaultValue = false,
            object defaultValue = null)
        {
            var memberSelectorType = type
                .GetCustomAttribute<MemberSelectorAttribute>()
                ?.SelectorType;

            var memberSelector =
                memberSelectorType is not null
                && memberSelectorType.IsInheritsFrom(typeof(IMemberSelector))
                    ? Activator.CreateInstance(memberSelectorType, type) as IMemberSelector
                    : !type.IsUnitySerializable()
                         ? new PublicFieldAndPropertySelector(type)
                         : null;

            IVariable variable = memberSelector is not null
                && memberSelector.Any()
                ? _dynamicVariableFactory.Create(
                    name,
                    type,
                    memberSelector)
                : _parameterFactory.Create(
                    name,
                    type);

            if (hasDefaultValue)
            {
                variable.Value = defaultValue;
            }

            return variable;
        }
    }
}