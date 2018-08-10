using System;
using System.Xml;
using System.Xml.Serialization;

namespace PalettePal
{
    static class XmlUtil
    {
        public static void ForEachChildElement(this XmlReader reader, Action<XmlReader> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            using (var subtree = reader.ReadSubtree())
            {
                subtree.Read();

                while (subtree.Read())
                {
                    if (subtree.NodeType != XmlNodeType.Element)
                    {
                        subtree.Skip();
                        continue;
                    }

                    using (var subtree2 = subtree.ReadSubtree())
                    {
                        subtree2.Read();

                        if (subtree2.NodeType != XmlNodeType.Element)
                        {
                            subtree2.Skip();
                            continue;
                        }

                        using (var subtree3 = subtree2.ReadSubtree())
                        {
                            subtree3.Read();
                            action.Invoke(subtree3);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes an object as an xml element.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="localName"></param>
        /// <param name="value"></param>
        public static void WriteElement(this XmlWriter writer, string localName, object value)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var root = new XmlRootAttribute(localName);
            var s = new XmlSerializer(value.GetType(), root);
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            s.Serialize(writer, value, ns);
        }

        /// <summary>
        /// Gets the specified attribute converted to the specified type.
        /// If the attribute cannot be converted to the specified type, then an exception is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetTypedAttribute<T>(this XmlReader reader, string name, T defaultValue = default(T))
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var rawValue = reader.GetAttribute(name);
            if (rawValue == null) return defaultValue;

            if (typeof(T) == typeof(Color))
                return (T)(object)Color.Parse(rawValue);
            else if (typeof(T) == typeof(Version))
                return (T)(object)new Version(rawValue);
            else
                return (T)Convert.ChangeType(rawValue, typeof(T));
        }
    }
}
