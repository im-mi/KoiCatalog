namespace KoiCatalog.Data
{
    public sealed class Progress
    {
        public int Value { get; }
        public int Maximum { get; }

        public Progress(int value, int maximum)
        {
            Value = value;
            Maximum = maximum;
        }
    }
}
