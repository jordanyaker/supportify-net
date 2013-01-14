namespace Supportify.Help {
    using System;

    /// <summary>
    /// A class representing meta-data details that can be used to describe help and support content.
    /// </summary>
    [Serializable]
    public class Tag {
        /// <summary>
        /// Gets the unique identifier for the <see cref="T:Supportify.Tag"/>.
        /// </summary>
        public long Id { get; internal set; }
        /// <summary>
        /// Gets the name of the <see cref="T:Supportify.Tag"/>.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Gets a brief description of the <see cref="T:Supportify.Tag"/>.
        /// </summary>
        public string Description { get; internal set; }
    }
}
