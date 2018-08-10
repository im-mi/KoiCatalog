using MessagePack;
using MessagePack.Resolvers;
using PalettePal;

namespace KoiCatalog.Plugins.Koikatu
{
    public static class KoikatuConstants
    {
        public const string UserDataDirectory = "UserData";

        public static Size CardImageSize { get; } = new Size(252, 352);

        static KoikatuConstants()
        {
            // Todo: Use an instance instead of a static resolver.
            CompositeResolver.Register(
                MessagePack.Unity.UnityResolver.Instance,
                StandardResolver.Instance
            );
            MessagePackResolver = CompositeResolver.Instance;
        }
        
        internal static IFormatterResolver MessagePackResolver { get; }
    }
}
