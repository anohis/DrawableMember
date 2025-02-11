namespace DrawableMember.Sample
{
    using DrawableMember.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    internal class Sample : MonoBehaviour
    {
        private int _intProp;

        #region Field
        [Drawable]
        [Name("ID")]
        [SerializeField]
        private int _number;

        [Drawable]
        private int _nonSerializedNumber;

        [Drawable]
        [MemberSelector(typeof(PublicFieldsSelector<NonSerializedClass>))]
        private NonSerializedClass _nonSerializedClass;
        #endregion

        #region Property
        [Drawable]
        public int IntProp
        {
            get => _intProp;
            set
            {
                Debug.Log($"IntProp changed: {_intProp} -> {value}");
                _intProp = value;
            }
        }

        [Drawable]
        public int ReadOnlyIntProp => _intProp;

        [Drawable]
        public int WriteOnlyIntProp
        {
            set => _intProp = value;
        }

        [Drawable]
        [Name("Int Array Prop")]
        public int[] IntArrayProp { get; private set; }

        [Drawable]
        public IEnumerable<int> NonSerializedIntArrayProp
        {
            get => IntArrayProp;
            set => IntArrayProp = value.ToArray();
        }

        [Drawable]
        [MemberSelector(typeof(PublicFieldsSelector<NonSerializedClass>))]
        private NonSerializedClass NonSerializedClassProp
        {
            get => _nonSerializedClass;
            set => _nonSerializedClass = value;
        }
        #endregion

        #region Method
        [Drawable]
        [Name("Show _nonSerializedClass")]
        public void Method()
        {
            Debug.Log(_nonSerializedClass);
        }

        [Drawable]
        public void Method(
            string arg0 = "asa",
            [Name("is great")] bool arg1 = true)
        {
            Debug.Log($"invoke Method with arg0<{arg0}>, arg1<{arg1}>");
        }

        [Drawable]
        public void Method(
            [Type(typeof(SerializableClass[]))]
            IEnumerable<SerializableClass> arg0,
            [MemberSelector(typeof(PublicFieldsSelector<NonSerializedClass>))]
            NonSerializedClass arg1)
        {
            Debug.Log($"invoke Method with arg0<{arg0}>, arg1<{arg1}>");
        }
        #endregion
    }

    [Serializable]
    internal class SerializableClass
    {
        public int A;
        public int B;

        public override string ToString()
            => $"A: {A}, B: {B}";
    }

    internal class NonSerializedClass
    {
        internal class Data
        {
            public string Name;
        }

        public int A;
        public string B;

        [MemberSelector(typeof(PublicFieldsSelector<Data>))]
        public Data CustomData;

        public NonSerializedClass(int a, string b = "asa")
        {
            A = a;
            B = b;
        }

        public override string ToString()
            => $"A: {A}, B: {B}, Name: {CustomData?.Name}";
    }

    internal class PublicFieldsSelector<T> : IMemberSelector
    {
        private readonly FieldInfo[] _fields;

        FieldInfo[] IMemberSelector.Fields => _fields;
        PropertyInfo[] IMemberSelector.Properties { get; } = Array.Empty<PropertyInfo>();
        MethodInfo[] IMemberSelector.Methods { get; } = Array.Empty<MethodInfo>();

        public PublicFieldsSelector()
        {
            _fields = typeof(T)
                .GetFields(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}