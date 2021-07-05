namespace Deliverystack.DeliveryApi.Models
{
    using Deliverystack.Interfaces;
    using Deliverystack.Models;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;

    public class PathApiClient
    {
        private HttpClient _client;

        public PathApiClient(HttpClient client, PathApiConfig config)
        {
            _client = client;
            _client.BaseAddress = new Uri(config.BaseAddress);
        }

        public PathApiResultModel Get(PathApiBindingModel values)
        {
            try
            {
                string data = _client.GetAsync("/pathapi?" + (values as ICanBeQueryStringParams).GetQueryString()).Result.Content.ReadAsStringAsync().Result;
                return JsonSerializer.Deserialize<PathApiResultModel>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Exception originalException = ex;

                while (ex != null)
                {
                    Console.WriteLine(ex.GetType() + " : " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    WebException wex = ex as WebException;

                    if (wex != null)
                    {
                        if (wex.Response != null)
                        {
                            using (var stream = wex.Response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    Console.WriteLine(wex.GetType() + " : Response Body:");
                                    Console.WriteLine(reader.ReadToEnd());
                                }
                            }
                        }
                    }

                    //TODO:                    HttpRequestException hrx = ex as HttpRequestException;


                    ex = ex.InnerException;
                }

                Console.WriteLine(originalException.StackTrace);
                throw;
            }
        }
        public PathApiEntryModel GetEntry(string urlPath)
        {
            return GetEntry(new PathApiBindingModel() { Path = urlPath });
        }

        public PathApiEntryModel GetEntry(PathApiBindingModel options)
        {
            PathApiResultModel results = Get(options);

            if (results.Total != 1)
            {
                //                throw new ArgumentException(this + " : cannot determine entry from " + options.Path);
                return null;
            }

            using var enumerator = results.Entries.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }
     }
}
