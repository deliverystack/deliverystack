// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Deliverystack.Models
{
    using System.Text.Json.Serialization;

    public class ReferenceField
    {
        public string Uid { get; set; }

        [JsonPropertyName("_content_type_uid")]
        public string ContentType { get; set; }
    }
}