namespace Supportify.Help {
    using System;

    /// <summary>
    /// A meta-data class representing the details needed to categorize help and support content.
    /// </summary>
    [Serializable]
    public class Category {
        /// <summary>
        /// Gets the unique identifier for the <see cref="T:Supportify.Category"/>.
        /// </summary>
        public long Id { get; internal set; }
        /// <summary>
        /// Gets the name of the <see cref="T:Supportify.Category"/>.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Gets a brief description of the <see cref="T:Supportify.Category"/>.
        /// </summary>
        public string Description { get; internal set; }
    }
}
