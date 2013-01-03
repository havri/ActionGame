using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionGame.Exceptions
{
    class PathGraphNotConnectedException : Exception
    {
        public PathGraphNotConnectedException()
        {
            
        }
        public PathGraphNotConnectedException(string message)
            : base(message)
        {
            
        }
        public PathGraphNotConnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected PathGraphNotConnectedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
