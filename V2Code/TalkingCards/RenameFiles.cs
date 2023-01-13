﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

namespace JSONLoader.Data.TalkingCards
{
    internal static class RenameFiles
    {
        internal static List<string> FindJSON()
            => Directory.GetFiles(Paths.PluginPath, "*_talk.json", SearchOption.AllDirectories)
            .ToList();

        internal static void RenameAll()
            => FindJSON().ForEach(Rename);

        internal static void Rename(string fileName)
        {
            if (!fileName.EndsWith("_talk.json")) return;
            string noExtension = fileName.Substring(0, fileName.Length - ".json".Length);
            string newFileName = $"{noExtension}.jldr2";
            if (!File.Exists(newFileName)) File.Move(fileName, newFileName);
        }
    }
}