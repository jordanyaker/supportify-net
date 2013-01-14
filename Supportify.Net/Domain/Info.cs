namespace Supportify.Help {
    using System;

    /// <summary>
    /// A class for encapsulating the basic details about the API. 
    /// </summary>
    [Serializable]
    public class Info {
        /// <summary>
        /// Gets the current version details about the Supportify API that the library is accessing.
        /// </summary>
        public Supportify Supportify { get; set; }
        /// <summary>
        /// Gets the details about the <see cref="T:Supportify.Application"/> that the requests are being handled for.
        /// </summary>
        public Application Application { get; set; }
    }
}
