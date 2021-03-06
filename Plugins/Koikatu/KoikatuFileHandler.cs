﻿using System;
using System.IO;
using KoiCatalog.Plugins.FileIO;
using FileInfo = KoiCatalog.Plugins.FileIO.FileInfo;

namespace KoiCatalog.Plugins.Koikatu
{
    [FileHandlerDependency(typeof(FileInfoFileHandler))]
    public sealed class KoikatuFileHandler : FileHandler
    {
        public override void HandleFile(FileLoader loader)
        {
            if (!Path.GetExtension(loader.Source.AbsolutePath)
                .Equals(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var fileInfo = loader.Entity.GetComponent(FileInfo.TypeCode);

            ChaFile chaFile;
            try
            {
                using (var stream = fileInfo.OpenRead())
                {
                    chaFile = new ChaFile(stream);
                }
            }
            catch (Exception)
            {
                return;
            }

            var card = loader.Entity.AddComponent(KoikatuCharacterCard.TypeCode);
            card.Initialize(chaFile);
        }
    }
}
