namespace Supportify {
    /// <summary>
    /// A class for managing the access credentials necessary to retrieve data from the Supportify API.
    /// </summary>
    public class SupportifyCredentials : ISupportifyCredentials {
        /// <summary>
        /// Default constructor for the <see cref="T:Supportify.Security.Credentials"/> class.
        /// </summary>
        /// <param name="apiKey">The authentication key for the API account.</param>
        /// <param name="appKey">The authentication key for the API application</param>
        public SupportifyCredentials(string apiKey, string appKey) {
            this.ApiKey = apiKey;
            this.AppKey = appKey;
        }
        /// <summary>
        /// Gets the authentication key for the API Account being accessed.
        /// </summary>
        public string ApiKey { get; private set; }
        /// <summary>
        /// Gets the authentication key for the API Application being accessed.
        /// </summary>
        public string AppKey { get; private set; }
    }
}
