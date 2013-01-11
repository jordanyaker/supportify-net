namespace Supportify {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RestSharp;
    using System.Net;
    using ServiceStack.Text;
    using System.Web;

    /// <summary>
    /// The proxy class for interfacing with the Supportify REST API.
    /// </summary>
    public class SupportifyProxy : ISupportifyProxy, IDisposable {
        static readonly string API_BASE_URL = "https://api.supportify.io/{0}/";
        static readonly string AUTHENTICATION_EXCEPTION = "An authentication error occurred while attempting to {0}.";
        static readonly string UNEXPECTED_EXCEPTION = "An unexpected exception occurred while attempting to {0}.";
        bool _disposed = false;
        IDictionary<Type, object> _serializers;
        IRestClient _client;

        /// <summary>
        /// Default constructor for the <see cref="T:Supportify.SupportifyProxy"/> class.
        /// </summary>
        /// <param name="credentials">The account/application credentials that will be used for the duration of the request.</param>
        /// <param name="version">The version of the API to be used for the call.</param>
        public SupportifyProxy(ISupportifyCredentials credentials, VersionTypes version = VersionTypes.V1) {
            this.Credentials = credentials;
            this.Version = version;

            _serializers = new Dictionary<Type, object>();
        }
        /// <summary>
        /// The default destructor.
        /// </summary>
        ~SupportifyProxy() {
            Dispose(false);
        }

        /// <summary>
        /// Gets the current <see cref="T:Supportify.ISupportifyCredentials"/> for use during the proxy session.
        /// </summary>
        public ISupportifyCredentials Credentials { get; private set; }
        /// <summary>
        /// Gets the current REST client used for interactions with the Supportify API.
        /// </summary>
        public IRestClient Client {
            get {
                if (_client == null) {
                    string url = string.Format(API_BASE_URL, Version);

                    _client = new RestClient(url);
                    _client.AddDefaultHeader("X-Supportify-ApiKey", Credentials.ApiKey);
                    _client.AddDefaultHeader("X-Supportify-AppKey", Credentials.AppKey);
                }

                return _client;
            }
            internal set {
                _client = value;
            }
        }
        /// <summary>
        /// Gets the current version of the API that the proxy is interfacing with
        /// </summary>
        public VersionTypes Version { get; private set; }

        /// <summary>
        /// Implementation of the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// The internal implementation of the <see cref="IDisposable.Dispose"/> method.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing) {
            if (!_disposed && disposing) {
                if (_client != null) {
                    _client = null;
                }
            }

            _disposed = true;
        }
        /// <summary>
        /// Retrieves the <see cref="T:Supportify.Category"/> with an ID that matches the supplied value.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="T:Supportify.Category"/> to be retrieved.</param>
        /// <returns>The <see cref="T:Supportify.Category"/> whose ID matches the supplied value; otherwise, null.</returns>
        public Category GetCategory(long id) {
            string resource = string.Format("categories/{0}", id);
            string description = "retrieve the specified Category";
            string key = "category";

            var category = this.MakeRequest<Category>(resource, key, Method.GET, description);
            return category;
        }
        /// <summary>
        /// Retrieves the collection of <see cref="T:Supportify.Category"/> entries that match the supplied parameters.
        /// </summary>
        /// <returns>The collection of <see cref="T:Supportify.Category"/> entries that match the supplied parameters.</returns>
        public IEnumerable<Category> GetCategories(int limit = 0, int skip = 0, CategoryProperties sortProperty = 0, SortOrders sortOrder = 0) {
            string resource = "categories";
            string description = "retrieving Category meta-data for the application";
            string key = "categories";

            var parameters = new Dictionary<string, string>();
            if (limit > 0) {
                parameters["limit"] = limit.ToString();
            }
            if (skip > 0) {
                parameters["skip"] = skip.ToString();
            }
            if (sortProperty != 0) {
                parameters["sort"] = sortProperty.ToString();
                if (sortOrder != 0) {
                    parameters["sort"] += ("-" + (sortOrder == SortOrders.Ascending ? "asc" : "desc"));
                }
            } else if (sortOrder != 0) {
                parameters["sort"] = "name-" + (sortOrder == SortOrders.Ascending ? "asc" : "desc");
            }

            if (parameters.Count > 0) {
                var queryString = string.Join("&", parameters.Select(x =>
                    string.Format("{0}={1}", x.Key, x.Value)));
                resource += ("?" + queryString);
            }

            var categories = this.MakeRequest<IEnumerable<Category>>(resource, key, Method.GET, description);
            return categories;
        }
        /// <summary>
        /// Retrieves the <see cref="T:Supportify.Faq"/> with an ID that matches the supplied value.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="T:Supportify.Faq"/> to be retrieved.</param>
        /// <returns>The <see cref="T:Supportify.Faq"/> whose ID matches the supplied value; otherwise, null.</returns>
        public Faq GetFaq(long id) {
            string resource = string.Format("faqs/{0}", id);
            string description = "retrieve the specified FAQ";
            string key = "faq";

            var faq = this.MakeRequest<Faq>(resource, key, Method.GET, description);
            return faq;
        }
        /// <summary>
        /// Retrieves the collection of <see cref="T:Supportify.Faq"/> entries that match the supplied parameters.
        /// </summary>
        /// <returns>The collection of <see cref="T:Supportify.Faq"/> entries that match the supplied parameters.</returns>
        public IEnumerable<Faq> GetFaqs(int limit = 0, int skip = 0, string[] categories = null, string[] tags = null, FaqProperties sortProperty = 0, SortOrders sortOrder = 0) {
            string resource = "faqs";
            string description = "retrieving Faq entries for the application";
            string key = "faqs";

            var parameters = new Dictionary<string, string>();
            if (limit > 0) {
                parameters["limit"] = limit.ToString();
            }
            if (skip > 0) {
                parameters["skip"] = skip.ToString();
            }
            if (categories != null && categories.Length > 0) {
                var encoded = categories.Select(x => HttpUtility.UrlEncode(x));
                parameters["categories"] = string.Join(",", encoded);
            }
            if (tags != null && tags.Length > 0) {
                var encoded = tags.Select(x => HttpUtility.UrlEncode(x));
                parameters["tags"] = string.Join(",", encoded);
            }
            if (sortProperty != 0) {
                parameters["sort"] = sortProperty.ToString();
                if (sortOrder != 0) {
                    parameters["sort"] += string.Format("-{0}", sortOrder == SortOrders.Ascending ? "asc" : "desc");
                }
            } else if (sortOrder != 0) {
                parameters["sort"] = string.Format("question-{0}", sortOrder == SortOrders.Ascending ? "asc" : "desc");
            }

            if (parameters.Count > 0) {
                var queryString = string.Join("&", parameters.Select(x =>
                    string.Format("{0}={1}", x.Key, x.Value)));
                resource += ("?" + queryString);
            }

            var faqs = this.MakeRequest<IEnumerable<Faq>>(resource, key, Method.GET, description);
            return faqs;
        }
        /// <summary>
        /// Gets the current details about the API interface being used by the proxy.
        /// </summary>
        /// <returns>A new <see cref="T:Supportify.Info"/> instance containing the basic information returned by the API.s</returns>
        public Info GetInfo() {
            string resource = "info";
            string description = "retrieve the API info";
            string key = "info";

            var info = this.MakeRequest<Info>(resource, key, Method.GET, description);
            return info;
        }
        /// <summary>
        /// Retrieves the <see cref="T:Supportify.Tag"/> with an ID that matches the supplied value.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="T:Supportify.Tag"/> to be retrieved.</param>
        /// <returns>The <see cref="T:Supportify.Tag"/> whose ID matches the supplied value; otherwise, null.</returns>
        public Tag GetTag(long id) {
            string resource = string.Format("tags/{0}", id);
            string description = "retrieve the specified Tag";
            string key = "tag";

            var tag = this.MakeRequest<Tag>(resource, key, Method.GET, description);
            return tag;
        }
        /// <summary>
        /// Retrieves the collection of <see cref="T:Supportify.Tag"/> entries that match the supplied parameters.
        /// </summary>
        /// <returns>The collection of <see cref="T:Supportify.Tag"/> entries that match the supplied parameters.</returns>
        public IEnumerable<Tag> GetTags(int limit = 0, int skip = 0, TagProperties sortProperty = 0, SortOrders sortOrder = 0) {
            string resource = "tags";
            string description = "retrieving Tag meta-data for the application";
            string key = "tags";

            var parameters = new Dictionary<string, string>();
            if (limit > 0) {
                parameters["limit"] = limit.ToString();
            }
            if (skip > 0) {
                parameters["skip"] = skip.ToString();
            }
            if (sortProperty != 0) {
                parameters["sort"] = sortProperty.ToString();
                if (sortOrder != 0) {
                    parameters["sort"] += ("-" + (sortOrder == SortOrders.Ascending ? "asc" : "desc"));
                }
            } else if (sortOrder != 0) {
                parameters["sort"] = "name-" + (sortOrder == SortOrders.Ascending ? "asc" : "desc");
            }

            if (parameters.Count > 0) {
                var queryString = string.Join("&", parameters.Select(x =>
                    string.Format("{0}={1}", x.Key, x.Value)));
                resource += ("?" + queryString);
            }

            var tags = this.MakeRequest<IEnumerable<Tag>>(resource, key, Method.GET, description);
            return tags;
        }

        internal JsonSerializer<Dictionary<string, T>> GetJsonSerializer<T>() {
            var type = typeof(T);
            if (_serializers.ContainsKey(type) == false) {
                _serializers[type] = new JsonSerializer<Dictionary<string, T>>();
            }

            return (JsonSerializer<Dictionary<string, T>>)_serializers[type];
        }
        internal T MakeRequest<T>(string resource, string key, Method method, string description = "") {
            var request = new RestRequest(resource);
            IRestResponse response = this.Client.Execute(request);

            if (response.ErrorException != null) {
                var message = string.Format(UNEXPECTED_EXCEPTION, description);
                throw new SupportifyException(message, response.ErrorException);
            } else if (response.StatusCode == HttpStatusCode.Forbidden) {
                var message = string.Format(AUTHENTICATION_EXCEPTION, description);
                throw new SupportifyAuthenticationException(message, response.ErrorException);
            }

            var deserialized = this.GetJsonSerializer<T>().DeserializeFromString(response.Content);
            return deserialized[key];
        }
    }
}
