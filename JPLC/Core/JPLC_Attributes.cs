using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JPLC
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order_;
        public OrderAttribute(int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class PLCStringAttribute : Attribute
    {
        private readonly int _StringSize;
        public PLCStringAttribute(int stringSize)
        {
            _StringSize = stringSize;
        }

        public int PLCString { get { return _StringSize; } }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ArraySizeAttribute : Attribute
    {
        private readonly int _ArraySize;
        public ArraySizeAttribute(int arraySize = 1)
        {
            _ArraySize = arraySize;
        }

        public int ArraySize { get { return _ArraySize; } }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DoubleArraySizeAttribute : Attribute
    {
        public readonly int Array1Size;
        public readonly int Array2Size;
        public DoubleArraySizeAttribute(int array1Size = 1, int array2Size = 1)
        {
            Array1Size = array1Size;
            Array2Size = array2Size;
        }

    }
}
