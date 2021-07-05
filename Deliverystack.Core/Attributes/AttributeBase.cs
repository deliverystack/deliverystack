namespace Deliverystack.Core.Attributes
{
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

public abstract class AttributeBase : Attribute
{
    // map the type of the attribute to a list of types that have that attribute.
    // static and cached to optimize performance.
    // ReSharper disable once InconsistentNaming
    private static readonly ConcurrentDictionary<Type, List<Type>> _types = new();

    public static Type[] GetTypesWithAttribute(Type attribute)
    {
        // Have assemblies already been scanned for this attribute.
        if (!_types.ContainsKey(attribute))
        {
            // no, create a new entry in the dictionary mapping
            // the attribute to the list of types that have that attribute.
            List<Type> result = new List<Type>();

            // for each assembly available
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // for each type in the assembly
                    foreach (Type type in assembly.GetTypes())
                    {
                        // if the type has the attribute
                        if (type.GetCustomAttributes(attribute, true).Length > 0)
                        {
                            // then add it to the list of types for the attribute
                            result.Add(type);
                        }
                    }
                }
                catch (Exception)
                {
                    // ignore; unable to load; assembly may be obfuscated, etc. 
                }
            }

            // store a record mapping the attribute to the list of types with that attribute
            _types[attribute] = result;
        }

        // return the list of types with the attribute
        return _types[attribute].ToArray();
    }
}
}