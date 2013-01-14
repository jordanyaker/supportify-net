namespace Supportify.Core.Tests {
    using System;
    using System.Linq;
    using Machine.Fakes;
    using Machine.Specifications;
    using RestSharp;
using ServiceStack.Text;
using System.Collections.Generic;

    public class SupportifyProxy_specifications : WithFakes {
        static SupportifyProxy _proxy;
        static SupportifyCredentials _credentials;
        static string _apiKey, _appKey;
        static IRestClient _client;

        Establish context = () => {
            _appKey = Guid.NewGuid().ToString().Replace("-", "");
            _apiKey = Guid.NewGuid().ToString().Replace("-", "");
            _credentials = new SupportifyCredentials(_apiKey,_appKey);
            
            _client = An<IRestClient>();
            _proxy = new SupportifyProxy(_credentials, VersionTypes.V1) {
                Client = _client
            };
        };

        It should_initialize_the_credentials_using_the_supplied_value = () =>
            _proxy.Credentials
                .ShouldEqual(_credentials);

        It should_initialize_the_version_using_the_supplied_value = () =>
            _proxy.Version
                .ShouldEqual(VersionTypes.V1);

        [Subject("SupportifyProxy specification")]
        public class when_disposing_of_the_proxy {
            It should_work_without_issues = () =>
                Catch.Exception(() => _proxy.Dispose())
                    .ShouldBeNull();
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_the_client {
            It should_return_the_initialized_client = () => {
                _proxy.Client
                    .ShouldEqual(_client);
            };

            [Subject("SupportifyProxy specification, when getting the client")]
            public class and_the_client_has_not_yet_been_initialized {
                Establish context = () =>
                    _proxy.Client = null;

                It should_return_an_initialized_client_instance_with_correctly_initialized_properties = () => {
                    var client = _proxy.Client;

                    client.BaseUrl.ShouldEqual("https://api.supportify.io/V1");
                    client.DefaultParameters
                        .Where(x => x.Type == ParameterType.HttpHeader)
                        .ShouldContain(x =>
                            x.Name == "X-Supportify-ApiKey" &&
                            (string)x.Value == _apiKey);
                    client.DefaultParameters
                        .Where(x => x.Type == ParameterType.HttpHeader)
                        .ShouldContain(x =>
                            x.Name == "X-Supportify-AppKey" &&
                            (string)x.Value == _appKey);
                };
            }
        }

        [Subject("SupportifyProxy specification")]
        public class when_using_default_values_with_the_constructor {
            It should_work_without_issues = () =>
                Catch.Exception(() => _proxy = new SupportifyProxy(_credentials))
                    .ShouldBeNull();
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_a_JSON_serializer {
            static JsonSerializer<Dictionary<string, int>> _serializer1;

            Because of =()=> {
                _serializer1 = _proxy.GetJsonSerializer<int>();
            };


            It should_return_a_new_serializer = () => 
                _serializer1.ShouldNotBeNull();

            It should_return_the_same_serializer_for_all_successive_types = () =>
                _proxy.GetJsonSerializer<int>()
                    .ShouldEqual(_serializer1);
        }
    }
}
