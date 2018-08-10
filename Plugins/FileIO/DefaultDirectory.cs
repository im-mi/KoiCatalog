using System;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class DefaultDirectory
    {
        public string Name { get; }
        public Uri Location { get; }
        public string GroupName { get; }

        public DefaultDirectory(string name, Uri location, string groupName)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (location == null) throw new ArgumentNullException(nameof(location));
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            Name = name;
            Location = location;
            GroupName = groupName;
        }
    }
}
