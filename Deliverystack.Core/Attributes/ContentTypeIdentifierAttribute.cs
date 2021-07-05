namespace Deliverystack.Core.Attributes
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    //TODO: support struct instead of just class? AllowMultople?
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContentTypeIdentifierAttribute : AttributeBase
    {
        public ContentTypeIdentifierAttribute(string value)
        {
            Value = value;
        }
        public string Value { get; }

        private static ConcurrentDictionary<string, Type[]> _contentTypeTypes =
            new ConcurrentDictionary<string, Type[]>();

        public static Type GetModelTypeForContentType(string contentType)
        {
            if (!_contentTypeTypes.ContainsKey(contentType))
            {
                List<Type> result = new List<Type>();

                foreach (Type t in ContentTypeIdentifierAttribute.GetTypesWithAttribute(
                    typeof(ContentTypeIdentifierAttribute)))
                {
                    foreach (var attr in t.GetCustomAttributes(
                        typeof(ContentTypeIdentifierAttribute), true))
                    {
                        if (attr is ContentTypeIdentifierAttribute ct
                            && ct.Value.Equals(contentType, 
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.Add(t);
                        }
                    }
                }

                _contentTypeTypes[contentType] = result.ToArray();
            }

            if (_contentTypeTypes[contentType].Length != 1)
            {
                string msg = _contentTypeTypes[contentType].Length
                    + " default entry models for content type " 
                    + contentType + ".";

                if (_contentTypeTypes[contentType].Length < 1)
                {
                    msg += " Ensure that you have attributed the entry models for your content types with the " 
                        + typeof(ContentTypeIdentifierAttribute) 
                        + " attribute. .NET may not yet have loaded the assembly that contains your entry model classes. Consider accessing typeof(HomePage) in Startup.cs, where HomePage is the entry model class for your home page content type in the assembly that has not yet loaded.";
                }

                throw new ApplicationException(msg);
            }

            return _contentTypeTypes[contentType][0];
        }
    }
}

