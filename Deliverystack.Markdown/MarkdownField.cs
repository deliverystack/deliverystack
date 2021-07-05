namespace Deliverystack.Markdown
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.AspNetCore.Html;

    using Markdig;

    using Deliverystack.Core.Attributes;

    public readonly struct MarkdownField
    {
        [AutoLoadJsonConverter()]
        public class MarkdownFieldJsonConverter : JsonConverter<MarkdownField>
        {
            public override MarkdownField Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new MarkdownField(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, MarkdownField value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private readonly string _markup;

        public MarkdownField(string markdown)
        {
            RawMarkdown = markdown;

            if (String.IsNullOrEmpty(RawMarkdown))
            {
                _markup = String.Empty;
            }
            else
            {
                _markup = Markdown.ToHtml(
                    RawMarkdown,
                    new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
            }
        }

        public static implicit operator MarkdownField(string input)
        {
            return new MarkdownField(input);
        }

        public static implicit operator string(MarkdownField input)
        {
            return input.ToString();
        }

        public static implicit operator HtmlString(MarkdownField input)
        {
            return new HtmlString(input.ToString());
        }

        public string RawMarkdown { get; }

        public HtmlString Markup => new HtmlString(_markup);

        public override string ToString()
        {
            //TODO: WebUtility.HtmlEncode?
            return Markup.ToString();
        }
    }
}