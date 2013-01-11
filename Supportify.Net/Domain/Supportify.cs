namespace Supportify {
    using System;

    /// <summary>
    /// A class for containing the basic details about the Supportify API.
    /// </summary>
    [Serializable]
    public class Supportify {
        /// <summary>
        /// Gets the current version of the API that the library is accessing.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Gets the name of the awesome people that are making Supportify happen.
        /// </summary>
        public string Company { get; set; }
    }
}
