using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using KoiCatalog.IO;
using KoiCatalog.Plugins.FileIO;
using Microsoft.Win32;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class KoikatuDefaultDirectoryHandler : DefaultDirectoryHandler
    {
        public static bool TryGetFullVersionInstallDirectory(out string result) =>
            TryGetInstallDirectoryInternal(out result, "koikatu");

        public static bool TryGetTrialVersionInstallDirectory(out string result) =>
            TryGetInstallDirectoryInternal(out result, "KoikatuTrial");

        private static bool TryGetInstallDirectoryInternal(out string result, string registryKey)
        {
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
            try
            {
                result = Registry.CurrentUser.OpenSubKey($@"Software\illusion\Koikatu\{registryKey}", false)
                    ?.GetValue("INSTALLDIR") as string;
                if (result != null) return true;
            }
            catch (Exception ex)
            {
                if (!(ex is SecurityException || ex is ObjectDisposedException))
                    throw;
            }
            result = null;
            return false;
        }

        private static List<DefaultDirectory> GetDefaultDirectories(string installDirectory, string groupName)
        {
            if (installDirectory == null) throw new ArgumentNullException(nameof(installDirectory));
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));

            var userDataDirectory = Path.Combine(installDirectory, KoikatuConstants.UserDataDirectory);

            var directories = new List<DefaultDirectory>
            {
                new DefaultDirectory(
                    "female",
                    new Uri(IOUtil.NormalizePath(Path.Combine(userDataDirectory, ChaFileDefine.CharaFileFemaleDir))),
                    groupName),
                new DefaultDirectory(
                    "male",
                    new Uri(IOUtil.NormalizePath(Path.Combine(userDataDirectory, ChaFileDefine.CharaFileMaleDir))),
                    groupName)
            };

            return directories;
        }

        public override IEnumerable<DefaultDirectory> GetDirectories()
        {
            var results = new List<DefaultDirectory>();

            if (TryGetFullVersionInstallDirectory(out var directory))
                results.AddRange(GetDefaultDirectories(directory, "Koikatu"));

            if (TryGetTrialVersionInstallDirectory(out var trialDirectory))
                results.AddRange(GetDefaultDirectories(trialDirectory, "Koikatu (Trial)"));

            return results;
        }
    }
}
