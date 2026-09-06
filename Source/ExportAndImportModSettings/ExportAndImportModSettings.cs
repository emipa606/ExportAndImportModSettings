using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ExportAndImportModSettings;

[StaticConstructorOnStartup]
public static class ExportAndImportModSettings
{
    static ExportAndImportModSettings()
    {
        new Harmony("Mlie.ExportAndImportModSettings").PatchAll(Assembly.GetExecutingAssembly());
    }

    public static string GetDefaultSaveLocation()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Desktop).NullOrEmpty()
            ? GenFilePaths.SaveDataFolderPath
            : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }
}