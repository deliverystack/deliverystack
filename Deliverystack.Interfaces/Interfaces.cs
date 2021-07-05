//TODO: IHasApiClient
//TODO: use IWebPageEntryModel
// use readonly struct rather than class when possible

namespace Deliverystack.Interfaces
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using System.Text.Json;

    public interface ICanNormalizeUrlPaths
    {
        public string NormalizeUrlPath(string urlPath)
        {
            if (urlPath == null)
            {
                return "/";
            }

            string path = Regex.Replace("/" + urlPath.TrimStart('/').Replace('\\', '/').TrimEnd('/').ToLower(),
                "//+",
                "/");
            return path;
        }
    }

    public interface ICanBeQueryStringParams
    {
        public string GetQueryString()
        {
            return String.Join("&", this.GetType().GetProperties().Where(
                prop => prop.GetValue(this, null) != null).Select(
                prop => $"{Uri.EscapeDataString(prop.Name)}={Uri.EscapeDataString(prop.GetValue(this)?.ToString() ?? string.Empty)}"));
        }
    }
    
    public interface IHasDeliveryClient
    {
        public void SetDeliveryClient(IDeliveryClient client);

        public IDeliveryClient GetDeliveryClient();
    }
    
    public interface IDeliveryClient
    {
        public  Task<JsonElement> AsElementAsync(
            IEntryIdentifier entryId);

        public  TType AsA<TType>(IEntryIdentifier entryId);

        public Task<string> AsStringAsync(
            IEntryIdentifier entryId);

        public object AsDefaultEntryModel(IEntryIdentifier entryId);

        public string[] GetContentTypeIds(bool requireUrls = false);

        public TEntryModel[] Entries<TEntryModel>(string contentType, string fields = null);

        public string GetRepositoryId();

        public JsonSerializerOptions GetJsonSerializerOptions();
    }
    
    public interface IConfigureJsonSerializer
    {
        public JsonSerializerOptions GetJsonSerializerOptions();

        public void SetDeliveryClient(IDeliveryClient deliveryClient);
    }
    
    public interface IEntryIdentifier
    {
        public string GetContentType();
        public string GetEntryId();
    }
    
    public interface IHasJson
    {
        public void SetJson(JsonElement element, JsonSerializerOptions options);

        public JsonElement GetJson();

        public JsonSerializerOptions GetSerializerOptions();

        public TType To<TType>();
    }
    
    public interface IEntryModel : IHasJson
    {
        public string GetTitle();
    }
    
    public interface IWebPageEntryModel : IEntryModel
    {
        public string GetUrl();
        public string GetNavTitle();

        public string GetLayout();

        public string GetPartialView();
    }
}