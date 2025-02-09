namespace DrawableMember.Editor
{
    using DrawableMember.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEditor;
    using UnityEngine;

    internal class ScriptableObjectFactory
    {
        private static readonly string NAME = "DrawableMember";
        private static readonly AssemblyBuilder ASSEMMBLY_BUILDER = AssemblyBuilder.DefineDynamicAssembly(new(NAME), AssemblyBuilderAccess.Run);
        private static readonly ModuleBuilder MODULE_BUILDER = ASSEMMBLY_BUILDER.DefineDynamicModule(NAME);

        private Dictionary<Type, Type> _typeMap;

        public ScriptableObjectFactory()
        {
            _typeMap = new Dictionary<Type, Type>();
        }

        public ScriptableObject Create(Type type)
            => ScriptableObject
                .CreateInstance(
                    _GetScriptableObjectType(type));

        private Type _GetScriptableObjectType(Type type)
        {
            if (!_typeMap.TryGetValue(type, out var soType))
            {
                soType = _CreateScriptableObjectType(type);
                _typeMap.Add(type, soType);
            }
            return soType;
        }

        private Type _CreateScriptableObjectType(Type type)
            => MODULE_BUILDER
                .DefineType(
                    GUID.Generate().ToString(),
                    TypeAttributes.NotPublic,
                    typeof(ScriptableObject<>).MakeGenericType(type))
                .CreateType();
    }
}