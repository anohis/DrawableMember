namespace DrawableMember.Sample
{
    using DrawableMember.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class Sample : MonoBehaviour
    {
        private int _intProp;

        [Drawable]
        [SerializeField]
        private int _number;

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
        [DrawableName("Int Array Prop")]
        public int[] IntArrayProp { get; private set; }

        [Drawable]
        [DrawableType(typeof(int[]))]
        public IEnumerable<int> NonSerializedIntArrayProp
        {
            get => IntArrayProp;
            set => IntArrayProp = value.ToArray();
        }

        [Drawable]
        [DrawableName("Test")]
        public void Method()
        {
            Debug.Log($"invoke Method");
        }

        [Drawable]
        public void Method(
            string arg0 = "asa",
            [DrawableName("is great")] bool arg1 = true)
        {
            Debug.Log($"invoke Method with {arg0}, {arg1}");
        }

        [Drawable]
        public void Method(
            [DrawableType(typeof(SerializableClass[]))] IEnumerable<SerializableClass> arg0,
            IEnumerable<NonSerializedClass> arg1,
            SerializableClass[] arg2,
            IEnumerable<NonSerializedClass> arg3,
            NonSerializedClass[] arg4)
        {
            Debug.Log($"invoke Method with {arg0}, {arg1}, {arg2}, {arg3}, {arg4}");
        }
    }

    [Serializable]
    internal class SerializableClass
    {
        public int A;
        public int B;
    }

    internal class NonSerializedClass
    {
        public int A;
        public int B;
    }
}