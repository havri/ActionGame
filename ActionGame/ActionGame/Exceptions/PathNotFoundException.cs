using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ActionGame.Exceptions
{
    public class PathNotFoundException : Exception
    {
        public PathNotFoundException()
        {
            
        }
        public PathNotFoundException(string message)
            : base(message)
        {
            
        }
        public PathNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected PathNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            
        }

    }
}
