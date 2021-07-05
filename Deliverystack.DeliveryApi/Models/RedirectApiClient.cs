namespace Deliverystack.DeliveryApi.Models
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;

    using Deliverystack.Interfaces;
    using Deliverystack.Models;
    
    public class RedirectApiClient
    {
        private HttpClient _client;

        public RedirectApiClient(HttpClient client, RedirectApiConfig config)
        {
            _client = client;
            _client.BaseAddress = new Uri(config.BaseAddress);
        }

        public RedirectModel Get(string path)
        {
            return Get(new RedirectModel(path));
        }

        public RedirectModel Get(RedirectModel values)
        {
            try
            {
                string text = _client.GetAsync("/redirectapi?" + (values as ICanBeQueryStringParams).GetQueryString()).Result.Content.ReadAsStringAsync().Result;

                // TODO: should probably use 404 or something
                if (String.IsNullOrEmpty(text))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<RedirectModel>(text, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
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
     }
}
