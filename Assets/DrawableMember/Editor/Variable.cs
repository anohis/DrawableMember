using DrawableMember.Runtime;
using System;

namespace DrawableMember.Editor
{
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
            Type memberSelectorType = null,
            bool hasDefaultValue = false,
            object defaultValue = null)
        {
            IVariable variable = (memberSelectorType
                   ?.IsInheritsFrom(typeof(IMemberSelector))
                   ?? false)
                ? _dynamicVariableFactory.Create(
                    name,
                    type,
                    Activator.CreateInstance(memberSelectorType) as IMemberSelector)
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