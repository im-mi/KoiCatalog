using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PalettePal
{
    [XmlRoot("PalettePal")]
    public sealed class Palette : IXmlSerializable
    {
        public List<Adjective> Adjectives { get; } = new List<Adjective>();

        public IEnumerable<Adjective> GetAdjectives(Color color)
        {
            return Adjectives.Where(i => i.Contains(color)).ToList();
        }

        public static Palette Load(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            var reader = new XmlSerializer(typeof(Palette));
            return (Palette)reader.Deserialize(stream);
        }

        public static Palette Load(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            using (var stream = File.OpenRead(path))
            {
                return Load(stream);
            }
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.ForEachChildElement(child =>
            {
                if (child.Name == nameof(Adjectives))
                {
                    Adjectives.Clear();

                    child.ForEachChildElement(adjectiveElement =>
                    {
                        if (adjectiveElement.Name == nameof(Adjective))
                        {
                            var adjective = new Adjective();
                            adjective.ReadXml(adjectiveElement);
                            Adjectives.Add(adjective);
                        }
                    });
                }
            });
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Adjectives.Count > 0)
                writer.WriteElement(nameof(Adjectives), Adjectives);
        }
    }
}
