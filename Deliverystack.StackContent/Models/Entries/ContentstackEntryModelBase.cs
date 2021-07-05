namespace Deliverystack.StackContent.Models.Entries
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Deliverystack.Interfaces;
    using Deliverystack.StackContent.Models.Groups;

    public class ContentstackEntryModelBase : IEntryModel, IHasDeliveryClient
    {
        public string Uid { get; set; }
        public string Title { get; set; }

        public List<string> Tags { get; set; }
        public PublishDetails PublishDetails { get; set; }

        [JsonPropertyName("_version")]
        public int Version { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset Created { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset Updated { get; set; }

        [JsonPropertyName("created_by")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("updated_by")]
        public string UpdatedBy { get; set; }

        public string ContentType { get; set; }

        public virtual string GetTitle()
        {
            return Title;
        }

        private JsonElement _element;
        private JsonSerializerOptions _serializerOptions;

        public JsonElement GetJson()
        {
            return _element;
        }

        public JsonSerializerOptions GetSerializerOptions()
        {
            return _serializerOptions;
        }

        public void SetJson(JsonElement element, JsonSerializerOptions options)
        {
            _element = element;
            _serializerOptions = options;
        }

        public TType To<TType>()
        {
            TType obj = JsonSerializer.Deserialize<TType>(
                _element.GetRawText(), 
                _serializerOptions);

            IHasJson iHasJson = obj as IHasJson;

            if (iHasJson != null)
            {
                iHasJson.SetJson(_element, GetSerializerOptions());
            }

            IHasDeliveryClient iHasDeliveryClient = obj as IHasDeliveryClient;

            if (iHasDeliveryClient != null)
            {
                iHasDeliveryClient.SetDeliveryClient(_deliveryClient);
            }

            return obj;
        }
 
        private IDeliveryClient _deliveryClient;

        public void SetDeliveryClient(IDeliveryClient client)
        {
            _deliveryClient = client;
        }

        public IDeliveryClient GetDeliveryClient()
        {
            return _deliveryClient;
        }
    }
}
