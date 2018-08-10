namespace PalettePal
{
    struct BinaryPoint
    {
        public Vector3i Location { get; set; }
        public bool Polarity { get; set; }

        public BinaryPoint(Vector3i location, bool polarity)
        {
            Location = location;
            Polarity = polarity;
        }
    }
}
