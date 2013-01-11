namespace Supportify {
    using System;

    /// <summary>
    /// A class used to encapsulate the details about an application within Supportify.
    /// </summary>
    [Serializable]
    public class Application {
        /// <summary>
        /// Gets the unique identifier of the <see cref="T:Supportify.Application"/>.
        /// </summary>
        public long Id { get; internal set; }
        /// <summary>
        /// Gets the name given to the <see cref="T:Supportify.Application"/>.
        /// </summary>
        public string Name { get; internal set; }
    }
}
