namespace Supportify.Help {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class which contains all of the data relating to a Frequently Asked Question (FAQ).
    /// </summary>
    [Serializable]
    public class Faq {
        /// <summary>
        /// Gets the unique identifier of the <see cref="T:Supportify.Faq"/>.
        /// </summary>
        public long Id { get; internal set; }
        /// <summary>
        /// Gets question that the <see cref="T:Supportify.Faq"/> entry covers.
        /// </summary>
        public string Question { get; internal set; }
        /// <summary>
        /// Gets the most common answer to the question that <see cref="T:Supportify.Faq"/> entry deals with.
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// Gets the collection of <see cref="T:Supportify.Category"/> entries that have been specified for the <see cref="T:Supportify.Faq"/>.
        /// </summary>
        public IEnumerable<Category> Categories { get; set; }
        /// <summary>
        /// Gets the collection of meta-data <see cref="T:Supportify.Tag"/> objects that have been specified for the <see cref="T:Supportify.Faq"/>.
        /// </summary>
        public IEnumerable<Category> Tags { get; set; }
    }
}
