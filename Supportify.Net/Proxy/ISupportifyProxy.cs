namespace Supportify {
    using System.Collections.Generic;

    /// <summary>
    /// The proxy class for interfacing with the Supportify REST API.
    /// </summary>
    public interface ISupportifyProxy {
        /// <summary>
        /// Retrieves the <see cref="T:Supportify.Category"/> with an ID that matches the supplied value.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="T:Supportify.Category"/> to be retrieved.</param>
        /// <returns>The <see cref="T:Supportify.Category"/> whose ID matches the supplied value; otherwise, null.</returns>
        Category GetCategory(long id);
        /// <summary>
        /// Retrieves the collection of <see cref="T:Supportify.Category"/> entries that match the supplied parameters.
        /// </summary>
        /// <returns>The collection of <see cref="T:Supportify.Category"/> entries that match the supplied parameters.</returns>
        IEnumerable<Category> GetCategories(int limit = 0, int skip = 0, CategoryProperties sortProperty = 0, SortOrders sortOrder = 0);
        /// <summary>
        /// Retrieves the <see cref="T:Supportify.Faq"/> with an ID that matches the supplied value.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="T:Supportify.Faq"/> to be retrieved.</param>
        /// <returns>The <see cref="T:Supportify.Faq"/> whose ID matches the supplied value; otherwise, null.</returns>
        Faq GetFaq(long id);
        /// <summary>
        /// Retrieves the collection of <see cref="T:Supportify.Faq"/> entries that match the supplied parameters.
        /// </summary>
        /// <returns>The collection of <see cref="T:Supportify.Faq"/> entries that match the supplied parameters.</returns>
        IEnumerable<Faq> GetFaqs(int limit = 0, int skip = 0, string[] categories = null, string[] tags = null, FaqProperties sortProperty = 0, SortOrders sortOrder = 0);
        /// <summary>
        /// Gets the current details about the API interface being used by the proxy.
        /// </summary>
        /// <returns>A new <see cref="T:Supportify.Info"/> instance containing the basic information returned by the API.s</returns>
        Info GetInfo();
        /// <summary>
        /// Retrieves the <see cref="T:Supportify.Tag"/> with an ID that matches the supplied value.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="T:Supportify.Tag"/> to be retrieved.</param>
        /// <returns>The <see cref="T:Supportify.Tag"/> whose ID matches the supplied value; otherwise, null.</returns>
        Tag GetTag(long id);
        /// <summary>
        /// Retrieves the collection of <see cref="T:Supportify.Tag"/> entries that match the supplied parameters.
        /// </summary>
        /// <returns>The collection of <see cref="T:Supportify.Tag"/> entries that match the supplied parameters.</returns>
        IEnumerable<Tag> GetTags(int limit = 0, int skip = 0, TagProperties sortProperty = 0, SortOrders sortOrder = 0);
    }
}
