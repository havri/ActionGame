using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ActionGame
{
    class NoSpaceForInterfaceException : Exception
    {
        public NoSpaceForInterfaceException()
        {
            
        }
        public NoSpaceForInterfaceException(string message)
            : base(message)
        {
            
        }
        public NoSpaceForInterfaceException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected NoSpaceForInterfaceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
