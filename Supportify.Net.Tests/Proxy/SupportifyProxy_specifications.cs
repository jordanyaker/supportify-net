namespace Supportify.Net.Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Machine.Fakes;
    using Machine.Specifications;
    using RestSharp;
    using ServiceStack.Text;
    using Supportify.Help;

    public class SupportifyProxy_specifications : WithFakes {
        static SupportifyProxy _proxy;
        static SupportifyCredentials _credentials;
        static string _apiKey, _appKey;
        static IRestClient _client;

        Establish context = () => {
            _appKey = Guid.NewGuid().ToString().Replace("-", "");
            _apiKey = Guid.NewGuid().ToString().Replace("-", "");
            _credentials = new SupportifyCredentials(_apiKey, _appKey);

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

            Because of = () => {
                _serializer1 = _proxy.GetJsonSerializer<int>();
            };


            It should_return_a_new_serializer = () =>
                _serializer1.ShouldNotBeNull();

            It should_return_the_same_serializer_for_all_successive_types = () =>
                _proxy.GetJsonSerializer<int>()
                    .ShouldEqual(_serializer1);
        }

        [Subject("SupportifyProxy specification")]
        public class when_making_a_get_request {
            static string _resource, _key, _description;
            static IRestResponse _response;

            Establish context = () => {
                _resource = "test/method";
                _key = "result";
                _description = "test execution";

                _response = An<IRestResponse>();
                _client.WhenToldTo(x => x
                    .Execute(Param<IRestRequest>.Matches(y =>
                        y.Resource == _resource &&
                        y.Method == Method.GET)))
                    .Return(_response);
            };

            [Subject("SupportifyProxy specification, when making a get request")]
            public class and_a_200_response_was_returned {
                static int _result;

                Establish context = () => {
                    _response
                        .WhenToldTo(x => x.Content)
                        .Return("{ \"result\" : 42 }");
                    _response
                        .WhenToldTo(x => x.ErrorException)
                        .Return<Exception>(null);
                    _response
                        .WhenToldTo(x => x.StatusCode)
                        .Return(HttpStatusCode.OK);
                };

                Because of = () =>
                    _result = _proxy.MakeGetRequest<int>(_resource, _key, _description);

                It should_return_the_correct_response = () =>
                    _result.ShouldEqual(42);
            }

            [Subject("SupportifyProxy specification, when making a get request")]
            public class and_an_error_exception_was_returned {
                static Exception _result, _expected;

                Establish context = () => {
                    _expected = new System.Net.WebException();
                    _response
                        .WhenToldTo(x => x.ErrorException)
                        .Return(_expected);
                    _client.WhenToldTo(x => x
                        .Execute(Param<IRestRequest>.Matches(y =>
                            y.Resource == _resource &&
                            y.Method == Method.GET)))
                        .Return(_response);
                };

                Because of = () =>
                    _result = (SupportifyException)Catch.Exception(() =>
                        _proxy.MakeGetRequest<int>(_resource, _key, _description));

                It should_wrap_and_return_the_exception = () => {
                    _result.InnerException
                        .ShouldEqual(_expected);
                    _result.Message
                        .ShouldEqual("An unexpected exception occurred while attempting to " + _description + ".");
                };
            }

            [Subject("SupportifyProxy specification, when making a get request")]
            public class and_an_unauthenticated_exception_was_returned {
                static Exception _result;

                Establish context = () => {
                    _response
                        .WhenToldTo(x => x.StatusCode)
                        .Return(HttpStatusCode.Unauthorized);
                    _response
                        .WhenToldTo(x => x.ErrorException)
                        .Return<Exception>(null);
                    _client.WhenToldTo(x => x
                        .Execute(Param<IRestRequest>.Matches(y =>
                            y.Resource == _resource &&
                            y.Method == Method.GET)))
                        .Return(_response);
                };

                Because of = () =>
                    _result = (SupportifyAuthenticationException)Catch.Exception(() =>
                        _proxy.MakeGetRequest<int>(_resource, _key, _description));

                It should_wrap_and_return_the_exception = () => {
                    _result.Message
                        .ShouldEqual("An authentication error occurred while attempting to " + _description + ".");
                };
            }
        }

        [Subject("SupportifyProxy specification")]
        public class when_making_a_post_request {
            static string _resource, _description;
            static IRestResponse _response;

            Establish context = () => {
                _resource = "test/method";
                _description = "test execution";

                _response = An<IRestResponse>();
                _client.WhenToldTo(x => x
                    .Execute(Param<IRestRequest>.Matches(y =>
                        y.Resource == _resource &&
                        y.Method == Method.POST)))
                    .Return(_response);
            };

            [Subject("SupportifyProxy specification, when making a post request")]
            public class and_a_200_response_was_returned {
                Establish context = () => {
                    _response
                        .WhenToldTo(x => x.Content)
                        .Return("{ \"result\" : 42 }");
                    _response
                        .WhenToldTo(x => x.ErrorException)
                        .Return<Exception>(null);
                    _response
                        .WhenToldTo(x => x.StatusCode)
                        .Return(HttpStatusCode.OK);
                };

                It should_work_without_issues = () =>
                    Catch.Exception(() => _proxy.MakePostRequest(_resource, _description))
                        .ShouldBeNull();
            }

            [Subject("SupportifyProxy specification, when making a post request")]
            public class and_an_error_exception_was_returned {
                static Exception _result, _expected;

                Establish context = () => {
                    _expected = new System.Net.WebException();
                    _response
                        .WhenToldTo(x => x.ErrorException)
                        .Return(_expected);
                    _client.WhenToldTo(x => x
                        .Execute(Param<IRestRequest>.Matches(y =>
                            y.Resource == _resource &&
                            y.Method == Method.POST)))
                        .Return(_response);
                };

                Because of = () =>
                    _result = (SupportifyException)Catch.Exception(() =>
                        _proxy.MakePostRequest(_resource, _description));

                It should_wrap_and_return_the_exception = () => {
                    _result.InnerException
                        .ShouldEqual(_expected);
                    _result.Message
                        .ShouldEqual("An unexpected exception occurred while attempting to " + _description + ".");
                };
            }

            [Subject("SupportifyProxy specification, when making a post request")]
            public class and_an_unauthenticated_exception_was_returned {
                static Exception _result;

                Establish context = () => {
                    _response
                        .WhenToldTo(x => x.StatusCode)
                        .Return(HttpStatusCode.Unauthorized);
                    _response
                        .WhenToldTo(x => x.ErrorException)
                        .Return<Exception>(null);
                    _client.WhenToldTo(x => x
                        .Execute(Param<IRestRequest>.Matches(y =>
                            y.Resource == _resource &&
                            y.Method == Method.POST)))
                        .Return(_response);
                };

                Because of = () =>
                    _result = (SupportifyAuthenticationException)Catch.Exception(() =>
                        _proxy.MakePostRequest(_resource, _description));

                It should_wrap_and_return_the_exception = () => {
                    _result.Message
                        .ShouldEqual("An authentication error occurred while attempting to " + _description + ".");
                };
            }
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_the_api_info {
            static Info _result, _expected;
            static IRestResponse _response;

            Establish context = () => {
                _expected = new Info {
                    Application = new Application {
                        Id = 123,
                        Name = "Test Application"
                    },
                    Supportify = new Supportify {
                        Version = "1.2.3",
                        Company = "Test Company"
                    }
                };

                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { info = _expected };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            Because of = () =>
                _result = _proxy.GetInfo();

            It should_use_the_correct_resource_address = () => {
                _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "info")));
            };

            It should_return_the_expected_info = () => {
                _result.Application.Name.ShouldEqual(_expected.Application.Name);
                _result.Application.Id.ShouldEqual(_expected.Application.Id);
                _result.Supportify.Version.ShouldEqual(_expected.Supportify.Version);
                _result.Supportify.Company.ShouldEqual(_expected.Supportify.Company);
            };
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_a_single_category {
            static long _id;
            static string _name, _description;
            static Category _result, _expected;
            static IRestResponse _response;

            Establish context = () => {
                _expected = new Category {
                    Name = (_name = "Test Category"),
                    Description = (_description = "This is the Test category.")
                };
                _id = _expected.Id;


                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { category = _expected };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            Because of = () =>
                _result = _proxy.GetCategory(_id);

            It should_use_the_correct_resource_address = () => {
                _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == ("categories/" + _id.ToString()))));
            };

            It should_return_the_expected_category = () => {
                _result.Id.ShouldEqual(_id);
                _result.Name.ShouldEqual(_name);
                _result.Description.ShouldEqual(_description);
            };
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_categories {
            static Category _expected1, _expected2;
            static IEnumerable<Category> _result;
            static IRestResponse _response;

            Establish context = () => {
                _expected1 = new Category {
                    Name = "Test Category 1",
                    Description = "This is the first Test category."
                };
                _expected2 = new Category {
                    Name = "Test Category 2",
                    Description = "This is the second Test category."
                };

                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { categories = new[] { _expected1, _expected2 } };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            [Subject("SupportifyProxy specification, when getting categories")]
            public class and_no_parameters_were_specified {
                Because of = () =>
                    _result = _proxy.GetCategories();

                It should_use_the_correct_resource_address = () => {
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                        y.Method == Method.GET &&
                        y.Resource == "categories")));
                };

                It should_return_a_list_containing_the_expected_categories = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting categories")]
            public class and_a_limit_is_specified {
                static int _limit;

                Establish context = () =>
                    _limit = 5;

                Because of = () =>
                    _result = _proxy.GetCategories(limit: _limit);

                It should_add_the_limit_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "categories?limit=5")));

                It should_return_a_list_containing_the_expected_categories = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting categories")]
            public class and_a_number_of_entries_to_skip_is_specified {
                static int _skip;

                Establish context = () =>
                    _skip = 5;

                Because of = () =>
                    _result = _proxy.GetCategories(skip: _skip);

                It should_add_the_skip_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "categories?skip=5")));

                It should_return_a_list_containing_the_expected_categories = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting categories")]
            public class and_a_sort_parameter_was_specified_with_no_order {
                static CategoryProperties _sortProperty;

                Establish context = () =>
                    _sortProperty = CategoryProperties.Description;

                Because of = () =>
                    _result = _proxy.GetCategories(sortProperty: _sortProperty);

                It should_add_the_sort_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "categories?sort=Description")));

                It should_return_a_list_containing_the_expected_categories = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting categories")]
            public class and_a_sort_parameter_was_specified_with_an_order {
                static CategoryProperties _sortProperty;
                static SortOrders _sortOrder;

                Establish context = () => {
                    _sortProperty = CategoryProperties.Description;
                    _sortOrder = SortOrders.Descending;
                };

                Because of = () =>
                    _result = _proxy.GetCategories(sortProperty: _sortProperty, sortOrder: _sortOrder);

                It should_add_the_sort_parameter_and_order_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "categories?sort=Description-desc")));

                It should_return_a_list_containing_the_expected_categories = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting categories")]
            public class and_a_sort_order_was_specified_with_no_sort_parameter_selected {
                static SortOrders _sortOrder;

                Establish context = () =>
                    _sortOrder = SortOrders.Descending;

                Because of = () =>
                    _result = _proxy.GetCategories(sortOrder: _sortOrder);

                It should_add_the_sort_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "categories?sort=name-desc")));

                It should_return_a_list_containing_the_expected_categories = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_a_single_tag {
            static long _id;
            static string _name, _description;
            static Tag _result, _expected;
            static IRestResponse _response;

            Establish context = () => {
                _expected = new Tag {
                    Name = (_name = "Test Tag"),
                    Description = (_description = "This is the Test tag.")
                };
                _id = _expected.Id;


                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { tag = _expected };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            Because of = () =>
                _result = _proxy.GetTag(_id);

            It should_use_the_correct_resource_address = () => {
                _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == ("tags/" + _id.ToString()))));
            };

            It should_return_the_expected_tag = () => {
                _result.Id.ShouldEqual(_id);
                _result.Name.ShouldEqual(_name);
                _result.Description.ShouldEqual(_description);
            };
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_tags {
            static Tag _expected1, _expected2;
            static IEnumerable<Tag> _result;
            static IRestResponse _response;

            Establish context = () => {
                _expected1 = new Tag {
                    Name = "Test Tag 1",
                    Description = "This is the first Test tag."
                };
                _expected2 = new Tag {
                    Name = "Test Tag 2",
                    Description = "This is the second Test tag."
                };

                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { tags = new[] { _expected1, _expected2 } };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            [Subject("SupportifyProxy specification, when getting tags")]
            public class and_no_parameters_were_specified {
                Because of = () =>
                    _result = _proxy.GetTags();

                It should_use_the_correct_resource_address = () => {
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                        y.Method == Method.GET &&
                        y.Resource == "tags")));
                };

                It should_return_a_list_containing_the_expected_tags = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting tags")]
            public class and_a_limit_is_specified {
                static int _limit;

                Establish context = () =>
                    _limit = 5;

                Because of = () =>
                    _result = _proxy.GetTags(limit: _limit);

                It should_add_the_limit_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "tags?limit=5")));

                It should_return_a_list_containing_the_expected_tags = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting tags")]
            public class and_a_number_of_entries_to_skip_is_specified {
                static int _skip;

                Establish context = () =>
                    _skip = 5;

                Because of = () =>
                    _result = _proxy.GetTags(skip: _skip);

                It should_add_the_skip_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "tags?skip=5")));

                It should_return_a_list_containing_the_expected_tags = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting tags")]
            public class and_a_sort_parameter_was_specified_with_no_order {
                static TagProperties _sortProperty;

                Establish context = () =>
                    _sortProperty = TagProperties.Description;

                Because of = () =>
                    _result = _proxy.GetTags(sortProperty: _sortProperty);

                It should_add_the_sort_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "tags?sort=Description")));

                It should_return_a_list_containing_the_expected_tags = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting tags")]
            public class and_a_sort_parameter_was_specified_with_an_order {
                static TagProperties _sortProperty;
                static SortOrders _sortOrder;

                Establish context = () => {
                    _sortProperty = TagProperties.Description;
                    _sortOrder = SortOrders.Descending;
                };

                Because of = () =>
                    _result = _proxy.GetTags(sortProperty: _sortProperty, sortOrder: _sortOrder);

                It should_add_the_sort_parameter_and_order_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "tags?sort=Description-desc")));

                It should_return_a_list_containing_the_expected_tags = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting tags")]
            public class and_a_sort_order_was_specified_with_no_sort_parameter_selected {
                static SortOrders _sortOrder;

                Establish context = () =>
                    _sortOrder = SortOrders.Descending;

                Because of = () =>
                    _result = _proxy.GetTags(sortOrder: _sortOrder);

                It should_add_the_sort_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "tags?sort=name-desc")));

                It should_return_a_list_containing_the_expected_tags = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_a_single_faq {
            static long _id;
            static string _question, _answer;
            static Faq _result, _expected;
            static IRestResponse _response;

            Establish context = () => {
                _expected = new Faq {
                    Id = (_id = 12345),
                    Question = (_question = "Is this the Test Faq?"),
                    Answer = (_answer = "Yes. This is the Test faq.")
                };

                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { faq = _expected };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            Because of = () =>
                _result = _proxy.GetFaq(_id);

            It should_use_the_correct_resource_address = () => {
                _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == ("faqs/" + _id.ToString()))));
            };

            It should_return_the_expected_faq = () => {
                _result.Id.ShouldEqual(_id);
                _result.Answer.ShouldEqual(_answer);
                _result.Question.ShouldEqual(_question);
            };
        }

        [Subject("SupportifyProxy specification")]
        public class when_getting_faqs {
            static Faq _expected1, _expected2;
            static IEnumerable<Faq> _result;
            static IRestResponse _response;

            Establish context = () => {
                _expected1 = new Faq {
                    Id = 5,
                    Question = "Is this Test Faq 1?",
                    Answer = "Yes. This is the first Test faq."
                };
                _expected2 = new Faq {
                    Id = 2132534,
                    Question = "Is this Test Faq 2?",
                    Answer = "Yes. This is the second Test faq."
                };

                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);

                var model = new { faqs = new[] { _expected1, _expected2 } };
                _response.WhenToldTo(x => x.Content)
                    .Return(JsonSerializer.SerializeToString(model));
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_no_parameters_were_specified {
                Because of = () =>
                    _result = _proxy.GetFaqs();

                It should_use_the_correct_resource_address = () => {
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                        y.Method == Method.GET &&
                        y.Resource == "faqs")));
                };

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_a_limit_is_specified {
                static int _limit;

                Establish context = () =>
                    _limit = 5;

                Because of = () =>
                    _result = _proxy.GetFaqs(limit: _limit);

                It should_add_the_limit_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?limit=5")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_categories_were_specified {
                static string[] _categories;

                Establish context = () =>
                    _categories = new[] { "test 1", "phunkadelic" };

                Because of = () =>
                    _result = _proxy.GetFaqs(categories: _categories);

                It should_add_the_limit_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?categories=test+1,phunkadelic")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_tags_were_specified {
                static string[] _tags;

                Establish context = () =>
                    _tags = new[] { "test 1", "phunkadelic" };

                Because of = () =>
                    _result = _proxy.GetFaqs(tags: _tags);

                It should_add_the_limit_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?tags=test+1,phunkadelic")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_a_number_of_entries_to_skip_is_specified {
                static int _skip;

                Establish context = () =>
                    _skip = 5;

                Because of = () =>
                    _result = _proxy.GetFaqs(skip: _skip);

                It should_add_the_skip_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?skip=5")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_a_sort_parameter_was_specified_with_no_order {
                static FaqProperties _sortProperty;

                Establish context = () =>
                    _sortProperty = FaqProperties.Answer;

                Because of = () =>
                    _result = _proxy.GetFaqs(sortProperty: _sortProperty);

                It should_add_the_sort_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?sort=Answer")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_a_sort_parameter_was_specified_with_an_order {
                static FaqProperties _sortProperty;
                static SortOrders _sortOrder;

                Establish context = () => {
                    _sortProperty = FaqProperties.Answer;
                    _sortOrder = SortOrders.Descending;
                };

                Because of = () =>
                    _result = _proxy.GetFaqs(sortProperty: _sortProperty, sortOrder: _sortOrder);

                It should_add_the_sort_parameter_and_order_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?sort=Answer-desc")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }

            [Subject("SupportifyProxy specification, when getting faqs")]
            public class and_a_sort_order_was_specified_with_no_sort_parameter_selected {
                static SortOrders _sortOrder;

                Establish context = () =>
                    _sortOrder = SortOrders.Descending;

                Because of = () =>
                    _result = _proxy.GetFaqs(sortOrder: _sortOrder);

                It should_add_the_sort_parameter_to_the_query_string = () =>
                    _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.GET &&
                    y.Resource == "faqs?sort=question-desc")));

                It should_return_a_list_containing_the_expected_faqs = () => {
                    _result.ShouldContain(x => x.Id == _expected1.Id);
                    _result.ShouldContain(x => x.Id == _expected2.Id);
                };
            }
        }

        [Subject("SupportifyProxy specification")]
        public class when_posting_a_faq_vote {
            static long _id;
            static VoteTypes _vote;
            static Exception _exception;
            static IRestResponse _response;

            Establish context = () => {
                _id = 12345;
                _vote = VoteTypes.Down;

                _response = An<IRestResponse>();
                _response.WhenToldTo(x => x.StatusCode)
                    .Return(HttpStatusCode.OK);
                _client
                    .WhenToldTo(x => x.Execute(Param.IsAny<IRestRequest>()))
                    .Return(_response);
            };

            Because of = () =>
                _exception = Catch.Exception(() => _proxy.PostFaqVote(_id, _vote));

            It should_use_the_correct_resource_address = () => {
                _client.WasToldTo(x => x.Execute(Param<IRestRequest>.Matches(y =>
                    y.Method == Method.POST &&
                    y.Resource == string.Format("faqs/{0}?vote={1}", _id, _vote))));
            };

            It should_work_without_issue = () =>
                _exception.ShouldBeNull();
        }
    }
}
