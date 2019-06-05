using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace JPLC
{
    public class JPLCProperty<T>
    {
        #region [Public Properties]
        public string Name { get; private set; }
        public T Value { get; set; }
        public JPLC_BASE Parent { get; private set; }
        public double Offset { get; private set; }
        public double Address { 
            get{
                if (Parent != null)
                {
                    return Parent.Address + Offset;
                }
                else
                {
                    return Offset;
                }
            }
        }
        #endregion

        #region [Constructor]
        public JPLCProperty(string name = "", JPLC_BASE parent = null, double offset = 0)
        {
            Name = name;
            Parent = parent;
            Offset = offset;
        }
        public JPLCProperty(T value, string name = "", JPLC_BASE parent = null, double offset = 0)
        {
            Name = name;
            Parent = parent;
            Offset = offset;
            Value = value;
        }
        #endregion

        #region [Methods]
        public void Write()
        {
            throw new NotImplementedException();
        }

        public T Read()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
