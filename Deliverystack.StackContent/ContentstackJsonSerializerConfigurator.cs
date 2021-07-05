namespace Deliverystack.StackContent
{
    using System;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    // ReSharper disable once RedundantNameQualifier
    using Deliverystack.Core.Attributes;
    // ReSharper disable once RedundantNameQualifier
    using Deliverystack.Interfaces;
    // ReSharper disable once RedundantNameQualifier
    using Deliverystack.StackContent.Models;
    // ReSharper disable once RedundantNameQualifier
    using Deliverystack.StackContent.Models.Fields;

    // ReSharper disable once UnusedType.Global
    public class ContentstackJsonSerializerConfigurator : IConfigureJsonSerializer
    {
        private IDeliveryClient _client;

        public JsonSerializerOptions GetJsonSerializerOptions()
        {
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

            foreach (Type blockType in ModularBlockBase.Implementations)
            {
                MethodInfo methodInfo = blockType.GetRuntimeMethod(
                    "GetJsonConverter", new Type[0]);
                var obj = Activator.CreateInstance(blockType);

                if (methodInfo is not null)
                {
                    JsonConverter jsonConverter = methodInfo.Invoke(
                        obj, new object[0]) as JsonConverter;
                    options.Converters.Add(jsonConverter);
                }
            }

            //TODO: refactor to avoid static, derived class passes itself as generic to base class? 
            foreach (Type type in AutoLoadJsonConverter.GetTypesWithAttribute(typeof(AutoLoadJsonConverter)))
            {
                if (AutoLoadJsonConverter.IsEnabledForType(type))
                {
                    options.Converters.Add(Activator.CreateInstance(type) as JsonConverter);
                }
            }

            options.Converters.Add(new  ReferenceField.ReferenceFieldJsonConverter(_client));
            return options;
        }

        public void SetDeliveryClient(IDeliveryClient deliveryClient)
        {
            _client = deliveryClient;
        }
    }
}