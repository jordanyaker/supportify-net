namespace Supportify.Net.Tests.Security {
    using Machine.Specifications;

    public class SupportifyCredentials_specification {
        static string _appKey, _apiKey;

        Establish context =()=> {
            _appKey = "appkey";
            _apiKey = "apikey";
        };

        [Subject("SupportifyCredentials specification")]
        public class when_using_the_default_constructor {
            static SupportifyCredentials _credentials;

            Because of = () =>
                _credentials = new SupportifyCredentials(_apiKey, _appKey);

            It should_initialize_the_api_key_correctly = () =>
                _credentials.ApiKey
                    .ShouldEqual(_apiKey);

            It should_initialize_the_app_key_correctly = () =>
                _credentials.AppKey
                    .ShouldEqual(_appKey);
        }
    }
}
