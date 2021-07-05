namespace Deliverystack.StackContent.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class ModularBlockBase
    {
        private static List<Type> _implementations = null;
        private static object _sync = new object();

        public static void Add(JsonSerializerOptions options)
        {
            foreach (Type blockType in Implementations)
            {
                MethodInfo methodInfo =
                    blockType.GetRuntimeMethod("GetJsonConverter", new Type[0]);
                ModularBlockBase obj =
                    (ModularBlockBase) Activator.CreateInstance(blockType);
                JsonConverter jsonConverter =
                  methodInfo.Invoke(obj, Array.Empty<object>()) as JsonConverter;
                options.Converters.Add(jsonConverter);
            }
        }

        public static Type[] Implementations
        {
            get
            {
                if (_implementations == null)
                {
                    lock (_sync)
                    {
                        _implementations = new List<Type>();

                        foreach (Assembly assembly
                            in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            try
                            {
                                foreach (Type type
                                    in assembly.GetExportedTypes())
                                {
                                    if (type.BaseType != null
                                        && type.BaseType.BaseType != null
                                        && type.BaseType.BaseType
                                            == typeof(ModularBlockBase))
                                    {
                                        _implementations.Add(type);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //TODO: assembly is obfuscated, etc.
                            }
                        }
                    }
                }

                return _implementations.ToArray();
            }
        }
    };

    public abstract class
        ModularBlockBase<TBlockBase, TBlockTypeEnum> : ModularBlockBase
        where TBlockBase : class
        where TBlockTypeEnum : struct, IConvertible
    {
        public class ModularBlocksJsonConverter : JsonConverter<TBlockBase>
        {
            public override void Write(Utf8JsonWriter writer, TBlockBase value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override TBlockBase Read(
                ref Utf8JsonReader reader, 
                Type typeToConvert, 
                JsonSerializerOptions options)
            {
                // Move from the StartObject token to the first element,
                // which identifies the type of the modular block
                reader.Read();

                // block type
                TBlockTypeEnum parsed;

                if (Enum.TryParse(
                    reader.GetString(),
                    true,
                    out parsed))
                {
                    foreach (Type t in Assembly.GetAssembly(
                        typeof(TBlockBase)).GetTypes().Where(
                            myType => myType.IsClass
                                && !myType.IsAbstract
                                && myType.IsSubclassOf(typeof(TBlockBase))))
                    {
                        TBlockBase obj = (TBlockBase)Activator.CreateInstance(t);

                        foreach (PropertyInfo propertyInfo
                            in obj.GetType().GetRuntimeProperties())
                        {
                            if (propertyInfo.PropertyType == typeof(TBlockTypeEnum))
                            {
                                if (((int)propertyInfo.GetValue(obj))
                                    == parsed.ToInt32(CultureInfo.InvariantCulture))
                                {
                                    TBlockBase result = (TBlockBase)JsonSerializer.Deserialize(ref reader, obj.GetType(), options);

                                    // move parser past the closing of the StartObject
                                    reader.Read();

                                    return result;
                                }
                            }
                        }
                    }
                }

                Trace.Assert(
                    false,
                    "Unable to locate " + typeof(TBlockTypeEnum) + " property or matching "
                    + parsed + " in classes of " + Assembly.GetAssembly(typeof(TBlockBase))
                    + " that derive from " + typeof(TBlockBase));
                return default(TBlockBase);
            }
        }

        public JsonConverter GetJsonConverter()
        {
            return new ModularBlocksJsonConverter();
        }
    }
}
