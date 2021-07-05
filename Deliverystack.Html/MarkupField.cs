namespace Deliverystack.Html
{
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Deliverystack.Core.Attributes;

    using HtmlAgilityPack;

    using Microsoft.AspNetCore.Html;

    public readonly struct MarkupField
    {
        [AutoLoadJsonConverter()]
        public class MarkupFieldJsonConverter : JsonConverter<MarkupField>
        {
            public override MarkupField Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new MarkupField(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, MarkupField value, JsonSerializerOptions options)
            {
            
                
                throw new NotImplementedException();
            }
        }
        
        private readonly string _markup;

        public MarkupField(string markup)
        {
            _markup = markup;
        }

        public static implicit operator MarkupField(string input)
        {
            return new MarkupField(input);
        }

        public static implicit operator string(MarkupField input)
        {
            return input.ToString();
        }

        public static implicit operator HtmlString(MarkupField input)
        {
            return new HtmlString(input.ToString());
        }

        public HtmlString Value
        {
            get
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(this.GetType()))
                {
                    if (attr is OpenExternalsInNewTabsAttribute)
                    {
                        return OpenExternalsLinksInNewTabsOrWindows;
                    }
                }

                return new HtmlString(_markup);
            }
        }

        public override string ToString()
        {
            return WebUtility.HtmlEncode(_markup);
        }

        public HtmlString OpenExternalsLinksInNewTabsOrWindows
        {
            //TODO: plug-in model - allow entry model that uses this field type 
            get
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(_markup);
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");

                if (nodes == null)
                {
                    return new HtmlString(_markup);
                }

                foreach (HtmlNode node in (nodes))
                {
                    string url = node.GetAttributeValue("href", string.Empty);

                    if (!url.StartsWith("/"))
                    {
                        string target = node.GetAttributeValue("target", string.Empty);

                        if (string.IsNullOrWhiteSpace(target) || target == "_self")
                        {
                            node.Attributes.Add(doc.CreateAttribute("target", "_blank"));
                        }
                    }
                }

                return new HtmlString(doc.DocumentNode.OuterHtml);
            }
        }
    }
}