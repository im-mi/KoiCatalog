using System;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PalettePal
{
    public sealed class Adjective : IComparable, IXmlSerializable
    {
        public Adjective()
        {
            Samples.CollectionChanged += Samples_CollectionChanged;
        }

        private void Samples_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }

        private bool Invalidated { get; set; }

        private void Invalidate()
        {
            Invalidated = true;
        }

        private void EnsureValidated()
        {
            if (!Invalidated) return;

            IncidenceMatrix.Clear();
            foreach (var sample in Samples)
            {
                var point = new BinaryPoint(ColorToPoint(sample.Color), sample.Polarity);
                IncidenceMatrix.Add(point);
            }

            Invalidated = false;
        }

        public ColorSampleCollection Samples { get; } =
            new ColorSampleCollection();

        private PointCloudBinaryMatrix IncidenceMatrix { get; } =
            new PointCloudBinaryMatrix();

        private static Vector3i ColorToPoint(Color color)
        {
            var rgb = ColorConversion.ColorToRgb(color);
            var xyz = ColorConversion.RgbToXyz(rgb);
            var lab = ColorConversion.XyzToLab(xyz);
            return lab;
        }

        public bool Contains(Color color)
        {
            EnsureValidated();
            return IncidenceMatrix[ColorToPoint(color)];
        }
        
        public string Name { get; set; }
        public Color DisplayColor { get; set; }
        public int DisplayIndex { get; set; }

        public override string ToString() => Name ?? string.Empty;

        public int CompareTo(object obj)
        {
            if (obj is Adjective)
            {
                var otherDisplayIndex = ((Adjective)obj).DisplayIndex;
                return DisplayIndex.CompareTo(otherDisplayIndex);
            }
            else
            {
                return 0;
            }
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute(nameof(Name), Name);
            DisplayColor = reader.GetTypedAttribute(nameof(DisplayColor), DisplayColor);
            DisplayIndex = reader.GetTypedAttribute(nameof(DisplayIndex), DisplayIndex);

            Samples.Clear();
            var rawSamples = reader.GetTypedAttribute(nameof(Samples), string.Empty);
            foreach (var sample in ColorSampleCollection.Parse(rawSamples))
                Samples.Add(sample);
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Name != default(string))
                writer.WriteAttributeString(nameof(Name), Name);
            if (DisplayColor != default(Color))
                writer.WriteAttributeString(nameof(DisplayColor), DisplayColor.ToString());
            if (DisplayIndex != default(int))
                writer.WriteAttributeString(nameof(DisplayIndex), DisplayIndex.ToString());
            if (Samples.Count > 0)
                writer.WriteAttributeString(nameof(Samples), Samples.ToString());
        }
    }
}
