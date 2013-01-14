namespace Supportify.Help {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class SupportifyException : Exception {
        public SupportifyException() { }
        public SupportifyException(string message) : base(message) { }
        public SupportifyException(string message, Exception inner) : base(message, inner) { }
        protected SupportifyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
