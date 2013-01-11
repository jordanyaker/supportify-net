namespace Supportify {
    /// <summary>
    /// A class for managing the access credentials necessary to retrieve data from the Supportify API.
    /// </summary>
    public interface ISupportifyCredentials {
        /// <summary>
        /// Gets the authentication key for the API Account being accessed.
        /// </summary>
        string ApiKey { get; }
        /// <summary>
        /// Gets the authentication key for the API Application being accessed.
        /// </summary>
        string AppKey { get; }
    }
}
