namespace Deliverystack.StackContent
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Deliverystack.Core.Attributes;
    using Deliverystack.Core.Threading;
    using Deliverystack.Interfaces;

    public class ContentstackDeliveryClient : IDeliveryClient
    {
        private readonly HttpClient _httpClient;
private IConfigureJsonSerializer _jsonConfigurator;

public ContentstackDeliveryClient(HttpClient httpClient,
    ContentstackDeliveryClientOptions options,
    IConfigureJsonSerializer jsonConfigurator)
{
    _httpClient = httpClient;
    ApiKey = options.ApiKey;
    AccessToken = options.AccessToken;
    BaseAddress = options.BaseAddress;
    ContenstackEnvironment = options.Environment;
    _jsonConfigurator = jsonConfigurator;
    _jsonConfigurator.SetDeliveryClient(this);
}

        public string ApiKey
        {
            set
            {
                _httpClient.DefaultRequestHeaders.Add("api_key", value);
            }
        }

        public string AccessToken
        {
            set
            {
                _httpClient.DefaultRequestHeaders.Add("access_token", value);
            }
        }

        public string ContenstackEnvironment
        {
            set
            {
                // Contentstack ignores this header, so this is just for storage
                // for a query string parameter value calculated in AsStringAsync().
                _httpClient.DefaultRequestHeaders.Add("environment", value);
            }

            private get
            {
                return _httpClient.DefaultRequestHeaders.GetValues("environment").First();
            }
        }

        public string BaseAddress
        {
            set
            {
                _httpClient.BaseAddress = new Uri(value);
            }
        }

        public Task<string> AsStringAsync(IEntryIdentifier entryId)
        {
            // https://www.contentstack.com/docs/developers/apis/content-delivery-api/#single-entry
            return _httpClient.GetStringAsync(
                $"/v3/content_types/{entryId.GetContentType()}/entries/{entryId.GetEntryId()}?environment={ContenstackEnvironment}");
        }

        public async Task<JsonElement> AsElementAsync(IEntryIdentifier entryId)
        {
            JsonDocument json = JsonDocument.Parse(await AsStringAsync(entryId));

            if (!json.RootElement.TryGetProperty("entry", out JsonElement entry))
            {
                throw new ArgumentException("Entry not found: " + entryId.GetContentType() + " : " + entryId.GetEntryId());
            }

            return entry;
        }

        public TType AsA<TType>(IEntryIdentifier entryId)
        {
            JsonElement element = AsElementAsync(entryId).Result;
            TType result = System.Text.Json.JsonSerializer.Deserialize<TType>(
                element.GetRawText(),
                GetJsonSerializerOptions());
            SetInterfaces(result, element);
            return result;
        }

        private void SetInterfaces(object obj, JsonElement element)
        {
            IHasJson iHasJson = obj as IHasJson;

            if (iHasJson != null)
            {
                iHasJson.SetJson(element, GetJsonSerializerOptions());
            }

            IHasDeliveryClient iHasDeliveryClient = obj as IHasDeliveryClient;

            if (iHasDeliveryClient != null)
            {
                iHasDeliveryClient.SetDeliveryClient(this);
            }
        }

        public string GetRepositoryId()
        {
            return String.Join('-',
                _httpClient.DefaultRequestHeaders.GetValues("api_key").First(),
                _httpClient.DefaultRequestHeaders.GetValues("environment").First());
        }

        public virtual JsonSerializerOptions GetJsonSerializerOptions()
        {
            return _jsonConfigurator.GetJsonSerializerOptions();
        }

        public object AsDefaultEntryModel(
            IEntryIdentifier entryId)
        {
            JsonElement json = AsElementAsync(entryId).Result;
            object entryModel = JsonSerializer.Deserialize(json.GetRawText(),
                ContentTypeIdentifierAttribute.GetModelTypeForContentType(entryId.GetContentType()),
                GetJsonSerializerOptions());
            SetInterfaces(entryModel, json);
            return entryModel;
        }

        public string[] GetContentTypeIds(bool requireUrls = false)
        {
            int pageSize = 100;
            int pageIndex = 0;
            ConcurrentBag<string> contentTypeIds = new ConcurrentBag<string>();

            // https://www.contentstack.com/docs/developers/apis/content-delivery-api/#all-content-types
            string url = $"/v3/content_types?environment={ContenstackEnvironment}&limit={pageSize}";
            JsonElement firstPage = JsonDocument.Parse(_httpClient.GetStringAsync(url + "&include_count=true").Result).RootElement;
            AddContentTypes(contentTypeIds, firstPage, requireUrls);
            int totalContentTypes = Int32.Parse(firstPage.GetProperty("count").ToString());

            if (totalContentTypes > pageSize)
            {
                ThreadList threads = new ThreadList();

                while (++pageIndex * pageSize < totalContentTypes)
                {
                    AddContentTypes(contentTypeIds, JsonDocument.Parse(_httpClient.GetStringAsync(
                        url + $"&skip={pageIndex * pageSize}").Result).RootElement, requireUrls);
                }

                threads.JoinAll();
            }

            return contentTypeIds.ToArray();
        }

        private void AddContentTypes(ConcurrentBag<string> contentTypeIds, JsonElement page, bool requireUrls)
        {
            foreach (JsonElement contentType in page.GetProperty("content_types").EnumerateArray())
            {
                bool includeContentType = !requireUrls;

                if (!includeContentType)
                {
                    foreach(JsonElement field in contentType.GetProperty("schema").EnumerateArray())
                    { 
                        if (field.GetProperty("uid").ToString() != "pagedata"
                            && field.GetProperty("uid").ToString() != "url")
                        {
                            continue;
                        }

                        includeContentType = true;
                        break;
                    }
                }

                if (includeContentType)
                {
                    contentTypeIds.Add(contentType.GetProperty("uid").ToString());
                }
            }
        }
        private void AddEntries<TEntryModel>(ConcurrentBag<TEntryModel> entries, JsonElement page)
        {
            foreach (JsonElement entry in page.GetProperty("entries").EnumerateArray())
            {
                entries.Add(JsonSerializer.Deserialize<TEntryModel>(
                    entry.GetRawText(),
                    GetJsonSerializerOptions()));
            }
        }

        public TEntryModel[] Entries<TEntryModel>(string contentType, string fields = null)
        {
            int pageIndex = 0;
            int pageSize = 100;
            ConcurrentBag<TEntryModel> entries = new ConcurrentBag<TEntryModel>();

            // https://www.contentstack.com/docs/developers/apis/content-delivery-api/#get-all-entries
            string url = $"/v3/content_types/{contentType}/entries?environment={ContenstackEnvironment}&limit={pageSize}";

            // https://www.contentstack.com/docs/developers/apis/content-delivery-api/#queries
            //            if (!String.IsNullOrWhiteSpace(query))
            //            {
            //                url += "&query=" + query;
            //            }

            if (!String.IsNullOrWhiteSpace(fields))
            {
                foreach (string field in fields.Split(','))
                {
                    //https://www.contentstack.com/docs/developers/apis/content-delivery-api/#only-operator
                    url += "&only[BASE][]=" + field;
                }
            }

            JsonElement firstPage = JsonDocument.Parse(_httpClient.GetStringAsync(
                url + "&include_count=true").Result).RootElement;
            AddEntries(entries, firstPage);
            int totalEntries = Int32.Parse(firstPage.GetProperty("count").ToString());

            if (totalEntries > pageSize)
            {
                ThreadList threads = new ThreadList();

                while (++pageIndex * pageSize < totalEntries)
                {
                    threads.Add(() =>
                    {
                        AddEntries(entries,
                            JsonDocument.Parse(_httpClient.GetStringAsync(
                                url + "&skip=" + pageIndex * pageSize).Result).RootElement);

                    });
                }

                threads.JoinAll();
            }

            //TODO: cache?
            return entries.ToArray();
        }
    }
}