namespace Supportify.Core.Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Machine.Specifications;
    using RestSharp;

    public class SupportifyProxy_specifications {
        static SupportifyProxy _proxy;
        static SupportifyCredentials _credentials;
        static string _apiKey, _appKey;
        static IRestClient _client;

        Establish context = () => {
            _appKey = Guid.NewGuid().ToString().Replace("-", "");
            _apiKey = Guid.NewGuid().ToString().Replace("-", "");
            _credentials = new SupportifyCredentials(_apiKey,_appKey);
            
            _proxy = new SupportifyProxy(_credentials, VersionTypes.V1);
        };

        It should_initialize_the_credentials_using_the_supplied_value = () =>
            _proxy.Credentials
                .ShouldEqual(_credentials);

        It should_initialize_the_version_using_the_supplied_value = () =>
            _proxy.Version
                .ShouldEqual(VersionTypes.V1);

        [Subject("SupportifyProxy specification")]
        public class when_using_default_values_with_the_constructor {
            It should_work_without_issues = () =>
                Catch.Exception(() => _proxy = new SupportifyProxy(_credentials))
                    .ShouldBeNull();
        }

    }
}
