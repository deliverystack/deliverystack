namespace Deliverystack.StackContent.Models.Fields
{
    using Deliverystack.Interfaces;
    using Deliverystack.Models;
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public struct ReferenceField
    {
        public class ReferenceFieldProperties
        {
            public string Uid { get; set; }

            [JsonPropertyName("_content_type_uid")]
            public string ContentType { get; set; }
        }

        private ReferenceFieldProperties _entryProperties;

        public ReferenceField(ReferenceFieldProperties properties, IDeliveryClient client)
        {
            _entryProperties = properties;
            _deliveryClient = client;
        }

        public string Uid
        {
            get { return _entryProperties.Uid; }
        }

        public string ContentType 
        {
            get { return _entryProperties.ContentType; }
        }
        
        private IDeliveryClient _deliveryClient;

        public object GetEntry()
        {
            return _deliveryClient.AsDefaultEntryModel(new EntryIdentifier(ContentType, Uid));
        }

/*        public void SetDeliveryClient(IDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
        }*/

        //CANNOT: [AutoLoadJsonConverter(true)] : requires DI 
        public class ReferenceFieldJsonConverter : JsonConverter<ReferenceField>
        {
            private IDeliveryClient DeliveryClient
            {
                get;
            } 
            
//            private IDeliveryClient _deliveryClient;
            
            public ReferenceFieldJsonConverter(IDeliveryClient deliveryClient)
            {
//                _deliveryClient = deliveryClient;
                DeliveryClient = deliveryClient;
            }

            public override ReferenceField Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
//                reader.Read();
                return new ReferenceField(
                    JsonSerializer.Deserialize(ref reader, typeof(ReferenceFieldProperties), options) as
                        ReferenceFieldProperties,
                    DeliveryClient);
                //               reader.Read();
//                return field;
            }

            public override void Write(Utf8JsonWriter writer, ReferenceField value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
