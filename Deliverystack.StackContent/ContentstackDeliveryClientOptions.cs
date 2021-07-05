namespace Deliverystack.StackContent
{
    using Microsoft.Extensions.Configuration;

    public class ContentstackDeliveryClientOptions
    {
        public static ContentstackDeliveryClientOptions GetDefault(
            IConfigurationRoot config)
        {
            return GetNamed(config, "ContentstackOptions");
        }

        public ContentstackDeliveryClientOptions(IConfigurationSection config)
        {
            config.Bind(this);
        }

        public static ContentstackDeliveryClientOptions GetNamed(
          IConfigurationRoot config, string name)
        {
            return GetFromConfig(config.GetSection(name));
        }

        public static ContentstackDeliveryClientOptions GetFromConfig(
            IConfigurationSection config)
        {
            return new ContentstackDeliveryClientOptions()
            {
                AccessToken = config["AccessToken"],
                ApiKey = config["ApiKey"],
                BaseAddress = config["BaseAddress"],
                Environment = config["Environment"]
            };
        }

        public ContentstackDeliveryClientOptions()
        {
        }

        public string ApiKey { get; set; }

        public string AccessToken { get; set; }

        public string Environment { get; set; }

        public string BaseAddress { get; set; }

    }
}