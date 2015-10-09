using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTT.Utilities
{
    public class ReturnValue<T>
    {
        private T _retValue;
        private string _msg;
        public ReturnValue(T inA, string inMgs)
        {
            _retValue = inA;
            _msg = inMgs;
        }

        public T RetValue
        {
            get { return _retValue; }
            set { _retValue = value; }
        }
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

    }
}
