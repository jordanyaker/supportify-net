namespace Supportify.Help {
    using System;

    [Serializable]
    public class SupportifyAuthenticationException : SupportifyException {
        public SupportifyAuthenticationException() { }
        public SupportifyAuthenticationException(string message) : base(message) { }
        public SupportifyAuthenticationException(string message, Exception inner) : base(message, inner) { }
        protected SupportifyAuthenticationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
